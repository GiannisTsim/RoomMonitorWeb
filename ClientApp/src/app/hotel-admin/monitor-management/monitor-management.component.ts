import { AssignMonitorDialogComponent } from "./assign-monitor-dialog/assign-monitor-dialog.component";
import { Component, OnInit } from "@angular/core";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { switchMap } from "rxjs/operators";
import { MatTableDataSource } from "@angular/material/table";
import { forkJoin } from "rxjs";
import { MatDialog } from "@angular/material/dialog";

import { IHotelKey } from "./../../shared/models/Hotel";
import {
  MonitorAssigned,
  MonitorUnassigned,
  IMonitorKey,
} from "./../../shared/models/Monitor";
import { MonitorService } from "./../../core/services/monitor.service";
import { OidcUserInfo } from "src/app/shared/models/OidcUserInfo";
import { ConfirmationDialogComponent } from "src/app/shared/components/confirmation-dialog/confirmation-dialog.component";
import { isEqual } from "lodash-es";

@Component({
  selector: "app-monitor-management",
  templateUrl: "./monitor-management.component.html",
  styleUrls: ["./monitor-management.component.scss"],
})
export class MonitorManagementComponent implements OnInit {
  userHotelKey: IHotelKey;
  unassignedColumns: string[] = [
    "index",
    "MACAddress",
    "configurationType",
    "registrationDtm",
    "actions",
  ];
  assignedColumns: string[] = [
    "index",
    "MACAddress",
    "configurationType",
    "registrationDtm",
    "room",
    "monitor",
    "placementDtm",
    "actions",
  ];
  monitorAssignedSource = new MatTableDataSource<MonitorAssigned>([]);
  monitorUnassignedSource = new MatTableDataSource<MonitorUnassigned>([]);

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private monitorService: MonitorService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.oidcSecurityService.userData$
      .pipe(
        switchMap((userData: OidcUserInfo) => {
          this.userHotelKey = JSON.parse(userData.hotel);
          return forkJoin([
            this.monitorService.getAssignedMonitors(this.userHotelKey),
            this.monitorService.getUnsassignedMonitors(this.userHotelKey),
          ]);
        })
      )
      .subscribe(
        ([monitorsAssigned, monitorsUnassigned]: [
          MonitorAssigned[],
          MonitorUnassigned[]
        ]) => {
          this.monitorAssignedSource.data = monitorsAssigned;
          this.monitorUnassignedSource.data = monitorsUnassigned;
        }
      );
  }

  openAssignMonitorDialog(monitorUnassigned: MonitorUnassigned) {
    const dialogRef = this.dialog.open(AssignMonitorDialogComponent, {
      data: {
        monitorKey: monitorUnassigned.getKey(),
      },
    });

    dialogRef.afterClosed().subscribe((monitorAssigned: MonitorAssigned) => {
      if (monitorAssigned) {
        this.monitorAssignedSource.data = [
          ...this.monitorAssignedSource.data,
          monitorAssigned,
        ];

        const index = this.monitorUnassignedSource.data.findIndex(
          (m) => m.MACAddress === monitorAssigned.MACAddress
        );
        this.monitorUnassignedSource.data.splice(index, 1);
        this.monitorUnassignedSource._updateChangeSubscription();
      }
    });
  }

  deassignMonitorAfterConfirm(monitorAssigned: MonitorAssigned) {
    const message = `Readings of Monitor '${monitorAssigned.monitor}' in ${monitorAssigned.roomType} '${monitorAssigned.room}' will not be stored until it is assigned again.\nAre you sure you want to proceed ?`;

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: message,
    });

    dialogRef.afterClosed().subscribe((isConfirmed: boolean) => {
      if (isConfirmed) {
        this.monitorService.deassignMonitor(monitorAssigned.getKey()).subscribe(
          (monitorUnassigned: MonitorUnassigned) => {
            this.monitorUnassignedSource.data = [
              ...this.monitorUnassignedSource.data,
              monitorUnassigned,
            ];

            const index = this.monitorAssignedSource.data.findIndex((m) =>
              isEqual(m.getKey(), monitorAssigned.getKey())
            );
            this.monitorAssignedSource.data.splice(index, 1);
            this.monitorAssignedSource._updateChangeSubscription();
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
