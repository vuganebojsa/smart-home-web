import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartDeviceService } from '../smart-device.service';
import { SnackbarService } from 'src/app/core/services/snackbar.service';
import { Location } from '@angular/common';
import { each } from 'chart.js/dist/helpers/helpers.core';
import { PageEvent } from '@angular/material/paginator';
@Component({
  selector: 'app-sprinkler-device',
  templateUrl: './sprinkler-device.component.html',
  styleUrls: ['./sprinkler-device.component.css']
})
export class SprinklerDeviceComponent  implements OnInit{

  constructor(private location: Location, private route: ActivatedRoute, private router: Router, private smartDeviceService: SmartDeviceService, private snackBar: SnackbarService) { }
  
 selectedTimePeriod = 0;
  error = false;
  errorMessageStart = false;
  errorMessageEnd = false;
  errorMessage = '';
  startTime = '';
  endTime = '';
  day = '';
  id = '';
  pageSize =5;
  currentPage = 1;
  visibleEvents = [];
  sprinklerEvents = [];
  isOn = false;
  selectedTimeStart;
  selectedTimeEnd;
  errorStartDate = false;
  errorStartDateTooBig = false;
  specialMode = false;
  activeDays = [];
  isOnline = true;
  allDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  availableDays = [];
  newDayForm = new FormGroup({
    day: new FormControl('', [Validators.required]),
  });
  chosenDateTime = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
    username: new FormControl(null)
  });

 

  ngOnInit(): void {
    
    this.route.params.subscribe((params) => {
      this.id = params['id'];
      console.log(this.id);
    });
    this.loadActiveDays();

    this.smartDeviceService.getSprinklerInfo(this.id).subscribe({
      next: (result) => {
        this.specialMode = result.isPublic;
        this.isOn = result.isOn;
        this.isOnline = result.isOnline;
        this.startTime = result.startTime;
        this.endTime = result.endTime;

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
    this.getSprinklerEventHistory(sixHourAgoISOTime, currentISOTime, null);
    
  }

  onPageChangeEvent(event: PageEvent): void {
    this.pageSize = event.pageSize
    this.currentPage = event.pageIndex + 1;
    this.updateViewEvent();
  }

  updateViewEvent(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.visibleEvents = this.sprinklerEvents.slice(startIndex, endIndex);
  }
  
  removeActiveDaysFromAvailableDays() {
    
    this.availableDays = this.allDays.filter(day => !this.activeDays.includes(day));
  }
  onTimeSetStart(event: any) {
    
    this.selectedTimeStart = event
    this.errorMessageStart = false
    
  }
  showOnlineReport(){
    this.router.navigate(['smart', this.id, 'online-report']);
    
  } 

  public ChooseTime(option: number) {
    this.errorStartDate = false
    this.errorStartDateTooBig = false
  const today = new Date();
  if (option == 0) {
    this.selectedTimePeriod =0;
    
    const sixHoursAgo = new Date(today.getTime() - 6 * 60 * 60 * 1000);
    if (!this.chosenDateTime.get('username').valid){
      this.getSprinklerEventHistory(sixHoursAgo.toISOString(), today.toISOString(), null);
    }else{
      this.getSprinklerEventHistory(sixHoursAgo.toISOString(), today.toISOString(), this.chosenDateTime.value.username);
    }

  }
  else if (option == 1) {
    this.selectedTimePeriod =1;

    const twelveHoursAgo = new Date(today.getTime() - 12 * 60 * 60 * 1000);
    if (!this.chosenDateTime.get('username').valid){
      this.getSprinklerEventHistory(twelveHoursAgo.toISOString(), today.toISOString(), null);
    }else{
      this.getSprinklerEventHistory(twelveHoursAgo.toISOString(), today.toISOString(), this.chosenDateTime.value.username);
    }
  }
  else if (option == 2) { 
    this.selectedTimePeriod =2;

    const dayAgo = new Date(today.getTime() - 24 * 60 * 60 * 1000);
    if (!this.chosenDateTime.get('username').valid){
      this.getSprinklerEventHistory(dayAgo.toISOString(), today.toISOString(), null);
    }else{
      this.getSprinklerEventHistory(dayAgo.toISOString(), today.toISOString(), this.chosenDateTime.value.username);
    }

  }
}

Submit() {
  if (this.chosenDateTime.get('start').valid && this.chosenDateTime.get('end').valid) {
    this.errorStartDateTooBig = false;
    if (this.chosenDateTime.value.start == null){
      this.errorStartDate = true;
      return
    }
    this.errorStartDate = false
    
    let username = this.chosenDateTime.value.username
    let localStartDate = this.chosenDateTime.value.start;
    let localEndDate = this.chosenDateTime.value.end;
    if (localEndDate == null) {
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
    if (!this.chosenDateTime.get('username').valid){
      this.getSprinklerEventHistory(startISOString, endISOString, null);
      
    }else{
      this.getSprinklerEventHistory(startISOString, endISOString, this.chosenDateTime.value.username);
    }
    this.selectedTimePeriod =4;

    this.updateViewEvent();



  }
}

  private getSprinklerEventHistory(startDate: string, endDate: string, username: string | null) {

    this.smartDeviceService.getSprinklerEventReport(this.id, startDate, endDate, username).subscribe({
      next: (result) => {
        this.sprinklerEvents = result;
        this.sprinklerEvents.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
        this.updateViewEvent();
      },
      error: (err) => {
        console.log(err.error);
      }
    });
  }

  onSetStartTime() {
    if (this.selectedTimeStart == undefined){
      this.errorMessageStart = true
     
      return
    }
  
    this.smartDeviceService.changeSprinklerStartTime(this.id, this.selectedTimeStart).subscribe({
      next: (result) => {
        console.log(result)
        this.snackBar.showSnackBar('Successfully changed start time', "Ok");
        this.startTime = result["startTime"]
      },
      error: (err) => {
        console.log(err)
        
      }
    })
  }

  onTimeSetEnd(event: any) {
    console.log('Selected time:', event);
    this.selectedTimeEnd = event
    this.errorMessageEnd = false
    
  }
  onSetEndTime() {
    if (this.selectedTimeEnd == undefined){
      this.errorMessageEnd = true
      return
    }

    this.smartDeviceService.changeSprinklerEndTime(this.id, this.selectedTimeEnd).subscribe({
      next: (result) => {
        this.snackBar.showSnackBar('Successfully changed end time', "Ok");
        this.endTime = result["startTime"]      },
      error: (err) => {
      }
    })
  }
 
  addSprinklerDay(): void {
    if (this.newDayForm.valid) {
      console.log(this.newDayForm.value.day)
      this.smartDeviceService.addSprinklerDay(this.id, this.newDayForm.value.day).subscribe({
        next: (result) => {
          
          this.loadActiveDays()
          this.availableDays = this.availableDays.filter(day => !this.newDayForm.value.day);
          this.newDayForm.get('day').setValue('');
          this.snackBar.showSnackBar('Successfully added new sprinkler day.', "Ok");

        },
        error: (err) => {
          this.error = true;
          this.errorMessage = err.error;
          this.newDayForm.get('day')?.valueChanges.subscribe((day) => {
            this.error = false;
          })
        }
      })
    }
  }
  goBack() {
    this.location.back();
  }

  ChangeSpecialMode(newStatus: boolean) {

    this.smartDeviceService.changeSpecialModeSprinkler(this.id, newStatus).subscribe({
      next: (result) => {
        this.specialMode = result;
      },
      error: (err) => {
      }
    })
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

  private loadActiveDays(): void {
    this.smartDeviceService.getActiveDays(this.id).subscribe({
      next: (result) => {
        if(result != null)
          this.activeDays = (result);
        this.removeActiveDaysFromAvailableDays();
      },
      error: (err) => {
        // Handle error
      }
    });
  }

  deleteSprinklerDay(day: string): void {
    this.smartDeviceService.removeSprinklerDay(this.id, day).subscribe({
      next: (result) => {
        this.loadActiveDays()
       
      },
      error: (err) => {
        this.loadActiveDays()
        this.snackBar.showSnackBar('Successfully deleted sprinkler day.', "Ok");
        
        console.log(err)
      }
    })


  }
}
