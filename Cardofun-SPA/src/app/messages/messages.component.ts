import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from '../_models/message';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { MessageContainer } from '../_models/enums/messageContainer';
import { MessageService } from '../_services/message/message.service';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { AuthService } from '../_services/auth/auth.service';
import { SignalrMessageService } from '../_services/signalr/signalr-message/signalr-message.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit, OnDestroy {
  messages: Message[];
  pagination: Pagination;
  messageContainer = MessageContainer.thread;
  currentUserId: number;
  newMessageSub: Subscription;
  readMessagesSub: Subscription;

  constructor(private route: ActivatedRoute, private messagesService: MessageService,
    private alertifyService: AlertifyService, private authService: AuthService,
    private signalrMessageService: SignalrMessageService) { }

  ngOnInit() {
    this.currentUserId = this.authService.currentUser.id;

    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });

    this.subscribeOnNewMessageEvent();
    this.subscribeOnReadMessageEvent();
  }

  subscribeOnNewMessageEvent() {
    this.newMessageSub = this.signalrMessageService.subscribeOnNewMessageReceived({
      next: message => {
        const theirUserId = this.currentUserId === message.senderId ? message.recipientId : message.senderId;
        const currentMessage = this.messages.find(m => m.senderId === theirUserId || m.recipientId === theirUserId);
        if (currentMessage) {
          const index = this.messages.indexOf(currentMessage, 0);
          if (index > -1) {
            this.messages.splice(index, 1);
          }
        }
        this.messages.unshift(message);
      },
      error: null,
      complete: null});
  }

  subscribeOnReadMessageEvent() {
    this.readMessagesSub = this.signalrMessageService.subscribeOnMessageMarkedAsRead({
      next: messages => {
        const messagesToAlter = this.messages.filter(x => messages.messageIds.includes(x.id));
        messagesToAlter.forEach(m => {
            m.isRead = true;
            m.readAt = new Date();
          });
      },
      error: null,
      complete: null});
  }

  ngOnDestroy() {
    this.signalrMessageService.unsubscribeFromNewMessageReceived(this.newMessageSub);
    this.signalrMessageService.unsubscribeFromMessageMarkedAsRead(this.readMessagesSub);
  }

  loadDialogues() {
    this.messagesService.getDialogues(this.pagination.currentPage, this.pagination.itemsPerPage, this.messageContainer)
      .subscribe((res: PaginatedResult<Message[]>) => {
        this.messages = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertifyService.error(error);
      });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadDialogues();
  }
}
