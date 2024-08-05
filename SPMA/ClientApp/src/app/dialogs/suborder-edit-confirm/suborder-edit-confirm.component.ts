import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { SubOrderItem } from '../../models/suborders/suborderitem.model';
import { SubOrderItemDiff } from '../../models/suborders/suborderitemdiff.model';

@Component({
  selector: 'app-suborder-edit-confirm',
  templateUrl: './suborder-edit-confirm.component.html',
  styleUrls: ['./suborder-edit-confirm.component.css']
})
export class SuborderEditConfirmComponent implements OnInit {


  constructor(
    private dialogRef: MatDialogRef<SuborderEditConfirmComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SubOrderItemDiff[]) {
    
  }

  ngOnInit() {
    console.log(this.data);
  }

  onConfirm() {
    this.dialogRef.close(true);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
