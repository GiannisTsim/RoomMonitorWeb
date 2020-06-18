import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HotelDialogComponent } from './hotel-dialog.component';

describe('HotelDialogComponent', () => {
  let component: HotelDialogComponent;
  let fixture: ComponentFixture<HotelDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HotelDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HotelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
