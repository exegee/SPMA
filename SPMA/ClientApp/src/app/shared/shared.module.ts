import { CommonModule } from '@angular/common';
import { NgModule } from "@angular/core";
import { MatButtonModule, MatMenuModule, MatSelectModule, MatDatepickerModule, MAT_DATE_LOCALE, MAT_DATE_FORMATS, DateAdapter, MatTooltipModule } from '@angular/material';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ContextMenuModule } from 'ngx-contextmenu';
import { CheckboxComponent } from './checkbox.component';
import { CollapseDirective } from './collapse.directive';
import { DropdownDirective } from './dropdown.directive';
import { AttrCheckDirective } from './attrcheck.directive';
import { ScrollToActiveDirective } from './scrolltoactive.directive';
import { OffsetTopDirective } from './offsettop.directive';
import { MomentDateModule, MomentDateAdapter } from '@angular/material-moment-adapter';
import { BrowserModule } from '@angular/platform-browser';

@NgModule({
  declarations: [
    DropdownDirective,
    CollapseDirective,
    CheckboxComponent,
    AttrCheckDirective,
    ScrollToActiveDirective,
    OffsetTopDirective,
  ],
  imports: [
    MatSelectModule,
    MatIconModule,
      FontAwesomeModule,
      ContextMenuModule.forRoot({
          useBootstrap4: true,
      }),
    MatDatepickerModule,
    MomentDateModule,
    CommonModule,
    MatTooltipModule 
  ],
  exports: [
    CommonModule,
    MatButtonModule,
    DropdownDirective,
    CollapseDirective,
    CheckboxComponent,
    MatSelectModule,
    MatIconModule,
    MatMenuModule,
    MatTabsModule,
    FontAwesomeModule,
    AttrCheckDirective,
    ScrollToActiveDirective,
    OffsetTopDirective,
    MatDatepickerModule,
    MomentDateModule,
    MatTooltipModule 
  ],
  providers: [
  ]
})

export class SharedModule {}
