import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FriendshipStatus } from 'src/app/_models/enums/friendshipStatus';
import { PaginatedResult } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { Observable } from 'rxjs';
import { UserFilterParams } from 'src/app/_models/userFilterParams';
import { UserService } from '../user/user.service';

@Injectable({
  providedIn: 'root'
})
export class FriendService {

  baseUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient, private authService: AuthService,
    private userService: UserService) { }

  getFriends(page?, itemsPerPage?, userParams?: UserFilterParams): Observable<PaginatedResult<User[]>> {
    return this.userService.getUsers(page, itemsPerPage, userParams);
  }

  requestFriendship(id: number) {
    return this.http.post(this.baseUrl + this.authService.currentUser.id + '/friends/' + id, {});
  }

  changeFriendshipStatus(id: number, status: FriendshipStatus) {
    return this.http.put(this.baseUrl + this.authService.currentUser.id + '/friends/' + id, {status});
  }

  deleteFriendship(id: number) {
    return this.http.delete(this.baseUrl + this.authService.currentUser.id + '/friends/' + id);
  }
}
