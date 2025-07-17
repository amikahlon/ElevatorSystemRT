using AutoMapper;
using ElevatorSystem.API.Models.DTOs.Users;
using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Repositories.Interfaces;
using ElevatorSystem.API.Services.Interfaces;
using ElevatorSystem.API.Common.Exceptions;
using ElevatorSystem.API.Common.Constants;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace ElevatorSystem.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        // Register a new user - check if email exists, hash password, save to DB
        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Starting user registration for email: {Email}", dto.Email);

            ValidateRegistrationData(dto);

            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", dto.Email);
                throw new BusinessException(ValidationConstants.ErrorMessages.EmailAlreadyExists);
            }

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = HashPassword(dto.Password);

            try
            {
                var createdUser = await _userRepository.AddAsync(user);
                _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);
                return _mapper.Map<UserDto>(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for email: {Email}", dto.Email);
                throw new BusinessException(ValidationConstants.ErrorMessages.RegistrationFailed);
            }
        }

        // Login user - validate password and return token
        public async Task<(UserDto? User, string? Token)> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            ValidateLoginData(dto);

            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid credentials - {Email}", dto.Email);
                    return (null, null);
                }

                var userDto = _mapper.Map<UserDto>(user);
                var token = _jwtService.GenerateToken(user);

                _logger.LogInformation("User logged in successfully: {UserId}", user.Id);
                return (userDto, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", dto.Email);
                throw new BusinessException(ValidationConstants.ErrorMessages.LoginFailed);
            }
        }

        // Make sure all the input data for register is valid
        private void ValidateRegistrationData(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException(ValidationConstants.ErrorMessages.NameRequired);

            if (dto.Name.Length < ValidationConstants.User.NameMinLength || dto.Name.Length > ValidationConstants.User.NameMaxLength)
                throw new ValidationException($"Name must be between {ValidationConstants.User.NameMinLength} and {ValidationConstants.User.NameMaxLength} characters");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException(ValidationConstants.ErrorMessages.EmailRequired);

            if (!IsValidEmail(dto.Email))
                throw new ValidationException(ValidationConstants.ErrorMessages.InvalidEmailFormat);

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException(ValidationConstants.ErrorMessages.PasswordRequired);

            if (dto.Password.Length < ValidationConstants.User.PasswordMinLength)
                throw new ValidationException($"Password must be at least {ValidationConstants.User.PasswordMinLength} characters long");
        }

        // Simple checks for login data
        private void ValidateLoginData(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException(ValidationConstants.ErrorMessages.EmailRequired);

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException(ValidationConstants.ErrorMessages.PasswordRequired);
        }

        // Basic email format check using regex
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, ValidationConstants.User.EmailRegex);
        }

        // Hash the password using BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        // Check if password matches the hash
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        // Get a user by their ID from DB
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
    }
}
