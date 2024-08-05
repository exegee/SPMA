import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort, MatTable } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ConfirmDeleteComponent } from '../../dialogs/confirm-delete/confirm-delete.component';
import { Book } from '../../models/books/book.model';
import { BookService } from '../../services/books/book.service';
import { BooksDataSource } from './books-datasource';

@Component({
  selector: 'app-books-list',
  templateUrl: './books-list.component.html',
  styleUrls: ['./books-list.component.css']
})
export class BooksListComponent implements OnInit {

  dataSource: BooksDataSource;
  books: Book[];
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatTable, { static: true }) table: MatTable<any>;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('searchInput', { static: true }) searchInput: ElementRef;

  constructor(private bookService: BookService, public dialog: MatDialog, private route: ActivatedRoute) { }

  displayedColumns = ["Position", "OfficeNumber", "Name", "ComponentNumber", "ModifiedDate", "Actions"];
  displayedColumnsHeaders = ['Numer'];


  ngOnInit() {
    this.dataSource = new BooksDataSource(this.bookService);
    this.dataSource.getBooks('', 'asc',0,15, '');
  }

  ngAfterViewInit() {
    fromEvent(this.searchInput.nativeElement, 'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.loadBooksPage();
        })
      )
      .subscribe();

    this.paginator.page
      .pipe(
        tap(() => this.loadBooksPage())
      )
      .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => this.loadBooks())
      )
      .subscribe();
  }

  loadBooksPage() {
    this.dataSource.getBooks(this.sort.active, this.sort.direction, this.paginator.pageIndex, this.paginator.pageSize, this.searchInput.nativeElement.value);
  }

  loadBooks() {
    this.dataSource.getBooks(this.sort.active, this.sort.direction, 0, 15, '');
  }
  onDeleteBook(book: Book) {
    const dialogRef = this.dialog.open(ConfirmDeleteComponent, {
      width: '500px',
      data: { message: `Czy na pewno chcesz usunąć książkę ${book.componentNumber} (${book.officeNumber})` }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result == true) {
        this.dataSource.deleteBook(book.bookId);
      }
    })
  }

  onReload() {
    this.dataSource.refresh();
    this.searchInput.nativeElement.value = '';
    this.paginator.pageIndex = 0;
  }
}
