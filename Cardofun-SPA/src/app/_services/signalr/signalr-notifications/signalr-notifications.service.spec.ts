/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { SignalrNotificationsService } from './signalr-notifications.service';

describe('Service: SignalrNotifications', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SignalrNotificationsService]
    });
  });

  it('should ...', inject([SignalrNotificationsService], (service: SignalrNotificationsService) => {
    expect(service).toBeTruthy();
  }));
});
