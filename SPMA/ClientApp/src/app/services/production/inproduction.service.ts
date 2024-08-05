import { state } from '@angular/animations';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BookComponent } from '../../models/books/bookcomponent.model';
import { OptimaItem } from '../../models/optima/optimaitem.model';
import { InProduction } from '../../models/production/inproduction.model';
import { InProductionRW } from '../../models/production/inproductionrw.model';
import { Ware } from '../../models/warehouse/ware.model';
import { BookListFromOrders } from '../../shared/Interfaces/BookListFromOrders';

@Injectable()
export class InProductionService {

  constructor(private http: HttpClient) { }

  addToProduction(inProduction: InProduction) {
    return this.http.post('/api/inproduction', inProduction);
  }

  addBookToProduction(bookComponent: BookComponent) {
    return this.http.post('/api/inproduction/book', bookComponent);
  }

  getInProductionRWByInProductionId(inProductionId: number) {
    return this.http.get(`/api/inproduction/rw/${inProductionId}`);
  }

  async getInProductionRWByInProductionIdAsync(inProductionId: number): Promise<InProductionRW> {
    return await this.http.get<InProductionRW>(`/api/inproduction/rw/${inProductionId}`).toPromise();
  }

  async GetComponentsWithSameMaterialFromOrderAsync(wareid: number, sourceType: number):
    Promise<BookListFromOrders[]> {
    return await this.http.get<BookListFromOrders[]>
      (`/api/inproduction/saw/samematerialinorder`, {
        params: new HttpParams()
          .set('wareId', wareid.toString())
          .set('sourceType', sourceType.toString())
      }).toPromise();
  }

  changeProductionState(statecode: number, inproductionid: number) {
    return this.http.patch(`/api/inproduction/statechange`, '', {
      params: {
        stateCode: statecode.toString(),
        inProductionId: inproductionid.toString()
      }
    });
  }

  changeLengthwithOverflow(inProductionRWId: number, newLength: number) {
    return this.http.patch(`/api/inproduction/overflowlengthchange`, '',
      {
        params: {
          inProductionRWId: inProductionRWId.toString(),
          newLength: newLength.toString()
        }
      })
  }

  changeTotalQty(inProductionRWId: number, newQty: number) {
    return this.http.patch(`/api/inproduction/quantitychange`, '',
      {
        params: {
          inProductionRWId: inProductionRWId.toString(),
          newQty: newQty.toString()
        }
      })
  }

  changeMaterialType(rwid: number, ware: Ware) {
    return this.http.patch(`/api/inproduction/changeware`, ware,
      {
        params: {
          RWid: rwid.toString()
        }
      })
  }

}
