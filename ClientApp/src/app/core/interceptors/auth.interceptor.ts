import { Injectable } from "@angular/core";
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from "@angular/common/http";
import { Observable } from "rxjs";

import { OidcSecurityService } from "angular-auth-oidc-client";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private secureRoutes = [`${window.location.origin}/api`];

  constructor(private oidcSecurityService: OidcSecurityService) { }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // Ensure we send the token only to routes which are secured
    if (!this.secureRoutes.find((x) => request.url.startsWith(x))) {
      return next.handle(request);
    }

    const token = this.oidcSecurityService.getToken();

    if (!token) {
      return next.handle(request);
    }

    request = request.clone({
      headers: request.headers.set("Authorization", "Bearer " + token),
    });

    return next.handle(request);
  }
}
