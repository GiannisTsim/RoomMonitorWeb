import { Component, OnInit, Inject } from "@angular/core";
import { Validators, FormBuilder } from "@angular/forms";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { IHotelKey } from "src/app/shared/models/Hotel";
import { HotelTag } from "src/app/shared/models/HotelTag";
import { HotelTagService } from "src/app/core/services/hotel-tag.service";
import { Observable } from "rxjs";

@Component({
  selector: "app-tag-dialog",
  templateUrl: "./tag-dialog.component.html",
  styleUrls: ["./tag-dialog.component.scss"],
})
export class TagDialogComponent {
  hotelTagForm = this.formBuilder.group({
    tag: ["", [Validators.required, Validators.maxLength(100)]],
    description: ["", [Validators.required, Validators.maxLength(256)]],
  });

  constructor(
    private hotelTagService: HotelTagService,
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<TagDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      hotelKey: IHotelKey;
      hotelTag: HotelTag;
    }
  ) {
    if (data.hotelTag) {
      this.hotelTagForm.get("tag").setValue(data.hotelTag.tag);
      this.hotelTagForm.get("description").setValue(data.hotelTag.description);
    }
  }

  onSubmit() {
    const newHotelTag: HotelTag = {
      ...this.hotelTagForm.value,
    };

    let req: Observable<HotelTag>;

    if (!this.data.hotelTag) {
      req = this.hotelTagService.addHotelTag(this.data.hotelKey, newHotelTag);
    } else {
      req = this.hotelTagService.editHotelTag(
        this.data.hotelTag.getKey(),
        newHotelTag
      );
    }

    req.subscribe(
      (hotelTag: HotelTag) => this.dialogRef.close(hotelTag),
      (error) => {
        console.log(error);
      }
    );
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
