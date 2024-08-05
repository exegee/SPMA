
export class InProductionXML {
    public componentNumber: string;
    public wareCode: string;
    public wareName: string;
    public wareUnit: string;
    public wareLength: number;
    public wareQuantity: number;
    public toIssue: number;
    public issued: number;
    public totalToIssue: number;
    public qAvailable: number;
    public qCheckStatus: number;
    

    constructor(componentNumber: string = null, wareCode: string = null, wareName: string = null,
        wareUnit: string = null, wareLength: number = 0, toIssue: number = 0, issued: number = 0,
        totalToIssue: number = 0, qAvailable: number = 0, qCheckStatus: number = 0, wareQuantity: number = 0) {
        this.componentNumber = componentNumber;
        this.wareCode = wareCode;
        this.wareName = wareName;
        this.wareUnit = wareUnit;
        this.wareLength = wareLength;
        this.toIssue = toIssue;
        this.issued = issued;
        this.totalToIssue = totalToIssue;
        this.qAvailable = qAvailable;
        this.qCheckStatus = qCheckStatus;
        this.wareQuantity = wareQuantity;
    }
}
