import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SmartDeviceDTO } from 'src/app/shared/models/SmartDevice';
import { SmartDeviceService } from '../smart-device.service';
import { PagedList } from 'src/app/shared/models/Pagination';
import { VehicleChargerActionsDTO, VehicleChargerAllActionsDTO } from 'src/app/shared/models/VehicleCharger';
import { PageEvent } from '@angular/material/paginator';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-vehicle-charger-reports',
  templateUrl: './vehicle-charger-reports.component.html',
  styleUrls: ['./vehicle-charger-reports.component.css']
})
export class VehicleChargerReportsComponent implements OnInit{
  goBack() {
    this.location.back();
  }
  hasLoaded = false;
  id: string = '';
  pagedList: PagedList = {};
  pagedListSecond: PagedList = {};
  values: VehicleChargerActionsDTO[] = [];
  valuesSecond: VehicleChargerAllActionsDTO[] = [];
  displayedValuesSecond :VehicleChargerAllActionsDTO[] = [];
  displayedValues :VehicleChargerActionsDTO[] = [];
  device: SmartDeviceDTO;
  hasError = false;
  errorValue = '';
  sortDirection = 'asc';
  sortKey: string = '';
  chosenDateTime = new FormGroup({
    start: new FormControl(''),
    end: new FormControl(''),
    username: new FormControl('')


  });
  constructor(private location: Location,
     private route: ActivatedRoute,
     private service: SmartDeviceService){}
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.id = params['id'];
    });
    this.device = JSON.parse(localStorage.getItem('selectedDevice')) as SmartDeviceDTO;

    this.GetReport();
    
  }
  private GetReport() {
    this.service.getVehicleChargerHistory(this.id).subscribe({
      next: (result) => {
        this.values = result;
        this.pagedList.currentPage = 1;
        this.pagedList.totalPages = Math.floor(this.values.length / 10);
        this.pagedList.totalDevices = this.values.length;
        this.pagedList.pageSize = 5;
        //console.log(Math.floor(this.values.length / 10));
        this.pagedList.hasNextPage = this.pagedList.currentPage < this.pagedList.totalPages;
        this.pagedList.hasPreviousPage = this.pagedList.currentPage > 1;
        const startIndex = (this.pagedList.currentPage - 1) * this.pagedList.pageSize;
        const endIndex = startIndex + this.pagedList.pageSize;
        this.displayedValues = this.values.slice(startIndex, endIndex);

        this.hasLoaded = true;


      },
      error: (err) => {
      }
    });
  }

  setValuesDisplay(page:number, pageSize: number): void{
    this.pagedList.currentPage = page;
    this.pagedList.pageSize = pageSize;
    this.pagedList.hasNextPage = this.pagedList.currentPage < this.pagedList.totalPages;
    this.pagedList.hasPreviousPage = this.pagedList.currentPage > 1;
    const startIndex = (this.pagedList.currentPage - 1) * this.pagedList.pageSize;
    const endIndex = startIndex + this.pagedList.pageSize;
    this.displayedValues = this.values.slice(startIndex, endIndex);

  }
  setPageSecondValuesDisplay(page:number, pageSize: number): void{
    this.pagedListSecond.currentPage = page;
    this.pagedListSecond.pageSize = pageSize;
    this.pagedListSecond.hasNextPage = this.pagedListSecond.currentPage < this.pagedListSecond.totalPages;
    this.pagedListSecond.hasPreviousPage = this.pagedListSecond.currentPage > 1;
    const startIndex = (this.pagedListSecond.currentPage - 1) * this.pagedListSecond.pageSize;
    const endIndex = startIndex + this.pagedListSecond.pageSize;
    this.displayedValuesSecond = this.valuesSecond.slice(startIndex, endIndex);

  }
  onPageChange(event: PageEvent): void {
    this.setValuesDisplay(event.pageIndex + 1, event.pageSize);
  }
  onPageSecondChange(event: PageEvent): void {
    this.setPageSecondValuesDisplay(event.pageIndex + 1, event.pageSize);
  }

  submit(){
    let username = this.chosenDateTime.value.username;
    let start = this.chosenDateTime.value.start;
    let end = this.chosenDateTime.value.end;
    if(!username) username = '';
    if (start == '' && end == '') {
      this.hasError = true;
      this.errorValue = 'Please enter both dates.'
      return;
    }
    const startDate = new Date(start);
    const endDate = new Date(end);
    if (start == null) start = '';
    else start = start.toLocaleString();
    if (end == null) end = '';
    else end = end.toLocaleString();


  if (startDate > endDate) {
    this.hasError = true;
    this.errorValue = 'Start date is after end date.'
    return;
  }
  username = username.trim();
    this.service.getVehicleChargerHistoryInRange(this.id, start, end, username).subscribe({
      next:(result)=>{
        this.valuesSecond = result;
        this.pagedListSecond.currentPage = 1;
        this.pagedListSecond.totalPages = Math.floor(this.valuesSecond.length / 10);
        this.pagedListSecond.totalDevices = this.valuesSecond.length;
        this.pagedListSecond.pageSize = 5;
        this.pagedListSecond.hasNextPage = this.pagedListSecond.currentPage < this.pagedListSecond.totalPages;
        this.pagedListSecond.hasPreviousPage = this.pagedListSecond.currentPage > 1;
        const startIndex = (this.pagedListSecond.currentPage - 1) * this.pagedListSecond.pageSize;
        const endIndex = startIndex + this.pagedListSecond.pageSize;
        this.displayedValuesSecond = this.valuesSecond.slice(startIndex, endIndex);
        this.hasError = false;
      },
      error:(err) =>{
        this.hasError = true;
        this.errorValue = err.error;
      }
    })
  }

  sortColumn(key:string){
    this.sortKey = key;
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';

    // Call your sorting function
    this.sortValues();
  }
  sortValues() {
    this.values.sort((a, b) => {
      const valA = a[this.sortKey];
      const valB = b[this.sortKey];

      if (this.sortDirection === 'asc') {
        return valA < valB ? -1 : valA > valB ? 1 : 0;
      } else {
        return valA > valB ? -1 : valA < valB ? 1 : 0;
      }
    });
    const startIndex = (this.pagedList.currentPage - 1) * this.pagedList.pageSize;
    const endIndex = startIndex + this.pagedList.pageSize;
    this.displayedValues = this.values.slice(startIndex, endIndex);
  }
  sortColumnSecond(key:string){
    this.sortKey = key;
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';

    // Call your sorting function
    this.sortValuesSecond();
  }
  sortValuesSecond() {
    this.valuesSecond.sort((a, b) => {
      const valA = a[this.sortKey];
      const valB = b[this.sortKey];

      if (this.sortDirection === 'asc') {
        return valA < valB ? -1 : valA > valB ? 1 : 0;
      } else {
        return valA > valB ? -1 : valA < valB ? 1 : 0;
      }
    });
    const startIndex = (this.pagedListSecond.currentPage - 1) * this.pagedListSecond.pageSize;
    const endIndex = startIndex + this.pagedListSecond.pageSize;
    this.displayedValuesSecond = this.valuesSecond.slice(startIndex, endIndex);
  }
}
