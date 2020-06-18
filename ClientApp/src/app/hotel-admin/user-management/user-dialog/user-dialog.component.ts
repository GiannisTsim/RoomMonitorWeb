import { Component, Inject } from "@angular/core";
import { Validators, FormBuilder } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Observable } from "rxjs";

import {
  HotelUser,
  UserInvitation,
  UserRole,
} from "src/app/shared/models/ApplicationUser";
import { HotelUserService } from "src/app/core/services/hotel-user.service";
import { IHotelKey } from "src/app/shared/models/Hotel";

@Component({
  selector: "app-user-dialog",
  templateUrl: "./user-dialog.component.html",
  styleUrls: ["./user-dialog.component.scss"],
})
export class UserDialogComponent {
  userForm = this.formBuilder.group({
    email: [
      "",
      [Validators.required, Validators.email, Validators.maxLength(100)],
    ],
    role: [UserRole.HOTEL_EMPLOYEE, [Validators.required]],
  });

  constructor(
    private hotelUserService: HotelUserService,
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<UserDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: { hotelKey: IHotelKey }
  ) {}

  onSubmit() {
    let req: Observable<HotelUser>;

    req = this.hotelUserService.inviteHotelUser(
      this.data.hotelKey,
      this.userForm.value as UserInvitation
    );

    req.subscribe(
      (user: HotelUser) => this.dialogRef.close(user),
      (error) => {
        console.log(error);
      }
    );
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
