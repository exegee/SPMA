import { Directive, ElementRef } from '@angular/core';

@Directive({ selector: '[appOffsetTop]'})
export class OffsetTopDirective {
  constructor(private _elementRef: ElementRef) {
    console.log(_elementRef.nativeElement.offsetTop);
  }
  get offsetTop(): number { return this._elementRef.nativeElement.offsetTop; }
}
