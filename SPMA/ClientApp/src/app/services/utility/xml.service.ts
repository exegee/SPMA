import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OrderBook } from '../../models/orders/orderbook.model';
import { InProductionXML } from '../../models/production/inproductionxml.model';

@Injectable()
export class XmlService {
  constructor(private http: HttpClient) { }

  generateWareList(orderBook: OrderBook, listType: number) {
    return this.http.get(`api/xml/book/bom/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        },
        responseType: 'text'
      });
  }

  exportWareListToXML(wareList: InProductionXML[], orderBook: OrderBook, updateQuantities: boolean, rwDate: string, rwType: string, magType: string) {
    return this.http.post('api/xml/book/getwarelistxml', wareList,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          componentNumber: orderBook.componentNumber,
          updateQuantitiesStr: String(updateQuantities),
          rwDate: rwDate,
          rwType: rwType,
          magType: magType
        },
        responseType: 'text'
      });
  }

  exportMultipleWareListToXML(wareList: InProductionXML[], orderNumber: string, updateQuantities: boolean, rwDate: string, rwType: string, magType: string, subOrdersnumbers: string[],componentnumbers:string[]) {
    return this.http.post('api/xml/book/getmultiplewarelistxml', wareList,
      {
        params: {
          orderNumber: orderNumber,
          subOrderNumbers: String(subOrdersnumbers),
          componentNumbers: String(componentnumbers),
          updateQuantitiesStr: String(updateQuantities),
          rwDate: rwDate,
          rwType: rwType,
          magType: magType
        },
        responseType: 'text'
      });
  }

  getWareList(orderBook: OrderBook, listType: number) {
    return this.http.get<InProductionXML[]>(`api/xml/book/getwares/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        }
      });
  }

  async getWareListAsync(orderBook: OrderBook, listType: number): Promise<InProductionXML[]> {
    return await this.http.get<InProductionXML[]>(`api/xml/book/getwares/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        }
      }).toPromise();
  }

  async getWareSummaryListAsync(orderNumber: string, listType: number, itemsList: string[]) {
    return await this.http.get<InProductionXML[]>(`api/xml/book/getwaressummary/`,
      {
        params: {
          orderNumber: orderNumber,
          listType: String(listType),
          itemsList: String(itemsList)
        }
      }).toPromise();
  }

  async getPlazmaInternalCutListAsync(orderBook: OrderBook, listType: number): Promise<InProductionXML[]> {
    return await this.http.get<InProductionXML[]>(`api/xml/book/getplazmainternalcut/${orderBook.book.bookId}`,
      {
        params: {
          orderNumber: orderBook.order.number,
          subOrderNumber: orderBook.number,
          listType: String(listType)
        }
      }).toPromise();
  }


}
