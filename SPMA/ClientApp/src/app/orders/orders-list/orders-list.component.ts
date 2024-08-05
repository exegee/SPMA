import { animate, state, style, transition, trigger } from '@angular/animations';
import { AfterViewInit, Component, OnInit, ViewChild, ElementRef, ViewContainerRef, OnDestroy } from '@angular/core';
import { MatDialog, MatDialogConfig, MatSort, MatTable, MatPaginator, MatInput } from '@angular/material';
import { ActivatedRoute, NavigationEnd, Router, RouterEvent } from '@angular/router';
import { tap, debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { ConfirmDeleteComponent } from 'src/app/dialogs/confirm-delete/confirm-delete.component';
import { NewOrderComponent } from 'src/app/dialogs/new-order/new-order.component';
import { Order } from 'src/app/models/orders/order.model';
import { OrdersService } from '../../services/orders/orders.service';
import { OrdersDataSource } from './orders-datasource';
import { fromEvent, Subject, Subscription } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { PdfService } from '../../services/utility/pdf.service';
import { faPrint, faClone } from '@fortawesome/free-solid-svg-icons';
import { QueryList } from '@angular/core';
import { OrdersHelper } from '../ordershelper';
import { EditOrderComponent } from 'src/app/dialogs/edit-order/edit-order.component';
import { ProgressBarComponent } from '../../dialogs/progress-bar/progress-bar.component';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorMessageComponent } from '../../dialogs/error-message/error-message.component';
import { Observable } from 'rxjs';
import { FormControl } from '@angular/forms';


@Component({
  selector: 'app-orders-list',
  templateUrl: './orders-list.component.html',
  styleUrls: ['./orders-list.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('500ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class OrdersListComponent implements OnInit, AfterViewInit, OnDestroy {

  selectedOrder: Order;
  dataSource: OrdersDataSource;
  orders: Order[];
  selection: SelectionModel<Order>;
  isOrdersListPrinting: boolean = false;
  isOrderDetailPrinting: boolean = false;
  readonly initialSelection = [];
  readonly allowMultiSelect = true;
  faPrint = faPrint;
  faClone = faClone;
  statusRWprogressBar: number = 0;
  numberSearchInputValue: string = '';
  nameSearchInputValue: string = '';
  clientNameSearchInputValue: string = '';
  ordersState: number;
  subscription: Subscription;
  selectedStateValue: number;
  expandedElement: Order | null;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatTable, { static: true }) table: MatTable<any>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild('searchInput', { static: true }) searchInput: ElementRef;
  @ViewChild('searchNumberInput', { static: false }) searchNumberInput: ElementRef;
  @ViewChild('searchNameInput', { static: false }) searchNameInput: ElementRef;
  @ViewChild('searchClientInput', { static: false }) searchClientInput: ElementRef;

  



  constructor(private ordersService: OrdersService, private pdfService: PdfService, public dialog: MatDialog, private router: Router, private route: ActivatedRoute) {
    this.selection = new SelectionModel<Order>(this.allowMultiSelect, this.initialSelection)
  }

  displayedColumns = ["Select", "PlanFinQty", "Number", "State", "StatusRW", "Name", "ClientName", "OrderDate", "RequiredDate"];
  //displayedColumnsHeaders = ["#", "Stan", "Ilość", "Numer", "Stan RW", "Nazwa", "Klient", "Data wpłynięcia", "Termin wykonania"];


  ngOnInit() {

    this.ordersState = Number.parseInt(this.route.snapshot.queryParams['state']);
    this.selectedStateValue = this.ordersState;
    this.dataSource = new OrdersDataSource(this.ordersService);
    this.dataSource.getOrders('', 'asc', this.ordersState, 0, 150, '', '', '');
    //refatch data on same url route
    this.subscription = this.router.events.pipe(
      filter((event: RouterEvent) => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.ordersState = Number.parseInt(this.route.snapshot.queryParams['state']);
      this.selectedStateValue = this.ordersState;
      this.dataSource.getOrders(this.sort.active, this.sort.direction, this.ordersState, 0, 150, '', '', '');
    });
    //this.newSearch.pipe(debounceTime(300)).subscribe(filter => {
    //  this.loadOrdersPage(filter);
    //});
  }
  //TODO ad observables to new inputs
  ngAfterViewInit() {

    fromEvent(this.searchNumberInput.nativeElement, 'keyup')
        .pipe(
          debounceTime(300),
          distinctUntilChanged(),
          tap(() => {
            this.loadFilteredOrders();
          })
    ).subscribe();

    fromEvent(this.searchNameInput.nativeElement, 'keyup')
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => {
          this.loadFilteredOrders();
        })
    ).subscribe();

    fromEvent(this.searchClientInput.nativeElement, 'keyup')
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => {
          this.loadFilteredOrders();
        })
      ).subscribe();
  


    //this.paginator.pageIndex = 0;
    //Create observable for searching Input elements keyup event
   //this.eventToObservable(this.searchInput);
   
   //this.eventToObservable(this.searchNameInput);
    //this.eventToObservable(this.searchClientInput);
    //this.searchNumberInput.focus();

