import { TypeOfPowerSupply } from "./PowerSupply";

export interface RegisterAmbientSensorDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string
}

export interface TemperatureDTO{
  timestamp: Date,
  roomTemperature: number
}

export interface HumidityDTO{
  timestamp: Date,
  roomHumidity: number
}
