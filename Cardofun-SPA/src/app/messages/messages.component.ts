import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from '../_models/message';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { MessageContainer } from '../_models/enums/messageContainer';
import { MessageService } from '../_services/message/message.service';
import { AlertifyService } from '../_services/alertify/alertify.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  container: MessageContainer = MessageContainer.thread;

  constructor(private route: ActivatedRoute, private messagesService: MessageService,
    private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }

  loadDialogues() {
    this.messagesService.getDialogues(this.pagination.currentPage, this.pagination.itemsPerPage, this.container)
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
