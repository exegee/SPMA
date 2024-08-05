import { animate, state, style, transition, trigger } from '@angular/animations';
import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
import { AfterContentInit, AfterViewInit, Component, ElementRef, OnInit, Pipe, PipeTransform, ViewChild } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef, MatSnackBar, MatTabChangeEvent } from '@angular/material';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { Params } from '@fortawesome/fontawesome-svg-core';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { debounceTime, finalize, switchMap, tap, delay, takeLast, take, last, first } from 'rxjs/operators';
import { ConfirmComponent } from '../../dialogs/confirm/confirm.component';
import { ErrorComponent } from '../../dialogs/error/error.component';
import { NewOrderComponent } from '../../dialogs/new-order/new-order.component';
import { ProgressComponent } from '../../dialogs/progress/progress.component';
import { Book } from '../../models/books/book.model';
import { BookComponent } from '../../models/books/bookcomponent.model';
import { ComponentItem } from '../../models/components/componentitem.model';
import { SortedComponents } from '../../models/components/sortedcomponents.model';
import { OptimaItem } from '../../models/optima/optimaitem.model';
import { OptimaMag } from '../../models/optima/optimamag.model';
import { Order } from '../../models/orders/order.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { PagedOrder } from '../../models/orders/pagedorder.model';
import { InProduction } from '../../models/production/inproduction.model';
import { InventorComponent } from '../../models/production/inventorcomponent.model';
import { SubOrderItem } from '../../models/suborders/suborderitem.model';
import { Ware } from '../../models/warehouse/ware.model';
import { BookService } from '../../services/books/book.service';
import { ComponentService } from '../../services/components/component.service';
import { PurchaseItemService } from '../../services/components/purchaseitem.service';
import { OptimaService } from '../../services/optima/optima.service';
import { OrdersService } from '../../services/orders/orders.service';
import { InProductionService } from '../../services/production/inproduction.service';
import { SubOrderService } from '../../services/suborder/suborder.service';
import { InProductionRW } from '../../models/production/inproductionrw.model';
import { BookComponentService } from '../../services/production/bookcomponent.service';
import { ProductionState } from '../../shared/Enums/productionstate';
import { SuborderEditConfirmComponent } from '../../dialogs/suborder-edit-confirm/suborder-edit-confirm.component';
import { SubOrderItemDiff } from '../../models/suborders/suborderitemdiff.model';
import { SuborderEditProgressComponent } from '../../dialogs/suborder-edit-progress/suborder-edit-progress.component';
import { SuborderEditErrorComponent } from '../../dialogs/suborder-edit-error/suborder-edit-error.component';
import { WarningMessageComponent } from '../../dialogs/warning-message/warning-message.component';

@Component({
  selector: 'app-suborder-edit',
  templateUrl: './suborder-edit.component.html',
  styleUrls: ['./suborder-edit.component.css'],
  animations: [
    trigger('divState', [
      state('enabled', style({
        opacity: 1
      })),
      state('disabled', style({
        opacity: 0
      })),
      transition('enabled => disabled', [animate(500)]),
      transition('disabled => enabled', [animate(500)]),
      transition('* => disabled', [animate(500), style({ opacity: 0 })]),
    ])
  ]
})
export class SuborderEditComponent implements OnInit, AfterContentInit, AfterViewInit {

  subOrderItemNumber: string = "Aktualizowanie części";
  progress: number = 0;
  progressString: string = "0";
  message: string = "";
  selectedFile: File = null;
  isBomImported: boolean = false;
  isBomSorted: boolean = false;
  sortedComponents: SortedComponents;
  //selectedSubOrderItem: SubOrderItem = new SubOrderItem();
  originalSubOrderItems: SubOrderItem[] = [];
  selectedSubOrderItem: SubOrderItem = new SubOrderItem;
  subOrderItemToEdit: SubOrderItem = new SubOrderItem();
  selectedSubOrderItemUpdate: Observable<boolean> = new Observable<boolean>();
  itemTest: Subject<SubOrderItem> = new Subject<SubOrderItem>();
  selectedPurchaseSubOrderItem: SubOrderItem = new SubOrderItem();
  observe: Observable<InventorComponent>;
  selectedIndex: number = 0;
  selectedPurchaseIndex: number = 0;
  selectedPurchaseStartIndex: number = 1;
  selectedPurchaseEndIndex: number = 0;
  allowNextComponent: boolean = true;
  allowPreviousComponent: boolean = false;
  allowNextPurchaseItem: boolean = true;
  allowPreviousPurchaseItem: boolean = false;
  allowAddWareToSubOrderItem: boolean = false;
  allowAddWareToPurchaseComponent: boolean = false;
  orders: PagedOrder;
  selectedOrder: Order = new Order();
  selectedOptimaItem: OptimaItem = new OptimaItem();
  selectedPurchaseOptimaItem: OptimaItem = new OptimaItem();
  componentItems: ComponentItem;
  wareToBeAdded: Ware = new Ware();
  purchaseItemWareToBeAdded: Ware = new Ware();
  searchOptimaWarehouseInputValue: string;
  searchPurchaseOptimaWarehouseInputValue: string;
  book: Book = new Book();
  orderBook: OrderBook = new OrderBook();
  componentsInProduction: InProduction[] = [];
  wareDivEnabled: boolean = true;
  bookAssembly: InventorComponent = new InventorComponent();
  bookComponents: BookComponent;
  bookExist: boolean = false;
  isPurchaseItemsPresent: boolean = false;
  wareToBeAddedLengthM: number;
  wareToBeAddedLengthMM: number;
  allStandardComponentsToProductionToggle: boolean = false;
  allPurchaseComponentsToProductionToggle: boolean = false;
  //0 - ware list
  //1 - purchase list
  tabSelected: number = 0;
  errorDialogConfig: MatDialogConfig = new MatDialogConfig();
  loadingComplete: boolean = false;

  replay = new ReplaySubject<number>(2);

  subOrderId: number;
  isLoadingSubOrder: boolean = true;
  subOrder: SubOrderItem[];
  subOrderInfo: OrderBook;

