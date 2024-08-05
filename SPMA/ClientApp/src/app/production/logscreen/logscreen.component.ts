import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-logscreen',
  templateUrl: './logscreen.component.html',
  styleUrls: ['./logscreen.component.css']
})
export class LogscreenComponent implements OnInit {

  hide: boolean = true;//for password eye icon
  //for parent component "productioncomponent"
  title: string = 'Ekran logowania';

  constructor(private router: Router, private route: ActivatedRoute) { 
}

  ngOnInit() {
    //console.log('fine');
  }


  onLogIn() {
    this.router.navigate(["../selectionScreen"], {relativeTo : this.route});
  }


}
