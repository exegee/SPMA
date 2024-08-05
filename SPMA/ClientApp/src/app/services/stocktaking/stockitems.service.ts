import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs/operators";
import { PagedStockItem } from "../../models/stocktaking/pagedstockitem.model";
import { StockItem } from "../../models/stocktaking/stockitem.model";

@Injectable()
export class StockItemsService {

  constructor(private http: HttpClient) { }


  getStockItems(orderBy: string, sortOrder: string, page: number, pageSize: number, filter: string) {
    return this.http.get('/api/stockitems', {
      params: new HttpParams()
        .set('orderBy', orderBy)
        .set('sortOrder', sortOrder)
        .set('page', page.toString())
        .set('pageSize', pageSize.toString())
        .set('filter', filter)
    })
      .pipe(
        map((res: PagedStockItem) => {
          return res;
        }))
  }

  updateStockItemQty(stockItem: StockItem) {
    return this.http.patch('/api/stockitems', stockItem);
  }

  getStockItemsCount() {
    return this.http.get<number>('/api/stockitems/count');
  }

  deleteStockItems() {
    return this.http.delete('api/stockitems');
  }

  exportStockItems() {
    return this.http.get('api/stockitems/export', { responseType: 'text' });
  }

  addStockItem(stockItem: StockItem) {
    return this.http.post('api/stockitems', stockItem);
  }

  deleteStockItem(stockItem: StockItem) {
    return this.http.delete(`api/stockitems/${stockItem.stockItemId}`);
  }
}
