import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-suborder-edit-error',
  templateUrl: './suborder-edit-error.component.html',
  styleUrls: ['./suborder-edit-error.component.css']
})
export class SuborderEditErrorComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<SuborderEditErrorComponent>,
    @Inject(MAT_DIALOG_DATA) public message: string) {

  }

  ngOnInit() {
  }

  onOK() {
    this.dialogRef.close();
  }

}
