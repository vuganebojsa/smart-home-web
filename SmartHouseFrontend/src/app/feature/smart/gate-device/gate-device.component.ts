import { Component, OnInit } from '@angular/core';
import { SmartDeviceService } from '../smart-device.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Chart, registerables } from 'node_modules/chart.js';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { GateEventDTO, GatePublicPrivateDTO } from 'src/app/shared/models/VehicleGate';
import { DeviceOnOffDTO } from 'src/app/shared/models/SmartDevice';
import { SnackbarService } from 'src/app/core/services/snackbar.service';
import { PageEvent } from '@angular/material/paginator';
Chart.register(...registerables);

@Component({
  selector: 'app-gate-device',
  templateUrl: './gate-device.component.html',
  styleUrls: ['./gate-device.component.css']
})
export class GateDeviceComponent implements OnInit {
  selectedTimePeriod = 0;
  myChart!: Chart;
  newLicencePlate: string = '';
  errorStartDate = false;
  dates = [];
  luminosities = [];
  errorStartDateTooBig = false;
  isOn = false;
  tooltipData = [];
  gateEvents = [];
  validPlates = [];
  gatePublicPrivate = [];
  totalOffline = 0;
  timestamps = [];
  liveMessage = "live"
  live = true;
  actions = [];
  currentPage = 1;
  visibleEvents = [];
  visibleOnOff = [];
  visiblePublicPrivate = [];
pageSize = 10;
pageSizePublicPrivate = 10;
pageSizeOnOff = 10;
  gateOnOff = []
  error = false;
  errorMessage = '';
  isOnline = true;
  id = '';
  isPublic: boolean = true;
  constructor(private location: Location, private route: ActivatedRoute, private router: Router, private smartDeviceService: SmartDeviceService, private snackBar: SnackbarService) { }
  chosenDateTime = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
    plate: new FormControl(null,[Validators.minLength(6)])
  });

  

  newLicencePlateForm = new FormGroup({
    licencePlate: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  initWebSocket() {

    let connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7217/hubs/gateEvent?deviceId=' + this.id)
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMessage', (from: string, message: string) => {
      let value = JSON.parse(from);
      if (this.live) {
        
        if (value.licencePlate != null ) {
          
          const newEvent: GateEventDTO = {
            timestamp: value.timeStamp,
            licencePlate: value.licencePlate,
            action: value.action
          }
          
          if(this.chosenDateTime.value.plate == null || this.chosenDateTime.value.plate == '' ||!this.chosenDateTime.get("plate").valid){
            
            this.gateEvents.push(newEvent)
          this.gateEvents.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
          
          }
          if (newEvent.licencePlate == this.chosenDateTime.value.plate ){
          this.gateEvents.push(newEvent)
          this.gateEvents.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
          console.log("2")
          }
          this.updateViewEvent();
          
        }
        if (value.onOff != null) {
          let numberOnOff = 0;
          if (value.onOff == true) {
            numberOnOff = 1;
          }
          const newOnOff: DeviceOnOffDTO = {
            timestamp: value.timeStamp,
            isOn: numberOnOff
          }
          this.gateOnOff.push(newOnOff)
          this.gateOnOff.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
          this.updateViewOnOff();
        }
        if (value.publicPrivate != null) {
          let numberPublicPrivate = 0;
          if (value.publicPrivate == true) {
            numberPublicPrivate = 1;
          }
          const newPublicPrivate: GatePublicPrivateDTO = {
            timestamp: value.timeStamp,
            isPublic: numberPublicPrivate
          }
          this.gatePublicPrivate.push(newPublicPrivate)
          this.gatePublicPrivate.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
          this.updateViewPublicPrivate();
        }

      }





    });

    connection.start()
      .then(() => {
      })
      .catch(err => {
      });

  }

  ngOnInit(): void {


    this.route.params.subscribe((params) => {
      this.id = params['id'];
      console.log(this.id);
    });
    this.initWebSocket();
    this.smartDeviceService.getValidLicencePlates(this.id).subscribe({
      next: (result) => {
        this.validPlates = result
      },
      error: (err) => {
      }
    })

    this.smartDeviceService.getGateInfo(this.id).subscribe({
      next: (result) => {
        this.isPublic = result.isPublic;
        this.isOn = result.isOn;
        this.isOnline = result.isOnline;

      },
      error: (err) => {
      }
    })
    const currentTime = new Date();
    // Get time 1 hour ago
    const sixHoursAgo = new Date(currentTime.getTime() - 6 * 60 * 60 * 1000);
    const currentISOTime = currentTime.toISOString();
    const sixHourAgoISOTime = sixHoursAgo.toISOString();
    console.log(currentISOTime + "     " + sixHourAgoISOTime)
    this.getGateEventHistory(sixHourAgoISOTime, currentISOTime, null);
    this.getGateOnOffHistory(sixHourAgoISOTime, currentISOTime);
    this.getGatePublicPrivateHistory(sixHourAgoISOTime, currentISOTime);
    this.getChart();
    this.myChart.update();
  }

  getChart(){
    this.myChart = new Chart('barChart', {
      type: 'bar',
      data: {

        datasets: [
          {
            label: 'Online',
            data: [0,2,4],
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1
          },
          {
            label: 'Offline',
            data: [1,1,7],
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            borderColor: 'rgba(255, 99, 132, 1)',
            borderWidth: 1
          }]
      },
      options: {
        scales: {
          y: {
            beginAtZero: true
          },
          x: {
            ticks: {
              autoSkip: true,
              maxTicksLimit: 3,
            },
          }
        }
      }
    });
  }
  

  onPageChangeEvent(event: PageEvent): void {
    this.pageSize = event.pageSize
    this.currentPage = event.pageIndex + 1;
    this.updateViewEvent();
  }
  onPageChangeOnOff(event: PageEvent): void {
    this.pageSizeOnOff = event.pageSize
    this.currentPage = event.pageIndex + 1;
    this.updateViewOnOff();
  }
  onPageChangePublicPrivate(event: PageEvent): void {
    this.pageSizePublicPrivate = event.pageSize
    this.currentPage = event.pageIndex + 1;
    this.updateViewPublicPrivate();
  }
  
  updateViewEvent(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.visibleEvents = this.gateEvents.slice(startIndex, endIndex);
  }
  updateViewOnOff(): void {
    const startIndex = (this.currentPage - 1) * this.pageSizeOnOff;
    const endIndex = startIndex + this.pageSizeOnOff;
    this.visibleOnOff = this.gateOnOff.slice(startIndex, endIndex);
  }
  updateViewPublicPrivate(): void {
    const startIndex = (this.currentPage - 1) * this.pageSizePublicPrivate;
    const endIndex = startIndex + this.pageSizePublicPrivate;
    this.visiblePublicPrivate = this.gatePublicPrivate.slice(startIndex, endIndex);
  }

  turnPublicPrivate(newStatus: boolean) {

    this.smartDeviceService.turnPublicPrivateGate(this.id, newStatus).subscribe({
      next: (result) => {
        this.isPublic = result;
      },
      error: (err) => {
      }
    })

  }
  Submit() {
    if (this.chosenDateTime.get('start').valid && this.chosenDateTime.get('end').valid) {
      this.errorStartDateTooBig = false;
      if (this.chosenDateTime.value.start == null){
        this.errorStartDate = true;
        return
      }
      this.liveMessage = "history"
      this.errorStartDate = false
      
      this.live = false
      let licencePlate = this.chosenDateTime.value.plate
      let localStartDate = this.chosenDateTime.value.start;
      let localEndDate = this.chosenDateTime.value.end;
      if (localEndDate == null) {
        this.live = true
        this.liveMessage = "live"
        const today = new Date();
        today.setHours(0);
        today.setMinutes(0)
        localEndDate = new Date(today.getTime() + (24 * 60 * 60 * 1000))
        if (this.chosenDateTime.value.start > localEndDate){
          this.errorStartDateTooBig = true;
          return;
          
        }

      }
      else {
        
        localEndDate = new Date(localEndDate.getTime() + (24 * 60 * 60 * 1000));
      }
      const startISOString = new Date(
        localStartDate.getTime()
      ).toISOString();

      const endISOString = new Date(
        localEndDate.getTime()
      ).toISOString();
      console.log(licencePlate)
      if (!this.chosenDateTime.get('plate').valid){
        this.getGateEventHistory(startISOString, endISOString, null);
        
      }else{
        this.getGateEventHistory(startISOString, endISOString, this.chosenDateTime.value.plate);
      }
      this.updateViewEvent();

      this.selectedTimePeriod = 4;
      this.getGatePublicPrivateHistory(startISOString, endISOString);
      this.updateViewPublicPrivate();
      this.getGateOnOffHistory(startISOString, endISOString);
      this.updateViewOnOff();
    }
  }
  
  

  updateChart(labelData: any, mainData: any, secondData:any) {
    this.myChart.data.labels = labelData;
    this.myChart.data.datasets[0].data = mainData
    this.myChart.data.datasets[1].data = secondData

    this.myChart.update();
  }

  private isWithinOneMonth(startDate: Date, endDate: Date): boolean {
    const nextMonth = new Date(startDate);
    nextMonth.setMonth(startDate.getMonth() + 1);
  
    return endDate <= nextMonth;
  }

  private getGateEventHistory(startDate: string, endDate: string, licencePlate: string | null) {

    this.smartDeviceService.getGateEventReport(this.id, startDate, endDate, licencePlate).subscribe({
      next: (result) => {
        this.gateEvents = result;
        this.gateEvents.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
        this.updateViewEvent();
      },
      error: (err) => {
        console.log(err.error);
      }
    });
  }

  private getGatePublicPrivateHistory(startDate: string, endDate: string) {
    this.smartDeviceService.getGatePublicPrivateReport(this.id, startDate, endDate).subscribe({
      next: (result) => {
        this.gatePublicPrivate = result;
        this.gatePublicPrivate.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
        this.updateViewPublicPrivate();
      },
      error: (err) => {
        console.log(err.error);
      }
    });
  }

  private getGateOnOffHistory(startDate: string, endDate: string) {
    this.smartDeviceService.getDeviceOnOffReport(this.id, startDate, endDate).subscribe({
      next: (result) => {
        this.gateOnOff = result;
        this.gateOnOff.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
        this.updateViewOnOff();
      },
      error: (err) => {
        console.log(err.error);
      }
    });
  }

  goBack() {
    this.location.back();
  }

  showOnlineReport(){
    this.router.navigate(['smart', this.id, 'online-report']);
    
  }
  

  deleteLicencePlate(plate: string): void {
    this.smartDeviceService.removeLicencePlate(this.id, plate).subscribe({
      next: (result) => {
        this.loadValidLicencePlates()
        this.snackBar.showSnackBar('Successfully registered new licence plate.', "Ok");
      },
      error: (err) => {
        this.loadValidLicencePlates()
        this.snackBar.showSnackBar('Successfully deleted licence plate.', "Ok");
        console.log(err)
      }
    })


  }

  private loadValidLicencePlates(): void {
    this.smartDeviceService.getValidLicencePlates(this.id).subscribe({
      next: (result) => {
        this.validPlates = (result);
      },
      error: (err) => {
        // Handle error
      }
    });
  }

  turnOnOff(newStatus: boolean) {
    this.smartDeviceService.turnOnOffDevice(this.id, newStatus).subscribe({
      next: (result) => {
        this.isOn = result
      },
      error: (err) => {

      }
    })
  }

  public ChooseTime(option: number) {
      this.live = true
      this.liveMessage = "live"
      this.errorStartDate = false
      this.errorStartDateTooBig = false
    const today = new Date();
    if (option == 0) {
      
      this.selectedTimePeriod = 0;
      const sixHoursAgo = new Date(today.getTime() - 6 * 60 * 60 * 1000);
      if (!this.chosenDateTime.get('plate').valid){
        this.getGateEventHistory(sixHoursAgo.toISOString(), today.toISOString(), null);
      }else{
        this.getGateEventHistory(sixHoursAgo.toISOString(), today.toISOString(), this.chosenDateTime.value.plate);
      }
      
      this.getGateOnOffHistory(sixHoursAgo.toISOString(), today.toISOString());
      this.getGatePublicPrivateHistory(sixHoursAgo.toISOString(), today.toISOString());


    }
    else if (option == 1) {
      
      this.selectedTimePeriod = 1;
      const twelveHoursAgo = new Date(today.getTime() - 12 * 60 * 60 * 1000);
      if (!this.chosenDateTime.get('plate').valid){
        this.getGateEventHistory(twelveHoursAgo.toISOString(), today.toISOString(), null);
      }else{
        this.getGateEventHistory(twelveHoursAgo.toISOString(), today.toISOString(), this.chosenDateTime.value.plate);
      }
      this.getGateOnOffHistory(twelveHoursAgo.toISOString(), today.toISOString());
      this.getGatePublicPrivateHistory(twelveHoursAgo.toISOString(), today.toISOString());
    }
    else if (option == 2) {
      
      this.selectedTimePeriod = 2;

      const dayAgo = new Date(today.getTime() - 24 * 60 * 60 * 1000);
      if (!this.chosenDateTime.get('plate').valid){
        this.getGateEventHistory(dayAgo.toISOString(), today.toISOString(), null);
      }else{
        this.getGateEventHistory(dayAgo.toISOString(), today.toISOString(), this.chosenDateTime.value.plate);
      }
      this.getGateOnOffHistory(dayAgo.toISOString(), today.toISOString());
      this.getGatePublicPrivateHistory(dayAgo.toISOString(), today.toISOString());

    }
  }




  addLicencePlate(): void {
    if (this.newLicencePlateForm.valid) {
      this.smartDeviceService.registerLicencePlate(this.id, this.newLicencePlateForm.value.licencePlate).subscribe({
        next: (result) => {
          
          this.loadValidLicencePlates()
          this.snackBar.showSnackBar('Successfully registered new licence plate.', "Ok");
        },
        error: (err) => {
          this.error = true;
          this.errorMessage = err.error;
          this.newLicencePlateForm.get('licencePlate')?.valueChanges.subscribe((plate) => {
            this.error = false;
          })
        }
      })
    }


  }
}
