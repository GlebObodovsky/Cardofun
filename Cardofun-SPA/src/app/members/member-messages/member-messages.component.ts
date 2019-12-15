import { Component, OnInit, Input, ElementRef, ViewChild } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { MessageService } from 'src/app/_services/message/message.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { MessageThread } from 'src/app/_models/messageThread';
import { UserForMessage } from 'src/app/_models/user-for-message';
import { Observable } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth/auth.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})

export class MemberMessagesComponent implements OnInit {
  @Input() theirUser: User;
  myUser: User;
  messages: Message[];
  newMessage: any = {};
  pagination = {
    currentPage: 1,
    itemsPerPage: 10
  } as Pagination;
  loading = false;

  @ViewChild('scrollMe', { static: false }) scrollDiv;

  constructor(private messageService: MessageService, private alertifyService: AlertifyService,
    private authService: AuthService) { }

  ngOnInit() {
    this.myUser = this.authService.currentUser;

    this.loadMessages(this.pagination.currentPage, this.pagination.itemsPerPage).subscribe(
      res => this.handleResponse(res),
      error => this.alertifyService.error(error)
    );
  }

  loadMessages(page?: number, itemsPerPage?: number): Observable<PaginatedResult<MessageThread>> {
    this.loading = true;

    return this.messageService.getMessageThread(this.theirUser.id,
      page || ++this.pagination.currentPage, itemsPerPage || this.pagination.itemsPerPage);
  }

  handleResponse(res: PaginatedResult<MessageThread>) {
    this.loading = false;
    if (this.messages == null) {
      this.messages = res.result.messages;
    } else {
      this.messages = this.messages.concat(res.result.messages);
    }

    this.pagination = res.pagination;
  }

  onScroll(event) {
    // get the scroll height before adding new messages
    const startingScrollHeight = event.target.scrollHeight;
    if (event.target.scrollTop < 100) {
      if (this.pagination.currentPage >= this.pagination.totalPages) {
        return;
      } else if (!this.loading) {
        this.loadMessages().subscribe((res) => {
          this.handleResponse(res);
          // using setTimeout lets the app "wait a beat" so it can measure
          // new scroll height *after* messages are added
          setTimeout(() => {
            const newScrollHeight = this.scrollDiv.nativeElement.scrollHeight;
            // set the scroll height from the difference of the new and starting scroll height
            this.scrollDiv.nativeElement.scrollTo(0, newScrollHeight - startingScrollHeight);
          });
        });
      }
    }
  }

  sendMessage(text: string) {
    this.newMessage.recipientId = this.theirUser.id;
    this.messageService.createMessage(this.newMessage).subscribe((message: Message) => {
      this.messages.unshift(message);
      this.newMessage.text = '';
    }, error => {
      this.alertifyService.error(error);
    });
  }
}
