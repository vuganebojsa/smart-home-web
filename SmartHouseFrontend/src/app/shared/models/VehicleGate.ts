import { TypeOfPowerSupply } from "./PowerSupply";

export interface RegisterVehicleGateDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string,
}

export interface GateEventDTO{

    timestamp: Date,
    licencePlate:string,
    action:number
}

export interface GatePublicPrivateDTO{

    timestamp: Date,
    isPublic:number
}

export interface GateInfoDTO{

    
    isPublic:boolean,
    isOn: boolean,
    isOnline:boolean
}