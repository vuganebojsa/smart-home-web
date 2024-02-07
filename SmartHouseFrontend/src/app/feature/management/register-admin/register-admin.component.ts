import { Component } from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthenticationService} from "../../../core/services/authentication.service";
import {TokenDecoderService} from "../../../core/services/token-decoder.service";
import {User} from "../../../shared/models/User";

@Component({
  selector: 'app-register-admin',
  templateUrl: './register-admin.component.html',
  styleUrls: ['./register-admin.component.css']
})
export class RegisterAdminComponent {
  protected readonly document = document;
  imageSrcDisplay:string


  registerForm = new FormGroup({
    imageType: new FormControl('', [Validators.required]),
    userName: new FormControl('', [Validators.required, Validators.minLength(5)]),
    image: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    lastName: new FormControl('', [Validators.required, Validators.minLength(3)]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    repeatedPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  constructor(private authenticationService: AuthenticationService, protected decodeService: TokenDecoderService){
    this.imageSrcDisplay = "";
  }
  hasError = false;
  errorValue = '';
  isSuccess = false;
  successValue = '';
  get password() { return this.registerForm.get('password'); }
  get repeatedPassword() { return this.registerForm.get('repeatedPassword'); }


  register():void{
    this.isSuccess = false;
    if (this.password.value !== this.repeatedPassword.value) {
      this.hasError = true;
      this.errorValue = "Passwords don't match";
      return;
    }

    if(this.registerForm.valid){
      this.hasError = false;
      const user: User = {
        email: this.registerForm.value.email,
        image: this.registerForm.value.image,
        imageType: this.registerForm.value.imageType,
        lastName: this.registerForm.value.lastName,
        name: this.registerForm.value.name,
        password: this.registerForm.value.password,
        repeatPassword: this.registerForm.value.repeatedPassword,
        userName: this.registerForm.value.userName
      }

     

        this.authenticationService.admineRgistration(user).subscribe({
          next: value => {
            this.isSuccess = true;
            this.successValue = "Successfully registered new admin with email " + this.registerForm.value.email;
          }, error: err => {
      
            this.hasError = true;
            this.errorValue = err.error;
          }
        })

    }else{
      this.hasError = true;
      this.errorValue = "All fields must be filled";
    }
     
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


          if(!(reader.result.includes("image/jpeg") || reader.result.includes("image/jpg") || reader.result.includes("image/png"))){
            this.hasError = true;
            this.errorValue = "You can only upload jpg, or png files";
            return;
          }

          this.imageSrcDisplay = reader.result;

          const parts = substrings[0].split(":");
          const mediaType = parts[1].split(";")[0];
          let fileExtension = mediaType.split("/")[1];
          if(fileExtension == "jpeg") fileExtension = "jpg"

          this.registerForm.patchValue({image: substrings[1], imageType: fileExtension})
        }

      };
    }
  }
}
