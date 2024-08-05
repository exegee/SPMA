import { Component, OnInit, ViewChild, TemplateRef, ViewContainerRef } from '@angular/core';
import { ContextMenuComponent } from 'ngx-contextmenu';
import { Subscription, fromEvent } from 'rxjs';
import { OverlayRef, Overlay } from '@angular/cdk/overlay';
import { TemplatePortal } from '@angular/cdk/portal';
import { take, filter } from 'rxjs/operators';
//import { environment } from './../../../environments/environment';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  sub: Subscription;
  chx: boolean = false;
  attrValue: any;

  @ViewChild('userMenu', { static: false }) userMenu: TemplateRef<any>;
  public version: string;
  overlayRef: OverlayRef | null;
  public progressBarValue: number = 23;

  testbox: boolean = false;
  public users = [
    { name: 'John', otherProperty: 'Foo' },
    { name: 'Joe', otherProperty: 'Bar' }
  ];
  @ViewChild(ContextMenuComponent, { static: false }) public basicMenu: ContextMenuComponent;

  time = { hour: 13, minute: 30 };
  constructor(public overlay: Overlay,
    public viewContainerRef: ViewContainerRef) { }

  onAttrCheck() {
    console.log(this.attrValue);
    
  }

  open({ x, y }: MouseEvent, user) {
    this.close();
    const positionStrategy = this.overlay.position()
      .flexibleConnectedTo({ x, y })
      .withPositions([
        {
          originX: 'end',
          originY: 'bottom',
          overlayX: 'end',
          overlayY: 'top',
        }
      ]);

    this.overlayRef = this.overlay.create({
      positionStrategy,
      scrollStrategy: this.overlay.scrollStrategies.close()
    });

    this.overlayRef.attach(new TemplatePortal(this.userMenu, this.viewContainerRef, {
      $implicit: user
    }));

    this.sub = fromEvent<MouseEvent>(document, 'click')
      .pipe(
        filter(event => {
          const clickTarget = event.target as HTMLElement;
          return !!this.overlayRef && !this.overlayRef.overlayElement.contains(clickTarget);
        }),
        take(1)
      ).subscribe(() => this.close())

  }

  close() {
    this.sub && this.sub.unsubscribe();
    if (this.overlayRef) {
      this.overlayRef.dispose();
      this.overlayRef = null;
    }


  }

  ngOnInit() {
    //const { version: appVersion } = require('../../../../../package.json');
    //this.version = environment.version;
  }
  onClick() {
    this.chx = !this.chx;
    console.log(this.chx);
  }

}
