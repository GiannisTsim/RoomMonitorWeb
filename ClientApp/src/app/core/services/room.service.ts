import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";


import { Room, IRoom, IRoomKey } from "src/app/shared/models/Room";
import { IHotelKey } from "src/app/shared/models/Hotel";
import { RoomType } from "src/app/shared/models/RoomType";

@Injectable({
  providedIn: "root",
})
export class RoomService {
  constructor(private http: HttpClient) { }

  getRoomsByHotel(hotelKey: IHotelKey): Observable<Room[]> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .get<IRoom[]>(`${window.location.origin}/api/hotels/rooms`, { params })
      .pipe(map((data) => data.map((room) => new Room().deserialize(room))));
  }

  getRoomDetails(roomKey: IRoomKey): Observable<Room> {
    const params = new HttpParams({ fromObject: { ...roomKey } });
    return this.http
      .get<IRoom>(`${window.location.origin}/api/hotels/rooms/detail`, {
        params,
      })
      .pipe(map((data) => new Room().deserialize(data)));
  }

  addRoom(hotelKey: IHotelKey, newRoom: Room): Observable<Room> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .post<IRoom>(`${window.location.origin}/api/hotels/rooms`, newRoom, {
        params,
      })
      .pipe(map((data) => new Room().deserialize(data)));
  }

  editRoom(roomKey: IRoomKey, updatedRoom: Room): Observable<Room> {
    console.log(roomKey);
    console.log(updatedRoom);
    const params = new HttpParams({ fromObject: { ...roomKey } });
    return this.http
      .put<IRoom>(
        `${window.location.origin}/api/hotels/rooms`,
        updatedRoom,
        {
          params,
        }
      )
      .pipe(map((data) => new Room().deserialize(data)));
  }

  deleteRoom(roomKey: IRoomKey) {
    const params = new HttpParams({ fromObject: { ...roomKey } });
    return this.http.delete(`${window.location.origin}/api/hotels/rooms`, {
      params,
    });
  }

  getRoomTypes(): Observable<RoomType[]> {
    return this.http.get<RoomType[]>(
      `${window.location.origin}/api/room-types`
    );
  }
}
