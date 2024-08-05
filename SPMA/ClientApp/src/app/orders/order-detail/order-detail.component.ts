import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatSort, MatTableDataSource } from '@angular/material';
import { ActivatedRoute, NavigationExtras, Params, Router } from '@angular/router';
import { faCalendarAlt, faEdit, faFileExport, faPrint, faTrashAlt, faPlus } from '@fortawesome/free-solid-svg-icons';
import * as FileSaver from 'file-saver';
import * as moment from 'moment';
import { ContextMenuComponent } from 'ngx-contextmenu';
import { ToastrService } from 'ngx-toastr';
import { Order } from 'src/app/models/orders/order.model';
import { Book } from '../../models/books/book.model';
import { OptimaMag } from '../../models/optima/optimamag.model';
import { OptimaRW } from '../../models/optima/optimarw.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { InProductionXML } from '../../models/production/inproductionxml.model';
import { OptimaService } from '../../services/optima/optima.service';
import { OrdersService } from '../../services/orders/orders.service';
import { PdfService } from '../../services/utility/pdf.service';
import { XmlService } from '../../services/utility/xml.service';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, tap } from 'rxjs/operators';
import { AddingExistingBookComponent } from '../../dialogs/adding-existing-book/adding-existing-book.component';
import { error } from 'util';
import { ErrorMessageComponent } from '../../dialogs/error-message/error-message.component';
import { HttpErrorResponse } from '@angular/common/http';
import { ProgressBarComponent } from '../../dialogs/progress-bar/progress-bar.component';
import { Ware } from '../../models/warehouse/ware.model';
import { DataSource, SelectionModel } from '@angular/cdk/collections';
import { SubOrderService } from '../../services/suborder/suborder.service';


interface cutStatuses {
  sawStatusPercentage: number,
  plasmaStatusPercentage: number
}

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css'],
  providers: [],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('500ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
    trigger('blurProgress', [
      state('enabled', style({
        opacity: 0
      })),
      state('disabled', style({
        opacity: 1
      })),
      transition('enabled => disabled', [animate(500)])
    ]),
  ]
})
export class OrderDetailComponent implements OnInit, AfterViewInit {

  // Icons
  faTrashAlt = faTrashAlt;
  faPrint = faPrint;
  faFileExport = faFileExport;
  faEdit = faEdit;
  faCalendarAlt = faCalendarAlt;
  faPlus = faPlus;

  filteredOrderBooks = new MatTableDataSource<OrderBook>();//zamienić później na filteredOrderBooks
  displayedColumns: string[] = ['Select', 'Position', 'Number', 'ComponentNumber', 'Name', 'Finished','OrderDate' ];
  selection = new SelectionModel<OrderBook>(true, []);
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  expandedElement: OrderBook | null;
  componentNumberInput = new FormControl();
  numberInput = new FormControl();
  descriptionInput = new FormControl();
  componentNumbers: string[] = [];
  orderBooksNumbers: string[] = [];

  id: number;
  orderBooks: OrderBook[];
  selectedSubOrder: OrderBook = new OrderBook();
  newSubOrders: OrderBook[] = [];
  order: Order;
  subOrderToDelete: OrderBook;
  deletingSubOrder: boolean = false;
  isExporting: boolean = false;
  isPrinting: boolean = false;
  rwDate: moment.Moment;
  rwDateStr: string;
  wareList: InProductionXML[];
  loadingWares: boolean = false;
  rwUpdateQuantities: boolean = true;
  xmlListType: number;
  isSummaryList: boolean = false;
  isMultipleSummaryList: boolean;
  subOrderRWS: OptimaRW[] = [
    new OptimaRW(-1, "RW/000/0000", -1, 0.0, "##### #####", new Date(), new Date()),
    new OptimaRW(-1, "RW/000/0000", -1, 0.0, "##### #####", new Date(), new Date()),
    new OptimaRW(-1, "RW/000/0000", -1, 0.0, "##### #####", new Date(), new Date()),
    new OptimaRW(-1, "RW/000/0000", -1, 0.0, "##### #####", new Date(), new Date())];
  subOrderExpandValue: boolean = false;
  @Output() subOrderExpandChange: EventEmitter<boolean> = new EventEmitter();
  isSubOrderExpanded: boolean = false;
  isCheckingRWS: boolean = false;
  blurProgress: string[] = [
    'enabled',
    'enabled',
    'enabled',
    'enabled',
  ];
  checkSubOrderRWSCompleted: boolean = false;
  cutStatuses: cutStatuses[] = [];

