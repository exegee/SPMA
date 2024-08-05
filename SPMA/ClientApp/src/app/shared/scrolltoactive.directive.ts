import { Directive, ElementRef, HostListener } from "@angular/core";

@Directive({
  selector: '[appScrollToActive]',
  exportAs: 'appScrollToActive',
})

export class ScrollToActiveDirective {

  @HostListener('document:keydown', ['$event'])
  onKeyPress(event) {
    console.log(this._elementRef);
  }

  constructor(private _elementRef: ElementRef) {
    console.log(this._elementRef);
    this._elementRef.nativeElement.scrollTop = 20;
  }
  set scrollTo(value: number) { this._elementRef.nativeElement.scrollTop = value; };
}

