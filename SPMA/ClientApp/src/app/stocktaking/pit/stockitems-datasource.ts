import { CollectionViewer } from '@angular/cdk/collections';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { PagedStockItem } from '../../models/stocktaking/pagedstockitem.model';
import { StockItem } from '../../models/stocktaking/stockitem.model';
import { StockItemsService } from '../../services/stocktaking/stockitems.service';

export class StockItemsDataSource implements DataSource<StockItem>{

  private stockItemsSubject = new BehaviorSubject<StockItem[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  data: StockItem[];
  dataLength: number;

  public loading$ = this.loadingSubject.asObservable();


  constructor(private stockItemsService: StockItemsService) {
  }

  connect(collectionViewer: CollectionViewer): Observable<StockItem[]> {
    return this.stockItemsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.stockItemsSubject.complete();
    this.loadingSubject.complete();
  }

  getStockItems(orderBy: string, sortOrder = 'asc', page: number, pageSize: number, filter: string) {
    this.loadingSubject.next(true);

    this.stockItemsService.getStockItems(orderBy, sortOrder, page, pageSize, filter).pipe(
      catchError(() => of([])),
      finalize(() => this.loadingSubject.next(false))
    )
      .subscribe((pagedStockItem: PagedStockItem) => {
        this.data = pagedStockItem.results as StockItem[];
        this.stockItemsSubject.next(pagedStockItem.results);
        this.dataLength = pagedStockItem.rowCount;
      });
  }

  refresh(filter: string) {
    if (filter == null)
      filter = '';
    this.getStockItems('', 'asc', 0, 15, filter);
  }

}
