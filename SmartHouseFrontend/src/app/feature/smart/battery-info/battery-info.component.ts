import { Component, OnInit } from '@angular/core';
import { SmartDeviceService } from '../smart-device.service';
import { ActivatedRoute, Router } from '@angular/router';
import { BatteryDTO } from 'src/app/shared/models/Battery';
import { Location } from '@angular/common';
import { DeviceStatusDTO } from 'src/app/shared/models/SmartDevice';
import { HubConnectionBuilder } from '@microsoft/signalr';

@Component({
  selector: 'app-battery-info',
  templateUrl: './battery-info.component.html',
  styleUrls: ['./battery-info.component.css']
})
export class BatteryInfoComponent implements OnInit {
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.batteryId = params['id'];
    });
    this.smartPropertyId = localStorage.getItem('smartPropertyId');

    this.service.getBattery(this.batteryId).subscribe({
      next: (battery) => {
        this.battery = battery;
        this.battery.occupationLevel =  Math.round(this.battery.occupationLevel * 100) / 100;
        this.hasError = false;
        this.service.getPicture(this.battery.pathToImage).subscribe(result =>{
            const url = URL.createObjectURL(result);
            (document.getElementById('profilna') as HTMLImageElement).src = url;
        });

        this.isLoaded = true;

      },
      error: (err) => {
        this.hasError = true;
        this.errorValue = err.error;
      }
    });
    this.initWebSocketForPower();
    
  }
  initWebSocketForPower(){
    let connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7217/hubs/spspower?deviceId=' + this.batteryId)
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMessage', (from: string, message: string) => {
      let value =  JSON.parse(from);
      this.battery.occupationLevel = Math.round(value * 100) / 100;
    

    });

    connection.start()
      .then(() => {
      })
      .catch(err => {

      });
  }

  showOnlineReport(){
    this.router.navigate(['smart', this.batteryId, 'online-report']);
    
  }
  initWebSocket() {
    let connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7217/hubs/status?propertyId=' + this.smartPropertyId)
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMessage', (from: string, message: string) => {
      let value: DeviceStatusDTO = JSON.parse(from);


      if (this.battery.id == value.Id || this.battery.id == value.deviceId) {
        if (value.IsOnline != null) {
          this.battery.isOnline = value.IsOnline;
        }
      }

    });

    connection.start()
      .then(() => {
      })
      .catch(err => {
      });

  }
  constructor(private service: SmartDeviceService, private route: ActivatedRoute, private location: Location, private router:Router) {

  }
  goBack() {
    this.location.back();
  }
  smartPropertyId = '';
  base64Image = '';
  isLoaded = false;
  battery: BatteryDTO;
  hasError = false;
  errorValue = '';
  batteryId = '';
  totalCapacity = 0;
  currentOccupation = 0;
}
