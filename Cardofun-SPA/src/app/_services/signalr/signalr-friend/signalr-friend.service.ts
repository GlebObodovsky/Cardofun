import { Injectable, isDevMode } from '@angular/core';
import { LocalStorageService } from '../../local-storage/local-storage.service';
import { environment } from 'src/environments/environment';
import * as signalR from '@aspnet/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalrFriendService {

constructor(private localStorageService: LocalStorageService) { }

  private hubConnection: signalR.HubConnection;
  private baseUrl = environment.baseServerUrl;

  private followersCountSource = new Subject<Number>();
  followersCount = this.followersCountSource.asObservable();

  public startConnection() {
    const logLevel = isDevMode() ? signalR.LogLevel.Debug : signalR.LogLevel.None;
    this.hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(logLevel)
      .withUrl(this.baseUrl + 'signalr/friends', { accessTokenFactory: () => this.localStorageService.getToken() })
      .build();

    this.hubConnection
      .start();
      // .then(() => console.log('Connection started'))
      // .catch(err => console.log('Error while starting connection: ' + err));

      this.addNotifyingOnFollowersCountReceived();
  }

  private addNotifyingOnFollowersCountReceived() {
    this.hubConnection.on('ReceiveFollowersCount', (count: Number) => {
      this.followersCountSource.next(count);
    });
  }

  public stopConnection() {
    this.hubConnection.stop();
  }
}
