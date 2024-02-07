import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RegisterHouseBatteryDTO } from 'src/app/shared/models/HouseBattery';
import { TypeOfPowerSupply } from 'src/app/shared/models/PowerSupply';
import { SmartDeviceService } from '../smart-device.service';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register-house-battery',
  templateUrl: './register-house-battery.component.html',
  styleUrls: ['./register-house-battery.component.css']
})
export class RegisterHouseBatteryComponent {
  registerForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
      batterySize: new FormControl('', [Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]+)?$/)]),
    }
  );
  hasError = false;
  errorValue: string = '';
  smartPropertyId: string = '';
  base64Image: string = '';
  imageType: string = 'jpg';
  constructor(private route: ActivatedRoute, private router: Router, 
    private smartDeviceService: SmartDeviceService, private snackBar: SnackbarService) {

  }
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) { this.smartPropertyId = id; }
      else { this.router.navigate(['home']); }
    })
  }
  register() {
    this.hasError = false;
    if (!this.registerForm.valid) {
      this.hasError = true;
      this.errorValue = 'Please fulfill all the fields correctly';
      return;
    }
    if(this.base64Image.trim() === ''){
      this.hasError = true;
      this.errorValue = 'Please choose an image.';
      return;
    }
    var battery: RegisterHouseBatteryDTO = {
      name: this.registerForm.value.name,
      powerUsage: 0,
      powerSupply: TypeOfPowerSupply.Grid,
      batterySize: Number.parseFloat(this.registerForm.value.batterySize),
      imageType: this.imageType,
      image: this.base64Image,
      smartPropertyId: this.smartPropertyId
    };
    this.smartDeviceService.registerBattery(battery).subscribe({
      next: (res) =>{
        this.snackBar.showSnackBar('Successfully registered new device with name: '+ res.name, "Ok");

      },
      error: (err) =>{
        this.hasError = true;
        this.errorValue = err.message;
      }
    })
  }
  base64ImageDisplay: string = '';

  onImageSelected(event: any) {
    const file: File = event.target.files[0];
    
    if (file) {
      const reader = new FileReader();
      const fileName = file.name;
      const lastDotIndex = fileName.lastIndexOf('.');
      const fileExtension = fileName.substring(lastDotIndex + 1);
      this.imageType = fileExtension;
      reader.onload = (e: any) => {
        const base64Image = e.target.result; // The base64-encoded image
        this.base64ImageDisplay = base64Image;
        const [_, imageData] = base64Image.split(',', 2);

        this.base64Image = imageData;
      };

      reader.readAsDataURL(file);
    }
  }
}
