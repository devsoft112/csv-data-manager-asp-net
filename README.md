# CSV Data Manager

CSV Data Manager is an ASP.NET Core MVC application that allows you to manage user data from CSV files. It supports CRUD operations (Create, Read, Update, Delete) and provides functionality to upload and process CSV files to populate the database.

## Features

- Upload and process CSV files to add users to the database.
- Display paginated lists of users.
- Create, read, update, and delete user information.
- Validation for user data.

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- CSVHelper
- X.PagedList
- Microsoft SQL Server

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- A code editor like [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/deliteser112/csv-data-manager.git
   cd csv-data-manager
2. Restore dependencies:
   ```bash
   dotnet restore

3. Update the database connection string in appsettings.json:
   ```bash
   {
      "ConnectionStrings": {
         "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CSVDataManager;Trusted_Connection=True;MultipleActiveResultSets=true"
      }
   }

4. Apply the database migrations:
   ```bash
   dotnet ef database update
5. Run the application:
   ```bash
      dotnet run
6. Open your browser and navigate to https://localhost:5001 to see the application running.

## Usage
- Upload CSV: Use the upload form to select and upload a CSV file containing user data.
- View Users: Navigate to the users page to view a paginated list of users.
- Create User: Use the create form to add a new user.
- Edit User: Edit existing user information.
- Delete User: Delete a user from the database.

## Project Structure
   ```bash
   CSVDataManager
   │   README.md
   │   .gitignore
   │   CSVDataManager.csproj
   │
   ├───Controllers
   │       UsersController.cs
   │
   ├───Data
   │       ApplicationDbContext.cs
   │
   ├───Models
   │       User.cs
   │
   ├───Repositories
   │       IUserRepository.cs
   │       UserRepository.cs
   │
   ├───Services
   │       IUserService.cs
   │       UserService.cs
   │
   ├───ViewModels
   │       UserViewModel.cs
   │
   ├───Views
   │   ├───Shared
   │   │       _Layout.cshtml
   │   │       Error.cshtml
   │   │
   │   ├───Users
   │   │       Create.cshtml
   │   │       Delete.cshtml
   │   │       Edit.cshtml
   │   │       Index.cshtml
   │
   └───wwwroot
      ├───css
      │       site.css
      ├───images
      │       empty-folder.png
      ├───js
               site.js