  toHighlight: string = '';
  isLoading: boolean;
  isLinear = false;
  fileInput = new FormControl(null, [Validators.required]);
  orderInput = new FormControl(null, [Validators.required]);
  bookComponentNumberInput = new FormControl(null, [Validators.required]);
  bookOfficeNumberInput = new FormControl(null, [Validators.required]);
  bookQuantityInput = new FormControl(null, [Validators.required]);
  bookNameInput = new FormControl(null, [Validators.required]);
  subOrderInput = new FormControl(null, [Validators.required]);
  componentQuantityInput = new FormControl(null, [
    Validators.required,
    Validators.pattern("^[0-9]*$"),
    Validators.minLength(1),
  ]);
  purchaseComponentQuantityInput = new FormControl(null, [
    Validators.required,
    Validators.pattern("^[0-9]*$"),
    Validators.minLength(1),
  ]);
  searchOptimaWarehouse: FormControl;
  searchPurchaseOptimaWarehouse: FormControl;
  filteredOptimaItems: Observable<OptimaItem[]>;
  filteredOrders: Observable<Order[]>;
  filteredPurchaseOptimaItems: Observable<OptimaItem[]>;
  navigationSubscription;
  plasmaToggleYesNo = new FormControl(null);
  plasmaToggleOrgin = new FormControl(null);
  plasmaToggleEntrustedMaterial = new FormControl(null);
  toProductionToggle = new FormControl(null);
  wareDivState = 'enabled';
  plasmaToggleYesNoState = 'enabled';
  displayedListColumns: string[] = ['number'];
  optimaMags: OptimaMag[] = [];
  selectedMag: OptimaMag;
  progressBarValue: number = 0;

  routeParamComponentNumber: string;
  routeParamOfficeNumber: string;
  subOrderBookComponent: BookComponent;
  loadingSubOrderSteps: boolean[] = [false, false, false, false];

  @ViewChild('acValue', { static: false }) searchOptimaWarehouseInputElRef: ElementRef;
  @ViewChild('acValuePurchase', { static: false }) searchOptimaWarehousePurchaseInputElRef: ElementRef;

  reserveInProductionButtonColor = "primary";

