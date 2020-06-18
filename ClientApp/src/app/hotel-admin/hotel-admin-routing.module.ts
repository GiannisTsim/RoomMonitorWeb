import { SensorDataComponent } from "./sensor-data/sensor-data.component";
import { MonitorManagementComponent } from "./monitor-management/monitor-management.component";
import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { HotelAdminGuard } from "../core/guards/hotel-admin.guard";
import { HotelAdminComponent } from "./hotel-admin.component";
import { UserManagementComponent } from "./user-management/user-management.component";
import { RoomManagementComponent } from "./room-management/room-management.component";

const routes: Routes = [
  {
    path: "",
    component: HotelAdminComponent,
    canActivate: [HotelAdminGuard],
    children: [
      {
        path: "",
        canActivateChild: [HotelAdminGuard],
        children: [
          {
            path: "users",
            component: UserManagementComponent,
          },
          {
            path: "rooms",
            component: RoomManagementComponent,
          },
          {
            path: "monitors",
            component: MonitorManagementComponent,
          },
          {
            path: "sensor-data",
            component: SensorDataComponent,
          },
        ],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HotelAdminRoutingModule {}
