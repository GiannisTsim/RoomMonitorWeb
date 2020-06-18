export interface ChartDataset {
  application: string;
  data: DataSeries[];
}

export interface DataPoint {
  name: Date;
  value: number | boolean;
}

export interface DataSeries {
  name: string;
  series: DataPoint[];
}
