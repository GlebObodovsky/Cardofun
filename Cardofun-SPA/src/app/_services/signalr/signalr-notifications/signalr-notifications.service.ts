import { Injectable } from '@angular/core';
import { SignalrBaseService } from '../signalr-base-service';
import { Subject, Observer, Subscription } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalrNotificationsService extends SignalrBaseService {
  private unreadMessagesCountSource = new Subject<Number>();
  private followersCountSource = new Subject<Number>();
  protected subjects: Subject<any>[] = [this.unreadMessagesCountSource, this.followersCountSource];

  constructor() {
    super('signalr/notifications');
  }

  //#region Unread messages count received
  public subscribeOnUnreadMessagesCountReceived(observer: Observer<Number>): Subscription {
    return this.subscribeOnEvent<Number>(observer, 'ReceiveUnreadMessagesCount', this.unreadMessagesCountSource);
  }

  public unsubscribeFromUnreadMessagesCountReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<Number>(subscriprion, 'ReceiveUnreadMessagesCount', this.unreadMessagesCountSource);
  }
  //#endregion Unread messages count received

  //#region Followers Count Received
  public subscribeOnCountOfFollowersReceived(observer: Observer<Number>): Subscription {
    return this.subscribeOnEvent<Number>(observer, 'ReceiveFollowersCount', this.followersCountSource);
  }

  public unsubscribeFromCountOfFollowersReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<Number>(subscriprion, 'ReceiveFollowersCount', this.followersCountSource);
  }
  //#endregion Followers Count Received
}
