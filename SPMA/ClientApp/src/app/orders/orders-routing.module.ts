import { NgModule } from "@angular/core";
import { Routes, RouterModule } from '@angular/router';
import { OrdersComponent } from './orders.component';
import { OrdersListComponent } from './orders-list/orders-list.component';
import { BookImportComponent } from './book-import/book-import.component';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { BookAddComponent } from './book-add/book-add.component';
import { BooksListComponent } from './books-list/books-list.component';
import { SuborderEditComponent } from './suborder-edit/suborder-edit.component';
import { OrdersArchiveComponent } from './orders-archive/orders-archive.component';

const ordersRoutes: Routes = [
  {
    path: '', component: OrdersComponent, children: [
      { path: '', component: OrdersListComponent, pathMatch: 'full' },
    { path: 'import', component: BookImportComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
    { path: 'add', component: BookAddComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
      { path: 'bookslist', component: BooksListComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
      //{ path: 'archive', component: OrdersArchiveComponent, runGuardsAndResolvers: 'always', pathMatch: '', },
    { path: 'suborder/:id/edit', component: SuborderEditComponent, runGuardsAndResolvers: 'always', pathMatch: 'full', },
    { path: ':id', component: OrderDetailComponent, pathMatch: 'full' }
    ]
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(ordersRoutes)
  ],
  exports: [RouterModule]
})

export class OrdersRoutingModule {}
