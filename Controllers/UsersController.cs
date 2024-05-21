using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSVDataManager.Data;
using CSVDataManager.Models;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSVDataManager.ViewModels;

namespace CSVDataManager.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public UsersController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Display all users (Read all)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        private async Task ProcessCsvFile(string filePath)
        {
            var validUsers = new List<User>();

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = context =>
                    {
                        _logger.LogInformation($"Bad data found on row {context.Context.Parser.Row}: {context.RawRecord}");
                    },
                    HeaderValidated = null,  // Ignore header validation issues
                    MissingFieldFound = null,  // Ignore missing fields
                    ReadingExceptionOccurred = exception =>
                    {
                        _logger.LogInformation($"Reading exception occurred: {exception.Exception.Message}");
                        return false; // return true if you want to ignore the error and continue processing.
                    }
                }))
                {
                    var records = csv.GetRecords<User>();

                    foreach (var record in records)
                    {
                        // Validate each record manually
                        var validationResults = new List<ValidationResult>();
                        var validationContext = new ValidationContext(record);

                        if (Validator.TryValidateObject(record, validationContext, validationResults, true))
                        {
                            validUsers.Add(record);
                        }
                        else
                        {
                            foreach (var error in validationResults)
                            {
                                _logger.LogWarning($"Validation error on row {csv.Context.Parser.Row}: {error.ErrorMessage}");
                            }
                        }
                    }

                    foreach (var user in validUsers)
                    {
                        Console.WriteLine(user.Firstname + "\t" + user.Surname + "\t" + user.Age + "\t" + user.Sex + "\t" + user.Mobile + "\t" + user.Active);
                    }

                    if (validUsers.Count > 0)
                    {
                        await _context.Users.AddRangeAsync(validUsers);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to process CSV file: {Message}", ex.Message);
            }
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

            // After saving the file, precess the CSV data
            await ProcessCsvFile(path);

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

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // Display the form to edit a user (Update)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
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
                SexOptions = GetSexOptions() // Make sure this method is available and properly populating the SelectList
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
                var user = await _context.Users.FindAsync(id);
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
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

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
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
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
