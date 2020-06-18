import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { SystemAdminGuard } from "./core/guards/system-admin.guard";
import { HotelAdminGuard } from "./core/guards/hotel-admin.guard";

import { HomeComponent } from "./core/components/home/home.component";
import { AboutComponent } from "./core/components/about/about.component";
import { ForbiddenComponent } from "./core/components/forbidden/forbidden.component";
import { NotFoundComponent } from "./core/components/not-found/not-found.component";

const routes: Routes = [
  { path: "", component: HomeComponent, pathMatch: "full" },
  { path: "about", component: AboutComponent },
  { path: "forbidden", component: ForbiddenComponent },
  { path: "not-found", component: NotFoundComponent },
  {
    path: "system-admin",
    loadChildren: () =>
      import("./system-admin/system-admin.module").then(
        (m) => m.SystemAdminModule
      ),
    canLoad: [SystemAdminGuard],
  },
  {
    path: "hotel-admin",
    loadChildren: () =>
      import("./hotel-admin/hotel-admin.module").then(
        (m) => m.HotelAdminModule
      ),
    canLoad: [HotelAdminGuard],
  },
  // {
  //   path: ":hotelId",
  //   loadChildren: () =>
  //     import("./hotel-user/hotel-user.module").then(m => m.HotelUserModule),
  //   canLoad: [HotelAccessGuard]
  // },
  {
    path: "**",
    redirectTo: "not-found",
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
