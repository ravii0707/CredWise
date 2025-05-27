# CredWise Admin Module - Implementation Guide

## Overview

The CredWise Admin module is a .NET Core-based application that manages loan and FD product configurations, application processing, user management, and email notifications. The application follows clean architecture principles and implements best practices for enterprise applications.

## Project Structure

The solution is organized into the following projects:

1. **CredWiseAdmin.API**

   - Controllers for handling HTTP requests
   - API configuration and middleware
   - Dependency injection setup
   - API documentation (Swagger)
   - User management endpoints
   - Email notification endpoints

2. **CredWiseAdmin.Core**

   - Domain entities
   - DTOs (Data Transfer Objects)
   - Interfaces for repositories and services
   - Enums and constants
   - User-related models
   - Email-related models

3. **CredWiseAdmin.Services**

   - Business logic implementation
   - Service interfaces
   - AutoMapper profiles
   - Validation logic
   - Email notification system
   - Email templates
   - User management services

4. **CredWiseAdmin.Repository**
   - Database context
   - Repository implementations
   - Entity configurations
   - Database migrations
   - User data access

## Key Features

1. **User Management**

   - User registration and authentication
   - Role-based access control (Admin/Customer)
   - User profile management
   - Password management
   - User status tracking

2. **Email Notification System**

   - User registration notifications
   - Loan approval notifications
   - Loan rejection notifications
   - Payment confirmation notifications
   - HTML email templates with responsive design
   - Secure email handling

3. **Loan Type Management**

   - Create, read, update, and delete loan types
   - Configure loan parameters (amount, interest rate, term)
   - Track loan type status and history

4. **FD Type Management**

   - Manage fixed deposit product configurations
   - Set interest rates and terms
   - Track FD type status

5. **Application Processing**

   - Handle loan applications
   - Track application status
   - Process application workflows

6. **Integration**
   - Authentication and authorization
   - External system integration
   - Logging and monitoring

## Implementation Details

### 1. Core Layer (CredWiseAdmin.Core)

The core layer contains the domain entities and interfaces that define the business rules and data structures.

#### Key Entities:

- `User`: Represents a system user
- `LoanType`: Represents a loan product configuration
- `FDType`: Represents a fixed deposit product configuration
- `LoanApplication`: Represents a loan application

#### Key Interfaces:

- `IUserRepository`: Repository interface for user management
- `IUserService`: Service interface for user operations
- `ILoanTypeRepository`: Repository interface for loan types
- `ILoanTypeService`: Service interface for loan type operations
- `IUnitOfWork`: Unit of work pattern interface
- `IEmailService`: Service interface for email notifications

### 2. Repository Layer (CredWiseAdmin.Repository)

The repository layer handles data access and persistence.

#### Key Components:

- `ApplicationDbContext`: Entity Framework Core context
- `GenericRepository<T>`: Base repository implementation
- `UnitOfWork`: Manages transactions and repository instances
- `UserRepository`: Handles user data access

### 3. Service Layer (CredWiseAdmin.Services)

The service layer implements business logic and orchestrates operations.

#### Key Services:

- `UserService`: Manages user operations
- `LoanTypeService`: Manages loan type operations
- `FDTypeService`: Manages FD type operations
- `LoanApplicationService`: Handles loan application processing
- `EmailService`: Handles email notifications

#### User Management:

- User registration with validation
- Password hashing using BCrypt
- Role-based authorization
- User status management
- Profile updates

#### Email Notification System:

The email notification system is implemented in the `EmailService` class and includes:

1. **User Registration Email**

   - Sends welcome email with login credentials
   - Includes login URL and password
   - Styled with professional HTML template
   - Security recommendations

2. **Loan Approval Email**

   - Notifies users of approved loan applications
   - Includes loan application ID
   - Provides next steps and support contact
   - Clear call-to-action buttons

3. **Loan Rejection Email**

   - Notifies users of rejected loan applications
   - Includes rejection reason
   - Provides support contact information
   - Offers guidance for future applications
   - Professional and empathetic tone

4. **Payment Confirmation Email**
   - Confirms successful payment
   - Includes transaction ID and date
   - Provides transaction details
   - Includes record-keeping reminder
   - Clear transaction summary

#### Email Templates:

All email templates are stored in the `EmailTemplates` directory and include:

- Responsive HTML design
- Professional styling
- Clear typography
- Color-coded status indicators
- Security notices
- Footer with important information
- Mobile-friendly layout
- Accessible design

### 4. API Layer (CredWiseAdmin.API)

The API layer exposes endpoints for client applications.

#### Key Components:

- `UsersController`: Handles user management operations
- `LoanTypesController`: Handles loan type operations
- `ErrorHandlingMiddleware`: Global error handling
- `ServiceCollectionExtensions`: Dependency injection setup
- `EmailsController`: Handles email-related operations

#### User Management Endpoints:

- POST /api/admin/users/register
- POST /api/admin/users/login
- GET /api/admin/users/{id}
- PUT /api/admin/users/{id}
- PUT /api/admin/users/{id}/deactivate

#### Email Testing Endpoints:

- POST /api/admin/users/test-send-credentials
- POST /api/admin/emails/send-registration
- POST /api/admin/emails/send-loan-approval
- POST /api/admin/emails/send-loan-rejection
- POST /api/admin/emails/send-payment-confirmation

## Best Practices

1. **Error Handling**

   - Global exception handling middleware
   - Custom exception types
   - Structured error responses
   - Detailed error logging

