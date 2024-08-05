
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InProduction } from '../../models/production/inproduction.model';
import { InventorComponent } from '../../models/production/inventorcomponent.model';


@Injectable()
export class ComponentService {

  constructor(private http: HttpClient) { }

  addOrModifyComponent(component: InventorComponent) {
    return this.http.post('/api/components/', component);
  }

  async getComponentWare(number: string): Promise<InventorComponent> {
    return await this.http.get<InventorComponent>(`/api/components/ware/${number}`).toPromise();
  }

  async getComponentWare2(component: InventorComponent): Promise<InventorComponent> {
    return await this.http.patch<InventorComponent>('/api/components/ware', component).toPromise();
  }

  deleteComponentWare(id: number) {
    const url = `/api/components/ware/${id}`;
    return this.http.delete(url);
  }

  addComponent(component: InventorComponent) {
    return this.http.post('/api/components', component);
  }

  addWare(component: InventorComponent) {
    return this.http.post('/api/components/ware', component);
  }

  getInProductionComponents(component: InventorComponent) {
    return this.http.patch<InProduction[]>('/api/components/inproduction', component);
  }

  updateComponent(component: InventorComponent) {
    return this.http.patch('/api/components', component);
  }

  getFilteredComponents(filterStr: string) {
    return this.http.get(`/api/components/filteredcomponents/${filterStr}`);
  }
}
