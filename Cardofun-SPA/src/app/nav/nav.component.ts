import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { Router } from '@angular/router';
import { MessageService } from '../_services/message/message.service';
import { SignalrMessageService } from '../_services/signalr/signalr-message/signalr-message.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  countOfUnreadMessages: Number;

  constructor(public authService: AuthService, private alertifyService: AlertifyService,
    private router: Router, private messageService: MessageService,
    private signalrMessageService: SignalrMessageService) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    if (this.isLoggedIn()) {
      this.subscribeOnGetCountOfUnread();
    }
  }

  login() {
    this.authService.login(this.model).subscribe(() => {
      this.subscribeOnGetCountOfUnread();
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
}
