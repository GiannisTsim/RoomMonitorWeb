import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";

import {
  IApplicationUser,
  UserInvitation,
  HotelUser,
} from "./../../shared/models/ApplicationUser";
import { IHotelKey } from "./../../shared/models/Hotel";

@Injectable({
  providedIn: "root",
})
export class HotelUserService {
  constructor(private http: HttpClient) {}

  getAllHotelUsers() {
    return this.http
      .get<IApplicationUser[]>(`${window.location.origin}/api/users`)
      .pipe(
        map((data) => data.map((user) => new HotelUser().deserialize(user)))
      );
  }

  getHotelUsersByHotel(hotelKey: IHotelKey) {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .get<IApplicationUser[]>(`${window.location.origin}/api/hotels/users`, {
        params,
      })
      .pipe(
        map((data) => data.map((user) => new HotelUser().deserialize(user)))
      );
  }

  getHotelUserDetails(hotelKey: IHotelKey, userId: string) {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .get<IApplicationUser>(
        `${window.location.origin}/api/hotels/users/${userId}`,
        { params }
      )
      .pipe(map((data) => new HotelUser().deserialize(data)));
  }

  inviteHotelUser(hotelKey: IHotelKey, userInvitation: UserInvitation) {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .post<IApplicationUser>(
        `${window.location.origin}/api/hotels/users`,
        userInvitation,
        { params }
      )
      .pipe(map((data) => new HotelUser().deserialize(data)));
  }

  deleteHotelUser(hotelKey: IHotelKey, userId: string) {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http.delete(
      `${window.location.origin}/api/hotels/users/${userId}`,
      { params }
    );
  }
}
