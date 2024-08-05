export class Ware {
  public wareId: number;
  public code: string;
  public name: string;
  public quantity: number;
  public length: number;
  public unit: string;
  public converter: number;
  public date: Date = new Date();
  public twG_Kod: string;
  public twG_Nazwa: string;
  public mag_Nazwa: string;
  public mag_Symbol: string;

  constructor(wareId: number = null ,code: string = null, name: string = null, quantity: number = 1, length: number = null
    , unit: string = null, converter: number = 0, date: Date = null, twG_Kod: string = null, twG_Nazwa: string = null
    , mag_Nazwa: string = null, mag_Symbol: string = null) {
    this.wareId = wareId;
    this.code = code;
    this.name = name;
    this.quantity = quantity;
    this.length = length;
    this.unit = unit;
    this.converter = converter;
    this.date = date;
    this.twG_Kod = twG_Kod;
    this.twG_Nazwa = twG_Nazwa;
    this.mag_Nazwa = mag_Nazwa;
    this.mag_Symbol = mag_Symbol;
  }
}
