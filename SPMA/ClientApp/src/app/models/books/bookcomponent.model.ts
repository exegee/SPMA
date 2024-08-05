import { Book } from './book.model';
import { Order } from '../orders/order.model';
import { InventorComponent } from '../production/inventorcomponent.model';
import { OrderBook } from '../orders/orderbook.model';

export class BookComponent {
  public bookComponentId: number;
  public bookId: number;
  public book: Book;
  public component: InventorComponent;
  public components: InventorComponent[];
  public mainOrder: Order;
  public orderBook: OrderBook;
  public level: number;
  public order: number;
  public quantity: number;

  constructor(book: Book = null,
    components: InventorComponent[] = null,
    bookComponentId: number = null,
    bookId: number = null,
    mainOrder: Order = null,
    orderBook: OrderBook = null,
    level: number = -1,
    order: number = -1,
    quantity: number = 1,
    component: InventorComponent = null
  ) {
    this.bookComponentId = bookComponentId;
    this.bookId = bookId;
    this.book = book;
    this.components = components;
    this.mainOrder = mainOrder;
    this.orderBook = orderBook;
    this.level = level;
    this.order = order;
    this.quantity = quantity;
    this.component = component;
  }
}
