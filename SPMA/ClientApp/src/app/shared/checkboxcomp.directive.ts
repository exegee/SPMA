import { Directive, HostListener, EventEmitter, Output, Input } from '@angular/core';

@Directive({
  selector: '[appCheckbox]',
  exportAs: 'appCheckbox',
  host: {
    '[style.cursor]': '"pointer"',
  }
})

export class CheckboxDirective {

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
