import { TestBed } from '@angular/core/testing';

import { HotelAdminGuard } from './hotel-admin.guard';

describe('HotelAdminGuard', () => {
  let guard: HotelAdminGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(HotelAdminGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
