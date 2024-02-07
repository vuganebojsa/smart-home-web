import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {AuthenticationService} from "../../services/authentication.service";
import {ChangePasswordDTO} from "../../../shared/models/User";
import {TokenDecoderService} from "../../services/token-decoder.service";

@Component({
  selector: 'app-change-first-password',
  templateUrl: './change-first-password.component.html',
  styleUrls: ['./change-first-password.component.css']
})
export class ChangeFirstPasswordComponent implements OnInit{
  editPasswordForm = new FormGroup(
    {
      oldPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
      newPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
      repeatNewPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
    }
  );
  hasError = false;
  errorValue: string = '';
  email: string = '';
  public constructor(private router: Router, private service: AuthenticationService, private route: ActivatedRoute, private tokenDecoderService: TokenDecoderService){

  }

  ngOnInit(): void {
    this.GetEmail();
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
    this.service.editFirstPassword(passInfo, this.email).subscribe({
      next:(result) =>{
        if(result.token !== null){

          this.hasError = false;
          localStorage.setItem("token", result.token);
          localStorage.setItem('role', this.tokenDecoderService.getDecodedAccesToken()['role']);
          localStorage.setItem('userId', this.tokenDecoderService.getDecodedAccesToken()['id']);
          localStorage.setItem('userEmail', this.tokenDecoderService.getDecodedAccesToken()['email']);
          this.router.navigate(['home']);
          this.service.userLoggedIn$.next(true);
          localStorage.removeItem('passwordChanged')
          this.router.navigate(['home']);

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


  private GetEmail() {
    this.route.params.subscribe(params => {
      this.email = params['email'];
    });
  }


}
