import { TypeOfPowerSupply } from "./PowerSupply";

export interface RegisterHouseBatteryDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string,
    batterySize: number

}