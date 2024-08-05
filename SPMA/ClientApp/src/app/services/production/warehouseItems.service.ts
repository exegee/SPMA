import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InventorComponent } from '../../models/production/inventorcomponent.model';
import { WarehouseItem } from '../../models/production/warehouseItem.model';

@Injectable({
  providedIn: 'root'
})
export class WarehouseItemsService {

  constructor(private http: HttpClient) { }

  getWarehouseItems() {
    return this.http.get('/api/warehouseitem/getItems');
  }

  addWarehouseItem(newSurplusItem: WarehouseItem) {
    return this.http.post('/api/warehouseitem/postItem', newSurplusItem);
  }

  editWarehouseItem(editedSurplusItem: WarehouseItem) {
    return this.http.put('/api/warehouseitem/edit', editedSurplusItem);
  }

  copyWarehouseItem(copyItem: WarehouseItem) {
    return this.http.post('/api/warehouseitem/copyItem', copyItem.warehouseItemId);
  }

  deleteWarehouseItem(surplusItemId: number) {
    return this.http.delete(`/api/warehouseitem/delete/${surplusItemId}`);
  }

  getComponentsFromWarehouseItems(component: InventorComponent) {
    return this.http.get(`/api/warehouseitem/reservation`, {
      params: {
         componentNumber: component.number
      }
    })
  }
}
