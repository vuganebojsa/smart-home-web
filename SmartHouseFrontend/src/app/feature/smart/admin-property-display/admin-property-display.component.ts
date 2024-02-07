import { HttpResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { Activation } from 'src/app/shared/models/Activation';
import { PagedList } from 'src/app/shared/models/Pagination';
import { SmartPropertyListDTO, SmartPropertyWithUserDTO } from 'src/app/shared/models/SmartProperty';
import { SmartPropertyService } from '../smart-property.service';
import { CityDTO } from 'src/app/shared/models/City';

@Component({
  selector: 'app-admin-property-display',
  templateUrl: './admin-property-display.component.html',
  styleUrls: ['./admin-property-display.component.css']
})
export class AdminPropertyDisplayComponent {
  Properties: SmartPropertyWithUserDTO[] = [];
  isLoaded = false;
  pagedList: PagedList;
  isCitiesLoaded = false;
  cities: CityDTO[] = [];
  cityValue: string = '';
  hasError = false;
  errorValue = '';
  constructor(private route: ActivatedRoute, private router: Router, private smartPropertyService: SmartPropertyService) {

  }
  ngOnInit(): void {
    this.getProperties(1, 10);
    this.smartPropertyService.getCities().subscribe(
      result =>{
        this.cities = result;
        this.isCitiesLoaded = true;
      }
    );
  }
  private getProperties(page: number, count:number) {
    this.isLoaded = false;
    this.smartPropertyService.getAdminProperties(page, count).subscribe(
      {
        next:(response :HttpResponse<SmartPropertyWithUserDTO[]>) =>{
        
            this.Properties = response.body;
            const headers = response.headers;
            const paginationData = headers.get('X-Pagination');
          
          if (paginationData) {
            const pagination = JSON.parse(paginationData);
            this.pagedList = {
              totalDevices: pagination.TotalCount,
              pageSize: pagination.PageSize,
              currentPage: pagination.CurrentPage,
              totalPages: pagination.TotalPages,
              hasNextPage: pagination.HasNext,
              hasPreviousPage: pagination.hasPreviousPage
  
            };
          }
          this.isLoaded = true;

        },
        error:(err) =>{

        }
    });

  }

  navigateToSingleProperty(propertyId: string) {
    this.router.navigate(['smart', propertyId, 'register-device']);
  }
  showProperty(property: SmartPropertyWithUserDTO){

      localStorage.setItem('selectedPropertyAdmin', JSON.stringify(property));
      this.router.navigate(['smart', property.id, 'report']);
    
  }

  onPageChange(event: PageEvent): void {
    this.getProperties(event.pageIndex + 1, event.pageSize);
  }


  showReportsForCity(){
    let valid = false;
    for(let city of this.cities){
      if(this.cityValue === city.name){
        valid = true;
        break;
      }
    }
    if(!valid){
      this.hasError = true;
      this.errorValue = 'Please enter an adequate city name.';
      return;
    }

    this.hasError = false;
    this.router.navigate(['smart', this.cityValue, 'city-report']);

  }
}
