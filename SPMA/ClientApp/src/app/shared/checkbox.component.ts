import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({
  selector: '[appCheckbox]',
  exportAs: 'appCheckbox',
  templateUrl: 'checkbox.component.html',
  styleUrls: ['checkbox.component.css']
})

export class CheckboxComponent {

  @HostListener('click') toggle() {
    this.toogle();
    console.log(this.checkValue);
  }

  checkValue = true;
  @Output() checkChange = new EventEmitter();

  @Input()
  get check() {
    return this.checkValue;
  }

  set check(value) {
    this.checkValue = value;
    this.checkChange.emit(this.checkValue);
  }

  toogle() {
    this.check = !this.check;

    //this.checkValue = !this.checkValue;
  }

}
