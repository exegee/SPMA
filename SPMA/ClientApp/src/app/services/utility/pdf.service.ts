import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Book } from '../../models/books/book.model';
import { Order } from '../../models/orders/order.model';
import { OrderBook } from '../../models/orders/orderbook.model';

@Injectable()
export class PdfService
{
  constructor(private http: HttpClient) { }

  generateBookRw(book: Book, order: Order, genZD: boolean, listType: number) {
    return this.http.get(`api/pdf/book/rw/${book.bookId}`,
      {
        responseType: 'arraybuffer',
        headers: new HttpHeaders({
          'orderNumber': order.number,
          'subOrderNumber': book.number,
          'genZD': String(genZD),
          'listType': String(listType)
        })
      });
  }


  // Generates BOM list
  generateBomList(orderBook: OrderBook, genZD: boolean, listType: number) {
    return this.http.get(`api/pdf/book/parts/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        },
        responseType: 'arraybuffer'
        //,
        //headers: new HttpHeaders({
        //  'orderNumber': orderBook.order.number,
        //  'subOrderNumber': orderBook.number,
        //  'genZD': String(genZD),
        //  'listType': String(listType)
        //})
      });
  }

  // Generates wares list to cut
  generateWareList(orderBook: OrderBook, genZD: boolean, listType: number) {
    return this.http.get(`api/pdf/book/bom/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        },
        responseType: 'arraybuffer'
      });
  }

  // Generates plazma internal cut list
  generatePlazmaInternalCutList(orderBook: OrderBook, genZD: boolean, listType: number) {
    return this.http.get(`api/pdf/book/bom/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        },
        responseType: 'arraybuffer'

      });
  }

  // Generates plazma external cut list
  generatePlazmaExternalCutList(orderBook: OrderBook, genZD: boolean, listType: number) {
    return this.http.get(`api/pdf/book/bom/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        },
        responseType: 'arraybuffer'

      });
  }

  generateOrderList(orders: Order[]) {
    console.log(orders);
    return this.http.post(`api/pdf/order/orderslist`, orders, {
      responseType: 'arraybuffer'
    });
  }

  generateOrderDetailsList(orderId: number) {
    return this.http.get(`api/pdf/order/${orderId}/details`, {
      responseType: 'arraybuffer'
    });
  }

  generateSubOrderList(orderBooks: OrderBook[]) {
    console.log(orderBooks);
    return this.http.post(`api/pdf/order/suborderslist`, orderBooks, {
      responseType: 'arraybuffer'
    });
  }
}
