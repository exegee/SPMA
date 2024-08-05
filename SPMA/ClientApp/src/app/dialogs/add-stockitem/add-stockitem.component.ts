import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { StockItem } from '../../models/stocktaking/stockitem.model';
import { StockItemsService } from '../../services/stocktaking/stockitems.service';

@Component({
  selector: 'app-add-stockitem',
  templateUrl: './add-stockitem.component.html',
  styleUrls: ['./add-stockitem.component.css']
})
export class AddStockitemComponent implements OnInit {

  stockItem: StockItem;
  form: FormGroup;
  description: string;

  constructor(
    private dialogRef: MatDialogRef<AddStockitemComponent>,
    private stockItemService: StockItemsService,
    @Inject(MAT_DIALOG_DATA) data
  ) {
    this.description = data.title;
    this.stockItem = new StockItem();
  }

  ngOnInit() {
    this.form = new FormGroup({
      'code': new FormControl(null, Validators.required),
      'name': new FormControl(null, Validators.required),
      'qty': new FormControl(null, Validators.required),
      'unit': new FormControl(null, Validators.required),
      'comment': new FormControl(null)
    });
  }

  onAddStockItem() {
    this.stockItem.code = this.form.get("code").value;
    this.stockItem.name = this.form.get("name").value;
    this.stockItem.pitQty = this.form.get("qty").value;
    this.stockItem.comment = this.form.get("comment").value;
    this.stockItem.unit = this.form.get("unit").value;

    this.stockItemService.addStockItem(this.stockItem)
      .subscribe(addedStockItem => {
        if (addedStockItem) {
          this.dialogRef.close(this.stockItem);
        }
      });
  }

  onCancel() {
    this.dialogRef.close();
  }

}
