import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { SnackbarService } from 'src/app/core/services/snackbar.service';
import { ChangePasswordDTO } from 'src/app/shared/models/User';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  editPasswordForm = new FormGroup(
    {
      oldPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
      newPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
      repeatNewPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
    }
  );
  hasError = false;
  errorValue: string = '';
  public constructor(private router: Router, private service: AuthenticationService, private snackBar: SnackbarService){

  }
  goBack(){
    this.router.navigate(['manage/profile']);
  }
  editPassword(){
    if(!this.editPasswordForm.valid){
      this.errorValue = 'Please fulfill all the fields correctly.';
      this.hasError = true;
      return;
    }
    if(this.editPasswordForm.value.newPassword === this.editPasswordForm.value.oldPassword){
      this.errorValue = 'The old and new password are the same.';
      this.hasError = true;
      return;
    }
    if(this.editPasswordForm.value.newPassword !== this.editPasswordForm.value.repeatNewPassword){
      this.errorValue = 'The new and repeat new passwords dont match.';
      this.hasError = true;
      return;
    }
    let passInfo: ChangePasswordDTO = {
      oldPassword: this.editPasswordForm.value.oldPassword,
      newPassword: this.editPasswordForm.value.newPassword,
      repeatNewPassword: this.editPasswordForm.value.repeatNewPassword,
    };
    this.service.editPassword(passInfo).subscribe({
      next:(result) =>{
        if(result === true){
          this.hasError = false;

         
          this.snackBar.showSnackBar('Successfully changed password.', "Ok");

          this.goBack();
        }else{
          this.errorValue = 'Something went wrong. Please try again.';
          this.hasError = true;
          
        }
      },
      error:(err) =>{
        this.errorValue = err.error;
        this.hasError = true;
      }
    })

  }
}
