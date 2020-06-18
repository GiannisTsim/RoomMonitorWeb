import { HotelTagService } from "./../../../core/services/hotel-tag.service";
import {
  Component,
  OnInit,
  ElementRef,
  ViewChild,
  Inject,
} from "@angular/core";
import { Validators, FormControl, FormBuilder } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Observable } from "rxjs";

import { HotelTag } from "./../../../shared/models/HotelTag";
import { Room } from "src/app/shared/models/Room";
import { IHotelKey } from "src/app/shared/models/Hotel";
import { RoomService } from "src/app/core/services/room.service";
import { RoomType } from "src/app/shared/models/RoomType";

@Component({
  selector: "app-room-dialog",
  templateUrl: "./room-dialog.component.html",
  styleUrls: ["./room-dialog.component.scss"],
})
export class RoomDialogComponent implements OnInit {
  roomForm = this.formBuilder.group({
    name: ["", [Validators.required, Validators.maxLength(100)]],
    roomType: ["", [Validators.required, Validators.maxLength(100)]],
    tags: [[]],
  });

  hotelTags: HotelTag[];

  constructor(
    private roomService: RoomService,
    private hotelTagService: HotelTagService,
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<RoomDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      hotelKey: IHotelKey;
      roomTypes: RoomType[];
      room: Room;
    }
  ) {
    if (data.room) {
      this.roomForm.get("name").setValue(data.room.name);
      this.roomForm.get("roomType").setValue(data.room.roomType);
      this.roomForm.get("tags").setValue(data.room.tags);
    }
  }

  ngOnInit() {
    this.hotelTagService
      .getTagsByHotel(this.data.hotelKey)
      .subscribe((hotelTags) => (this.hotelTags = hotelTags));
  }

  onSubmit() {
    const newRoom: Room = {
      ...this.roomForm.value,
    };

    let req: Observable<Room>;

    if (!this.data.room) {
      req = this.roomService.addRoom(this.data.hotelKey, newRoom);
    } else {
      req = this.roomService.editRoom(this.data.room.getKey(), newRoom);
    }

    req.subscribe(
      (room: Room) => this.dialogRef.close(room),
      (error) => {
        console.log(error);
      }
    );
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
