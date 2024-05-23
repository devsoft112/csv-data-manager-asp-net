using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CSVDataManager.Models;
using CSVDataManager.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace CSVDataManager.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IPagedList<User>> GetUsersAsync(int pageNumber, int pageSize)
        {
            return await _userRepository.GetUsersAsync(pageNumber, pageSize);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public bool UserExists(int id)
        {
            return _userRepository.UserExists(id);
        }

        public async Task ProcessCsvFileAsync(string filePath)
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

                    if (validUsers.Count > 0)
                    {
                        foreach (var user in validUsers)
                        {
                            await _userRepository.AddUserAsync(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to process CSV file: {Message}", ex.Message);
            }
        }
    }
}
