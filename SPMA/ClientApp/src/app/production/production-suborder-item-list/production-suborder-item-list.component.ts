import { DOCUMENT } from '@angular/common';
import { AfterViewInit } from '@angular/core';
import { ElementRef, Inject, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatSort, MatTableDataSource } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { Socket } from 'dgram';
import { SuborderEditErrorComponent } from '../../dialogs/suborder-edit-error/suborder-edit-error.component';
import { BookComponent } from '../../models/books/bookcomponent.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { SubOrderItem } from '../../models/suborders/suborderitem.model';
import { Ware } from '../../models/warehouse/ware.model';
import { BookService } from '../../services/books/book.service';
import { BookComponentService } from '../../services/production/bookcomponent.service';
import { SubOrderDataService } from '../../services/suborder/sub-order-data.service';
import { SubOrderService } from '../../services/suborder/suborder.service';
import { ProductionState } from '../../shared/Enums/productionstate';

@Component({
  selector: 'app-production-suborder-item-list',
  templateUrl: './production-suborder-item-list.component.html',
  styleUrls: ['./production-suborder-item-list.component.css']
})
export class ProductionSuborderItemListComponent implements OnInit, AfterViewInit {

  title: string = 'Wybór elementu podzlecenia';
  socketType: string;
  sourceType; number;
  orderId: number;
  subOrderId: number;
  officeNumber: string;
  componentNumber: string;
  scrollIndex: number;
  //scrollPosition: number;
  displayedColumns: string[] = ['name','number','material','status','description','button'];
  progressBarValue: number;
  subOrderItemNumber: string = 'Aktualizowanie części';
  loadingSubOrderSteps: boolean[] = [false, false, false, false];
  selectedSubOrderItem: SubOrderItem = new SubOrderItem;
  selectedIndex: number = 0;
  originalSubOrderItems: SubOrderItem[] = [];
  isLoadingSubOrder: boolean = true;
  wareToBeAdded: Ware = new Ware();
  wareToBeAddedLengthM: number;
  allowAddWareToSubOrderItem: boolean;
  allowAddWareToPurchaseComponent: boolean;
  selectedPurchaseSubOrderItem: SubOrderItem = new SubOrderItem;
  staticSuborderData: SubOrderItem[] = [];
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  materialInput = new FormControl();
  numberInput = new FormControl();
  nameInput = new FormControl();


  constructor(private router: Router, private route: ActivatedRoute, private bookService: BookService,
    private bookComponentService: BookComponentService, private subOrderService: SubOrderService,
    private dialog: MatDialog, private subOrderData: SubOrderDataService, @Inject(DOCUMENT) private _document: Document) { }

