import { OrderBook } from '../orders/orderbook.model';
import { Ware } from '../warehouse/ware.model';
import { InventorComponent } from './inventorcomponent.model';
import { ProductionSocket } from './productionsocket.model';
import { ProductionState } from './productionstate.model';

export class InProduction {
  public inProductionId: number;
  public orderBookId: number;
  public orderBook: OrderBook;
  public componentId: number;
  public component: InventorComponent;
  public plannedQty: number;
  public finishedQty: number;
  public productionSocketId: number;
  public productionSocket: ProductionSocket;
  public productionStateId: number;
  public productionState: ProductionState;
  public ware: Ware;
  public wareLength: number;
  public subOrderNumber: string;
  public qcheck: number;
  public sourceType: number;
  public bookQty: number;
  
  public components: InventorComponent[];

  constructor(inProductionId: number = null, orderBookId: number = null, orderBook: OrderBook = null, componentId: number = null, component: InventorComponent = null,
    plannedQty: number = null, finishedQty: number = null, productionSocketId: number = null, productionSocket: ProductionSocket = null,
      productionStateId: number = null, productionState: ProductionState = null, ware: Ware = null, wareLength:number=null, components: InventorComponent[] = null, subOrderNumber: string,
  qcheck: number = 0, sourceType: number = 0, bookQty:number = 0) {
    this.inProductionId = inProductionId;
    this.orderBookId = orderBookId;
    this.orderBook = orderBook;
    this.componentId = componentId;
    this.component = component;
    this.plannedQty = plannedQty;
    this.finishedQty = finishedQty;
    this.productionSocketId = productionSocketId;
    this.productionSocket = productionSocket;
    this.productionStateId = productionStateId;
    this.productionState = productionState;
    this.ware = ware;
    this.wareLength = wareLength;
    this.components = components;
    this.subOrderNumber = subOrderNumber;
    this.qcheck = qcheck;
    this.sourceType = sourceType;
    this.bookQty = this.bookQty;
  }
}
