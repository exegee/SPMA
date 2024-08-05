import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Order } from 'src/app/models/orders/order.model';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import * as moment from 'moment';
import { OrdersService } from '../../services/orders/orders.service';
import { OptimaClient } from '../../models/optima/optimaclient.model';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { OptimaService } from '../../services/optima/optima.service';
import { Observable } from 'rxjs';

export interface orderType {
  value: number;
  viewValue: string;
}



@Component({
  selector: 'app-new-order',
  templateUrl: './new-order.component.html',
  styleUrls: ['./new-order.component.css']
})
export class NewOrderComponent implements OnInit {

  form: FormGroup;
  description: string;
  initialQty: number;
  orderType: number;
  orderTypes: orderType[];
  searchOptimaClientsInputValue: string;
  isLoadingClients: boolean;
  filteredOptimaClients: Observable<OptimaClient[]>;
  selectedClient: OptimaClient;

  constructor(
    private dialogRef: MatDialogRef<NewOrderComponent>,
    @Inject(MAT_DIALOG_DATA) data,
    private orderService: OrdersService,
    private optimaService: OptimaService) {

    this.description = data.title;
    this.initialQty = data.initialQty;
    data.orderType != null ? this.orderType = data.orderType : this.orderType = 0;//Domyślnie komercyjne
    //this.orderType = data.orderType;
  }

  ngOnInit() {
    this.form = new FormGroup({
      'number': new FormControl(null, Validators.required),
      'name': new FormControl(null, Validators.required),
      'type': new FormControl(this.orderType, Validators.required),
      'plannedQty': new FormControl(this.initialQty, Validators.required),
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
    this.dialogRef.close();
  }

  onCreate() {
    var order = new Order();
    order.state = 0;
    order.type = this.form.get("type").value;
    order.clientName = this.selectedClient.knt_Nazwa1;
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
    //console.log(order);
    //console.log(this.form.value as Order);
    this.dialogRef.close(order);
  }

  onGeneraterOrderNumber() {
    var orderType = this.form.get('type').value;
    var orderDate = this.form.get('orderDate').value as moment.Moment;
    var orderYear = orderDate.toDate().getFullYear();
    this.orderService.genOrderNumber(orderType, orderYear).subscribe(
      data => {
        this.form.patchValue({
          number: data.value
        });
      });
  }

  onOrderTypeChange() {
    this.onGeneraterOrderNumber();
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
    if (item) { return item.knt_Nazwa1; }
  }
}