  ngOnInit() {

    this.orderId = this.route.snapshot.queryParams['orderID'];
    this.subOrderId = this.route.snapshot.queryParams['subOrderID'];
    this.officeNumber = this.route.snapshot.queryParams['officeNumber'];
    this.componentNumber = this.route.snapshot.queryParams['componentNumber'];
    this.socketType = this.route.snapshot.queryParams['socket'];
    this.scrollIndex = this.route.snapshot.queryParams['index'];

    if (this.socketType == "saw") {
      this.sourceType = 0;
    } else if (this.socketType == "plasma") {
      this.sourceType = 2;
    }

    if (!this.subOrderData.downloaded) {
      // Combine all methods and retrive data
      this.subOrderData.getBookInfo(this.componentNumber, this.officeNumber).then(() => {
        this.subOrderData.getSubOrderInfo(this.subOrderId).then(() => {
          this.loadingSubOrderSteps[0] = true;
          //console.log(this.subOrderBookComponent);
          this.subOrderData.getSubOrder(this.subOrderId, this.sourceType).then(() => {
            this.loadingSubOrderSteps[1] = true;
            this.subOrderData.getSubOrderAdditionalInfo().then(() => {
              this.loadingSubOrderSteps[2] = true;
              this.subOrderItemNumber = "Aktualizowanie części.";
              // Update suborder input fields
              //this.bookQuantityInput.setValue(this.subOrderInfo.plannedQty);
              //this.bookNameInput.setValue(this.subOrderInfo.book.name);
              //this.bookComponentNumberInput.setValue(this.subOrderBookComponent.component.number);
              //this.subOrderInput.setValue(this.subOrderInfo.number);
              //this.bookOfficeNumberInput.setValue(this.subOrderInfo.book.officeNumber);
              this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.slice();
              this.staticSuborderData = Object.assign([],this.subOrderData.subOrder.data);
              console.log(this.staticSuborderData);
            },
              (rejected) => {
                const errorDialogConfig = new MatDialogConfig();
                errorDialogConfig.disableClose = true;
                errorDialogConfig.autoFocus = true;
                errorDialogConfig.data = rejected;
                errorDialogConfig.width = "28vw";

                const errorDialogRef = this.dialog.open(SuborderEditErrorComponent, errorDialogConfig);
                errorDialogRef.afterClosed().subscribe(() => {
                  this.backToSubOrderList();
                });
              }).finally(() => {

                //this.selectedSubOrderItem = this.subOrder.data[this.selectedIndex];
                //this.subOrderItemToEdit = this.subOrder[this.selectedIndex];
                //!!!!!!this.componentQuantityInput.setValue(this.selectedSubOrderItem.inProduction.plannedQty);
                //if (this.subOrder.filter(item => item.inProduction.sourceType == 5).length > 0) {
                //  this.selectedPurchaseSubOrderItem = this.subOrder.filter(item => item.inProduction.sourceType == 5)[this.selectedPurchaseIndex];
                //  this.isPurchaseItemsPresent = true;
                //}
                // Copy array
                //this.originalSubOrderItems = JSON.parse(JSON.stringify(this.subOrderData.subOrder.data));
                //this.subOrder.forEach(item => {
                //  this.originalSubOrderItems.push(Object.assign({}, item));
                //});
                //console.log(this.originalSubOrderItems);
                //this.updateAddToPurchaseSubOrderItemButton();
                // this.updateAddToSubOrderItemButton();
                this.loadingSubOrderSteps[3] = true;
                this.subOrderData.downloaded = true;
                setTimeout(() => {
                  if (this.loadingSubOrderSteps.every(x => x == true)) {
                    this.isLoadingSubOrder = false;
                    this.setTableHeaderSort();
                  }
                }, 2000)

              })
          })
        })
      })
    } else {
      this.isLoadingSubOrder = false;
      this.staticSuborderData = Object.assign([], this.subOrderData.subOrder.data);
    }

  }

  ngAfterViewInit() {
    if (this.subOrderData.downloaded) {
      this.scrollIndex == null ? this.scrollIndex = 0 : '';
      if (this.scrollIndex > 2) {
        let val = (this.scrollIndex - 2) * 48;
        document.getElementById('scrollTable').scrollTo(0, val);
      }
    }
  }

  setTableHeaderSort() {
    console.log(this.sort);
    setTimeout(() => {
      this.sort.sortChange.subscribe(() => {
        if (this.sort.active == 'number') {
          if (this.sort.direction == "asc")
            this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.sort((a, b) => a.inProduction.component.number.localeCompare(b.inProduction.component.number));
          else {
            this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.sort((a, b) => -a.inProduction.component.number.localeCompare(b.inProduction.component.number));
          }
        } else if (this.sort.active == 'name') {
          if (this.sort.direction == "desc")
            this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.sort((a, b) => a.inProduction.component.name.localeCompare(b.inProduction.component.name));
          else {
            this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.sort((a, b) => -a.inProduction.component.name.localeCompare(b.inProduction.component.name));
          }
        } else if (this.sort.active == 'material') {
          if (this.sort.direction == "desc")
            this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.sort((a, b) => a.inProduction.ware.name.localeCompare(b.inProduction.ware.name));
          else {
            this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.sort((a, b) => -a.inProduction.ware.name.localeCompare(b.inProduction.ware.name));
          }
        }

      });
    }, 1000)

  }

