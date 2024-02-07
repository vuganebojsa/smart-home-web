import {Component, OnInit} from '@angular/core';
import {Location} from "@angular/common";
import {ActivatedRoute, Router} from "@angular/router";
import {SmartDeviceService} from "../smart-device.service";
import {HubConnectionBuilder} from "@microsoft/signalr";
import {HumidityDTO, TemperatureDTO} from "../../../shared/models/AmbientSensor";
import {EnergyConsumptionDTO, TotalTimePeriod} from "../../../shared/models/Battery";
import {Chart} from "chart.js";
import {FormControl, FormGroup} from "@angular/forms";

@Component({
  selector: 'app-ambient-sensor-device',
  templateUrl: './ambient-sensor-device.component.html',
  styleUrls: ['./ambient-sensor-device.component.css']
})
export class AmbientSensorDeviceComponent implements  OnInit{
  deviceId = ''
  temperatureData: TemperatureDTO[] = [];
  realtimeChartTemperature!: Chart;
  dateRangeChartTemperature!: Chart;
  chosenDateTime = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });
  tooltipData = [];
  tooltipNonRealTimeData = [];
  hasError = false;
  errorValue = '';


  humidityData: HumidityDTO[] = [];
  realtimeChartHumidity!: Chart;
  dateRangeChartHumidity!: Chart;
  chosenDateTimeHumidity = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });
  tooltipDataHumidity = [];
  tooltipNonRealTimeDataHumidity = [];
  hasErrorHumidity = false;
  errorValueHumidity = '';
  constructor(private location: Location,private router: Router, private route: ActivatedRoute, private service: SmartDeviceService) { }

  goBack() {
    this.location.back();
  }

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.deviceId = params['id'];
    })
    this.service.getAmbientSensorTemperatureLastHour(this.deviceId).subscribe({
      next:(result) =>{
        this.temperatureData = result;
        this.InitRealtimeChart(this.temperatureData);
        this.initWebSocket();

      }
    })


    this.service.getAmbientSensorHumidityLastHour(this.deviceId).subscribe({
      next:(result) => {
        this.humidityData = result;
        this.InitRealtimeChartHumidity(this.humidityData)
      }
    })

    this.InitBarChart();
    this.InitBarChartHumidity();
  }


  showOnlineReport(){
    this.router.navigate(['smart', this.deviceId, 'online-report']);
    
  } 


  private InitRealtimeChart(data: TemperatureDTO[]) {

    const timestamps = data.map(item => {
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
    const consumedValues = data.map(item => Math.round(item.roomTemperature * 100) / 100);
    const tooltipData = data.map(item => ({
      timestamp: item.timestamp,
      consumedValues: Math.round(item.roomTemperature * 100) / 100
    }));
    this.tooltipData = tooltipData;

    this.realtimeChartTemperature = new Chart('realtimeChart', {
      type: 'line',
      data: {
        labels: timestamps,
        datasets: [{
          label: 'Temperature (celsius)',
          data: consumedValues,
          backgroundColor: ['#e10808'],
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
                return `Recorded temperature was: ${this.tooltipData[index].consumedValues} celsius`;
              }
            }
          }
        }
      }
    });
  }

  private InitRealtimeChartHumidity(data: HumidityDTO[]) {

    const timestamps = data.map(item => {
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
    const consumedValues = data.map(item => Math.round(item.roomHumidity * 100) / 100);
    const tooltipData = data.map(item => ({
      timestamp: item.timestamp,
      consumedValues: Math.round(item.roomHumidity * 100) / 100
    }));
    this.tooltipDataHumidity = tooltipData;

    this.realtimeChartHumidity = new Chart('realtimeChartHumidity', {
      type: 'line',
      data: {
        labels: timestamps,
        datasets: [{
          label: 'Percentage',
          data: consumedValues,
          backgroundColor: ['#08A8E1'],
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
                return `Recorded humidity was: ${this.tooltipDataHumidity[index].consumedValues}%`;
              }
            }
          }
        }
      }
    });
  }

  private InitBarChart() {

    this.dateRangeChartTemperature = new Chart('barChart', {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: 'Temperature (celsius)',
          data: [],
          backgroundColor: ['#e10808'],
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
                return `Recorded temperature was: ${this.tooltipNonRealTimeData[index].consumedValues} celsius`;
              }
            }
          }
        }
      }
    });
  }

  private InitBarChartHumidity() {

    this.dateRangeChartHumidity = new Chart('barChartHumidity', {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: 'Percentage',
          data: [],
          backgroundColor: ['#08A8E1'],
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
                return `Recorded humidity was: ${this.tooltipNonRealTimeDataHumidity[index].consumedValues}%`;
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
      this.errorValue = 'Please enter start date.'
      return;
    }
    else start = start.toLocaleString();
    if(end == null || end == ''){
      this.hasError = true;
      this.errorValue = 'Please enter end date.'
      return;
    }
    else end = end.toLocaleString();
    this.service.getAmbientSensorTemperatureFromTo(this.deviceId, start, end).subscribe({
      next:(result) =>{
        this.updateBarChart(result);
        this.hasError = false;
      },
      error:(err) =>{
        this.hasError = true;
        this.errorValue = err.error;
      }
    })
  }

  searchByTime(totalTimePeriod: TotalTimePeriod){
    this.service.getAmbientSensorTemperatureInTimePeriod(this.deviceId, totalTimePeriod).subscribe({
      next:(result) =>{
        this.updateBarChart(result);
      },
      error:(err)=>{

      }
    })
  }
  updateBarChart(temperatureData: TemperatureDTO[]) {
    const timestamps = temperatureData.map(item => {
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
    const consumedValues = temperatureData.map(item => Math.round(item.roomTemperature* 100) / 100);
    this.tooltipNonRealTimeData = temperatureData.map(item => ({
      timestamp: item.timestamp,
      consumedValues: Math.round(item.roomTemperature * 100) / 100
    }));

    this.dateRangeChartTemperature.data.labels = timestamps;
    this.dateRangeChartTemperature.data.datasets[0].data = consumedValues;
    this.dateRangeChartTemperature.update();
  }




  submitHumidity(){
    if(!this.chosenDateTimeHumidity.valid){
      return;
    }
    let start = this.chosenDateTimeHumidity.value.start;
    let end = this.chosenDateTimeHumidity.value.end;
    if(start == null || start == ''){
      this.hasErrorHumidity = true;
      this.errorValueHumidity = 'Please enter start date.'
      return;
    }
    
    else start = start.toLocaleString();
    if(end == null || end == ''){
      this.hasErrorHumidity = true;
      this.errorValueHumidity = 'Please enter end date.'
      return;
    }
    else end = end.toLocaleString();
    this.service.getAmbientSensorHumidityFromTo(this.deviceId, start, end).subscribe({
      next:(result) =>{
        this.updateBarChartHumidity(result);
        this.hasErrorHumidity = false;
      },
      error:(err) =>{
        this.hasErrorHumidity = true;
        this.errorValueHumidity = err.error;
      }
    })
  }


  searchByTimeHumidity(totalTimePeriod: TotalTimePeriod){
    this.service.getAmbientSensorHumidityInTimePeriod(this.deviceId, totalTimePeriod).subscribe({
      next:(result) =>{
        this.updateBarChartHumidity(result);
      },
      error:(err)=>{

      }
    })
  }


  updateBarChartHumidity(humidityData: HumidityDTO[]) {
    const timestamps = humidityData.map(item => {
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
    const consumedValues = humidityData.map(item => Math.round(item.roomHumidity * 100) / 100);
    this.tooltipNonRealTimeDataHumidity = humidityData.map(item => ({
      timestamp: item.timestamp,
      consumedValues: Math.round(item.roomHumidity * 100) / 100
    }));

    this.dateRangeChartHumidity.data.labels = timestamps;
    this.dateRangeChartHumidity.data.datasets[0].data = consumedValues;
    this.dateRangeChartHumidity.update();
  }


  initWebSocket() {

    let connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7217/hubs/sensorData?deviceId=' + this.deviceId)
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMessage', (from: string, message: string) => {
      let value = JSON.parse(from);
      let temperature = value.temperature;
      let humidity = value.humidity;

      temperature = Math.round(temperature* 100) / 100;
      humidity = Math.round(humidity * 100) / 100;

      let timestamp = new Date();
      let formattedTimestamp = timestamp.toLocaleString('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      });

      this.realtimeChartTemperature.data.labels.push(formattedTimestamp);
      this.realtimeChartTemperature.data.datasets[0].data.push(temperature);
      const tooltipItem = {
        timestamp: formattedTimestamp,
        consumedValues: temperature
      };
      this.tooltipData.push(tooltipItem);
      this.realtimeChartTemperature.update();


      this.realtimeChartHumidity.data.labels.push(formattedTimestamp);
      this.realtimeChartHumidity.data.datasets[0].data.push(humidity);
      const tooltipItemHumidity = {
        timestamp: formattedTimestamp,
        consumedValues: humidity
      };
      this.tooltipDataHumidity.push(tooltipItemHumidity);
      this.realtimeChartHumidity.update();
    });

    connection.start()
      .then(() => {
      })
      .catch(err => {
      });

  }




}