2. **Validation**

   - FluentValidation for DTOs
   - Custom validation attributes
   - Service-level validation
   - Input sanitization

3. **Logging**

   - Structured logging with Serilog
   - Correlation IDs for request tracking
   - Appropriate log levels
   - Audit logging for user actions

4. **Security**

   - JWT-based authentication
   - Role-based authorization
   - Secure configuration management
   - Secure email handling
   - Password hashing with BCrypt
   - Input validation and sanitization

5. **Performance**

   - Async/await throughout
   - Caching implementation
   - Optimized database queries
   - Non-blocking email sending
   - Efficient user data access

6. **Testing**
   - Unit tests for services
   - Integration tests for repositories
   - API functional tests
   - Email template testing

## Configuration

### Database

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CredWiseAdmin;Trusted_Connection=True;"
  }
}
```

### Email Configuration

```json
{
  "Email": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "noreply@credwise.com",
    "Password": "your-smtp-password",
    "From": "noreply@credwise.com",
    "DisplayName": "CredWise Admin"
  },
  "AppSettings": {
    "LoginUrl": "https://app.credwise.com/login",
    "SupportEmail": "support@credwise.com",
    "SupportPhone": "+1 (800) 123-4567"
  }
}
```

### Authentication

```json
{
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "credwise.com",
    "Audience": "credwise-users",
    "ExpiryInMinutes": 60
  }
}
```

### Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Development Guidelines

1. **Code Organization**

   - Follow clean architecture principles
   - Use dependency injection
   - Implement proper separation of concerns
   - Maintain consistent file structure

2. **Naming Conventions**

   - Use PascalCase for public members
   - Use camelCase for private members
   - Use meaningful names
   - Follow C# naming conventions

3. **Documentation**

   - XML comments for public APIs
   - README files for each project
   - API documentation with Swagger
   - Keep email templates well-documented
   - Maintain clear configuration documentation

4. **Email Template Guidelines**

   - Use responsive design
   - Include clear call-to-actions
   - Maintain consistent branding
   - Ensure accessibility
   - Test across email clients
   - Follow email best practices

5. **Version Control**
   - Use feature branches
   - Write meaningful commit messages
   - Review code before merging
   - Keep commits focused and atomic

## Security Considerations

### Authentication

- JWT-based authentication
- Secure password storage with BCrypt
- Token refresh mechanism
- Session management

### Authorization

- Role-based access control
- Resource-level permissions
- API endpoint security
- User role validation

### Data Protection

- Encrypted sensitive data
- Secure configuration storage
- Audit logging
- Data backup encryption
- Email security

## Next Steps

1. **Planned Features**

   - Enhanced user profile management
   - Additional email templates
   - Advanced reporting
   - Audit trail system

2. **Improvements**

   - Performance optimization
   - Enhanced error handling
   - Additional security measures
   - Extended testing coverage

3. **Documentation**
   - API documentation updates
   - User guides
   - Deployment guides
   - Troubleshooting guides

## Deployment

### Prerequisites

- .NET 6.0 SDK
- SQL Server 2019 or later
- Required third-party DLLs in the Libs folder

### Build Process

1. Run database migrations:
   ```bash
   dotnet ef database update
   ```
2. Build the solution:
   ```bash
   dotnet build
   ```
3. Run tests:
   ```bash
   dotnet test
   ```

### Deployment Steps

1. Deploy database migrations
2. Configure environment variables
3. Deploy application files
4. Configure IIS or other hosting environment
5. Set up SSL certificates
6. Configure logging and monitoring

## Monitoring and Maintenance

### Health Checks

- Database connectivity
- External service dependencies
- Application status
- Memory usage
- CPU utilization

### Performance Monitoring

- Response times
- Resource usage
- Error rates
- Database query performance
- API endpoint metrics

### Backup and Recovery

- Daily database backups
- Configuration backups
- Log file rotation
- Disaster recovery plan
- Backup verification process

## Support and Troubleshooting

### Common Issues

1. Database Connection Problems

   - Check connection string
   - Verify SQL Server is running
   - Check network connectivity

2. Authentication Failures

   - Verify API keys
   - Check token expiration
   - Validate user permissions

3. Performance Issues
   - Monitor database queries
   - Check resource utilization
   - Review application logs

### Logging and Diagnostics

- Application logs in `./logs` directory
- Database logs in SQL Server
- System event logs
- IIS logs (if applicable)

### Contact Information

- Development Team: dev@credwise.com
- Database Administrators: dba@credwise.com
- System Administrators: sysadmin@credwise.com

## API Documentation

### Swagger UI

Access the API documentation at `/swagger` endpoint when running the application.

### Key Endpoints

1. Loan Types

   - GET /api/loantypes
   - POST /api/loantypes
   - PUT /api/loantypes/{id}
   - DELETE /api/loantypes/{id}

2. FD Types

   - GET /api/fdtypes
   - POST /api/fdtypes
   - PUT /api/fdtypes/{id}
   - DELETE /api/fdtypes/{id}

3. Applications
   - GET /api/applications
   - POST /api/applications
   - PUT /api/applications/{id}
   - GET /api/applications/{id}/status

## Security Considerations

### Authentication

- JWT-based authentication
- Token refresh mechanism
- Secure password policies

### Authorization

- Role-based access control
- Resource-level permissions
- API endpoint security

### Data Protection

- Encrypted sensitive data
- Secure configuration storage
- Audit logging
- Data backup encryption
