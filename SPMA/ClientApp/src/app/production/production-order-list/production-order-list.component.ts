import { animate, state, style, transition, trigger } from '@angular/animations';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort } from '@angular/material';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { relative } from 'path';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { Order } from '../../models/orders/order.model';
import { OrdersDataSource } from '../../orders/orders-list/orders-datasource';
import { OrdersService } from '../../services/orders/orders.service';

@Component({
  selector: 'app-production-order-list',
  templateUrl: './production-order-list.component.html',
  styleUrls: ['./production-order-list.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('500ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
export class ProductionOrderListComponent implements OnInit, AfterViewInit{

  //for parent component
  title: string = 'Wybór zlecenia';
  socketType: string;
  selectedOrder: Order;
  dataSource: OrdersDataSource;
  activeInput: string;
  //expandedElement: Order | null;

  displayedColumns : string[] = ["Number", "Name", "ClientName", "OrderDate", "RequiredDate", "Buttons"];
  //displayedColumnsHeaders : string[] = ["#", "Stan", "Ilość", "Numer", "Stan RW", "Nazwa", "Klient", "Data wpłynięcia", "Termin wykonania"];

  numberSearchInputValue: string = '';
  nameSearchInputValue: string = '';
  clientNameSearchInputValue: string = '';
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  //@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild('searchNumberInput', { static: false }) searchNumberInput: ElementRef;
  @ViewChild('searchNameInput', { static: false }) searchNameInput: ElementRef;
  @ViewChild('searchClientInput', { static: false }) searchClientInput: ElementRef;

  constructor(private ordersService: OrdersService, public dialog: MatDialog, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.dataSource = new OrdersDataSource(this.ordersService);
    this.dataSource.getOrders('', 'asc', 0, 0, 150, '', '', '');
    this.socketType = this.route.snapshot.queryParams['socket'];
    //console.log(this.dataSource);
  }

  ngAfterViewInit() {

    //this.paginator.pageIndex = 0;
    //Create observable for searching Input elements keyup event
    this.eventToObservable(this.searchNumberInput);
    this.eventToObservable(this.searchNameInput);
    this.eventToObservable(this.searchClientInput);


    //this.paginator.page
    //  .pipe(
    //    tap(() => this.loadOrdersPage(''))
    //  )
    //  .subscribe();

    this.dataSource.loading$.subscribe(result => {
      if (result == false) { }
    })

    this.sort.sortChange
      .pipe(
        tap(() => this.loadOrders())
      )
      .subscribe();

  }

  //isAllSelected
  //masterToggle
  //onArchiveOrder
  //confirmOrderDelete
  //onReload

  loadFilteredOrders() {
    //this.dataSource.getOrders(this.sort.active, this.sort.direction, this.paginator.pageIndex, this.paginator.pageSize, this.searchInput.nativeElement.value);
    this.dataSource.getOrders(this.sort.active, this.sort.direction, 0, 0, 100, this.nameSearchInputValue, this.numberSearchInputValue, this.clientNameSearchInputValue);
  }

  loadOrders() {
    //this.dataSource.getOrders(this.sort.active, this.sort.direction, 0, 15, '');
    this.dataSource.getOrders(this.sort.active, this.sort.direction, 0, 0, 150, '', '', '');
  }

  //choose the input with focus on  
  activatedInput(event) {
    //console.log(event.target.id);
    //this.activeInput = event.target.id;
  }

  onClearNumberSearchInputValue() {
    this.numberSearchInputValue = '';
    this.loadFilteredOrders();
  }
  onClearNameSearchInputValue() {
    this.nameSearchInputValue = '';
    this.loadFilteredOrders();
  }

  onClearClientNameSearchInputValue() {
    this.clientNameSearchInputValue = '';
    this.loadFilteredOrders();
  }


  //change event from inputs into observable
  eventToObservable(element: ElementRef) {
    if (element) {
      fromEvent(element.nativeElement, 'keyup')
        .pipe(
          debounceTime(150),
          distinctUntilChanged(),
          tap(() => {
            //this.paginator.pageIndex = 0;
            this.loadFilteredOrders();
          })
        ).subscribe();
    }
    else {
      console.log('element undefined');
    }
  }

  //method on table row click event, whitch take user to corresponding suborders page
  toSuborderList(order: Order) {
    this.router.navigate(['/production/productionSuborderList'], { queryParams: { orderID: order.orderId, socket: this.socketType } });
  }

  onOrderClick(order: Order) {
    if (order == this.selectedOrder) {
      return;
    }
    this.selectedOrder = order;
  }

  backSelectionScreen() {
    this.router.navigate(['../selectionScreen'], { relativeTo: this.route, queryParams: { socket: this.socketType } });
  }

}
