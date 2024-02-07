import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterAirConditionerDTO } from 'src/app/shared/models/AirConditioner';
import {HumidityDTO, RegisterAmbientSensorDTO, TemperatureDTO} from 'src/app/shared/models/AmbientSensor';
import { RegisterHouseBatteryDTO } from 'src/app/shared/models/HouseBattery';
import { LuminosityDTO, RegisterLampDTO } from 'src/app/shared/models/Lamp';
import { DeviceOnOffDTO, DeviceOnlineOfflineReportDTO, RegisterSmartDeviceDTO, SmartDeviceDTO } from 'src/app/shared/models/SmartDevice';
import { RegisterSprinklerSystemDTO, SprinklerEventDTO, SprinklerInfoDTO } from 'src/app/shared/models/SprinklerSystem';
import { Cycle, RegisterWashingMachineDTO } from 'src/app/shared/models/WashingMachine';
import { environment } from 'src/environment/environment';
import { RegisterElectricVehicleChargetComponent } from './register-electric-vehicle-charget/register-electric-vehicle-charget.component';
import { RegisterElectricVehicleChargerDTO } from 'src/app/shared/models/ElectricVehicleCharger';

import { GateEventDTO, GateInfoDTO, GatePublicPrivateDTO } from 'src/app/shared/models/VehicleGate';
import { PanelDTO, RegisterSolarPanelSystemDTO, SPSAction, SpsDTO } from 'src/app/shared/models/SolarPanelSystem';
import { BatteryDTO, EnergyConsumptionDTO, TotalTimePeriod } from 'src/app/shared/models/Battery';
import { VehicleChargerActionsDTO, VehicleChargerAllActionsDTO, VehicleChargerDTO } from 'src/app/shared/models/VehicleCharger';
import { EnergyDTO } from 'src/app/shared/models/Energy';

@Injectable({
  providedIn: 'root'
})
export class SmartDeviceService {

  smartDeviceUrl: string = environment.apiHost + 'smartDevices';
  reportUrl: string = environment.apiHost + 'reports';
  constructor(private http: HttpClient) { }


  registerLamp(lamp: RegisterLampDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-lamp', lamp);
  }
  registerAmbientSensor(sensor: RegisterAmbientSensorDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-ambient-sensor', sensor);
  }
  registerSprinklerSystem(sprinkler: RegisterSprinklerSystemDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-sprinkler-system', sprinkler);
  }

  registerAirConditioner(ac: RegisterAirConditionerDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-air-conditioner', ac);
  }
  registerWashingMachine(wm: RegisterWashingMachineDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-washing-machine', wm);
  }
  registerVehicleGate(device: RegisterSmartDeviceDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-vehicle-gate', device);
  }
  registerBattery(device: RegisterHouseBatteryDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-house-battery', device);
  }

  registerElectricVehicleCharger(device: RegisterElectricVehicleChargerDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-electric-vehicle-charger', device);
  }
  registerSolarPanelSystem(device: RegisterSolarPanelSystemDTO): Observable<SmartDeviceDTO>{
    return this.http.post<SmartDeviceDTO>(this.smartDeviceUrl + '/register-solar-panel-system', device);
  }
  getWashingMachineCycles(): Observable<Cycle[]>{
    return this.http.get<Cycle[]>(this.smartDeviceUrl + '/washing-machine-cycles');
  }

