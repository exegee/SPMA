import { CollectionViewer } from '@angular/cdk/collections';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { Order } from '../../models/orders/order.model';
import { PagedOrder } from '../../models/orders/pagedorder.model';
import { OrdersService } from '../../services/orders/orders.service';

export class OrdersDataSource implements DataSource<Order>{

  private ordersSubject = new BehaviorSubject<Order[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  data: Order[];
  dataLength: number;

  public loading$ = this.loadingSubject.asObservable();


  constructor(private ordersService: OrdersService) {
  }



  connect(collectionViewer: CollectionViewer): Observable<Order[]> {
    return this.ordersSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.ordersSubject.complete();
    this.loadingSubject.complete();
  }

  getOrders(orderBy: string, sortOrder = 'asc', orderState: number, page: number, pageSize: number, filterByName: string, filterByNumber: string, filterByClientName: string) {
    this.loadingSubject.next(true);

    this.ordersService.getOrders(orderBy, sortOrder, orderState, page, pageSize, filterByName, filterByNumber, filterByClientName).pipe(
      catchError(() => of([])),
      finalize(() => this.loadingSubject.next(false))
    )
      .subscribe((pagedOrder: PagedOrder) => {
        this.data = pagedOrder.results as Order[];
        this.ordersSubject.next(pagedOrder.results);
        this.dataLength = pagedOrder.rowCount;
      });
  }

  deleteOrder(order: Order) {
    this.ordersService.deleteOrder(order.orderId.toString())
      .subscribe();
    let orders: any[] = this.ordersSubject.getValue();
    orders.forEach((item: Order, index) => {
      if (item === order) {
        orders.splice(index, 1);
      }
    });
    this.ordersSubject.next(orders);
  }

  refresh() {
    this.getOrders('', 'asc', 0, 0, 15, '','','');
  }

  archiveOrder(orderId: number) {
    this.ordersService.archiveOrder(orderId.toString(), '10')
      .subscribe();
  }
}
