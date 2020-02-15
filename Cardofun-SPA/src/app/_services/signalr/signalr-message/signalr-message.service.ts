import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { environment } from 'src/environments/environment';
import { LocalStorageService } from '../../local-storage/local-storage.service';
import { Subject, Observer, Subscription } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { ReadMessagesList } from 'src/app/_models/read-messages-list';
import { isDevMode } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SignalrMessageService {

  constructor(private localStorageService: LocalStorageService) {
    const logLevel = isDevMode() ? signalR.LogLevel.Debug : signalR.LogLevel.None;
    this.hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(logLevel)
      .withUrl(this.baseUrl + 'signalr/chat', { accessTokenFactory: () => this.localStorageService.getToken() })
      .build();
  }

  private isConnectionInProgress = false;

  private hubConnection: signalR.HubConnection;
  private baseUrl = environment.baseServerUrl;

  private newMessageSource = new Subject<Message>();
  private readMessagesSource = new Subject<ReadMessagesList>();
  private unreadMessagesCountSource = new Subject<Number>();

  //#region New message received
  public subscribeOnNewMessageReceived(observer: Observer<Message>): Subscription {
    return this.subscribeOnEvent<Message>(observer, 'ReceiveMessage', this.newMessageSource);
  }

  public unsubscribeFromNewMessageReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<Message>(subscriprion, 'ReceiveMessage', this.newMessageSource);
  }
  //#endregion

  //#region Message marked as read
  public subscribeOnMessageMarkedAsRead(observer: Observer<ReadMessagesList>): Subscription {
    return this.subscribeOnEvent<ReadMessagesList>(observer, 'MarkMessageAsRead', this.readMessagesSource);
  }

  public unsubscribeFromMessageMarkedAsRead(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<ReadMessagesList>(subscriprion, 'MarkMessageAsRead', this.readMessagesSource);
  }
  //#endregion Message marked as read

  //#region Unread messages count received
  public subscribeOnUnreadMessagesCountReceived(observer: Observer<Number>): Subscription {
    return this.subscribeOnEvent<Number>(observer, 'ReceiveUnreadMessagesCount', this.unreadMessagesCountSource);
  }

  public unsubscribeFromUnreadMessagesCountReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<Number>(subscriprion, 'ReceiveUnreadMessagesCount', this.unreadMessagesCountSource);
  }
  //#endregion Unread messages count received

  private checkStateAndConnect() {
    // check the connections state. If it's disconnected - start the connection
    if (!this.isConnectionInProgress && this.hubConnection.state === signalR.HubConnectionState.Disconnected) {
      this.isConnectionInProgress = true;
      console.log('Создаю соединение....');
      this.hubConnection.start()
        .then(() => { this.isConnectionInProgress = false; })
        .catch(err => { this.isConnectionInProgress = false; });
    }
  }

  private checkStateAndDisonnect() {
    // check the connection state. If it's connected but there's no one
    // who'd listen for the events - disconnect the hub
    if (this.hubConnection.state === signalR.HubConnectionState.Disconnected
      || this.newMessageSource.observers.length > 0
      || this.readMessagesSource.observers.length > 0
      || this.unreadMessagesCountSource.observers.length > 0) {
      return;
    }
    this.hubConnection.stop();
  }

  //#region Generic methods fur subscription / unsubscription
  private subscribeOnEvent<TSubject>(observer: Observer<TSubject>, nameOfHubMethod: string,
    subject: Subject<TSubject>): Subscription {
    // checking that the hub is connected and if it's not - connecting
    this.checkStateAndConnect();

    if (subject.observers.length === 0) {
      this.hubConnection.on(nameOfHubMethod, (object: TSubject) => {
        subject.next(object);
      });
    }
    return subject.subscribe(observer);
  }

  private unsubscribeFromEvent<TSubject>(subscriprion: Subscription, nameOfHubMethod: string,
    subject: Subject<TSubject>) {
    if (!subscriprion) {
      return;
    }

    subscriprion.unsubscribe();
    if (subject.observers.length === 0) {
      this.hubConnection.off(nameOfHubMethod);
    }
    // checking if there's no observers left and if there isn't - close the hub connection
    this.checkStateAndDisonnect();
  }
  //#endregion Generic methods fur subscription / unsubscription
}
