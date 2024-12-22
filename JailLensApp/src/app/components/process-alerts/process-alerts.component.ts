import { Component, Input, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { AlertsComponent } from '../alerts/alerts.component';
import { Alerts } from 'src/app/models/alerts.model';
import { Subject, takeUntil } from 'rxjs';
import { AlertsService } from 'src/app/services/alerts.service';

@Component({
  selector: 'app-process-alerts',
  templateUrl: './process-alerts.component.html',
  styleUrls: ['./process-alerts.component.css']
})
export class ProcessAlertsComponent implements OnInit {

  constructor(private alertsService: AlertsService) { }

  intakeid: string = '';
  alertDescription: string = '';
  alertCategory: string = '';
  comments: string = '';
  destroy$: Subject<boolean> = new Subject<boolean>();

  @Input() alertData: any;

  ngOnInit(): void {
    this.intakeid = this.alertData.alert.intakeid;
    this.alertDescription = this.alertData.alert.alertdescription;
    this.alertCategory = this.alertData.alert.alertcategory;
  }

  onSubmit() {
    let alertToProcess: Alerts = {
      jaillensalertid: this.alertData.alert.jaillensalertid,
      intakeid: this.intakeid,
      programname: this.alertData.alert.programname,
      inmatename: this.alertData.alert.inmatename,
      alertdescription: this.alertDescription,
      alertcategory: this.alertCategory,
      comments: this.comments,
      createddate: this.alertData.alert.createddate,
      isprocessed: 1,
      actualprogramname: this.alertData.alert.actualprogramname
    }
    
    this.alertsService.processAlerts(alertToProcess).pipe(takeUntil(this.destroy$))
    .subscribe((response) => {
      if (response == 200) {
        alert("Alert processed successfully");
      }
    })
  }

}
