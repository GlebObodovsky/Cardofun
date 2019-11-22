import { Injectable } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FriendshipStatus } from 'src/app/_models/friendshipStatus';
import { FriendFilterParams } from 'src/app/_models/friendFilterParams';
import { PaginatedResult } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FriendService {

  baseUrl = environment.apiUrl + 'users/' + this.authService.currentUser.id + '/friends/';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getFriends(page?, itemsPerPage?, userParams?: FriendFilterParams): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams) {
      if (userParams.sex) {
        params = params.append('sex', userParams.sex);
      }
      if (userParams.ageMin) {
        params = params.append('ageMin', userParams.ageMin.toString());
      }
      if (userParams.ageMax) {
        params = params.append('ageMax', userParams.ageMax.toString());
      }
      if (userParams.cityId) {
        params = params.append('cityId', userParams.cityId.toString());
      }
      if (userParams.countryIsoCode) {
        params = params.append('countryIsoCode', userParams.countryIsoCode);
      }
      if (userParams.languageSpeakingCode) {
        params = params.append('languageSpeakingCode', userParams.languageSpeakingCode);
      }
      if (userParams.languageLearningCode) {
        params = params.append('languageLearningCode', userParams.languageLearningCode);
      }
    } else if (this.authService.currentUser.languagesTheUserLearns.length === 1) {
      params = params.append('languageSpeakingCode', this.authService.currentUser.languagesTheUserLearns[0].code);
    }

    return this.http.get<User[]>(this.baseUrl, { observe: 'response', params })
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
    return this.http.post(this.baseUrl + id, {});
  }

  changeFriendshipStatus(id: number, status: FriendshipStatus) {
    return this.http.put(this.baseUrl + id, {status});
  }

  deleteFriendship(id: number) {
    return this.http.delete(this.baseUrl + id);
  }
}
