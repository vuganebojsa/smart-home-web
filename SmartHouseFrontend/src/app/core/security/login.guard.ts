import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';
import {TokenDecoderService} from "../services/token-decoder.service";

@Injectable({
  providedIn: 'root'
})
export class LoginGuard {

  constructor(private authenticationService: AuthenticationService, private router: Router, private decodeService: TokenDecoderService){

  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

      if(this.authenticationService.isLoggedIn()){
        this.router.navigate(['home']);
        return false;
      }
      return true;
  }

}
