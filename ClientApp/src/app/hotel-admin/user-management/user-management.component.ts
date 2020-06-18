import { Component, OnInit } from "@angular/core";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { MatDialog } from "@angular/material/dialog";
import { switchMap } from "rxjs/operators";
import { MatTableDataSource } from "@angular/material/table";

import { IHotelKey } from "src/app/shared/models/Hotel";
import { HotelUserService } from "./../../core/services/hotel-user.service";
import { HotelUser, UserRole } from "./../../shared/models/ApplicationUser";
import { OidcUserInfo } from "src/app/shared/models/OidcUserInfo";
import { UserDialogComponent } from "./user-dialog/user-dialog.component";
import { ConfirmationDialogComponent } from "src/app/shared/components/confirmation-dialog/confirmation-dialog.component";
import { isEqual } from "lodash-es";

@Component({
  selector: "app-user-management",
  templateUrl: "./user-management.component.html",
  styleUrls: ["./user-management.component.scss"],
})
export class UserManagementComponent implements OnInit {
  displayedColumns: string[] = ["index", "email", "actions"];
  userHotelKey: IHotelKey;
  userSource = new MatTableDataSource<HotelUser>([]);

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private hotelUserService: HotelUserService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.oidcSecurityService.userData$
      .pipe(
        switchMap((userData: OidcUserInfo) => {
          this.userHotelKey = JSON.parse(userData.hotel);
          console.log(this.userHotelKey);

          return this.hotelUserService.getHotelUsersByHotel(this.userHotelKey);
        })
      )
      .subscribe(
        (users: HotelUser[]) =>
          (this.userSource.data = users.filter(
            (user) => user.role === UserRole.HOTEL_EMPLOYEE
          ))
      );
  }

  openUserDialog() {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      data: {
        hotelKey: this.userHotelKey,
      },
    });

    dialogRef.afterClosed().subscribe((newUser: HotelUser) => {
      if (newUser) {
        this.userSource.data = [...this.userSource.data, newUser];
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
          .deleteHotelUser(this.userHotelKey, user.userId)
          .subscribe(
            () => {
              const index = this.userSource.data.findIndex((u) =>
                isEqual(u.userId, user.userId)
              );
              this.userSource.data.splice(index, 1);
              this.userSource._updateChangeSubscription();
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
