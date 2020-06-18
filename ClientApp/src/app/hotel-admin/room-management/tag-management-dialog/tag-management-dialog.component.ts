import { Component, OnInit, Inject } from "@angular/core";
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialog,
} from "@angular/material/dialog";

import { HotelTag } from "../../../shared/models/HotelTag";
import { IHotelKey } from "src/app/shared/models/Hotel";
import { HotelTagService } from "src/app/core/services/hotel-tag.service";
import { TagDialogComponent } from "./tag-dialog/tag-dialog.component";
import { isEqual } from "lodash-es";

@Component({
  selector: "app-tag-dialog",
  templateUrl: "./tag-management-dialog.component.html",
  styleUrls: ["./tag-management-dialog.component.scss"],
})
export class TagManagementDialogComponent implements OnInit {
  hotelTags: HotelTag[];

  constructor(
    private hotelTagService: HotelTagService,
    public dialog: MatDialog,
    private dialogRef: MatDialogRef<TagManagementDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      hotelKey: IHotelKey;
    }
  ) {}

  ngOnInit(): void {
    this.hotelTagService
      .getTagsByHotel(this.data.hotelKey)
      .subscribe((hotelTags) => (this.hotelTags = hotelTags));
  }

  onClose() {
    this.dialogRef.close();
  }

  openTagDialog(hotelTag?: HotelTag) {
    const dialogRef = this.dialog.open(TagDialogComponent, {
      data: {
        hotelKey: this.data.hotelKey,
        hotelTag,
      },
    });

    dialogRef.afterClosed().subscribe((newOrUpdatedHotelTag: HotelTag) => {
      if (newOrUpdatedHotelTag) {
        if (!hotelTag) {
          this.hotelTags = [...this.hotelTags, newOrUpdatedHotelTag];
        } else {
          const index = this.hotelTags.findIndex((r) =>
            isEqual(r.getKey(), hotelTag.getKey())
          );
          this.hotelTags[index] = hotelTag;
        }
      }
    });
  }
}
