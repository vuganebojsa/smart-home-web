export enum TotalTimePeriod{
    SIX_HOURS,
    TWELVE_HOURS,
    TWENTY_FOUR_HOURS,
    LAST_WEEK,
    LAST_MONTH
}

export interface EnergyConsumptionDTO{
    timestamp: Date,
    consumed: number
}

export interface BatteryDTO{
    id?:string,
    name:string,
    pathToImage:string,
    isOnline:boolean,
    isOn:boolean,
    batterySize: number,
    occupationLevel:number
}