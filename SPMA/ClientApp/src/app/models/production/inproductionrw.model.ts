import { Ware } from '../warehouse/ware.model';
import { InProduction } from './inproduction.model';

export class InProductionRW {
    public inProductionRWId: number;
    public inProduction: InProduction;
    public issued: number;
    public toIssue: number;
    public ware: Ware;
    public qcheckstatus: number;
    public qavailable: number;
    public wareLength: number;
    public isAdditionallyPurchasable: boolean;

    constructor(inProductionRWId: number = null, issued: number = 0, toIssue: number = null,
      ware: Ware = null, qcheckstatus: number = 0, inProduction: InProduction = null, qavailable: number = 0,
      wareLength: number = 0, isAdditionallyPurchasable: boolean = false) {
        this.inProductionRWId = inProductionRWId;
        this.issued = issued;
        this.toIssue = toIssue;
        this.inProduction = inProduction;
        this.ware = ware;
        this.qcheckstatus = qcheckstatus;
        this.qavailable = qavailable;
      this.wareLength = wareLength;
      this.isAdditionallyPurchasable = isAdditionallyPurchasable;
    }
}
