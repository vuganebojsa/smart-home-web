import { timestamp } from "rxjs";
import { TypeOfPowerSupply } from "./PowerSupply";

export interface RegisterLampDTO{
    name: string,
    luminosity: number,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string
}

export interface LuminosityDTO{

    timestamp: Date,
    luminosity:number
}