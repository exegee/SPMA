import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig, MatPaginator, MatSort, MatTable } from '@angular/material';
import { ToastrService } from 'ngx-toastr';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { AddStockitemComponent } from '../../dialogs/add-stockitem/add-stockitem.component';
import { EditStockitemComponent } from '../../dialogs/edit-stockitem/edit-stockitem.component';
import { StockItem } from '../../models/stocktaking/stockitem.model';
import { StockItemsService } from '../../services/stocktaking/stockitems.service';
import { StockItemsDataSource } from './stockitems-datasource';

@Component({
  selector: 'app-pit',
  templateUrl: './pit.component.html',
  styleUrls: ['./pit.component.css']
})
export class PitComponent implements OnInit {

  dataSource: StockItemsDataSource;
  searchInputValue: string = '';
  stockItem: StockItem;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatTable, { static: true }) table: MatTable<any>;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('searchInput', { static: true }) searchInput: ElementRef;


  constructor(private stockItemsService: StockItemsService, public dialog: MatDialog, private toastr: ToastrService) { }

  displayedColumns = ["Code", "PitQty", "ActualQty", "DiffQty", "Unit", "Date", "Comment"];
  displayedColumnsHeaders = ["Kod", "Nazwa", "Ilość wg spisu", "Stan bieżący", "Róźnica", "Jm", "Komentarz", "Data modyfikacji"];

  ngOnInit() {
    this.dataSource = new StockItemsDataSource(this.stockItemsService);
    this.dataSource.getStockItems('', 'asc', 0, 15, '');
  }

  ngAfterViewInit() {
    fromEvent(this.searchInput.nativeElement, 'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.loadStockItemsPage();
        })
      )
      .subscribe();


    this.paginator.page
      .pipe(
        tap(() => this.loadStockItemsPage())
      )
      .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => this.loadStockItems())
      )
      .subscribe();
  }

  loadStockItemsPage() {
    this.dataSource.getStockItems('', 'asc', this.paginator.pageIndex, this.paginator.pageSize, this.searchInput.nativeElement.value);
  }

  loadStockItems() {
    this.dataSource.getStockItems('', 'asc', 0, 15, '');
  }

  onStockItemClick(stockItem: StockItem) {
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      title: 'Wprowadź stan bieżący dla towaru',
      item: stockItem
    }

    const dialogRef = this.dialog.open(EditStockitemComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(
      updatedStockItem => {
        if (updatedStockItem != null && updatedStockItem.type != 2) {
          this.dataSource.refresh(this.searchInput.nativeElement.value);
          this.toastr.success(`${updatedStockItem.code} został zaktualizowany!`, '', { positionClass: 'toast-top-right', timeOut: 2500 });
        }
        else {
          this.dataSource.refresh(this.searchInput.nativeElement.value);
          this.toastr.success(`${updatedStockItem.code} został usunięty!`, '', { positionClass: 'toast-top-right', timeOut: 2500 });
        }
      })
  }

  onAddStockItem() {
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      title: 'Dodaj nowy towar'
    }

    const dialogRef = this.dialog.open(AddStockitemComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(
      addedStockItem => {
        if (addedStockItem != null) {
          this.dataSource.refresh(this.searchInput.nativeElement.value);
          this.toastr.success(`${addedStockItem.code} został dodany!`, '', { positionClass: 'toast-top-right', timeOut: 2500 });
        }
      })
  }

}
