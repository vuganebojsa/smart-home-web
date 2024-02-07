import { TypeOfPowerSupply } from "./PowerSupply";

export interface RegisterSprinklerSystemDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string
}

export interface SprinklerEventDTO{

    timestamp: Date,
    licencePlate:string,
    value:string,
    action:number
}

export interface SprinklerInfoDTO{

    isPublic:boolean,
    isOn: boolean,
    isOnline:boolean
    startTime:string,
    endTime:string
}