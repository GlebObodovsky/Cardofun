import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { PaginatedResult } from 'src/app/_models/pagination';
import { Message } from 'src/app/_models/message';
import { AuthService } from '../auth/auth.service';
import { map } from 'rxjs/operators';
import { MessageContainer } from 'src/app/_models/enums/messageContainer';
import { MessageThread } from 'src/app/_models/messageThread';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getDialogues(page?, itemsPerPage?, container?: MessageContainer) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (container) {
      params = params.append('container', container);
    }

    return this.http.get<Message[]>(this.baseUrl + this.authService.currentUser.id + '/messages/dialogues', { observe: 'response', params })
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

  getMessageThread(secondUserId: number, page?, itemsPerPage?) {
    const paginatedResult: PaginatedResult<MessageThread> = new PaginatedResult<MessageThread>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<MessageThread>(this.baseUrl + this.authService.currentUser.id + '/messages/thread/' + secondUserId, { observe: 'response', params })
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

  getCountOfUnread(): Observable<Number> {
    return this.http.get<number>(this.baseUrl + this.authService.currentUser.id + '/messages/countOfUnread');
  }

  createMessage(message: any) {
    message.senderId = this.authService.currentUser.id;
    return this.http.post(this.baseUrl + this.authService.currentUser.id + '/messages', message);
  }

  markMessageAsRead(id: string) {
    return this.http.post(this.baseUrl + this.authService.currentUser.id + '/messages/' + id + '/read', {});
  }
}
