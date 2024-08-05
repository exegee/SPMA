export namespace OrdersHelper {

  export class OrdersSearchFilter {
    filterByNumber: string;
    filterByName: string;
    filterByClientName: string;

    constructor(filterByNumber: string = '', filterByName: string = '', filterByClientName: string = '') {
      this.filterByClientName = filterByClientName;
      this.filterByName = filterByName;
      this.filterByNumber = filterByNumber;
    }
  }
}
