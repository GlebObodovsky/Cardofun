import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { AlertifyService } from '../../alertify/alertify.service';
import { environment } from 'src/environments/environment';
import { LocalStorageService } from '../../local-storage/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class SignalrMessageService {

  constructor(private alertifyService: AlertifyService, private localStorageService: LocalStorageService) { }

  private hubConnection: signalR.HubConnection;
  private baseUrl = environment.baseServerUrl;

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl(this.baseUrl + 'message', { accessTokenFactory: () => this.localStorageService.getToken() })
                            .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public addNotifyingOnMessageRecieved = () => {
    this.hubConnection.on('ReceiveMessage', (data) => {
      this.alertifyService.message(data);
    });
  }
}
