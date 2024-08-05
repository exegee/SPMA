export class ComponentItem {
  public componentWareId: number;
  public componentId: number;
  public wareId: string;
  public quantity: number;
  public length: number;
  public unit: string;

  constructor(componentWareId: number = null,
    componentId: number = null,
    wareId: string = null,
    quantity: number = null,
    length: number = null,
    unit: string = null) {
    this.componentWareId = componentWareId;
    this.componentId = componentId;
    this.wareId = wareId;
    this.quantity = quantity;
    this.length = length;
    this.unit = unit;
  }
}
