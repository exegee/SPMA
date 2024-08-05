import { Order } from './order.model';

export class PagedOrder {
  public results: Order[];
  public rowCount: number;

  constructor(results: Order[] = [], rowCount: number = 0) {
    results = this.results;
    rowCount = this.rowCount;
  }
}
