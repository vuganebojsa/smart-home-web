import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartDeviceService } from '../smart-device.service';
import { CycleName, Cycle, RegisterWashingMachineDTO } from 'src/app/shared/models/WashingMachine';
import { TypeOfPowerSupply } from 'src/app/shared/models/PowerSupply';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register-washing-machine',
  templateUrl: './register-washing-machine.component.html',
  styleUrls: ['./register-washing-machine.component.css']
})
export class RegisterWashingMachineComponent {

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
  selectedModes = [];
  modes: Cycle[];
  getModeEnumValue(modeLabel: number): string {
    return CycleName[modeLabel];
  }

  toggleMode(mode: Cycle): void {
    if (this.selectedModes.includes(mode)) {
      // Mode is selected, remove it
      const updatedModes = this.selectedModes.filter((m) => m !== mode);
      this.selectedModes = updatedModes;
      
    } else {
      this.selectedModes.push(mode);
    }
    

  }
  constructor(private route: ActivatedRoute, private router: Router, 
    private smartDeviceService: SmartDeviceService,
    private snackBar: SnackbarService) {

  }

  ngOnInit(): void {
    this.GetIdFromRoute();
    this.GetCycles();
  }

  private GetIdFromRoute() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) { this.smartPropertyId = id; }
      else { this.router.navigate(['home']); }
    });
  }

  private GetCycles() {
    this.smartDeviceService.getWashingMachineCycles().subscribe({
      next: (res) => {
        this.modes = res;
        this.selectedModes = this.modes;
      },
      error: err => {
        this.hasError = true;
        this.errorValue = err.message;
      }
    });
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
    if(this.base64Image.trim() === ''){
      this.hasError = true;
      this.errorValue = 'Please choose an image.';
      return;
    }
    if(this.selectedModes.length < 1){
      this.hasError = true;
      this.errorValue = 'Please select atleast 1 cycle';
      return;
    }
    const cycleIds = this.selectedModes.map(mode => mode.id);

    var washingMchine: RegisterWashingMachineDTO = {
      name: this.registerForm.value.name,
      powerUsage: Number.parseFloat(this.registerForm.value.usage),
      powerSupply: this.registerForm.value.powerSupply === 'Battery' ? TypeOfPowerSupply.Battery : TypeOfPowerSupply.Grid,
      imageType: this.imageType,
      image: this.base64Image,
      smartPropertyId: this.smartPropertyId,
      supportedCycles: cycleIds
    };
    this.smartDeviceService.registerWashingMachine(washingMchine).subscribe({
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
