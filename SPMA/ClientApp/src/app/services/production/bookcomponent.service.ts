import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { SubOrderItem } from '../../models/suborders/suborderitem.model';
import { BookComponent } from '../../models/books/bookcomponent.model';

@Injectable()
export class BookComponentService {

  constructor(private http: HttpClient) { }

  //get single comonent specifide by bookId and component Id
  getBookComponent(bookId: number, componentId: number) {
    return this.http.get('/api/bookcomponent/',
      {
        params: {
          bookId: String(bookId),
          componentId: String(componentId)
        }
      });
  }
  //get single comonent specifide by bookId and component Id
  async getBookComponentAsync(subOrder: SubOrderItem, bookid: number): Promise<BookComponent> {
    return await this.http.get<BookComponent>('/api/bookcomponent/',
      {
        params: {
          bookId: String(bookid),
          componentId: String(subOrder.inProduction.component.componentId)
        }
      }).toPromise();
  }

  //get number of spicifide component
  getComponentNumber(officeNumber: string) {
    return this.http.get('/api/bookcomponent/getcomponentnumber', {
      params: {
        officenumber: officeNumber
      }
    })
  }

}