  // Update addWareToComponentButton
  //updateAddToSubOrderItemButton() {
  //  // Check if ware can be added to component
  //  if (this.selectedSubOrderItem.ware == null) {
  //    if (this.wareToBeAdded != null && this.wareToBeAddedLengthM != null) {
  //      this.allowAddWareToSubOrderItem = true;
  //    }
  //    else {
  //      this.allowAddWareToSubOrderItem = false;
  //    }
  //  }
  //  else {
  //    this.allowAddWareToSubOrderItem = false;
  //  }
  //}

  //// Update addWareToPurchaseComponentButton
  //updateAddToPurchaseSubOrderItemButton() {
  //  // Check if ware can be added to component
  //  if (this.selectedPurchaseSubOrderItem.ware != null) {
  //    this.allowAddWareToPurchaseComponent = false;
  //  }
  //  else {
  //    this.allowAddWareToPurchaseComponent = true;
  //  }
  //}

  backToSubOrderList() {
    //console.log('click!');
    this.subOrderData.downloaded = false;
    this.router.navigate(['/production/productionSuborderList'],
      {
        queryParams: {
          orderID: this.orderId,
          socket: this.socketType
        }
      });
  }

  toBandSawScreen() {

    this.router.navigate(['/production/bandsaw'],
      {
        queryParams: {
          orderID: this.orderId,
          subOrderID: this.subOrderId,
          officeNumber: this.officeNumber,
          componentNumber: this.componentNumber,
          index: this.subOrderData.subOrder.filteredData.indexOf(this.selectedSubOrderItem),
          socket: this.socketType
        }
      });
  }

  onSubOrderItemClick(subOrderItem: SubOrderItem) {
    if (subOrderItem == this.selectedSubOrderItem) {
      return;
    }
    this.selectedSubOrderItem = subOrderItem;
  }

  onReloadPage() {
    this.subOrderData.downloaded = false;
    this._document.defaultView.location.reload();
  }


  assignCopy() {
    this.subOrderData.subOrder.data = Object.assign([], this.staticSuborderData);
  }

  onNumberInputClear() {
    this.numberInput.setValue('');
    this.filterSuborders();
  }

  onNameInputClear() {
    this.nameInput.setValue('');
    this.filterSuborders();
  }

  onMaterialInputClear() {
    this.materialInput.setValue('');
    this.filterSuborders();
  }

  filterSuborders() {

    let numberValue = this.numberInput.value ? this.numberInput.value.toLowerCase() : '';
    let nameValue = this.nameInput.value ? this.nameInput.value.toLowerCase() : '';
    let materialValue = this.materialInput.value ? this.materialInput.value.toLowerCase() : '';
    if (numberValue == '' && nameValue == '' && materialValue == '') {
      this.subOrderData.subOrder.data = Object.assign([], this.staticSuborderData);
    } else {
      this.subOrderData.subOrder.data = Object.assign([], this.staticSuborderData).filter(
        item => {
          let val: boolean;
          val = item.inProduction.component.name.toLowerCase().indexOf(nameValue) > -1 &&
            item.inProduction.component.number.toLowerCase().indexOf(numberValue) > -1 &&
            item.ware.name.toLowerCase().indexOf(materialValue) > -1;
          return val
        })
    }
  }

  setTrBgColor(subOrderItem: SubOrderItem): string {
    switch (subOrderItem.inProduction.productionState.name) {
      case 'Ucięty': {
        return 'lawngreen'
      }
      case 'Rezerwacja': {
        return 'mediumorchid'
      } default: {
        return 'none'
      }
    }
  }

}
