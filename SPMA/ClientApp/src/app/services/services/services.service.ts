import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { text } from '@fortawesome/fontawesome-svg-core';
import { map } from 'rxjs/operators';
import { Service } from '../../models/core/service.model';

@Injectable()
export class ServicesService {
  constructor(private http: HttpClient) { }

  getServices() {
    return this.http.get('/api/service').pipe(map((res: Service[]) => {
      return res;
    }));
  }
  serviceToggle(id: number, serviceStatus: boolean) {
    // id not used, maybe later ???
    return this.http.post('/api/service/SubOrderRWCompletionCheck', null, {
      params: {
        serviceStatusChange: String(serviceStatus)
      }
    });
  }

  getServiceJob() {
    return this.http.get('/api/service/SubOrderRWCompletionCheck/currentjob', {
      responseType:  "text"    });
  }
}
