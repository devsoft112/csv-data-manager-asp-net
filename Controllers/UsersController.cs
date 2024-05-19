using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSVDataManager.Data;
using CSVDataManager.Models;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

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
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = context =>
                    {
                        Console.WriteLine($"Bad data found on row {context.Context.Parser.Row}: {context.RawRecord}");
                    },
                    ReadingExceptionOccurred = exception =>
                    {
                        Console.WriteLine($"Reading exception occurred: {exception.Exception.Message}");
                        return false; // return true if you want to ignore the error and continue processing.
                    }
                }))
                {
                    var records = csv.GetRecords<User>();
                    await _context.Users.AddRangeAsync(records);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error and handle it, e.g., return a view with an error message
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
            return View();
        }

        // POST: Create a new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Firstname, Surname, Age, Sex, Mobile, Active")] User user)
        {
            if (ModelState.IsValid) {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(user);
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

            return View(user);
        }

        // POST: Update a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Firstname, Surname, Age, Sex, Mobile, Active")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound(user);
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
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
    }
}
