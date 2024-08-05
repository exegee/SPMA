export class OptimaMag {
  public mag_MagId: number;
  public mag_Symbol: string;
  public mag_Nazwa: string;

  constructor(mag_MagId: number = null, mag_Symbol: string = null, mag_Nazwa: string = null) {
    this.mag_MagId = mag_MagId;
    this.mag_Symbol = mag_Symbol;
    this.mag_Nazwa = mag_Nazwa;
  }
}
