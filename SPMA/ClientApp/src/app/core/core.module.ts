import { NgModule } from "@angular/core";
import { FormsModule } from '@angular/forms';
import { MatAutocompleteModule, MatButtonModule, MatExpansionModule, MatTooltipModule } from '@angular/material';
import { MatListModule } from '@angular/material/list';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ContextMenuModule } from 'ngx-contextmenu';
import { AppRoutingModule } from '../app-routing.module';
import { BookService } from '../services/books/book.service';
import { ComponentService } from '../services/components/component.service';
import { PurchaseItemService } from '../services/components/purchaseitem.service';
import { OptimaService } from '../services/optima/optima.service';
import { OrdersService } from '../services/orders/orders.service';
import { InProductionService } from '../services/production/inproduction.service';
import { PdfService } from '../services/utility/pdf.service';
import { XmlService } from '../services/utility/xml.service';
import { SharedModule } from '../shared/shared.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeaderComponent } from './header/header.component';
import { SidenavComponent } from './sidenav/sidenav.component';
import { SubOrderService } from '../services/suborder/suborder.service';
import { BookComponentService } from '../services/production/bookcomponent.service';
import { ServicesService } from '../services/services/services.service';
import { StockItemsService } from "../services/stocktaking/stockitems.service";


@NgModule({
  declarations: [
    HeaderComponent,
    SidenavComponent,
    DashboardComponent
  ],
  imports: [
    SharedModule,
    AppRoutingModule,
    MatExpansionModule,
    MatButtonModule,
    MatListModule,
    MatSnackBarModule,
    FormsModule,
    NgbModule,
    ContextMenuModule.forRoot({
      useBootstrap4: true,
    }),
    MatTooltipModule
  ],
  exports: [
    HeaderComponent,
    SidenavComponent,
  ],
  providers: [
    OrdersService,
    OptimaService,
    ComponentService,
    BookService,
    PdfService,
    XmlService,
    PurchaseItemService,
    InProductionService,
    SubOrderService,
    BookComponentService,
    ServicesService,
    StockItemsService
  ]
})

export class CoreModule { }
