import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LogscreenComponent } from './logscreen/logscreen.component';


@Component({
  selector: 'app-production',
  templateUrl: './production.component.html',
  styleUrls: ['./production.component.css']
})
export class ProductionComponent implements OnInit {
  //local variables
  title: string[] = ['Panel operatorski',''];
  LogOutButtonDisable: boolean = false;

  constructor(private router: Router) { }

  ngOnInit() {
  }


  //on router link activation
  onActivate(event) {
    if (event.route.snapshot.queryParams['socket'] == "saw") {
      this.title[0] = "Panel operatora piły"
    } else if (event.route.snapshot.queryParams['socket'] == "plasma") {
      this.title[0] = "Panel operatora plazmy"
    } else {
      this.title[0] = "Panel operatorski"
    }
    this.title[1] = event.title;
    if (event.title == 'Ekran logowania')
      this.LogOutButtonDisable = true;
    else
      this.LogOutButtonDisable = false;
    //console.log(event);
  }

  // on log out button pressed
  onLogOut() {
    this.router.navigate(['/production/log']);
  }

}
