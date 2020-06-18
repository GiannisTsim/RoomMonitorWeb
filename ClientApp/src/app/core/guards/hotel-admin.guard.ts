import { Injectable } from "@angular/core";
import {
  CanActivate,
  CanActivateChild,
  CanLoad,
  Route,
  UrlSegment,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router,
} from "@angular/router";
import { Observable } from "rxjs";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { take, map } from "rxjs/operators";

import { OidcUserInfo } from "./../../shared/models/OidcUserInfo";
import { UserRole } from "src/app/shared/models/ApplicationUser";

@Injectable({
  providedIn: "root",
})
export class HotelAdminGuard implements CanActivate, CanActivateChild, CanLoad {
  constructor(
    private oidcSecurityService: OidcSecurityService,
    private router: Router
  ) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    console.log("SystemAdminGuard - canActivate");
    return this.checkHotelAdminRole();
  }

  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    console.log("SystemAdminGuard - canActivateChild");
    return this.checkHotelAdminRole();
  }

  canLoad(
    route: Route,
    segments: UrlSegment[]
  ): Observable<boolean> | Promise<boolean> | boolean {
    console.log("SystemAdminGuard - canLoad");
    return this.checkHotelAdminRole().pipe(take(1));
  }

  checkHotelAdminRole(): Observable<boolean> {
    return this.oidcSecurityService.userData$.pipe(
      map((user: OidcUserInfo) => {
        if (user.role !== UserRole.HOTEL_ADMIN) {
          this.router.navigate(["/forbidden"]);
          return false;
        }
        return true;
      })
    );
  }
}
