import { TestBed } from '@angular/core/testing';

import { HotelTagService } from './hotel-tag.service';

describe('HotelTagService', () => {
  let service: HotelTagService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HotelTagService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
