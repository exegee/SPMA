import { AfterViewInit, Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { Observable, Subscriber, Subscription, interval  } from 'rxjs';
import { observe } from 'rxjs-observe';
import { Service } from '../../models/core/service.model';
import { ServicesService } from '../../services/services/services.service';
import { ServicesDataSource } from './services-datasource';


@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrls: ['./services.component.css']
})
export class ServicesComponent implements OnInit, AfterViewInit {

  public services: Service[];
  servicesDataSource: MatTableDataSource<Service>;

  displayedColumns = ["Id", "Name", "Description", "Interval", "CurrentJob", "Buttons"];
  public getServicesInterval: Subscription;


  constructor(private servicesService: ServicesService) { }
    ngAfterViewInit(): void {
      this.getServicesInterval = interval(5000).subscribe(() => {
        this.getServices();
      });
    }

  ngOnInit() {
    this.getServices();

  }

  getServices() {
      this.servicesService.getServices().subscribe((data: Service[]) => {
      this.services = data;
      this.servicesDataSource = new MatTableDataSource(this.services);
      //console.log(this.servicesDataSource);
    });
}


  onServiceToggleChange(id: number, serviceStatus: boolean) {
    serviceStatus = !serviceStatus;
    this.servicesService.serviceToggle(id, serviceStatus).subscribe(
      () => { },
      (error) => {
        console.log(error)
      },
      () => {
        // Change service status locally - neeed for updating the view
        console.log(this.servicesDataSource.filteredData);
        var serviceIndex = this.servicesDataSource.filteredData.findIndex((item, index) => {
          if (item.serviceID === id) {
            return true
          }
        });
        this.servicesDataSource.filteredData[serviceIndex].isRunning = serviceStatus;
        // Get service job - needed for updating the view
        this.servicesService.getServiceJob().subscribe((data: string) => {
          this.servicesDataSource.filteredData[serviceIndex].currentJob = data;
        },
          (error) => { console.log(error) });
      });
  }
}
