import { NgModule } from "@angular/core";

import { SharedModule } from "../shared/shared.module";

import { SystemAdminRoutingModule } from "./system-admin-routing.module";
import { SystemAdminComponent } from "./system-admin.component";
import { NavigationComponent } from "./navigation/navigation.component";
import { ConfigurationTypesComponent } from "./engineering/configuration-types/configuration-types.component";
import { SensorsComponent } from "./engineering/sensors/sensors.component";
import { HotelsComponent } from "./administration/hotels/hotels.component";
import { HotelDialogComponent } from "./administration/hotels/hotel-dialog/hotel-dialog.component";
import { UsersComponent } from "./administration/users/users.component";
import { UserDialogComponent } from "./administration/users/user-dialog/user-dialog.component";
import { DevicesComponent } from "./administration/devices/devices.component";

@NgModule({
  declarations: [
    SystemAdminComponent,
    NavigationComponent,
    ConfigurationTypesComponent,
    SensorsComponent,
    HotelsComponent,
    HotelDialogComponent,
    UsersComponent,
    UserDialogComponent,
    DevicesComponent,
  ],
  imports: [SharedModule, SystemAdminRoutingModule],
})
export class SystemAdminModule {}
