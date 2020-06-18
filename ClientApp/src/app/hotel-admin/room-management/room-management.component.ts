import { Component, OnInit } from "@angular/core";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { switchMap } from "rxjs/operators";
import { MatTableDataSource } from "@angular/material/table";
import { MatDialog } from "@angular/material/dialog";
import { isEqual } from "lodash-es";

import { TagManagementDialogComponent } from "./tag-management-dialog/tag-management-dialog.component";
import { RoomDialogComponent } from "./room-dialog/room-dialog.component";
import { OidcUserInfo } from "src/app/shared/models/OidcUserInfo";
import { IHotelKey } from "src/app/shared/models/Hotel";
import { RoomService } from "./../../core/services/room.service";
import { Room } from "src/app/shared/models/Room";
import { RoomType } from "src/app/shared/models/RoomType";
import { ConfirmationDialogComponent } from "src/app/shared/components/confirmation-dialog/confirmation-dialog.component";

@Component({
  selector: "app-room-management",
  templateUrl: "./room-management.component.html",
  styleUrls: ["./room-management.component.scss"],
})
export class RoomManagementComponent implements OnInit {
  displayedColumns: string[] = ["index", "name", "roomType", "tags", "actions"];
  userHotelKey: IHotelKey;
  roomSource = new MatTableDataSource<Room>([]);
  roomTypes: RoomType[];

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private roomService: RoomService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.oidcSecurityService.userData$
      .pipe(
        switchMap((userData: OidcUserInfo) => {
          this.userHotelKey = JSON.parse(userData.hotel);
          console.log(this.userHotelKey);

          return this.roomService.getRoomsByHotel(this.userHotelKey);
        })
      )
      .subscribe((rooms: Room[]) => (this.roomSource.data = rooms));

    this.roomService
      .getRoomTypes()
      .subscribe((roomTypes) => (this.roomTypes = roomTypes));
  }

  openRoomDialog(room?: Room) {
    const dialogRef = this.dialog.open(RoomDialogComponent, {
      data: {
        hotelKey: this.userHotelKey,
        roomTypes: this.roomTypes,
        room,
      },
    });

    dialogRef.afterClosed().subscribe((newOrUpdatedRoom: Room) => {
      if (newOrUpdatedRoom) {
        if (!room) {
          this.roomSource.data = [...this.roomSource.data, newOrUpdatedRoom];
        } else {
          const index = this.roomSource.data.findIndex((r) =>
            isEqual(r.getKey(), room.getKey())
          );
          this.roomSource.data[index] = newOrUpdatedRoom;
          this.roomSource._updateChangeSubscription();
        }
      }
    });
  }

  deleteRoomAfterConfirm(room: Room) {
    const message = `${room.roomType} '${room.name}' will be permanently deleted.\nAre you sure you want to proceed ?`;

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: message,
    });

    dialogRef.afterClosed().subscribe((isConfirmed: boolean) => {
      if (isConfirmed) {
        console.log("confirmed");
        this.roomService.deleteRoom(room.getKey()).subscribe(
          () => {
            const index = this.roomSource.data.findIndex((r) =>
              isEqual(r.getKey(), room.getKey())
            );
            this.roomSource.data.splice(index, 1);
            this.roomSource._updateChangeSubscription();
          },
          (error) => {
            console.log(error);
          }
        );
      } else {
        console.log("not confirmed");
      }
    });
  }

  openTagManagementDialog() {
    const dialogRef = this.dialog.open(TagManagementDialogComponent, {
      data: {
        hotelKey: this.userHotelKey,
      },
    });
  }
}
