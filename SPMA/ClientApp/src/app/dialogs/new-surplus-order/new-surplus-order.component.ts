import { validateHorizontalPosition } from '@angular/cdk/overlay';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { ErrorStateMatcher, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { debounceTime, filter, finalize, skipWhile, switchMap, tap } from 'rxjs/operators';
import { OptimaItem } from '../../models/optima/optimaitem.model';
import { OptimaMag } from '../../models/optima/optimamag.model';
import { InventorComponent } from '../../models/production/inventorcomponent.model';
import { WarehouseItem } from '../../models/production/warehouseItem.model';
import { Ware } from '../../models/warehouse/ware.model';
import { ComponentService } from '../../services/components/component.service';
import { OptimaService } from '../../services/optima/optima.service';


export class MyErrorStateMatcher_Text implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    //var res = !!(control && control.invalid && isSubmitted && control.hasError);
    //console.log(res);
    return !!(control && control.invalid && (control.hasError && (control.dirty || control.touched)));//control.value.length!=0
  }
}

export class MyErrorStateMucher_Number implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    //var res = !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
    //console.log(res);
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));//control.value.length!=0
  }
}

interface dialogData {
  "title": string,
  "mode": string,
  "surplusOrderItem": WarehouseItem
}

@Component({
  selector: 'app-new-surplus-order',
  templateUrl: './new-surplus-order.component.html',
  styleUrls: ['./new-surplus-order.component.css']
})
export class NewSurplusOrderComponent implements OnInit {

  matcher_text = new MyErrorStateMatcher_Text();
  matcher_number = new MyErrorStateMucher_Number();
  form: FormGroup;
  surplusOrderItem: WarehouseItem = new WarehouseItem();
  isComponentLoading: boolean = false;
  isWareItemLoading: boolean = false;
  filtereComponentItems: InventorComponent[] = [];
  filteredOptimaItems: OptimaItem[] = [];
  optimaMags: OptimaMag[] = [];
  //selectedMag: OptimaMag;
  selectedMag: OptimaMag = new OptimaMag(0, "WSZYSTKIE", "WSZYSTKIE");



  constructor(
    public dialogRef: MatDialogRef<NewSurplusOrderComponent>, @Inject(MAT_DIALOG_DATA) public data: dialogData,
    private componentService: ComponentService, private optimaService: OptimaService) { }


