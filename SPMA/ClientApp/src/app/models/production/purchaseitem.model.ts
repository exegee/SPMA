export class PurchaseItem {
  public number: string;
  public name: string;
  public comment: string;

  constructor(number: string = null, name: string = null, comment: string = null) {
    this.number = number;
    this.name = name;
    this.comment = comment;
  }
}
