import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth/auth.service';
import { SignalrMessageService } from './_services/signalr/signalr-message/signalr-message.service';
import { SignalrFriendService } from './_services/signalr/signalr-friend/signalr-friend.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Cardofun';

  constructor(private authService: AuthService, private signalrMessageService: SignalrMessageService,
    private signalrFriendService: SignalrFriendService) {}

  ngOnInit() {
    this.authService.refreshUserInformation();
    if (this.authService.currentUser) {
      this.signalrMessageService.startConnection();
      this.signalrFriendService.startConnection();
    }
  }
}
