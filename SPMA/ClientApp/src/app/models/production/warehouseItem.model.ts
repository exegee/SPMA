import { ProductionState } from "../../shared/Enums/productionstate";
import { Ware } from "../warehouse/ware.model";
import { InProductionRW } from "./inproductionrw.model";
import { InventorComponent } from "./inventorcomponent.model";



export class WarehouseItem {

  public warehouseItemId: number;
  public componentQty: number;//number of pieces
  public reservedQty: number;
  public wareQty: number;//amount of material in one piece
  public wareQtySum: number;//total amount of used material
  public component: InventorComponent;
  public ware: Ware;
  public addedBy: string;
  public addedDate: Date;
  public comment: string;
  public state: number;
  public parentRWItem: InProductionRW;

  constructor(warehouseItemId: number = null, componentQty: number = -1, reservedQty: number = 0, wareQty: number = -1, wareQtySum: number = -1, component: InventorComponent = null,
    ware: Ware = null, addedDate: Date = null, addedBy: string = null, state: number = 0, parentRWItem: InProductionRW = null,
    comment: string = null)
  {
    this.warehouseItemId = warehouseItemId;
    this.componentQty = componentQty;
    this.reservedQty = reservedQty;
    this.wareQty = wareQty;
    this.wareQtySum = wareQtySum;
    this.component = component;
    this.ware = ware;
    this.addedDate = addedDate;
    this.addedBy = addedBy;
    this.comment = comment;
    this.state = state;
    this.parentRWItem = parentRWItem;
  }

}
