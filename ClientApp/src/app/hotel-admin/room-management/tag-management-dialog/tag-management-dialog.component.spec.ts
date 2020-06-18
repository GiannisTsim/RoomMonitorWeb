import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { TagManagementDialogComponent } from "./tag-management-dialog.component";

describe("TagManagementDialogComponent", () => {
  let component: TagManagementDialogComponent;
  let fixture: ComponentFixture<TagManagementDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TagManagementDialogComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TagManagementDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
