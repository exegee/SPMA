export class StockItem {
  public stockItemId: number;
  public code: string;
  public name: string;
  public type: number;
  public pitQty: number;
  public actualQty: number;
  public diffQty: number;
  public unit: string;
  public comment: string;
  public dateAdded: Date;


  constructor(stockItemId: number = 0, code: string = null, name: string = null, type: number = 0, pitQty: number = 0, actualQty: number = 0,
    diffQty: number = 0, unit: string = '', comment: string = '', dateAdded: Date = null) {
    this.stockItemId = stockItemId;
    this.code = code;
    this.name = name;
    this.type = type;
    this.pitQty = pitQty;
    this.actualQty = actualQty;
    this.diffQty = diffQty;
    this.unit = unit;
    this.comment = comment;
    this.dateAdded = dateAdded;
  }

}
