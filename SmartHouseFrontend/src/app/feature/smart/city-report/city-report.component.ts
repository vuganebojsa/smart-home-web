import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SmartDeviceService } from '../smart-device.service';
import { TotalTimePeriod } from 'src/app/shared/models/Battery';
import { FormControl, FormGroup } from '@angular/forms';
import { Chart } from 'chart.js';
import { EnergyDTO } from 'src/app/shared/models/Energy';

@Component({
  selector: 'app-city-report',
  templateUrl: './city-report.component.html',
  styleUrls: ['./city-report.component.css']
})
export class CityReportComponent implements OnInit{
  constructor(private route: ActivatedRoute,
    private location: Location,
    private service: SmartDeviceService){}
  name:string = '';
  hasError = false;
  errorValue = '';
  consumedChart!: Chart;
  producedChart!: Chart;
  pieChart!:any;
  tooltipNonRealTimeData = [];
  tooltipProduced = [];
  produced: EnergyDTO[] = [];
  consumed: EnergyDTO[] = [];
  pieChartData: number[] = [0, 0];
  chosenDateTime = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });
  selectedTimePeriod: TotalTimePeriod = TotalTimePeriod.SIX_HOURS;

  totalProduced:number = 0;
  totalConsumed:number = 0;
  public pieChartLabels: string[] = ['Total Produced', 'Total Consumed'];
  public pieChartType: string = 'pie';
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.name = params['name'];
    });  
    this.initChartConsumed( 'consumedChart', 'Power Consumed', 'consumed');
    this.initChartProduced( 'producedChart', 'Power Produced', 'produced');
    this.initPieChart();
    
    this.searchByTime(TotalTimePeriod.SIX_HOURS);


  }

  private initChartProduced(chartName:string, title:string, type:string, typeOfChart:string='line') {

    this.producedChart = new Chart(chartName, {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: title,
          data: [],
          backgroundColor: ['#01ACAB'],
          borderColor: [
            'rgba(0, 0, 0, 1)'
          ],
          borderWidth: 1
        }]
      },
      options: {
        maintainAspectRatio: false,
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

        },
        plugins: {
          tooltip: {
            callbacks: {
              title: function (tooltipItem) {
                return tooltipItem[0].label;
              },
              label:  (tooltipItem) => {
                const index = tooltipItem.dataIndex;
                return `The city ${type}: ${this.tooltipProduced[index].consumedValues} KW`;
              }
            }
          }
        }
      }
    });
  }
  private initChartConsumed(chartName:string, title:string, type:string, typeOfChart:string='line') {

    this.consumedChart = new Chart(chartName, {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: title,
          data: [],
          backgroundColor: ['#01ACAB'],
          borderColor: [
            'rgba(0, 0, 0, 1)'
          ],
          borderWidth: 1
        }]
      },
      options: {
        maintainAspectRatio: false,
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

        },
        plugins: {
          tooltip: {
            callbacks: {
              title: function (tooltipItem) {
                return tooltipItem[0].label;
              },
              label:  (tooltipItem) => {
                const index = tooltipItem.dataIndex;
                return `The city ${type}: ${this.tooltipNonRealTimeData[index].consumedValues} KW`;
              }
            }
          }
        }
      }
    });
  }
  private initPieChart() {

    this.pieChart = new Chart("pieChart", {
      type: 'doughnut',

      data: {
        labels: ['Produced KW', 'Consumed KW'],
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

  goBack() {
    this.location.back();
  }
  submit(){
    if(!this.chosenDateTime.valid){
      return;
    }
    let start = this.chosenDateTime.value.start;
    let end = this.chosenDateTime.value.end;
    if(start == null || start == ''){
      this.hasError = true;
      this.errorValue = 'Please enter the start date.';
      return;
    } 
    else start = start.toLocaleString();
    if(end == null || end == ''){
      this.hasError = true;
      this.errorValue = 'Please enter the end date.';
      return;
    }
    else end = end.toLocaleString();

    this.hasError = false;
    this.selectedTimePeriod = null;
    this.service.getEnergyConsumedInCityInRange(this.name, start, end, true).subscribe({
      next:(res) =>{
        this.consumed = res;
        this.totalConsumed = this.consumed.reduce((sum, energyItem) => sum + energyItem.value, 0);
        this.pieChartData = [this.totalProduced, this.totalConsumed];
        this.pieChart.data.datasets[0].data = this.pieChartData;
        this.pieChart.update();
        this.updateBarChartConsumed(this.consumedChart, this.consumed);

      },
      error:(err)=>{
        this.hasError = true;
        this.errorValue = err.error;
      }
    });
    this.service.getEnergyConsumedInCityInRange(this.name, start, end, false).subscribe({
      next:(res) =>{
        this.produced = res;
        this.totalProduced = this.produced.reduce((sum, energyItem) => sum + energyItem.value, 0);
        this.pieChartData = [this.totalProduced, this.totalConsumed];
        this.pieChart.data.datasets[0].data = this.pieChartData;
        this.pieChart.update();

        this.updateBarChartProduced(this.producedChart, this.produced);
      },
      error:(err)=>{
        this.hasError = true;
        this.errorValue = err.error;
      }
    })

  }
  searchByTime(totalTimePeriod: TotalTimePeriod){
    this.selectedTimePeriod = totalTimePeriod;
    this.service.getEnergyConsumedInCityByTimePeriod(this.name, totalTimePeriod, true).subscribe({
      next:(result) =>{
        this.consumed = result;
        this.totalConsumed = this.consumed.reduce((sum, energyItem) => sum + energyItem.value, 0);
        this.pieChartData = [this.totalProduced, this.totalConsumed];
        this.hasError = false;
        this.pieChart.data.datasets[0].data = this.pieChartData;
        this.pieChart.update();
        
        this.updateBarChartConsumed(this.consumedChart, this.consumed);
      },
      error:(err)=>{
        this.hasError = true;
        this.errorValue = err.error;
      }
    });
    this.service.getEnergyConsumedInCityByTimePeriod(this.name, totalTimePeriod, false).subscribe({
      next:(result) =>{

        this.produced = result;        this.hasError = false;
        this.totalProduced = this.produced.reduce((sum, energyItem) => sum + energyItem.value, 0);
        this.pieChartData = [this.totalProduced, this.totalConsumed];
        this.pieChart.data.datasets[0].data = this.pieChartData;
        this.pieChart.update();
        this.updateBarChartProduced(this.producedChart, this.produced);
      },
      error:(err)=>{
        this.hasError = true;
        this.errorValue = err.error;
      }
    });
  }

  updateBarChartConsumed(chart:Chart, energyConsumed: EnergyDTO[]) {
    const timestamps = energyConsumed.map(item => {
      // Assuming item.timestamp is a string, parse it into a Date object
      const dateFromBackend = new Date(item.timeStamp);

      // Format the Date object
      const formattedTimestamp = dateFromBackend.toLocaleString('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      });

      return formattedTimestamp;
    });
    const consumedValues = energyConsumed.map(item => Math.round(item.value * 100) / 100);
    this.tooltipNonRealTimeData = energyConsumed.map(item => ({
      timestamp: item.timeStamp,
      consumedValues: Math.round(item.value * 100) / 100
    }));

    chart.data.labels = timestamps;
    chart.data.datasets[0].data = consumedValues;
    chart.update();

  }
  updateBarChartProduced(chart:Chart, energyConsumed: EnergyDTO[]) {
    const timestamps = energyConsumed.map(item => {
      // Assuming item.timestamp is a string, parse it into a Date object
      const dateFromBackend = new Date(item.timeStamp);

      // Format the Date object
      const formattedTimestamp = dateFromBackend.toLocaleString('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      });

      return formattedTimestamp;
    });
    const consumedValues = energyConsumed.map(item => Math.round(item.value * 100) / 100);
    this.tooltipProduced = energyConsumed.map(item => ({
      timestamp: item.timeStamp,
      consumedValues: Math.round(item.value * 100) / 100
    }));

    chart.data.labels = timestamps;
    chart.data.datasets[0].data = consumedValues;
    chart.update();

  }
}
