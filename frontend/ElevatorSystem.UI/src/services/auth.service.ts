import api from "./api";
import { StorageUtil } from "../utils/storage.util";
import { AUTH_CONSTANTS } from "../constants/auth.constants";
import type {
  LoginRequest,
  RegisterRequest,
  LoginResponse,
  User,
  AuthError,
  HttpErrorResponse,
} from "../types";

export class AuthService {
  static async login(credentials: LoginRequest): Promise<LoginResponse> {
    try {
      const response = await api.post<LoginResponse>(
        AUTH_CONSTANTS.API_ENDPOINTS.LOGIN,
        credentials
      );

      if (response.data.token && response.data.user) {
        StorageUtil.saveToken(response.data.token);
        StorageUtil.saveUser(response.data.user);
      }

      return response.data;
    } catch (error: unknown) {
      throw this.handleAuthError(error as HttpErrorResponse);
    }
  }

  static async register(userData: RegisterRequest): Promise<User> {
    try {
      const response = await api.post<User>(
        AUTH_CONSTANTS.API_ENDPOINTS.REGISTER,
        userData
      );
      return response.data;
    } catch (error: unknown) {
      throw this.handleAuthError(error as HttpErrorResponse);
    }
  }

  static logout(): void {
    StorageUtil.clearAuthData();
  }

  static getCurrentUser(): User | null {
    return StorageUtil.getUser();
  }

  static getToken(): string | null {
    return StorageUtil.getToken();
  }

  static isAuthenticated(): boolean {
    const token = this.getToken();
    const user = this.getCurrentUser();
    return !!(token && user);
  }

  static async getUserById(id: number): Promise<User> {
    try {
      const response = await api.get<User>(
        `${AUTH_CONSTANTS.API_ENDPOINTS.GET_USER}/${id}`
      );
      return response.data;
    } catch (error: unknown) {
      throw this.handleAuthError(error as HttpErrorResponse);
    }
  }

  static async refreshUser(): Promise<User> {
    try {
      const currentUser = this.getCurrentUser();
      if (!currentUser) {
        throw new Error("No current user found");
      }

      const updatedUser = await this.getUserById(currentUser.id);
      StorageUtil.saveUser(updatedUser);
      return updatedUser;
    } catch (error: unknown) {
      throw this.handleAuthError(error as HttpErrorResponse);
    }
  }

  static async validateToken(): Promise<boolean> {
    try {
      const user = this.getCurrentUser();
      if (!user) return false;

      await this.getUserById(user.id);
      return true;
    } catch {
      this.logout();
      return false;
    }
  }

  private static handleAuthError(error: HttpErrorResponse): AuthError {
    const authError: AuthError = new Error();

    if (error.response) {
      const data = error.response.data;

      authError.message =
        data?.message ?? JSON.stringify(data) ?? "Unknown error";
      authError.statusCode = error.response.status;

      switch (error.response.status) {
        case 401:
          authError.message =
            data?.message || AUTH_CONSTANTS.ERROR_MESSAGES.INVALID_CREDENTIALS;

          break;
        case 409:
          authError.message = data?.message ?? "Conflict error";
          break;
        case 400:
          authError.message = data?.message ?? "Bad request";
          break;
        default:
          authError.message = data?.message ?? "Server error";
      }
    } else if (error.request) {
      authError.message = AUTH_CONSTANTS.ERROR_MESSAGES.NETWORK_ERROR;
      authError.statusCode = 0;
    } else {
      authError.message = error.message ?? "Unknown error";
    }

    return authError;
  }

  static async initializeAuth(): Promise<{
    user: User | null;
    isAuthenticated: boolean;
  }> {
    try {
      const token = this.getToken();
      const user = this.getCurrentUser();

      if (!token || !user) {
        return { user: null, isAuthenticated: false };
      }

      const isValid = await this.validateToken();

      return isValid
        ? { user, isAuthenticated: true }
        : { user: null, isAuthenticated: false };
    } catch {
      this.logout();
      return { user: null, isAuthenticated: false };
    }
  }
}

export { default as api } from "./api";
