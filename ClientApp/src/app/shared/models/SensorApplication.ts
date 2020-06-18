export interface ISwitchApplication {
  name: string;
  description: string;
  value_0: string;
  value_1: string;
}

export interface IMeasureApplication {
  name: string;
  description: string;
  unitMeasure: string;
  limitMin?: number;
  limitMax?: number;
  defaultMin?: number;
  defaultMax?: number;
}
