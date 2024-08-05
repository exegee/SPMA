import { Book } from './book.model';


export class PagedBook {
  public results: Book[];
  public rowCount: number;

  constructor(results: Book[] = [], rowCount: number = 0) {
    results = this.results;
    rowCount = this.rowCount;
  }
}
