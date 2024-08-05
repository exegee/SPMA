import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StocktakingComponent } from './stocktaking.component';
import { PitComponent } from './pit/pit.component';
import { ImportStockComponent } from './import-stock/import-stock.component';
import { StocktakingRoutingModule } from './stocktaking-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MatAutocompleteModule, MatButtonToggleModule, MatCardModule, MatCheckboxModule, MatDividerModule, MatGridListModule, MatInputModule, MatListModule, MatNativeDateModule, MatPaginatorModule, MatProgressBarModule, MatProgressSpinnerModule, MatSlideToggleModule, MatSortModule, MatStepperModule, MatTableModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PitDataComponent } from './pit-data/pit-data.component';



@NgModule({
  declarations:
    [
      StocktakingComponent,
      PitComponent,
      ImportStockComponent,
      PitDataComponent
    ],
  imports: [
    CommonModule,
    SharedModule,
    StocktakingRoutingModule,
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
    NgbModule,
  ],
  exports: [
    PitComponent,
    ImportStockComponent,
    StocktakingComponent
  ]
})
export class StocktakingModule { }
