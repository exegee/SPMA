import { InventorComponent } from '../production/inventorcomponent.model';


export class SortedComponents {
  public standardComponents: InventorComponent[];
  public plazmaCutComponents: InventorComponent[];
  public storeBoughtComponents: InventorComponent[];

  constructor(standardComponents: InventorComponent[],
              plazmaCutComponents: InventorComponent[],
              storeBoughtComponents: InventorComponent[]) {
    this.standardComponents = standardComponents;
    this.plazmaCutComponents = plazmaCutComponents;
    this.storeBoughtComponents = storeBoughtComponents;
  }
}


