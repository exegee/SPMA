import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SubOrderItem } from '../../models/suborders/suborderitem.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { Observable } from 'rxjs';
import { Order } from '../../models/orders/order.model';


@Injectable()
export class SubOrderService {
  constructor(private http: HttpClient) { }

  getSubOrderById(id: number) {
    return this.http.get<any>(`/api/suborder/${id}`);
  }

  async getSubOrderByIdAsync(id: number): Promise<SubOrderItem[]> {
    return await this.http.get<SubOrderItem[]>(`/api/suborder/${id}`).toPromise();
  }

  //get filtered subOrder items for cutting
  async getSubOrderByIdAsyncForCut(id: number, sourceType: number): Promise<SubOrderItem[]> {
    return await this.http.get<SubOrderItem[]>(`/api/suborder/bandsaw/${id}`, { params: { sourceType: sourceType.toString() }}).toPromise();
  }

  async getSubOrderByIdInfo(id: number): Promise<OrderBook> {
    return await this.http.get<OrderBook>(`/api/suborder/${id}/info`).toPromise();
  }

  async updateSubOrderItem(subOrderItem: SubOrderItem[]): Promise<boolean> {
    return await this.http.patch<boolean>('/api/suborder', subOrderItem).toPromise();
  }

  getComponentIndex(orderbookId: number, wareid: number, sourceType: number) {
    return this.http.get('/api/suborder/getcomponentindex', {
      params: {
        id: orderbookId.toString(),
        wareId: wareid.toString(),
        sourceType: sourceType.toString()
      }
    });
  }

  public getCutStatus(orderBookId: number) {
    return this.http.get('/api/suborder/getcutstatus',
      {
        params: {
          orderBookId: orderBookId.toString()
        }
      });
  }

  public getCutStatuses(orderId: number) {
    return this.http.get('/api/suborder/getcutstatuses', {
      params: {
        orderId: orderId.toString()
      }
    });
  }

}
