import { DecimalPipe, registerLocaleData } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import localepl from '@angular/common/locales/pl';
import { LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule, MatButtonModule, MatCheckboxModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatList, MatListModule, MatNativeDateModule, MatProgressBarModule, MatTableModule } from '@angular/material';
import { MomentDateModule } from '@angular/material-moment-adapter';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';
import { ConfirmDeleteComponent } from 'src/app/dialogs/confirm-delete/confirm-delete.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { BookExistComponent } from './dialogs/book-exist/book-exist.component';
import { ConfirmComponent } from './dialogs/confirm/confirm.component';
import { ErrorComponent } from './dialogs/error/error.component';
import { NewOrderComponent } from './dialogs/new-order/new-order.component';
import { ProgressComponent } from './dialogs/progress/progress.component';
import { AlarmComponent } from './production/alarm/alarm.component';
import { SharedModule } from './shared/shared.module';
import { SuborderEditConfirmComponent } from './dialogs/suborder-edit-confirm/suborder-edit-confirm.component';
import { SuborderEditProgressComponent } from './dialogs/suborder-edit-progress/suborder-edit-progress.component';
import { SuborderEditErrorComponent } from './dialogs/suborder-edit-error/suborder-edit-error.component';
import { ChangeWithAutocompleteComponent, HighlightPipe } from './dialogs/change-with-autocomplete/change-with-autocomplete.component';
import { BandsawCutConfirmComponent } from './dialogs/bandsaw-cut-confirm/bandsaw-cut-confirm.component';
//import { MatIconModule } from '@angular/material/icon';
import { APP_INITIALIZER } from '@angular/core';
import { ServicesService } from './services/services/services.service';
import { EditOrderComponent } from './dialogs/edit-order/edit-order.component';
import { AddingExistingBookComponent } from './dialogs/adding-existing-book/adding-existing-book.component';
import { ProgressBarComponent } from './dialogs/progress-bar/progress-bar.component';
import { ErrorMessageComponent } from './dialogs/error-message/error-message.component';
import { WarningMessageComponent } from './dialogs/warning-message/warning-message.component';
import { NewSurplusOrderComponent } from './dialogs/new-surplus-order/new-surplus-order.component';
import { EditStockitemComponent } from './dialogs/edit-stockitem/edit-stockitem.component';
import { AddStockitemComponent } from './dialogs/add-stockitem/add-stockitem.component';
registerLocaleData(localepl);


@NgModule({
  declarations: [
    AppComponent,
    ConfirmDeleteComponent,
    NewOrderComponent,
    ProgressComponent,
    BookExistComponent,
    AlarmComponent,
    ErrorComponent,
    ConfirmComponent,
    SuborderEditConfirmComponent,
    SuborderEditProgressComponent,
    SuborderEditErrorComponent,
    ChangeWithAutocompleteComponent,
    HighlightPipe,
    BandsawCutConfirmComponent,
    EditOrderComponent,
    AddingExistingBookComponent,
    ProgressBarComponent,
    ErrorMessageComponent,
    WarningMessageComponent,
    NewSurplusOrderComponent,
    EditStockitemComponent,
    AddStockitemComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    NgbModule,
    SharedModule,
    CoreModule,
    HttpClientModule,
    FontAwesomeModule,
    BrowserAnimationsModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    MatListModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressBarModule,
    MatAutocompleteModule,
    ToastrModule.forRoot({
      progressBar: true
    }),
    MomentDateModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatTableModule,
    MatCheckboxModule
  ],
  exports: [
    
  ],
  entryComponents: [
    ConfirmDeleteComponent,
    NewOrderComponent,
    EditOrderComponent,
    ProgressComponent,
    BookExistComponent,
    ConfirmComponent,
    ErrorComponent,
    SuborderEditConfirmComponent,
    SuborderEditProgressComponent,
    SuborderEditErrorComponent,
    ChangeWithAutocompleteComponent,
    BandsawCutConfirmComponent,
    AddingExistingBookComponent,
    ProgressBarComponent,
    ErrorMessageComponent,
    WarningMessageComponent,
    NewSurplusOrderComponent,
    EditStockitemComponent,
    AddStockitemComponent
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'pl' },
    DecimalPipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
