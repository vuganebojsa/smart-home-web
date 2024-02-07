import { TypeOfPowerSupply } from "./PowerSupply";

export interface RegisterElectricVehicleChargerDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string,
    power:number,
    numberOfConnections:number
}