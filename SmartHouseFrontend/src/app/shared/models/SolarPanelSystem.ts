import { TypeOfPowerSupply } from "./PowerSupply";
export interface PanelDTO{
    size: number,
    efficency: number
}

export interface SpsDTO{
    id?:string,
    name:string,
    pathToImage:string,
    isOnline:boolean,
    isOn:boolean,
    panels?: PanelDTO[]
}

export interface RegisterSolarPanelSystemDTO{
    name: string,
    powerSupply: TypeOfPowerSupply,
    powerUsage?:number,
    smartPropertyId: string,
    image: string,
    imageType:string,
    panels: PanelDTO[]
}

export interface SPSAction{
    timeStamp: Date,
    isOn: boolean,
    username:string
}