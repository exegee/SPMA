export class ProductionState {
  public productionStateId: number;
  public name: string;
  public comment: string;
  public productionStateCode: number;

  constructor(productionStateId: number, name: string, comment: string, productionStateCode: number) {
    this.productionStateId = productionStateId;
    this.name = name;
    this.comment = comment;
    this.productionStateCode = productionStateCode;
  }
}
