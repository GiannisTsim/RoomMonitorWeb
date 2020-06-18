import { OidcUserInfo } from "../../../shared/models/OidcUserInfo";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { Component, OnInit } from "@angular/core";

import { UserRole } from "src/app/shared/models/ApplicationUser";

@Component({
  selector: "app-header",
  templateUrl: "./header.component.html",
  styleUrls: ["./header.component.scss"],
})
export class HeaderComponent implements OnInit {
  isAuthenticated: boolean;
  userInfo: OidcUserInfo;

  UserRole = UserRole;

  constructor(private oidcSecurityService: OidcSecurityService) {}

  ngOnInit() {
    this.oidcSecurityService.isAuthenticated$.subscribe((auth) => {
      this.isAuthenticated = auth;
    });

    this.oidcSecurityService.userData$.subscribe((userData: OidcUserInfo) => {
      this.userInfo = userData;
    });
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoff();
  }
}
