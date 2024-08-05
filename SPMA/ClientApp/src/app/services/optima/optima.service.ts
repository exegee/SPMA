import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { OptimaClient } from '../../models/optima/optimaclient.model';
import { OptimaItem } from '../../models/optima/optimaitem.model';
import { OptimaMag } from '../../models/optima/optimamag.model';
import { OptimaRW } from '../../models/optima/optimarw.model';

@Injectable()
export class OptimaService {

  constructor(private http:HttpClient) { }

  searchItem(filter: string, mag: OptimaMag) {
    return this.http.get('api/Optima', {
      params: new HttpParams()
        .set('filter', filter)
        .set('magName', mag.mag_Symbol)
        .set('magIdStr', mag.mag_MagId.toString())
    })
    .pipe(
      map((res: OptimaItem[]) => {
        //console.log(res);
      return res;
      })
    )
  }

  searchClient(filter: string) {
    return this.http.get('api/Optima/searchClient', {
      params: new HttpParams()
        .set('filter', filter)
    }).pipe(
      map((res: OptimaClient[]) => {
        return res;
      })
    )
  }

  getItem(wareCode: string) {
    return this.http.get('api/Optima/bywarecode', {
      headers: new HttpHeaders({
        'wareCode': wareCode
      })
    });
  }

  getItemQty(wareCode: string, rwDate: string, magName: string) {
      return this.http.get<number>('api/Optima/getWarehouseItemQty', {
          params: {
            wareCode: wareCode,
            rwDate: rwDate,
            magName: magName
      }
    });
    
  }

  async getRWSInfo(orderNumber: string, subOrderNumber: string, componentNumber: string): Promise<OptimaRW[]> {
    return await this.http.get<OptimaRW[]>('api/Optima/getRWNumber', {
      params: {
        orderNumber: orderNumber,
        subOrderNumber: subOrderNumber,
        componentNumber: componentNumber
      }
    }).toPromise();
  }

  async getMags(extendedList: boolean): Promise<OptimaMag[]> {
    return await this.http.get<OptimaMag[]>('api/Optima/getMags', {
      params: {
        extendedListStr: String(extendedList)
      }
    }).toPromise();
  }
}
