import { animate, state, style, transition, trigger } from '@angular/animations';
import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
import { Component, ElementRef, OnDestroy, OnInit, Pipe, PipeTransform, ViewChild } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatSnackBar, MatTabChangeEvent, MatDialogRef } from '@angular/material';
import { NavigationEnd, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { NewOrderComponent } from 'src/app/dialogs/new-order/new-order.component';
import { ProgressComponent } from 'src/app/dialogs/progress/progress.component';
import { OptimaItem } from 'src/app/models/optima/optimaitem.model';
import { Order } from 'src/app/models/orders/order.model';
import { InProduction } from 'src/app/models/production/inproduction.model';
import { InventorComponent } from 'src/app/models/production/inventorcomponent.model';
import { Ware } from 'src/app/models/warehouse/ware.model';
import { Book } from '../../models/books/book.model';
import { BookComponent } from '../../models/books/bookcomponent.model';
import { ComponentItem } from '../../models/components/componentitem.model';
import { SortedComponents } from '../../models/components/sortedcomponents.model';
import { OptimaMag } from '../../models/optima/optimamag.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { BookService } from '../../services/books/book.service';
import { ComponentService } from '../../services/components/component.service';
import { OptimaService } from '../../services/optima/optima.service';
import { OrdersService } from '../../services/orders/orders.service';
import { InProductionService } from '../../services/production/inproduction.service';
import { PurchaseItemService } from '../../services/components/purchaseitem.service';
import { PagedOrder } from '../../models/orders/pagedorder.model';
import { ToastrService } from 'ngx-toastr';
import { ErrorComponent } from '../../dialogs/error/error.component';
import { ConfirmDeleteComponent } from '../../dialogs/confirm-delete/confirm-delete.component';
import { ConfirmComponent } from '../../dialogs/confirm/confirm.component';
import { WarehouseItemsService } from '../../services/production/warehouseItems.service';
import { WarehouseItem } from '../../models/production/warehouseItem.model';


@Component({
  selector: 'app-order-import',
  templateUrl: './book-import.component.html',
  styleUrls: ['./book-import.component.css'],
  providers: [
  ],
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


export class BookImportComponent implements OnInit, OnDestroy {

  // ############## DEBUG ##############
  debugFlag: boolean = true;
  // ############## DEBUG ##############


  progress: number = 0;
  progressString: string = "0";
  message: string = "";
  selectedFile: File = null;
  isBomImported: boolean = false;
  isBomSorted: boolean = false;
  sortedComponents: SortedComponents;
  selectedComponent: InventorComponent = new InventorComponent();
  selectedPurchaseComponent: InventorComponent = new InventorComponent();
  observe: Observable<InventorComponent>;
  selectedIndex: number = 0;
  selectedPurchaseIndex: number = 0;
  selectedPurchaseStartIndex: number = 1;
  selectedPurchaseEndIndex: number = 0;
  components: InventorComponent[];
  allowNextComponent: boolean = true;
  allowPreviousComponent: boolean = false;
  allowNextPurchaseItem: boolean = true;
  allowPreviousPurchaseItem: boolean = false;
  allowAddWareToComponent: boolean = false;
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
  reservationComponents: WarehouseItem[] = [];
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


  @ViewChild('acValue', { static: false }) searchOptimaWarehouseInputElRef: ElementRef;
  @ViewChild('acValuePurchase', { static: false }) searchOptimaWarehousePurchaseInputElRef: ElementRef;

  reserveInProductionButtonColor = "primary";

  //@HostListener('document:keydown', ['$event'])
  //keyEvent(event: KeyboardEvent) {
  //if (this.isBomImported) {
  //  if (event.keyCode == KeyCodes.DownArrow) {

  //    this.onNextComponent();
  //  }
  //  else if (event.keyCode == KeyCodes.UpArrow) {
  //    this.onPreviousComponent();
  //  }
  //}
  //}

  constructor(private http: HttpClient, private ordersService: OrdersService, private optimaService: OptimaService,
    private componentService: ComponentService, public dialog: MatDialog, private bookService: BookService,
    private router: Router, private purchaseItemService: PurchaseItemService, private snackBar: MatSnackBar,
    private inProductionService: InProductionService, private warehouseItemService: WarehouseItemsService) {
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

  selectionChange(event) {
    //console.log(event);
  }

  getFileErrorMessage() {
    //console.log(this.fileInput.hasError('required'));
    return this.fileInput.hasError('required') ? 'You must enter a value' : '';
  }

  onPurchaseItemProductionToggleClick(component) {
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
    this.updateAddToComponentButton();
  }
  wareLengthMChange() {
    this.wareToBeAddedLengthMM = this.wareToBeAddedLengthM * 1000;
    this.updateAddToComponentButton();
  }

  // Uploads selected xls file to server and then calls getBom method
  onUpload() {
    const formData = new FormData();
    formData.append(this.selectedFile.name, this.selectedFile);
    this.http.post(
      `api/upload`,
      formData, {
      headers: new HttpHeaders({ 'FileUploadPath': 'xlsOrders' }),
      reportProgress: true,
      observe: 'events'
    }).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
        this.message = this.progress.toString();
      }
      else if (event.type === HttpEventType.Response) {
        if (event.ok) {
          this.message = event.body.toString();
          setTimeout(() => {
            this.isBomImported = true;
            this.getBom();
          }, 2000);
        }
      }
    });
  }

  // Extracts components from selected xls file
  getBom() {

    this.http.get<InventorComponent[]>('api/ImportOrder', {
      params: {
        fileName: this.selectedFile.name
      }
    }).subscribe(
      (response) => {
        this.components = response;
        console.log(this.components);
        this.isBomSorted = true;
        this.selectedComponent = this.components[this.selectedIndex];
        if (this.components.filter(item => item.lastSourceType == 5).length > 0) {
          this.selectedPurchaseComponent = this.components.filter(item => item.lastSourceType == 5)[this.selectedPurchaseIndex];
          this.isPurchaseItemsPresent = true;
        }
        var bookName = this.selectedFile.name;
        var fileNameSplited = bookName.split(/\s*-\s*/);
        this.book = new Book(fileNameSplited[0], fileNameSplited[1]);


        // Create new instance of BookComponents with basic info about imported book
        this.bookComponents = new BookComponent(this.book, this.components);
        this.bookComponentNumberInput.setValue(this.book.componentNumber);
        this.bookOfficeNumberInput.setValue(this.book.officeNumber);


        // Check if book already exist in database
        this.bookService.checkIfBookExist(this.bookComponents)
          .subscribe((bookExist) => {
            
            // If book exists get it from database
            if (bookExist) {
              this.bookService.getBook(this.book).then(
                (bookComponent: BookComponent) => {
           
                  this.book = bookComponent.book;
                  this.bookComponents.book = bookComponent.book;
                  this.bookOfficeNumberInput.setValue(this.book.officeNumber);
                  //this.bookOfficeNumberInput.disable();
                  this.bookNameInput.setValue(this.book.name);
                  //this.bookNameInput.disable();
                  //this.bookComponentNumberInput.disable();
                  this.bookExist = true;
                },
                (error) => { console.log(error); }
              );
            }
            else {

            }

          });

        this.evaluateNextPurchaseItemIndex();
        this.findLastIndextOfPurchaseItems();
        this.selectedPurchaseStartIndex = this.selectedPurchaseIndex;
        this.loadComponentWare(this.selectedComponent);
        this.updateAddToComponentButton();
        this.updateAddToPurchaseComponentButton();
        this.getOptimaMags();
        this.componentQuantityInput.setValue(this.selectedComponent.quantity);
      });
  }

  // For testing
  getBomTest() {
    this.http.get<InventorComponent[]>('api/ImportOrder', {
      params: {
        fileName: "042.000B.xlsx"
      }
    }).subscribe(
      (response) => {
        this.components = response;
        this.isBomSorted = true;
        this.selectedComponent = this.components[this.selectedIndex];
        this.selectedPurchaseComponent = this.components.filter(item => item.lastSourceType == 5)[this.selectedPurchaseIndex];
        this.evaluateNextPurchaseItemIndex();
        this.findLastIndextOfPurchaseItems();
        this.selectedPurchaseStartIndex = this.selectedPurchaseIndex;
        this.loadComponentWare(this.selectedComponent);
      });
  }

  // Switch to next standard component
  onNextComponent() {

    // Get current index increment by 1
    var index = this.selectedIndex + 1;
    var isNextComponent = false;
    // Look for next standard component ( lastSourceType != 5)
    for (index; index < this.components.length + 1; index++) {
      if (index + 1 < this.components.length && this.components[index].lastSourceType != 5) {
        this.selectedIndex = index;
        this.selectedComponent = this.components[this.selectedIndex];

        this.loadComponentWare(this.selectedComponent);
        var nextIndex = index + 1;
        // Check for next component
        for (nextIndex; nextIndex < this.components.length + 1; nextIndex++) {
          if (nextIndex < this.components.length && this.components[nextIndex].lastSourceType != 5) {
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
    this.updateAddToComponentButton();

  }

  // Switch to previous standard component
  onPreviousComponent() {
    // Get current index decremented by 1
    var index = this.selectedIndex - 1;

    // Look for previous standard component ( lastSourceType != 5)
    for (index; index >= 0; index--) {
      if (index - 1 < this.components.length && this.components[index].lastSourceType != 5) {
        this.selectedIndex = index;
        this.selectedComponent = this.components[this.selectedIndex];
        this.loadComponentWare(this.selectedComponent);
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
    this.updateAddToComponentButton();

  }

  onKeydownMMInput(event) {
    this.updateAddToComponentButton();
    if (event.key === "Enter" || event.key === "Tab") {
      if (this.allowAddWareToComponent) {
        this.wareLengthMMChange();
        this.onAddWareToComponent();
        this.onNextComponent();
        setTimeout(() => {
          this.searchOptimaWarehouseInputElRef.nativeElement.focus()
        });
      }
    }
  }

  onKeydownMInput(event) {
    this.updateAddToComponentButton();

    if (event.key === "Enter" || event.key === "Tab") {
      if (this.allowAddWareToComponent) {
        this.wareLengthMChange();

        this.onAddWareToComponent();

        this.onNextComponent();

        setTimeout(() => this.searchOptimaWarehouseInputElRef.nativeElement.focus());
      }
    }
  }

  // Components list click event
  onSelectedComponent(component: InventorComponent) {
    if (this.selectedComponent != component) {
      if (!Number.isInteger(this.selectedComponent.quantity) || this.selectedComponent.quantity <= 0) {
        this.componentQuantityInput.setValue(1);
      }
    }
    //this.onComponentQtyChange(this.selectedComponent);
    this.selectedComponent = component;
    this.loadComponentWare(component);
    this.getReservationComponents(component);
    this.selectedIndex = this.components.indexOf(component);
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
    for (nextIndex; nextIndex < this.components.length + 1; nextIndex++) {
      if (nextIndex < this.components.length && this.components[nextIndex].lastSourceType != 5) {
        isNextComponent = true;
        break;
      }
    }
    this.componentQuantityInput.setValue(this.selectedComponent.quantity);
    this.allowNextComponent = isNextComponent;
    this.updatePlasmaSliders();


    this.wareToBeAdded = new Ware();
    this.wareToBeAddedLengthM = null;
    this.wareToBeAddedLengthMM = null;
    this.selectedOptimaItem = new OptimaItem();
    this.updateAddToComponentButton();
    this.searchOptimaWarehouse.setValue(new OptimaItem());
    
  }

  // Update addWareToComponentButton
  updateAddToComponentButton() {
    // Check if ware can be added to component
    //console.log(this.selectedComponent);
    if (this.selectedComponent.ware == null) {
      if (this.wareToBeAdded != null && this.wareToBeAddedLengthM != null) {
        this.allowAddWareToComponent = true;
      }
      else {
        this.allowAddWareToComponent = false;
      }
    }
    else {
      this.allowAddWareToComponent = false;
    }
  }

  // Update addWareToPurchaseComponentButton
  updateAddToPurchaseComponentButton() {
    // Check if ware can be added to component
    if (this.selectedPurchaseComponent.ware != null) {
      this.allowAddWareToPurchaseComponent = false;
    }
    else {
      this.allowAddWareToPurchaseComponent = true;
    }
  }

  // Modifies total component quantity value
  onComponentQtyChange(component: InventorComponent) {
    this.selectedComponent.quantity = this.componentQuantityInput.value;
    var componentsToUpdate = this.components.filter(x => x.number == component.number);
    var sum = componentsToUpdate.reduce((sum: number, item) => sum + item.quantity, 0);
    componentsToUpdate.forEach((component) => {
      component.sumQuantity = sum;
    });
    this.onComponentUpdate(component);
    //// If component is assembly change sub parts quantity
    //if (component.componentType == 1 && component.level != 0) {
    //  this.onComponentAssemblyQtyChange(component);
    //}


  }

  // Modifies quantity of all subsparts in assembly
  onComponentAssemblyQtyChange(component: InventorComponent) {
    var currentLevel = component.level + 1;
    var index = this.selectedIndex + 1;
    for (index; index < this.components.length; index++) {
      if (this.components[index].level >= currentLevel) {
        if (this.components[index].componentType == 0 && this.components[index].level == currentLevel) {
          this.components[index].quantity = this.components[index].singlePieceQty * component.quantity;
        }
      }
      else {
        break;
      }
    }
  }

  // Toogles toProduction value
  onToProductionToggleChange() {
    this.selectedComponent.toProduction = !this.selectedComponent.toProduction;
    if (this.selectedComponent.toProduction) {

      this.wareDivState = 'enabled';
      this.plasmaToggleYesNoState = 'enabled';
    }
    else {
      this.wareDivState = 'disabled';
      this.plasmaToggleYesNoState = 'disabled';
    }
    this.onComponentUpdate(this.selectedComponent);
    this.updatePlasmaSliders();
  }

  // Updates sliders values based on lastSourceType property
  updatePlasmaSliders() {
    //console.log(this.selectedComponent);
    switch (this.selectedComponent.lastSourceType) {
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

    // Disable sub sliders
    if (!plasma.value && (orgin.value || material.value)) {
      orgin.setValue(false);
      material.setValue(false);
    }
    else if (plasma.value && !orgin.value) {
      material.setValue(false);
    }
    //console.log('Plasma toggle: ' + plasma.value);
    // Assign new value according to slider values
    if (plasma.value && !orgin.value && !material.value) {
      var componentsToUpdate = this.components.filter(x => x.number == this.selectedComponent.number);
      componentsToUpdate.forEach(item => {
        item.lastSourceType = 2;
      })
      //this.selectedComponent.lastSourceType = 2;
      this.wareDivState = 'enabled';
      this.wareDivEnabled = true;

    }
    else if (plasma.value && orgin.value && !material.value) {
      var componentsToUpdate = this.components.filter(x => x.number == this.selectedComponent.number);
      componentsToUpdate.forEach(item => {
        item.lastSourceType = 3;
      })
      //this.selectedComponent.lastSourceType = 3;
      this.wareDivState = 'disabled';
      this.wareDivEnabled = false;
    }
    else if (plasma.value && orgin.value && material.value) {
      var componentsToUpdate = this.components.filter(x => x.number == this.selectedComponent.number);
      componentsToUpdate.forEach(item => {
        item.lastSourceType = 4;
      })
      //this.selectedComponent.lastSourceType = 4;
      this.wareDivState = 'enabled';
      this.wareDivEnabled = true;
    }
    else {
      var componentsToUpdate = this.components.filter(x => x.number == this.selectedComponent.number);
      componentsToUpdate.forEach(item => {
        item.lastSourceType = 0;
      })
      //this.selectedComponent.lastSourceType = 0;
      this.wareDivState = 'enabled';
      this.wareDivEnabled = true;
    }
    // Update component in database
    this.componentService.updateComponent(this.selectedComponent).subscribe();
  }

  // Adds Optima ware to component
  onAddWareToComponent() {

    if (this.selectedOptimaItem.twr_Kod == null && (this.wareToBeAddedLengthM != null || this.wareToBeAddedLengthMM != null))
      return;
    this.wareToBeAdded.length = this.wareToBeAddedLengthM;
    this.selectedComponent.ware = this.wareToBeAdded;
    //console.log(this.wareToBeAdded);
    // Call addComponentWare function in component service
    this.componentService.addWare(this.selectedComponent).subscribe(
      () => { },
      (error) => { console.log(error); },
      () => {
        this.loadComponentWare(this.selectedComponent);
        this.searchOptimaWarehouse.setValue(new OptimaItem());
      });

    this.wareToBeAdded = new Ware();
    this.wareToBeAddedLengthM = null;
    this.wareToBeAddedLengthMM = null;
    this.selectedOptimaItem = new OptimaItem();
    this.onComponentUpdate(this.selectedComponent);
    this.updateAddToComponentButton();
  }

  // Deletes Optima ware from component
  onDeleteWareFromComponent(component: InventorComponent) {
    // Call deleteComponentWare function in component service
    this.componentService.deleteComponentWare(component.componentId).subscribe(
      () => { },
      (error) => { console.log(error); },
      () => {
        if (component.lastSourceType == 5) {
          this.selectedPurchaseComponent.ware = null;
          this.allowAddWareToPurchaseComponent = true;
        }
        else {
          this.selectedComponent.ware = null;
          this.allowAddWareToComponent = true;
        }
      });

    if (component.lastSourceType == 5) {
      this.onComponentUpdate(this.selectedPurchaseComponent);
      this.updateAddToPurchaseComponentButton();
    }
    else {
      this.onComponentUpdate(this.selectedComponent);
      this.updateAddToComponentButton();
    }
  }

  // Gets component Optima ware
  async loadComponentWare(component: InventorComponent): Promise<any> {
    return await this.componentService.getComponentWare2(component).then((data: InventorComponent) => {
      if (data.ware == null) {
        if (component.lastSourceType === 5) {
          this.updateAddToPurchaseComponentButton();
        }
        else {
          this.updateAddToComponentButton();
        }
        return;
      }
      if (component.lastSourceType === 5) {
        this.selectedPurchaseComponent.ware = data.ware;
        this.selectedPurchaseComponent.wareLength = data.wareLength;
        this.selectedPurchaseComponent.wareQuantity = data.wareQuantity;
        this.selectedPurchaseComponent.wareUnit = data.wareUnit;
        this.selectedPurchaseComponent.componentId = data.componentId;
        this.updateAddToPurchaseComponentButton();
      }
      else {
        this.selectedComponent.ware = data.ware;
        this.selectedComponent.wareLength = data.wareLength;
        this.selectedComponent.wareQuantity = data.wareQuantity;
        this.selectedComponent.wareUnit = data.wareUnit;
        this.selectedComponent.componentId = data.componentId;
        this.updateAddToComponentButton();
      }
    });
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
    this.loadComponentWare(this.selectedPurchaseComponent);
    this.updateAddToPurchaseComponentButton();
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
    this.loadComponentWare(this.selectedPurchaseComponent);
    this.updateAddToPurchaseComponentButton();
  }

  // Evaluates next purchase item index
  evaluateNextPurchaseItemIndex() {
    var index = this.selectedPurchaseIndex + 1;
    for (index; index < this.components.length + 1; index++) {
      if (index < this.components.length && this.components[index].lastSourceType == 5) {
        this.selectedPurchaseComponent = this.components[index];
        this.selectedPurchaseIndex = index;
        break;
      }
    }
  }

  // Evaluates previous purchase item index
  evaluatePreviousPurchaseItemIndex() {
    var index = this.selectedPurchaseIndex - 1;
    for (index; index > 0; index--) {
      if (index > 0 && this.components[index].lastSourceType == 5) {
        this.selectedPurchaseComponent = this.components[index];
        this.selectedPurchaseIndex = index;
        break;
      }
    }
  }

  // Finds last index of purchase components list
  findLastIndextOfPurchaseItems() {
    for (var index = 0; index < this.components.length; index++) {
      if (this.components[index].lastSourceType == 5) {
        this.selectedPurchaseEndIndex = index;
      }
    }
  }

  // Purchase components list click event
  onSelectedPurchaseComponent(component: InventorComponent, index: any) {
    this.selectedPurchaseComponent = component;
    this.selectedPurchaseIndex = index;
    this.loadComponentWare(component);

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
    //this.updateAddToPurchaseComponentButton();
  }

  // Adds Optima ware to purchase component
  onAddWareToPurchaseComponent() {

    if (this.selectedPurchaseOptimaItem.twr_Kod == null)
      return;
    this.selectedPurchaseComponent.ware = this.purchaseItemWareToBeAdded;

    // Call addComponentWare function in component service
    this.componentService.addWare(this.selectedPurchaseComponent).subscribe(
      () => { },
      (error) => { console.log(error); },
      () => {
        this.loadComponentWare(this.selectedPurchaseComponent);
        this.searchPurchaseOptimaWarehouse.setValue(new OptimaItem());
      });

    this.purchaseItemWareToBeAdded = new Ware();
    this.selectedPurchaseOptimaItem = new OptimaItem();
    this.onComponentUpdate(this.selectedPurchaseComponent);
    this.updateAddToPurchaseComponentButton();
  }

  // On order change event
  onOrderChange(event) {
    this.selectedOrder = event.option.value;
    var orderId = this.selectedOrder.orderId;

    this.bookService.genSubOrderNumber(orderId).subscribe(
      data => {
        this.subOrderInput.patchValue(data.value);
      });

  }

  // Get all active orders
  getOrders() {
    this.ordersService.getOrders('', 'asc', 0, 0, 9999, '','','')
      .subscribe(response => {
        this.orders = response;
        this.selectedOrder = new Order();
      });

  }

  // Component init 
  ngOnInit() {
    this.bookQuantityInput.setValue(1);
    this.bookNameInput.valueChanges.subscribe(
      (next) => this.book.name = next);
    this.bookComponentNumberInput.valueChanges.subscribe(
      (next) => this.book.componentNumber = next);
    this.bookOfficeNumberInput.valueChanges.subscribe(
      (next) => this.book.officeNumber = next);
    this.bookQuantityInput.valueChanges.subscribe(
      (next) => this.book.quantity = next);
    this.getOrders();
    //this.getBomTest();

    this.searchOptimaWarehouse = new FormControl;
    this.searchPurchaseOptimaWarehouse = new FormControl;
    this.orderInput = new FormControl;

    this.orderInput.valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isLoading = true;
          }
        }),
        switchMap(value => value.length >= 3 ? this.ordersService.searchOrder(value).pipe(finalize(() => {
          this.isLoading = false;

        })) : []),
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(orders => {
        this.filteredOrders = orders;
      }, () => { });

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
        //console.log(items);
      }, () => { });
  }



  async getOptimaMags() {
    return await this.optimaService.getMags(true).then(
      (data: OptimaMag[]) => {
        this.optimaMags = data;
        //console.log(this.optimaMags);
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

  // Component update
  onComponentUpdate(component: InventorComponent) {
    var componentsToUpdate = this.components.filter(x => x.number == component.number);
    componentsToUpdate.forEach((item) => {
      item.updated = true;
      item.ware = component.ware;
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

  // New order popup window
  onNewOrder() {

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      title: 'Nowe zlecenie'
    };

    const dialogRef = this.dialog.open(NewOrderComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(
      data => {
        if (data != null) {
          this.ordersService.createOrder(data).subscribe(
            () => { this.getOrders(); })
        }
      }
    )
  }

  // Snack bar message window
  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 10000,
    });
  }

  onCopyToPurchaseListToggleChange(event) {
    var isChecked = event.checked;
    this.selectedComponent.isAdditionallyPurchasable = isChecked;
    // Update component in database
    this.componentService.updateComponent(this.selectedComponent).subscribe();
    //if (isChecked) {
    //  this.toastr.success(`${this.selectedComponent.number} skopiowany do listy części kupnych`, '', { positionClass: 'toast-bottom-center', timeOut: 2500 });
    //}
    //else {
    //  this.toastr.success(`${this.selectedComponent.number} usunięty z listy części kupnych`, '', { positionClass: 'toast-bottom-center', timeOut: 2500 });
    //}
  }

  toogleAllStandardComponentsToProduction() {
    this.components.filter(comp => comp.lastSourceType != 5).forEach(item => item.toProduction = this.allStandardComponentsToProductionToggle);
    this.allStandardComponentsToProductionToggle = !this.allStandardComponentsToProductionToggle;
  }

  toogleAllPurchaseItemsToProduction() {
    this.components.filter(comp => comp.lastSourceType == 5).forEach(item => item.toProduction = this.allPurchaseComponentsToProductionToggle);
    this.allPurchaseComponentsToProductionToggle = !this.allPurchaseComponentsToProductionToggle;
  }

  onConfirmAddBookToOrder() {
    const confrimDialogConfig = new MatDialogConfig();

    confrimDialogConfig.disableClose = true;
    confrimDialogConfig.autoFocus = true;

    confrimDialogConfig.data = {
      message: `Czy na pewno chcesz dodać podzlecenie ${this.subOrderInput.value} ?`
    };

    const confirmDialogRef = this.dialog.open(ConfirmComponent, confrimDialogConfig);

    confirmDialogRef.afterClosed().subscribe(
      response => {
        if (response == true) {
          this.onAddBookToOrder();
        }
      });
  }

  // Add book to order and components
  onAddBookToOrder() {
    // If imported book do not exists in database create new component assembly
    if (this.bookAssembly.number == "" || this.bookAssembly.number == null) {
      var assembly: InventorComponent = {
        materialType: null,
        cost: 0,
        addedBy: null,
        author: null,
        comment: null,
        existInDatabase: false,
        order: 0,
        requiredItems: null,
        lastSourceType: 0,
        sumQuantity: 0,
        toProduction: true,
        updated: false,
        ware: null,
        weight: null,
        quantity: 1,
        componentId: 0,
        treeNumber: null,
        componentType: 2,
        level: 0,
        name: this.bookNameInput.value,
        number: this.bookComponentNumberInput.value,
        modifiedDate: new Date(),
        wareLength: null,
        wareUnit: null,
        wareQuantity: null,
        singlePieceQty: 1,
        inProductionReservation: null,
        isAdditionallyPurchasable: false,
        reservedItemId: -1,
        reservedQty: 0
      };
      this.bookAssembly = assembly;
    }
    //console.log("Assembly: " + this.bookAssembly);
    //console.log(this.bookAssembly);
    // Add book component assembly to the begining of the components list
    //var componentsDto = this.components.slice();
    this.components.unshift(this.bookAssembly);
    //console.log("Unshifted BOM: " + this.components);

    // OrderBookDto
    var orderBook: OrderBook = {
      orderBookId: null,
      bookId: null,
      book: this.book,
      orderId: null,
      order: this.selectedOrder,
      plannedQty: this.bookQuantityInput.value,
      finishedQty: 0,
      addedBy: null,
      addedDate: new Date(),
      comment: null,
      componentNumber: null,
      officeNumber: null,
      orderNumber: null,
      position: null,
      type: 0,
      number: this.subOrderInput.value,
      plasmaInList: 0,
      plasmaOutList: 0,
      wareList: 0,
      purchaseList: 0,
      isExpanded: false
    }
    //console.log("OrderBook: " + orderBook);
    this.bookComponents.book.quantity = this.bookQuantityInput.value;
    const progressDialogConfig = new MatDialogConfig();
    var progressDialogRef;

    progressDialogConfig.disableClose = true;
    progressDialogConfig.autoFocus = true;
    progressDialogConfig.data = {
      message: `Tworzenie książki ${this.book.componentNumber}.`,
      title: "Importowanie książki produkcyjnej"
    };



    // Progress dialog box subscription
    progressDialogRef = this.dialog.open(ProgressComponent, progressDialogConfig);
    progressDialogRef.afterClosed().subscribe(() => {
      this.initialize();
      this.router.navigate(['/orders/import']);
    });
    // Write selected order to bookComponents
    this.bookComponents.mainOrder = this.selectedOrder;
    // Write OrderBook model to bookComponents
    this.bookComponents.orderBook = orderBook;
    
    // If book do not exist in database
    // *** CREATE NEW BOOK WITH COMPONENTS => ADD BOOK TO MAIN ORDER => ADD BOOK TO PRODUCTION ***
    if (!this.bookExist) {
      if (this.debugFlag) {
        console.log("========== Książka nie istnieje =========");
      }
      // Create new book with components
      this.bookService.addComponentsToBook(this.bookComponents).subscribe(
        (book: Book) => {
          if (this.debugFlag) {
            console.log("Tworzenie nowej ksiazki z komponentami.");
          }
          this.bookComponents.book = book;
          //console.log("Book: " + this.bookComponents.book);
          progressDialogRef.componentInstance.data =
          {
            message: `Tworzenie książki ${this.book.officeNumber}.`,
            title: "Importowanie książki produkcyjnej",
            canClose: false
          };
        },
        (error) => {
          this.showErrorDialog(error);
          console.log(error);
        },
        () => {
          // When finished adding components to book add this book to main order
          if (this.debugFlag) {
            console.log("Dodawanie ksiązki do zlecenia głównego.");
          }
          this.ordersService.addBook(orderBook).subscribe(
            () => {
              progressDialogRef.componentInstance.data =
              {
                message: `Dodawanie książki do zlecenia głównego.`,
                title: "Importowanie książki produkcyjnej",
                canClose: false
              };
            },
            (error) => { this.showErrorDialog(error) },
            () => {
              // Next add book components to book production and rw tables
              if (this.debugFlag) {
                console.log("Dodawanie podzlecenia do produkcji.");
              }
              this.inProductionService.addBookToProduction(this.bookComponents).subscribe(
                () => {
                  progressDialogRef.componentInstance.data =
                  {
                    message: `Dodawanie podzlecenia ${orderBook.number} do produkcji.`,
                    title: "Importowanie książki produkcyjnej",
                    canClose: false
                  };
                },
                (error) => {
                  this.showErrorDialog(error);
                  console.log(error);
                },
                () => {
                  if (this.debugFlag) {
                    console.log("Zakończono import książki!");
                  }
                  // When finished close dialog window and redirect
                  progressDialogRef.componentInstance.data =
                  {
                    message: `Zakończono import książki!`,
                    title: "Importowanie książki produkcyjnej",
                    canClose: true
                  };
                }
              );
            });
        });
    }
    // Else if book exist in database
    // *** SEARCH FOR BOOK IN DATABASE => ADD BOOK TO MAIN ORDER => ADD BOOK TO PRODUCTION ***
    else {
      //console.log("Book exist =>");
      // Add book to main order
      if (this.debugFlag) {
        console.log("========== Książka istnieje =========");
      }
      if (this.debugFlag) {
        console.log("Dodawanie książki do zlecenia głównego.");
      }
      this.ordersService.addBook(orderBook).subscribe(
        () => {
          progressDialogRef.componentInstance.data =
          {
            message: `Dodawanie książki do zlecenia głównego.`,
            title: "Importowanie książki produkcyjnej",
            canClose: false
          };
        },
        (error) => {
          this.showErrorDialog(error);
          console.log(error);
        },
        () => {
          // Next add book components to book production and rw tables
          // Write OrderBook model to bookComponents
          if (this.debugFlag) {
            console.log("Dodawanie podzlecenia do produkcji.");
          }
          this.inProductionService.addBookToProduction(this.bookComponents).subscribe(
            () => {
              //console.log(this.bookComponents);
              progressDialogRef.componentInstance.data =
              {
                message: `Dodawanie podzlecenia ${orderBook.number} do produkcji.`,
                title: "Importowanie książki produkcyjnej",
                canClose: false
              };
            },
            (error) => {
              this.showErrorDialog(error);
              console.log(error);
            },
            () => {
              if (this.debugFlag) {
                console.log("Zakończono import książki!");
              }
              // When finished close dialog window and redirect
              progressDialogRef.componentInstance.data =
              {
                message: `Zakończono import książki!`,
                title: "Importowanie książki produkcyjnej",
                canClose: true
              };
            }
          );
        });
    }

  }
  onKeydownACValuePurchase(event) {
    if (this.tabSelected == 1) {
      if (event.key === "Enter" || event.key === "Tab") {
        this.onAddWareToPurchaseComponent();
        this.onNextPurchaseItem();
        setTimeout(() => this.searchOptimaWarehousePurchaseInputElRef.nativeElement.focus());
      }
    }
  }

  showErrorDialog(error: any) {
    console.log(error);
    this.errorDialogConfig.data = {
      message: error
    };
    const errorDialogRef = this.dialog.open(ErrorComponent, this.errorDialogConfig);

    errorDialogRef.afterClosed().subscribe(
      response => {
        // Sproboj ponownie utworzyc podzlecenie
        if (response == true) {
          this.onAddBookToOrder();
        }
        // Wroc do edycji, nic nie rob, wyczyscic jakies dane ?
        else {

        }
      }
    );
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
    this.selectedComponent = new InventorComponent();
    this.selectedPurchaseComponent = new InventorComponent();
    this.observe = new Observable<InventorComponent>();
    this.selectedIndex = 0;
    this.selectedPurchaseIndex = 0;
    this.components = [];
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
    this.orderInput.setValue(new Order());
    this.bookComponentNumberInput.setValue(null);
    this.bookComponentNumberInput.updateValueAndValidity();
    this.bookOfficeNumberInput.setValue(null);
    this.bookOfficeNumberInput.updateValueAndValidity();
    this.bookNameInput.setValue(null);
    this.bookNameInput.updateValueAndValidity();
    this.bookQuantityInput.setValue(1);
    this.bookQuantityInput.updateValueAndValidity();
    this.subOrderInput.setValue(null);
    this.subOrderInput.updateValueAndValidity();
    this.bookAssembly = new InventorComponent();
     }

  getReservationComponents(component: InventorComponent) {
    this.warehouseItemService.getComponentsFromWarehouseItems(component).subscribe(
      (data: WarehouseItem[]) => {
        this.reservationComponents = data;
        console.log(data);
      },
      (error) => { console.log(error) },
      () => { }
    );
  }

  onReserveInProduction(reservationComponent: WarehouseItem) {

    let index: number = this.bookComponents.components
      .findIndex(x => x.treeNumber == this.selectedComponent.treeNumber);

    //reserve
    if (this.selectedComponent.inProductionReservation == null) {
      this.selectedComponent.inProductionReservation = reservationComponent.parentRWItem.inProduction;
      this.reserveInProductionButtonColor = "warn";
      this.bookComponents.components[index].reservedItemId = reservationComponent.warehouseItemId;
      if ((this.componentQuantityInput.value * this.bookQuantityInput.value) < (reservationComponent.componentQty - reservationComponent.reservedQty)) {
        this.bookComponents.components[index].reservedQty = this.componentQuantityInput.value * this.bookQuantityInput.value;
      } else {
        this.bookComponents.components[index].reservedQty = reservationComponent.componentQty - reservationComponent.reservedQty;
      }
      
    }
    //delete reservation
    else {
      this.selectedComponent.inProductionReservation = null;
      this.reserveInProductionButtonColor = "primary";
      this.bookComponents.components[index].reservedItemId = -1;
      this.bookComponents.components[index].reservedQty = 0;
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

