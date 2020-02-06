/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { SignalrMessageService } from './signalr-message.service';

describe('Service: SignalrMessage', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SignalrMessageService]
    });
  });

  it('should ...', inject([SignalrMessageService], (service: SignalrMessageService) => {
    expect(service).toBeTruthy();
  }));
});
