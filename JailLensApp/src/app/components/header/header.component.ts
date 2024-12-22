import { Component, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AlertsService } from 'src/app/services/alerts.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(private alertsService: AlertsService) { }

  alertsCount = 0;
  destroy$: Subject<boolean> = new Subject<boolean>();

  ngOnInit(): void {
    this.getAlertCount();
    this.startInterval();
  }

  startInterval() {
    setInterval(() => {
      this.getAlertCount();
    }, 1000*60*5);             // 5 here stands for 5 minutes
  }

  getAlertCount() {
    this.alertsService.getAlertCount().pipe(takeUntil(this.destroy$))
    .subscribe((response: any) => {
      if (response) {
        this.alertsCount = response;
      } else {
        alert("Something went wrong while fetching the alerts");
      }
    });
  }

}
