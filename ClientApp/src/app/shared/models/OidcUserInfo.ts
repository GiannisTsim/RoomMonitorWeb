import { Hotel } from "src/app/shared/models/Hotel";
import { UserRole } from "./ApplicationUser";

export interface OidcUserInfo {
  sub: string;
  email: string;
  role: UserRole;
  hotel?: string; // JSON string
}
