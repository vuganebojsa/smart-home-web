import { TypeOfPowerSupply } from "./PowerSupply";
import { SmartDeviceType } from "./SmartDeviceType";

export interface SmartDeviceDTO{
    id:string,
    name: string,
    pathToImage: string,
    smartPropertyId:string,
    smartDeviceType?: SmartDeviceType,
    isOnline?:boolean,
    isOn?:boolean
}

export interface RegisterSmartDeviceDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string
}
export interface DeviceStatusDTO{
    Id?:string,
    IsOnline?:boolean,
    on?:boolean
    deviceId:string
}
export interface DeviceOnOffChangeDTO{
    on:boolean,
    deviceId:string
}

export interface DeviceOnOffDTO{
    timestamp: Date,
    isOn:number
}

export interface DeviceOnlineOfflineReportDTO{
    totalTimeOnline:number,
    totalTimeOffline:number,
    percentageOnline:number,
    onlineMap:Map<string, number>,
    offlineMap:Map<string, number>,
}