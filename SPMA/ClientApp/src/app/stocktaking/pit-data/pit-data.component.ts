import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { StockItemsService } from '../../services/stocktaking/stockitems.service';
import * as FileSaver from 'file-saver';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';

@Component({
  selector: 'app-pit-data',
  templateUrl: './pit-data.component.html',
  styleUrls: ['./pit-data.component.css'],
  providers: [DatePipe]
})
export class PitDataComponent implements OnInit {

  stockItemsCount: number;
  isImportingStockItems: boolean;
  isExportingStockItems: boolean;
  isDeletingStockItems: boolean;
  selectedFile: File = null;

  constructor(private http: HttpClient, private stockItemsService: StockItemsService) { }

  ngOnInit() {
    this.checkStockItemsCount();
  }

  onFileImport(event) {
    this.selectedFile = <File>event.target.files[0];

    const formData = new FormData();
    formData.append(this.selectedFile.name, this.selectedFile);
    this.http.post(
      `api/upload`,
      formData, {
      headers: new HttpHeaders({ 'FileUploadPath': 'xlsPIT' }),
      reportProgress: true,
      observe: 'events'
    }).subscribe(event => {
        if (event.type === HttpEventType.Response) {
        if (event.ok) {
          setTimeout(() => {
            this.isImportingStockItems = true;
            this.importToDatabase();
          }, 2000);
        }
      }
    });
  }

  importToDatabase() {
    this.http.get<boolean>('api/StockImport', {
      params: {
        fileName: this.selectedFile.name
      }
    }).subscribe(
      (response) => {
        if (response) {
          this.isImportingStockItems = false;
          this.checkStockItemsCount();
        }
      }
    )
  }

  onDeleteStockItems() {
    this.isDeletingStockItems = true;
    this.stockItemsService.deleteStockItems()
      .subscribe(result => {
        this.isDeletingStockItems = false;
        this.checkStockItemsCount();
      })
  }

  checkStockItemsCount() {
    this.stockItemsService.getStockItemsCount()
      .subscribe(count => {
        this.stockItemsCount = count;
      });
  }

  exportToTxt() {
    this.isExportingStockItems = true;
    this.stockItemsService.exportStockItems()
      .subscribe(
        (response) => {
          var date = moment(new Date()).format('YYYY');
          var data = new Blob([response], { type: 'text/plain;charset=ANSI' });
          
          FileSaver.saveAs(data, 'Arkusz inwentaryzacyjny ' + date + ".txt");
          this.isExportingStockItems = false;
        });
  }


}
