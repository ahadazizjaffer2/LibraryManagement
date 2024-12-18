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

## API Endpoints

### Authentication

#### Sign Up (Customer or Admin)
**POST** `/api/auth/signup`
Registers a new user (Customer or Admin).

#### Login
**POST** `/api/auth/login`
Allows a user to log in and receive a JWT token.

### Books

#### Get All Books
**GET** `/api/books`
Fetches all books in the library.

#### Get Book by ID
**GET** `/api/books/{id}`
Fetches a single book by its ID.

#### Add Book
**POST** `/api/books`
Allows a librarian to add a new book to the library.

#### Update Book
**PUT** `/api/books/{id}`
Updates the details of an existing book.

#### Delete Book
**DELETE** `/api/books/{id}`
Deletes a book from the library.

### Borrowed Books

#### Get Borrowed Books by User
**GET** `/api/borrowed-books/user/{userId}`
Fetches all borrowed books by a specific user.

#### Borrow Book
**POST** `/api/borrowed-books`
Allows a customer to borrow a book.

#### Return Book
**PUT** `/api/borrowed-books/return`
Allows a customer to return a borrowed book.

### Users

#### Get All Users (Admin Only)
**GET** `/api/users`
Fetches a list of all users in the system.

#### Delete User (Admin Only)
**DELETE** `/api/users/{id}`
Allows an admin to delete a user from the system.

## Testing the API

You can test the API using **Postman** or **Swagger UI**:

- The Swagger UI is available at: `https://localhost:7098/swagger`.

## Contact
For any questions, suggestions, or issues, feel free to reach out via email: [ahadaziz4@gmail.com].
