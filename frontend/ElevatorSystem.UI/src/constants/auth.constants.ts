export const AUTH_CONSTANTS = {
  STORAGE_KEYS: {
    TOKEN: "auth_token",
    USER: "auth_user",
  },

  API_ENDPOINTS: {
    LOGIN: "/api/users/login",
    REGISTER: "/api/users/register",
    GET_USER: "/api/users",
  },

  ERROR_MESSAGES: {
    INVALID_CREDENTIALS: "Incorrect username or password",
    NETWORK_ERROR: "Network error occurred",
    REGISTRATION_FAILED: "Registration failed",
    TOKEN_EXPIRED: "Token has expired",
    UNAUTHORIZED: "Unauthorized access",
  },
} as const;