  getDevicesByPropertyId(propertyId:string, page:number, count:number): Observable<HttpResponse<SmartDeviceDTO[]>>{
    return this.http.get<SmartDeviceDTO[]>(this.smartDeviceUrl + '/' + propertyId + '/devices?PageNumber=' + page +'&PageSize=' + count, { observe: 'response' });
  }
  //turn-on-off/{id}
  turnOnOffDevice(deviceId:string, newStatus: boolean): Observable<boolean>{
    return this.http.put<boolean>(this.smartDeviceUrl + '/turn-on-off/' + deviceId, {
      'isOn': newStatus
    });
  }
  getLampLuminosityReport(id:string, from: string, to:string):Observable<LuminosityDTO[]>{
    return this.http.get<LuminosityDTO[]>(`${environment.apiHost}reports/getLampLuminosityHistory/${id}?startDate=${from}&endDate=${to}`);
  }
  getGateEventReport(id:string, from: string, to:string, licencePlate:string | null):Observable<GateEventDTO[]>{
    if (licencePlate == null){
      return this.http.get<GateEventDTO[]>(`${environment.apiHost}reports/getGateEventHistory/${id}?startDate=${from}&endDate=${to}`);
    }
    return this.http.get<GateEventDTO[]>(`${environment.apiHost}reports/getGateEventHistory/${id}?startDate=${from}&endDate=${to}&licencePlate=${licencePlate}`);
  }
  getGatePublicPrivateReport(id:string, from: string, to:string):Observable<GatePublicPrivateDTO[]>{
    return this.http.get<GatePublicPrivateDTO[]>(`${environment.apiHost}reports/get-gate-public-private-history/${id}?startDate=${from}&endDate=${to}`);
  }

  getDeviceOnOffReport(id:string, from: string, to:string):Observable<DeviceOnOffDTO[]>{
    return this.http.get<DeviceOnOffDTO[]>(`${environment.apiHost}reports/get-device-on-off-history/${id}?startDate=${from}&endDate=${to}`);
  }

  getDeviceOnlineOfflineReport(id:string, from: string, to:string):Observable<DeviceOnlineOfflineReportDTO>{
    return this.http.get<DeviceOnlineOfflineReportDTO>(`${environment.apiHost}reports/get-device-online-offline-history/${id}?startDate=${from}&endDate=${to}`);
  }

  getGateInfo(id:string):Observable<GateInfoDTO>{
    return this.http.get<GateInfoDTO>(this.smartDeviceUrl + '/gate-info/' + id)
  }

  getSprinklerInfo(id:string):Observable<SprinklerInfoDTO>{
    return this.http.get<SprinklerInfoDTO>(this.smartDeviceUrl + '/sprinkler-info/' + id)
  }

  getSprinklerEventReport(id:string, from: string, to:string, username:string | null):Observable<SprinklerEventDTO[]>{
    if (username == null){
      return this.http.get<SprinklerEventDTO[]>(`${environment.apiHost}reports/getSprinklerEventHistory/${id}?startDate=${from}&endDate=${to}`);
    }
    return this.http.get<SprinklerEventDTO[]>(`${environment.apiHost}reports/getSprinklerEventHistory/${id}?startDate=${from}&endDate=${to}&username=${username}`);
  }

  turnPublicPrivateGate(deviceId:string, newStatus: boolean): Observable<boolean>{
    return this.http.put<boolean>(this.smartDeviceUrl + '/turn-gate-public-private/' + deviceId, {
      'isPublic': newStatus
    });
  }

  changeSpecialModeSprinkler(deviceId:string, newStatus: boolean): Observable<boolean>{
    return this.http.put<boolean>(this.smartDeviceUrl + '/change-sprinkle-special-state/' + deviceId, {
      'isActive': newStatus
    });
  }

  getValidLicencePlates(id:string): Observable<string[]>{
    return this.http.get<string[]>(this.smartDeviceUrl + '/licence-plates/' + id);
  }

  getActiveDays(id:string): Observable<string[]>{
    return this.http.get<string[]>(this.smartDeviceUrl + '/active-days/' + id);
  }

  registerLicencePlate(id:string, licencePlate: string): Observable<string>{
    return this.http.put<string>(this.smartDeviceUrl + '/register-licence-plate/' + id, {
      'plate': licencePlate
    });
  }

  removeLicencePlate(id: string, licencePlate: string): Observable<string> {
    return this.http.put<string>(`${this.smartDeviceUrl}/remove-licence-plate/${id}`, {
      'plate': licencePlate
    });
  }

 

  addSprinklerDay(id:string, day: string): Observable<string>{
    return this.http.put<string>(this.smartDeviceUrl + '/add-sprinkler-day/' + id, {
      'day': day
    });
  }


