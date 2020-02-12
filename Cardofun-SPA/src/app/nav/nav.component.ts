import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { Router } from '@angular/router';
import { MessageService } from '../_services/message/message.service';
import { SignalrMessageService } from '../_services/signalr/signalr-message/signalr-message.service';
import { Subscription } from 'rxjs';
import { FriendService } from '../_services/friend/friend.service';
import { SignalrFriendService } from '../_services/signalr/signalr-friend/signalr-friend.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  countOfUnreadMessages: Number;
  countOfFollowers: Number;

  constructor(public authService: AuthService, private alertifyService: AlertifyService,
    private router: Router, private messageService: MessageService,
    private frienService: FriendService,
    private signalrMessageService: SignalrMessageService,
    private signalrFriendService: SignalrFriendService) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    if (this.isLoggedIn()) {
      this.subscribeOnGetCountOfUnread();
      this.subscribeOnGetCountOfFollowers();
    }
  }

  login() {
    this.authService.login(this.model).subscribe(() => {
      this.subscribeOnGetCountOfUnread();
      this.subscribeOnGetCountOfFollowers();
    }, error => {
      this.alertifyService.error(error);
    }, () => {
      this.router.navigate(['/messages']);
    });
  }

  logout() {
    this.authService.logout();
    this.alertifyService.success('Logged out sucessfully');
    this.router.navigate(['']);
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }

  subscribeOnGetCountOfUnread() {
    this.messageService.getCountOfUnread().subscribe((countOfUnread: Number) => {
      this.countOfUnreadMessages = countOfUnread;
    }, () => {
      this.alertifyService.error('Cannot get the amount of unread messages');
    });
    this.signalrMessageService.unreadMessagesCount.subscribe((count: Number) => {
      this.countOfUnreadMessages = count;
    });
  }

  subscribeOnGetCountOfFollowers() {
    this.frienService.getCountOfFollowers().subscribe((countOfFollowers: Number) => {
      this.countOfFollowers = countOfFollowers;
    }, () => {
      this.alertifyService.error('Cannot get the amount of followers');
    });
    this.signalrFriendService.followersCount.subscribe((count: Number) => {
      this.countOfFollowers = count;
    });
  }
}
