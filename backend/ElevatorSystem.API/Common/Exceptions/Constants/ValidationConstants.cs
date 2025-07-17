namespace ElevatorSystem.API.Common.Constants
{
    public static class ValidationConstants
    {
        public static class User
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 100;
            public const int EmailMaxLength = 255;
            public const int PasswordMinLength = 6;
            public const int PasswordHashMaxLength = 500;
            
            public const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        }

        public static class ErrorMessages
        {
            public const string NameRequired = "Name is required";
            public const string EmailRequired = "Email is required";
            public const string PasswordRequired = "Password is required";
            public const string InvalidEmailFormat = "Invalid email format";
            public const string EmailAlreadyExists = "Email already registered";
            public const string InvalidCredentials = "Invalid credentials";
            public const string RegistrationFailed = "Registration failed. Please try again.";
            public const string LoginFailed = "Login failed. Please try again.";
        }
    }
}