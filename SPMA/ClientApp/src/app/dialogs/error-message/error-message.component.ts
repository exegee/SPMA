import { HttpErrorResponse } from '@angular/common/http';
import { Inject } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-error-message',
  templateUrl: './error-message.component.html',
  styleUrls: ['./error-message.component.css']
})
export class ErrorMessageComponent implements OnInit {

  detailsShowed: boolean = false;

  constructor(@Inject(MAT_DIALOG_DATA) private errorMessage: HttpErrorResponse) { }

  ngOnInit() {
  }

  showDetails() {
    this.detailsShowed = true;
  }

}
