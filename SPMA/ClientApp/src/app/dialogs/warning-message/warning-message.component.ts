import { Inject } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
import { privateDecrypt } from 'crypto';

@Component({
  selector: 'app-warning-message',
  templateUrl: './warning-message.component.html',
  styleUrls: ['./warning-message.component.css']
})
export class WarningMessageComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) private warningMessage: string) { }

  ngOnInit() {
  }

}
