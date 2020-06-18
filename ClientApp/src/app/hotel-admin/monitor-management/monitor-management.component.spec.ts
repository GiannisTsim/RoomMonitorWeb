import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitorManagementComponent } from './monitor-management.component';

describe('MonitorManagementComponent', () => {
  let component: MonitorManagementComponent;
  let fixture: ComponentFixture<MonitorManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MonitorManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MonitorManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