  removeSprinklerDay(id: string, day: string): Observable<string> {
    return this.http.put<string>(`${this.smartDeviceUrl}/remove-sprinkler-day/${id}`, {
      'day': day
    });
  }

  changeSprinklerStartTime(id:string, time: string): Observable<string>{
    return this.http.put<string>(this.smartDeviceUrl + '/change-sprinkler-start-time/' + id, {
      'StartTime': time
    });
  }

  changeSprinklerEndTime(id:string, time: string): Observable<string>{
    return this.http.put<string>(this.smartDeviceUrl + '/change-sprinkler-end-time/' + id, {
      'StartTime': time
    });
  }

  getSolarPanelSystemReport(deviceId:string,from: string, to: string, username: string): Observable<SPSAction[]>{
    let path = '/solar-panel-history';
    let query = '';
    if(username.trim() != '' && from.trim() != '' && to.trim() != ''){
      path += '/';
      query = '?from=' + from + '&to=' + to + '&username=' + username;

    }else if(username.trim() != '' && from.trim() == '' && to.trim() == ''){
      path += '-username/';
      query = '?username=' + username;

    }else if(username.trim() != '' && from.trim() != '' && to.trim() == ''){
      path += '-from-username/';
      query = '?username=' + username + '&from=' + from;
    }else if(username.trim() != '' && from.trim() == '' && to.trim() != ''){
      path += '-to-username/';
      query = '?username=' + username + '&to=' + to;
    }
    else if(username.trim() == '' && from.trim() != '' && to.trim() != ''){
      path += '-from-to/';
      query = '?from=' + from + '&to=' + to;
    }
    else if(username.trim() == '' && from.trim() == '' && to.trim() != ''){
      path += '-to/';
      query = '?to=' + to;
    }
    else if(username.trim() == '' && from.trim() != '' && to.trim() == ''){
      path += '-from/';
      query = '?from=' + from;
    }

    return this.http.get<SPSAction[]>(this.reportUrl + path + deviceId + query);
  }

  getSmartPropertyConsuptionLastHour(smartPropertyId: string): Observable<EnergyConsumptionDTO[]>{
    return this.http.get<EnergyConsumptionDTO[]>(this.reportUrl + '/property-energy-consumption/' + smartPropertyId);
  }
  getSmartPropertyConsuptionFromTo(smartPropertyId: string, from: string, to:string): Observable<EnergyConsumptionDTO[]>{
    return this.http.get<EnergyConsumptionDTO[]>(this.reportUrl + '/property-energy-consumption-from-to/' + smartPropertyId + '?from=' + from + '&to=' + to);
  }
  getSmartPropertyConsuptionInTimePeriod(smartPropertyId: string, timePeriod: TotalTimePeriod): Observable<EnergyConsumptionDTO[]>{
    return this.http.get<EnergyConsumptionDTO[]>(this.reportUrl + '/property-energy-consumption-time-period/' + smartPropertyId + '/' + timePeriod);
  }

  getBattery(batteryId: string): Observable<BatteryDTO>{
    return this.http.get<BatteryDTO>(this.smartDeviceUrl + '/battery/' + batteryId);
  }
  getSPS(panelId: string): Observable<SpsDTO>{
    return this.http.get<SpsDTO>(this.smartDeviceUrl + '/solar-panel-system/' + panelId);
  }

  getAmbientSensorTemperatureLastHour(deviceId: string):Observable<TemperatureDTO[]> {
    return this.http.get<TemperatureDTO[]>(this.reportUrl + '/temperature-data/' + deviceId)
  }

  getAmbientSensorTemperatureFromTo(deviceId: string, from: string, to: string): Observable<TemperatureDTO[]> {
    return this.http.get<TemperatureDTO[]>(this.reportUrl + '/temperature-data-from-to/' + deviceId + '?from=' + from + '&to=' + to);
  }

