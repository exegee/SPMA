import { CollectionViewer } from '@angular/cdk/collections';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { Service } from '../../models/core/service.model';
import { ServicesService } from '../../services/services/services.service';

export class ServicesDataSource implements DataSource<Service>{

  private servicesSubject = new BehaviorSubject<Service[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  data: Service[];
  dataLength: number;
  public loading$ = this.loadingSubject.asObservable();

  constructor(private servicesService: ServicesService) {}

  connect(collectionViewer: CollectionViewer): Observable<Service[] | readonly Service[]> {
    return this.servicesSubject.asObservable();
  }
  disconnect(collectionViewer: CollectionViewer): void {
    this.servicesSubject.complete();
    this.loadingSubject.complete();
  }

  getServices() {
    this.loadingSubject.next(true);

    this.servicesService.getServices().pipe(
      catchError(() => of([])),
      finalize(() => this.loadingSubject.next(false))
    )
      .subscribe((data: Service[]) => {
        this.data = data;
      })
  }


}
