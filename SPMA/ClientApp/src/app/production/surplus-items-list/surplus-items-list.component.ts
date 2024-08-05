import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmDeleteComponent } from '../../dialogs/confirm-delete/confirm-delete.component';
import { ConfirmComponent } from '../../dialogs/confirm/confirm.component';
import { NewSurplusOrderComponent } from '../../dialogs/new-surplus-order/new-surplus-order.component';
import { Order } from '../../models/orders/order.model';
import { WarehouseItem } from '../../models/production/warehouseItem.model';
import { WarehouseItemsService } from '../../services/production/warehouseItems.service';




@Component({
  selector: 'app-surplus-items-list',
  templateUrl: './surplus-items-list.component.html',
  styleUrls: ['./surplus-items-list.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('500ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class SurplusItemsListComponent implements OnInit {


  title: string = "Zlecenia nadmiarowe";
  dataSource: MatTableDataSource<WarehouseItem> = new MatTableDataSource<WarehouseItem>();
  displayedColumns: string[] = ['componentName', 'componentNumber', 'wareCode', 'quantity','wareQtySum', 'addedBy', 'addedDate', 'comment'];
  //selectedItem: WarehouseItem = new WarehouseItem();
  expandedElement: WarehouseItem | null;

  constructor(private router: Router, private route: ActivatedRoute, private dialog: MatDialog, private WarehouseItemService: WarehouseItemsService) { }

  ngOnInit() {
    this.WarehouseItemService.getWarehouseItems().subscribe(
      (items: WarehouseItem[]) => {
        this.dataSource.data = items;
        console.log(items);
      });
  }


  backToSelectionScreen() {
    this.router.navigate([`/production/selectionScreen`], { queryParams: { 'socket': this.route.snapshot.queryParams['socket'] } });
  }


  onTableRowClick(item: WarehouseItem) {
    //console.log(item);
    this.expandedElement = this.expandedElement === item ? null : item;
    console.log(this.expandedElement);
    //this.selectedItem = item;

  }

  onAddNewSurplusOrder() {

    const dialogConfig: MatDialogConfig = {
      minWidth: '1000px',
      minHeight: '250px',
      disableClose: true,
      data: {
        "title": "Nowe zlecenie",
        "surplusOrderItem": null,
        "mode": "new"
      },
      restoreFocus: false
    };

    const dialogRef = this.dialog.open(NewSurplusOrderComponent, dialogConfig);

    dialogRef.afterClosed().subscribe((result: WarehouseItem) => {
      console.log("result", result);
      if (result) {
        this.WarehouseItemService.addWarehouseItem(result).subscribe(
          {
            next: () => {
              this.WarehouseItemService.getWarehouseItems()
                .subscribe((items: WarehouseItem[]) => this.dataSource.data = items);
            },
            error: (err) => { console.error('Request error: ', err) }
          }
        );
      }
    });
  }


  onDeleteOrder() {
    //console.log(this.selectedItem);
    // console.log(this.selectedItem.WarehouseItemId);


    const dialogRef = this.dialog.open(ConfirmDeleteComponent, { disableClose: true, restoreFocus: false, data: { message: "Zastanów się czy na pewno chcesz to zrobić" }})

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
                this.WarehouseItemService.deleteWarehouseItem(this.expandedElement.warehouseItemId).subscribe(
          () => {
            console.log("Deleted");
            this.WarehouseItemService.getWarehouseItems().subscribe({
              next: (items: WarehouseItem[]) => this.dataSource.data = items,
              error: (err) => console.error(err)
            });
          }
        );
      }
    });
  }

  onEditOrder() {
    let Id: number = this.expandedElement.warehouseItemId;
    let origin = this.dataSource.data.find(x => x.warehouseItemId == Id);
    let inputData: WarehouseItem = {
      warehouseItemId: origin.warehouseItemId,
      componentQty: origin.componentQty,
      reservedQty: origin.reservedQty,
      wareQty: origin.wareQty,
      wareQtySum: origin.wareQtySum,
      component: origin.component,
      ware: origin.ware,
      addedBy: origin.addedBy,
      addedDate: origin.addedDate,
      comment: origin.comment,
      state: origin.state,
      parentRWItem: origin.parentRWItem
    }


    const dialogConfig: MatDialogConfig = {
      minWidth: '1000px',
      minHeight: '250px',
      disableClose: true,
      data: {
        "title": "Edycja zlecenia",
        "surplusOrderItem": inputData,
        "mode": "edit"
      },
      restoreFocus: false
    };

    const dialogRef = this.dialog.open(NewSurplusOrderComponent, dialogConfig);

    dialogRef.afterClosed().subscribe((result: WarehouseItem) => {
      console.log("result", result);
      if (result) {
        this.WarehouseItemService.editWarehouseItem(result).subscribe(
          {
            next: () => {
              this.WarehouseItemService.getWarehouseItems()
                .subscribe((items: WarehouseItem[]) => this.dataSource.data = items);
            },
            error: (err) => { console.error('Request error: ', err) }
          }
        );
      }
    });

  }

  onNewOrderFromExisting() {
    const dialogRef = this.dialog.open(ConfirmComponent, { disableClose: true, restoreFocus: false, data: { message: "Czy chcesz skopiować wybrane zlecenie" } })
    
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.WarehouseItemService.copyWarehouseItem(this.expandedElement).subscribe(
          () => {
            console.log("Copied");
            this.WarehouseItemService.getWarehouseItems().subscribe({
              next: (items: WarehouseItem[]) => this.dataSource.data = items,
              error: (err) => console.error(err)
            });
          }
        );
      }
    });
  }

  setTrBgColor(warehouseItem: WarehouseItem): string {
    switch (warehouseItem.state) {
      case 0: {
        return 'antiquewhite'
      }
      case 1: {
        return 'lightgreen'
      } default: {
        return 'none'
      }
    }
  }

  printState(item: WarehouseItem): string {
    if (item.state == 0) { return 'Oczekuje' }
    else if (item.state == 1) { return 'Ucięty' }
  }

}
