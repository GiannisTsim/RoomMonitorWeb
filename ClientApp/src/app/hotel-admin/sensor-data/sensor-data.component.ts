import { ChartDataset } from "../../shared/models/ChartDataset";
import { Component, OnInit } from "@angular/core";
import { switchMap } from "rxjs/operators";

import { IMonitorAssignedAlternateKey } from "./../../shared/models/Monitor";
import { Room } from "./../../shared/models/Room";
import { RoomService } from "./../../core/services/room.service";
import { MonitorService } from "./../../core/services/monitor.service";
import { MonitorAssigned } from "src/app/shared/models/Monitor";
import { IHotelKey } from "src/app/shared/models/Hotel";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { OidcUserInfo } from "src/app/shared/models/OidcUserInfo";
import { SensorDataService } from "./../../core/services/sensor-data.service";
import { MonitorData, RoomData } from "./../../shared/models/SensorData";

@Component({
  selector: "app-sensor-data",
  templateUrl: "./sensor-data.component.html",
  styleUrls: ["./sensor-data.component.scss"],
})
export class SensorDataComponent implements OnInit {
  userHotelKey: IHotelKey;
  rooms: Room[] = [];
  selectedRoom: Room;
  roomMonitorKeys: IMonitorAssignedAlternateKey[] = [];
  selectedMonitorKey: IMonitorAssignedAlternateKey;

  chartContainerTitle: string;

  chartDatasets: ChartDataset[] = [];

  // chart options -----------------
  view: any[] = [700, 300];
  legend = true;
  showLabels = true;
  animations = true;
  xAxis = true;
  yAxis = true;
  showYAxisLabel = true;
  showXAxisLabel = true;
  xAxisLabel = "Datetime";
  // yAxisLabel: string = 'Population';
  timeline = true;
  colorScheme = {
    domain: ["#5AA454", "#E44D25", "#CFC0BB", "#7aa3e5", "#a8385d", "#aae3f5"],
  };
  // --------------------------------

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private roomService: RoomService,
    private monitorService: MonitorService,
    private sensorDataService: SensorDataService
  ) {}

  ngOnInit(): void {
    this.oidcSecurityService.userData$
      .pipe(
        switchMap((userData: OidcUserInfo) => {
          this.userHotelKey = JSON.parse(userData.hotel);
          return this.roomService.getRoomsByHotel(this.userHotelKey);
        })
      )
      .subscribe((rooms: Room[]) => (this.rooms = rooms));
  }

  onRoomSelectionChange(room: Room) {
    this.roomMonitorKeys = [];
    this.selectedMonitorKey = undefined;
    this.selectedRoom = room;
    this.monitorService
      .getMonitorHistoryByRoom(room.getKey())
      .subscribe((monitorKeys) => (this.roomMonitorKeys = monitorKeys));
  }

  onGetRoomReadingsClick() {
    this.chartContainerTitle = `${this.selectedRoom.roomType} '${this.selectedRoom.name}'`;
    this.sensorDataService
      .getRoomReadings(this.selectedRoom.getKey())
      .subscribe((roomData: RoomData) => {
        this.chartDatasets = this.sensorDataService.formatToChartData(
          roomData.monitorData
        );
      });
  }

  onMonitorSelectionChange(monitorKey: IMonitorAssignedAlternateKey) {
    this.selectedMonitorKey = monitorKey;
  }

  onGetMonitorReadingsClick() {
    this.chartContainerTitle = `${this.selectedMonitorKey.roomType} '${this.selectedMonitorKey.room}' / ${this.selectedMonitorKey.monitor}`;
    this.sensorDataService
      .getMonitorReadings(this.selectedMonitorKey)
      .subscribe((monitorData: MonitorData) => {
        this.chartDatasets = this.sensorDataService.formatToChartData([
          monitorData,
        ]);
      });
  }

  // chart methods ------------------------------------------------

  onSelect(data): void {
    console.log("Item clicked", JSON.parse(JSON.stringify(data)));
  }

  onActivate(data): void {
    console.log("Activate", JSON.parse(JSON.stringify(data)));
  }

  onDeactivate(data): void {
    console.log("Deactivate", JSON.parse(JSON.stringify(data)));
  }

  // ---------------------------------------------------------------
}
