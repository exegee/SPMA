import { Component, OnInit } from '@angular/core';
import { Order } from '../../models/orders/order.model';
import { OrdersService } from '../../services/orders/orders.service';

@Component({
  selector: 'app-orders-archive',
  templateUrl: './orders-archive.component.html',
  styleUrls: ['./orders-archive.component.css']
})
export class OrdersArchiveComponent implements OnInit {

  public orders: Order[];

  constructor(private ordersService: OrdersService) { }
 

  ngOnInit() {
    this.ordersService.getArchiveOrders().subscribe((orders: Order[]) => {
      this.orders = orders;
      console.log(this.orders);
    });
  }
}
