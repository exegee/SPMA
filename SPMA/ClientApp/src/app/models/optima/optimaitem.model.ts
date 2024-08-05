export class OptimaItem {
  public twr_TwrId: number;
  public mag_Symbol: string;
  public twr_Kod: string;
  public twr_Nazwa: string;
  public twI_Ilosc: number;
  public twr_JM: string;
  public twG_Kod: string;
  public twG_Nazwa: string;
  public mag_Nazwa: string;

  constructor(twr_TwrId: number = null, mag_Symbol: string = null, twr_Kod: string = null, twr_Nazwa: string = null, twI_Ilosc: number = null, twr_JM: string = null
    ,twG_Kod: string = null, twG_Nazwa: string = null
    ,mag_Nazwa: string = null) {
    this.twr_TwrId = twr_TwrId;
    this.mag_Symbol = mag_Symbol;
    this.twr_Kod = twr_Kod;
    this.twr_Nazwa = twr_Nazwa;
    this.twI_Ilosc = twI_Ilosc;
    this.twr_JM = twr_JM;
    this.twG_Kod = twG_Kod;
    this.twG_Nazwa = twG_Nazwa;
    this.mag_Nazwa = mag_Nazwa;
    this.mag_Symbol = mag_Symbol;
  }
}
