import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartDeviceService } from '../smart-device.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RegisterSmartDeviceDTO } from 'src/app/shared/models/SmartDevice';
import { TypeOfPowerSupply } from 'src/app/shared/models/PowerSupply';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register-vehicle-gate',
  templateUrl: './register-vehicle-gate.component.html',
  styleUrls: ['./register-vehicle-gate.component.css']
})
export class RegisterVehicleGateComponent {

  registerForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
      usage: new FormControl('0', [Validators.pattern(/^[0-9]+(\.[0-9]+)?$/)]),
      powerSupply: new FormControl('Battery'),
    }
  );
  hasError = false;
  errorValue: string = '';
  smartPropertyId: string = '';
  base64Image: string = '';
  imageType: string = 'jpg';
  constructor(private route: ActivatedRoute, 
    private router: Router, 
    private smartDeviceService: SmartDeviceService,
    private snackBar:SnackbarService) {

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
    if (this.registerForm.value.powerSupply === 'Battery') {
      this.registerForm.value.usage = '0';
    }else if (this.registerForm.value.powerSupply === 'Grid' && !this.registerForm.value.usage) {
      this.registerForm.value.usage = '0';
    }
    if(this.base64Image.trim() === ''){
      this.hasError = true;
      this.errorValue = 'Please choose an image.';
      return;
    }
    var device: RegisterSmartDeviceDTO = {
      name: this.registerForm.value.name,
      powerUsage: Number.parseFloat(this.registerForm.value.usage),
      powerSupply: this.registerForm.value.powerSupply === 'Battery' ? TypeOfPowerSupply.Battery : TypeOfPowerSupply.Grid,
      imageType: this.imageType,
      image: this.base64Image,
      smartPropertyId: this.smartPropertyId
    };
    this.smartDeviceService.registerVehicleGate(device).subscribe({
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
