import { CollectionViewer } from '@angular/cdk/collections';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { Book } from '../../models/books/book.model';
import { PagedBook } from '../../models/books/pagedbook.model';
import { BookService } from '../../services/books/book.service';


export class BooksDataSource implements DataSource<Book>{

  private booksSubject = new BehaviorSubject<Book[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  data: Book[];
  dataLength: number;

  public loading$ = this.loadingSubject.asObservable();

  constructor(private bookService: BookService) { }

  connect(collectionViewer: CollectionViewer): Observable<Book[]> {
    return this.booksSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.booksSubject.complete();
    this.loadingSubject.complete();
  }

  getBooks(orderBy: string, sortOrder = 'asc', page: number, pageSize: number, filter: string) {
    this.loadingSubject.next(true);

    this.bookService.getBooks(orderBy, sortOrder, page, pageSize, filter).pipe(
      catchError(() => of([])),
      finalize(() => this.loadingSubject.next(false))
    )
      .subscribe((pagedBook: PagedBook) => {
        this.data = pagedBook.results as Book[];
        this.booksSubject.next(pagedBook.results);
        this.dataLength = pagedBook.rowCount;
      })
  }

  deleteBook(id: number) {
    this.bookService.deleteBook(id)
      .subscribe();
    let books: any[] = this.booksSubject.getValue();
    books.forEach((item: Book, index) => {
      if (item.bookId == id) {
        books.splice(index, 1);
      }
    })
    this.booksSubject.next(books);
  }

  refresh() {
    this.getBooks('', 'asc', 0, 15, '');
  }
}
