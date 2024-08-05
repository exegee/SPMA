export class SubOrderItemDiff {

  public number: string = '';
  public name: string = '';

  public isInProductionCheck: boolean = false;
  public isInProductionOld: boolean = false;
  public isInProductionNew: boolean = false;

  public bookMultiplierCheck: boolean = false;
  public bookMultiplierOld: number = 0;
  public bookMultiplierNew: number = 0;

  public quantitiesCheck: boolean = false;
  public quantitiesOld: number = 0;
  public quantitiesNew: number = 0;

  public sourceTypeCheck: boolean = false;
  public sourceTypeOld: number = 0;
  public sourceTypeNew: number = 0;

  public isAdditionallyPurchasableCheck: boolean = false;
  public isAdditionallyPurchasableOld: boolean = false;
  public isAdditionallyPurchasableNew: boolean = false;

  public wareCodeCheck: boolean = false;
  public wareCodeOld: string = '';
  public wareCodeNew: string = '';

  public wareLengthCheck: boolean = false;
  public wareLengthOld: number = 0;
  public wareLengthNew: number = 0;

  public wareUnitCheck: boolean = false;
  public wareUnitOld: string = '';
  public wareUnitNew: string = '';

  constructor() {
  }
}
