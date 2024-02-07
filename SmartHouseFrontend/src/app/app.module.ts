import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { Interceptor } from './core/security/interceptor';
import { LoginComponent } from './core/layout/login/login.component';
import { RegisterComponent } from './core/layout/register/register.component';
import { SharedModule } from './shared/shared.module';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {HomeComponent} from "./core/layout/home/home.component";
import { SmartModule } from './feature/smart/smart.module';
import { ChangeFirstPasswordComponent } from './core/layout/change-first-password/change-first-password.component';
import { SessionExpiredComponent } from './core/layout/session-expired/session-expired.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    ChangeFirstPasswordComponent,
    SessionExpiredComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    SmartModule,
    BrowserAnimationsModule,
    MatSnackBarModule
    
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS, useClass: Interceptor, multi: true
  },],
  exports: [
    LoginComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
