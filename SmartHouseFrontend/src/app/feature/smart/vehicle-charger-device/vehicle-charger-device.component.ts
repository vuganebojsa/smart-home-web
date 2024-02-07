import { Component } from '@angular/core';
import { SmartDeviceService } from '../smart-device.service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { Location } from '@angular/common';
import { SmartDeviceDTO } from 'src/app/shared/models/SmartDevice';
import { VehicleChargerDTO } from 'src/app/shared/models/VehicleCharger';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-vehicle-charger-device',
  templateUrl: './vehicle-charger-device.component.html',
  styleUrls: ['./vehicle-charger-device.component.css']
})
export class VehicleChargerDeviceComponent {
  constructor(private location: Location,
    private smartDeviceService: SmartDeviceService,
    private route: ActivatedRoute,
    private snackBar:SnackbarService,
    private router: Router) { }

    goBack() {
      this.location.back();
    }

    showOnlineReport(){
      this.router.navigate(['smart', this.id, 'online-report']);
      
    } 
    chargePercentageForm = new FormGroup(
      {
        chargePercentage: new FormControl('100', [Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]+)?$/), Validators.min(50), Validators.max(100)])
      }
    );
    device: SmartDeviceDTO = null;
    charger: VehicleChargerDTO;
    id:string = '';
    hasLoaded = false;
    ngOnInit(): void {
      this.route.params.subscribe((params) => {
        this.id = params['id'];
      });
      this.device = JSON.parse(localStorage.getItem('selectedDevice')) as SmartDeviceDTO;

      this.getVehicleCharger();
    }
  getVehicleCharger() {
    this.smartDeviceService.getVehicleCharger(this.id).subscribe({
      next:(result)=>{
        this.charger = result;
        this.smartDeviceService.getPicture(this.charger.pathToImage).subscribe(result =>{
          const url = URL.createObjectURL(result);
          (document.getElementById('profilna') as HTMLImageElement).src = url;
      });
        this.hasLoaded = true;
        this.chargePercentageForm.patchValue({chargePercentage: this.charger.percentageOfCharge.toString()});
      },
      error:(err) =>{
        console.log(err.error);
      }
    })
  }

  setChargePercentage():void{
    if(!this.chargePercentageForm.valid){
      return;
    }
    this.smartDeviceService.setChargePercentage(Number.parseFloat(this.chargePercentageForm.value.chargePercentage), this.id).subscribe({
      next:(res) =>{
        this.charger.percentageOfCharge = res;
        this.snackBar.showSnackBar("Successfully changed charge percentage", "Ok");
      },
      error:(err) =>{

      }
    })
  }

  showReports():void{
    this.router.navigate(['smart', this.id, 'vehicle-charger-manage']);

  }
}
