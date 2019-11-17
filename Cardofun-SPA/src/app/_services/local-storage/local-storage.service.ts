import { Injectable } from '@angular/core';
import { User } from 'src/app/_models/user';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {

  constructor() { }

  private tokenKey = 'token';
  private userKey = 'user';

  setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string {
    return localStorage.getItem(this.tokenKey);
  }

  removeToken() {
    localStorage.removeItem(this.tokenKey);
  }

  setUser(user: User) {
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }

  getUser(): User {
    return JSON.parse(localStorage.getItem(this.userKey));
  }

  removeUser() {
    localStorage.removeItem(this.userKey);
  }
}
