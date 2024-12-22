import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Alerts } from '../models/alerts.model';

@Injectable({
  providedIn: 'root'
})
export class AlertsService {

  constructor(private httpservice: HttpClient) { }

  getAlerts() {
    return this.httpservice.get(`${environment.apiUrl}/Alerts/GetUnprocessedAlerts`);
  }

  processAlerts(inputData: Alerts) {
    return this.httpservice.post(`${environment.apiUrl}/Alerts/ProcessJailAlerts`, inputData);
  }

  getAlertCount() {
    return this.httpservice.get(`${environment.apiUrl}/Alerts/GetAlertCount`);
  }
}
