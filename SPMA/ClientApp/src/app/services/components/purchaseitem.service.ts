import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ComponentWare } from '../../models/components/componentware.model';
import { InventorComponent } from '../../models/production/inventorcomponent.model';
import { Ware } from '../../models/warehouse/ware.model';



@Injectable()
export class PurchaseItemService {

  constructor(private http: HttpClient) { }

  addPurchaseItemWare(componentWare: ComponentWare) {
    return this.http.post('/api/purchaseitem/ware', componentWare);
  }

  //async getPurchaseItemWare(number: string): Promise<Ware[]> {
  //  return await this.http.get<Ware[]>('/api/purchaseitem/ware', {
  //    headers: new HttpHeaders({
  //      "Accept-Language": "pl",
  //      "Content-Language": "pl",
  //      'number': test
  //    })
  //  }).toPromise();
  //}
  async getPurchaseItemWare(component: InventorComponent): Promise<Ware> {
    return await this.http.patch<Ware>('/api/purchaseitem/ware', component).toPromise();
  }

  deletePurchaseItemWare(id: number) {
    const url = `/api/purchaseitem/ware/${id}`;
    return this.http.delete(url);
  }

}
