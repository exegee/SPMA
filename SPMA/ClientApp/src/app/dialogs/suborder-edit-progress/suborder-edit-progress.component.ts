import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
import { OrderBook } from '../../models/orders/orderbook.model';

@Component({
  selector: 'app-suborder-edit-progress',
  templateUrl: './suborder-edit-progress.component.html',
  styleUrls: ['./suborder-edit-progress.component.css']
})
export class SuborderEditProgressComponent implements OnInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) public orderBook: OrderBook[]
  ) { }

  ngOnInit() {
  }

}
