import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({
  selector: '[appToProductionToggle]',
  exportAs: 'appToProductionToggle',
  templateUrl: 'to-production-toggle.component.html',
  styleUrls: ['to-production-toggle.component.css']
})

export class ToProductionToggleComponent {

  @HostListener('click') toggle() {
    this.toogle();
    //this.click = true;
    //console.log(this.checkValue);
    this.clickChange.next();
  }
  clickValue = false;
  checkValue = true;
  @Output() checkChange = new EventEmitter();
  @Output() clickChange = new EventEmitter();

  @Input()
  get check() {
    return this.checkValue;
  }
  @Input()
  get click() {
    return this.clickValue;
  }

  set click(value) {
    if (this.clickValue != value) {
      this.clickValue = value;
      this.clickChange.emit(this.clickValue);
    }
  }

  set check(value) {
    if (this.checkValue != value) {
      this.checkValue = value;
      this.checkChange.emit(this.checkValue);
    }
  }

  toogle() {
    this.check = !this.check;
  }

}
