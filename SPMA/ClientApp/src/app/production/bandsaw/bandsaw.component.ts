import { DOCUMENT } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Inject, OnDestroy, OnInit, Pipe, PipeTransform, ViewChild } from '@angular/core';
import { FormControl, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { ErrorStateMatcher, MatDialog, MatDialogConfig, MatSnackBar, MatSnackBarConfig } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { privateDecrypt } from 'crypto';
import { Observable } from 'rxjs';
import { debounceTime, finalize, switchMap, tap, filter } from 'rxjs/operators';
import { BandsawCutConfirmComponent } from '../../dialogs/bandsaw-cut-confirm/bandsaw-cut-confirm.component';
import { ChangeWithAutocompleteComponent } from '../../dialogs/change-with-autocomplete/change-with-autocomplete.component';
import { ConfirmComponent } from '../../dialogs/confirm/confirm.component';
import { SuborderEditErrorComponent } from '../../dialogs/suborder-edit-error/suborder-edit-error.component';
import { OptimaItem } from '../../models/optima/optimaitem.model';
import { OptimaMag } from '../../models/optima/optimamag.model';
import { Ware } from '../../models/warehouse/ware.model';
import { OptimaService } from '../../services/optima/optima.service';
import { BookComponentService } from '../../services/production/bookcomponent.service';
import { InProductionService } from '../../services/production/inproduction.service';
import { SubOrderDataService } from '../../services/suborder/sub-order-data.service';
import { SubOrderService } from '../../services/suborder/suborder.service';
import { BookListFromOrders } from '../../shared/Interfaces/BookListFromOrders';


@Component({
  selector: 'app-bandsaw',
  templateUrl: './bandsaw.component.html',
  styleUrls: ['./bandsaw.component.css']
})
export class BandsawComponent implements OnInit, OnDestroy {
  //variables
  title: string = 'Produkcyjny';
  socketType: string;
  sourceType: number;
  orderId: string;
  subOrderId: string;
  officenumber: string;
  componentnumber: string;
  index: number;
  toPreviousAppPage: boolean = false;
  isLoadingSubOrder: boolean = true;
  noWareAssigned: boolean = false;//when true dont display the second list
  isLoading: boolean = false;
  /*QtyPerBook: number = 0;*/
  doneAlreadyQty: number = 0;//number of already cut materials
  totalQty: number = 0;
  setQty: number = 0;
  totalQtyPerBook: number = 0;
  //totalMaterialQty: number = 0;
  //list store number of drawings with same material and their indexies in this.subOrder.data list
  listOfDrawingsWithSameMaterial: { name: string, index: number }[] = [];
  currentListIndex: number = 0;
  listOfOrdersWithSameMaterial: BookListFromOrders[] = [];
  //for both lists selection
  currentListItem2: number;
/*  currentListItem1: number;*/
  //variable to improve the number inputs behavior
  //lengthInputPrecision: number;
  //lengthInputStep: number =1;
  //lengthWithOverflowInputPrecision: number;
  //lengthWithOverflowInputStep: number = 1;
  wareToBeAdded: Ware = new Ware();
  selectedOptimaItem: OptimaItem = new OptimaItem();
  filteredOptimaItems: Observable<OptimaItem[]>;
  selectedMag: OptimaMag = new OptimaMag(0, "WSZYSTKIE", "WSZYSTKIE");

  snackBarConfig: MatSnackBarConfig = {
  duration: 2000,
  horizontalPosition: 'center',
  verticalPosition: 'bottom'
}


  //bookNumberInput = new FormControl('');
  //drawingNumberInput = new FormControl('');
  materialNameInput = new FormControl('');//readonly
  lengthInput = new FormControl('');//readonly
  sumlengthInput = new FormControl('');//readonly
  lengthWithOverflowInput = new FormControl('');
  materialTypeInput = new FormControl({ value: '', disabled: true });//readonly
  quantityInput = new FormControl('');//readonly
  ordersQty = new FormControl('');//readonly
  setQuantityInput = new FormControl('');//readonly
  totalQuantityInput = new FormControl('');//readonly

  matcher = new MyErrorStateMatcher();//class used for immidietly validating input

    


  constructor(private route: ActivatedRoute, private router: Router, private subOrderData: SubOrderDataService,
    private dialog: MatDialog, private autocompleteDialog: MatDialog, private inproductionService: InProductionService,
    private _snackBar: MatSnackBar, private subOrderService: SubOrderService, @Inject(DOCUMENT) private _document: Document,
    private bookComponentService: BookComponentService) { }

  ngOnInit() {

    //retrieve data from url
    this.fetchDatafromUrl();
    //console.log(this.subOrderData.subOrder.data);
    this.downloadData();
    
  }

  ngOnDestroy() {
    //make sure subOrder data won't be loaded only when pages change
    //from "production-component-item-list" to "bandsaw" and vice - versa
    this.toPreviousAppPage ? 0 : this.subOrderData.downloaded = false;
    //console.log(this.subOrderData.downloaded);
  }

  test() {
    //this.subOrderData.subOrder.data[this.index].inProduction.component.name = "zmieniony yekst";
    console.log(this.subOrderData.subOrder.data[this.index]);
  }

  downloadData() {

    if (!this.subOrderData.downloaded) {
      // Combine all methods and retrive data
      this.subOrderData.getBookInfo(this.componentnumber, this.officenumber).then(() => {
        this.subOrderData.getSubOrderInfo(this.subOrderId).then(() => {     
          //console.log(this.subOrderBookComponent);
          this.subOrderData.getSubOrder(this.subOrderId, this.sourceType).then(() => {
            this.subOrderData.getSubOrderAdditionalInfo().then(() => {
              // Update suborder input fields
              this.subOrderData.subOrder.data = this.subOrderData.subOrder.data.slice();
              //console.log(this.subOrderData.subOrder.data);
              // this.listOfOrdersWithSameMaterial = val;
              this.setQty = this.subOrderData.subOrder.data[this.index].inProduction.plannedQty / this.subOrderData.subOrder.data[this.index].inProduction.bookQty;
              /*this.totalQty = this.subOrderData.subOrder.data[this.index].inProduction.bookQty* this.setQty;*/
              //get drwaings with same material
              this.findDrawingsWithSameMaterial();
              this.fillInputs();

              //download list of books with the same occuring material
              this.downloadListOfOrdersWithSameMaterial();
              //change status to 'cutting in progress'
              this.changeMaterialStatus(3);
              //set min validator value
              this.lengthWithOverflowInput.setValidators([Validators.min(this.subOrderData.subOrder.data[this.index].inProduction.wareLength * this.subOrderData.subOrder.data[this.index].toIssue), Validators.required]);
              this.totalQuantityInput.setValidators([Validators.min(this.subOrderData.subOrder.data[this.index].toIssue), Validators.required]);
                  //this.listOfOrdersWithSameMaterial[1].numberOfOccurences = 1;
                  //console.log(this.listOfOrdersWithSameMaterial[1].numberOfOccurences);
              this.subOrderData.downloaded = true;
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
                setTimeout(() => {
                    this.isLoadingSubOrder = false;
                }, 2000)
              })
          })
        })
      })
    } else {
      this.isLoadingSubOrder = false;
      //download list of books with the same occuring material
      this.downloadListOfOrdersWithSameMaterial();
      this.setQty = this.subOrderData.subOrder.data[this.index].inProduction.plannedQty / this.subOrderData.subOrder.data[this.index].inProduction.bookQty;
      //this.totalQty = this.subOrderData.subOrder.data[this.index].inProduction.bookQty * this.setQty;
      this.findDrawingsWithSameMaterial();
      this.fillInputs();
      //change status to 'cutting in progress'
      this.changeMaterialStatus(3);
      //set min validator value
      this.lengthWithOverflowInput.setValidators([Validators.min(this.subOrderData.subOrder.data[this.index].inProduction.wareLength), Validators.required]);
      this.totalQuantityInput.setValidators([Validators.min(this.subOrderData.subOrder.data[this.index].toIssue), Validators.required]);
      //console.log(this.subOrderData.subOrder.data);
    }
  }

  backToSubOrderItemList() {
    this.toPreviousAppPage = true;
    this.router.navigate(['/production/productionSuborderItemList'], {
      queryParams: {
        orderID: this.orderId,
        subOrderID: this.subOrderId,
        officeNumber: this.officenumber,
        componentNumber: this.componentnumber,
        socket: this.socketType,
        index: this.index
      }
    });
  }


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
  //go to page with another suborder but the same type of material
  //TODO page redirect function
  goToAnotherOrder(index) {

    console.log('orderID', this.listOfOrdersWithSameMaterial[index].orderId);
    console.log('SuborderID', this.listOfOrdersWithSameMaterial[index].subOrderId);
    console.log('officeNumber', this.listOfOrdersWithSameMaterial[index].officeNumber);

    this.bookComponentService.getComponentNumber(this.listOfOrdersWithSameMaterial[index].officeNumber).subscribe((componentnumber) => {

      this.subOrderService.getComponentIndex(this.listOfOrdersWithSameMaterial[index].subOrderId, this.subOrderData.subOrder.data[this.index].ware.wareId, this.sourceType)
        .subscribe((componentindex) => {

          console.log('componentNumber', componentnumber);
          console.log('index', componentindex);

          this.router.navigate(['/production/bandsaw'],
            {
              queryParams: {
                orderID: this.listOfOrdersWithSameMaterial[index].orderId,
                subOrderID: this.listOfOrdersWithSameMaterial[index].subOrderId,
                officeNumber: this.listOfOrdersWithSameMaterial[index].officeNumber,
                componentNumber: componentnumber,
                index: componentindex,
                socket: this.socketType
              }
            }).then(() => {
              this.fetchDatafromUrl();
              this.subOrderData.downloaded = false;
              this.isLoadingSubOrder = true;
              this.downloadData();
            });
          // this._document.defaultView.location.reload();

        });

    })

  }


  //filter suborder to get the list of drawing with the same material from the given book
  findDrawingsWithSameMaterial() {
    //clear list
    this.listOfDrawingsWithSameMaterial = [];
    this.totalQtyPerBook = 0;
    this.totalQty = 0;
    //this.totalMaterialQty = 0;
    //start searching
    let index: number = 0;
    let componentNumber: string;
    this.subOrderData.subOrder.data.forEach((item) => {
      //choose the same material
      if (this.subOrderData.subOrder.data[this.index].ware.code) {
        if (this.subOrderData.subOrder.data[this.index].ware.code == item.ware.code) {
          componentNumber = item.inProduction.component.number;
          //add to list
          this.listOfDrawingsWithSameMaterial.push({ name: componentNumber, index: index });
          //choose items with the same length
          if (this.subOrderData.subOrder.data[this.index].inProduction.wareLength == item.inProduction.wareLength) {
            this.totalQty += item.inProduction.plannedQty;
            //this.totalMaterialQty += this.customRound(item.wareLength * item.inProduction.plannedQty, 3);
          }
        }
      }
      index++;
    });
    this.totalQtyPerBook = this.totalQty / this.setQty;
    
    //find current item position in list
    this.currentListIndex = this.listOfDrawingsWithSameMaterial.findIndex((item) => { return item.index == this.index; });
  }

 // count already cut components of the same name (isnt used for now)
  countAlreadyCutMaterial() {
    this.doneAlreadyQty = 0;
    this.subOrderData.subOrder.data.forEach((item) => {
      if (this.subOrderData.subOrder.data[this.index].ware.code == item.ware.code &&
        this.subOrderData.subOrder.data[this.index].inProduction.wareLength == item.inProduction.wareLength &&
        item.inProduction.productionState.productionStateCode == 4) {
        this.doneAlreadyQty += (item.inProduction.plannedQty * item.inProduction.bookQty);
        //console.log(`quantity in one drawing ${this.doneAlreadyQty}`);
      }
    });
  }

  listWitoutRepetition(original: BookListFromOrders[], dest: BookListFromOrders[]) {

    let occured: boolean = false;
    let N: number = original.length;
    let M: number = 1;

    dest.push(original[0]);
    dest[0].numberOfOccurences = 1;

    for (let i = 1; i < N; i++) {
      for (let j = 0; j < M; j++) {
        if (original[i].orderNumber == dest[j].orderNumber &&
          original[i].subOrderNumber == dest[j].subOrderNumber &&
          original[i].officeNumber == dest[j].officeNumber) {
          //if present increment quantity by 1
          occured = true;
          dest[j].numberOfOccurences++;
          break;
        } else {
          occured = false;
        }
      }
      //if not present
      if (!occured) {
        dest.push(original[i]);
        dest[M].numberOfOccurences = 1;
        occured = false;
        M++;
      }
    }

  }


  //fill all input elements with data
  fillInputs() {
    
    if (this.subOrderData.subOrder.data[this.index].ware !==null) {
      this.materialNameInput.setValue(this.subOrderData.subOrder.data[this.index].ware.code);
      this.lengthInput.setValue(this.subOrderData.subOrder.data[this.index].inProduction.wareLength);
      this.sumlengthInput.setValue(this.MathCustomRound(this.subOrderData.subOrder.data[this.index].toIssue * this.subOrderData.subOrder.data[this.index].inProduction.wareLength,2));
      /*      this.setPrecision();*/
      this.lengthWithOverflowInput.setValue(this.MathCustomRound(this.subOrderData.subOrder.data[this.index].toIssue  * this.subOrderData.subOrder.data[this.index].wareLength,2));

        //`${this.subOrderData.subOrder.data[this.index].wareLength.toFixed(2)} ${this.subOrderData.subOrder.data[this.index].wareUnit}`);
     // this.totalQuantityInput.setValue(this.QtyPerBook * this.subOrderData.subOrder.data[this.index].inProduction.bookQty);
    } else {
      this.materialNameInput.setValue("");
      this.lengthInput.setValue("");
      this.sumlengthInput.setValue('');
      this.lengthWithOverflowInput.setValue("");
      //this.totalQuantityInput.setValue("");
    }

    //this.materialTypeInput.setValue(this.subOrderData.subOrder.data[this.index].inProduction.component.materialType);

    this.quantityInput.setValue(this.subOrderData.subOrder.data[this.index].inProduction.bookQty);
    this.ordersQty.setValue(this.subOrderData.subOrderInfo.order.plannedQty);
    this.setQuantityInput.setValue(this.setQty);
    this.totalQuantityInput.setValue(this.subOrderData.subOrder.data[this.index].toIssue);
    
    
    
    
  }


  MathCustomRound(value: number, precision: number): number {
    return Math.round(value * Math.pow( 10, precision ))/Math.pow(10, precision);
  }

  downloadListOfOrdersWithSameMaterial() {
    if (this.subOrderData.subOrder.data[this.index].ware !== null) {

      this.inproductionService
        .GetComponentsWithSameMaterialFromOrderAsync
        (this.subOrderData.subOrder.data[this.index].ware.wareId, this.sourceType)
        .then((val) => {
          //console.log(val);
          // filter repeating items
          //clear list 
          this.listOfOrdersWithSameMaterial = [];
          this.listWitoutRepetition(val, this.listOfOrdersWithSameMaterial);
          //console.log(this.listOfOrdersWithSameMaterial);
          //find currently choosen item
          this.currentListItem2=this.listOfOrdersWithSameMaterial.findIndex((item) => {
           return item.orderNumber == this.subOrderData.subOrderInfo.order.number &&
              item.subOrderNumber == this.subOrderData.subOrderInfo.number &&
              item.officeNumber == this.officenumber;
          });
          //console.log(val);
          //console.log("wareId present");
          this.noWareAssigned = false;
        });
    } else {
      this.noWareAssigned = true;
      //clear list 
      this.listOfOrdersWithSameMaterial = [];
      //console.log("no wareId");
    }
  }


  customRound(value: number, precisoin: number):number {
    let multiplier = Math.pow(10, precisoin);
    value *= multiplier;
    value = Math.round(value);
    return value /= multiplier;
  }

  //findNumberPrecision(num: number):number {
  //  let str: string = num.toString();
  //  let len: number =0;
    
  // // console.log(`search ${str.search('.')}`);
  //  if (num - Math.trunc(num)!==0) {
  //   str=str.split('.')[1];
  //    len = str.length;
  //    let i: number = len;
  //    while (str[i] == '0') {
  //      i--;
  //      len--;
  //    }
  //  } else {
  //    len = 0;
  //  }

  //  //console.log(`len=${len}`);
  //  return len;
  //}

  //setPrecision() {
  //  //set the precision and step to number inputs
  //  this.lengthInputPrecision = this.findNumberPrecision(this.lengthInput.value);
  //  this.lengthInputPrecision <= 2 ? this.lengthWithOverflowInputPrecision = this.lengthInputPrecision : this.lengthWithOverflowInputPrecision = this.lengthInputPrecision - 1;
  //  this.lengthInputStep = Math.pow(10, -this.lengthInputPrecision);
  //  this.lengthWithOverflowInputStep = Math.pow(10, -this.lengthWithOverflowInputPrecision);
  //}

  //ceil(num: number, precision:number): number {
  //  let mlt: number;
  //  if (precision >= 0) {
  //    mlt = Math.pow(10, precision);
  //    num = num * mlt;
  //    num = Math.ceil(num);
  //    num = num / mlt;
  //  }

  //  return num
  //}

  //floor(num: number, precision: number): number {
  //  let mlt: number;
  //  if (precision >= 0) {
  //    mlt = Math.pow(10, precision);
  //    num = num * mlt;
  //    num = Math.floor(num);
  //    num = num / mlt;
  //  }

  //  return num
  //}




  //############################
  //EVENT FUNCTIONS

  onItemSelect2(index: number) {
    //this.selectedListItem2 = index;
  }

  onItemSelect1(index: number) {
    //this.currentListItem1 = index;
  }

  onInputChange(event) {
    //console.log(event);
  }


  onLengthwithOverflowChange(newLength: number) {

    newLength /= this.subOrderData.subOrder.data[this.index].toIssue;//must be divided by number of components

    if (this.lengthWithOverflowInput.valid) {
      //update locally
      this.subOrderData.subOrder.data[this.index].wareLength = newLength;

      //update length in database
      this.inproductionService.changeLengthwithOverflow(this.subOrderData.subOrder.data[this.index].inProductionRWId, newLength)
        .subscribe(() => console.log('change lenngth'));
    }  
  }

  onQtyChange(val: number) {
    if (this.totalQuantityInput.valid) {

      ////update data localy
      this.subOrderData.subOrder.data[this.index].toIssue = val;
      let newTotalLength = this.MathCustomRound(this.subOrderData.subOrder.data[this.index].toIssue * this.subOrderData.subOrder.data[this.index].inProduction.wareLength,2);
      this.sumlengthInput.setValue(newTotalLength);
      this.lengthWithOverflowInput.setValue(newTotalLength);
      //update data in DB
      this.inproductionService.changeTotalQty(this.subOrderData.subOrder.data[this.index].inProductionRWId, val).subscribe(() => console.log("quantity changed"));
      //if total component amount is changing reset overflow in inProductionRW->wareLength
      this.inproductionService.changeLengthwithOverflow(this.subOrderData.subOrder.data[this.index].inProductionRWId, this.subOrderData.subOrder.data[this.index].inProduction.wareLength).subscribe(() => console.log('change lenngth'));
      //TODO if given number is higher then total amount of components in order than create automaticly surplus item in WarehouseItems table 

    }
  }

  onSelectedMaterialNameItem(event) {
    console.log("item selected");
  }

  //onSelectedOptimaItem(event) {
  //  this.selectedOptimaItem = event.option.value;
  //  this.wareToBeAdded.code = this.selectedOptimaItem.twr_Kod;
  //  this.wareToBeAdded.unit = this.selectedOptimaItem.twr_JM;
  //  this.wareToBeAdded.name = this.selectedOptimaItem.twr_Nazwa;
  //  this.wareToBeAdded.mag_Nazwa = this.selectedOptimaItem.mag_Nazwa;
  //  this.wareToBeAdded.mag_Symbol = this.selectedOptimaItem.mag_Symbol;
  //  this.wareToBeAdded.twG_Kod = this.selectedOptimaItem.twG_Kod;
  //  this.wareToBeAdded.twG_Nazwa = this.selectedOptimaItem.twG_Nazwa;
  //  this.wareToBeAdded.date = new Date();

  //}

  //countNewLength(direction:number) {
  //  let temp = this.lengthInput.value;
  //  if (direction < 0) {
  //    temp = this.floor(temp - this.lengthInputStep, this.lengthInputPrecision);
  //    this.lengthInput.setValue(temp);
  //  } else if (direction > 0) {
  //    temp = this.ceil(temp + this.lengthInputStep, this.lengthInputPrecision);
  //    this.lengthInput.setValue(temp);
  //  }
  //  this.subOrderData.subOrder.data[this.index].wareLength = temp;
  //  this.lengthWithOverflowInput.setValue(this.ceil(temp, this.lengthWithOverflowInputPrecision));
  //}

  //countNewLengthWithOverflow(direction:number) {
  //  let temp = this.lengthInput.value;
  //  if (direction < 0) {
  //    temp = this.floor(temp - this.lengthInputStep, this.lengthWithOverflowInputPrecision);
  //    this.lengthWithOverflowInput.setValue(temp);
  //  } else if (direction > 0) {
  //    temp = this.ceil(temp + this.lengthInputStep, this.lengthWithOverflowInputPrecision);
  //    this.lengthWithOverflowInput.setValue(temp);
  //  }
  //  this.subOrderData.subOrder.data[this.index].wareLength = temp;
  //  this.lengthInput.setValue(this.ceil(temp, this.lengthInputPrecision));
  //}



  // Adds Optima ware to component
  onAddWareToComponent() {

    //if (this.selectedOptimaItem.twr_Kod == null && (this.wareToBeAddedLengthM != null || this.wareToBeAddedLengthMM != null))
    //  return;
    //this.wareToBeAdded.length = this.wareToBeAddedLengthM;
    //this.selectedComponent.ware = this.wareToBeAdded;
    ////console.log(this.wareToBeAdded);
    //// Call addComponentWare function in component service
    //this.componentService.addWare(this.selectedComponent).subscribe(
    //  () => { },
    //  (error) => { console.log(error); },
    //  () => {
    //    this.loadComponentWare(this.selectedComponent);
    //    this.searchOptimaWarehouse.setValue(new OptimaItem());
    //  });

    //this.wareToBeAdded = new Ware();
    //this.wareToBeAddedLengthM = null;
    //this.wareToBeAddedLengthMM = null;
    //this.selectedOptimaItem = new OptimaItem();
    //this.onComponentUpdate(this.selectedComponent);
    //this.updateAddToComponentButton();
  }


  //display dialogbox for component type changing
  onMaterialChange() {

    let config: MatDialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '35vw',
    }
    const dialogRef = this.dialog.open(ChangeWithAutocompleteComponent, config);

    dialogRef.afterClosed().subscribe((result: OptimaItem) => {
      console.log(`Dialog result: ${result.twr_Kod}`);
      if (result.twr_Kod) {
        //update input
        this.materialNameInput.setValue(result.twr_Kod);
        //create WareItem
        let ware: Ware = new Ware();
        ware.code = result.twr_Kod;
        ware.name = result.twr_Nazwa;
        ware.unit = result.twr_JM;
        ware.date = new Date();
        ware.twG_Kod = result.twG_Kod;
        ware.twG_Nazwa = result.twG_Nazwa;
        ware.mag_Nazwa = result.mag_Nazwa;
        ware.mag_Symbol = result.mag_Symbol;
        //this.subOrderData.subOrder.data[this.index].inProduction.ware.code = result.twr_Kod;
        //this.subOrderData.subOrder.data[this.index].inProduction.component.ware.code = result.twr_Kod;
        //update data in database
        this.inproductionService.changeMaterialType(this.subOrderData.subOrder.data[this.index].inProductionRWId, ware).subscribe(
          result => {
            console.log(result);
            this.subOrderData.subOrder.data[this.index].ware = result as Ware;
            //console.log(this.subOrderData.subOrder.data);
          });
      }
    });
  }

  //display data of the next component in the book
  nextComponent() {

    let length = this.subOrderData.subOrder.data.length;
    let index = this.index;
    //increment index
    if (index < length - 1) {
      index++;
    } else {
      index = 0;
    }
    this.index = index;
    //reload data
    this.downloadData();
    //update list index
    this.currentListIndex = this.listOfDrawingsWithSameMaterial.findIndex((item) => {
      return item.index == this.index;
    });
    //show confirm snackbar
    this._snackBar.open('Zmieniono na następny rodzaj materiału', '', this.snackBarConfig);
    //update url
    this.router.navigate(['/production/bandsaw'], {
      queryParams: {
        orderID: this.orderId,
        subOrderID: this.subOrderId,
        officeNumber: this.officenumber,
        componentNumber: this.componentnumber,
        index: this.index,
        socket: this.socketType
      }
    })
    //change status to in cut
    this.changeMaterialStatus(3);
    //update min validation value in inputs, its different for every component
    this.updateWareInputValidation();
    this.updateQtyInputValidation();
  }

  //dislpay data of the previous component in the book
  previousComponent() {

    let index = this.index;
    let length = this.subOrderData.subOrder.data.length;
    //decrement index
    if (index > 0) {
      index--;
    } else {
      index = length - 1;
    }
    
    this.index = index;
    //reload data
    this.downloadData();
    //update list index
    this.currentListIndex = this.listOfDrawingsWithSameMaterial.findIndex((item) => {
      return item.index == this.index;
    });
    //show confirm snackbar
    this._snackBar.open('Zmieniono na poprzedni rodzaj materiału', '', this.snackBarConfig);
    //update url
    this.router.navigate(['/production/bandsaw'], {
      queryParams: {
        orderID: this.orderId,
        subOrderID: this.subOrderId,
        officeNumber: this.officenumber,
        componentNumber: this.componentnumber,
        index: this.index,
        socket: this.socketType
      }
    })

    //change status to in cut
    this.changeMaterialStatus(3);
    //update min validation value in inputs, its different for every component
    this.updateWareInputValidation();
    this.updateQtyInputValidation();
  }

  //display data of the same component but from next drawing
  nextDrawing() {

    this.currentListIndex = this.listOfDrawingsWithSameMaterial.findIndex((item) => {
      let i: number =this.index;
      return item.index == i;
    })
    if (this.listOfDrawingsWithSameMaterial.length > 1) {
     //check for index validity
      if (this.currentListIndex < this.listOfDrawingsWithSameMaterial.length - 1) {
        this.currentListIndex++;
      } else {
        this.currentListIndex = 0;
      }
      //actualize component index
      this.index = this.listOfDrawingsWithSameMaterial[this.currentListIndex].index;
      this._snackBar.open("wybrano następny materiał z listy rysunków", '', this.snackBarConfig);
       //reload the page
      this.downloadData();
      this.router.navigate(['/production/bandsaw'], {
        queryParams: {
          orderID: this.orderId,
          subOrderID: this.subOrderId,
          officeNumber: this.officenumber,
          componentNumber: this.componentnumber,
          index: this.listOfDrawingsWithSameMaterial[this.currentListIndex].index,
          socket: this.socketType
        }
      })
      
      console.log(`currentListIndex ${this.currentListIndex}`, `this.index ${this.index}`);
    } else {
      this._snackBar.open("nie ma innych rysunków",'',this.snackBarConfig);
    }

    //change status to in cut
    this.changeMaterialStatus(3);
    //update min validation value in inputs, its different for every component
    this.updateWareInputValidation();
    this.updateQtyInputValidation();
  }

  //display data of the same component but from previous drawing
  previousDrawing() {

    this.currentListIndex = this.listOfDrawingsWithSameMaterial.findIndex((item) => {
      let i: number = this.index;
      return item.index == i;
    })
    if (this.listOfDrawingsWithSameMaterial.length > 1) {
      //check for index validity
      if (this.currentListIndex > 0) {
        this.currentListIndex--;
      } else {
        this.currentListIndex = this.listOfDrawingsWithSameMaterial.length - 1;
      }
      //actualize component index
      this.index = this.listOfDrawingsWithSameMaterial[this.currentListIndex].index;
      this._snackBar.open("wybrano poprzedni materiał z listy rysunków", '', this.snackBarConfig);
      //reload the page
      this.downloadData();
      this.router.navigate(['/production/bandsaw'], {
        queryParams: {
          orderID: this.orderId,
          subOrderID: this.subOrderId,
          officeNumber: this.officenumber,
          componentNumber: this.componentnumber,
          index: this.listOfDrawingsWithSameMaterial[this.currentListIndex].index,
          socket: this.socketType
        }
      })

      console.log(`currentListIndex ${this.currentListIndex}`,`this.index ${this.index}`);
    } else {
      this._snackBar.open("nie ma innych rysunków", '', this.snackBarConfig);
    }
    //change status to in cut
    this.changeMaterialStatus(3);
    //update min validation value in inputs, its different for every component
    this.updateWareInputValidation();
    this.updateQtyInputValidation();
  }

  onExit() {
      let config: MatDialogConfig = {
        width: '400px',
        data: { message: `Czy chcesz wyjść do menu głównego?` },
        disableClose: true
      }
      const dialogRef = this.dialog.open(ConfirmComponent, config);

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.router.navigate(['/production/selectionScreen'], {
            queryParams: {
              socket: this.socketType
            }
          });
        }

      });
  }
  

  onConfirmCut() {
    //count 'doneAlreadyQty'
    this.countAlreadyCutMaterial();
    console.log(this.sumlengthInput.value)
    let sendingData: dataToConfirm = {
      plannedQty: this.quantityInput.value,
      bookQty: this.setQuantityInput.value,
      QtyPerBook: this.ordersQty.value,
      alreadyDoneQty: this.doneAlreadyQty,
      totalQty: this.totalQuantityInput.value,
      totalMaterialQty: this.lengthWithOverflowInput.value,
      wareUnit: this.subOrderData.subOrder.data[this.index].wareUnit
    };

    let config: MatDialogConfig = {
      width: '500px',
      disableClose: true,
      data: sendingData
    }

    const dialogRef = this.dialog.open(BandsawCutConfirmComponent, config);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        console.log(`dialog resoult is ${result}`);
        //mark materials as cut in database
        this.changeMaterialStatus(4);
      }
      
    });
  }


  //change status of materials in database
  changeMaterialStatus(newStatus: number) {
    //let i = this.listOfDrawingsWithSameMaterial[this.currentListIndex].index;
    //dont change status to smaller number
    if (this.subOrderData.subOrder.data[this.index].inProduction.productionState.productionStateCode < newStatus) {
      //this.listOfDrawingsWithSameMaterial.forEach((item) => {
        //if (this.subOrderData.subOrder.data[this.index].inProduction.wareLength == this.subOrderData.subOrder.data[item.index].inProduction.wareLength) {
          //update local data
          this.subOrderData.subOrder.data[this.index].inProduction.productionState.productionStateCode = newStatus;
          switch (newStatus) {
            case 2:
              this.subOrderData.subOrder.data[this.index].inProduction.productionState.name = "Oczekuje";
              break;
            case 3:
              this.subOrderData.subOrder.data[this.index].inProduction.productionState.name = "Cięcie";
              break;
            case 4:
              this.subOrderData.subOrder.data[this.index].inProduction.productionState.name = "Ucięty";
              break;
          }
          //update data on server
          this.inproductionService.changeProductionState(newStatus, this.subOrderData.subOrder.data[this.index].inProduction.inProductionId)
            .subscribe(() => console.log(`statue change to ${newStatus}`));



        //}
     // })
    } else if (this.subOrderData.subOrder.data[this.index].inProduction.productionState.productionStateCode > newStatus){
      console.log('cant change status to previous one');
    }

  }


  updateWareInputValidation() {
    this.lengthWithOverflowInput.clearValidators();
    this.lengthWithOverflowInput.setValidators([Validators.required, Validators.min(this.subOrderData.subOrder.data[this.index].toIssue * this.subOrderData.subOrder.data[this.index].inProduction.wareLength)]);
    this.lengthWithOverflowInput.updateValueAndValidity();
  }

  updateQtyInputValidation() {
    this.totalQuantityInput.clearValidators();
    this.totalQuantityInput.setValidators([Validators.required, Validators.min(this.subOrderData.subOrder.data[this.index].inProduction.plannedQty)]);
    this.totalQuantityInput.updateValueAndValidity();
  }

  onOrderListClick(index) {
    if (this.currentListItem2 != index) {
      let config: MatDialogConfig = {
        width: '400px',
        data: { message: `Czy na pewno chcesz przejść do innego zlecenia?` },
        disableClose: true
      }
      const dialogRef = this.dialog.open(ConfirmComponent, config);

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          //console.log('urwało Ci dupe');
          this.goToAnotherOrder(index);
        }

      });
    }

  }


  fetchDatafromUrl() {
    this.orderId = this.route.snapshot.queryParams['orderID'];
    this.subOrderId = this.route.snapshot.queryParams['subOrderID'];
    this.officenumber = this.route.snapshot.queryParams['officeNumber'];
    this.componentnumber = this.route.snapshot.queryParams['componentNumber'];
    this.index = parseInt(this.route.snapshot.queryParams['index']);
    this.socketType = this.route.snapshot.queryParams['socket'];

    if (this.socketType == "saw") {
      this.sourceType = 0;
    } else if (this.socketType == "plasma") {
      this.sourceType = 2;
    }
  }

  //onMaterialChange() {
  //  let config: MatDialogConfig = {
  //    disableClose: true,
  //    autoFocus: true,
  //    width: '25vw',
  //    data: { option: 'material' }
  //  }
  //  const dialogRef = this.dialog.open(ChangeWithAutocompleteComponent, config);


  //  dialogRef.afterClosed().subscribe(result => {
  //    console.log(`Dialog result: ${result}`);
  //  });
  //}


}

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}


