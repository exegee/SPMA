import { NgModule } from "@angular/core";
import { Routes, RouterModule, PreloadAllModules, NoPreloading } from '@angular/router';
import { DashboardComponent } from './core/dashboard/dashboard.component';

const appRoutes: Routes = [
  { path: '', component: DashboardComponent },
  {
    path: 'orders',
    loadChildren: './orders/orders.module#OrdersModule', 
  },
  {
    path: 'production',
    loadChildren: './production/production.module#ProductionModule',
  },
  {
    path: 'settings',
    loadChildren: './settings/settings.module#SettingsModule',
  },
  {
    path: 'stocktaking',
    loadChildren: './stocktaking/stocktaking.module#StocktakingModule',
  }
];


@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes, {
      preloadingStrategy: PreloadAllModules,
      onSameUrlNavigation: 'reload',
    })
  ],
  exports: [RouterModule]
})

export class AppRoutingModule {}
