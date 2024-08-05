
import { InventorComponent } from '../production/inventorcomponent.model';
import { Ware } from '../warehouse/ware.model';

export class ComponentWare {
  public componentId: number;
  public component: InventorComponent;
  public wareId: number;
  public ware: Ware;
  public quantity: number;
  public length: number;
  public unit: string;

  constructor(componentId: number = null, component: InventorComponent = null, wareId: number = null, ware: Ware = null,
    quantity: number = null, length: number = null, unit: string = null) {
    this.componentId = componentId;
    this.component = component;
    this.wareId = wareId;
    this.ware = ware;
    this.quantity = quantity;
    this.length = length;
    this.unit = unit;
  }
}

