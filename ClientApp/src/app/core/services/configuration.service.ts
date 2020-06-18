import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import {
  ISwitchApplication,
  IMeasureApplication,
} from "./../../shared/models/SensorApplication";
import { IConfigurationType } from "./../../shared/models/ConfigurationType";

@Injectable({
  providedIn: "root",
})
export class ConfigurationService {
  constructor(private http: HttpClient) {}

  getConfigurationTypes() {
    return this.http.get<IConfigurationType[]>(
      `${window.location.origin}/api/configuration-types`
    );
  }

  getMeasureApplications() {
    return this.http.get<IMeasureApplication[]>(
      `${window.location.origin}/api/applications/measure`
    );
  }

  getSwitchApplications() {
    return this.http.get<ISwitchApplication[]>(
      `${window.location.origin}/api/applications/switch`
    );
  }
}
