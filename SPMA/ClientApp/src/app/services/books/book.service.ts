import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Book } from '../../models/books/book.model';
import { BookComponent } from '../../models/books/bookcomponent.model';
import { PagedBook } from '../../models/books/pagedbook.model';

export interface newSubOderNumber {
  value: string;
}

@Injectable()
export class BookService {
  constructor(private http: HttpClient) { }

  addComponentsToBook(bookComponent: BookComponent) {
    return this.http.post<Book>('/api/book/components', bookComponent);
  }
  addPurchaseItemsToBook(bookComponent: BookComponent) {
    return this.http.post('/api/book/addpurchaseitems', bookComponent);
  }
  addPlazmaItemsToBook(bookComponent: BookComponent) {
    return this.http.post('/api/book/addplazmaitems', bookComponent);
  }

  genSubOrderNumber(orderId: number) {
    return this.http.get<newSubOderNumber>(`/api/book/generatenumber/${orderId}`);
  }

  checkIfBookExist(bookComponent: BookComponent) {
    return this.http.patch('/api/book/addcomponents/checkifbookexist', bookComponent);
  }

  async getBook(arg1: Book): Promise<BookComponent>;
  async getBook(arg1: string, arg2: string): Promise<BookComponent>;
  async getBook(arg1: any, arg2?: string) {
    var componentNumber;
    var officeNumber;
    if (arg1 instanceof Book) {
      componentNumber = arg1.componentNumber;
      officeNumber = arg1.officeNumber;
    }
    else {
      componentNumber = arg1;
      officeNumber = arg2;
    }
    return await this.http.get<BookComponent>('/api/book', {
      params: new HttpParams()
        .set('componentNumber', componentNumber)
        .set('officeNumber', officeNumber)
    }).toPromise();
  }

  getBooks(orderBy: string, sortOrder: string, page: number, pageSize: number, filter: string) {
    return this.http.get('/api/book/getpagedbooks', {
      params: new HttpParams()
        .set('orderBy', orderBy)
        .set('sortOrder', sortOrder)
        .set('page', page.toString())
        .set('pageSize', pageSize.toString())
        .set('filter', filter)
    })
      .pipe(
        map((res: PagedBook) => {
          return res;
        })
      )
  }

  getBooksCount() {
    return this.http.get('/api/book/bookscount');
  }

  deleteBook(id: number) {
    return this.http.delete(`/api/book/${id.toString()}`);
  }

  getSingleComponent(bookId: number, componentId: number) {
    return this.http.get('/api/book/getsinglecomponent', {
      params: new HttpParams()
        .set('bookId', bookId.toString())
        .set('componentId', componentId.toString())
    })
  }

  async getBookInfoAsync(id: number) {
    return await this.http.get<Book>(`/api/book/${id.toString()}/info`).toPromise();
  }

}
