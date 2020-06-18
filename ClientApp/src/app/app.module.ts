import { AuthInterceptor } from "./core/interceptors/auth.interceptor";
import { BrowserModule } from "@angular/platform-browser";
import { NgModule, APP_INITIALIZER } from "@angular/core";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import {
  OidcConfigService,
  AuthModule,
  LogLevel,
} from "angular-auth-oidc-client";

import { SharedModule } from "./shared/shared.module";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { HeaderComponent } from "./core/components/header/header.component";
import { FooterComponent } from "./core/components/footer/footer.component";
import { HomeComponent } from "./core/components/home/home.component";
import { AboutComponent } from "./core/components/about/about.component";
import { ForbiddenComponent } from "./core/components/forbidden/forbidden.component";
import { NotFoundComponent } from "./core/components/not-found/not-found.component";

export function configureAuth(oidcConfigService: OidcConfigService) {
  return () =>
    oidcConfigService.withConfig({
      stsServer: window.location.origin,
      redirectUrl: window.location.origin,
      postLogoutRedirectUri: window.location.origin,
      clientId: "roomMonitorSpaClient",
      scope: "openid roomMonitorApi.user profile.user",
      responseType: "code",
      silentRenew: true,
      silentRenewUrl: `${window.location.origin}/silent-renew.html`,
      logLevel: LogLevel.Warn,
      triggerAuthorizationResultEvent: true,
    });
}

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    HomeComponent,
    AboutComponent,
    ForbiddenComponent,
    NotFoundComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    AuthModule.forRoot(),
    SharedModule,
  ],
  providers: [
    OidcConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: configureAuth,
      deps: [OidcConfigService],
      multi: true,
    },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
