import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SmartPropertyService } from '../smart-property.service';
import { SmartPropertyListDTO } from 'src/app/shared/models/SmartProperty';
import { Activation } from 'src/app/shared/models/Activation';
import { PagedList } from 'src/app/shared/models/Pagination';
import { PageEvent } from '@angular/material/paginator';
import { HttpEvent, HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-smart-property-list',
  templateUrl: './smart-property-list.component.html',
  styleUrls: ['./smart-property-list.component.css']
})
export class SmartPropertyListComponent implements OnInit{
  Properties: SmartPropertyListDTO[] = [];
  isLoaded = false;
  pagedList: PagedList;
  constructor(private route: ActivatedRoute, private router: Router, private smartPropertyService: SmartPropertyService) {

  }
  ngOnInit(): void {
    this.getProperties(1, 10);
  }
  private getProperties(page: number, count:number) {
    this.isLoaded = false;
    this.smartPropertyService.getUserProperties(page, count).subscribe(
      {
        next:(response :HttpResponse<SmartPropertyListDTO[]>) =>{
        
            this.Properties = response.body;
            const headers = response.headers;
            const paginationData = headers.get('X-Pagination');
            console.log(paginationData)

          
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
  showDevices(property: SmartPropertyListDTO, isAccepted:Activation){
    if(isAccepted == Activation.Accepted){
      localStorage.setItem('selectedProperty', JSON.stringify(property));
      this.router.navigate(['smart', property.id, 'devices']);
    }
  }

  onPageChange(event: PageEvent): void {
    this.getProperties(event.pageIndex + 1, event.pageSize);
  }
  
}
