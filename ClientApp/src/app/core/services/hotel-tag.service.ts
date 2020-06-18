import { Injectable } from "@angular/core";
import { HttpParams, HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

import { IHotelKey } from "src/app/shared/models/Hotel";
import {
  HotelTag,
  IHotelTagKey,
  IHotelTag,
} from "src/app/shared/models/HotelTag";
import { map } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class HotelTagService {
  constructor(private http: HttpClient) { }

  getTagsByHotel(hotelKey: IHotelKey): Observable<HotelTag[]> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .get<HotelTag[]>(`${window.location.origin}/api/hotels/tags`, {
        params,
      })
      .pipe(
        map((data) =>
          data.map((hotelTag) => new HotelTag().deserialize(hotelTag))
        )
      );
  }

  getHotelTagDetails(hotelTagKey: IHotelTagKey): Observable<HotelTag> {
    const params = new HttpParams({ fromObject: { ...hotelTagKey } });
    return this.http
      .get<IHotelTag>(`${window.location.origin}/api/hotels/tags/detail`, {
        params,
      })
      .pipe(map((data) => new HotelTag().deserialize(data)));
  }

  addHotelTag(
    hotelKey: IHotelKey,
    newHotelTag: HotelTag
  ): Observable<HotelTag> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .post<IHotelTag>(
        `${window.location.origin}/api/hotels/tags`,
        newHotelTag,
        {
          params,
        }
      )
      .pipe(map((data) => new HotelTag().deserialize(data)));
  }

  editHotelTag(
    hotelTagKey: IHotelTagKey,
    updatedHotelTag: HotelTag
  ): Observable<HotelTag> {
    const params = new HttpParams({ fromObject: { ...hotelTagKey } });
    return this.http
      .put<IHotelTag>(
        `${window.location.origin}/api/hotels/tags`,
        updatedHotelTag,
        {
          params,
        }
      )
      .pipe(map((data) => new HotelTag().deserialize(data)));
  }

  deleteHotelTag(hotelTagKey: IHotelTagKey) {
    const params = new HttpParams({ fromObject: { ...hotelTagKey } });
    return this.http.delete<IHotelTag>(
      `${window.location.origin}/api/hotels/tags`,
      {
        params,
      }
    );
  }
}
