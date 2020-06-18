import { IRoomKey } from "./../../shared/models/Room";
import { Observable } from "rxjs";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs/operators";

import {
  IMonitorAssigned,
  MonitorAssigned,
  IMonitor,
  MonitorUnassigned,
  IMonitorKey,
  IMonitorAssignedAlternateKey,
} from "./../../shared/models/Monitor";
import { IHotelKey } from "src/app/shared/models/Hotel";

@Injectable({
  providedIn: "root",
})
export class MonitorService {
  constructor(private http: HttpClient) {}

  getAssignedMonitors(hotelKey: IHotelKey): Observable<MonitorAssigned[]> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .get<IMonitorAssigned[]>(
        `${window.location.origin}/api/hotels/monitors/assigned`,
        { params }
      )
      .pipe(
        map((data) =>
          data.map((monitorAssigned) =>
            new MonitorAssigned().deserialize(monitorAssigned)
          )
        )
      );
  }

  getUnsassignedMonitors(hotelKey: IHotelKey): Observable<MonitorUnassigned[]> {
    const params = new HttpParams({ fromObject: { ...hotelKey } });
    return this.http
      .get<IMonitor[]>(
        `${window.location.origin}/api/hotels/monitors/unassigned`,
        { params }
      )
      .pipe(
        map((data) =>
          data.map((monitorUnassigned) =>
            new MonitorUnassigned().deserialize(monitorUnassigned)
          )
        )
      );
  }

  getMonitorHistoryByRoom(
    roomKey: IRoomKey
  ): Observable<IMonitorAssignedAlternateKey[]> {
    const params = new HttpParams({ fromObject: { ...roomKey } });
    return this.http.get<IMonitorAssignedAlternateKey[]>(
      `${window.location.origin}/api/hotels/rooms/monitors/history`,
      { params }
    );
  }

  assignMonitor(
    monitorKey: IMonitorKey,
    monitorAssigned: MonitorAssigned
  ): Observable<MonitorAssigned> {
    const params = new HttpParams({ fromObject: { ...monitorKey } });
    return this.http
      .put<IMonitorAssigned>(
        `${window.location.origin}/api/hotels/monitors/assigned`,
        monitorAssigned,
        { params }
      )
      .pipe(map((data) => new MonitorAssigned().deserialize(data)));
  }

  deassignMonitor(monitorKey: IMonitorKey): Observable<MonitorUnassigned> {
    const params = new HttpParams({ fromObject: { ...monitorKey } });
    return this.http
      .put<IMonitor>(
        `${window.location.origin}/api/hotels/monitors/unassigned`,
        {},
        { params }
      )
      .pipe(map((data) => new MonitorUnassigned().deserialize(data)));
  }
}
