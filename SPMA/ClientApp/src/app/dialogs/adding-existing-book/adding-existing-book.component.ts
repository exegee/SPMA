import { SelectionModel } from '@angular/cdk/collections';
import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialogRef, MatSelectionList, MatTableDataSource, MAT_DIALOG_DATA } from '@angular/material';
import { faCheckSquare, faSquare } from '@fortawesome/free-regular-svg-icons';
import { debounceTime, finalize, last, switchMap, tap } from 'rxjs/operators';
import { Order } from '../../models/orders/order.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { OrdersService } from '../../services/orders/orders.service';
import { SubOrderService } from '../../services/suborder/suborder.service';
import { BookListFromOrders } from '../../shared/Interfaces/BookListFromOrders';

interface inputData {
  'order': Order,
  'subOrders': OrderBook[]
}


@Component({
  selector: 'app-adding-existing-book',
  templateUrl: './adding-existing-book.component.html',
  styleUrls: ['./adding-existing-book.component.css']
})
export class AddingExistingBookComponent implements OnInit {


  faCheckSquare = faCheckSquare;
  faSquare = faSquare;
  orderSelected: boolean = false;
  isLoading: boolean;
  orderEmpty: boolean = true;
  isAnyElChecked: boolean = false;
  //loadedSuborders: OrderBook[];
  choosenSubOrders: OrderBook[] = [];
  filteredOrders: Order[];
  chooseOrderInput: FormControl = new FormControl();
  loadedSuborders = new MatTableDataSource<OrderBook>();
  selection = new SelectionModel<OrderBook>(true, []);
  displayedColumns: string[] = ['Numer','Opis','Select'];
  @ViewChild('suborderList', { static: false }) suborderListRef: MatSelectionList;
    

  constructor(private dialogRef: MatDialogRef<AddingExistingBookComponent>,
    @Inject(MAT_DIALOG_DATA) private data: inputData, private orderService: OrdersService,
    private subOrderService: SubOrderService) { }

  ngOnInit() {
    //this.filteredOrders = this.chooseOrderInput.valueChanges.pipe(
    //  startWith(''),
    //  map(value => this._filter(value))
    //);
    //console.log(this.input);

    this.chooseOrderInput.valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isLoading = true;
          }
        }
        ),
        switchMap(value => value.length >= 3 ? this.orderService.getFilteredOrders(this.chooseOrderInput.value).pipe(finalize(() => {
          this.isLoading = false

        })) : []),
        finalize(() => {
          this.isLoading = false
        })
      ).subscribe(items => {
        this.filteredOrders = items;
        this.filteredOrders = this.filteredOrders.filter(order => order.orderId != this.data.order.orderId);
      }, () => { });

  }

  //things happen when books are added to current order
  onBookAdd(): void {
    console.log(this.selection.selected);
    let length = this.data.subOrders.length;
    let startPosition = this.getNextSubOrderNumber(this.data.subOrders[length - 1].number);
    let startPositionStr: string;
    this.selection.selected.forEach((item) => {
    startPosition++;
    startPosition < 10 ? startPositionStr = `0${startPosition}` : startPositionStr = `${startPosition}`;
    item.addedDate = this.data.order.orderDate//new Date();//actualize date
    item.position = startPosition;//actualize position
    item.number = `${this.data.order.number}_${startPositionStr}`;//actualize subOrder number
    item.order = this.data.order;//actualize order
    item.orderId = this.data.order.orderId;//actualize orderId
    this.choosenSubOrders.push(item);
    })
 
    //console.log(this.choosenSubOrders);
    //this.dialogRef.close(this.choosenSubOrders);
    

  }

  getNextSubOrderNumber(lastNumber: string): number{
    let lastValue: string = lastNumber.substring(10);
    let num = Number.parseInt(lastValue);
    return num
  }

  //data format displayed by autocomplete option selection
  ordersDisplay(item: Order) {
    if (item) {

      return `${item.number} - ${item.name}`;
    }

  }

  //autocomplete option selection
  onOrderSelected(selectedOrder: Order) {
    console.log("item selcted: ", selectedOrder);
    this.orderEmpty = true;
    this.orderService.getOrderBooks(selectedOrder.orderId).subscribe(
      (data: OrderBook[]) => {
        this.loadedSuborders.data = [];
        data.forEach(item => {
          if (!this.findSameListItem(item)){
            this.loadedSuborders.data.push(item);
          }
        })
        this.orderSelected = true;
        if (this.loadedSuborders.data.length == 0) {
          this.orderEmpty = true;
          this.isAnyElChecked = false;
        } else {
          this.orderEmpty = false;
          setTimeout(() => {
            console.log(this.suborderListRef);
          }, 200);
        }
      });

  }


  //find same element in searched order(in autocomplete) and in editing order elements
  //if element already exist in editing order than its not selected in list by default
  findSameListItem(suborder: OrderBook) {

    for (let item of this.data.subOrders) {
      if (item.book.bookId == suborder.book.bookId)
        return true;
    }
    return false;
  }

  //clear searching input
  clearInput() {
    this.chooseOrderInput.setValue('');
  }

    /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.loadedSuborders.data.length;
    return numSelected === numRows;
  }

  isAnyElementChecked(): boolean{
    if (this.selection.selected.length != 0) {
      return true
    } else {
      return false
    }
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
      return;
    }
    console.log(this.selection);
    this.loadedSuborders.data.forEach(row => this.selection.select(row));
  }

  /** The label for the checkbox on the passed row */
  checkboxLabel(row?: OrderBook): string {
    if (!row) {
      return `${this.isAllSelected() ? 'deselect' : 'select'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.position + 1}`;
  }

}
