import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { WarehouseItem } from "../models/production/warehouseItem.model";
import { BandsawComponent } from "./bandsaw/bandsaw.component";
import { InBuiltComponent } from "./in-built/in-built.component";
import { LogscreenComponent } from "./logscreen/logscreen.component";
import { ProductionOrderListComponent } from "./production-order-list/production-order-list.component";
import { ProductionSuborderItemListComponent } from "./production-suborder-item-list/production-suborder-item-list.component";
import { ProductionSuborderListComponent } from "./production-suborder-list/production-suborder-list.component";
import { ProductionComponent } from "./production.component";
import { SelectionScreenComponent } from "./selection-screen/selection-screen.component";
import { SurplusItemsListComponent } from "./surplus-items-list/surplus-items-list.component";

const productionRoutes: Routes = [
  {
    path: '', component: ProductionComponent, children: [
      { path: 'log', component: LogscreenComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
      { path: 'productionOrderList', component: ProductionOrderListComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
      { path: 'productionSuborderList', component: ProductionSuborderListComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
      { path: 'productionSuborderItemList', component: ProductionSuborderItemListComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
      { path: 'bandsaw', component: BandsawComponent, runGuardsAndResolvers: 'always', pathMatch: 'prefix', },
      { path: 'selectionScreen', component: SelectionScreenComponent, runGuardsAndResolvers: 'always', pathMatch: '' },
      { path: 'inbuilt', component: InBuiltComponent, runGuardsAndResolvers: 'always', pathMatch: '' },
      { path: 'surplusWarehouse', component: SurplusItemsListComponent, runGuardsAndResolvers: 'always', pathMatch: '' },
      { path: '', redirectTo: '/'}
    ]
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(productionRoutes)
  ],
  exports: [RouterModule]
})

export class ProductionRoutingModule { }
