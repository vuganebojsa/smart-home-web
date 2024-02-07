import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { SnackbarService } from 'src/app/core/services/snackbar.service';
import { EditUserDTO, UserProfileDTO } from 'src/app/shared/models/User';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css']
})
export class EditProfileComponent implements OnInit{
  editProfileForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(3)]),
      username: new FormControl('', [Validators.required, Validators.minLength(5)]),
      email: new FormControl('', [Validators.required, Validators.minLength(5), Validators.email]),
    }
  );
  hasError = false;
  errorValue: string = '';
  base64Image: string = '';
  imageType: string = 'jpg';
  user: UserProfileDTO;
  isLoaded = false;
  public constructor(private router: Router, private service: AuthenticationService, private snackBar: SnackbarService){

  }
  extractFileExtension(filePath: string): string {
    const parts = filePath.split('.');
    if (parts.length > 1) {
      return parts.pop(); // Return the last part after the last period
    }
    return ''; // No file extension found
  }
  ngOnInit(): void {
    const storedValue = localStorage.getItem('userInfo');
    this.user =  JSON.parse(storedValue);
    this.editProfileForm.patchValue({
      name: this.user.name,
      lastName: this.user.lastName,
      username: this.user.username,
      email: this.user.email
    });
    this.base64Image = this.user.profilePicture.startsWith('data:')
          ? this.user.profilePicture
          : `data:image/${this.extractFileExtension(this.user.profilePicturePath)};base64,${this.user.profilePicture}`;
      this.isLoaded = true;
  }
  editProfile(){
    if(!this.editProfileForm.valid){
      this.errorValue = 'Please fulfill all the fields correctly.';
      this.hasError = true;
      return;
    }

    let editUserDTO: EditUserDTO = {
      name: this.editProfileForm.value.name,
      lastName: this.editProfileForm.value.lastName,
      username: this.editProfileForm.value.username,
      typeOfImage: this.imageType,
      profilePicture: this.getImageWithoutData(),
      email:this.editProfileForm.value.email
    }
    this.service.editProfile(editUserDTO).subscribe({
      next:(result) =>{
        this.snackBar.showSnackBar('Successfully edited profile.', "Ok");
        this.goBack();
      },
      error:(err) =>{
        this.errorValue = err.error;
        this.hasError = true;
      
      }
    })

    this.hasError = false;
  }

  goBack(){
    localStorage.removeItem('userInfo');
    this.router.navigate(['manage/profile']);
  }

  onImageSelected(event){
    const file: File = event.target.files[0];
    
    if (file) {
      const reader = new FileReader();
      const fileName = file.name;
      const lastDotIndex = fileName.lastIndexOf('.');
      const fileExtension = fileName.substring(lastDotIndex + 1);
      this.imageType = fileExtension;
      reader.onload = (e: any) => {
        const base64Image = e.target.result; // The base64-encoded image
        this.base64Image = base64Image;
        this.user.profilePicture = this.getImageWithoutData();
      };

      reader.readAsDataURL(file);
    }
  }
  getImageWithoutData(): string{
      const [_, imageData] = this.base64Image.split(',', 2);
      return imageData;
  }
}