  constructor(private route: ActivatedRoute, private subOrderService: SubOrderService, private http: HttpClient, private ordersService: OrdersService, private optimaService: OptimaService,
    private componentService: ComponentService, public dialog: MatDialog, private bookService: BookService,
    private router: Router, private purchaseItemService: PurchaseItemService, private snackBar: MatSnackBar,
    private inProductionService: InProductionService, private toastr: ToastrService, private bookComponentService: BookComponentService) {

    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.initialize();
      }
    });
    this.errorDialogConfig = {
      disableClose: true,
      autoFocus: true
    }
  }
  ngAfterViewInit(): void {
    //throw new Error('Method not implemented.');
  }
  ngAfterContentInit(): void {
    //throw new Error('Method not implemented.');
  }


  selectionChange(event) {
    //console.log(event);
  }

  getFileErrorMessage() {
    //console.log(this.fileInput.hasError('required'));
    return this.fileInput.hasError('required') ? 'You must enter a value' : '';
  }

  onPurchaseItemProductionToggleClick(component) {
    //console.log(component);
  }

  componentQuantityChange() {
    //var value = this.componentQuantityInput.value;
    //this.selectedComponent.quantity = value;
  }

  onFileSelected(event) {
    this.selectedFile = <File>event.target.files[0];
    this.fileInput.setValue(this.selectedFile.name);
  }

  wareLengthMMChange() {
    this.wareToBeAddedLengthM = this.wareToBeAddedLengthMM / 1000;
    this.updateAddToSubOrderItemButton();
  }
  wareLengthMChange() {
    this.wareToBeAddedLengthMM = this.wareToBeAddedLengthM * 1000;
    this.updateAddToSubOrderItemButton();
  }


  // Extracts components from selected xls file
  //getBom() {

  //  this.http.get<InventorComponent[]>('api/ImportOrder', {
  //    params: {
  //      fileName: this.selectedFile.name
  //    }
  //  }).subscribe(
  //    (response) => {
  //      this.components = response;
  //      this.isBomSorted = true;
  //      this.selectedComponent = this.components[this.selectedIndex];
  //      if (this.components.filter(item => item.lastSourceType == 5).length > 0) {
  //        this.selectedPurchaseComponent = this.components.filter(item => item.lastSourceType == 5)[this.selectedPurchaseIndex];
  //        this.isPurchaseItemsPresent = true;
  //      }
  //      var bookName = this.selectedFile.name;
  //      var fileNameSplited = bookName.split(/\s*-\s*/);
  //      this.book = new Book(fileNameSplited[0], fileNameSplited[1]);
  //      //console.log(this.selectedPurchaseComponent);

  //      // Create new instance of BookComponents with basic info about imported book
  //      this.bookComponents = new BookComponent(this.book, this.components);
  //      this.bookComponentNumberInput.setValue(this.book.componentNumber);
  //      this.bookOfficeNumberInput.setValue(this.book.officeNumber);
  //      //console.log(this.book);

  //      // Check if book already exist in database
  //      this.bookService.checkIfBookExist(this.bookComponents)
  //        .subscribe((bookExist) => {
  //          //console.log("Check if book exist: " + bookExist);
  //          // If book exists get it from database
  //          if (bookExist) {
  //            this.bookService.getBook(this.book).then(
  //              (bookComponent: BookComponent) => {
  //                //console.log("Book exist!: " + bookComponent.book);
  //                this.book = bookComponent.book;
  //                this.bookComponents.book = bookComponent.book;
  //                this.bookOfficeNumberInput.setValue(this.book.officeNumber);
  //                //this.bookOfficeNumberInput.disable();
  //                this.bookNameInput.setValue(this.book.name);
  //                //this.bookNameInput.disable();
  //                //this.bookComponentNumberInput.disable();
  //                this.bookExist = true;
  //              },
  //              (error) => { console.log(error); }
  //            );
  //          }
  //          else {

  //          }

  //        });

  //      this.evaluateNextPurchaseItemIndex();
  //      this.findLastIndextOfPurchaseItems();
  //      this.selectedPurchaseStartIndex = this.selectedPurchaseIndex;
  //      this.loadComponentWare(this.selectedComponent);
  //      this.updateAddToComponentButton();
  //      this.updateAddToPurchaseComponentButton();
  //      this.getOptimaMags();
  //      this.componentQuantityInput.setValue(this.selectedComponent.quantity);
  //    });
  //}

  // Switch to next standard component


  onNextComponent() {
    // Get current index increment by 1
    var index = this.selectedIndex + 1;
    var isNextComponent = false;
    // Look for next standard component ( lastSourceType != 5)
    for (index; index < this.subOrder.length + 1; index++) {
      if (index + 1 < this.subOrder.length && this.subOrder[index].inProduction.sourceType != 5) {
        this.selectedIndex = index;
        this.selectedSubOrderItem = this.subOrder[this.selectedIndex];
        //TODO load suborder item ware ???
        //this.loadComponentWare(this.selectedComponent);
        var nextIndex = index + 1;
        // Check for next component
        for (nextIndex; nextIndex < this.subOrder.length + 1; nextIndex++) {
          if (nextIndex < this.subOrder.length && this.subOrder[nextIndex].inProduction.sourceType != 5) {
            isNextComponent = true;
            break;
          }
        }
        break;
      }
    };
    this.allowNextComponent = isNextComponent;
    this.allowPreviousComponent = true;
    this.updatePlasmaSliders();
    this.updateAddToSubOrderItemButton();

  }

  // Switch to previous standard component
  onPreviousComponent() {
    // Get current index decremented by 1
    var index = this.selectedIndex - 1;

    // Look for previous standard component ( lastSourceType != 5)
    for (index; index >= 0; index--) {
      if (index - 1 < this.subOrder.length && this.subOrder[index].inProduction.sourceType != 5) {
        this.selectedIndex = index;
        this.selectedSubOrderItem = this.subOrder[this.selectedIndex];
        //TOOD load suborder item ware ???
        //this.loadComponentWare(this.selectedComponent);
        break;
      }
    };
    if (this.selectedIndex - 1 < 0) {
      this.allowPreviousComponent = false;
    }
    else {
      this.allowPreviousComponent = true;
    }
    this.allowNextComponent = true;
    this.updatePlasmaSliders();
    this.updateAddToSubOrderItemButton();

  }

  onKeydownMMInput(event) {
    this.updateAddToSubOrderItemButton();
    if (event.key === "Enter" || event.key === "Tab") {
      if (this.allowAddWareToSubOrderItem) {
        this.wareLengthMMChange();
        this.onAddWareToSubOrderItem();
        this.onNextComponent();
        setTimeout(() => this.searchOptimaWarehouseInputElRef.nativeElement.focus());
      }
    }
  }

  onKeydownMInput(event) {
    this.updateAddToSubOrderItemButton();
    if (event.key === "Enter" || event.key === "Tab") {
      if (this.allowAddWareToSubOrderItem) {
        this.wareLengthMChange();
        this.onAddWareToSubOrderItem();
        this.onNextComponent();
        setTimeout(() => this.searchOptimaWarehouseInputElRef.nativeElement.focus());
      }
    }
  }


  // Components list click event
  onSelectedSubOrderItem(subOrderItem: SubOrderItem) {
    this.selectedSubOrderItem = subOrderItem;
    //this.getComponentsInProduction(subOrderItem.inProduction.component);

    this.selectedIndex = this.subOrder.indexOf(subOrderItem);
    if (this.selectedIndex > 0) {
      this.allowPreviousComponent = true;
    }
    else {
      this.allowPreviousComponent = false;
    }
    // Get current index increment by 1
    var nextIndex = this.selectedIndex + 1;
    var isNextComponent = false;
    // Check for next component and if it exist enable next component button
    for (nextIndex; nextIndex < this.subOrder.length + 1; nextIndex++) {
      if (nextIndex < this.subOrder.length && this.subOrder[nextIndex].inProduction.sourceType != 5) {
        isNextComponent = true;
        break;
      }
    }
    this.componentQuantityInput.setValue(this.selectedSubOrderItem.inProduction.plannedQty);
    this.allowNextComponent = isNextComponent;
    this.updatePlasmaSliders();

    this.wareToBeAdded = new Ware();
    this.wareToBeAddedLengthM = null;
    this.wareToBeAddedLengthMM = null;
    this.selectedOptimaItem = new OptimaItem();
    this.updateAddToSubOrderItemButton();
    this.searchOptimaWarehouse.setValue(new OptimaItem());

  }

  // Update addWareToComponentButton
  updateAddToSubOrderItemButton() {
    // Check if ware can be added to component
    if (this.selectedSubOrderItem.ware == null) {
      if (this.wareToBeAdded != null && this.wareToBeAddedLengthM != null) {
        this.allowAddWareToSubOrderItem = true;
      }
      else {
        this.allowAddWareToSubOrderItem = false;
      }
    }
    else {
      this.allowAddWareToSubOrderItem = false;
    }
  }

  // Update addWareToPurchaseComponentButton
  updateAddToPurchaseSubOrderItemButton() {
    // Check if ware can be added to component
    if (this.selectedPurchaseSubOrderItem.ware != null) {
      this.allowAddWareToPurchaseComponent = false;
    }
    else {
      this.allowAddWareToPurchaseComponent = true;
    }
  }
  onToProductionToggleClick() {
    this.onSubOrderItemUpdate(this.selectedSubOrderItem);
    this.updatePlasmaSliders();
  }
  // Modifies total SubOrderItem quantity value
  onComponentQtyChange(subOrderItem: SubOrderItem) {
    if (subOrderItem.inProduction.sourceType == 5) {
      this.selectedPurchaseSubOrderItem.inProduction.plannedQty = this.purchaseComponentQuantityInput.value;
    }
    else {
      this.selectedSubOrderItem.inProduction.plannedQty = this.componentQuantityInput.value;
    }

    var componentsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == subOrderItem.inProduction.component.number);
    var sum = componentsToUpdate.reduce((sum: number, item) => sum + item.inProduction.plannedQty, 0);
    componentsToUpdate.forEach((component) => {
      component.totalToIssue = sum;
    });
    //console.log("Qty changed");
    this.onSubOrderItemUpdate(subOrderItem);
  }

  // Toogles SubOrderItem toProduction value
  onToProductionToggleChange(subOrderItem: SubOrderItem, toggleCog: boolean) {
    if (toggleCog) {
      this.selectedSubOrderItem = subOrderItem;
    }
    if (!toggleCog) {
      this.selectedSubOrderItem.isInProduction = !this.selectedSubOrderItem.isInProduction;
    }
    //if (this.selectedSubOrderItem.isInProduction) {

    //  this.wareDivState = 'enabled';
    //  this.plasmaToggleYesNoState = 'enabled';
    //}
    //else {
    //  this.wareDivState = 'disabled';
    //  this.plasmaToggleYesNoState = 'disabled';
    //}
    //console.log("Production toggle changed");
    this.onSubOrderItemUpdate(this.selectedSubOrderItem);
    this.updatePlasmaSliders();
  }

  onToProductionPurchaseToggleChange(subOrderItem: SubOrderItem, toggleCog: boolean) {
    if (toggleCog) {
      this.selectedPurchaseSubOrderItem = subOrderItem;
    }
    if (!toggleCog) {
      this.selectedPurchaseSubOrderItem.isInProduction = !this.selectedPurchaseSubOrderItem.isInProduction;
    }
    this.onSubOrderItemUpdate(this.selectedPurchaseSubOrderItem);
  }

  // Updates sliders values based on lastSourceType property
  updatePlasmaSliders() {
    //console.log(this.selectedComponent);
    if (this.selectedSubOrderItem.isInProduction) {

      this.wareDivState = 'enabled';
      this.plasmaToggleYesNoState = 'enabled';
    }
    else {
      this.wareDivState = 'disabled';
      this.plasmaToggleYesNoState = 'disabled';
    }
    switch (this.selectedSubOrderItem.inProduction.sourceType) {
      case 0: {
        this.plasmaToggleYesNo.setValue(false);
        this.plasmaToggleOrgin.setValue(false);
        this.plasmaToggleEntrustedMaterial.setValue(false);
        this.onLastSourceTypeChange();
        break;
      }
      case 2: {
        this.plasmaToggleYesNo.setValue(true);
        this.plasmaToggleOrgin.setValue(false);
        this.plasmaToggleEntrustedMaterial.setValue(false);
        this.onLastSourceTypeChange();
        break;
      }
      case 3: {
        this.plasmaToggleYesNo.setValue(true);
        this.plasmaToggleOrgin.setValue(true);
        this.plasmaToggleEntrustedMaterial.setValue(false);
        this.onLastSourceTypeChange();
        break;
      }
      case 4: {
        this.plasmaToggleYesNo.setValue(true);
        this.plasmaToggleOrgin.setValue(true);
        this.plasmaToggleEntrustedMaterial.setValue(true);
        this.onLastSourceTypeChange();
        break;
      }
    }
  }

  // Modifies lastSourceType property according to sliders value
  onLastSourceTypeChange() {
    // Shorten variables
    var plasma = this.plasmaToggleYesNo;
    var orgin = this.plasmaToggleOrgin;
    var material = this.plasmaToggleEntrustedMaterial;

    // Old sourceType value - before check
    var oldSourceType = this.selectedSubOrderItem.inProduction.sourceType;

    // Disable sub sliders
    if (!plasma.value && (orgin.value || material.value)) {
      orgin.setValue(false);
      material.setValue(false);
    }
    else if (plasma.value && !orgin.value) {
      material.setValue(false);
    }
    // Assign new value according to slider values
    if (plasma.value && !orgin.value && !material.value) {
      var componentsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);
      componentsToUpdate.forEach(item => {
        item.inProduction.sourceType = 2;
      })
      this.wareDivState = 'enabled';
      this.wareDivEnabled = true;

    }
    else if (plasma.value && orgin.value && !material.value) {
      var componentsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);
      componentsToUpdate.forEach(item => {
        item.inProduction.sourceType = 3;
      })
      this.wareDivState = 'disabled';
      this.wareDivEnabled = false;
    }
    else if (plasma.value && orgin.value && material.value) {
      var componentsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);
      componentsToUpdate.forEach(item => {
        item.inProduction.sourceType = 4;
      })
      this.wareDivState = 'enabled';
      this.wareDivEnabled = true;
    }
    else {
      var componentsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);
      componentsToUpdate.forEach(item => {
        item.inProduction.sourceType = 0;
      })
      this.wareDivState = 'enabled';
      this.wareDivEnabled = true;
    }
    // console.log("OldSourceType: " + oldSourceType + " ==== Current SourceType: " + this.selectedSubOrderItem.inProduction.sourceType)
    // Call onSubOrderItemUpdate only if sourceType is different than before check
    if (oldSourceType != this.selectedSubOrderItem.inProduction.sourceType) {
      //console.log("LastSource Change");
      this.onSubOrderItemUpdate(this.selectedSubOrderItem);
    }
    // Update component in database
    //this.componentService.updateComponent(this.selectedComponent).subscribe();
  }





  // Adds Optima ware to SubOrderItem locally
  onAddWareToSubOrderItem() {
    if (this.selectedOptimaItem.twr_Kod == null && (this.wareToBeAddedLengthM != null || this.wareToBeAddedLengthMM != null))
      return;
    var subOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);

    subOrderItemsToUpdate.forEach(item => {
      this.wareToBeAdded.length = this.wareToBeAddedLengthM;
      item.ware = this.wareToBeAdded;
      item.wareLength = this.wareToBeAdded.length;
      item.wareQuantity = 1;
      item.wareUnit = this.wareToBeAdded.unit;
    });

    this.wareToBeAdded = new Ware();
    this.wareToBeAddedLengthM = null;
    this.wareToBeAddedLengthMM = null;
    this.selectedOptimaItem = new OptimaItem();
    this.searchOptimaWarehouse.setValue(new OptimaItem());
    this.filteredOptimaItems = null;
    this.onSubOrderItemUpdate(this.selectedSubOrderItem);
    //console.log("Ware changed");
    this.updateAddToSubOrderItemButton();
  }

  // Deletes locally Optima ware from SubOrderItem
  onDeleteWareFromComponent(subOrderItem: SubOrderItem) {
    if (subOrderItem.inProduction.sourceType == 5) {
      var purchaseSubOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedPurchaseSubOrderItem.inProduction.component.number);
      purchaseSubOrderItemsToUpdate.forEach(item => {
        item.inProduction.ware = null;
        item.ware = null;
        item.wareLength = null;
        item.wareQuantity = null;
        item.wareUnit = null;
      })
      this.allowAddWareToPurchaseComponent = true;
      this.onSubOrderItemUpdate(this.selectedPurchaseSubOrderItem);
      this.updateAddToPurchaseSubOrderItemButton();
    }
    else {
      var subOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);
      subOrderItemsToUpdate.forEach(item => {
        item.inProduction.ware = null;
        item.ware = null;
        item.wareLength = null;
        item.wareQuantity = null;
        item.wareUnit = null;
      });
      //console.log("Delete ware changed");
      this.allowAddWareToSubOrderItem = true;
      this.onSubOrderItemUpdate(this.selectedSubOrderItem);
      this.updateAddToSubOrderItemButton();
    }
  }

  //TODO DELETE THIS ???!!! - ware vale will be stored locally and updated in database
  // Gets component Optima ware
  async loadSubOrderItemWare(subOrderItem: SubOrderItem): Promise<any> {
    //return await this.componentService.getComponentWare2(component).then((data: InventorComponent) => {
    //  if (data.ware == null) {
    //    if (component.lastSourceType === 5) {
    //      this.updateAddToPurchaseComponentButton();
    //    }
    //    else {
    //      this.updateAddToComponentButton();
    //    }
    //    return;
    //  }
    //  if (component.lastSourceType === 5) {
    //    this.selectedPurchaseComponent.ware = data.ware;
    //    this.selectedPurchaseComponent.wareLength = data.wareLength;
    //    this.selectedPurchaseComponent.wareQuantity = data.wareQuantity;
    //    this.selectedPurchaseComponent.wareUnit = data.wareUnit;
    //    this.selectedPurchaseComponent.componentId = data.componentId;
    //    this.updateAddToPurchaseComponentButton();
    //  }
    //  else {
    //    this.selectedComponent.ware = data.ware;
    //    this.selectedComponent.wareLength = data.wareLength;
    //    this.selectedComponent.wareQuantity = data.wareQuantity;
    //    this.selectedComponent.wareUnit = data.wareUnit;
    //    this.selectedComponent.componentId = data.componentId;
    //    this.updateAddToComponentButton();
    //  }
    //});
  }

  // Switch to next purchase component
  onNextPurchaseItem() {
    // Get current purchase item index
    var index = this.selectedPurchaseIndex;

    // Look for next purchase component ( lastSourceType == 5)
    this.evaluateNextPurchaseItemIndex();

    if (index == this.selectedPurchaseIndex || this.selectedPurchaseIndex == this.selectedPurchaseEndIndex) {
      this.allowNextPurchaseItem = false;
    }
    else {
      this.allowNextPurchaseItem = true;
      this.allowPreviousPurchaseItem = true;
    }
    //TODO do we need this ???
    //this.loadComponentWare(this.selectedPurchaseComponent);
    this.updateAddToPurchaseSubOrderItemButton();
  }

  // Switch to previous purchase component
  onPreviousPurchaseItem() {
    // Get current purchase item index
    var index = this.selectedPurchaseIndex;

    // Look for previous purchase component ( lastSourceType == 5)
    this.evaluatePreviousPurchaseItemIndex();

    if (index == this.selectedPurchaseIndex || this.selectedPurchaseIndex == this.selectedPurchaseStartIndex) {
      this.allowPreviousPurchaseItem = false;
    }
    else {
      this.allowPreviousPurchaseItem = true;
      this.allowNextPurchaseItem = true;
    }
    //TODO do we need this ???
    //this.loadComponentWare(this.selectedPurchaseComponent);
    this.updateAddToPurchaseSubOrderItemButton();
  }

  // Evaluates next purchase item index
  evaluateNextPurchaseItemIndex() {
    var index = this.selectedPurchaseIndex + 1;
    for (index; index < this.subOrder.length + 1; index++) {
      if (index < this.subOrder.length && this.subOrder[index].inProduction.sourceType == 5) {
        this.selectedPurchaseSubOrderItem = this.subOrder[index];
        this.selectedPurchaseIndex = index;
        break;
      }
    }
  }

  // Evaluates previous purchase item index
  evaluatePreviousPurchaseItemIndex() {
    var index = this.selectedPurchaseIndex - 1;
    for (index; index > 0; index--) {
      if (index > 0 && this.subOrder[index].inProduction.sourceType == 5) {
        this.selectedPurchaseSubOrderItem = this.subOrder[index];
        this.selectedPurchaseIndex = index;
        break;
      }
    }
  }

  // Finds last index of purchase components list
  findLastIndextOfPurchaseItems() {
    for (var index = 0; index < this.subOrder.length; index++) {
      if (this.subOrder[index].inProduction.sourceType == 5) {
        this.selectedPurchaseEndIndex = index;
      }
    }
  }

  // Purchase components list click event
  onSelectedPurchaseSubOrderItem(subOrderItem: SubOrderItem, index: any) {
    this.selectedPurchaseSubOrderItem = subOrderItem;
    this.selectedPurchaseIndex = index;

    // Disable or enable previous, next buttons
    if (this.selectedPurchaseIndex > this.selectedPurchaseStartIndex) {
      this.allowPreviousPurchaseItem = true;
    }
    else {
      this.allowPreviousPurchaseItem = false;
    }
    if (this.selectedPurchaseIndex < this.selectedPurchaseEndIndex) {
      this.allowNextPurchaseItem = true;
    }
    else {
      this.allowNextPurchaseItem = false;
    }
    this.purchaseComponentQuantityInput.setValue(this.selectedPurchaseSubOrderItem.inProduction.plannedQty);
    this.wareToBeAdded = new Ware();
    this.wareToBeAddedLengthM = null;
    this.wareToBeAddedLengthMM = null;
    this.selectedOptimaItem = new OptimaItem();
    this.updateAddToPurchaseSubOrderItemButton();
    this.searchPurchaseOptimaWarehouse.setValue(new OptimaItem());
  }

  // Adds Optima ware to purchase component
  onAddWareToPurchaseSubOrderItem() {

    if (this.selectedPurchaseOptimaItem.twr_Kod == null)
      return;
    var purchaseSubOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedPurchaseSubOrderItem.inProduction.component.number);

    purchaseSubOrderItemsToUpdate.forEach(item => {
      item.ware = this.purchaseItemWareToBeAdded;
      item.wareLength = this.purchaseItemWareToBeAdded.length;
      item.wareQuantity = this.purchaseItemWareToBeAdded.quantity;
      item.wareUnit = this.purchaseItemWareToBeAdded.unit;
    });

    this.purchaseItemWareToBeAdded = new Ware();
    this.selectedPurchaseOptimaItem = new OptimaItem();
    this.searchPurchaseOptimaWarehouse.setValue(new OptimaItem());
    this.filteredPurchaseOptimaItems = null;
    this.onSubOrderItemUpdate(this.selectedPurchaseSubOrderItem);
    this.updateAddToPurchaseSubOrderItemButton();
  }

  // Get all active orders
  getOrders() {
    this.ordersService.getOrders('', 'asc', 0, 0, 9999, '','','')
      .subscribe(response => {
        this.orders = response;
        this.selectedOrder = new Order();
      });

  }

  // Get route parameters
  getRouteParams() {
    this.route.params
      .subscribe(
        (params: Params) => {
          this.subOrderId = +params['id'];

        });
    this.route.queryParams.subscribe(params => {
      this.routeParamComponentNumber = params['componentNumber'];
      this.routeParamOfficeNumber = params['officeNumber'];
    });
  }


  ngOnInit() {
    // Get suborder id from route params
    //console.log('Started...');
    this.getRouteParams();

    // Get book
    var getBookInfo = async (componentNumber: string, officeNumber: string) => {
      //console.log("Download book...");
      await this.bookService.getBook(componentNumber, officeNumber).then((book: BookComponent) => {
        this.subOrderBookComponent = book;
        //console.log("Book downloaded!");
      });
    }

    // Get data from BookComponents table - method download relationship between current component and book
    var getBookComponentInfo = (item: SubOrderItem, bookid: number) => {
      return new Promise((resolve, reject) => {
        resolve(this.bookComponentService.getBookComponentAsync(item, bookid))
      })
    }

    // Get suborder
    var getSubOrderInfo = async () => {
      //console.log('Downloading suborder additional info');
      await this.subOrderService.getSubOrderByIdInfo(this.subOrderId).then((data: OrderBook) => {
        // console.log('Suborder info downloaded.');
        //console.log(data);
        this.subOrderInfo = data;
      })
    }

    // Get all data in suborder table based on parsed suborder id
    var getSubOrder = async () => {
      //console.log('Downloading suborder...');
      await this.subOrderService.getSubOrderByIdAsync(this.subOrderId).then((data: SubOrderItem[]) => {
        //console.log('Suborder downloaded.');
        //console.log(data);
        this.subOrder = data;
      },
        (error) => { console.log(error) })
    }

    // Method updates progress bar - NOT USED
    //var updateProgressBar = (subOrderLength, i) => {
    //  return new Promise((resolve, reject) => {
    //    this.progressBarValue = (i / (subOrderLength - 1)) * 100;
    //    resolve()
    //  })
    //}

    // Gets all the necessary info for each subOrder item
    var getSubOrderAdditionalInfo = async () => {
      var subOrderLength = this.subOrder.length;
      //console.log("Downloading data...");

      for (let i = 0; i < subOrderLength; i++) {

        // Get current subOrder item
        var subOrderItem = this.subOrder[i];

        // Get bookComponent relationship for current subOrderItem
        var bookComponentItem = await getBookComponentInfo(subOrderItem, this.subOrderBookComponent.bookId) as BookComponent;
        //console.log(subOrderItem);
        //console.log(bookComponentItem);
        //console.log("-------------");
        // Updates additional properties
        subOrderItem.orderTree = bookComponentItem.order;
        subOrderItem.levelTree = bookComponentItem.level;

        this.subOrder[i].inProduction.productionStateId >= ProductionState["Awaiting"] ? this.subOrder[i].isInProduction = true : this.subOrder[i].isInProduction = false;

        // Update progress bar
        this.progressBarValue = (i / (subOrderLength - 1)) * 100;
        // Update progress bar text
        this.subOrderItemNumber = `Aktualizowanie części ${subOrderItem.inProduction.component.number}.`;
        //console.log(i + "/" + (subOrderLength - 1) + " ----------> " + (i / (subOrderLength - 1)) * 100 + "%");
        //delay(100);
      }
    }




    // Combine all methods and retrive data
    getBookInfo(this.routeParamComponentNumber, this.routeParamOfficeNumber).then(() => {
      getSubOrderInfo().then(() => {
        this.loadingSubOrderSteps[0] = true;
        getSubOrder().then(() => {
          this.loadingSubOrderSteps[1] = true;
          getSubOrderAdditionalInfo().then(() => {
            this.loadingSubOrderSteps[2] = true;
            this.subOrderItemNumber = "Aktualizowanie części.";
            // Update suborder input fields
            this.bookQuantityInput.setValue(this.subOrderInfo.plannedQty);
            this.bookNameInput.setValue(this.subOrderInfo.book.name);
            this.bookComponentNumberInput.setValue(this.subOrderBookComponent.component.number);
            this.subOrderInput.setValue(this.subOrderInfo.number);
            this.bookOfficeNumberInput.setValue(this.subOrderInfo.book.officeNumber);
            this.subOrder = this.subOrder.sort((a, b) => 0 - (a.orderTree > b.orderTree ? -1 : 1)).slice(1);

          },
            (rejected) => {
              const errorDialogConfig = new MatDialogConfig();
              errorDialogConfig.disableClose = true;
              errorDialogConfig.autoFocus = true;
              errorDialogConfig.data = rejected;
              errorDialogConfig.width = "28vw";

              const errorDialogRef = this.dialog.open(SuborderEditErrorComponent, errorDialogConfig);
              errorDialogRef.afterClosed().subscribe(() => {
                this.navigateToOrderDetails();
              });
            }).finally(() => {

            this.selectedSubOrderItem = this.subOrder[this.selectedIndex];
            this.subOrderItemToEdit = this.subOrder[this.selectedIndex];
            this.componentQuantityInput.setValue(this.selectedSubOrderItem.inProduction.plannedQty);
            if (this.subOrder.filter(item => item.inProduction.sourceType == 5).length > 0) {
              this.selectedPurchaseSubOrderItem = this.subOrder.filter(item => item.inProduction.sourceType == 5)[this.selectedPurchaseIndex];
              this.isPurchaseItemsPresent = true;
            }
            // Copy array
            this.originalSubOrderItems = JSON.parse(JSON.stringify(this.subOrder));
            //this.subOrder.forEach(item => {
            //  this.originalSubOrderItems.push(Object.assign({}, item));
            //});
            //console.log(this.originalSubOrderItems);
            this.updateAddToPurchaseSubOrderItemButton();
            this.updateAddToSubOrderItemButton();
            this.loadingSubOrderSteps[3] = true;
              setTimeout(() => {
                if (this.loadingSubOrderSteps.every(x => x == true)) {
                  this.isLoadingSubOrder = false;
                }
            }, 2000)

          })
        })
      })
    })

    this.searchOptimaWarehouse = new FormControl;
    this.searchPurchaseOptimaWarehouse = new FormControl;

    this.searchOptimaWarehouse.valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isLoading = true
          }
        }
        ),
        switchMap(value => value.length >= 3 ? this.optimaService.searchItem(value, this.selectedMag).pipe(finalize(() => {
          this.isLoading = false

        })) : []),
        finalize(() => {
          this.isLoading = false
        })
      ).subscribe(items => {
        this.filteredOptimaItems = items;
      }, () => { });

    this.searchPurchaseOptimaWarehouse.valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isLoading = true
          }
        }
        ),
        switchMap(value => value.length >= 3 ? this.optimaService.searchItem(value, this.selectedMag).pipe(finalize(() => {
          this.isLoading = false;


        })) : []),
        finalize(() => {
          this.isLoading = false
        })
      ).subscribe(items => {
        this.filteredPurchaseOptimaItems = items;
      }, () => { });
    this.loadingComplete = true;
  }


  //Method saves suborder item or items (if same item exist in list) in database
  onSaveSubOrderItem(subOrderItemType: number) {
    if (subOrderItemType == 0) {
      var subOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);
    }
    else if (subOrderItemType == 5) {
      var subOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedPurchaseSubOrderItem.inProduction.component.number);
    }
    else {
      return;
    }
    subOrderItemsToUpdate.forEach(item => {
      item.isUpdating = true;
    });
    this.subOrderService.updateSubOrderItem(subOrderItemsToUpdate).then(() => {
      subOrderItemsToUpdate.forEach(item => {
        item.isUpdating = false;
      });
    }).finally(() => {
      if (subOrderItemType == 0) {
        this.onSubOrderItemUpdateComplete(this.selectedSubOrderItem);
      }
      else {
        this.onSubOrderItemUpdateComplete(this.selectedPurchaseSubOrderItem);
      }
    });
  }

  // Get Optima warehouses
  async getOptimaMags() {
    return await this.optimaService.getMags(true).then(
      (data: OptimaMag[]) => {
        this.optimaMags = data;
      }).finally(() => {
        if (this.optimaMags != null) {
          this.selectedMag = this.optimaMags[1];
        }
      });
  }

  // Optima standard component display
  searchOptimaWarehouseDisplay(item: OptimaItem) {
    if (item) { return item.twr_Kod; }
  }

  // Order standard component display
  searchOrderDisplay(item: Order) {
    if (item) { return item.number; }
  }

  // Optima purchase component display
  searchPurchaseOptimaWarehouseDisplay(item: OptimaItem) {
    if (item) { return item.twr_Kod; }
  }

  // Optima standard component search field event
  onSelectedOptimaItem(event) {
    this.selectedOptimaItem = event.option.value;
    this.wareToBeAdded.code = this.selectedOptimaItem.twr_Kod;
    this.wareToBeAdded.unit = this.selectedOptimaItem.twr_JM;
    this.wareToBeAdded.name = this.selectedOptimaItem.twr_Nazwa;
    this.wareToBeAdded.mag_Nazwa = this.selectedOptimaItem.mag_Nazwa;
    this.wareToBeAdded.mag_Symbol = this.selectedOptimaItem.mag_Symbol;
    this.wareToBeAdded.twG_Kod = this.selectedOptimaItem.twG_Kod;
    this.wareToBeAdded.twG_Nazwa = this.selectedOptimaItem.twG_Nazwa;
    this.wareToBeAdded.date = new Date();

  }

  // Optima purchase component search field event
  onPurchaseSelectedOptimaItem(event) {
    this.selectedPurchaseOptimaItem = event.option.value;
    this.purchaseItemWareToBeAdded.code = this.selectedPurchaseOptimaItem.twr_Kod;
    this.purchaseItemWareToBeAdded.unit = this.selectedPurchaseOptimaItem.twr_JM;
    this.purchaseItemWareToBeAdded.name = this.selectedPurchaseOptimaItem.twr_Nazwa;
    this.purchaseItemWareToBeAdded.length = 0;
    this.purchaseItemWareToBeAdded.quantity = 0;
    this.purchaseItemWareToBeAdded.date = new Date();
  }

  // Method sets suborder item flags when updating database is started
  onSubOrderItemUpdate(subOrderItem: SubOrderItem) {
    var subOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == subOrderItem.inProduction.component.number);
    subOrderItemsToUpdate.forEach((item) => {
      item.updatedLocally = true;
      item.updatedInDatabase = false;
    });
  }
  // Method sets suborder item flags when updating database is finished
  onSubOrderItemUpdateComplete(subOrderItem: SubOrderItem) {
    var subOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == subOrderItem.inProduction.component.number);
    subOrderItemsToUpdate.forEach((item) => {
      item.updatedLocally = false;
      item.updatedInDatabase = true;
    });
  }

  onTabSelectionChange(event: MatTabChangeEvent) {
    this.tabSelected = event.index;
    if (event.index == 0) {
      this.toProductionToggle.markAsTouched();
      setTimeout(() => this.searchOptimaWarehouseInputElRef.nativeElement.focus(), 1000);
    }
    if (event.index == 1) {
      setTimeout(() => this.searchOptimaWarehousePurchaseInputElRef.nativeElement.focus(), 1000);
    }
  }

  // On Component destroy
  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  onBookQtyChange() {
    this.subOrder.forEach(item => {
      item.bookQty = this.bookQuantityInput.value;
      item.updatedLocally = true;
      item.isBookQtyChanged = true;
    })
  }

  // Snack bar message window
  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 10000,
    });
  }

  onCopyToPurchaseListToggleChange(event) {
    console.log(event);
    var isChecked = event.checked;
    var subOrderItemsToUpdate = this.subOrder.filter(x => x.inProduction.component.number == this.selectedSubOrderItem.inProduction.component.number);
    subOrderItemsToUpdate.forEach(item => {
      item.isAdditionallyPurchasable = isChecked;
    })
      ;this.selectedSubOrderItem.isAdditionallyPurchasable = isChecked;
    this.onSubOrderItemUpdate(this.selectedSubOrderItem);
  }

  toogleAllStandardComponentsToProduction() {
    this.subOrder.filter(comp => comp.inProduction.sourceType != 5)
      .forEach(item => {
        item.isInProduction = this.allStandardComponentsToProductionToggle,
          item.updatedLocally = true
      });

    this.allStandardComponentsToProductionToggle = !this.allStandardComponentsToProductionToggle;
  }

  toogleAllPurchaseItemsToProduction() {
    this.subOrder.filter(comp => comp.inProduction.sourceType == 5)
      .forEach(item => {
        item.isInProduction = this.allPurchaseComponentsToProductionToggle,
          item.updatedLocally = true
      });
    this.allPurchaseComponentsToProductionToggle = !this.allPurchaseComponentsToProductionToggle;
  }



  onKeydownACValuePurchase(event) {
    if (this.tabSelected == 1) {
      if (event.key === "Enter" || event.key === "Tab") {
        this.onAddWareToPurchaseSubOrderItem();
        this.onNextPurchaseItem();
        setTimeout(() => this.searchOptimaWarehousePurchaseInputElRef.nativeElement.focus());
      }
    }
  }


  // Initialise after adding the book
  initialize() {
    this.bookExist = false;
    this.progress = 0;
    this.progressString = "0";
    this.message = "";
    this.selectedFile = null;
    this.isPurchaseItemsPresent = false;
    this.isBomImported = false;
    this.isBomSorted = false;
    this.sortedComponents = new SortedComponents([], [], []);
    this.selectedSubOrderItem = new SubOrderItem();
    this.selectedPurchaseSubOrderItem = new SubOrderItem();
    this.observe = new Observable<InventorComponent>();
    this.selectedIndex = 0;
    this.selectedPurchaseIndex = 0;
    this.allowNextComponent = true;
    this.allowPreviousComponent = false;
    this.allowNextPurchaseItem = true;
    this.allowPreviousPurchaseItem = false;
    this.selectedOptimaItem = new OptimaItem();
    this.selectedPurchaseOptimaItem = new OptimaItem();
    this.wareToBeAdded = new Ware();
    this.purchaseItemWareToBeAdded = new Ware();
    this.book = new Book();
    this.selectedOrder = new Order();
    this.orderBook = new OrderBook();
    this.fileInput.setValue(null);
    this.fileInput.updateValueAndValidity();
    this.subOrder = [];
    this.bookAssembly = new InventorComponent();
    this.getOptimaMags();
  }

  getComponentsInProduction(component: InventorComponent) {
    this.componentService.getInProductionComponents(component).subscribe(
      (data) => {
        this.componentsInProduction = data;
        console.log(data);
      },
      (error) => { console.log(error) },
      () => { }
    );
  }

  onReserveInProduction(inProduction: InProduction) {
    //TODO Reserve inproduction ???
    //if (this.selectedComponent.inProductionReservation == null) {
    //  this.selectedComponent.inProductionReservation = inProduction;
    //  this.reserveInProductionButtonColor = "warn";
    //}
    //else {
    //  this.selectedComponent.inProductionReservation = null;
    //  this.reserveInProductionButtonColor = "primary";
    //}

  }

  onFinishModifyingSubOrder() {
    console.log("All element fine?: ", this.AreAllItemFine())
    //check if all elements are configure properly
    if (!this.AreAllItemFine()) {
      let warningDialogConfig: MatDialogConfig = {
        disableClose: true,
        minHeight: "150px",
        maxWidth: "500px",
        data: `Nie wszystkie elementy oznaczone jako "do produkcji" mają przypisany materiał. Czy chcesz kontynuować?`
      }

      let warningDialogRef = this.dialog.open(WarningMessageComponent, warningDialogConfig);

      warningDialogRef.afterClosed().subscribe((res: Boolean) => {
        if (res) {
          this.openConfirmDialog();
        }
      });
    } else {
      this.openConfirmDialog();
    }


    
  }

  navigateToOrderDetails() {
    this.router.navigate([`/orders/${this.subOrderInfo.orderId}`], { relativeTo: this.route });
  }

  checkSubOrderItemsDifferences(modifiedSubOrderItems: SubOrderItem[], originalSubOrderItems: SubOrderItem[]): SubOrderItemDiff[] {
    var subOrderItemsDiff: SubOrderItemDiff[] = [];

    modifiedSubOrderItems.forEach(newItem => {
      //console.log(originalSubOrderItems);
      var oldItem = originalSubOrderItems.filter(oldItem => oldItem.inProduction.inProductionId == newItem.inProduction.inProductionId)[0];
      if (oldItem != null) {
        var itemDiff: SubOrderItemDiff = new SubOrderItemDiff;
        // Check if isInProduction has changed
        if (oldItem.isInProduction != newItem.isInProduction) {
          itemDiff.isInProductionCheck = true;
          itemDiff.isInProductionOld = oldItem.isInProduction;
          itemDiff.isInProductionNew = newItem.isInProduction;
        }
        // Quantities check
        if (oldItem.inProduction.plannedQty != newItem.inProduction.plannedQty) {
          itemDiff.quantitiesCheck = true;
          itemDiff.quantitiesOld = oldItem.inProduction.plannedQty;
          itemDiff.quantitiesNew = newItem.inProduction.plannedQty;
        }
        if (newItem.bookQty != this.subOrderInfo.plannedQty) {
          itemDiff.bookMultiplierCheck = true;
          itemDiff.bookMultiplierOld = this.subOrderInfo.plannedQty;
          itemDiff.bookMultiplierNew = newItem.bookQty;
        }
        // SourceType check
        if (oldItem.inProduction.sourceType != newItem.inProduction.sourceType) {
          itemDiff.sourceTypeCheck = true;
          itemDiff.sourceTypeOld = oldItem.inProduction.sourceType;
          itemDiff.sourceTypeNew = newItem.inProduction.sourceType;
        }
        // isAdditionallyPurchasableCheck check
        if (oldItem.isAdditionallyPurchasable != newItem.isAdditionallyPurchasable) {
          itemDiff.isAdditionallyPurchasableCheck = true;
          itemDiff.isAdditionallyPurchasableOld = oldItem.isAdditionallyPurchasable == 1 ? true : false;
          itemDiff.isAdditionallyPurchasableNew = newItem.isAdditionallyPurchasable == 1 ? true : false;
        }
        // wareCode check
        if (oldItem.ware != null && newItem.ware == null) {
          itemDiff.wareCodeCheck = true;
          itemDiff.wareCodeOld = oldItem.ware.code;
          itemDiff.wareCodeNew = '';
        }
        else if (oldItem.ware == null && newItem.ware != null) {
          itemDiff.wareCodeCheck = true;
          itemDiff.wareCodeOld = '';
          itemDiff.wareCodeNew = newItem.ware.code;
        }
        else if (oldItem.ware != null && newItem.ware != null) {
          if (oldItem.ware.code != newItem.ware.code) {
            itemDiff.wareCodeCheck = true;
            itemDiff.wareCodeOld = oldItem.ware.code;
            itemDiff.wareCodeNew = newItem.ware.code;
          }
        }

        // wareLength check
        if (oldItem.wareLength != newItem.wareLength) {
          itemDiff.wareLengthCheck = true;
          itemDiff.wareLengthOld = oldItem.wareLength;
          itemDiff.wareLengthNew = newItem.wareLength;
        }
        // wareUnit check
        if (oldItem.wareUnit != newItem.wareUnit) {
          itemDiff.wareUnitCheck = true;
          itemDiff.wareUnitOld = oldItem.wareUnit;
          itemDiff.wareUnitNew = newItem.wareUnit;
        }

        itemDiff.name = newItem.inProduction.component.name;
        itemDiff.number = newItem.inProduction.component.number;
        subOrderItemsDiff.push(itemDiff);
      }
    });

    return subOrderItemsDiff;
  }

  AreAllItemFine():boolean {

    let result: boolean = true;
    let isFalse: boolean = false;

    this.subOrder.forEach((item) => {
      if (item.inProduction.sourceType == 0 && item.inProduction.component.componentType == 0
        && item.isInProduction && item.isAdditionallyPurchasable == 0 && !isFalse) {
        if (item.ware == null) {
          result = false;
          isFalse = true;
        }
      }
    })

    return result;
  }

  openConfirmDialog() {

    var modifiedSubOrderItems = this.subOrder.filter(x => x.updatedLocally != x.updatedInDatabase);
    var subOrderItemsDiff = this.checkSubOrderItemsDifferences(modifiedSubOrderItems, this.originalSubOrderItems);

    if (subOrderItemsDiff.length > 0) {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = true;
      dialogConfig.autoFocus = true;
      dialogConfig.data = subOrderItemsDiff;
      dialogConfig.width = "800px";

      const dialogRef = this.dialog.open(SuborderEditConfirmComponent, dialogConfig);


      dialogRef.afterClosed().subscribe((confirm: boolean) => {
        if (confirm) {
          const dialogConfig = new MatDialogConfig();
          dialogConfig.disableClose = true;
          dialogConfig.autoFocus = true;
          dialogConfig.width = '500px';
          dialogConfig.data = this.subOrderInfo;
          //console.log(this.subOrderInfo);
          const dialogRefProgress = this.dialog.open(SuborderEditProgressComponent, dialogConfig);
          this.subOrderService.updateSubOrderItem(modifiedSubOrderItems).then(() => {

          }).finally(() => {
            setTimeout(() => {
              dialogRefProgress.close();
              this.navigateToOrderDetails();
            }, 2000);
          });
        }
      });
    }
    else {
      this.navigateToOrderDetails();
    }
  }

}


// Pipes
@Pipe({ name: 'highlight' })
export class HighlightPipe implements PipeTransform {
  transform(value: string, search: string, uppercase: boolean): string {
    const pattern = new RegExp(search, 'gi');

    var result = value.replace(pattern, uppercase ? `<b>${search.toUpperCase()}</b>` : `<b>${search}</b>`);

    return result;
  }
}


