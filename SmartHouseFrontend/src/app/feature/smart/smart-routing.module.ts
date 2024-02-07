import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterSmartDeviceComponent } from './register-smart-device/register-smart-device.component';
import { SmartPropertyListComponent } from './smart-property-list/smart-property-list.component';
import { SmartPropertyRequestsListComponent } from './smart-property-requests-list/smart-property-requests-list.component';
import { RegisterPropertyComponent } from './register-property/register-property.component';
import { SinglePropertyComponent } from './single-property/single-property.component';
import { SmartPropertyDevicesDisplayComponent } from './smart-property-devices-display/smart-property-devices-display.component';
import { LampDeviceComponent } from './lamp-device/lamp-device.component';
import { SpsDeviceComponent } from './sps-device/sps-device.component';
import { BatteryDeviceComponent } from './battery-device/battery-device.component';
import { GateDeviceComponent } from './gate-device/gate-device.component';
import { BatteryInfoComponent } from './battery-info/battery-info.component';
import {AmbientSensorDeviceComponent} from "./ambient-sensor-device/ambient-sensor-device.component";
import { SprinklerDeviceComponent } from './sprinkler-device/sprinkler-device.component';
import { OnlineReportComponent } from './online-report/online-report.component';
import { VehicleChargerDeviceComponent } from './vehicle-charger-device/vehicle-charger-device.component';
import { VehicleChargerReportsComponent } from './vehicle-charger-reports/vehicle-charger-reports.component';
import { AdminPropertyDisplayComponent } from './admin-property-display/admin-property-display.component';
import { SmartPropertyReportComponent } from './smart-property-report/smart-property-report.component';
import { CityReportComponent } from './city-report/city-report.component';

const routes: Routes = [
  {path: ':id/register-device', component:RegisterSmartDeviceComponent},
  {path: '*', component:SmartPropertyListComponent},
  {path: '*', component:SmartPropertyRequestsListComponent},
  {path: 'register-property', component:RegisterPropertyComponent},
  {path: ':id/single-property', component:SinglePropertyComponent},
  {path: ':id/devices', component:SmartPropertyDevicesDisplayComponent},
  {path: ':id/lamp-device', component: LampDeviceComponent},
  {path: ':id/sps-device', component: SpsDeviceComponent},
  {path: ':id/battery-device', component: BatteryDeviceComponent},
  {path: ':id/gate-device', component: GateDeviceComponent},
  {path: ':id/energy-consumption', component: BatteryDeviceComponent},
  {path: ':id/battery', component: BatteryInfoComponent},
  {path: ':id/ambient-sensor-device', component: AmbientSensorDeviceComponent},
  {path: ':id/sprinkler-device', component: SprinklerDeviceComponent},
  {path: ':id/online-report', component: OnlineReportComponent},
  {path: ':id/vehicle-charger', component: VehicleChargerDeviceComponent},
  {path: ':id/vehicle-charger-manage', component: VehicleChargerReportsComponent},
  {path: 'properties', component: AdminPropertyDisplayComponent},
  {path: ':id/report', component: SmartPropertyReportComponent},
  {path: ':name/city-report', component: CityReportComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SmartRoutingModule { }
