import { NgModule } from "@angular/core";

import { SharedModule } from "./../shared/shared.module";
import { NgxChartsModule } from "@swimlane/ngx-charts";

import { HotelAdminRoutingModule } from "./hotel-admin-routing.module";
import { HotelAdminComponent } from "./hotel-admin.component";
import { NavigationComponent } from "./navigation/navigation.component";
import { UserManagementComponent } from "./user-management/user-management.component";
import { RoomManagementComponent } from "./room-management/room-management.component";
import { RoomDialogComponent } from "./room-management/room-dialog/room-dialog.component";
import { TagManagementDialogComponent } from "./room-management/tag-management-dialog/tag-management-dialog.component";
import { MonitorManagementComponent } from "./monitor-management/monitor-management.component";
import { SensorDataComponent } from "./sensor-data/sensor-data.component";
import { TagDialogComponent } from "./room-management/tag-management-dialog/tag-dialog/tag-dialog.component";
import { UserDialogComponent } from "./user-management/user-dialog/user-dialog.component";
import { AssignMonitorDialogComponent } from "./monitor-management/assign-monitor-dialog/assign-monitor-dialog.component";

@NgModule({
  declarations: [
    HotelAdminComponent,
    NavigationComponent,
    UserManagementComponent,
    RoomManagementComponent,
    RoomDialogComponent,
    TagManagementDialogComponent,
    MonitorManagementComponent,
    SensorDataComponent,
    TagDialogComponent,
    UserDialogComponent,
    AssignMonitorDialogComponent,
  ],
  imports: [SharedModule, NgxChartsModule, HotelAdminRoutingModule],
})
export class HotelAdminModule {}
