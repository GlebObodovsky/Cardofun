import { Injectable } from '@angular/core';
import { Subject, Observer, Subscription } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { ReadMessagesList } from 'src/app/_models/read-messages-list';
import { SignalrBaseService } from '../signalr-base-service';

@Injectable({
  providedIn: 'root'
})
export class SignalrMessageService extends SignalrBaseService {
  private newMessageSource = new Subject<Message>();
  private readMessagesSource = new Subject<ReadMessagesList>();
  protected subjects: Subject<any>[] = [this.newMessageSource, this.readMessagesSource];

  constructor() {
    super('signalr/chat');
  }

  //#region New message received
  public subscribeOnNewMessageReceived(observer: Observer<Message>): Subscription {
    return this.subscribeOnEvent<Message>(observer, 'ReceiveMessage', this.newMessageSource);
  }

  public unsubscribeFromNewMessageReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<Message>(subscriprion, 'ReceiveMessage', this.newMessageSource);
  }
  //#endregion New message received

  //#region Message marked as read
  public subscribeOnMessageMarkedAsRead(observer: Observer<ReadMessagesList>): Subscription {
    return this.subscribeOnEvent<ReadMessagesList>(observer, 'MarkMessageAsRead', this.readMessagesSource);
  }

  public unsubscribeFromMessageMarkedAsRead(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<ReadMessagesList>(subscriprion, 'MarkMessageAsRead', this.readMessagesSource);
  }
  //#endregion Message marked as read
}
