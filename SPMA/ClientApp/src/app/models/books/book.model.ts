export class Book {
  public bookId: number;
  public officeNumber: string;
  public name: string;
  public type: number;
  public comment: string;
  public author: string;
  public addedby: string;
  public componentNumber: string;
  public modifiedDate: Date = new Date();
  public quantity: number;
  public number: string;

  constructor(componentNumber: string = null, officeNumber: string = null, quantity: number = 1, bookId: number = 0, name: string = null, type: number = 0
    , comment: string = null, author: string = null, addedby: string = null, number: string = null,
    modifiedDate: Date = null) {
    this.bookId = bookId;
    this.officeNumber = officeNumber;
    this.name = name;
    this.type = type;
    this.comment = comment;
    this.author = author;
    this.addedby = addedby;
    this.modifiedDate = modifiedDate;
    this.componentNumber = componentNumber;
    this.quantity = quantity;
    this.number = number;
  }
}
