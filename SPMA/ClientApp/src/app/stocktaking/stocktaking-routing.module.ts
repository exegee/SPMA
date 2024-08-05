import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ImportStockComponent } from "./import-stock/import-stock.component";
import { PitDataComponent } from "./pit-data/pit-data.component";
import { PitComponent } from "./pit/pit.component";
import { StocktakingComponent } from "./stocktaking.component";



const stocktakingRoutes: Routes = [
  {
    path: '', component: StocktakingComponent, children: [
      { path: 'pit-data', component: PitDataComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
      { path: 'pit', component: PitComponent, runGuardsAndResolvers: 'always', pathMatch: '', }
    ]
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(stocktakingRoutes)
  ],
  exports: [RouterModule]
})

export class StocktakingRoutingModule {}
