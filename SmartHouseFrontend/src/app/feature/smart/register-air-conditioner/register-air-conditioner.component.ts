import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartDeviceService } from '../smart-device.service';
import { Mode, RegisterAirConditionerDTO } from 'src/app/shared/models/AirConditioner';
import { TypeOfPowerSupply } from 'src/app/shared/models/PowerSupply';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register-air-conditioner',
  templateUrl: './register-air-conditioner.component.html',
  styleUrls: ['./register-air-conditioner.component.css']
})
export class RegisterAirConditionerComponent {

  registerForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
      usage: new FormControl('0', [Validators.pattern(/^[0-9]+(\.[0-9]+)?$/)]),
      minValue: new FormControl('14', [Validators.min(14)]),
      maxValue: new FormControl('30', [Validators.max(30)]),
      powerSupply: new FormControl('Battery')
    }
  );
  hasError = false;
  isSuceess = false;
  successValue = '';
  errorValue: string = '';
  smartPropertyId: string = '';
  base64Image: string = '';
  imageType: string = 'jpg';
  modesArray = ["Cooling", "Heating", "Automatic", "Ventilation"];
  modes: Mode[] = [Mode.Automatic, Mode.Cooling, Mode.Heating, Mode.Ventilation];

  constructor(private route: ActivatedRoute,
    private router: Router,
    private smartDeviceService: SmartDeviceService,
    private snackBar: SnackbarService) {

  }
  getModeEnumValue(modeLabel: string): Mode {
    return Mode[modeLabel];
  }

  toggleMode(modeLabel: string): void {
    const mode = this.getModeEnumValue(modeLabel);
    if (this.modes.includes(mode)) {
      // Mode is selected, remove it
      const updatedModes = this.modes.filter((m) => m !== mode);
      this.modes = updatedModes;
      
    } else {
      this.modes.push(mode);
    }
  

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
    }
    else if (this.registerForm.value.powerSupply === 'Grid' && !this.registerForm.value.usage) {
      this.registerForm.value.usage = '0';
    }
    if (this.registerForm.value.powerSupply === 'Grid' && this.registerForm.value.usage === '') {
      this.hasError = true;
      this.errorValue = 'Please enter the usage in KWH.';
    }
    if (this.base64Image.trim() === '') {
      this.hasError = true;
      this.errorValue = 'Please choose an image.';
      return;
    }
    if(this.modes.length < 1){
      this.hasError = true;
      this.errorValue = 'Please select atleast 1 mode';
      return;
    }
    let minTemp = Number.parseFloat(this.registerForm.value.minValue);
    let maxTemp = Number.parseFloat(this.registerForm.value.maxValue);
    if(minTemp > maxTemp){
      this.hasError = true;
      this.errorValue = 'Minimum temperature must be less than maximum temperature.';
      return;
    }
    var ac: RegisterAirConditionerDTO = {
      name: this.registerForm.value.name,
      powerUsage: Number.parseFloat(this.registerForm.value.usage),
      powerSupply: this.registerForm.value.powerSupply === 'Battery' ? TypeOfPowerSupply.Battery : TypeOfPowerSupply.Grid,
      imageType: this.imageType,
      image: this.base64Image,
      smartPropertyId: this.smartPropertyId,
      minTemperature: minTemp,
      maxTemperature: maxTemp,
      modes: this.modes
    };
    this.smartDeviceService.registerAirConditioner(ac).subscribe({
      next: (res) => {
        this.hasError = false;
        this.snackBar.showSnackBar('Successfully registered new device with name: '+ ac.name, "Ok");
      },
      error: (err) => {
        this.hasError = true;
        if(err.errors)
          this.errorValue = err.errors[0];
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
