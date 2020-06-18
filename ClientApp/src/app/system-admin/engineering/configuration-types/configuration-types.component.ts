import { Component, OnInit } from "@angular/core";

import { ConfigurationService } from "./../../../core/services/configuration.service";
import { IConfigurationType } from "src/app/shared/models/ConfigurationType";

@Component({
  selector: "app-configuration-types",
  templateUrl: "./configuration-types.component.html",
  styleUrls: ["./configuration-types.component.scss"],
})
export class ConfigurationTypesComponent implements OnInit {
  configurationTypes: IConfigurationType[] = [];

  constructor(private configurationService: ConfigurationService) {}

  ngOnInit(): void {
    this.configurationService
      .getConfigurationTypes()
      .subscribe((types) => (this.configurationTypes = types));
  }

  openConfigurationTypeDialog() {}
}
