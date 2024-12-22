import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegisterInmateComponent } from './components/register-inmate/register-inmate.component';
import { FormsModule } from '@angular/forms';
import { WebcamModule } from 'ngx-webcam';
import { AlertsComponent } from './components/alerts/alerts.component';
import { HeaderComponent } from './components/header/header.component';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProcessAlertsComponent } from './components/process-alerts/process-alerts.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    RegisterInmateComponent,
    AlertsComponent,
    HeaderComponent,
    ProcessAlertsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    WebcamModule,
    MatDialogModule,
    BrowserAnimationsModule
  ],
  providers: [
    { provide: MatDialogRef, useValue: {} },
    { provide: MAT_DIALOG_DATA, useValue: {} }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