  getAmbientSensorTemperatureInTimePeriod(deviceId: string, timePeriod: TotalTimePeriod): Observable<TemperatureDTO[]> {
    return this.http.get<TemperatureDTO[]>(this.reportUrl + '/temperature-data-time-period/' + deviceId + '/' + timePeriod);
  }

  getAmbientSensorHumidityLastHour(deviceId: string) :Observable<HumidityDTO[]> {
    return this.http.get<HumidityDTO[]>(this.reportUrl + '/humidity-data/' + deviceId)
  }

  getAmbientSensorHumidityFromTo(deviceId: string, from: string, to: string): Observable<HumidityDTO[]> {
    return this.http.get<HumidityDTO[]>(this.reportUrl + '/humidity-data-from-to/' + deviceId + '?from=' + from + '&to=' + to);
  }

  getAmbientSensorHumidityInTimePeriod(deviceId: string, timePeriod: TotalTimePeriod): Observable<HumidityDTO[]> {
    return this.http.get<HumidityDTO[]>(this.reportUrl + '/humidity-data-time-period/' + deviceId + '/' + timePeriod);
  }
  getVehicleCharger(vehicleChargerId: string): Observable<VehicleChargerDTO>{
    return this.http.get<VehicleChargerDTO>(this.smartDeviceUrl + '/vehicle-charger/' + vehicleChargerId);
  }
  setChargePercentage(percentageOfCharge:number, vehicleChargerId: string): Observable<number>{
    return this.http.put<number>(this.smartDeviceUrl + '/vehicle-charge/' + vehicleChargerId, {'percentageOfCharge': percentageOfCharge});
  }
  getVehicleChargerHistory(vehicleChargerId: string): Observable<VehicleChargerActionsDTO[]>{
    return this.http.get<VehicleChargerActionsDTO[]>(this.reportUrl + '/vehicle-charger-history/' + vehicleChargerId);
  }
  getVehicleChargerHistoryInRange(deviceId:string,from: string, to: string, username: string): Observable<VehicleChargerAllActionsDTO[]>{
    let path = '/vehicle-charger-history-in-range';
    let query = '';
    if(username === ''){
      path += '/';
      query += '?from=' + from + '&to=' + to;
    }else{
      path += '-with-executer/';
      query += '?from=' + from + '&to=' + to + '&executer=' + username;
    }
    
    return this.http.get<VehicleChargerAllActionsDTO[]>(this.reportUrl + path + deviceId + query);
  }
  getEnergyConsumedInCityByTimePeriod(cityName:string, timePeriod: TotalTimePeriod, isConsumed: boolean): Observable<EnergyDTO[]> {
    return this.http.get<EnergyDTO[]>(this.reportUrl + '/energy-consumed-city/' + cityName + '/' + isConsumed + '/' + timePeriod);
  }
  getEnergyConsumedInCityInRange(cityName:string, from:string, to:string, isConsumed: boolean): Observable<EnergyDTO[]> {
    return this.http.get<EnergyDTO[]>(this.reportUrl + '/energy-consumed-city-in-range/' + cityName + '/' + isConsumed + '?from=' + from + '&to=' + to);
  }
  getEnergyConsumedInPropertyByTimePeriod(propertyId:string, timePeriod: TotalTimePeriod, isConsumed: boolean): Observable<EnergyDTO[]> {
    return this.http.get<EnergyDTO[]>(this.reportUrl + '/energy-consumed-property/' + propertyId + '/' + isConsumed + '/' + timePeriod);
  }
  getEnergyConsumedInPropertyInRange(propertyId:string, from:string, to:string, isConsumed: boolean): Observable<EnergyDTO[]> {
    return this.http.get<EnergyDTO[]>(this.reportUrl + '/energy-consumed-property-in-range/' + propertyId + '/' + isConsumed + '?from=' + from + '&to=' + to);
  }

  getPicture(imagePath: string): Observable<Blob> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
  
    return this.http.get('http://localhost/' + imagePath, {
      headers: headers,
      responseType: 'blob',
      withCredentials: true  // Ensure credentials are sent with the request
    });
  }
}
