import { TypeOfPowerSupply } from "./PowerSupply";
export enum Mode{
    Cooling,
    Heating,
    Automatic,
    Ventilation
}
export interface RegisterAirConditionerDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string,
    minTemperature:number,
    maxTemperature:number,
    modes: Mode[]
}