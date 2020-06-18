import { ICompositeKey } from "./../interfaces/ICompositeKey";
import { IDeserializable } from "../interfaces/IDeserializable";

export interface IHotelKey {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
}

export interface IHotel extends IHotelKey {
  numStar: number;
}

export class Hotel
  implements IHotel, IDeserializable<IHotel>, ICompositeKey<IHotelKey> {
  hotelChain: string;
  countryCode: string;
  town: string;
  suburb: string;
  numStar: number;

  deserialize(input: IHotel): this {
    return Object.assign(this, input);
  }

  getKey() {
    return {
      hotelChain: this.hotelChain,
      countryCode: this.countryCode,
      town: this.town,
      suburb: this.suburb,
    };
  }
}
