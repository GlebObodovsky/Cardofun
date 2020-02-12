/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { SignalrFriendService } from './signalr-friend.service';

describe('Service: SignalrFriend', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SignalrFriendService]
    });
  });

  it('should ...', inject([SignalrFriendService], (service: SignalrFriendService) => {
    expect(service).toBeTruthy();
  }));
});
