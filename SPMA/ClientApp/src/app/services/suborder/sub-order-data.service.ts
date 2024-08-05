import { Injectable } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { BookComponent } from '../../models/books/bookcomponent.model';
import { OrderBook } from '../../models/orders/orderbook.model';
import { SubOrderItem } from '../../models/suborders/suborderitem.model';
import { ProductionState } from '../../shared/Enums/productionstate';
import { BookService } from '../books/book.service';
import { BookComponentService } from '../production/bookcomponent.service';
import { SubOrderService } from './suborder.service';

@Injectable({
  providedIn: 'root'
})
export class SubOrderDataService {

  subOrderBookComponent: BookComponent;
  subOrderInfo: OrderBook;
  subOrder: MatTableDataSource<SubOrderItem> = new MatTableDataSource<SubOrderItem>();
  downloaded: boolean = false;

  constructor(private bookService: BookService, private bookComponentService: BookComponentService,
    private subOrderService: SubOrderService) { }

  // Get book
     getBookInfo = async (componentNumber: string, officeNumber: string) => {
  //console.log("Download book...");
  await this.bookService.getBook(componentNumber, officeNumber).then((book: BookComponent) => {
    this.subOrderBookComponent = book;
    //console.log(book);
  });
}

// Get data from BookComponents table - method download relationship between current component and book
 getBookComponentInfo = (item: SubOrderItem, bookid: number) => {
  return new Promise((resolve, reject) => {
    resolve(this.bookComponentService.getBookComponentAsync(item, bookid))
  })
}

// Get suborder
  getSubOrderInfo = async (subOrderId) => {
  //console.log('Downloading suborder additional info');
  await this.subOrderService.getSubOrderByIdInfo(subOrderId).then((data: OrderBook) => {
    // console.log('Suborder info downloaded.');
    this.subOrderInfo = data;
    // console.log(data);
  })
}

// Get all data in suborder table based on parsed suborder id
  getSubOrder = async (subOrderId, sourceType) => {
  //console.log('Downloading suborder...');
  await this.subOrderService.getSubOrderByIdAsyncForCut(subOrderId,sourceType).then((data: SubOrderItem[]) => {
    //console.log(data);
    this.subOrder.data = data;
  },
    (error) => { console.log(error) })
}


// Gets all the necessary info for each subOrder item
 getSubOrderAdditionalInfo = async () => {
  var subOrderLength = this.subOrder.data.length;
  //console.log("Downloading data...");

  for (let i = 0; i < subOrderLength; i++) {

    // Get current subOrder item
    var subOrderItem = this.subOrder.data[i];
    //console.log(subOrderItem);

    // Get bookComponent relationship for current subOrderItem
    var bookComponentItem = await this.getBookComponentInfo(subOrderItem, this.subOrderBookComponent.bookId) as BookComponent;

    // Updates additional properties
    subOrderItem.orderTree = bookComponentItem.order;
    subOrderItem.levelTree = bookComponentItem.level;

    this.subOrder.data[i].inProduction.productionStateId == ProductionState["Awaiting"] ? this.subOrder.data[i].isInProduction = true : this.subOrder.data[i].isInProduction = false;

  }
}

}
