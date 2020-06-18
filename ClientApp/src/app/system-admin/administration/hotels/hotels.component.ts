import { Component, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatTableDataSource } from "@angular/material/table";
import { isEqual } from "lodash-es";

import { ConfirmationDialogComponent } from "./../../../shared/components/confirmation-dialog/confirmation-dialog.component";
import { HotelService } from "./../../../core/services/hotel.service";
import { Hotel } from "./../../../shared/models/Hotel";
import { HotelDialogComponent } from "./hotel-dialog/hotel-dialog.component";

@Component({
  selector: "app-hotels",
  templateUrl: "./hotels.component.html",
  styleUrls: ["./hotels.component.scss"],
})
export class HotelsComponent implements OnInit {
  displayedColumns: string[] = [
    "index",
    "hotelChain",
    "countryCode",
    "town",
    "suburb",
    "numStar",
    "actions",
  ];
  hotelSource = new MatTableDataSource<Hotel>([]);

  constructor(private hotelService: HotelService, public dialog: MatDialog) {}

  ngOnInit(): void {
    this.hotelService
      .getAllHotels()
      .subscribe((hotels) => (this.hotelSource.data = hotels));
  }

  openHotelDialog(hotel?: Hotel) {
    const dialogRef = this.dialog.open(HotelDialogComponent, {
      data: hotel,
    });
    dialogRef.afterClosed().subscribe((newOrUpdatedHotel: Hotel) => {
      if (newOrUpdatedHotel) {
        if (!hotel) {
          this.hotelSource.data = [...this.hotelSource.data, newOrUpdatedHotel];
        } else {
          const index = this.hotelSource.data.findIndex((h) =>
            isEqual(h.getKey(), hotel.getKey())
          );
          this.hotelSource.data[index] = newOrUpdatedHotel;
          this.hotelSource._updateChangeSubscription();
        }
      }
    });
  }

  deleteHotelAfterConfirm(hotel: Hotel) {
    const message = `'${hotel.hotelChain}' at ${hotel.countryCode},${hotel.town},${hotel.suburb} and all related assets will be permanently deleted.\nAre you sure you want to proceed ?`;
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: message,
    });
    dialogRef.afterClosed().subscribe((isConfirmed: boolean) => {
      if (isConfirmed) {
        console.log("confirmed");
        this.hotelService.deleteHotel(hotel.getKey()).subscribe(() => {
          const index = this.hotelSource.data.findIndex((h) =>
            isEqual(h.getKey(), hotel.getKey())
          );
          this.hotelSource.data.splice(index, 1);
          this.hotelSource._updateChangeSubscription();
        });
      }
    });
  }
}
