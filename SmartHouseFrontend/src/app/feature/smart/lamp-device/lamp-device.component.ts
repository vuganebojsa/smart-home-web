import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Chart, registerables } from 'node_modules/chart.js';
import { Location } from '@angular/common';
import { SmartDeviceService } from '../smart-device.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LuminosityDTO } from 'src/app/shared/models/Lamp';
Chart.register(...registerables);
const today = new Date();
const sixHoursAgo = new Date(today.getTime() - 6 * 60 * 60 * 1000);
const twelveHoursAgo = new Date(today.getTime() - 12 * 60 * 60 * 1000);
const dayAgo = new Date(today.getTime() - 24 * 60 * 60 * 1000);
const oneWeekAgo = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000);
const oneMonthAgo = new Date(today);


oneMonthAgo.setMonth(today.getMonth() - 1);

@Component({
  selector: 'app-lamp-device',
  templateUrl: './lamp-device.component.html',
  styleUrls: ['./lamp-device.component.css']
})
export class LampDeviceComponent implements OnInit {
  myChart!: Chart;
  id = '';
  selectedTimePeriod = 0;
  errorStartDate = false;
  errorStartDateTooBig = false
  luminosities = [];
  errorMessage: string | null = null;
  chosenDateTime = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });
  ngOnInit(): void {

    this.route.params.subscribe((params) => {
      this.id = params['id'];
    });
    this.getLuminostiy(sixHoursAgo.toISOString(), today.toISOString())

    this.myChart = new Chart('barChart', {
      type: 'line',
      data: {

        datasets: [{
          label: 'Luminosity',
          data: [],
          backgroundColor: ['#01ACAB'],
          borderColor: [
            'rgba(0, 0, 0, 1)'
          ],
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
  constructor(private location: Location,private router: Router, private smartDeviceService: SmartDeviceService, private route: ActivatedRoute) { }

  updateChart(labelData: any, mainData: any) {
    this.myChart.data.labels = labelData;
    this.myChart.data.datasets[0].data = mainData

    this.myChart.update();
  }
  showOnlineReport(){
    this.router.navigate(['smart', this.id, 'online-report']);
    
  } 

  Submit() {
    if (this.chosenDateTime.valid) {
      this.errorStartDateTooBig = false;
      if (this.chosenDateTime.value.start == null){
        this.errorStartDate = true;
        return
      }
      this.errorStartDate = false;
      let localStartDate = this.chosenDateTime.value.start;
    let localEndDate = this.chosenDateTime.value.end;
    if(localEndDate == null){
      
      const today = new Date();
      today.setHours(0);
      today.setMinutes(0)
      localEndDate = new Date(today.getTime() + (24 * 60 * 60 * 1000))
      if(this.chosenDateTime.value.start > localEndDate){
        this.errorStartDateTooBig = true;
        return
      }

    }
    else{
      localEndDate = new Date(localEndDate.getTime() + (24 * 60 * 60 * 1000));
    }
    
      
    if (!this.isWithinOneMonth(localStartDate, localEndDate)) {
      this.errorMessage = 'The difference between start and end dates cannot be more than one month.';
        return;
      
    }
    this.errorMessage = null;
    const startISOString = new Date(
      localStartDate.getTime() 
    ).toISOString();

    const endISOString = new Date(
      localEndDate.getTime()
    ).toISOString();

    
      this.getLuminostiy(startISOString, endISOString);
      this.selectedTimePeriod = 5;
    }
  }
  goBack() {
    this.location.back();
  }
  
  private isWithinOneMonth(startDate: Date, endDate: Date): boolean {
    const nextMonth = new Date(startDate);
    nextMonth.setMonth(startDate.getMonth() + 1);
  
    return endDate <= nextMonth;
  }

  private getLuminostiy(startDate: string, endDate: string) {

    this.smartDeviceService.getLampLuminosityReport(this.id, startDate, endDate).subscribe({
      next: (result) => {
        this.luminosities = result
        const timestamps = this.luminosities.map(entry => entry.timestamp);
        const formattedTimestamps = timestamps.map(timestamp => {
          return new Date(timestamp).toLocaleString('en-GB', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
          });
        });
  
        const luminosityValues = this.luminosities.map(entry => entry.luminosity);
        console.log(luminosityValues)
        this.updateChart(formattedTimestamps, luminosityValues);
      },
      error: (err) => {
        console.log(err.error);
      }
    });
  }
  public ChooseTime(option: number) {
    this.errorStartDate = false;
    this.errorStartDateTooBig = false;
  if(option == 0){
    this.selectedTimePeriod = 0;
    this.getLuminostiy(sixHoursAgo.toISOString(), today.toISOString())
  }
  else if(option == 1){
    this.selectedTimePeriod = 1;
    this.getLuminostiy(twelveHoursAgo.toISOString(), today.toISOString())
  }
  else if(option == 2){
    this.selectedTimePeriod = 2;
    this.getLuminostiy(dayAgo.toISOString(), today.toISOString())
  }
  else if(option == 3){
    this.selectedTimePeriod = 3;
    this.getLuminostiy(oneWeekAgo.toISOString(), today.toISOString())
  }
  else if(option == 4){
    this.selectedTimePeriod = 4;
    this.getLuminostiy(oneMonthAgo.toISOString(), today.toISOString())
  }
  

  }







}
