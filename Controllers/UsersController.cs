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

        // Constructor to inject the required services
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Display all users (Read all)
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // Number of items per page
            int pageNumber = (page ?? 1); // Set the current page number, default to 1 if null

            // Get paginated list of users
            var users = await _userService.GetUsersAsync(pageNumber, pageSize);

            return View(users); // Pass the user list to the view
        }

        // Upload and process CSV file
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View("Error", new ErrorViewModel { RequestId = "File is empty" });
            }

            // Save the uploaded file to a specified path
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Process the CSV file
            await _userService.ProcessCsvFileAsync(path);

            return RedirectToAction("Index");
        }

        // Display the form to create a new user (Create)
        public IActionResult Create()
        {
            // Initialize the view model with options for the Sex field
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

        // Create a new user (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            ModelState.Remove("SexOptions"); // Remove the SexOptions validation as it is not a field in the database
            if (!ModelState.IsValid)
            {
                // Log validation errors
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

            // Map the view model to a user entity and save it to the database
            var user = new User
            {
                Firstname = model.Firstname,
                Surname = model.Surname,
                Age = model.Age,
                Sex = model.Sex,
                Mobile = model.Mobile,
                Active = model.Active
            };

            await _userService.AddUserAsync(user); // Add the user to the database
            return RedirectToAction("Index"); // Redirect to the index page
        }

        // Display the form to edit a user (Update)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Return not found if the id is null
            }

            var user = await _userService.GetUserByIdAsync(id.Value); // Get the user by id
            if (user == null)
            {
                return NotFound(); // Return not found if the user doesn't exist
            }

            // Initialize the view model with the user data and options for the Sex field
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

            return View(model); // Pass the model to the view
        }

        // Update a user (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Firstname, Surname, Age, Sex, Mobile, Active")] UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound(); // Return not found if the id does not match
            }

            ModelState.Remove("SexOptions"); // Remove the SexOptions validation

            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(id); // Get the user by id
                if (user == null)
                {
                    return NotFound(); // Return not found if the user doesn't exist
                }

                // Update the user entity with the view model data
                user.Firstname = model.Firstname;
                user.Surname = model.Surname;
                user.Age = model.Age;
                user.Sex = model.Sex;
                user.Mobile = model.Mobile;
                user.Active = model.Active;

                try
                {
                    await _userService.UpdateUserAsync(user); // Update the user in the database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_userService.UserExists(user.Id))
                    {
                        return NotFound(); // Return not found if the user no longer exists
                    }
                    else
                    {
                        throw; // Rethrow the exception if there is a concurrency issue
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to the index page
            }

            model.SexOptions = GetSexOptions(); // Reinitialize on validation failure
            return View(model); // Return the view with the model
        }

        // Display the confirmation page for deleting a user (Delete)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Return not found if the id is null
            }

            var user = await _userService.GetUserByIdAsync(id.Value); // Get the user by id
            if (user == null)
            {
                return NotFound(); // Return not found if the user doesn't exist
            }

            return View(user); // Pass the user to the view
        }

        // Confirm delete action (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id); // Delete the user from the database
            return RedirectToAction(nameof(Index)); // Redirect to the index page
        }

        // Helper method to get options for the Sex field
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
