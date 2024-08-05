import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { Order } from '../../models/orders/order.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { PagedOrder } from '../../models/orders/pagedorder.model';

export interface newOderNumber {
  value: string;
}


@Injectable()
export class OrdersService {

  constructor(private http: HttpClient) { }

  getOrderRWStatus(id: number) {
    const url = `/api/orders/${id}/statusrw`;
    return this.http.get(url);
  }

  getOrders(orderBy: string, sortOrder: string, orderState: number, page: number, pageSize: number, filterByName: string, filterByNumber: string, filterByClientName: string) {
    return this.http.get('/api/orders', {
      params: new HttpParams()
        .set('orderBy', orderBy)
        .set('sortOrder', sortOrder)
        .set('orderState', orderState.toString())
        .set('page', page.toString())
        .set('pageSize', pageSize.toString())
        .set('filterByName', filterByName)
        .set('filterByNumber', filterByNumber)
        .set('filterByClientName', filterByClientName)
    })
      .pipe(
        map((res: PagedOrder) => {
          return res;
        })
      )
  }

  getFilteredOrders(filter: string) {
    return this.http.get<Order[]>('/api/orders/filteredorders', {
      params: new HttpParams()
        .set('filter', filter)
    })
  }



  deleteOrder(id: string) {
    const url = `/api/orders/${id}`;
    return this.http.delete(url);
  }

  createOrder(order: Order) {
    return this.http.post('/api/orders', order);
  }

  genOrderNumber(type: number, orderYear: any) {
    return this.http.get<newOderNumber>(`/api/orders/generatenumber/${type}`, {
      params: new HttpParams()
        .set('orderYear', orderYear)
    });
  }

  addBook(orderBook: OrderBook) {
    return this.http.post('/api/orders/book', orderBook);
  }

  getOrderBooks(id: number) {
    return this.http.get(`/api/orders/${id}/books`);
  }

  async getOrderBooksAsync(id: number): Promise<any> {
    return await this.http.get(`/api/orders/${id}/books`).toPromise();
  }

  async getOrderAsync(id: number): Promise<any> {
    return await this.http.get(`/api/orders/async/${id}`).toPromise();
  }

  getOrder(id: number) {
    return this.http.get(`/api/orders/${id}`);
  }
  getOrder2(id: number) {
    return this.http.get(`/api/orders/${id}`, { responseType: 'blob' as 'json' });
  }

  async deleteBook(orderBook: OrderBook) {
    return await this.http.delete(`api/orders/book`,
      {
        headers: new HttpHeaders({
          'orderId': orderBook.order.orderId.toString(),
          'bookId': orderBook.book.bookId.toString(),
          'subOrderNumber': orderBook.number
        }),
        reportProgress: true,
        observe: "response"
      }).toPromise();
  }

  searchOrder(filter: string) {
    return this.http.get('api/orders/searchorder', {
      params: new HttpParams()
        .set('filter', filter)
    })
      .pipe(
        map((res: Order[]) => {
          return res;
        })
      )
  }


  archiveOrder(id: string, state: string) {
    const url = `/api/orders/archive/`;
    return this.http.patch(url, null,
      {
        headers: new HttpHeaders({
          'orderId': id,
          'orderState': state
        })
      });
  }

  getArchiveOrders() {
    return this.http.get(`/api/orders/archive`);
  }

  updateOrder(order: Order) {
    return this.http.put(`api/orders/${order.orderId}`, order);
  }

  copyOrder(existingOneId: number, newOne: Order) {
    return this.http.post(`api/orders/copyorder/${existingOneId}`, newOne);
  }

  copyOrderParts(existingOrder: number, orderBooks: OrderBook[]) {
    return this.http.post(`api/orders/copyorderpart/${existingOrder}`, orderBooks);
  }

}
