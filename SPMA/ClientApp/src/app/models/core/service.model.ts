export class Service {
  public serviceID: number;
  public name: string;
  public description: string;
  public isRunning: boolean;
  public runInterval: number;
  public currentJob: string;

  constructor(serviceID: number, name: string, description: string, isRunning: boolean, runInterval: number, currentJob: string) {
    this.serviceID = serviceID;
    this. name = name;
    this.description = description;
    this.isRunning = isRunning;
    this.runInterval = runInterval;
    this.currentJob = currentJob;
  }
}
