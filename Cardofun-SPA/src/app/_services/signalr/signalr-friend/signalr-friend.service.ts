import { Injectable, isDevMode, OnInit } from '@angular/core';
import { LocalStorageService } from '../../local-storage/local-storage.service';
import { environment } from 'src/environments/environment';
import * as signalR from '@aspnet/signalr';
import { Subject, Observer, Subscription } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalrFriendService {

  constructor(private localStorageService: LocalStorageService) {
    const logLevel = isDevMode() ? signalR.LogLevel.Debug : signalR.LogLevel.None;
    this.hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(logLevel)
      .withUrl(this.baseUrl + 'signalr/friends', { accessTokenFactory: () => this.localStorageService.getToken() })
      .build();
  }

  private hubConnection: signalR.HubConnection;
  private baseUrl = environment.baseServerUrl;

  private followersCountSource = new Subject<Number>();

  private checkStateAndConnect() {
    // check the connections state. If it's disconnected - start the connection
    if (this.hubConnection.state === signalR.HubConnectionState.Disconnected) {
      this.hubConnection.start();
      // .then(() => console.log('Connection started'))
      // .catch(err => console.log('Error while starting connection: ' + err));
    }
  }

  private checkStateAndDisonnect() {
    // check the connection state. If it's connected but there's no one
    // who'd listen for the events - disconnect the hub
    if (this.hubConnection.state === signalR.HubConnectionState.Disconnected
      || this.followersCountSource.observers.length > 0) {
      return;
    }
    this.hubConnection.stop();
  }

  public subscribeOnCountOfFollowersReceived(observer: Observer<Number>): Subscription {
    // checking that the hub is connected and if it's not - connecting
    this.checkStateAndConnect();

    if (this.followersCountSource.observers.length === 0) {
      this.hubConnection.on('ReceiveFollowersCount', (count: Number) => {
        this.followersCountSource.next(count);
      });
    }
    return this.followersCountSource.subscribe(observer);
  }

  public unsubscribeFromCountOfFollowersReceived(subscriprion: Subscription) {
    if (!subscriprion) {
      return;
    }

    subscriprion.unsubscribe();
    if (this.followersCountSource.observers.length === 0) {
      this.hubConnection.off('ReceiveFollowersCount');
    }
    // checking if there's no observers left and if there isn't - close the hub connection
    this.checkStateAndDisonnect();
  }
}
