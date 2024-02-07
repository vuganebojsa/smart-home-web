import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RegisterLampDTO } from 'src/app/shared/models/Lamp';
import { TypeOfPowerSupply } from 'src/app/shared/models/PowerSupply';
import { SmartDeviceService } from '../smart-device.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PanelDTO, RegisterSolarPanelSystemDTO } from 'src/app/shared/models/SolarPanelSystem';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register-solar-panel-system',
  templateUrl: './register-solar-panel-system.component.html',
  styleUrls: ['./register-solar-panel-system.component.css']
})
export class RegisterSolarPanelSystemComponent {
  registerForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    }
  );
  addPanelForm = new FormGroup(
    {
      size: new FormControl('0', [Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]+)?$/), Validators.min(0), Validators.max(300)]),
      efficency: new FormControl('0', [Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]+)?$/), Validators.min(0), Validators.max(100)]),
    }
  );
  hasError = false;
  errorValue: string = '';
  smartPropertyId: string = '';
  base64Image: string = '';
  imageType: string = 'jpg';
  panels: PanelDTO[] = [];
  constructor(private route: ActivatedRoute, private router: Router, 
    private smartDeviceService: SmartDeviceService, private snackBar: SnackbarService) {

  }
  addPanel():void{
    let size = Number.parseFloat(this.addPanelForm.value.size);
    let efficency = Number.parseFloat(this.addPanelForm.value.efficency);
    this.hasError = false;
    if(efficency > 100 || efficency < 0){
      this.hasError = true;
      this.errorValue = 'Efficency must be in range of 0 and 100';
      return;
    }
    if(size < 0){
      this.hasError = true;
      this.errorValue = 'Size must be above 0 m^2';
      return;
    }
    this.addPanelForm.value.efficency = '0';
    this.addPanelForm.value.size = '0';
    this.panels.push({size: size, efficency: efficency});
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
    if(this.panels.length < 1){
      this.hasError = true;
      this.errorValue = 'Please add atleast 1 panel.';
      return;
    }
    var solarPanelSystem: RegisterSolarPanelSystemDTO = {
      name: this.registerForm.value.name,
      powerUsage: 0,
      powerSupply: TypeOfPowerSupply.Battery,
      imageType: this.imageType,
      image: this.base64Image,
      smartPropertyId: this.smartPropertyId,
      panels: this.panels
    };
    this.smartDeviceService.registerSolarPanelSystem(solarPanelSystem).subscribe({
      next: (res) =>{
        this.snackBar.showSnackBar('Successfully registered new device with name: '+ res.name, "Ok");

        this.resetForms();
      },
      error: (err) =>{
        this.hasError = true;
        this.errorValue = err.message;
      }
    })
  }

  resetForms():void{
    this.panels.length = 0;
    this.addPanelForm.reset();
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
