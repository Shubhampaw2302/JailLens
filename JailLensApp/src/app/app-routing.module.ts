import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterInmateComponent } from './components/register-inmate/register-inmate.component';
import { AlertsComponent } from './components/alerts/alerts.component';

const routes: Routes = [
  {
    path: 'register', component: RegisterInmateComponent, data: { IsRelease: false }
  },
  {
    path: 'alerts', component: AlertsComponent
  },
  {
    path: 'release', component: RegisterInmateComponent, data: { IsRelease: true }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
