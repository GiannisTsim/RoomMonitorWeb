import { IDeserializable } from "./../interfaces/IDeserializable";
import { IHotelKey } from "src/app/shared/models/Hotel";

export enum UserRole {
  SYSTEM_ADMIN = "System Administrator",
  HOTEL_ADMIN = "Hotel Administrator",
  HOTEL_EMPLOYEE = "Hotel Employee",
}

export interface IApplicationUser {
  userId: string;
  email: string;
  role: UserRole;
  hotelChain?: string;
  countryCode?: string;
  town?: string;
  suburb?: string;
}

export interface UserInvitation {
  email: string;
  role: UserRole;
}

export class HotelUser implements IDeserializable<IApplicationUser> {
  userId: string;
  email: string;
  role: UserRole;
  hotel: IHotelKey;

  deserialize(input: IApplicationUser): this {
    Object.assign(this, input);
    this.hotel = {
      hotelChain: input.hotelChain,
      countryCode: input.countryCode,
      town: input.town,
      suburb: input.suburb,
    };
    return this;
  }
}
