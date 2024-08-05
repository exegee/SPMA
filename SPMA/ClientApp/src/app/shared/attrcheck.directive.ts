import { Directive, EventEmitter, Input, Output, ElementRef, Renderer2, HostListener } from '@angular/core';
import { Subject, Observable } from 'rxjs';

@Directive({
  selector: '[appAttrCheck]',
  exportAs: 'appAttrCheck',
})

export class AttrCheckDirective {

  constructor(private elementRef: ElementRef) {
    
  }
  


  @Input() attrName: string;
  @Input() attrValue: any;

  @Output() attrValueChange = new EventEmitter<any>();
  //observer: MutationObserver;

  @HostListener('click') attrValueCheck() {
    this.attrValue = this.elementRef.nativeElement.getAttribute(this.attrName);
    this.attrValue = this.valueTypeCheck(this.attrValue);
    this.attrValueChange.emit(this.attrValue);
    setTimeout(() => {
      this.attrValue = this.elementRef.nativeElement.getAttribute(this.attrName);
      this.attrValue = this.valueTypeCheck(this.attrValue);
      this.attrValueChange.emit(this.attrValue);
    }, 500);

  }

  //ngAfterViewInit() {
  //  this.observer = new MutationObserver((mutations: MutationRecord[]) => {
  //    for (let mutation of mutations) {
  //      console.log(mutation);
  //      if (mutation.attributeName == this.attrName) {
          
  //        this.attrValue = this.elementRef.nativeElement.getAttribute(this.attrName);
  //        this.attrValue = this.valueTypeCheck(this.attrValue);
  //        this.attrValueChange.emit(this.attrValue);
  //        break;
  //      }
  //    }
  //  })
  //  var config = { attributes: true, childList: false, characterData: false };
  //  this.observer.observe(this.elementRef.nativeElement, config);
  //}

  valueTypeCheck(value: any) {
    switch (value) {
      case "true":
        return true;
      case "false":
        return false;
    }
  }

  ngOnDestroy() {
    //this.observer.disconnect();
  }
}