  ngOnInit() {
    this.form = new FormGroup({
      'componentNumber': new FormControl(null, { validators: [Validators.required], updateOn: 'change' }),
      'materialCode': new FormControl({value: null, disabled: true}, Validators.required),
      'plannedQty': new FormControl({ value: null, disabled: true }, [Validators.required, Validators.min(1),Validators.max(999), Validators.pattern('([1-9]|[1-9]\\d{1,2})')]),
      'addedBy': new FormControl(null),
      'comment': new FormControl(null),
      'sumQty': new FormControl(0),
      'usedQty': new FormControl(0, Validators.pattern('[1234567890.]'))
    });

    //this.getOptimaMags();

    this.form.get('componentNumber').valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isComponentLoading = true;
          }
        }
        ),
        switchMap(value => value.length >= 3 ? this.componentService.getFilteredComponents(value).pipe(finalize(() => {
          this.isComponentLoading = false

        })) : []),
        finalize(() => {
          this.isComponentLoading = false
        })
      ).subscribe(items => {
        this.filtereComponentItems = items;
        //console.log(items);
      }, () => { });

    this.form.get('materialCode').valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isWareItemLoading = true;
          }
        }
        ),
        switchMap(value => value.length >= 3 ? this.optimaService.searchItem(value, this.selectedMag).pipe(finalize(() => {
          this.isWareItemLoading = false

        })) : []),
        finalize(() => {
          this.isWareItemLoading = false
        })
    ).subscribe(items => {
      this.filteredOptimaItems = [];
        this.filteredOptimaItems = items;
      }, () => { });

    this.form.get('plannedQty').valueChanges
      .pipe(filter(() => this.form.get('plannedQty').valid))
      .subscribe((value:number) => {

        this.surplusOrderItem.componentQty = value;
        this.form.get('sumQty').setValue(Math.round((value * this.surplusOrderItem.wareQty) * 100) / 100);
        this.form.get('usedQty').setValue(Math.round((value * this.surplusOrderItem.wareQty) * 100) / 100);
        this.updateValidation(Math.round((value * this.surplusOrderItem.wareQty) * 100) / 100);
        //console.log(this.surplusOrderItem.wareQty);
      });

    this.form.get('usedQty').valueChanges
      .pipe(filter(() => this.form.get('usedQty').valid))
      .subscribe(value => {
        //if ((this.form.get('usedQty').value*100) >= (this.form.get('plannedQty').value*100)) {
          this.surplusOrderItem.wareQtySum = this.form.get('usedQty').value;
        //console.log(this.form.get('usedQty').valid);
        //} else {
        //  this.form.get('usedQty').setErrors({ "invalidValue": true });
        //  console.log(this.form.get('usedQty').valid)
        //} 
      });


    this.setDefoultData();

  }


  searchComponentDisplay(item: InventorComponent) {
    if (item) { return item.number;}  //`${item.number} - ${item.name}`
  }

  searchOptimaWarehouseDisplay(item: OptimaItem) {
    if (item) { return item.twr_Kod; }
  }

  onSelectComponentItem(item: InventorComponent) {

    //check if componentNumber input is filled properly
    this.surplusOrderItem.component = item;
    this.surplusOrderItem.wareQty = item.wareLength;
    this.form.get('materialCode').enable();
    this.form.get('plannedQty').enable();
      //console.log( this.form.get('materialCode').enabled);
    //add defaultll options to materialCode Input
    this.filtereComponentItems = [];
    this.filtereComponentItems.push(item);
    if (item.ware) {
      //this.surplusOrderItem.ware = item.ware;
      this.optimaService.searchItem(item.ware.code, this.selectedMag).subscribe(
        (res: OptimaItem[]) => {
          if (res.length > 0) {
            this.filteredOptimaItems = [];
            this.filteredOptimaItems = res;
            this.form.get('materialCode').setValue(this.filteredOptimaItems[0]);
            this.surplusOrderItem.ware = {
              wareId: null,
              code: this.filteredOptimaItems[0].twr_Kod,
              name: this.filteredOptimaItems[0].twr_Nazwa,
              quantity: Math.round(this.filteredOptimaItems[0].twI_Ilosc),
              length: 0,
              unit: this.filteredOptimaItems[0].twr_JM,
              converter: 0,
              date: null,
              twG_Kod: this.filteredOptimaItems[0].twG_Kod,
              twG_Nazwa: this.filteredOptimaItems[0].twG_Nazwa,
              mag_Nazwa: this.filteredOptimaItems[0].mag_Nazwa,
              mag_Symbol: this.filteredOptimaItems[0].mag_Symbol
            };
            console.log(this.filteredOptimaItems);
          }
        }
      );
    }
    if (this.form.get('plannedQty').enabled && this.form.get('plannedQty').value >= 0) {
      let value: number = Math.round((this.form.get('plannedQty').value * this.surplusOrderItem.wareQty) * 100) / 100;
      this.form.get('sumQty').setValue(value);
      this.form.get('usedQty').setValue(value);
      this.updateValidation(value);
    }
  }

  onSelectMaterialItem(item: OptimaItem) {
    //check if materialCode input is filled properly
   
    var optimaItem: OptimaItem = item;
      console.log(optimaItem);
      this.surplusOrderItem.ware = {
        wareId: null,
        code: optimaItem.twr_Kod,
        name: optimaItem.twr_Nazwa,
        quantity: Math.round(optimaItem.twI_Ilosc),
        length: 0,
        unit: optimaItem.twr_JM,
        converter: 0,
        date: null,
        twG_Kod: optimaItem.twG_Kod,
        twG_Nazwa: optimaItem.twG_Nazwa,
        mag_Nazwa: optimaItem.mag_Nazwa,
        mag_Symbol: optimaItem.mag_Symbol
      };
      //console.log(this.surplusOrderItem);
      this.form.get('plannedQty').enable();

    this.filteredOptimaItems = [];
    this.filteredOptimaItems.push(item);
  }

  
  onSubmit(): void {
    var anyError: boolean = false;

    if (this.data.mode == 'new') {
      //check if componentNumber input is filled properly
      var index: number = this.filtereComponentItems.findIndex(item => item.number == this.form.get('componentNumber').value.number);
      if (this.form.get('componentNumber').valid && index != -1) {
        //this.form.get('componentNumber').setErrors({ "invalidValue": false });
      }
      else {
        console.warn("Lack of order number");
        anyError = true;
        this.form.get('componentNumber').setErrors({ "invalidValue": true });
      }


      //check if materialCode input is filled properly
      index = this.filteredOptimaItems.findIndex(item => item.twr_Kod == this.form.get('materialCode').value.twr_Kod);
      if (this.form.get('materialCode').valid && index != -1) {
        //this.form.get('materialCode').setErrors({ "invalidValue": false });
      }
      else {
        console.warn("Lack of ware code");
        anyError = true;
        this.form.get('materialCode').setErrors({ "invalidValue": true });
      }
    }
    this.filteredOptimaItems = [];
    this.filteredOptimaItems.push(this.form.get('materialCode').value);

    if (!anyError) {
      this.surplusOrderItem.wareQtySum = this.form.get('usedQty').value;
      this.surplusOrderItem.addedDate = new Date();
      this.surplusOrderItem.addedBy = this.form.get('addedBy').value;
      this.surplusOrderItem.comment = this.form.get('comment').value;
      this.surplusOrderItem.state = 1;

      this.dialogRef.close(this.surplusOrderItem);
      console.log(this.surplusOrderItem);


    }

  }

  updateValidation(value: number) {
    this.form.get('usedQty').clearValidators();
    this.form.get('usedQty').setValidators([Validators.required, Validators.min(value), Validators.pattern('[1234567890.]+')]);
    this.form.get('usedQty').updateValueAndValidity();
  }

  setDefoultData() {
    if (this.data.mode == "edit") {
      this.surplusOrderItem = this.data.surplusOrderItem;
      this.form.get('componentNumber').setValue(this.surplusOrderItem.component.number);
      this.form.get('materialCode').setValue(this.surplusOrderItem.ware.code);
      this.form.get('plannedQty').setValue(this.surplusOrderItem.componentQty);
      this.form.get('addedBy').setValue(this.surplusOrderItem.addedBy);
      this.form.get('comment').setValue(this.surplusOrderItem.comment);
      let sumQty: number = this.surplusOrderItem.component.wareLength * this.surplusOrderItem.componentQty;
      this.form.get('sumQty').setValue(sumQty);
      this.form.get('usedQty').setValue(sumQty);

      this.form.get('materialCode').enable();
      this.form.get('plannedQty').enable();
      this.updateValidation(sumQty);
    }
  }


  changeNumberPlaceholder(): string {
    if (this.data.mode == 'edit')
      return this.surplusOrderItem.component.number
    else
      return "Numer części"
  }

  changeCodePlaceholder(): string {
    if (this.data.mode == 'edit')
      return this.surplusOrderItem.ware.code
    else
      return "Kod materiału"
  }
  //// Get Optima warehouses
  //async getOptimaMags() {
  //  return await this.optimaService.getMags(true).then(
  //    (data: OptimaMag[]) => {
  //      this.optimaMags = data.slice(1);
  //    }).finally(() => {
  //      if (this.optimaMags != null) {
  //        this.selectedMag = this.optimaMags[1];
  //      }
  //    });
  //}

}
