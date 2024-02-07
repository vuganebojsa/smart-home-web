import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginGuard } from './core/security/login.guard';
import { HomeComponent } from './core/layout/home/home.component';
import { LoginComponent } from './core/layout/login/login.component';
import { RegisterComponent } from './core/layout/register/register.component';
import {ChangeFirstPasswordComponent} from "./core/layout/change-first-password/change-first-password.component";
import { SessionExpiredComponent } from './core/layout/session-expired/session-expired.component';

const routes: Routes = [
  {path:'', component: HomeComponent},
  {path:'login', component: LoginComponent, canActivate:[LoginGuard]},
  {path:'register', component: RegisterComponent, canActivate:[LoginGuard]},
  {path:'session-expired', component: SessionExpiredComponent},
  {path:'edit-first-password/:email', component: ChangeFirstPasswordComponent},
  {path: 'smart', loadChildren: () => import('./feature/smart/smart.module').then(m => m.SmartModule)},
  {path: 'report', loadChildren: () => import('./feature/report/report.module').then(m => m.ReportModule)},
  {path: 'manage', loadChildren: () => import('./feature/management/management.module').then(m => m.ManagementModule)},
  { path: '', redirectTo: '', pathMatch: 'full'},
  { path: '**', component: HomeComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
