export interface VehicleChargerDTO{
    id?:string,
    name:string,
    pathToImage:string,
    isOnline:boolean,
    isOn:boolean,
    numberOfConnections:number,
    power:number,
    percentageOfCharge:number
}

export interface VehicleChargerActionsDTO{
    timeStamp:Date,
    username:string,
    value:number
}
export interface VehicleChargerAllActionsDTO{
    timeStamp:Date,
    executer:string,
    value:string,
    action:string
}