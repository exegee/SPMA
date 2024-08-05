export class Order {
  public orderId: number;
  public number: string;
  public name: string;
  public plannedQty: number;
  public finishedQty: number;
  public state: number;
  public type: number;
  public clientName: string;
  public orderDate: Date;
  public requiredDate: Date;
  public shippedDate: Date;
  public shippingName: string;
  public shippingAddress: string;
  public shippingCity: string;
  public shippingPostalCode: string;
  public shippingRegion: string;
  public shippingCountry: string;
  public shippingTypeId: number;
  public addedById: number;
  public comment: string;
  public clientId: number;
  public position: number;
  public rwCompletion: number;
  public loadingRWStatusCompleted: boolean;

  constructor(orderId: number = null, number: string = null, name: string = null, plannedQty: number = null, finishedQty: number = null, state: number = null, type: number = null,
    clientName: string = null, orderDate: Date = null, requiredDate: Date = null, shippedDate: Date = null, shippingName: string = null, shippingAddress: string = null,
    shippingCity: string = null, shippingPostalCode: string = null, shippingRegion: string = null, shippingCountry: string = null, shippingTypeId: number = null,
    addedById: number = null, comment: string = null, clientId: number = null, position: number = null, rwCompletion: number = 0, loadingRWStatusCompleted: boolean = false) {
    this.orderId = orderId;
    this.number = number;
    this.name = name;
    this.plannedQty = plannedQty;
    this.finishedQty = finishedQty;
    this.state = state;
    this.type = type;
    this.clientName = clientName;
    this.orderDate = orderDate;
    this.requiredDate = requiredDate;
    this.shippedDate = shippedDate;
    this.shippingName = shippingName;
    this.shippingAddress = shippingAddress;
    this.shippingCity = shippingCity;
    this.shippingPostalCode = shippingPostalCode;
    this.shippingRegion = shippingRegion;
    this.shippingCountry = shippingCountry;
    this.shippingTypeId = shippingTypeId;
    this.addedById = addedById;
    this.comment = comment;
    this.clientId = clientId;
    this.position = position;
    this.rwCompletion = rwCompletion;
    this.loadingRWStatusCompleted = loadingRWStatusCompleted;
  }
}
