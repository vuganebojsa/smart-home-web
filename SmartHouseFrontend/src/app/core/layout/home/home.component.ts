import {Component, OnInit} from '@angular/core';
import {AuthenticationService} from "../../services/authentication.service";
import {Router} from "@angular/router";
import { TokenDecoderService } from '../../services/token-decoder.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{

  isLoggedIn: boolean = false;
  userLogged:boolean = false;
  adminLogged:boolean = false;
  superAdminLogged:boolean = false
  constructor(private authenticationService: AuthenticationService, private decodeService: TokenDecoderService){

  }
  ngOnInit(): void {
    this.authenticationService.userStateLoggedIn$.subscribe(res =>{
      this.isLoggedIn = res;
    })
    if(this.isLoggedIn == true && this.decodeService.getDecodedAccesToken()["role"]==="USER"){
      this.adminLogged = false;
      this.userLogged = true;
      this.superAdminLogged = false
    }
    if(this.isLoggedIn == true && this.decodeService.getDecodedAccesToken()["role"]==="ADMIN"){
      this.userLogged = false;
      this.adminLogged = true;
      this.superAdminLogged = false
    }
    if(this.isLoggedIn == true && this.decodeService.getDecodedAccesToken()["role"]==="SUPERADMIN"){
      this.userLogged = false;
      this.adminLogged = false
      this.superAdminLogged = true;
    }
  }

}
