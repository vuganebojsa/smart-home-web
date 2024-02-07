import { TypeOfProperty } from "./TypeOfProperty";
import { Activation } from "./Activation";
import { SmartDeviceDTO } from "./SmartDevice";


export interface SmartPropertyListDTO{
    id:string,
    typeOfProperty: TypeOfProperty,
    name: string,
    city:string,
    country:string,
    address: string,
    isAccepted:Activation
    
}
export interface SmartPropertyWithUserDTO{
    id:string,
    typeOfProperty: TypeOfProperty,
    name: string,
    city:string,
    country:string,
    address: string,
    ownerName?:string,

}
export interface PendingSmartPropertyListDTO{
    id:string,
    typeOfProperty: TypeOfProperty,
    name: string,
    address: string,
    city: string,
    country: string,
    quadrature:number,
    numberOfFloors:number,
    userName:string
}

export interface SinglePropertyDTO{
        typeOfProperty:TypeOfProperty;
        name:string,
        address:string,
        city:string,
        country:string,
        quadrature:number,
        numberOfFloors:number,
        longitude:number,
        latitude:number,
        image:string,
        imageType:string
        devices:SmartDeviceDTO[]
}

export interface SmartPropertyRegisterDTO{
        typeOfProperty:TypeOfProperty;
        name:string,
        address:string,
        city:string,
        country:string,
        quadrature:number,
        numberOfFloors:number,
        longitude:number,
        latitude:number,
        image:string,
        imageType:string
}
export interface ProccesRequest{
    Accept:boolean,
    Reason?:string
}