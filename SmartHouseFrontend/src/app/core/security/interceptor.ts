import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { SnackbarService } from '../services/snackbar.service';

@Injectable()
export class Interceptor implements HttpInterceptor {

  constructor(private router: Router, private authenticationService: AuthenticationService, private snackBar: SnackbarService){}

  intercept(
    request: HttpRequest<any>, 
    next: HttpHandler
    ): Observable<HttpEvent<any>> {
    const accessToken: any = localStorage.getItem('token');
    //const decodedItem = JSON.parse(accessToken);
    // const refreshToken: any = localStorage.getItem('refreshToken');
    // const decodedRefreshToken = JSON.parse(refreshToken);

    if (request.headers.get('skip')) return next.handle(request);
    if (accessToken) {
      const cloned = request.clone({
        headers: request.headers.set('Authorization', 'Bearer ' +  accessToken)
                                // .set('refreshToken', decodedRefreshToken)
                                ,
      });

      return next.handle(cloned).pipe(
        tap(() =>{},
          (error: HttpErrorResponse | any) =>{
            if (error instanceof HttpErrorResponse && error.status === 401) {
              // Logout logic here
              this.snackBar.showSnackBar('Your Session has expired. Please log in again.', "Ok");
              this.authenticationService.logoutUser();
              this.router.navigate(['/login']);
            }
          }
        )
      );
    } else {
      return next.handle(request).pipe(
        tap(() =>{},
          (error:any) =>{
            if (error instanceof HttpErrorResponse && error.status === 401) {
              // Logout logic here
              this.snackBar.showSnackBar('Your Session has expired. Please log in again.', "Ok");
              this.authenticationService.logoutUser();
              this.router.navigate(['/login']);
            }
          }
        )
      );
    }
      
  }
}