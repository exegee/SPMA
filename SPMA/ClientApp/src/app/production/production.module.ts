import { NgModule } from "@angular/core";
import { ProductionComponent } from './production.component';
import { BandsawComponent } from './bandsaw/bandsaw.component';
import { ProductionRoutingModule } from "./production-routing.module";
import { LogscreenComponent } from './logscreen/logscreen.component';
import { MatAutocompleteModule, MatButtonModule, MatButtonToggleModule, MatCardModule, MatDialogModule, MatDividerModule, MatFormFieldModule, MatIconModule, MatInputModule, MatListModule, MatPaginatorModule, MatProgressBarModule, MatProgressSpinnerModule, MatSnackBarModule, MatSortModule, MatTableModule } from "@angular/material";
import { MatToolbarModule } from '@angular/material/toolbar';
import { ProductionOrderListComponent } from './production-order-list/production-order-list.component';
import { CommonModule } from "@angular/common";
import { ProductionSuborderListComponent } from './production-suborder-list/production-suborder-list.component';
import { ProductionSuborderItemListComponent } from './production-suborder-item-list/production-suborder-item-list.component';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SelectionScreenComponent } from './selection-screen/selection-screen.component';
import { InBuiltComponent } from './in-built/in-built.component';
import { SurplusItemsListComponent } from './surplus-items-list/surplus-items-list.component';


@NgModule({
  declarations:[
    ProductionComponent,
    BandsawComponent,
    LogscreenComponent,
    ProductionOrderListComponent,
    ProductionSuborderListComponent,
    ProductionSuborderItemListComponent,
    SelectionScreenComponent,
    InBuiltComponent,
    SurplusItemsListComponent
    ],
  imports: [
    CommonModule,
    ProductionRoutingModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressBarModule,
    MatDividerModule,
    MatListModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSnackBarModule,
    FormsModule,
    MatCardModule,
    MatButtonToggleModule
  ],
  exports: [
    ProductionComponent,
    BandsawComponent
  ]
})
export class ProductionModule {

}
