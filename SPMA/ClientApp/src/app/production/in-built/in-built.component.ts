import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-in-built',
  templateUrl: './in-built.component.html',
  styleUrls: ['./in-built.component.css']
})
export class InBuiltComponent implements OnInit {

  constructor(private router : Router) { }

  ngOnInit() {
  }

  turnBack() {
    this.router.navigate(['production/selectionScreen']);
  }

}
