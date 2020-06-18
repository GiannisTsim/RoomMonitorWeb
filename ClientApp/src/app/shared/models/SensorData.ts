import { IDeserializable } from "../interfaces/IDeserializable";
import { DataSeries, DataPoint } from "./ChartDataset";

export interface ISensorData {
  readingDtm: string;
  value: number | boolean;
}

export interface IApplicationData {
  application: string;
  sensorType: string;
  sensorData: ISensorData[];
}

export interface IMonitorData {
  monitor: string;
  applicationMeasureData: IApplicationData[];
  applicationSwitchData: IApplicationData[];
}

export interface IRoomData {
  roomType: string;
  room: string;
  monitorData: IMonitorData[];
}

export interface IHotelData {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
  roomData: IRoomData[];
}

abstract class SensorData implements IDeserializable<ISensorData> {
  readingDtm: Date;
  abstract value: number | boolean;
  abstract deserialize(input: ISensorData): this;
}

export class SensorMeasureData extends SensorData {
  value: number;
  deserialize(input: ISensorData): this {
    return Object.assign(this, {
      readingDtm: new Date(input.readingDtm),
      value: input.value,
    });
  }
}

export class SensorSwitchData extends SensorData {
  value: boolean;
  deserialize(input: ISensorData): this {
    return Object.assign(this, {
      readingDtm: new Date(input.readingDtm),
      value: input.value,
    });
  }
}

abstract class ApplicationData implements IDeserializable<IApplicationData> {
  application: string;
  sensorType: string;
  abstract deserialize(input: IApplicationData): this;
}

export class ApplicationMeasureData extends ApplicationData {
  sensorData: SensorMeasureData[];
  deserialize(input: IApplicationData): this {
    Object.assign(this, input);
    this.sensorData = input.sensorData.map((data) =>
      new SensorMeasureData().deserialize(data)
    );
    return this;
  }
}

export class ApplicationSwitchData extends ApplicationData {
  sensorData: SensorSwitchData[];
  deserialize(input: IApplicationData): this {
    Object.assign(this, input);
    this.sensorData = input.sensorData.map((data) =>
      new SensorSwitchData().deserialize(data)
    );
    return this;
  }
}

export class MonitorData implements IDeserializable<IMonitorData> {
  monitor: string;
  applicationMeasureData: ApplicationMeasureData[];
  applicationSwitchData: ApplicationSwitchData[];
  deserialize(input: IMonitorData): this {
    Object.assign(this, input);
    this.applicationMeasureData = input.applicationMeasureData.map((data) =>
      new ApplicationMeasureData().deserialize(data)
    );
    this.applicationSwitchData = input.applicationSwitchData.map((data) =>
      new ApplicationSwitchData().deserialize(data)
    );
    return this;
  }

  buildChartMap(
    seriesName: string = "",
    chartMap: Map<string, DataSeries[]> = new Map<string, DataSeries[]>()
  ): Map<string, DataSeries[]> {
    seriesName += `${this.monitor}`;
    this.applicationMeasureData.forEach((applicationMeasure) => {
      const dataPoints: DataPoint[] = [];
      applicationMeasure.sensorData.forEach((sensor) => {
        dataPoints.push({ name: sensor.readingDtm, value: sensor.value });
      });
      if (!chartMap.has(applicationMeasure.application)) {
        chartMap.set(applicationMeasure.application, []);
      }
      chartMap
        .get(applicationMeasure.application)
        .push({ name: seriesName, series: dataPoints });
    });

    this.applicationSwitchData.forEach((applicationSwitch) => {
      const dataPoints: DataPoint[] = [];
      applicationSwitch.sensorData.forEach((sensor) => {
        dataPoints.push({ name: sensor.readingDtm, value: sensor.value });
      });
      if (!chartMap.has(applicationSwitch.application)) {
        chartMap.set(applicationSwitch.application, []);
      }
      chartMap
        .get(applicationSwitch.application)
        .push({ name: seriesName, series: dataPoints });
    });

    return chartMap;
  }
}

export class RoomData implements IDeserializable<IRoomData> {
  roomType: string;
  room: string;
  monitorData: MonitorData[];
  deserialize(input: IRoomData): this {
    Object.assign(this, input);
    this.monitorData = input.monitorData.map((data) =>
      new MonitorData().deserialize(data)
    );
    return this;
  }

  buildChartMap(
    seriesName: string = "",
    chartMap: Map<string, DataSeries[]> = new Map<string, DataSeries[]>()
  ): Map<string, DataSeries[]> {
    seriesName += `${this.roomType},${this.room} - `;
    this.monitorData.forEach((monitor) => {
      chartMap = monitor.buildChartMap(seriesName, chartMap);
    });
    return chartMap;
  }
}

export class HotelData implements IDeserializable<IHotelData> {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
  roomData: RoomData[];
  deserialize(input: IHotelData): this {
    Object.assign(this, input);
    this.roomData = input.roomData.map((data) =>
      new RoomData().deserialize(data)
    );
    return this;
  }

  buildChartMap(
    seriesName: string = "",
    chartMap: Map<string, DataSeries[]> = new Map<string, DataSeries[]>()
  ): Map<string, DataSeries[]> {
    seriesName += `${this.hotelChain},${this.countryCode},${this.town},${this.suburb} - `;
    this.roomData.forEach((room) => {
      chartMap = room.buildChartMap(seriesName, chartMap);
    });
    return chartMap;
  }
}
