import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';


@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.css'],
})
export class SidenavComponent implements OnInit {

  constructor(private router: Router) { }
  ngOnInit() {
    
  }

  toOrderList(state: number) {
    this.router.navigate(["orders"], {
      queryParams: {
        state: state
      }
    })
  }

}
