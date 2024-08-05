import { EventEmitter } from 'events';
import { InProduction } from '../production/inproduction.model';
import { Ware } from '../warehouse/ware.model';

export class SubOrderItem {
  public inProductionRWId: number;
  public inProduction: InProduction;
  public ware: Ware;
  public wareQuantity: number;
  public wareLength: number;
  public wareUnit: string;
  public toIssue: number;
  public plannedToCut: number;
  public issued: number;
  public totalToIssue: number;
  public isAdditionallyPurchasable: number;
  public orderTree: number;
  public levelTree: number;
  public isInProduction: boolean;
  public sourceType: number;
  public updatedLocally: boolean;
  public updatedInDatabase: boolean;
  public isUpdating: boolean;
  public bookQty: number;
  public isBookQtyChanged: boolean;

  constructor(
    inProductionRWId: number = null,
    inProduction: InProduction = null,
    ware: Ware = null,
    wareQuantity: number = null,
    wareLength: number = null,
    wareUnit: string = null,
    toIssue: number = null,
    plannedToCut: number = null,
    issued: number = null,
    isAdditionallyPurchasable: number = 0,
    totalToIssue: number = 0,
    orderTree: number = 0,
    levelTree: number = 0,
    isInProduction: boolean = false,
    sourceType: number = 0,
    updatedLocally: boolean = false,
    updatedInDatabase: boolean = false,
    isUpdating: boolean = false,
    bookQty: number = 0,
    isBookQtyChanged: boolean = false  ) {
    this.inProductionRWId = inProductionRWId,
    this.inProduction = inProduction,
    this.ware = ware;
    this.wareQuantity = wareQuantity;
    this.wareLength = wareLength;
    this.wareUnit = wareUnit;
    this.toIssue = toIssue;
    this.plannedToCut = plannedToCut;
    this.issued = issued;
    this.isAdditionallyPurchasable = isAdditionallyPurchasable;
    this.totalToIssue = totalToIssue;
    this.orderTree = orderTree;
    this.levelTree = levelTree;
    this.isInProduction = isInProduction;
    this.sourceType = sourceType;
    this.updatedLocally = updatedLocally;
    this.updatedInDatabase = updatedInDatabase;
    this.isUpdating = isUpdating;
    this.bookQty = bookQty;
    this.isBookQtyChanged = isBookQtyChanged;
  }
}
