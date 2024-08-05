import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ServicesComponent } from './services/services.component';
import { SettingsComponent } from './settings.component';

const settingsRoutes: Routes = [
  {
    path: '', component: SettingsComponent, children: [
      { path: 'services', component: ServicesComponent, runGuardsAndResolvers: 'always', pathMatch: '', }
    ]
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(settingsRoutes)
  ],
  exports: [RouterModule]
})

export class SettingsRoutingModule { }
