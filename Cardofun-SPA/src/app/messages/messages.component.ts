import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from '../_models/message';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { MessageContainer } from '../_models/enums/messageContainer';
import { MessageService } from '../_services/message/message.service';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { AuthService } from '../_services/auth/auth.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = MessageContainer.thread;
  currentUserId: number;

  constructor(private route: ActivatedRoute, private messagesService: MessageService,
    private alertifyService: AlertifyService, private authService: AuthService) { }

  ngOnInit() {
    this.currentUserId = this.authService.currentUser.id;

    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
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
