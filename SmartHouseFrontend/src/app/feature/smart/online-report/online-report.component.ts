import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Chart, registerables } from 'node_modules/chart.js';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartDeviceService } from '../smart-device.service';

Chart.register(...registerables);

@Component({
  selector: 'app-online-report',
  templateUrl: './online-report.component.html',
  styleUrls: ['./online-report.component.css']
})
export class OnlineReportComponent implements OnInit{
  selectedTimePeriod = 0;
  onlineValues = [];
  offlineValues =[];
  myChartOnline!: Chart;
  pieChart!:any;
  totalOnline = 0;
  totalOffline = 0;
  pieChartData: number[] = [1, 1];
  percentageOnline = 0;
  id = '';

  errorMessageOnlineHistory = null;
  errorStartDateOnlineHistoryTooBig = false;
  errorStartDateOnlineHistory = false;
  errorStartDateOnlineAfterToday = false;
  errorStartDate = false;
  errorStartDateTooBig = false;


  constructor(private location: Location, private route: ActivatedRoute, private router: Router, private smartDeviceService: SmartDeviceService) { }


  chosenDateTimeOnlineHistory = new FormGroup({
    startOnlineHistory: new FormControl(),
    endOnlineHistory: new FormControl(),
  });

  ngOnInit(): void {

    console.log("cao")
    this.route.params.subscribe((params) => {
      this.id = params['id'];
      console.log(this.id);
    });
    const currentTime = new Date();
    // Get time 1 hour ago
    const sixHoursAgo = new Date(currentTime.getTime() - 6 * 60 * 60 * 1000);
    const currentISOTime = currentTime.toISOString();
    const sixHourAgoISOTime = sixHoursAgo.toISOString();
    this.getChart();
    this.initPieChart();
    this.getOnlineHistory(sixHourAgoISOTime,currentISOTime);
    
    this.myChartOnline.update();
  }
  goBack() {
    this.location.back();
  }

  private initPieChart() {

    this.pieChart = new Chart("pieChart", {
      type: 'doughnut',

      data: {
        labels: ['Offline (min)', 'Online (min)'],
	       datasets: [{
    label: '',
    data: this.pieChartData,
    backgroundColor: [
      'red',
      'blue',			
    ],
    hoverOffset: 4
  }],
      },
      options: {
        maintainAspectRatio: false,
        plugins: {
          tooltip: {
            titleFont: {
              size: 18
            },
            bodyFont: {
              size: 16
            },
            footerFont: {
              size: 16 // there is no footer by default
            }
          }
        }
      },

    });
  }


