import { IDeserializable } from "../interfaces/IDeserializable";
import { ICompositeKey } from "../interfaces/ICompositeKey";
import { IHotelKey } from "src/app/shared/models/Hotel";

export interface IMonitorAssignedAlternateKey extends IHotelKey {
  roomType: string;
  room: string;
  monitor: string;
}

export interface IMonitorKey extends IHotelKey {
  MACAddress: string;
}

export interface IMonitor extends IMonitorKey {
  monitorType: string;
  configurationType: string;
  manufacturer: string;
  model: string;
  SWVersion: string;
  SWUpdateDtm: string;
  RegistrationDtm: string;
  RegistrationInfo: string;
}

export interface IMonitorAssigned extends IMonitor {
  roomType: string;
  room: string;
  monitor: string;
  placementDtm: string;
}

export class MonitorAssigned
  implements
    IMonitorAssigned,
    IDeserializable<IMonitorAssigned>,
    ICompositeKey<IMonitorKey> {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
  MACAddress: string;
  monitorType: string;
  configurationType: string;
  manufacturer: string;
  model: string;
  SWVersion: string;
  SWUpdateDtm: string;
  RegistrationDtm: string;
  RegistrationInfo: string;

  roomType: string;
  room: string;
  monitor: string;
  placementDtm: string;

  deserialize(input: IMonitorAssigned): this {
    return Object.assign(this, input);
  }

  getKey(): IMonitorKey {
    return {
      hotelChain: this.hotelChain,
      countryCode: this.countryCode,
      town: this.town,
      suburb: this.suburb,
      MACAddress: this.MACAddress,
    };
  }

  getAlternateKey(): IMonitorAssignedAlternateKey {
    return {
      hotelChain: this.hotelChain,
      countryCode: this.countryCode,
      town: this.town,
      suburb: this.suburb,
      roomType: this.roomType,
      room: this.room,
      monitor: this.monitor,
    };
  }
}

export class MonitorUnassigned
  implements IMonitor, IDeserializable<IMonitor>, ICompositeKey<IMonitorKey> {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
  MACAddress: string;
  monitorType: string;
  configurationType: string;
  manufacturer: string;
  model: string;
  SWVersion: string;
  SWUpdateDtm: string;
  RegistrationDtm: string;
  RegistrationInfo: string;

  deserialize(input: IMonitor): this {
    return Object.assign(this, input);
  }

  getKey(): IMonitorKey {
    return {
      hotelChain: this.hotelChain,
      countryCode: this.countryCode,
      town: this.town,
      suburb: this.suburb,
      MACAddress: this.MACAddress,
    };
  }
}
