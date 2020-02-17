import { Injectable } from '@angular/core';
import { Subject, Observer, Subscription } from 'rxjs';
import { SignalrBaseService } from '../signalr-base-service';
import { User } from 'src/app/_models/user';
import { FriendshipRequestStatus } from 'src/app/_models/friendship-request-status';

@Injectable({
  providedIn: 'root'
})
export class SignalrFriendService extends SignalrBaseService {

  constructor() {
    super('signalr/friends');
  }

  private followersCountSource = new Subject<Number>();
  private friendshipRequestSource = new Subject<User>();
  private friendshipStatusSource = new Subject<FriendshipRequestStatus>();
  protected subjects: Subject<any>[] = [this.followersCountSource, this.friendshipRequestSource, this.friendshipStatusSource];

  //#region Followers Count Received
  public subscribeOnCountOfFollowersReceived(observer: Observer<Number>): Subscription {
    return this.subscribeOnEvent<Number>(observer, 'ReceiveFollowersCount', this.followersCountSource);
  }

  public unsubscribeFromCountOfFollowersReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<Number>(subscriprion, 'ReceiveFollowersCount', this.followersCountSource);
  }
  //#endregion Followers Count Received

  //#region Friendship Request Received
  public subscribeOnFriendshipRequestReceived(observer: Observer<User>): Subscription {
    return this.subscribeOnEvent<User>(observer, 'ReceiveFriendshipRequest', this.friendshipRequestSource);
  }

  public unsubscribeFromFriendshipRequestReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<User>(subscriprion, 'ReceiveFriendshipRequest', this.friendshipRequestSource);
  }
  //#endregion Friendship Request Received

  //#region Friendship Status Received
  public subscribeOnFriendshipStatusReceived(observer: Observer<FriendshipRequestStatus>): Subscription {
    return this.subscribeOnEvent<FriendshipRequestStatus>(observer, 'ReceiveFriendshipStatus', this.friendshipStatusSource);
  }

  public unsubscribeFromFriendshipStatusReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<FriendshipRequestStatus>(subscriprion, 'ReceiveFriendshipStatus', this.friendshipStatusSource);
  }
  //#endregion Friendship Status Received
}
