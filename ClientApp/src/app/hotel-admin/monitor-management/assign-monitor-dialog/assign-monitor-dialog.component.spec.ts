import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignMonitorDialogComponent } from './assign-monitor-dialog.component';

describe('AssignMonitorDialogComponent', () => {
  let component: AssignMonitorDialogComponent;
  let fixture: ComponentFixture<AssignMonitorDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignMonitorDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignMonitorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
