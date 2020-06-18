import { Observable } from "rxjs";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs/operators";

import { IHotelKey } from "./../../shared/models/Hotel";
import { IRoomKey } from "./../../shared/models/Room";
import { IMonitorAssignedAlternateKey } from "./../../shared/models/Monitor";
import {
  HotelData,
  IHotelData,
  RoomData,
  MonitorData,
  IRoomData,
  IMonitorData,
} from "./../../shared/models/SensorData";
import { ChartDataset, DataSeries } from "src/app/shared/models/ChartDataset";

@Injectable({
  providedIn: "root",
})
export class SensorDataService {
  constructor(private http: HttpClient) {}

  getHotelReadings(hotelKey: IHotelKey): Observable<HotelData> {
    const params = new HttpParams({
      fromObject: { ...hotelKey },
    });
    return this.http
      .get<IHotelData>(`${window.location.origin}/api/hotels/readings`, {
        params,
      })
      .pipe(map((hotelData) => new HotelData().deserialize(hotelData)));
  }

  getRoomReadings(roomKey: IRoomKey): Observable<RoomData> {
    const params = new HttpParams({
      fromObject: { ...roomKey },
    });
    return this.http
      .get<IRoomData>(`${window.location.origin}/api/hotels/rooms/readings`, {
        params,
      })
      .pipe(map((roomData) => new RoomData().deserialize(roomData)));
  }

  getMonitorReadings(
    monitorKey: IMonitorAssignedAlternateKey
  ): Observable<MonitorData> {
    const params = new HttpParams({
      fromObject: { ...monitorKey },
    });
    return this.http
      .get<IMonitorData>(
        `${window.location.origin}/api/hotels/rooms/monitors/readings`,
        { params }
      )
      .pipe(map((monitorData) => new MonitorData().deserialize(monitorData)));
  }

  formatToChartData(
    baseData: HotelData[] | RoomData[] | MonitorData[]
  ): ChartDataset[] {
    let chartMap: Map<string, DataSeries[]>;
    const chartDatasets: ChartDataset[] = [];

    baseData.forEach(
      (d: HotelData | RoomData | MonitorData) =>
        (chartMap = d.buildChartMap("", chartMap))
    );
    for (const [application, data] of chartMap) {
      chartDatasets.push({ application, data });
    }

    return chartDatasets;
  }
}
