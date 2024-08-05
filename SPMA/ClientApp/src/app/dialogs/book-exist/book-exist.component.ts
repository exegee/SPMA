import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

export interface DialogData {
  message: string;
}

@Component({
  selector: 'app-book-exist',
  templateUrl: './book-exist.component.html',
  styleUrls: ['./book-exist.component.css']
})
export class BookExistComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<BookExistComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) { }

  ngOnInit() {
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}
