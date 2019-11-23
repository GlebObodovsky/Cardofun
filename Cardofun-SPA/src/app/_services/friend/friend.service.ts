import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FriendshipStatus } from 'src/app/_models/enums/friendshipStatus';
import { FriendFilterParams } from 'src/app/_models/friendFilterParams';
import { PaginatedResult } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FriendService {

  baseUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getFriends(page?, itemsPerPage?, userFriendParams?: FriendFilterParams): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userFriendParams) {
      if (userFriendParams.sex) {
        params = params.append('sex', userFriendParams.sex);
      }
      if (userFriendParams.ageMin) {
        params = params.append('ageMin', userFriendParams.ageMin.toString());
      }
      if (userFriendParams.ageMax) {
        params = params.append('ageMax', userFriendParams.ageMax.toString());
      }
      if (userFriendParams.cityId) {
        params = params.append('cityId', userFriendParams.cityId.toString());
      }
      if (userFriendParams.countryIsoCode) {
        params = params.append('countryIsoCode', userFriendParams.countryIsoCode);
      }
      if (userFriendParams.languageSpeakingCode) {
        params = params.append('languageSpeakingCode', userFriendParams.languageSpeakingCode);
      }
      if (userFriendParams.languageLearningCode) {
        params = params.append('languageLearningCode', userFriendParams.languageLearningCode);
      }
      if (userFriendParams.friendshipStatus) {
        params = params.append('friendshipStatus', userFriendParams.friendshipStatus);
      }
      if (userFriendParams.isFriendshipOwned != null) {
        params = params.append('isFriendshipOwned', String(userFriendParams.isFriendshipOwned));
      }
    } else if (this.authService.currentUser.languagesTheUserLearns.length === 1) {
      params = params.append('languageSpeakingCode', this.authService.currentUser.languagesTheUserLearns[0].code);
    }

    return this.http.get<User[]>(this.baseUrl + this.authService.currentUser.id + '/friends/', { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
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
