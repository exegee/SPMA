import { Directive, HostListener, HostBinding } from '@angular/core';
import { AnimationMetadata } from '@angular/animations';

@Directive({
  selector: '[appCollapse]',
  exportAs: 'appCollapse',
})
export class CollapseDirective {
  public isOpen = false;

  @HostListener('click') toggleOpen() {
    this.isOpen = !this.isOpen;
  }

}
