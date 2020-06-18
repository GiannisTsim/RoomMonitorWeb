import { Component, OnInit, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { FormBuilder, Validators } from "@angular/forms";

import { IHotelKey } from "./../../../shared/models/Hotel";
import { IMonitorKey, MonitorAssigned } from "./../../../shared/models/Monitor";
import { MonitorService } from "./../../../core/services/monitor.service";
import { RoomService } from "./../../../core/services/room.service";
import { Room } from "src/app/shared/models/Room";

@Component({
  selector: "app-assign-monitor-dialog",
  templateUrl: "./assign-monitor-dialog.component.html",
  styleUrls: ["./assign-monitor-dialog.component.scss"],
})
export class AssignMonitorDialogComponent implements OnInit {
  monitorForm = this.formBuilder.group({
    roomType: ["", [Validators.required, Validators.maxLength(100)]],
    room: ["", [Validators.required, Validators.maxLength(100)]],
    monitor: ["", [Validators.required, Validators.maxLength(100)]],
  });

  rooms: Room[];

  constructor(
    private roomService: RoomService,
    private monitorService: MonitorService,
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<AssignMonitorDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      monitorKey: IMonitorKey;
    }
  ) {}

  ngOnInit(): void {
    const { MACAddress, ...hotelKey } = this.data.monitorKey;
    this.roomService
      .getRoomsByHotel(hotelKey)
      .subscribe((rooms) => (this.rooms = rooms));
  }

  onRoomSelectionChange(room: Room) {
    console.log(room);
    this.monitorForm.get("roomType").setValue(room.roomType);
    this.monitorForm.get("room").setValue(room.name);
  }

  onSubmit() {
    this.monitorService
      .assignMonitor(
        this.data.monitorKey,
        this.monitorForm.value as MonitorAssigned
      )
      .subscribe(
        (monitorAssigned: MonitorAssigned) =>
          this.dialogRef.close(monitorAssigned),
        (error) => {
          console.log(error);
        }
      );
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
