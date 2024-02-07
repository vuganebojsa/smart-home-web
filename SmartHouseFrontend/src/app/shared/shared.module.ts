import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './layout/navbar/navbar.component';
import { AppRoutingModule } from '../app-routing.module';
import { ChartComponent } from './layout/chart/chart.component';



@NgModule({
  declarations: [
    NavbarComponent,
    ChartComponent
  ],
  imports: [
    CommonModule,
    AppRoutingModule
  ],
  exports: [
    NavbarComponent,
    ChartComponent
  ]
})
export class SharedModule { }
