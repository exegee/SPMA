import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SettingsComponent } from './settings.component';
import { ServicesComponent } from './services/services.component';
import { SettingsRoutingModule } from './settings-routing.module';
import { SharedModule } from '../shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatListModule, MatSlideToggle, MatSlideToggleModule, MatTableModule } from '@angular/material';



@NgModule({
  declarations:
    [
      SettingsComponent,
      ServicesComponent
    ],
  imports: [
    MatTableModule,
    MatSlideToggleModule,
    SharedModule,
    CommonModule,
    NgbModule,
    SettingsRoutingModule
  ],
  exports: [
    ServicesComponent,
    SettingsComponent
  ]
})
export class SettingsModule { }
