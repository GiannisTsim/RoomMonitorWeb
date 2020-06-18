import { Component, OnInit } from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";

import {
  IMeasureApplication,
  ISwitchApplication,
} from "./../../../shared/models/SensorApplication";
import { ConfigurationService } from "./../../../core/services/configuration.service";

@Component({
  selector: "app-sensors",
  templateUrl: "./sensors.component.html",
  styleUrls: ["./sensors.component.scss"],
})
export class SensorsComponent implements OnInit {
  measureDisplayedColumns: string[] = [
    "index",
    "name",
    "description",
    "unitMeasure",
    "limitRange",
    "defaultRange",
    "actions",
  ];

  switchDisplayedColumns: string[] = [
    "index",
    "name",
    "description",
    "value_0",
    "value_1",
    "actions",
  ];

  applicationMeasureSource = new MatTableDataSource<IMeasureApplication>([]);
  applicationSwitchSource = new MatTableDataSource<ISwitchApplication>([]);

  constructor(private configurationService: ConfigurationService) {}

  ngOnInit(): void {
    this.configurationService
      .getMeasureApplications()
      .subscribe(
        (measureApplications) =>
          (this.applicationMeasureSource.data = measureApplications)
      );

    this.configurationService
      .getSwitchApplications()
      .subscribe(
        (switchApplications) =>
          (this.applicationSwitchSource.data = switchApplications)
      );
  }

  openMeasureApplicationDialog(application?: IMeasureApplication) {}

  openSwitchApplicationDialog(application?: ISwitchApplication) {}

  deleteApplicationAfterConfirm(
    application: ISwitchApplication | IMeasureApplication
  ) {}
}