  optimaMags: OptimaMag[] = [];
  selectedMag: OptimaMag;

  @ViewChild(ContextMenuComponent, { static: true }) public subOrderMenu: ContextMenuComponent;
  @ViewChild('subordernumberfilter', { static: false }) subOrderNumberFilterInput: ElementRef;
  @ViewChild('componentnumberfilter', { static: false }) componentNumberFilterInput: ElementRef;
  @ViewChild('descriptionfilter', { static: false }) descriptionFilterInput: ElementRef;

  form: FormGroup;


  set subOrderExpand(value: boolean) {
    this.subOrderExpandValue = value;
    this.subOrderExpandChange.emit(this.subOrderExpandValue);
  }
  @Input()
  get subOrderExpand() {
    return this.subOrderExpandValue;
  }

  subOrderExpandEmitter() {
    return this.subOrderExpandChange;
  }

  // **************
  // typeahead, moze kiedys sie wykorzysta
  public model: any;
       search = (text$: Observable<string>) =>
        text$.pipe(
          debounceTime(200),
          distinctUntilChanged(),
          map(term => term.length < 2 ? []
            : this.orderBooks.filter(v => v.number.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
    )
  resultFormatBandListValue(value: OrderBook) {
    return value.number;
  }
  //

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

  filterSuborders() {
    this.selection.clear();
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


  
  constructor(private route: ActivatedRoute, private ordersService: OrdersService,
    private pdfService: PdfService, public dialog: MatDialog, private toastr: ToastrService,
    private xmlService: XmlService, private optimaService: OptimaService, private subOrderService: SubOrderService,
    private router: Router) { }

  ngOnInit() {
    this.subOrderToDelete = null;
    this.route.params
      .subscribe(
        (params: Params) => {
          this.id = +params['id'];
        }
      );
    //this.ordersService.getOrderBooks(this.id).subscribe(
    //  (data: OrderBook[]) => {
    //    this.orderBooks = data;
    //    console.log(this.orderBooks);
    //  },
    //  (error) => { console.log(error) },
    //  () => {
    //  });
    this.getOrderBooksAsync();
    

    this.ordersService.getOrder(this.id).subscribe(
      (data: Order) => {
        this.order = data;
        this.subOrderService.getCutStatuses(this.order.orderId).subscribe((data: cutStatuses[]) => {
          this.cutStatuses = data;
          console.log(this.cutStatuses);
        });
        //console.log(this.order);
      },
      (error) => { console.log(error) },
      () => {
      });


    this.subOrderExpandChange.subscribe((response) => {
      console.log("Event: " + response);
    })
    this.getOptimaMags();

    this.form = new FormGroup({
      'rwDate': new FormControl(moment())  
    });

    this.rwDate = this.form.get('rwDate').value as moment.Moment;
    this.rwDateStr = this.rwDate.format("YYYY-MM-DD");
  }

  ngAfterViewInit() {

    this.sort.sortChange.subscribe(() => {
      console.log(this.sort);
      if (this.sort.active == 'Number') {
        if (this.sort.direction == "asc")
          this.filteredOrderBooks.data=this.filteredOrderBooks.data.sort((a, b) => a.number.localeCompare(b.number));
        else {
          this.filteredOrderBooks.data=this.filteredOrderBooks.data.sort((a, b) => -a.number.localeCompare(b.number));
        }
      } else if (this.sort.active == 'ComponentNumber') {
        if (this.sort.direction == "desc")
          this.filteredOrderBooks.data=this.filteredOrderBooks.data.sort((a, b) => a.book.officeNumber.localeCompare(b.book.officeNumber));
        else {
          this.filteredOrderBooks.data=this.filteredOrderBooks.data.sort((a, b) => -a.book.officeNumber.localeCompare(b.book.officeNumber));
        }
      } else if (this.sort.active == 'OrderDate') {
        if (this.sort.direction == "desc")
          this.filteredOrderBooks.data=this.filteredOrderBooks.data.sort((a, b) => new Date(a.addedDate).getTime() - new Date(b.addedDate).getTime());
        else {
          this.filteredOrderBooks.data=this.filteredOrderBooks.data.sort((a, b) => new Date(b.addedDate).getTime() - new Date(a.addedDate).getTime());
        }
      }

    });

  }

  async getOrderBooksAsync(): Promise<any> {
    return await this.ordersService.getOrderBooksAsync(this.id)
      .then((data: OrderBook[]) => {
        this.orderBooks = data;
        console.log(data);
        this.assignCopy();
      });
  }

  onEditSubOrder(suborder:OrderBook) {

    this.router.navigate([`/orders/suborder/${suborder.orderBookId}/edit`],
      {
        queryParams: {
          componentNumber: this.selectedSubOrder.componentNumber,
          officeNumber: this.selectedSubOrder.book.officeNumber
        }
      });
  }

  onPrintRW(rw) {
    // ListType:
    // 0 - Piła
    // 1 - Plazma

    this.pdfService.generateBookRw(rw.book, this.order, true, rw.listType)
      .subscribe(response => {
        const blob = new Blob([response], { type: 'application/pdf' });
        var url = window.URL.createObjectURL(blob);
        var rw = window.open(url);

        rw.print();
      },
        (error) => { console.log(error) });
  }

  onPrintBomListClick(subOrder: OrderBook) {
    this.isPrinting = true;
    this.pdfService.generateBomList(subOrder, true, 0)
      .subscribe(response => {
        const blob = new Blob([response], { type: 'application/pdf' });
        var url = window.URL.createObjectURL(blob);
        var bookList = window.open(url);
        this.isPrinting = false;
        bookList.print();

      },
        (error) => {
          console.log(error);
          this.isPrinting = true;
        });
  }

  onPrintWaresListClick(subOrder: OrderBook) {
    this.isPrinting = true;
    this.pdfService.generateWareList(subOrder, false, 0)
      .subscribe(response => {
        console.log(response);
        const blob = new Blob([response], { type: 'application/pdf' });
        var url = window.URL.createObjectURL(blob);
        var bookList = window.open(url);
        this.isPrinting = false;
        bookList.print();
      },
        (error) => {
          console.log(error);
          this.isPrinting = false;
        })
  }

  prepareWareListClick(subOrder: OrderBook) {
    this.loadingWares = true;
    this.xmlService.getWareList(subOrder, 0)
      .subscribe(response => {
        this.wareList = response;
        this.wareList.forEach(item => {
          item.qCheckStatus = 0;
          item.totalToIssue = item.toIssue * item.wareLength;
        });
        this.loadingWares = false;
      });
  }

  // Pobiera listę rw danego typu z bazy danych asynchronicznie
  async prepareWareListAsyncClick(subOrder: OrderBook, listType: number): Promise<any> {
    // xmlListType
    // 0 - Lista materiałowa (surowce)
    // 1 - Lista części handlowych
    // 2 - CNC plazma
    // 3 - CNC kooperacja
    this.xmlListType = listType;
    this.loadingWares = true;
    this.isSummaryList = false;
    this.isMultipleSummaryList = false;
    // Pobierz listę rw z bazy danych
    return await this.xmlService.getWareListAsync(subOrder, listType).then((data: InProductionXML[]) => {
      if (data != null) {
        this.wareList = data;
        this.wareList.forEach(item => {
          item.qCheckStatus = 0;
          if (listType == 0) {
            item.totalToIssue = item.wareQuantity * item.wareLength;
            item.issued = item.issued * item.wareLength;
          }
          item.toIssue = 0;
        });
        this.loadingWares = false;
        this.rwDate = moment(subOrder.addedDate);
        // Wyzwól event zmiany daty
        this.onRWXMLDateChange();
        //console.log(this.wareList);
      }
    })
  }

  //prepare summary list
  async prepareWareListAsyncClickSMList(subOrder: OrderBook, listType: number): Promise<any>{
    this.isSummaryList = true;
    this.isMultipleSummaryList = false;
    // xmlListType
    // 0 - Lista materiałowa (surowce)
    // 1 - Lista części handlowych
    // 2 - CNC plazma
    // 3 - CNC kooperacja
    this.xmlListType = listType;
    this.loadingWares = true;
    // Pobierz listę rw z bazy danych
    return await this.xmlService.getWareListAsync(subOrder, listType).then((data: InProductionXML[]) => {
      if (data != null) {
        this.wareList = data;
        this.wareList.forEach(item => {
          item.qCheckStatus = 0;
          if (listType == 0) {
            item.totalToIssue = item.wareQuantity * item.wareLength;
            item.issued = item.issued * item.wareLength;
          }
          item.toIssue = 0;
        });
        this.loadingWares = false;
        this.MakeWareListWithNoRepeat();
       
      }
    })
  }

  async onExportMultipleSummaryList(listType: number) {
    this.isSummaryList = true;
    this.isMultipleSummaryList = true;
    // xmlListType
    // 0 - Lista materiałowa (surowce)
    // 1 - Lista części handlowych
    // 2 - CNC plazma
    // 3 - CNC kooperacja
    this.xmlListType = listType;
    this.loadingWares = true;
    console.log(this.selection.selected);
    this.orderBooksNumbers = [];
    this.componentNumbers = [];
    this.selection.selected.forEach(
      select => {
        this.orderBooksNumbers.push(select.number);
        this.componentNumbers.push(select.componentNumber);
      });
    // Pobierz listę rw z bazy danych
    return await this.xmlService.getWareSummaryListAsync(this.order.number, listType, this.orderBooksNumbers).then((data: InProductionXML[]) => {
      if (data != null) {
        this.wareList = data;
        this.wareList.forEach(item => {
          item.qCheckStatus = 0;
          item.toIssue = Math.round(item.totalToIssue * 100) / 100;
        });
        this.loadingWares = false;
      }
    })
  }


  MakeWareListWithNoRepeat() {
    var condenseList: InProductionXML[] =[];
    var index: number = 0;
    this.wareList.forEach((item) => {
      index = condenseList.findIndex(x => x.wareCode == item.wareCode);
      if (index == -1) {
        condenseList.push(item);
      }
      else {
        condenseList[index].wareQuantity += item.wareQuantity;
        condenseList[index].totalToIssue += item.totalToIssue;
        condenseList[index].issued += item.issued;
      }
    });
    this.wareList = condenseList;
    this.wareList.forEach(x => x.toIssue = Math.round(x.totalToIssue * 100) / 100); // get rid of infinte expansion
  }


  OnDateChange(event) {
    console.log(event.value);
  }

  // Event zmiany daty od ngbDatePicker
  onRWXMLDateChange() {
    this.rwDate = this.form.get('rwDate').value as moment.Moment;
    this.wareList.forEach(item => {
      item.qCheckStatus = 1,
        item.toIssue = 0
    });
    //[this.rwDate.format((), this.rwDate.month(), this.rwDate.day()].filter(Boolean).join("-");
    this.rwDateStr = this.rwDate.format("YYYY-MM-DD");
    //console.log(this.rwDateStr);
    var toIssue;
    for (let item of this.wareList) {
      if (item.totalToIssue == item.issued) {
        continue;
      }
      this.optimaService.getItemQty(item.wareCode, this.rwDateStr, this.selectedMag.mag_Symbol).subscribe(response => {
        var sum = this.wareList.filter(x => x.wareCode == item.wareCode).reduce((sum, current) => sum + current.toIssue, 0);
        item.qAvailable = response - sum;
        if (item.qAvailable >= item.totalToIssue) {
          toIssue = Math.round(item.totalToIssue * 100) / 100;
          item.toIssue = Math.round(toIssue * 100) / 100;
          item.qCheckStatus = 2;
        }
        if (item.qAvailable > 0 && item.qAvailable < item.totalToIssue) {
          item.toIssue = item.qAvailable;
          item.qCheckStatus = 3;
        }
        if (item.qAvailable == 0) {
          item.qCheckStatus = 4;
        }
      },
        (error) => {
          console.log(error);
        });
    }
  }
  onRWXMLMagChange() {
    this.onRWXMLDateChange();
  }

  onCloseExportToXMLClick() {
    this.cleanAfterXmlExport();
  }

  cleanAfterXmlExport() {
    this.wareList = [];
    this.rwDate = null;
  }

  onExportWareListClick() {
    //this.isExporting = true;
    var xmlType;
    if (this.xmlListType == 0) {
      xmlType = `Lista materiałowa`;
    }
    else if (this.xmlListType == 1) {
      xmlType = `Lista części handlowych`;
    }
    else if (this.xmlListType == 2) {
      xmlType = `Cięcie CNC (plazma)`;
    }
    else if (this.xmlListType == 3) {
      xmlType = `Cięcie CNC (kooperacja)`;
    }

    if (!this.isMultipleSummaryList) {
      this.xmlService.exportWareListToXML(this.wareList, this.selectedSubOrder, this.rwUpdateQuantities, this.rwDateStr, xmlType, this.selectedMag.mag_Symbol)
        .subscribe(
          (response) => {
            this.XMLCreate(response, xmlType);
          },
          (error) => { console.log(error); },
          () => { }
        );
    } else {

      this.xmlService.exportMultipleWareListToXML
        (this.wareList, this.order.number, this.rwUpdateQuantities,
          this.rwDateStr, xmlType, this.selectedMag.mag_Symbol, this.orderBooksNumbers, this.componentNumbers)
        .subscribe(
          (response) => {
            this.XMLCreate(response, xmlType);
          },
          (error) => { console.log(error); },
          () => { }
        );
    }

    //this.xmlService.generateWareList(subOrder, 0)
    //    .subscribe(response => {
    //        var xmlString = "<root></root>";
    //        var parser = new DOMParser();
    //        var xmlDoc = parser.parseFromString(response, "text/xml");              
    //        var serializer = new XMLSerializer();
    //        var xmlString = serializer.serializeToString(xmlDoc);
    //        const blob = new Blob([xmlString], { type: 'text' });
    //        //const url = window.URL.createObjectURL(blob);
    //        //window.open(url);
    //        FileSaver.saveAs(blob, `${subOrder.number}(${subOrder.componentNumber})_RW.xml`);
    //        this.isExporting = false;
    //    },
    //        (error) => {
    //            console.log(error);
    //            this.isExporting = false;
    //        })
  }

  XMLCreate(input:string,xmlType:string) {
    var parser = new DOMParser();
    var xmlDoc = parser.parseFromString(input, "text/xml");
    var serializer = new XMLSerializer();
    var xmlString = serializer.serializeToString(xmlDoc);
    const blob = new Blob([xmlString], { type: 'text' });
    if (!this.isMultipleSummaryList) {
      FileSaver.saveAs(blob,
        `${this.selectedSubOrder.number}(${this.selectedSubOrder.componentNumber})_RW - ${xmlType}.xml`);
      this.cleanAfterXmlExport();
    } else {
      FileSaver.saveAs(blob,
        `${this.order.number}_RW - ${xmlType}_zbiorcza.xml`);
      this.cleanAfterXmlExport();
    }

  }


  onPrintPurchaseListClick(subOrder: OrderBook) {
    this.isPrinting = true;
    this.pdfService.generateWareList(subOrder, false, 1)
      .subscribe(response => {
        const blob = new Blob([response], { type: 'application/pdf' });
        var url = window.URL.createObjectURL(blob);
        var bookList = window.open(url);
        this.isPrinting = false;
        bookList.print();
      },
        (error) => {
          console.log(error);
          this.isPrinting = false;
        })
  }

  onPrintPlazmaInternalCutListClick(subOrder: OrderBook) {
    this.isPrinting = true;
    this.pdfService.generateWareList(subOrder, false, 2)
      .subscribe(response => {
        const blob = new Blob([response], { type: 'application/pdf' });
        var url = window.URL.createObjectURL(blob);
        var bookList = window.open(url);
        this.isPrinting = false;
        bookList.print();
      },
        (error) => {
          console.log(error);
          this.isPrinting = false;
        })
  }

  onPrintPlazmaExternalCutListClick(subOrder: OrderBook) {
    this.isPrinting = true;
    this.pdfService.generateWareList(subOrder, false, 3)
      .subscribe(response => {
        const blob = new Blob([response], { type: 'application/pdf' });
        var url = window.URL.createObjectURL(blob);
        var bookList = window.open(url);
        this.isPrinting = false;
        bookList.print();
      },
        (error) => {
          console.log(error);
          this.isPrinting = false;
        })
  }

  onDownloadRW(book: Book, listType: number) {
    this.pdfService.generateBookRw(book, this.order, true, listType)
      .subscribe(response => {
        const blob = new Blob([response], { type: 'application/pdf' });
        FileSaver.saveAs(blob, `${book.componentNumber}_RW.pdf`);
      },
        (error) => { console.log(error) });
  }

  onDeleteSubOrderClick(subOrder: OrderBook) {
    this.subOrderToDelete = subOrder;
  }

  async onDeleteSubOrder() {
    this.deletingSubOrder = true;
    return await this.ordersService.deleteBook(this.subOrderToDelete)
      .then((response) => {
        if (response.ok) {
          const index = this.orderBooks.indexOf(this.subOrderToDelete);
          if (index > -1) {
            this.orderBooks.splice(index, 1);
            this.assignCopy();
          }
          this.toastr.info(`Podzlecenie zostało usunięte`, '');
          this.subOrderToDelete = null;
          this.deletingSubOrder = false;
        }
      }
        , error => { console.log(error) });
  }


  async onSubOrderClick(subOrder: OrderBook) {

    if (subOrder == this.selectedSubOrder) {
      return;
    }
    this.selectedSubOrder = subOrder;
    console.log(this.selectedSubOrder);
  }

  async onSubOrderExpand(subOrderIndex: number) {

    this.subOrderRWS = [];
    this.checkSubOrderRWSCompleted = false;

    if (this.filteredOrderBooks.data[subOrderIndex].isExpanded) {
      this.subOrderRWS.forEach((item) => {
        item.trN_Bufor = -1;
        item.trN_TrNID = -1;
      });
      for (var index in this.blurProgress) {
        this.blurProgress[index] = 'enabled';
      }
      this.isCheckingRWS = true;
      await this.checkSubOrderRWS();
    }
  }

  async checkSubOrderRWS() {
    var orderNumber = this.selectedSubOrder.order.number;
    var subOrderNumber = this.selectedSubOrder.number;
    var componentNumber = this.selectedSubOrder.componentNumber;
    return await this.optimaService.getRWSInfo(orderNumber, subOrderNumber, componentNumber).then(
      (data: OptimaRW[]) => {
        this.subOrderRWS = data;
        //console.log(this.subOrderRWS);
        this.isCheckingRWS = false;
        this.subOrderRWS.forEach((item, index) => {
          //if (item.trN_TrNID > 0) {
            this.blurProgress[index] = 'disabled';
          //}
        });
      }
    ).finally(() => {
      this.checkSubOrderRWSCompleted = true;
    });
  }

  showMessage(message: any) {
    console.log(message);
  }

  async getOptimaMags() {
    return await this.optimaService.getMags(false).then(
      (data: OptimaMag[]) => {
        this.optimaMags = data;
        //console.log(this.optimaMags);
      }).finally(() => {
        if (this.optimaMags != null) {
          this.selectedMag = this.optimaMags[0];
        }
      });
  }


  onBookAdding() {
    let dialogConfig: MatDialogConfig = {
      disableClose: true,
      minHeight: "150px",
      minWidth: "700px",
      data: {
        'order': this.order,
        'subOrders' : this.orderBooks
      },
      autoFocus: true,
      restoreFocus: false
    }

    let dialogRef = this.dialog.open(AddingExistingBookComponent, dialogConfig);
    dialogRef.afterClosed().subscribe((res: OrderBook[]) => {
      //console.log(res);
      this.newSubOrders = res;

      if (res.length != 0) {

        //open progressbar dialog
        let progressDialogRef = this.dialog.open(ProgressBarComponent, {
          minWidth: "400px",
          minHeight: "150px",
          disableClose: true,
          data: {
            title: `Zapis nowych komponentów do Zlecenia: ${this.order.number} - ${this.order.name}`,
            message: "Proszę czekać",
            canClose: false
          }
        });

        this.ordersService.copyOrderParts(this.order.orderId, res).subscribe({
          next: res => {
            //console.log(res);
            //assign new suborders and refresh table in view (*ngFor)
            progressDialogRef.close();
            //this.orderBooks = this.orderBooks.concat(this.newSubOrders);
            this.getOrderBooksAsync();
            this.assignCopy();
          },
          error: (err: HttpErrorResponse) => {
            console.log(err);
            progressDialogRef.close();
            this.newSubOrders = [];
            let dialogRef = this.dialog.open(ErrorMessageComponent, {
              minWidth: "400px",
              minHeight: "150px",
              disableClose: true,
              data: err
            });
          }
        });
      }

      //console.log(this.orderBooks);
    });

  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.filteredOrderBooks.data.length;
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

    this.filteredOrderBooks.data.forEach(row => this.selection.select(row));
  }

  /** The label for the checkbox on the passed row */
  checkboxLabel(row?: OrderBook): string {
    if (!row) {
      return `${this.isAllSelected() ? 'deselect' : 'select'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.position + 1}`;
  }


  onDetailExpand(suborder: OrderBook) {
    let newExpandedElementindex: number;
    let oldExpandedElementindex: number;
    this.selectedSubOrder = suborder;
    //console.log(this.selectedSubOrder);
    this.expandedElement = this.expandedElement === suborder ? null : suborder;
    //find index of old expanded element and new to expand
    oldExpandedElementindex = this.filteredOrderBooks.data.findIndex(ob => ob.isExpanded == true);
    newExpandedElementindex = this.filteredOrderBooks.data.findIndex(ob => ob.orderBookId == suborder.orderBookId);
    //if old is the same as new than toggle isExpanded value
    //else set new element as expandedand and reset old one
    if (newExpandedElementindex == oldExpandedElementindex) {
      this.filteredOrderBooks.data[newExpandedElementindex].isExpanded = !this.filteredOrderBooks.data[newExpandedElementindex].isExpanded;
    } else {
      this.filteredOrderBooks.data[newExpandedElementindex].isExpanded = true;
      if (oldExpandedElementindex != -1) {
        this.filteredOrderBooks.data[oldExpandedElementindex].isExpanded = false;
      }
    }

    this.onSubOrderExpand(newExpandedElementindex);
  }

  onPrintSubOrderListClick() {
    this.isPrinting = true;
    this.pdfService.generateSubOrderList(this.selection.selected).subscribe(response => {
      const blob = new Blob([response], { type: 'application/pdf' });
      var url = window.URL.createObjectURL(blob);
      var bookList = window.open(url);
      bookList.print();
      this.isPrinting = false;
    })
  }


}
