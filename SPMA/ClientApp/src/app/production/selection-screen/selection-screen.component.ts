import { animate, state, style, transition, trigger } from '@angular/animations';
import { ElementRef, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-selection-screen',
  templateUrl: './selection-screen.component.html',
  styleUrls: ['./selection-screen.component.css'],
  animations: [
    trigger('canvaState', [
      state('notActive', style({
        fill: 'grey',
        stroke: 'white'
      })),
      state('active', style({
        fill: 'green',
        stroke: '#66fe44'
      })),
      transition('active <=> notActive', [
        animate('1s')
      ])
    ])
  ]
})

export class SelectionScreenComponent implements OnInit {

  title: string ='Ekran wyboru';
  sawButtonChecked: boolean = false;
  plasmaButtonChecked: boolean = false;
  //@ViewChild('myCanvas', { static: true }) canvas: ElementRef<HTMLCanvasElement>;
  //private ctx: CanvasRenderingContext2D;
  canvaState: string = 'notActive';

  constructor(private router: Router,private route: ActivatedRoute) { }

  ngOnInit() {

    //this.ctx = this.canvas.nativeElement.getContext("2d");
    //this.ctx.moveTo(0, 50);
    //this.ctx.lineTo(100, 50);
    //this.ctx.moveTo(100, 10);
    //this.ctx.lineTo(100, 90);
    //this.ctx.moveTo(100, 10);
    //this.ctx.lineTo(200, 10);
    //this.ctx.moveTo(100, 90);
    //this.ctx.lineTo(200, 90);
    //this.ctx.stroke();
    //console.log(this.canvas);
    //this.ctx.fillStyle = "#FF0033";
    //this.ctx.fillRect(0, 0, 100, 100);
  }

  sawButtonClick() {
    this.sawButtonChecked = !this.sawButtonChecked;
    this.sawButtonChecked? this.canvaState = 'active' : this.canvaState = 'notActive'
    if (this.sawButtonChecked) {
      this.plasmaButtonChecked = false;
    }
    console.log(this.canvaState);
  }

  plasmaButtonClick() {
    this.plasmaButtonChecked = !this.plasmaButtonChecked;
    if (this.plasmaButtonChecked) {
      this.sawButtonChecked = false;
      this.canvaState = 'notActive';
    }
  }


  toMainOrderNav() {
    if (this.route.snapshot.queryParams['socket'] == "saw") {
      this.router.navigate(["../productionOrderList"], { relativeTo: this.route, queryParams: { socket: "saw" } });
    } else if (this.route.snapshot.queryParams['socket'] == "plasma") {
      this.router.navigate(["../productionOrderList"], { relativeTo: this.route, queryParams: { socket: "plasma" } });
    }
  }

  //toMainOrderNavPlasma() {
  //  this.router.navigate(["../productionOrderList"], { relativeTo: this.route, queryParams: { socket: "plasma" } });
  //}

  //toBuiltDir() {
  //  this.router.navigate(['../inbuilt'], { relativeTo: this.route });
  //}

  toSurplusWarehouse() {
    if (this.route.snapshot.queryParams['socket'] == "saw") {
      this.router.navigate(['../surplusWarehouse'], { relativeTo: this.route, queryParams: { socket: "saw" } });
    } else if (this.route.snapshot.queryParams['socket'] == "plasma") {
      this.router.navigate(['../surplusWarehouse'], { relativeTo: this.route, queryParams: { socket: "plasma" } });
    }
    
  }

  //toSurplusWarehousePlasma() {
  //  this.router.navigate(['../surplusWarehouse'], { relativeTo: this.route, queryParams: { socket: "plasma"} });
  //}


} 
