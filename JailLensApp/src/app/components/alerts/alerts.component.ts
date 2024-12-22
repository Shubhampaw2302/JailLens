import { Component, Input, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { Alerts } from 'src/app/models/alerts.model';
import { AlertsService } from 'src/app/services/alerts.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.css']
})
export class AlertsComponent implements OnInit {

  constructor(private alertsService: AlertsService) { }

  formData: any;

  destroy$: Subject<boolean> = new Subject<boolean>();
  alerts: any;
  IntakeID = '';
  alertDescription = '';
  alertCategory = '';
  comments = '';
  selectedAlert: any;
  isForm = false;


  ngOnInit(): void {
    this.loadAlerts();

    this.startInterval();
  }

  startInterval() {
    setInterval(() => {
      this.loadAlerts();
    }, 1000*60*5);
  }

  loadAlerts() {
    this.alertsService.getAlerts().pipe(takeUntil(this.destroy$))
    .subscribe((response : any) => {
      if (response) {
        this.alerts = response;
      } else {
        alert("Something went wrong while fetching the alerts");
      }
    });
  }

  processAlert(alert: any) {
    this.isForm = true;
    this.formData = {
      alert: alert
    }
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

}
