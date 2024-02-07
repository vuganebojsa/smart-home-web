import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { Role, UserProfileDTO } from 'src/app/shared/models/User';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit{

  constructor(private service: AuthenticationService, private router: Router, private activatedRoute: ActivatedRoute){}
  profile: UserProfileDTO;
  base64Image: string = '';
  isLoaded = false;
  getRole(role: number): string {
    return Role[role];
  }
  extractFileExtension(filePath: string): string {
    const parts = filePath.split('.');
    if (parts.length > 1) {
      return parts.pop(); // Return the last part after the last period
    }
    return ''; // No file extension found
  }
  ngOnInit(): void {
    this.GetProfile();

  }

  changePassword(){
    this.router.navigate(['change-password'], {relativeTo : this.activatedRoute});
  }
  editProfile(){
    localStorage.setItem('userInfo', JSON.stringify(this.profile));
    this.router.navigate(['edit'], {relativeTo : this.activatedRoute});

  }
  private GetProfile() {
    this.service.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;

          this.isLoaded = true;
        this.service.getPicture(this.profile.profilePicturePath).subscribe(
          result =>{
            const url = URL.createObjectURL(result);
            (document.getElementById('profilna') as HTMLImageElement).src = url;
          }
        )
      },
      error: (err) => {
      }
    });
   
  }
}
