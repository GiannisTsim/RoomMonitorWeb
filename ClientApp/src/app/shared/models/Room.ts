import { IDeserializable } from "../interfaces/IDeserializable";
import { ICompositeKey } from "../interfaces/ICompositeKey";
import { IHotelKey } from "src/app/shared/models/Hotel";

export interface IRoomKey extends IHotelKey {
  roomType: string;
  name: string;
}

export interface IRoom extends IRoomKey {
  tags: string[];
}

export class Room
  implements IRoom, IDeserializable<IRoom>, ICompositeKey<IRoomKey> {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
  roomType: string;
  name: string;
  tags: string[];

  deserialize(input: IRoom): this {
    return Object.assign(this, input);
  }

  getKey(): IRoomKey {
    return {
      hotelChain: this.hotelChain,
      countryCode: this.countryCode,
      town: this.town,
      suburb: this.suburb,
      roomType: this.roomType,
      name: this.name,
    };
  }
}
