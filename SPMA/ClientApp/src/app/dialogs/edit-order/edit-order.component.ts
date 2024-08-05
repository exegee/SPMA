import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatAutocompleteDefaultOptions, MatAutocompleteTrigger, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import * as moment from 'moment';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { OptimaClient } from '../../models/optima/optimaclient.model';
import { Order } from '../../models/orders/order.model';
import { OptimaService } from '../../services/optima/optima.service';
import { OrdersService } from '../../services/orders/orders.service';

export interface orderType {
  value: number;
  viewValue: string;
}

@Component({
  selector: 'app-edit-order',
  templateUrl: './edit-order.component.html',
  styleUrls: ['./edit-order.component.css']
})
export class EditOrderComponent implements OnInit {

  plcholder: string = "";
  form: FormGroup;
  auto: MatAutocompleteTrigger;
  description: string;
  orderId: number;
  order: Order;
  orderTypes: orderType[];
  searchOptimaClientsInputValue: string;
  isLoadingClients: boolean;
  filteredOptimaClients: BehaviorSubject<OptimaClient[]>;
  selectedClient: OptimaClient;

  

  constructor(
    private dialogRef: MatDialogRef<EditOrderComponent>,
    @Inject(MAT_DIALOG_DATA) data,
    private orderService: OrdersService,
    private optimaService: OptimaService) {

    this.description = data.title;
    this.orderId = data.id
  }

  ngOnInit() {
   

    this.form = new FormGroup({
      'number': new FormControl(null),
      'name': new FormControl(null, Validators.required),
      'type': new FormControl(null),
      'plannedQty': new FormControl(1, Validators.required),
      'clientName': new FormControl(null),
      'orderDate': new FormControl(moment()),
      'requiredDate': new FormControl(null),
      'shippingAddress': new FormControl(null),
      'shippingCity': new FormControl(null),
      'shippingPostalCode': new FormControl(null),
      'shippingRegion': new FormControl(null),
      'shippingCountry': new FormControl(null),
      'comment': new FormControl(null),
    });
    this.getOrder(this.orderId);

    this.form.get("clientName").valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isLoadingClients = true;
          }
        }
        ),
        switchMap(value => value.length >= 3 ? this.optimaService.searchClient(value).pipe(finalize(() => {
          this.isLoadingClients = false;
        })) : []),
        finalize(() => {
          this.isLoadingClients = false;
        })
      ).subscribe(items => {
        this.filteredOptimaClients = items;
        //console.log(this.filteredOptimaClients);

      }, () => { });


    // 0 - External, 1 - Warranty, 2 - Internal
    this.orderTypes = [
      { value: 0, viewValue: 'Komercyjne' },
      { value: 1, viewValue: 'Reklamacyjne' },
      { value: 2, viewValue: 'Wewnętrzne' }]


    
  }

  onCancel(): void {
    //console.log(this.orderId);
    this.dialogRef.close();
  }

  onUpdate() {
    var order = new Order();
    order.orderId = this.orderId;
    order.type = this.orderTypes.find(x => [this.form.get("type").value]).value;

    if (this.selectedClient != null) {
      order.clientName = this.selectedClient.knt_Nazwa1;
    }
    else {
      order.clientName = this.order.clientName;
    }
    order.state = this.order.state;
    order.comment = this.form.get("comment").value;
    order.name = this.form.get("name").value;
    order.number = this.form.get("number").value;
    order.orderDate = this.form.get("orderDate").value;
    order.plannedQty = this.form.get("plannedQty").value;
    order.requiredDate = this.form.get("requiredDate").value;
    order.shippingAddress = this.form.get("shippingAddress").value;
    order.shippingCity = this.form.get("shippingCity").value;
    order.shippingPostalCode = this.form.get("shippingPostalCode").value;
    order.shippingRegion = this.form.get("shippingRegion").value;
    order.shippingCountry = this.form.get("shippingCountry").value;
    //console.log(this.form.value as Order);
    this.dialogRef.close(order);
  }

  getOrder(id: number) {
    this.orderService.getOrderAsync(id).then(
      (data: Order) => {
        this.order = data;
      }).finally(() => {
        this.updateFormControls();
      }
    );
  }

  updateFormControls() {
    this.form.patchValue(this.order);
    this.plcholder = this.order.clientName;
    this.form.get("type").setValue(this.orderTypes[this.order.type].viewValue);
    //console.log(this.order.clientName)
    //this.optimaService.searchClient("Bilka").subscribe((client) => {
    //  console.log(client);
     
    //  console.log(this.selectedClient);
/*    })*/
    //this.filteredOptimaClients = new BehaviorSubject<OptimaClient[]>([new OptimaClient(4, "asda", "asd")]);
  }

  onSelectedOptimaClient(event) {
    this.selectedClient = event.option.value;
    this.form.get("shippingAddress").setValue(this.selectedClient.knt_Ulica);
    this.form.get("shippingCity").setValue(this.selectedClient.knt_Miasto);
    this.form.get("shippingPostalCode").setValue(this.selectedClient.knt_KodPocztowy);
    this.form.get("shippingRegion").setValue(this.selectedClient.knt_Wojewodztwo);
    this.form.get("shippingCountry").setValue(this.selectedClient.knt_Kraj);
  }

  searchClientDisplay(item: OptimaClient) {
    if (item) { { return item.knt_Nazwa1; }
  }
  }
  onClientNameFocus() {
    this.plcholder = "Zamawiający";
  }

  onClientNameFocusOut() {
    console.log("focus out");
    if (this.selectedClient == null) {
      this.plcholder = this.order.clientName;
    }
  }

  onOptionActivated(event) {
    console.log(event);
  }
}
