import { Component, Inject, OnInit } from '@angular/core';
import { Form, FormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-bandsaw-cut-confirm',
  templateUrl: './bandsaw-cut-confirm.component.html',
  styleUrls: ['./bandsaw-cut-confirm.component.css']
})
export class BandsawCutConfirmComponent implements OnInit {

  confirmed: boolean = false;

  plannedQtyInput: FormControl = new FormControl('');
  bookQtyInput: FormControl = new FormControl('');
  totalInDrawingInput: FormControl = new FormControl('');
  doneAlreadyQtyInput: FormControl = new FormControl('');
  totalQtyInput: FormControl = new FormControl('');
  totalMaterialQty: FormControl = new FormControl('');

  constructor(public dialogRef: MatDialogRef<BandsawCutConfirmComponent>,
    @Inject(MAT_DIALOG_DATA) public data: dataToConfirm) { }

  ngOnInit() {
/*    this.plannedQtyInput.setValue(this.data.plannedQty);*/
    //this.bookQtyInput.setValue(this.data.bookQty);
    //this.totalInDrawingInput.setValue(this.data.plannedQty);
    //this.doneAlreadyQtyInput.setValue(this.data.alreadyDoneQty);
    this.totalQtyInput.setValue(this.data.totalQty);
    this.totalMaterialQty.setValue(this.data.totalMaterialQty);
  }

  onConfirm() {
    //console.log('cut confirmed');
    this.confirmed = true;
    //this.data.alreadyDoneQty += (this.data.plannedQty * this.data.bookQty);
    //this.doneAlreadyQtyInput.setValue(this.data.alreadyDoneQty);
    this.dialogRef.close(this.confirmed)
    //this.dialogRef.close(true);
  }

  onClose() {
    this.confirmed = false;
    this.dialogRef.close(this.confirmed);
  }

}
