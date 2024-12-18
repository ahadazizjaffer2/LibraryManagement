# Library Management System API

## Overview
This is a backend API for a **Library Management System** built with **.NET Core**. The API allows users to manage books, borrow and return books, and manage users with different roles such as Admin, Librarian, and Customer.

## Features
- **JWT Authentication**: Secure API access using JWT tokens.
- **Role-based Access Control**: Admin, Librarian, and Customer roles with different permissions.
- **Books Management**: Create, update, delete, and view books.
- **Borrowing Books**: Users can borrow and return books.
- **User Management**: Admin can manage users and roles.

## Technologies Used
- **.NET 8 Core**: Backend framework.
- **PostgreSQL**: Database for storing data.
- **JWT**: JSON Web Tokens for authentication.
- **Swagger**: API documentation and testing interface.

### Install Dependencies
Navigate to the project directory and restore the NuGet packages:
```bash
cd LibraryManagementSystem
dotnet restore
```

### Database Configuration
1. Create a database in any PostgreSQL (e.g., `LibraryDB`).
2. Update the `appsettings.json` file to match your database connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your-server;Database=LibraryDB;User Id=your-user;Password=your-password;"
}
```

### Run the API
To run the API locally, use the following command:

```bash
dotnet run
```

The API will be available at `https://localhost:7098`.

## Testing the API

You can test the API using **Postman** or **Swagger UI**:

- The Swagger UI is available at: `https://localhost:7098/swagger`.

## Contact
For any questions, suggestions, or issues, feel free to reach out via email: [ahadaziz4@gmail.com].
