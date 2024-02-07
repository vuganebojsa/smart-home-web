import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {RegisterAdminComponent} from "./register-admin/register-admin.component";
import { ProfileComponent } from './profile/profile.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { ChangePasswordComponent } from './change-password/change-password.component';

const routes: Routes = [
  {path: "register", component: RegisterAdminComponent},
  {path: "profile", component: ProfileComponent},
  {path: "profile/change-password", component: ChangePasswordComponent},
  {path: "profile/edit", component: EditProfileComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManagementRoutingModule { }
