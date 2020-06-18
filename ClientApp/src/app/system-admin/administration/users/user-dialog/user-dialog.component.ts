import { Component, Inject, Optional, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { FormBuilder, Validators } from "@angular/forms";

import {
  UserRole,
  UserInvitation,
  HotelUser,
} from "../../../../shared/models/ApplicationUser";
import { HotelService } from "./../../../../core/services/hotel.service";
import { HotelUserService } from "../../../../core/services/hotel-user.service";
import { IHotelKey } from "./../../../../shared/models/Hotel";

@Component({
  selector: "app-user-dialog",
  templateUrl: "./user-dialog.component.html",
  styleUrls: ["./user-dialog.component.scss"],
})
export class UserDialogComponent implements OnInit {
  userForm = this.formBuilder.group({
    email: [
      "",
      [Validators.required, Validators.email, Validators.maxLength(100)],
    ],
    role: ["", Validators.required],
  });

  UserRole = UserRole;
  hotelKeys: IHotelKey[] = [];
  selectedHotelKey: IHotelKey;

  constructor(
    private hotelUserService: HotelUserService,
    private hotelService: HotelService,
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<UserDialogComponent>,
    @Optional()
    @Inject(MAT_DIALOG_DATA)
    public data: { hotelKey: IHotelKey }
  ) {
    if (data?.hotelKey) {
      this.selectedHotelKey = data.hotelKey;
    }
  }

  ngOnInit(): void {
    if (!this.data?.hotelKey) {
      this.hotelService
        .getAllHotels()
        .subscribe(
          (hotels) => (this.hotelKeys = hotels.map((hotel) => hotel.getKey()))
        );
    }
  }

  onSubmit() {
    this.hotelUserService
      .inviteHotelUser(
        this.selectedHotelKey,
        this.userForm.value as UserInvitation
      )
      .subscribe(
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
