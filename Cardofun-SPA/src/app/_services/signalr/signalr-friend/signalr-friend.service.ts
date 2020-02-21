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

  private incommingFriendshipRequestSource = new Subject<User>();
  private outgoingFriendshipRequestSource = new Subject<User>();
  private acceptedFriendshipSource = new Subject<User>();
  private friendshipStatusSource = new Subject<FriendshipRequestStatus>();
  protected subjects: Subject<any>[] = [
    this.incommingFriendshipRequestSource,
    this.outgoingFriendshipRequestSource,
    this.acceptedFriendshipSource,
    this.friendshipStatusSource
  ];

  //#region Incomming Friendship Request Received
  public subscribeOnIncommingFriendshipRequestReceived(observer: Observer<User>): Subscription {
    return this.subscribeOnEvent<User>(observer, 'ReceiveIncommingFriendshipRequest', this.incommingFriendshipRequestSource);
  }

  public unsubscribeFromIncommingFriendshipRequestReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<User>(subscriprion, 'ReceiveIncommingFriendshipRequest', this.incommingFriendshipRequestSource);
  }
  //#endregion Incomming Friendship Request Received

  //#region Outgoing Friendship Request Received
  public subscribeOnOutgoingFriendshipRequestReceived(observer: Observer<User>): Subscription {
    return this.subscribeOnEvent<User>(observer, 'ReceiveOutgoingFriendshipRequest', this.outgoingFriendshipRequestSource);
  }

  public unsubscribeFromOutgoingFriendshipRequestReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<User>(subscriprion, 'ReceiveOutgoingFriendshipRequest', this.outgoingFriendshipRequestSource);
  }
  //#endregion Outgoing Friendship Request Received

  //#region Accepted Friendship Received
  public subscribeOnAcceptedFriendshipReceived(observer: Observer<User>): Subscription {
    return this.subscribeOnEvent<User>(observer, 'ReceiveAcceptedFriendship', this.acceptedFriendshipSource);
  }

  public unsubscribeFromAcceptedFriendshipReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<User>(subscriprion, 'ReceiveAcceptedFriendship', this.acceptedFriendshipSource);
  }
  //#endregion Accepted Friendship Received

  //#region Friendship Status Received
  public subscribeOnFriendshipStatusReceived(observer: Observer<FriendshipRequestStatus>): Subscription {
    return this.subscribeOnEvent<FriendshipRequestStatus>(observer, 'ReceiveFriendshipStatus', this.friendshipStatusSource);
  }

  public unsubscribeFromFriendshipStatusReceived(subscriprion: Subscription) {
    return this.unsubscribeFromEvent<FriendshipRequestStatus>(subscriprion, 'ReceiveFriendshipStatus', this.friendshipStatusSource);
  }
  //#endregion Friendship Status Received
}
