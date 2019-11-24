import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from 'src/app/_models/user';
import { PaginatedResult } from 'src/app/_models/pagination';
import { map } from 'rxjs/operators';
import { UserFilterParams } from 'src/app/_models/userFilterParams';
import { AuthService } from '../auth/auth.service';
import { FriendshipStatus } from 'src/app/_models/enums/friendshipStatus';
import { SupscriptionState } from 'src/app/_models/enums/supscriptionState';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.apiUrl + 'users';

  constructor(private http: HttpClient, private authService: AuthService) { }

  checkIfUserExists(login: string) {
    return this.http.head(this.baseUrl + '/' + login);
  }

  getUsers(page?, itemsPerPage?, userParams?: UserFilterParams): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();
    let address = this.baseUrl;

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
      if (userParams.subscriptionState) {
        address = this.baseUrl + '/' + this.authService.currentUser.id + '/friends';

        if (userParams.subscriptionState !== SupscriptionState.friends) {
          params = params.append('friendshipStatus', FriendshipStatus.requested);
          params = params.append('friendshipStatus', FriendshipStatus.declined);
        }
        // subscriptions are owned by me
        if (userParams.subscriptionState === SupscriptionState.subscriptions) {
          params = params.append('isFriendshipOwned', String(true));
        // subscriptions are owned by them
        } else if (userParams.subscriptionState === SupscriptionState.followers) {
          params = params.append('isFriendshipOwned', String(false));
        }
      }
    } else if (this.authService.currentUser.languagesTheUserLearns.length === 1) {
      params = params.append('languageSpeakingCode', this.authService.currentUser.languagesTheUserLearns[0].code);
    }

    return this.http.get<User[]>(address, { observe: 'response', params })
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

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + '/' + id);
  }

  putUser(id: number, user: User) {
    return this.http.put<User>(this.baseUrl + '/' + id, user);
  }

  setMainPhoto(userId: number, id: string) {
    return this.http.post(this.baseUrl + '/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: string) {
    return this.http.delete(this.baseUrl + '/' + userId + '/photos/' + id);
  }
}
