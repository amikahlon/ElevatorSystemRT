import { AUTH_CONSTANTS } from "../constants/auth.constants";
import type { User } from "../types";

export class StorageUtil {
  static setLocalStorage<T>(key: string, value: T): void {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
      console.error("Error setting localStorage:", error);
    }
  }

  static getLocalStorage<T>(key: string): T | null {
    try {
      const item = localStorage.getItem(key);
      return item ? (JSON.parse(item) as T) : null;
    } catch (error) {
      console.error("Error getting localStorage:", error);
      return null;
    }
  }

  static removeLocalStorage(key: string): void {
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error("Error removing localStorage:", error);
    }
  }

  static saveToken(token: string): void {
    this.setLocalStorage<string>(AUTH_CONSTANTS.STORAGE_KEYS.TOKEN, token);
  }

  static getToken(): string | null {
    return this.getLocalStorage<string>(AUTH_CONSTANTS.STORAGE_KEYS.TOKEN);
  }

  static removeToken(): void {
    this.removeLocalStorage(AUTH_CONSTANTS.STORAGE_KEYS.TOKEN);
  }

  static saveUser(user: User): void {
    this.setLocalStorage<User>(AUTH_CONSTANTS.STORAGE_KEYS.USER, user);
  }

  static getUser(): User | null {
    return this.getLocalStorage<User>(AUTH_CONSTANTS.STORAGE_KEYS.USER);
  }

  static removeUser(): void {
    this.removeLocalStorage(AUTH_CONSTANTS.STORAGE_KEYS.USER);
  }

  static clearAuthData(): void {
    this.removeToken();
    this.removeUser();
  }
}
