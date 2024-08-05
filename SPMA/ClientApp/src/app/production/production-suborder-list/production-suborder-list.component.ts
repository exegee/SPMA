import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSort, MatTable, MatTableDataSource } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { Order } from '../../models/orders/order.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { OrdersService } from '../../services/orders/orders.service';
import { SubOrderService } from '../../services/suborder/suborder.service';


interface completionStatus {
  sawStatusPercentage: number ,
  plasmaStatusPercentage: number
}



@Component({
  selector: 'app-production-suborder-list',
  templateUrl: './production-suborder-list.component.html',
  styleUrls: ['./production-suborder-list.component.css']
})
export class ProductionSuborderListComponent implements OnInit, AfterViewInit {


  title: string = 'Wybór podzlecenia';
  socketType: string;
  orderID: number = 0;
  mainOrder: Order = new Order();
  //subordersList: OrderBook[] =[];
  selectedSubOrder: OrderBook = new OrderBook();
  displayedColumns: string[] = ['number', 'componentNumber', 'bookOfficeNumber', 'state', 'addedDate', 'button'];
  filteredOrderBooks: MatTableDataSource<OrderBook> = new MatTableDataSource<OrderBook>();
  orderBooks: OrderBook[] = [];
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild('subordernumberfilter', { static: false }) subOrderNumberFilterInput: ElementRef;
  @ViewChild('componentnumberfilter', { static: false }) componentNumberFilterInput: ElementRef;
  @ViewChild('descriptionfilter', { static: false }) descriptionFilterInput: ElementRef;
  componentNumberInput = new FormControl();
  numberInput = new FormControl();
  descriptionInput = new FormControl();
  completionStatuses: number[] = [];
  test: number;
  

  constructor(private router: Router, private route: ActivatedRoute, private ordersService: OrdersService,
              private subOrderService: SubOrderService) { }

  ngOnInit() {

    //get choosen order ID value
    this.orderID = this.route.snapshot.queryParams['orderID'];
    this.socketType = this.route.snapshot.queryParams['socket'];

    //download  main order object
    this.ordersService.getOrder(this.orderID).subscribe(
      (data: Order) => {
        this.mainOrder = data;
        //console.log(this.mainOrder);
      },
      (error) => { console.log(error) },
      () => {
      });

    this.subOrderService.getCutStatuses(this.orderID)
      .subscribe((data: completionStatus[]) => {
        if (this.socketType == 'saw') {
          data.forEach(item => this.completionStatuses.push(item.sawStatusPercentage));
        } else {
          data.forEach(item => this.completionStatuses.push(item.plasmaStatusPercentage));
        }
          
    });

    //this.subOrderService.getCutStatus(this.selectedSubOrder.orderBookId).subscribe(
    //  (input: number[]) => {
    //    console.log(input);
    //    this.sawStatusPercentage = input[0];
    //    this.plasmaStatusPercentage = input[1];
    //  }
    /*);*/

    //download suborders
    //this.getOrderBooksAsync();
    this.ordersService.getOrderBooksAsync(this.orderID)
      .then((data: OrderBook[]) => {
       // this.subordersList = data;
        this.orderBooks = data;
        this.filteredOrderBooks.data = data;
       //console.log(data);
      });

  }

  ngAfterViewInit() {

    this.sort.sortChange.subscribe(() => {
      console.log(this.sort);
      if (this.sort.active == 'number') {
        if (this.sort.direction == "asc")
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => a.number.localeCompare(b.number));
        else {
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => -a.number.localeCompare(b.number));
        }
      } else if (this.sort.active == 'componentNumber') {
        if (this.sort.direction == "desc")
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => a.componentNumber.localeCompare(b.componentNumber));
        else {
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => -a.componentNumber.localeCompare(b.componentNumber));
        }
      } else if (this.sort.active == 'bookOfficeNumber') {
        if (this.sort.direction == "desc")
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => a.book.officeNumber.localeCompare(b.book.officeNumber));
        else {
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => -a.book.officeNumber.localeCompare(b.book.officeNumber));
        }
      }
      else if (this.sort.active == 'addedDate') {
        if (this.sort.direction == "desc")
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => new Date(a.addedDate).getTime() - new Date(b.addedDate).getTime());
        else {
          this.filteredOrderBooks.data = this.filteredOrderBooks.data.sort((a, b) => new Date(b.addedDate).getTime() - new Date(a.addedDate).getTime());
        }
      }

    });

  }

  filterSuborders() {

    let numberValue = this.numberInput.value ? this.numberInput.value.toLowerCase() : '';
    let componentNameValue = this.componentNumberInput.value ? this.componentNumberInput.value.toLowerCase() : '';
    let descriptionValue = this.descriptionInput.value ? this.descriptionInput.value.toLowerCase() : '';
    if (numberValue == '' && componentNameValue == '' && descriptionValue == '') {
      this.filteredOrderBooks.data = Object.assign([], this.orderBooks);
    } else {
      this.filteredOrderBooks.data = Object.assign([], this.orderBooks).filter(
        item => {
          let val: boolean;
          val = item.componentNumber.toLowerCase().indexOf(componentNameValue) > -1 &&
            item.number.toLowerCase().indexOf(numberValue) > -1 &&
            (item.book.officeNumber.toLowerCase().indexOf(descriptionValue) > -1 ||
              item.book.name.toLowerCase().indexOf(descriptionValue) > -1);
          return val
        })
    }
  }

  assignCopy() {
    this.filteredOrderBooks.data = Object.assign([], this.orderBooks);
  }

  onDescriptionFilterInputFocus() {
    this.subOrderNumberFilterInput.nativeElement.value = "";
    this.componentNumberFilterInput.nativeElement.value = "";
    this.assignCopy();
  }

  onNumberFilterInputFocus() {
    this.descriptionFilterInput.nativeElement.value = "";
    this.subOrderNumberFilterInput.nativeElement.value = "";
    this.assignCopy();
  }

  onSubOrderFilterInputFocus() {
    this.descriptionFilterInput.nativeElement.value = "";
    this.componentNumberFilterInput.nativeElement.value = "";
    this.assignCopy();
  }

  onDescriptionInputClear() {
    this.descriptionInput.setValue('');
    this.filterSuborders();
  }

  onNumberInputClear() {
    this.numberInput.setValue('');
    this.filterSuborders();
  }

  onComponentNumberInputClear() {
    this.componentNumberInput.setValue('');
    this.filterSuborders();
  }

  backToOrderList() {
    //console.log('click!');
    this.router.navigate(['/production/productionOrderList'], { queryParams: {socket: this.socketType} });
  }

  //async getOrderBooksAsync(): Promise<any> {
  //  return await this.ordersService.getOrderBooksAsync(this.orderID)
  //    .then((data: OrderBook[]) => {
  //      this.subordersList = data;
  //      //console.log(data);
  //    });
  //}

  onSubOrderClick(subOrder: OrderBook) {
    if (subOrder == this.selectedSubOrder) {
      return;
    }
    this.selectedSubOrder = subOrder;
  }


  toSuborderItemsList() {
    //console.log(element);
    this.router.navigate([`/production/productionSuborderItemList`],
      {
        queryParams: {
          orderID: this.orderID,
          subOrderID: this.selectedSubOrder.orderBookId,
          officeNumber: this.selectedSubOrder.book.officeNumber,
          componentNumber: this.selectedSubOrder.componentNumber,
          socket: this.socketType
        }
      });
    //console.log(this.selectedSubOrder);
  }
}