/*    this.searchNumberInput.*/


    //this.paginator.page
    //  .pipe(
    //    tap(() => this.loadOrdersPage(''))
    //  )
    //  .subscribe();

    this.dataSource.loading$.subscribe(result => {
      if (result == false) {
        //var ordersListLength = this.dataSource.data.length;
        //var i = 1;
        //this.dataSource.data.forEach(order => {
        //  this.ordersService.getOrderRWStatus(order.orderId).subscribe((data:number) => {
        //    order.statusRW = data;
        //    order.loadingRWStatusCompleted = true;
        //    this.statusRWprogressBar = (i / ordersListLength) * 100;
        //    i++;
        //    console.log(this.statusRWprogressBar);
            
        //  })
        //})
      }
      
    })

    // Paginator - not used
    //this.paginator.page
    //  .pipe(
    //    tap(() => {
    //      this.loadOrdersPage();
    //    })
    //  )
    //  .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => this.loadOrders())
      )
      .subscribe();

    
  }

  ngOnDestroy() {
    this.subscription.unsubscribe;
  }

  showOrderDetails(order) {
    
  }

  //onPrintBomListClick(subOrder: OrderBook, event: any) {
  //  this.isPrinting = true;
  //  this.pdfService.generateBomList(subOrder, true, 0)
  //    .subscribe(response => {
  //      const blob = new Blob([response], { type: 'application/pdf' });
  //      var url = window.URL.createObjectURL(blob);
  //      var bookList = window.open(url);
  //      this.isPrinting = false;
  //      bookList.print();

  //    },
  //      (error) => {
  //        console.log(error);
  //        this.isPrinting = true;
  //      });
  //}

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

  onPrintOrders() {
    this.isOrdersListPrinting = true;
    this.pdfService.generateOrderList(this.selection.selected).subscribe(response => {
      const blob = new Blob([response], { type: 'application/pdf' });
      var url = window.URL.createObjectURL(blob);
      var bookList = window.open(url);
      bookList.print();
      this.isOrdersListPrinting = false;
    })
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected == numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  loadFilteredOrders() {
    //this.dataSource.getOrders(this.sort.active, this.sort.direction, this.paginator.pageIndex, this.paginator.pageSize, this.searchInput.nativeElement.value);
    this.dataSource.getOrders(this.sort.active, this.sort.direction, this.ordersState, 0, 150,

      this.nameSearchInputValue, this.numberSearchInputValue, this.clientNameSearchInputValue);
  }

  loadOrders() {
    //this.dataSource.getOrders(this.sort.active, this.sort.direction, 0, 15, '');
    this.dataSource.getOrders(this.sort.active, this.sort.direction, this.ordersState, 0, 150, '','','');
  }

  onDeleteOrder(span: HTMLSpanElement) {
    var selectedOrderId = parseInt(span.id);
    let element: HTMLElement = document.getElementById(selectedOrderId.toString()) as HTMLElement;
    element.click();
    this.confirmOrderDelete();
  }

  onShowOrderDetail(span: HTMLSpanElement) {
    var selectedOrderId = parseInt(span.id);
    this.router.navigate([`/orders/${selectedOrderId}`]);
  }

  printOrderDetails(span: HTMLSpanElement) {
    this.isOrderDetailPrinting = true;
    var selectedOrderId = parseInt(span.id);
    this.pdfService.generateOrderDetailsList(selectedOrderId).subscribe(response => {
      const blob = new Blob([response], { type: 'application/pdf' });
      var url = window.URL.createObjectURL(blob);
      var orderDetails = window.open(url);
      orderDetails.print();
      this.isOrderDetailPrinting = false;
    });
  }

  onArchiveOrder(span: HTMLSpanElement) {
    var selectedOrderId = parseInt(span.id);
    this.dataSource.archiveOrder(selectedOrderId);
    this.onReload();
  }

  confirmOrderDelete(): void {
    const dialogRef = this.dialog.open(ConfirmDeleteComponent, {
      width: '500px',
      data: { message: `Czy na pewno chcesz usunąć zlecenie ${this.selectedOrder.number}?` }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result == true) {
        this.dataSource.deleteOrder(this.selectedOrder);
      }
    });

  }
  onDetailExpand(order: Order) {
    this.selectedOrder = order;
    //setTimeout(() => { this.showOrderDetails = true; }, 1000);
  }

  //onClientSearchFilterKeyUp(event: KeyboardEvent) {
    
  //}
  //onNameSearchFilterKeyUp(event: KeyboardEvent) {

  //}

  //onNumberSearchFilterKeyUp(event: KeyboardEvent) {
  //  this.searchFilter.filterByClientName = this.searchNumberInput.;
  //  console.log(this.searchNumberInput);
  //  console.log(this.searchFilter);
  //  this.newSearch.next(this.searchFilter);
  //}

  onNewOrder() {

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      title: 'Nowe zlecenie',
      initialQty: 1,
      orderType: 0
    };

    const dialogRef = this.dialog.open(NewOrderComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(
      data => {
        if (data != null) {
          this.ordersService.createOrder(data).subscribe(
            () => { this.onReload() })
        }
      }
    )
  }

  onNewOrderFromExisting(existingOrder: Order) {

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      title: `Utwórz nowe zlecenie na podstawie ${existingOrder.number}`,
      initialQty: existingOrder.plannedQty,
      orderType: existingOrder.type
    };

    const dialogRef = this.dialog.open(NewOrderComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(
      (newOrder: Order) => {

        //copy order in data base
        if (newOrder != null) {

          //open progress window
          const progressDialogRef = this.dialog.open(ProgressBarComponent,
            {
              disableClose: true, minWidth: '600px', minHeight: "150px",
              data: {
                title: `Tworzenie nowego Zlecenia: ${newOrder.number} - ${newOrder.name}`,
                message: "Proszę czekać",
                canClose: false
              }
            })

          this.ordersService.copyOrder(existingOrder.orderId, newOrder).subscribe(
            {
              next: () => {
                this.onReload();
                console.log("order copied");
                setTimeout(() => progressDialogRef.close(), 1000);

              },
              error: (err: HttpErrorResponse) => {
                console.log(err);
                setTimeout(() => progressDialogRef.close(), 1000);
                let dialogRef = this.dialog.open(ErrorMessageComponent, {
                  minWidth: "400px",
                  minHeight: "150px",
                  disableClose: true,
                  data: err
                });
              }
          });
        }
        //console.log(newOrder);
      })
  }

  onEditOrder(span: HTMLSpanElement) {
    var selectedOrderId = parseInt(span.id);

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      title: 'Edycja zlecenia',
      id: selectedOrderId
    };

    const dialogRef = this.dialog.open(EditOrderComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(
      data => {
        if (data != null) {

          const progressDialogConf: MatDialogConfig = {

            data: {
              title: `Aktualizacja zlecenia ${this.selectedOrder.name}`,
              message: "Proszę czekać",
              canClose: false
            }
          };

          const progressDialogRef = this.dialog.open(ProgressBarComponent, progressDialogConf);

          this.ordersService.updateOrder(data).subscribe(
            () => {
              this.onReload()
              progressDialogRef.close();//afterClosed().subscribe();
            })
        }
      }
    )
  }

  onReload() {
    this.dataSource.refresh();
    //this.dataSource.loading$.subscribe((loading) => {
    //  if (loading == false) {
    //    setTimeout(() => {
    //      this.searchInput.nativeElement.value = '';
    //    }, 5000);
       
    //  }
    //});
   
    //this.searchNumberInput.nativeElement.value = '';
    this.searchNameInput.nativeElement.value = '';
    this.searchClientInput.nativeElement.value = '';
    //this.paginator.pageIndex = 0;
  }

  onDoubleOrderClick() {
    //console.log("dbl click");
  }

  //eventToObservable(element: ElementRef) {
  //  if (element) {
  //    fromEvent(element.nativeElement, 'keyup')
  //      .pipe(
  //        debounceTime(300),
  //        distinctUntilChanged(),
  //        tap(() => {
  //          this.loadOrdersPage();
  //        })
  //      ).subscribe();
  //  }
  //  else {
  //    //console.log('element undefined');
  //  }
  //}

  activatedInput(event) {
    //console.log(event.target.id);
    //this.activeInput = event.target.id;
  }

  printState(order:Order): string {
    if (order.state == 0)
    { return 'W produkcji' }
    else if (order.state == 10)
    { return 'Zakończony' }
  }

  onStateChange(order: Order, event) {
    //update locally
    let index: number = this.dataSource.data.findIndex(item => item.orderId == order.orderId);
    this.dataSource.data[index].state = event.value;
    //update in DB
    this.ordersService.archiveOrder(order.orderId.toString(),event.value.toString()).subscribe();
  }

  onTableRowClick(order: Order) {
    this.expandedElement = this.expandedElement === order ? null : order;
    this.selectedStateValue = order.state;
  }

}