  getChart(){
    console.log('dsdas')
    this.myChartOnline = new Chart('barChartOnline', {
      type: 'bar',
      data: {

        datasets: [
          {
            label: 'Online (min)',
            data: [0,2,4],
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1
          },
          {
            label: 'Offline (min)',
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




  SubmitOnlineHistory() {
    if (this.chosenDateTimeOnlineHistory.valid) {
      this.errorStartDateOnlineHistoryTooBig = false;
      this.errorStartDateOnlineAfterToday = false;
      if (this.chosenDateTimeOnlineHistory.value.startOnlineHistory == null){
        this.errorStartDateOnlineHistory = true;
        return
      }
      this.errorStartDateOnlineHistory = false;
      let localStartDate = this.chosenDateTimeOnlineHistory.value.startOnlineHistory;
    let localEndDate = this.chosenDateTimeOnlineHistory.value.endOnlineHistory;
    const today = new Date();
    if(this.chosenDateTimeOnlineHistory.value.startOnlineHistory > today){
      this.errorStartDateOnlineAfterToday = true;
      return
    }
    if(localEndDate == null){
      
      const today = new Date();
      
      today.setHours(0);
      today.setMinutes(0)
      localEndDate = new Date(today.getTime() + (24 * 60 * 60 * 1000))
      if(this.chosenDateTimeOnlineHistory.value.startOnlineHistory > localEndDate){
        this.errorStartDateOnlineHistoryTooBig = true;
        return
      }

    }
    else{
      localEndDate = new Date(localEndDate.getTime() + (24 * 60 * 60 * 1000));
    }
    
      
    if (!this.isWithinOneMonth(localStartDate, localEndDate)) {
      this.errorMessageOnlineHistory = 'The difference between start and end dates cannot be more than one month.';
        return;
      
    }
    this.errorMessageOnlineHistory = null;
    const startISOString = new Date(
      localStartDate.getTime() 
    ).toISOString();

    const endISOString = new Date(
      localEndDate.getTime()
    ).toISOString();

    
      this.getOnlineHistory(startISOString, endISOString);
      this.selectedTimePeriod = 5;

    }
  }

  private isWithinOneMonth(startDate: Date, endDate: Date): boolean {
    const nextMonth = new Date(startDate);
    nextMonth.setMonth(startDate.getMonth() + 1);
  
    return endDate <= nextMonth;
  }

  private getOnlineHistory(startDate: string, endDate: string) {

    this.smartDeviceService.getDeviceOnlineOfflineReport(this.id, startDate, endDate).subscribe({
      next: (result) => {
        console.log(result)
        let onlineKeys = (Object.keys(result.onlineMap))
        onlineKeys.sort();
        let onlineValues = onlineKeys.map(key => result.onlineMap[key] / 60);
let offlineValues = onlineKeys.map(key => result.offlineMap[key] / 60);
        this.totalOffline = (Math.round((result.totalTimeOffline/60) * 100) / 100) ;
        this.totalOnline = (Math.round((result.totalTimeOnline/60) * 100) / 100) ;
        this.percentageOnline = Math.round((Math.round(result.percentageOnline * 100) / 100)*100);
        this.pieChartData = [this.totalOffline, this.totalOnline];
        this.pieChart.data.datasets[0].data = this.pieChartData;
        this.pieChart.update();

        this.updateChart(onlineKeys, onlineValues, offlineValues);
      },
      error: (err) => {
        console.log(err.error);
      }
    });
  }
  updateChart(labelData: any, mainData: any, secondData:any) {
    this.myChartOnline.data.labels = labelData;
    this.myChartOnline.data.datasets[0].data = mainData
    this.myChartOnline.data.datasets[1].data = secondData

    this.myChartOnline.update();
  }
  public ChooseTimeOnline(option: number) {
    const today = new Date();
const sixHoursAgo = new Date(today.getTime() - 6 * 60 * 60 * 1000);
const twelveHoursAgo = new Date(today.getTime() - 12 * 60 * 60 * 1000);
const dayAgo = new Date(today.getTime() - 24 * 60 * 60 * 1000);
const oneWeekAgo = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000);
const oneMonthAgo = new Date(today);
oneMonthAgo.setMonth(today.getMonth() - 1);
    this.errorStartDate = false;
    this.errorStartDateTooBig = false;
    this.errorStartDateOnlineAfterToday = false;
  if(option == 0){
    this.selectedTimePeriod = 0;
    this.getOnlineHistory(sixHoursAgo.toISOString(), today.toISOString())
  }
  else if(option == 1){
    this.selectedTimePeriod = 1;
    this.getOnlineHistory(twelveHoursAgo.toISOString(), today.toISOString())
  }
  else if(option == 2){
    this.selectedTimePeriod = 2;
    this.getOnlineHistory(dayAgo.toISOString(), today.toISOString())
  }
  else if(option == 3){
    this.selectedTimePeriod = 3;
    this.getOnlineHistory(oneWeekAgo.toISOString(), today.toISOString())
  }
  else if(option == 4){
    this.selectedTimePeriod = 4;
    this.getOnlineHistory(oneMonthAgo.toISOString(), today.toISOString())
  }
}


}
