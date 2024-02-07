import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartPropertyService } from '../smart-property.service';
import { CityDTO } from 'src/app/shared/models/City';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SmartPropertyRegisterDTO } from 'src/app/shared/models/SmartProperty';
import { TypeOfProperty } from 'src/app/shared/models/TypeOfProperty';
import { MapService } from 'src/app/core/services/map.service';
import { MapComponent } from '../map/map.component';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register-property',
  templateUrl: './register-property.component.html',
  styleUrls: ['./register-property.component.css']
})
export class RegisterPropertyComponent implements OnInit {
  @ViewChild(MapComponent) private mapComponent: MapComponent;
  cities: CityDTO[];
  city: string = '';
  private userChangedCity: boolean = true;
  cityValue: string;
  selectedCity: string = '';
  selectedCountry: string = '';
  addressValue: string;
  stringCities: string[] = [];
  protected readonly document = document;
  imageSrcDisplay: string = "";
  public latitude: number;
  public longitude: number;
  hasError = false;
  errorValue = '';
  isLoaded = false;

  registerForm = new FormGroup({
    imageType: new FormControl('', [Validators.required]),
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    image: new FormControl('', [Validators.required]),
    propertyType: new FormControl('', [Validators.required]),
    city: new FormControl('',[Validators.required]),
    quadrature: new FormControl('', [Validators.required, Validators.min(0), Validators.max(100000)]),
    address: new FormControl('', [Validators.required]),
    numberOfFloors: new FormControl('', [Validators.required, Validators.min(0), Validators.max(1000)]),
  });

  constructor(private route: ActivatedRoute, private router: Router,
     private smartPropertyService: SmartPropertyService, private mapService: MapService,
     private snackBar: SnackbarService) { }

  ngOnInit(): void {
    this.registerForm.get('propertyType').setValue("HOUSE");
      
    this.registerForm.get('city')?.valueChanges.subscribe((city) => {
      
      if (this.userChangedCity) {
        this.cities.forEach(element => {

          if (element.name == city.split(",")[0]) {
            this.latitude = element.latitude;
            this.longitude = element.longitude;
            this.mapService.reverseSearch(this.latitude, this.longitude).subscribe(data => {
              this.addressValue = data.address.road;
              this.selectedCountry = data.address.country;
              let city = data.address.city;
              
             
              if (city == undefined) {
                city = data.address.city_district;

              }
              if (city == undefined) {
                city = data.address.village;
              }
              if (city == undefined) {
                city = data.address.town;
              }
              if (city == undefined) {
                city = data.address.municipality;
              }
              if (city == undefined) {
                city = data.address.county;
              }
              this.selectedCity = city;
            });
           
           
            
            this.mapComponent.updateMapView(this.latitude, this.longitude);
          }




        });
      }
      this.userChangedCity = true;

    });

    this.smartPropertyService.getCities().subscribe({
      
      next: (result) => {
        
        this.cities = result;
        this.cities.forEach(city => {

          let stringCity = city.name + "," + city.country
          this.stringCities.push(stringCity);
        });
        this.isLoaded = true;
        
      },
      error: (error) => {
     
      }
    });
  }
  public onCoordinatesClicked(coordinates: { lat: number, lon: number, country: string, city: string, address: string }) {
    this.userChangedCity = false;
    this.latitude = coordinates.lat;
    this.longitude = coordinates.lon;
    this.selectedCity = coordinates.city;
    this.selectedCountry = coordinates.country;
    this.addressValue = coordinates.address;
    this.cityValue = this.selectedCity + "," + this.selectedCountry;
  
  }



  onFileChange($event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files[0]) {

      const file = target.files[0];
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        if (typeof reader.result === "string") {
          const substrings = reader.result.split(",");
          if (!(reader.result.includes("image/jpeg") || reader.result.includes("image/jpg") || reader.result.includes("image/png"))) {
            this.hasError = true;
            this.errorValue = 'You can only upload jpg, or png files';
            return;
          }
          this.imageSrcDisplay = reader.result;
          
          const parts = substrings[0].split(":");
          const mediaType = parts[1].split(";")[0];
          let fileExtension = mediaType.split("/")[1];
          if (fileExtension == "jpeg") fileExtension = "jpg"

          this.registerForm.patchValue({ image: substrings[1], imageType: fileExtension })
        }

      };
    }
  }
  register() {
    let propertyType: TypeOfProperty;
    if(this.longitude == null || this.latitude == null){
      this.hasError = true;
        this.errorValue = 'Choose a city on the map.';
      return
    }
    if (this.registerForm.value.name.length < 3){
      this.hasError = true;
      this.errorValue = 'Name must have atleast 3 characters.';
      return
    }
    if (Number(this.registerForm.value.numberOfFloors) < 0 ||Number(this.registerForm.value.numberOfFloors)>1000){
      this.hasError = true;
      this.errorValue = 'Number of floors must be between 0 and 1000.';
      return
    }
    if (Number(this.registerForm.value.quadrature) < 0 ||Number(this.registerForm.value.quadrature)>100000){
      this.hasError = true;
      this.errorValue = 'Quadrature must be between 0 and 100000.';
      return
    }
    if (this.registerForm.valid && this.selectedCountry != null) {
      if (this.registerForm.value.propertyType == "House") {
        propertyType = TypeOfProperty.House
      }
      else {
        propertyType = TypeOfProperty.Apartment
      }
      
      if (this.selectedCity == undefined) {
        this.selectedCity = "Village";
      }

      const smartProperty: SmartPropertyRegisterDTO = {
        name: this.registerForm.value.name,
        image: this.registerForm.value.image,
        imageType: this.registerForm.value.imageType,
        typeOfProperty: propertyType,
        quadrature: Number(this.registerForm.value.quadrature),
        numberOfFloors: Number(this.registerForm.value.numberOfFloors),
        address: this.registerForm.value.address,
        city: this.selectedCity,
        country: this.selectedCountry,
        latitude: this.latitude,
        longitude: this.longitude
      }
      this.smartPropertyService.registerSmartProperty(smartProperty).subscribe({
        next: value => {
          this.hasError = false;
          this.snackBar.showSnackBar("Successfully registered your property.", "Ok");
        }, error: err => {
          this.hasError = true;
          this.errorValue = err.error;
        }
      });
    } else {
      this.hasError = true;
        this.errorValue = 'All fields must be filled.';
    }

  }

}


