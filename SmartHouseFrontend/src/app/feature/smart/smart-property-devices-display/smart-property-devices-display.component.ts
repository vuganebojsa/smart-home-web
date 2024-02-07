import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartDeviceService } from '../smart-device.service';
import { DeviceOnOffChangeDTO, DeviceStatusDTO, SmartDeviceDTO } from 'src/app/shared/models/SmartDevice';
import { SmartDeviceType } from 'src/app/shared/models/SmartDeviceType';
import { HubConnectionBuilder } from '@microsoft/signalr/dist/esm/HubConnectionBuilder';
import { SmartPropertyListDTO } from 'src/app/shared/models/SmartProperty';
import { PagedList } from 'src/app/shared/models/Pagination';
import { PageEvent } from '@angular/material/paginator';
import { HttpResponse } from '@microsoft/signalr';

@Component({
  selector: 'app-smart-property-devices-display',
  templateUrl: './smart-property-devices-display.component.html',
  styleUrls: ['./smart-property-devices-display.component.css']
})
export class SmartPropertyDevicesDisplayComponent {

  devices: SmartDeviceDTO[] = [];
  smartPropertyId:string = '';
  property: SmartPropertyListDTO;
  isLoaded = false;
  pagedList: PagedList;
  constructor(private route: ActivatedRoute, private router: Router, private smartDeviceService: SmartDeviceService) {

  }

  onPageChange(event: PageEvent): void {
    this.GetDevices(event.pageIndex + 1, event.pageSize);
  }

  ngOnInit(): void {
    this.GetIdForProperty();
    this.property = JSON.parse(localStorage.getItem('selectedProperty')) as SmartPropertyListDTO;

    this.initWebSocket();


    this.GetDevices(1, 10);
  }
  initWebSocket() {
    let connection = new HubConnectionBuilder()
    .withUrl('https://localhost:7217/hubs/status?propertyId=' + this.smartPropertyId)
    .withAutomaticReconnect()
    .build();

connection.on('ReceiveMessage', (from: string, message: string) => {
    let value: DeviceStatusDTO = JSON.parse(from);

    for (let device of this.devices) {

      if (device.id == value.Id || device.id == value.deviceId) {
        if (value.IsOnline != null){
            device.isOnline = value.IsOnline;
        }
        if (value.on!= null){

            device.isOn = value.on

        }
            break;

        }
    }
});


connection.start()
    .then(() => {
    })
    .catch(err => {
    });

  }

 trackDevices(index, device):void{
      return device ? device.id : undefined;
  }
  private GetDevices(page: number, count:number) {
    this.isLoaded = false;

    this.smartDeviceService.getDevicesByPropertyId(this.smartPropertyId, page, count).subscribe({
      next:(response) =>{

        this.devices = response.body;
        const headers = response.headers;
        const paginationData = headers.get('X-Pagination');

      if (paginationData) {
        const pagination = JSON.parse(paginationData);
        this.pagedList = {
          totalDevices: pagination.TotalCount,
          pageSize: pagination.PageSize,
          currentPage: pagination.CurrentPage,
          totalPages: pagination.TotalPages,
          hasNextPage: pagination.HasNext,
          hasPreviousPage: pagination.hasPreviousPage

        };
      }
      this.isLoaded = true;

    },
      error:(err) =>{

      }
    });
  }
  getModeEnumValue(deviceNumber: number): string {
    let deviceStr =  SmartDeviceType[deviceNumber];
    if(deviceNumber == SmartDeviceType.AirConditioner){
      deviceStr = 'Air Conditioner'
    }else if(deviceNumber == SmartDeviceType.AmbientSensor){
      deviceStr = 'Ambient Sensor'
    }else if(deviceNumber == SmartDeviceType.ElectricVehicleCharger){
      deviceStr = 'Electric Vehicle Charger'
    }else if(deviceNumber == SmartDeviceType.HouseBattery){
      deviceStr = 'House Battery'
    }else if(deviceNumber == SmartDeviceType.SolarPanelSystem){
      deviceStr = 'Solar Panel System'
    }else if(deviceNumber == SmartDeviceType.SprinklerSystem){
      deviceStr = 'Sprinkler System'
    }
    else if(deviceNumber == SmartDeviceType.VehicleGate){
      deviceStr = 'Vehicle Gate'
    }
    else if(deviceNumber == SmartDeviceType.WashingMachine){
      deviceStr = 'Washing Machine'
    }
    return deviceStr
  }
  goBack(){
    this.router.navigate(['']);
  }
  addDevice(){
    this.router.navigate(['smart', this.smartPropertyId, 'register-device']);

  }
  showConsumption(){
    //localStorage.setItem('smartPropertyId', this.smartPropertyId);
    this.router.navigate(['smart', this.smartPropertyId, 'energy-consumption']);
  }
  showSelectedDevice(device: SmartDeviceDTO){
      localStorage.setItem('selectedDevice', JSON.stringify(device));
      if (device.smartDeviceType==3){
      this.router.navigate(['smart', device.id, 'lamp-device']);
      }else if(device.smartDeviceType == 6){
        this.router.navigate(['smart', device.id, 'sps-device']);

      }else if(device.smartDeviceType == 7){
        this.router.navigate(['smart', device.id, 'battery']);

      }
      else if(device.smartDeviceType == 4){
        this.router.navigate(['smart', device.id, 'gate-device']);
      }

      else if(device.smartDeviceType == 0){
        this.router.navigate(['smart', device.id, 'ambient-sensor-device'])
      }
      else if(device.smartDeviceType == 5){
        this.router.navigate(['smart', device.id, 'sprinkler-device'])
      }
      else if(device.smartDeviceType == 8){
        this.router.navigate(['smart', device.id, 'vehicle-charger'])
      }


  }


  private GetIdForProperty() {
     this.route.params.subscribe(params => {
      this.smartPropertyId = params['id'];
    });
  }
  turnOnOff(deviceId: string, newStatus:boolean){
    this.smartDeviceService.turnOnOffDevice(deviceId, newStatus).subscribe({
      next:(result) =>{
          for(let item of this.devices){
            if(item.id == deviceId){
              item.isOn = result;
              break;
            }
          }
      },
      error:(err) =>{

      }
    })
  }
}
