import { StockItem } from "./stockitem.model";


export class PagedStockItem {
  public results: StockItem[];
  public rowCount: number;

  constructor(results: StockItem[] = [], rowCount: number = 0) {
    results = this.results;
    rowCount = this.rowCount;
  }
}
