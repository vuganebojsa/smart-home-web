import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManagementRoutingModule } from './management-routing.module';
import { RegisterAdminComponent } from './register-admin/register-admin.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { ProfileComponent } from './profile/profile.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';


@NgModule({
  declarations: [
    RegisterAdminComponent,
    ProfileComponent,
    ChangePasswordComponent,
    EditProfileComponent
  ],
    imports: [
        CommonModule,
        ManagementRoutingModule,
        ReactiveFormsModule,
        FormsModule
    ]
})
export class ManagementModule { }
