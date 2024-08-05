import { OptimaItem } from '../optima/optimaitem.model';
import { Ware } from '../warehouse/ware.model';
import { InProduction } from './inproduction.model';
import { BookComponent } from '../books/bookcomponent.model';

export class InventorComponent {
 

  public componentId: number;
  public number: string;
  public name: string;
  public materialType: string;
  public cost: number;
  public weight: number;

  // COMPONENT TYPE
  // 0 - Part,
  // 1 - Assembly,
  // 2 - Book
  public componentType: number;

  // LAST SOURCE TYPE USED
  // 0 - standard,
  // 1 - saw,
  // 2 - plasma IN,
  // 3 - plasma OUT without entrusted material,
  // 4 - plasma OUT with entrusted material,
  // 5 - purchase
  public lastSourceType: number;

  // ASSOSIATED OPTIMA WARE
  public ware: Ware;

  public comment: string;
  public author: string;
  public addedBy: string;
  public modifiedDate: Date = new Date();

  public wareQuantity: number;
  public wareLength: number;
  public wareUnit: string;

  public quantity: number;
  public level: number;
  public order: number;
  public existInDatabase: boolean;
  public sumQuantity: number;
  public toProduction: boolean;
  public treeNumber: string;
  public singlePieceQty: number;
  public isAdditionallyPurchasable: boolean;

  public requiredItems: OptimaItem[];
  public updated: boolean;

  public inProductionReservation: InProduction;

  public reservedItemId: number;
  public reservedQty: number;

  constructor(id: number = null, number: string = "", name: string = "", materialType: string = null,
    cost: number = null, weight: number = null, componentType: number = null, lastSourceType: number = null, ware: Ware = new Ware(), comment: string = null,
    author: string = null, addedBy: string = null, modifiedDate: Date = null, wareQuantity: number = 0, wareLength: number = 0, wareUnit: string = null, quantity: number = 0,
    level: number = null, order: number = null, existInDatabase: boolean = null, sumQuantity: number = null, toProduction: boolean = true, treeNumber: string = null,
    updated: boolean = false, singlePieceQty: number = 0, inProductionReservation: InProduction = null, isAdditionallyPurchasable: boolean = false,
    reservedItemId: number = -1, reservedQty: number = 0) {
    this.componentId = id;
    this.number = number;
    this.name = name;
    this.materialType = materialType;
    this.cost = cost;
    this.weight = weight;
    this.componentType = componentType;
    this.lastSourceType = lastSourceType;
    this.ware = ware;
    this.comment = comment;
    this.author = author;
    this.addedBy = addedBy;
    this.modifiedDate = modifiedDate;
    this.wareQuantity = wareQuantity;
    this.wareLength = wareLength;
    this.wareUnit = wareUnit;
    this.quantity = quantity;
    this.level = level;
    this.order = order;
    this.existInDatabase = existInDatabase;
    this.sumQuantity = sumQuantity;
    this.toProduction = toProduction;
    this.treeNumber = treeNumber;
    this.updated = updated;
    this.singlePieceQty = singlePieceQty;
    this.inProductionReservation = inProductionReservation;
    this.isAdditionallyPurchasable = isAdditionallyPurchasable;
    this.reservedItemId = reservedItemId;
    this.reservedQty = reservedQty;
  }
}
