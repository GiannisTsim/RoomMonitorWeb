import { Observable } from "rxjs";
import { Component, Inject, Optional } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { FormBuilder, Validators } from "@angular/forms";

import { Hotel } from "src/app/shared/models/Hotel";
import { HotelService } from "src/app/core/services/hotel.service";

@Component({
  selector: "app-hotel-dialog",
  templateUrl: "./hotel-dialog.component.html",
  styleUrls: ["./hotel-dialog.component.scss"],
})
export class HotelDialogComponent {
  availableCountries = [{ countryCode: "GR", name: "Greece" }];

  hotelForm = this.formBuilder.group({
    hotelChain: ["", [Validators.required, Validators.maxLength(100)]],
    countryCode: [
      this.availableCountries[0].countryCode,
      [Validators.required, Validators.maxLength(2)],
    ],
    town: ["", [Validators.required, Validators.maxLength(100)]],
    suburb: ["", [Validators.required, Validators.maxLength(100)]],
    numStar: [0, [Validators.required]],
  });

  constructor(
    private hotelService: HotelService,
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<HotelDialogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public hotel: Hotel
  ) {
    if (hotel) {
      this.hotelForm.get("hotelChain").setValue(hotel.hotelChain);
      this.hotelForm.get("countryCode").setValue(hotel.countryCode);
      this.hotelForm.get("town").setValue(hotel.town);
      this.hotelForm.get("suburb").setValue(hotel.suburb);
      this.hotelForm.get("numStar").setValue(hotel.numStar);
    }
  }

  handleNumStarChange(numStar) {
    if (this.hotelForm.get("numStar").value !== numStar) {
      this.hotelForm.get("numStar").setValue(numStar);
    } else {
      this.hotelForm.get("numStar").setValue(0);
    }
  }

  onSubmit() {
    let req: Observable<Hotel>;
    console.log(this.hotelForm.value);

    if (!this.hotel) {
      req = this.hotelService.addHotel(this.hotelForm.value as Hotel);
    } else {
      req = this.hotelService.editHotel(
        this.hotel,
        this.hotelForm.value as Hotel
      );
    }

    req.subscribe(
      (hotel: Hotel) => this.dialogRef.close(hotel),
      (error) => {
        console.log(error);
      }
    );
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
