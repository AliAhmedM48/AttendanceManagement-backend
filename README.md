# 📘 Project Documentation: ASP.NET Core Web API

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Database Schema](#database-schema)
3. [Usage Guide](#usage-guide)
4. [Security](#security)
5. [Deployment](#deployment)
6. [Test Accounts for Experimentation](#test-accounts-for-experimentation)
7. [Frontend Repository](#frontend-repository)
8. [Future Enhancements](#future-enhancements)

---

## Architecture Overview

### 1.1 Project Structure

The project follows a layered architecture, promoting separation of concerns and maintainability:

- **Presentation Layer**: Handles HTTP requests and responses.
  - **Controllers**: `AuthController`, `EmployeesController`, `AttendancesController`.

- **Application Layer**: Contains business logic and service interfaces.
  - **Services**: `IEmployeeService`, `IAttendanceService`, `IAuthenticationService`.

- **Infrastructure Layer**: Manages data access and external services.
  - **Repositories**: `IRepository<T>`, `UnitOfWork`.
  - **Data Context**: `AppDbContext` using Entity Framework Core.

### 1.2 Technologies Used

- **Framework**: ASP.NET Core 8.0  
- **Authentication**: JWT Bearer Tokens  
- **Database**: SQL Server  
- **ORM**: Entity Framework Core  
- **API Documentation**: Swagger  
- **Dependency Injection**: Built-in ASP.NET Core DI  
- **Logging**: Built-in Logging Providers

---

## Database Schema

The application uses a relational database with the following primary entities:

### 2.1 Entities

#### Employee

- `Id` (int, Primary Key)  
- `FirstName` (string)  
- `LastName` (string)  
- `Email` (string, Unique)  
- `PasswordHash` (string)  
- `SignaturePath` (string)  
- `CreatedAt` (datetime)  
- `UpdatedAt` (datetime)

#### Attendance

- `Id` (int, Primary Key)  
- `EmployeeId` (int, Foreign Key → Employee.Id)  
- `CheckInTime` (datetime)  
- `CheckOutTime` (datetime, nullable)  
- `CreatedAt` (datetime)  
- `UpdatedAt` (datetime)

### 2.2 Relationships

- **Employee ↔ Attendance**: One-to-Many  
  An employee can have multiple attendance records.

---

## Usage Guide

### 3.1 Authentication

#### Login

- **Endpoint**: `POST /api/auth/login`  
- **Description**: Authenticates a user and returns a JWT token.  
- **Request Body**:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```
- **Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Validate Token

- **Endpoint**: `GET /api/auth/validate-token`  
- **Description**: 
This endpoint is responsible for validating the authenticity and validity of a JWT token provided by the client. It ensures that the token has not expired and is correctly signed, thereby allowing the server to confirm the legitimacy of the user's session. If the token is valid, it returns a success response; otherwise, it triggers an error, signaling that the token is either expired or invalid.
- **Headers**:  
  - `Authorization`: `Bearer {token}`

---

### 3.2 Employees

#### Get All Employees

- **Endpoint**: `GET /api/employees`  
- **Headers**:  
  - `Authorization`: `Bearer {token}`

#### Create New Employee

- **Endpoint**: `POST /api/employees`  
- **Form Data**:
  - `FirstName`: string  
  - `LastName`: string  
  - `Email`: string  
  - `Password`: string  
  - `Signature`: file (image)
- **Headers**:  
  - `Authorization`: `Bearer {token}`

#### Update Employee

- **Endpoint**: `PUT /api/employees/{id}`  
- **Form Data**:
  - `FirstName`: string  
  - `LastName`: string  
  - `Email`: string  
  - `Password`: string  
  - `Signature`: file (image)
- **Headers**:  
  - `Authorization`: `Bearer {token}`

#### Delete Employee

- **Endpoint**: `DELETE /api/employees/{id}`
- **Headers**:  
  - `Authorization`: `Bearer {token}`

#### Get Employee Profile

- **Endpoint**: `GET /api/employees/me/profile`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

#### Get Employee Attendance

- **Endpoint**: `GET /api/employees/me/attendances`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

#### Get Today's Attendance

- **Endpoint**: `GET /api/employees/me/attendance/today`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

#### Update Employee Signature

- **Endpoint**: `PATCH /api/employees/{id}/signature`  
- **Form Data**:
  - `Signature`: file (image)
- **Headers**:  
  - `Authorization`: `Bearer {token}`

#### Get Signature Image

- **Endpoint**: `GET /api/employees/me/signatures/{fileName}`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

---

### 3.3 Attendances

#### Get All Attendances

- **Endpoint**: `GET /api/attendances`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

#### Check-In

- **Endpoint**: `POST /api/attendances/check-in`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

#### Check-Out

- **Endpoint**: `PATCH /api/attendances/check-out/{id}`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

#### Delete Attendance

- **Endpoint**: `DELETE /api/attendances/{id}`  
- **Headers**:
  - `Authorization`: `Bearer {token}`

---

## Security

- **Authentication**: Implemented using JWT Bearer Tokens.  
- **Authorization**: Secured endpoints require a valid JWT token in the `Authorization` header.  
- **Password Storage**: Passwords are hashed using a secure hashing algorithm before storage.  
- **CORS Policy**: Configured to allow requests from specified origins.

---

## Deployment

### 5.1 Prerequisites

- **.NET SDK**: Version 8.0 or higher  
- **Database**: SQL Server instance  
- **Environment Variables**:
  - `ConnectionStrings__DefaultConnection`: Database connection string  
  - `Jwt__Key`: Secret key for JWT token generation  
  - `Jwt__Issuer`: JWT token issuer  
  - `Jwt__Audience`: JWT token audience

### 5.2 Running the Application

1. **Apply Migrations**:
```bash
dotnet ef database update
```

2. **Run the App**:
```bash
dotnet run
```
---

## Test Accounts for Experimentation

The application will seed default accounts for both Admin and Employees upon first run for testing purposes.

### Admin Account:

- **Email**: admin@admin.com  
- **Password**: admin@123  

### Employee Accounts:

1. **Employee 1**:
   - **Email**: ahmed.hassan123@company.com  
   - **Password**: 123456  

2. **Employee 2**:
   - **Email**: sara.ali234@company.com  
   - **Password**: 123456  

3. **Employee 3**:
   - **Email**: mohamed.ali567@company.com  
   - **Password**: 123456  

### How to Access the Accounts:

- **Admin Account**:  
  Use the credentials above to log in as an admin. You can access the admin panel and perform admin-level actions such as managing users and viewing reports.

- **Employee Accounts**:  
  Use the credentials above to log in as employees. These accounts allow you to check attendance records and update personal information.

---
## Frontend Repository

The frontend of the project is available at the following GitHub repository:

[Frontend Repository - Attendance Management](https://github.com/AliAhmedM48/AttendanceManagement-frontend)

---

## 🔮 Future Enhancements

### 1. **Integrating Automapper for Entity-to-DTO Mapping**

   - **What**: Currently, data transformations (e.g., converting `Employee` and `Attendance` entities into Data Transfer Objects (DTOs)) are performed manually. **Automapper** will streamline this process by automating object-to-object mapping, reducing boilerplate code and the risk of errors.

   - **How**: 
     - Instead of writing code manually to map data from one object to another, like converting an `Employee` to `EmployeeDTO`, Automapper will handle this automatically by creating mapping profiles between entities and DTOs.
     - For example, in the `EmployeeService`, instead of manually copying properties from the `Employee` entity to an `EmployeeDTO`, Automapper will perform this transformation with a simple method call.

### 2. **Adding Structured Logging with Serilog**

   - **What**: To enhance error tracking and log management, **Serilog** will be integrated for structured logging, enabling logs to be saved in a more organized format in databases, files, or external services.

   - **How**:
     - Serilog will be configured in the `Startup.cs` or `Program.cs` file to capture key data such as user logins, attendance tracking, and error handling. This will allow the team to monitor and debug the system more effectively.
     - For example, when a user logs in, the application will log this event with the user's ID and status, which can be later queried in case of any issues.

### 3. **Background Task Management with Hangfire**

   - **What**: **Hangfire** will be used to manage background tasks such as sending notifications, cleaning up old data, or generating periodic reports without blocking the main request-response cycle.

   - **How**:
     - For example, instead of manually running a task to clean up old attendance records, Hangfire will automatically run this task in the background at scheduled intervals, ensuring the database is kept clean without affecting performance.

### 4. **Implementing Redis Caching for Employee and Attendance Data**

   - **What**: To improve data retrieval speed and reduce database load, **Redis** will be utilized as an in-memory cache for frequently accessed data such as employee and attendance records.

   - **How**:
     - For instance, when the application needs to retrieve a list of employees, it will first check the cache. If the data is not found, it will fetch the data from the database and store it in Redis for future use, reducing the need to query the database every time.

### 5. **Global Exception Handling System**

   - **What**: A **global exception handling system** will be introduced to handle unhandled exceptions in a centralized manner, ensuring that users receive consistent error messages without exposing sensitive internal details.

   - **How**:
     - A middleware will be implemented to catch exceptions that occur during the execution of requests. This middleware will log the exception details and return a user-friendly error message, ensuring that sensitive internal information is not exposed to end-users.
     - For example, if an error occurs while processing a request, the middleware will log the error and return a message like "An unexpected error occurred" to the user.

---
### Thank You for Using Our Application! 😊

We appreciate your interest in our ASP.NET Core Web API project. If you have any questions or feedback, feel free to reach out. We’re always looking to improve and provide a better experience.

Happy coding! 🚀