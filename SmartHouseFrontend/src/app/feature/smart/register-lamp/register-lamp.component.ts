import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RegisterLampDTO } from 'src/app/shared/models/Lamp';
import { TypeOfPowerSupply } from 'src/app/shared/models/PowerSupply';
import { SmartDeviceService } from '../smart-device.service';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register-lamp',
  templateUrl: './register-lamp.component.html',
  styleUrls: ['./register-lamp.component.css']
})
export class RegisterLampComponent implements OnInit {
  registerForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
      usage: new FormControl('0', [Validators.pattern(/^[0-9]+(\.[0-9]+)?$/)]),
      luminosity: new FormControl('', [Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]+)?$/)]),
      powerSupply: new FormControl('Battery'),
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
    if (this.registerForm.value.powerSupply === 'Battery') {
      this.registerForm.value.usage = '0';
    }
    else if (this.registerForm.value.powerSupply === 'Grid' && !this.registerForm.value.usage) {
      this.registerForm.value.usage = '0';
    }
    var lamp: RegisterLampDTO = {
      name: this.registerForm.value.name,
      powerUsage: Number.parseFloat(this.registerForm.value.usage),
      powerSupply: this.registerForm.value.powerSupply === 'Battery' ? TypeOfPowerSupply.Battery : TypeOfPowerSupply.Grid,
      luminosity: Number.parseFloat(this.registerForm.value.luminosity),
      imageType: this.imageType,
      image: this.base64Image,
      smartPropertyId: this.smartPropertyId
    };
    this.smartDeviceService.registerLamp(lamp).subscribe({
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
