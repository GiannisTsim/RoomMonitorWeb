import { IHotelKey } from "./Hotel";
import { ICompositeKey } from "../interfaces/ICompositeKey";
import { IDeserializable } from "../interfaces/IDeserializable";

export interface IHotelTagKey extends IHotelKey {
  tag: string;
}

export interface IHotelTag extends IHotelTagKey {
  description: string;
}

export class HotelTag
  implements
    IHotelTag,
    IDeserializable<IHotelTag>,
    ICompositeKey<IHotelTagKey> {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
  tag: string;
  description: string;

  deserialize(input: IHotelTag): this {
    return Object.assign(this, input);
  }

  getKey(): IHotelTagKey {
    return {
      hotelChain: this.hotelChain,
      countryCode: this.countryCode,
      town: this.town,
      suburb: this.suburb,
      tag: this.tag,
    };
  }
}
