import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DateAdapter, MatButtonToggleModule, MatCheckbox, MatCheckboxModule, MatInputModule, MatNativeDateModule, MatPaginatorModule, MatSortModule, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { NgbModule, NgbTypeaheadModule } from '@ng-bootstrap/ng-bootstrap';
import { ContextMenuModule } from 'ngx-contextmenu';
import { SharedModule } from '../shared/shared.module';
import { BookAddComponent } from './book-add/book-add.component';
import { BookImportComponent, HighlightPipe } from './book-import/book-import.component';
import { BooksListComponent } from './books-list/books-list.component';
import { ToProductionToggleComponent } from './directives/to-production-toggle/to-production-toggle.component';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { OrdersListComponent } from './orders-list/orders-list.component';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrdersComponent } from './orders.component';
import { SuborderEditComponent } from './suborder-edit/suborder-edit.component';
import { OrdersArchiveComponent } from './orders-archive/orders-archive.component';

export const DateFormat = {
  parse: {
    dateInput: 'YYYY-MM-DD',
  },
  display: {
    dateInput: 'YYYY-MM-DD',
    monthYearLabel: 'YYYY MMMM',
    dateA11yLabel: 'YYYY-MM-DD',
    monthYearA11yLabel: 'YYYY MMMM',
  }
};

@NgModule({

  declarations: [
    OrdersComponent,
    OrdersListComponent,
    BookImportComponent,
    HighlightPipe,
    OrderDetailComponent,
    BookAddComponent,
    BooksListComponent,
    ToProductionToggleComponent,
    SuborderEditComponent,
    OrdersArchiveComponent
  ],
  imports: [
    SharedModule,
    NgbModule,
    OrdersRoutingModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatListModule,
    FormsModule,
    MatTableModule,
    MatInputModule,
    MatPaginatorModule,
    MatSortModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    MatDividerModule,
    MatGridListModule,
    MatSlideToggleModule,
    MatButtonToggleModule,
    MatAutocompleteModule,
    MatStepperModule,
    MatCheckboxModule,
    MatProgressBarModule,
      NgbTypeaheadModule,
      ContextMenuModule.forRoot({
          useBootstrap4: true,
      })
  ],
  exports: [
    MatTableModule,
    OrdersListComponent,
    OrdersComponent,
    ToProductionToggleComponent
  ],
  providers: [
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: DateFormat }
  ]
})

export class OrdersModule {}
