import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { SmartDeviceService } from '../smart-device.service';
import { SPSAction, SpsDTO } from 'src/app/shared/models/SolarPanelSystem';
import { HubConnectionBuilder } from '@microsoft/signalr/dist/esm/HubConnectionBuilder';
import { SmartDeviceDTO } from 'src/app/shared/models/SmartDevice';
import { PagedList } from 'src/app/shared/models/Pagination';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-sps-device',
  templateUrl: './sps-device.component.html',
  styleUrls: ['./sps-device.component.css']
})
export class SpsDeviceComponent implements OnInit {

  hasLoaded = false;
  id: string = '';
  hasError = false;
  errorValue = '';
  device: SmartDeviceDTO = null;
  sps: SpsDTO = null;
  noData = false;
  chosenDateTime = new FormGroup({
    start: new FormControl(''),
    end: new FormControl(''),
    username: new FormControl('')


  });
  totalPowerGeneratedLastMinute = 0;
  constructor(private location: Location,
    private smartDeviceService: SmartDeviceService,
    private route: ActivatedRoute, private router: Router) { }

  initWebSocket() {

    let connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7217/hubs/spspower?deviceId=' + this.id)
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMessage', (from: string, message: string) => {
      let value = JSON.parse(from);
      this.totalPowerGeneratedLastMinute = Math.round(value * 100) / 100;

    });

    connection.start()
      .then(() => {
      })
      .catch(err => {
      });

  }
  
  goBack() {
    this.location.back();
  }
  showOnlineReport(){
    this.router.navigate(['smart', this.id, 'online-report']);
    
  } 
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.id = params['id'];
    });
    this.device = JSON.parse(localStorage.getItem('selectedDevice')) as SmartDeviceDTO;
    this.initWebSocket();
    this.getSPSValues();
    this.getSPS();
  }
  getSPS(){
    this.smartDeviceService.getSPS(this.id).subscribe({
      next:(sps) =>{
        this.sps = sps;
        this.smartDeviceService.getPicture(this.sps.pathToImage).subscribe(result =>{
          const url = URL.createObjectURL(result);
          (document.getElementById('profilna') as HTMLImageElement).src = url;
      });
      },
      error:(err) =>{
        console.log(err);
      }
    })
  }
  
  onPageChange(event: PageEvent): void {
    this.setValuesDisplay(event.pageIndex + 1, event.pageSize);
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
  values: SPSAction[] = [];
  pagedList: PagedList = {};
  displayedValues :SPSAction[] = [];
  sortDirection = 'desc';
  sortKey: string = '';
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
  private getSPSValues() {
    this.smartDeviceService.getSolarPanelSystemReport(this.id, '', '', 'nebojsavuga').subscribe({
      next: (result) => {
        this.values = result;
        this.pagedList.currentPage = 1;
        this.pagedList.totalPages = Math.floor(this.values.length / 10);
        this.pagedList.totalDevices = this.values.length;
        this.pagedList.pageSize = 5;
        this.pagedList.hasNextPage = this.pagedList.currentPage < this.pagedList.totalPages;
        this.pagedList.hasPreviousPage = this.pagedList.currentPage > 1;
        const startIndex = (this.pagedList.currentPage - 1) * this.pagedList.pageSize;
        const endIndex = startIndex + this.pagedList.pageSize;
        this.displayedValues = this.values.slice(startIndex, endIndex);
        this.hasLoaded = true;
      },
      error: (err) => {
        console.log(err.error);
      }
    });
  }
  
  submit() {
    if (!this.chosenDateTime.valid) {
      this.hasError = true;
      this.errorValue = 'Please fulfill all the fields corectly.'
      return;
    }
    this.hasError = false;
    let start = this.chosenDateTime.value.start;
    let end = this.chosenDateTime.value.end;
    let username = this.chosenDateTime.value.username;
    if (start == '' && end == '' && username == '') {
      this.hasError = true;
      this.errorValue = 'Please fulfill all the fields corectly.'
      return;
    }
    if (start == null) start = '';
    else start = start.toLocaleString();
    if (end == null) end = '';
    else end = end.toLocaleString();
    if (username == null) username = '';
    this.smartDeviceService.getSolarPanelSystemReport(this.id, start, end, username).subscribe({
      next: (result) => {

        this.values = result;
        this.pagedList.currentPage = 1;
        this.pagedList.totalPages = Math.floor(this.values.length / 10);
        this.pagedList.totalDevices = this.values.length;
        this.pagedList.pageSize = 5;
        this.pagedList.hasNextPage = this.pagedList.currentPage < this.pagedList.totalPages;
        this.pagedList.hasPreviousPage = this.pagedList.currentPage > 1;
        const startIndex = (this.pagedList.currentPage - 1) * this.pagedList.pageSize;
        const endIndex = startIndex + this.pagedList.pageSize;
        this.displayedValues = this.values.slice(startIndex, endIndex);
        this.hasLoaded = true;
      },
      error: (err) => {
        this.hasError = true;
        this.errorValue = err.error;
      }
    });

  }
}
