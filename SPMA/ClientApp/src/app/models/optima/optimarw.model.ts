export class OptimaRW {
  public trN_TrNID: number;
  public trN_NumerPelny: string;
  public trN_Bufor: number;
  public trN_RazemNetto: number;
  public trN_OpeModNazwisko: string;
  public trN_DataDok: Date;
  public trN_DataOpe: Date;

  constructor(trN_TrNID: number = null, trN_NumerPelny: string = null, trN_Bufor: number = null, trN_RazemNetto: number = null,
    trN_OpeModNazwisko: string = null, trN_DataDok: Date = null, trN_DataOpe: Date = null) {
    this.trN_Bufor = trN_Bufor;
    this.trN_NumerPelny = trN_NumerPelny;
    this.trN_Bufor = trN_Bufor;
    this.trN_RazemNetto = trN_RazemNetto;
    this.trN_OpeModNazwisko = trN_OpeModNazwisko;
    this.trN_DataDok = trN_DataDok;
    this.trN_DataOpe = trN_DataOpe;
  }
}
