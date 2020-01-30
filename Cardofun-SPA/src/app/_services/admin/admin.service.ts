import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { UserForModeration } from 'src/app/_models/user-for-moderation';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private http: HttpClient) { }

  baseUrl = environment.apiUrl + 'admin/';

  getUserWithRoles(userName: string): Observable<UserForModeration> {
    return this.http.get<UserForModeration>(this.baseUrl + 'usersWithRoles/' + userName);
  }

  updateUserRoles(userName: string, roles: {}) {
    return this.http.post(this.baseUrl + 'editRoles/' + userName, roles);
  }

  getRoles(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'roles');
  }
}
