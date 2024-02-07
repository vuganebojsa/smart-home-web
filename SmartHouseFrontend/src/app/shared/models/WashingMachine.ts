import { TypeOfPowerSupply } from "./PowerSupply";

export enum CycleName{
    Cotton,
    EcoWash,
    QuickWash,
    Synthetics,
    Delicate,
    Wool
}
export interface Cycle{
    id?:string,
    name:CycleName,
    duration:number,
    temperature:number,
}
export interface RegisterWashingMachineDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string,
    supportedCycles: string[]
}