import { TestBed } from '@angular/core/testing';

import { HotelUserService } from './hotel-user.service';

describe('HotelUserService', () => {
  let service: HotelUserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HotelUserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
