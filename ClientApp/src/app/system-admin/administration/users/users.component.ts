import { Component, OnInit } from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";
import { MatDialog } from "@angular/material/dialog";

import { ConfirmationDialogComponent } from "./../../../shared/components/confirmation-dialog/confirmation-dialog.component";
import { HotelUserService } from "./../../../core/services/hotel-user.service";
import { HotelUser } from "./../../../shared/models/ApplicationUser";
import { UserDialogComponent } from "./user-dialog/user-dialog.component";

@Component({
  selector: "app-users",
  templateUrl: "./users.component.html",
  styleUrls: ["./users.component.scss"],
})
export class UsersComponent implements OnInit {
  displayedColumns: string[] = ["index", "email", "hotel", "role", "actions"];
  hotelUserSource = new MatTableDataSource<HotelUser>([]);

  constructor(
    private hotelUserService: HotelUserService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.hotelUserService
      .getAllHotelUsers()
      .subscribe((users) => (this.hotelUserSource.data = users));
  }

  openUserDialog() {
    const dialogRef = this.dialog.open(UserDialogComponent);
    dialogRef.afterClosed().subscribe((newUser: HotelUser) => {
      if (newUser) {
        this.hotelUserSource.data = [...this.hotelUserSource.data, newUser];
      }
    });
  }

  deleteUserAfterConfirm(user: HotelUser) {
    const message = `'${user.email}' will be permanently deleted.\nAre you sure you want to proceed ?`;
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: message,
    });
    dialogRef.afterClosed().subscribe((isConfirmed: boolean) => {
      if (isConfirmed) {
        console.log("confirmed");
        this.hotelUserService
          .deleteHotelUser(user.hotel, user.userId)
          .subscribe(
            () => {
              const index = this.hotelUserSource.data.findIndex(
                (h) => h.userId === user.userId
              );
              this.hotelUserSource.data.splice(index, 1);
              this.hotelUserSource._updateChangeSubscription();
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
}
