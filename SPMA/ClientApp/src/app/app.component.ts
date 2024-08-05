import { Component, OnInit } from '@angular/core';
import * as feather from 'feather-icons';
import { ServicesService } from './services/services/services.service';


interface navBarOption {
  display: string,
  paddingLeft: string,
  paddingTop: string
}


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Stolarczyk';
  navBarOpt: navBarOption = { display: "block", paddingLeft: "200px", paddingTop:"50px"};

  constructor(private servicesService: ServicesService) {
        //TODO uruchomic z opoznieniem usluge!!!
    // Turns services on startup, id will be used later???
    //setTimeout(() => {
    //  this.servicesService.serviceToggle(0, true).subscribe();
    //},30000)
  }

  ngOnInit() {
    feather.replace();

  }

  onActivate(event) {
    if (event.hasOwnProperty("title")) {
      let str: string = event.title[0];
      if (str.includes("Panel")) {
        this.navBarOpt = { display: "none", paddingLeft: "0px", paddingTop: "0px"};
      }
    } else {
      this.navBarOpt = { display: "block", paddingLeft: "200px", paddingTop: "50px" };
    }
  }

}
