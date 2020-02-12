import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { environment } from 'src/environments/environment';
import { LocalStorageService } from '../../local-storage/local-storage.service';
import { Subject } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { ReadMessagesList } from 'src/app/_models/read-messages-list';
import { isDevMode } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SignalrMessageService {

  constructor(private localStorageService: LocalStorageService) { }

  private hubConnection: signalR.HubConnection;
  private baseUrl = environment.baseServerUrl;

  private newMessageSource = new Subject<Message>();
  newMessage = this.newMessageSource.asObservable();

  private readMessagesSource = new Subject<ReadMessagesList>();
  readMessages = this.readMessagesSource.asObservable();

  private unreadMessagesCountSource = new Subject<Number>();
  unreadMessagesCount = this.unreadMessagesCountSource.asObservable();

  public startConnection() {
    const logLevel = isDevMode() ? signalR.LogLevel.Debug : signalR.LogLevel.None;
    this.hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(logLevel)
      .withUrl(this.baseUrl + 'signalr/chat', { accessTokenFactory: () => this.localStorageService.getToken() })
      .build();

    this.hubConnection
      .start();
      // .then(() => console.log('Connection started'))
      // .catch(err => console.log('Error while starting connection: ' + err));

      this.addNotifyingOnMessageRecieved();
      this.addNotifyingOnMessageMarkedAsRead();
      this.addNotifyingOnUnreadMessagesCountReceived();
  }

  private addNotifyingOnMessageRecieved() {
    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      this.newMessageSource.next(message);
    });
  }

  private addNotifyingOnMessageMarkedAsRead() {
    this.hubConnection.on('MarkMessageAsRead', (messages: ReadMessagesList) => {
      this.readMessagesSource.next(messages);
    });
  }

  private addNotifyingOnUnreadMessagesCountReceived() {
    this.hubConnection.on('ReceiveUnreadMessagesCount', (count: Number) => {
      this.unreadMessagesCountSource.next(count);
    });
  }

  public stopConnection() {
    this.hubConnection.stop();
  }
}
