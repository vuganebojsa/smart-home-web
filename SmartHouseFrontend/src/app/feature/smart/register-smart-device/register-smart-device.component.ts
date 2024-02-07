import { Location } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register-smart-device',
  templateUrl: './register-smart-device.component.html',
  styleUrls: ['./register-smart-device.component.css']
})
export class RegisterSmartDeviceComponent {
  selectedCategory: string;
  options: { [key: string]: string[] } = {
    smartHomeDevices: ['Air Conditioner', 'Ambient Sensor', 'Washing Machine'],
    outsideSmartDevices: ['Lamp', 'Vehicle Gate', 'Sprinkler System'],
    electromagneticDevices: ['Electric Vehicle Charger', 'House Battery', 'Solar Panel System']
  };
  selectedOption: string;
  showRegistration = false;
  constructor(private router: Router, private location: Location){
    
  }
  onCategoryChange(event: Event) {
    this.selectedCategory = (event.target as HTMLSelectElement).value;
    this.selectedOption = '';
    this.showRegistration = false;
  }

  onOptionChange(event: Event) {
    this.selectedOption = (event.target as HTMLSelectElement).value;
    this.showRegistration = true;
  
  }
  goBack(){
    this.location.back();
  }
}
