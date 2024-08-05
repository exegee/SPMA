import { Component, Inject, OnInit, ElementRef } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { StockItem } from '../../models/stocktaking/stockitem.model';
import { StockItemsService } from '../../services/stocktaking/stockitems.service';

@Component({
  selector: 'app-edit-stockitem',
  templateUrl: './edit-stockitem.component.html',
  styleUrls: ['./edit-stockitem.component.css']
})
export class EditStockitemComponent implements OnInit {


  isStockItemUpdating: boolean;
  stockItem: StockItem;
  description: string;
  stockItemActualQuantityInput = new FormControl(null, [Validators.required]);
  stockItemCommentInput = new FormControl(null);


  constructor(
    private dialogRef: MatDialogRef<EditStockitemComponent>,
    private stockItemsService: StockItemsService,
    @Inject(MAT_DIALOG_DATA) data,
  ) {
    this.description = data.title;
    this.stockItem = data.item
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.stockItemActualQuantityInput.setValue(this.stockItem.pitQty);
    this.stockItemCommentInput.setValue(this.stockItem.comment);
  }

  onSubmitClick() {
    if (!this.stockItemActualQuantityInput.valid) {
      this.stockItemActualQuantityInput.updateValueAndValidity();
      return;
    }
    this.isStockItemUpdating = true;
    this.stockItem.pitQty = this.stockItemActualQuantityInput.value;
    this.stockItem.comment = this.stockItemCommentInput.value;
    this.stockItemsService.updateStockItemQty(this.stockItem)
      .subscribe(updatedStockItem => {
        this.isStockItemUpdating = false;
        this.dialogRef.close(updatedStockItem);
      });
  }

  onCancelClick() {
    this.dialogRef.close();
  }

  onKeyDownActualQtyInput(event) {
    if (event.key === "Enter") {
      this.onSubmitClick();
    }
  }

  onKeyDownCommentInput(event) {
    if (event.key === "Enter") {
      this.onSubmitClick();
    }
  }

  onDeleteClick() {
    this.stockItemsService.deleteStockItem(this.stockItem)
      .subscribe(stockItemRemoved => {
        this.dialogRef.close(stockItemRemoved);
      });
  }
}
