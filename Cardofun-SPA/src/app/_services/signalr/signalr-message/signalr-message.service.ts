import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { AlertifyService } from '../../alertify/alertify.service';
import { environment } from 'src/environments/environment';
import { LocalStorageService } from '../../local-storage/local-storage.service';
import { Subject } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { ReadMessagesList } from 'src/app/_models/read-messages-list';

@Injectable({
  providedIn: 'root'
})
export class SignalrMessageService {

  constructor(private alertifyService: AlertifyService, private localStorageService: LocalStorageService) { }

  private hubConnection: signalR.HubConnection;
  private baseUrl = environment.baseServerUrl;

  private newMessageSource = new Subject<Message>();
  newMessage = this.newMessageSource.asObservable();

  private readMessagesSource = new Subject<ReadMessagesList>();
  readMessages = this.readMessagesSource.asObservable();

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'message', { accessTokenFactory: () => this.localStorageService.getToken() })
      .build();

    this.hubConnection
      .start();
      // .then(() => console.log('Connection started'))
      // .catch(err => console.log('Error while starting connection: ' + err));

      this.addNotifyingOnMessageRecieved();
      this.addNotifyingOnMessageMarkedAsRead();
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

  public stopConnection() {
    this.hubConnection.stop();
  }
}
