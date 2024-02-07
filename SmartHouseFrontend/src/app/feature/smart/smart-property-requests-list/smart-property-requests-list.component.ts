import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PendingSmartPropertyListDTO, ProccesRequest } from 'src/app/shared/models/SmartProperty';
import { SmartPropertyService } from '../smart-property.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { SnackbarService } from 'src/app/core/services/snackbar.service';
import { PageEvent } from '@angular/material/paginator';
import { PagedList } from 'src/app/shared/models/Pagination';
import { HttpEvent, HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-smart-property-requests-list',
  templateUrl: './smart-property-requests-list.component.html',
  styleUrls: ['./smart-property-requests-list.component.css']
})
export class SmartPropertyRequestsListComponent implements OnInit{
  Properties: PendingSmartPropertyListDTO[];
  hasLoaded: boolean = false;
  hasError: boolean = false;
  visibleProperties = [];

  pagedList: PagedList;
  isLoaded = false;
  option = '';
  selectedProperty: PendingSmartPropertyListDTO = null;
  proccesRequest: ProccesRequest = {
    Accept:false,
    Reason:"Accepted"
  }
  errorMessage: string = '';
  showDeclineReason = false;

  requestForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required]),
      address: new FormControl('', [Validators.required]),
      user: new FormControl('', [Validators.required]),
      reason: new FormControl('', [Validators.minLength(2)])
      
    }
  );
  constructor(private route: ActivatedRoute, private router: Router, 
    private smartPropertyService: SmartPropertyService,
    private snackBar: SnackbarService) {

  }
  onPageChange(event: PageEvent): void {
    this.getAllPendingProperties(event.pageIndex + 1, event.pageSize);
  }

  
  ngOnInit(): void {

    this.getAllPendingProperties(1, 10);
    

  }
  getAllPendingProperties(page: number, count:number){
    this.smartPropertyService.getPendingProperties(page, count).subscribe({
      next: (result :HttpResponse<PendingSmartPropertyListDTO[]>) => {
        this.Properties = result.body;
        const headers = result.headers;
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
      error: (error) => {
     
      }
    });
  }
  acceptPressed(){
    this.showDeclineReason = false;
    this.acceptDecline();
  }
  declinePressed(){
    this.showDeclineReason = true;
  }
  showRequest(property: PendingSmartPropertyListDTO):void{
    this.selectedProperty = property;
    this.requestForm.controls['name'].setValue( "Name: " + String(property.name));
    this.requestForm.controls['address'].setValue("Address: " + String(property.address) + ", " + String(property.city) + ", " + property.country);
    this.requestForm.controls['user'].setValue("User: " + String(property.userName));
  }
  acceptDecline():void{
    if(!this.requestForm.valid){
      this.hasError = true;
      this.errorMessage = 'Decline reason must be atleast 2 characters long.';
      return;
    }
    if(this.showDeclineReason === true && this.requestForm.value.reason.trim()===''){
      this.hasError = true;
      this.errorMessage = 'Please enter a reason for declining the request';
      return;
    }
    if(!this.showDeclineReason){
      this.proccesRequest.Accept = true;

      this.smartPropertyService.proccesPendingPropertyRequest(this.selectedProperty.id, this.proccesRequest).subscribe({
        next: (result) =>{
          
          if(this.proccesRequest.Accept === true) {this.snackBar.showSnackBar('Successfully accepted request.', "Ok");}
          else{this.snackBar.showSnackBar('Successfully declined request.', "Ok");}
          
          this.requestForm.reset();
          this.selectedProperty = null;
          return;
          
        },
        error: (myError: HttpErrorResponse) =>{
         
          if(myError.status===200) {
            if(this.proccesRequest.Accept === true) {this.snackBar.showSnackBar('Successfully accepted request.', "Ok");}
            else{this.snackBar.showSnackBar('Successfully declined request.', "Ok");}
            this.requestForm.reset();
            this.selectedProperty = null;
            return;

          }

          this.hasError = true;
          this.errorMessage = myError.error.message;
        }
      });
    }
    else{
      this.proccesRequest.Accept = false;
      this.proccesRequest.Reason = this.requestForm.value.reason;
     
      this.smartPropertyService.proccesPendingPropertyRequest(this.selectedProperty.id, this.proccesRequest).subscribe({
        next: (result) =>{
          this.hasError = false;
          if(this.proccesRequest.Accept === true) {this.snackBar.showSnackBar('Successfully accepted request.', "Ok");}
            else{this.snackBar.showSnackBar('Successfully declined request.', "Ok");}
          this.requestForm.reset();
          this.selectedProperty = null;

        },
        error: (myError: HttpErrorResponse) =>{
          if(myError.status===200) {
            if(this.proccesRequest.Accept === true) {this.snackBar.showSnackBar('Successfully accepted request.', "Ok");}
            else{this.snackBar.showSnackBar('Successfully declined request.', "Ok");}            this.requestForm.reset();
            this.selectedProperty = null;
            return;
          }
          this.hasError = true;
          this.errorMessage = myError.error.message;
        }
      })
    }


  }


}
