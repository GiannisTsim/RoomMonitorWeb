import { Observable } from "rxjs";
import { IHotelKey } from "./../../shared/models/Hotel";
import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";

import { Hotel, IHotel } from "src/app/shared/models/Hotel";

@Injectable({
  providedIn: "root",
})
export class HotelService {

  constructor(private http: HttpClient) { }

  getAllHotels(): Observable<Hotel[]> {
    return this.http
      .get<IHotel[]>(`${window.location.origin}/api/hotels`)
      .pipe(map((data) => data.map((hotel) => new Hotel().deserialize(hotel))));
  }

  getHotelDetails(hotelKey: IHotelKey): Observable<Hotel> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .get<IHotel>(`${window.location.origin}/api/hotels/detail`, {
        params,
      })
      .pipe(map((data) => new Hotel().deserialize(data)));
  }

  addHotel(newHotel: Hotel): Observable<Hotel> {
    return this.http
      .post<IHotel>(`${window.location.origin}/api/hotels`, newHotel)
      .pipe(map((data) => new Hotel().deserialize(data)));
  }

  editHotel(hotelKey: IHotelKey, updatedHotel: Hotel): Observable<Hotel> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .put<IHotel>(`${window.location.origin}/api/hotels`, updatedHotel, {
        params,
      })
      .pipe(map((data) => new Hotel().deserialize(data)));
  }

  deleteHotel(hotelKey: IHotelKey) {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http.delete(`${window.location.origin}/api/hotels`, {
      params,
    });
  }
}
