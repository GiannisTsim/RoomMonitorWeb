import { Component, OnInit } from "@angular/core";
import { OidcSecurityService } from "angular-auth-oidc-client";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
})
export class AppComponent implements OnInit {
  constructor(private oidcSecurityService: OidcSecurityService) {
    // if (this.oidcSecurityService.moduleSetup) {
    //   this.doCallbackLogicIfRequired();
    // } else {
    //   this.oidcSecurityService.onModuleSetup.subscribe(() => {
    //     this.doCallbackLogicIfRequired();
    //   });
    // }
  }

  ngOnInit() {
    console.log("APP INIT");
    this.oidcSecurityService
      .checkAuth()
      .subscribe((isAuthenticated) =>
        console.log("app authenticated", isAuthenticated)
      );
  }

  // private doCallbackLogicIfRequired() {
  //   // Will do a callback, if the url has a code and state parameter.
  //   console.log("Executing OIDC Callback...");
  //   this.oidcSecurityService.authorizedCallbackWithCode(
  //     window.location.toString()
  //   );
  // }
}
