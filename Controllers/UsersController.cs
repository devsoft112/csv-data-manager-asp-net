using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSVDataManager.Models;
using CSVDataManager.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSVDataManager.ViewModels;
using System.Threading.Tasks;

namespace CSVDataManager.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Display all users (Read all)
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // Number of items per page
            int pageNumber = (page ?? 1);

            var users = await _userService.GetUsersAsync(pageNumber, pageSize);

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View("Error", new ErrorViewModel { RequestId = "File is empty" });
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // After saving the file, process the CSV data
            await _userService.ProcessCsvFileAsync(path);

            return RedirectToAction("Index");
        }

        // GET: Users/Create
        // Display the form to create a new user (Create)
        public IActionResult Create()
        {
            var model = new UserViewModel
            {
                SexOptions = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "M" },
                    new SelectListItem { Text = "Female", Value = "F" }
                }
            };
            return View(model);
        }

        // POST: Create a new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            ModelState.Remove("SexOptions");
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        _logger.LogError($"Validation errors for {entry.Key}:");
                        foreach (var error in entry.Value.Errors)
                        {
                            _logger.LogError(error.ErrorMessage);
                        }
                    }
                }

                // Reinitialize the SexOptions to ensure they're available for the view on return
                model.SexOptions = GetSexOptions();
                return View(model);
            }

            // If model state is valid, proceed to map to database entity and save
            var user = new User
            {
                Firstname = model.Firstname,
                Surname = model.Surname,
                Age = model.Age,
                Sex = model.Sex,
                Mobile = model.Mobile,
                Active = model.Active
            };

            await _userService.AddUserAsync(user);
            return RedirectToAction("Index");
        }

        // Display the form to edit a user (Update)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                Age = user.Age,
                Sex = user.Sex,
                Mobile = user.Mobile,
                Active = user.Active,
                SexOptions = GetSexOptions()
            };

            return View(model);
        }

        // POST: Update a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Firstname, Surname, Age, Sex, Mobile, Active")] UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            ModelState.Remove("SexOptions");

            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Firstname = model.Firstname;
                user.Surname = model.Surname;
                user.Age = model.Age;
                user.Sex = model.Sex;
                user.Mobile = model.Mobile;
                user.Active = model.Active;

                try
                {
                    await _userService.UpdateUserAsync(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_userService.UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            model.SexOptions = GetSexOptions(); // Reinitialize on validation failure
            return View(model);
        }

        // Display the confirmation page for deleting a user (Delete)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Confirm delete action
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> GetSexOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text = "Male" },
                new SelectListItem { Value = "F", Text = "Female" }
            };
        }
    }
}
