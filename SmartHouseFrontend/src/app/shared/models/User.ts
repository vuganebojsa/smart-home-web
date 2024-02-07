export interface User{
    id?:number,
    userName: string,
    name:string,
    lastName:string,
    email:string,
    password:string,
    repeatPassword:string,
    image:string
    imageType:string
}
export interface UserProfileDTO{
    username: string,
    name:string,
    lastName:string,
    email:string,
    profilePicture:string
    profilePicturePath:string
    role: Role
}
export interface EditUserDTO{
    username:string,
    name:string,
    lastName:string,
    profilePicture:string,
    typeOfImage:string,
    email?:string
}
export enum Role{
    SUPERADMIN,
    ADMIN,
    USER
}
export interface ChangePasswordDTO{
    oldPassword: string,
    newPassword: string,
    repeatNewPassword: string
}