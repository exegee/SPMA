export class ProductionSocket {
  public productionSocketId: number;
  public name: string;
  public comment: string;

  constructor(productionSocketId: number, name: string, comment: string) {
    this.productionSocketId = productionSocketId;
    this.name = name;
    this.comment = comment;
  }
}
