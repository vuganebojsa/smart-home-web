import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Chart } from 'chart.js';
import { Location } from '@angular/common';
import { HubConnectionBuilder } from '@microsoft/signalr/dist/esm/HubConnectionBuilder';
import { ActivatedRoute } from '@angular/router';
import { EnergyConsumptionDTO, TotalTimePeriod } from 'src/app/shared/models/Battery';
import { SmartDeviceService } from '../smart-device.service';
import { SmartPropertyListDTO } from 'src/app/shared/models/SmartProperty';

@Component({
  selector: 'app-battery-device',
  templateUrl: './battery-device.component.html',
  styleUrls: ['./battery-device.component.css']
})
export class BatteryDeviceComponent implements OnInit{
  realtimeChart!: Chart;
  dateRangeChart!: Chart;
  energyConsumed: EnergyConsumptionDTO[] = [];
  chosenDateTime = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });
  selectedTimePeriod: TotalTimePeriod = TotalTimePeriod.SIX_HOURS;
  smartPropertyId: string = '';
  currentOccupation = 0;
  totalCapacity = 0;
  tooltipData = [];
  tooltipNonRealTimeData = [];
  property: SmartPropertyListDTO;
  hasError = false;
  errorValue = '';
  ngOnInit(): void {
    this.smartPropertyId = localStorage.getItem('smartPropertyId');

    this.route.params.subscribe((params) => {
      this.smartPropertyId = params['id'];
    });
    this.property = JSON.parse(localStorage.getItem('selectedProperty')) as SmartPropertyListDTO;
    this.service.getSmartPropertyConsuptionLastHour(this.smartPropertyId).subscribe({
      next:(result) =>{
        this.energyConsumed = result;
        this.InitRealtimeChart(this.energyConsumed);
      }
    })
    this.InitBarChart();
    this.searchByTime(0);
    this.initWebSocket();
  }
  initWebSocket() {

    let connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7217/hubs/energy?smartPropertyId=' + this.smartPropertyId)
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMessage', (from: string, message: string) => {
      let value = JSON.parse(from);
      let energyRequired = value.energyRequired;
      energyRequired = Math.round(energyRequired * 100) / 100;
      let timestamp = new Date();
      let formattedTimestamp = timestamp.toLocaleString('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      });
      this.realtimeChart.data.labels.push(formattedTimestamp);
      this.realtimeChart.data.datasets[0].data.push(energyRequired);
      const tooltipItem = {
        timestamp: formattedTimestamp,
        consumedValues: energyRequired
      };
      this.tooltipData.push(tooltipItem);
      this.realtimeChart.update();
    });

    connection.start()
      .then(() => {
      })
      .catch(err => {
      });

  }

  constructor(private location: Location, private route: ActivatedRoute, private service: SmartDeviceService) { }

  goBack() {
    this.location.back();
  }
  private InitRealtimeChart(energyConsumed: EnergyConsumptionDTO[]) {

    const timestamps = energyConsumed.map(item => {
      // Assuming item.timestamp is a string, parse it into a Date object
      const dateFromBackend = new Date(item.timestamp);

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
    const consumedValues = energyConsumed.map(item => Math.round(item.consumed * 100) / 100);
    const tooltipData = energyConsumed.map(item => ({
      timestamp: item.timestamp,
      consumedValues: Math.round(item.consumed * 100) / 100
    }));
    this.tooltipData = tooltipData;

    this.realtimeChart = new Chart('realtimeChart', {
      type: 'line',
      data: {
        labels: timestamps,
        datasets: [{
          label: 'Power Consumed',
          data: consumedValues,
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
                return `The system consumed: ${this.tooltipData[index].consumedValues} KWH`;
              }
            }
          }
        }
      }
    });
  }
  private InitBarChart() {

    this.dateRangeChart = new Chart('barChart', {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: 'Power Consumed',
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
                return `The system consumed: ${this.tooltipNonRealTimeData[index].consumedValues} KWH`;
              }
            }
          }
        }
      }
    });
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
    this.service.getSmartPropertyConsuptionFromTo(this.smartPropertyId, start, end).subscribe({
      next:(result) =>{
        this.updateBarChart(result);
        this.hasError = false;
        this.selectedTimePeriod = null;
      },
      error:(err) =>{
        this.hasError = true;
        this.errorValue = err.error;
      }
    })
  }

  searchByTime(totalTimePeriod: TotalTimePeriod){
    this.selectedTimePeriod = totalTimePeriod;
    this.service.getSmartPropertyConsuptionInTimePeriod(this.smartPropertyId, totalTimePeriod).subscribe({
      next:(result) =>{
        this.updateBarChart(result);
      },
      error:(err)=>{

      }
    })
  }
  updateBarChart(energyConsumed: EnergyConsumptionDTO[]) {
    const timestamps = energyConsumed.map(item => {
      // Assuming item.timestamp is a string, parse it into a Date object
      const dateFromBackend = new Date(item.timestamp);

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
    const consumedValues = energyConsumed.map(item => Math.round(item.consumed * 100) / 100);
    this.tooltipNonRealTimeData = energyConsumed.map(item => ({
      timestamp: item.timestamp,
      consumedValues: Math.round(item.consumed * 100) / 100
    }));

    this.dateRangeChart.data.labels = timestamps;
    this.dateRangeChart.data.datasets[0].data = consumedValues;
    this.dateRangeChart.update();

  }
}
