import { Order } from '../orders/order.model';
import { Book } from '../books/book.model';

export class OrderBook {
  public orderBookId: number;
  public orderId: number;
  public order: Order;
  public bookId: number;
  public book: Book;
  public plannedQty: number;
  public finishedQty: number;
  public addedDate: Date = new Date();
  public comment: string;
  public addedBy: number;
  public orderNumber: string;
  public componentNumber: string;
  public position: number;
  public type: number;
  public officeNumber: string;
  public number: string;
  public wareList: number;
  public purchaseList: number;
  public plasmaInList: number;
  public plasmaOutList: number;
  public isExpanded: boolean; //for expandable table rows

  constructor(orderBookId :number = null, orderId: number = null, order: Order = null, bookId: number = null, book: Book = null,
    plannedQty: number = 1, finishedQty: number = 0, addedDate: Date = null, comment: string = null,
    addedBy: number = null, orderNumber: string = null, componentNumber: string = null, position: number = 0,
    type: number = 0, officeNumber: string = null, number: string = null,
    wareList: number = 0, purchaseList: number = 0, plasmaInList: number = 0, plasmaOutList: number = 0) {

    this.orderBookId = orderBookId;
    this.orderId = orderId;
    this.order = order;
    this.bookId = bookId;
    this.book = book;
    this.plannedQty = plannedQty;
    this.finishedQty = finishedQty;
    this.addedDate = addedDate;
    this.comment = comment;
    this.addedBy = addedBy;
    this.orderNumber = orderNumber;
    this.componentNumber = componentNumber;
    this.type = type;
    this.officeNumber = officeNumber;
    this.position = position;
    this.number = number;
    this.wareList = wareList;
    this.purchaseList = purchaseList;
    this.plasmaInList = plasmaInList;
    this.plasmaOutList = plasmaOutList;
    this.isExpanded = false;
  }
}
