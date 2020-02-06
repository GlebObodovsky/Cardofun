import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth/auth.service';
import { SignalrMessageService } from './_services/signalr/signalr-message/signalr-message.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Cardofun';

  constructor(private authService: AuthService, private signalrMessage: SignalrMessageService) {}

  ngOnInit() {
    this.authService.refreshUserInformation();
    this.signalrMessage.startConnection();
    this.signalrMessage.addNotifyingOnMessageRecieved();
  }
}
