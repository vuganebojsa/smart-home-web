import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SmartRoutingModule } from './smart-routing.module';
import { RegisterSmartDeviceComponent } from './register-smart-device/register-smart-device.component';
import { RegisterElectricVehicleChargetComponent } from './register-electric-vehicle-charget/register-electric-vehicle-charget.component';
import { RegisterHouseBatteryComponent } from './register-house-battery/register-house-battery.component';
import { RegisterSolarPanelSystemComponent } from './register-solar-panel-system/register-solar-panel-system.component';
import { RegisterLampComponent } from './register-lamp/register-lamp.component';
import { RegisterSprinklerSystemComponent } from './register-sprinkler-system/register-sprinkler-system.component';
import { RegisterVehicleGateComponent } from './register-vehicle-gate/register-vehicle-gate.component';
import { RegisterAirConditionerComponent } from './register-air-conditioner/register-air-conditioner.component';
import { RegisterAmbientSensorComponent } from './register-ambient-sensor/register-ambient-sensor.component';
import { RegisterWashingMachineComponent } from './register-washing-machine/register-washing-machine.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SmartPropertyListComponent } from './smart-property-list/smart-property-list.component';
import { SmartPropertyRequestsListComponent } from './smart-property-requests-list/smart-property-requests-list.component';
import { MapComponent } from './map/map.component';
import { RegisterPropertyComponent } from './register-property/register-property.component';
import { SinglePropertyComponent } from './single-property/single-property.component';
import { SmartPropertyDevicesDisplayComponent } from './smart-property-devices-display/smart-property-devices-display.component';
import { LampDeviceComponent } from './lamp-device/lamp-device.component';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { SpsDeviceComponent } from './sps-device/sps-device.component';
import { BatteryDeviceComponent } from './battery-device/battery-device.component';
import { GateDeviceComponent } from './gate-device/gate-device.component';
import { BatteryInfoComponent } from './battery-info/battery-info.component';
import {MatPaginatorModule} from '@angular/material/paginator';
import { AmbientSensorDeviceComponent } from './ambient-sensor-device/ambient-sensor-device.component';
import { SprinklerDeviceComponent } from './sprinkler-device/sprinkler-device.component';
import {NgxMaterialTimepickerModule} from 'ngx-material-timepicker';
import { OnlineReportComponent } from './online-report/online-report.component';

import { VehicleChargerDeviceComponent } from './vehicle-charger-device/vehicle-charger-device.component';
import { VehicleChargerReportsComponent } from './vehicle-charger-reports/vehicle-charger-reports.component';
import { AdminPropertyDisplayComponent } from './admin-property-display/admin-property-display.component';
import { SmartPropertyReportComponent } from './smart-property-report/smart-property-report.component';
import { CityReportComponent } from './city-report/city-report.component';



@NgModule({
  declarations: [
    RegisterSmartDeviceComponent,
    RegisterElectricVehicleChargetComponent,
    RegisterHouseBatteryComponent,
    RegisterSolarPanelSystemComponent,
    RegisterLampComponent,
    RegisterSprinklerSystemComponent,
    RegisterVehicleGateComponent,
    RegisterAirConditionerComponent,
    RegisterAmbientSensorComponent,
    RegisterWashingMachineComponent,
    SmartPropertyListComponent,
    SmartPropertyRequestsListComponent,
    MapComponent,
    RegisterPropertyComponent,
    SinglePropertyComponent,
    SmartPropertyDevicesDisplayComponent,
    LampDeviceComponent,
    SpsDeviceComponent,
    BatteryDeviceComponent,
    GateDeviceComponent,
    BatteryInfoComponent,
    AmbientSensorDeviceComponent,
    SprinklerDeviceComponent,
    OnlineReportComponent,
    VehicleChargerDeviceComponent,
    VehicleChargerReportsComponent,
    AdminPropertyDisplayComponent,
    SmartPropertyReportComponent,
    CityReportComponent
  ],
  imports: [
    MatFormFieldModule,
    MatDatepickerModule,
    NgxMaterialTimepickerModule,
    MatNativeDateModule,
    CommonModule,
    SmartRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MatPaginatorModule
  ],
  exports: [SmartPropertyListComponent, SmartPropertyRequestsListComponent],
})
export class SmartModule { }
