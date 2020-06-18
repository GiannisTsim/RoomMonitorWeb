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
import { map, take } from "rxjs/operators";

import { OidcUserInfo } from "./../../shared/models/OidcUserInfo";
import { UserRole } from "./../../shared/models/ApplicationUser";

@Injectable({
  providedIn: "root",
})
export class SystemAdminGuard
  implements CanActivate, CanActivateChild, CanLoad {
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
    return this.checkSystemAdminRole();
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
    return this.checkSystemAdminRole();
  }

  canLoad(
    route: Route,
    segments: UrlSegment[]
  ): Observable<boolean> | Promise<boolean> | boolean {
    console.log("SystemAdminGuard - canLoad");
    return this.checkSystemAdminRole().pipe(take(1));
  }

  checkSystemAdminRole(): Observable<boolean> {
    return this.oidcSecurityService.userData$.pipe(
      map((user: OidcUserInfo) => {
        if (user.role !== UserRole.SYSTEM_ADMIN) {
          this.router.navigate(["/forbidden"]);
          return false;
        }
        return true;
      })
    );
  }
}
