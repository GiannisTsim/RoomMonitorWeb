import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { SystemAdminGuard } from "./../core/guards/system-admin.guard";
import { SystemAdminComponent } from "./system-admin.component";
import { ConfigurationTypesComponent } from "./engineering/configuration-types/configuration-types.component";
import { SensorsComponent } from "./engineering/sensors/sensors.component";
import { UsersComponent } from "./administration/users/users.component";
import { HotelsComponent } from "./administration/hotels/hotels.component";
import { DevicesComponent } from "./administration/devices/devices.component";

const routes: Routes = [
  {
    path: "",
    component: SystemAdminComponent,
    canActivate: [SystemAdminGuard],
    children: [
      {
        path: "",
        canActivateChild: [SystemAdminGuard],
        children: [
          {
            path: "engineering",
            children: [
              {
                path: "configuration-types",
                component: ConfigurationTypesComponent,
              },
              {
                path: "sensors",
                component: SensorsComponent,
              },
            ],
          },
          {
            path: "administration",
            children: [
              {
                path: "hotels",
                component: HotelsComponent,
              },
              {
                path: "users",
                component: UsersComponent,
              },
              {
                path: "devices",
                component: DevicesComponent,
              },
            ],
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
export class SystemAdminRoutingModule {}
