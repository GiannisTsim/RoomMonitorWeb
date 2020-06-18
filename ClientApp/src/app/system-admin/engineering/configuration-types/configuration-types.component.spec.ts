import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationTypesComponent } from './configuration-types.component';

describe('ConfigurationTypesComponent', () => {
  let component: ConfigurationTypesComponent;
  let fixture: ComponentFixture<ConfigurationTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurationTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
