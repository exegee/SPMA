import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-import-stock',
  templateUrl: './import-stock.component.html',
  styleUrls: ['./import-stock.component.css']
})
export class ImportStockComponent implements OnInit {

  importingData: boolean = false;
  progress: number = 0;
  progressString: string = "0";
  message: string = "";
  selectedFile: File = null;

  fileInput = new FormControl(null, [Validators.required]);

  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  onFileSelected(event) {
    this.selectedFile = <File>event.target.files[0];
    this.fileInput.setValue(this.selectedFile.name);
  }

  onUpload() {
    const formData = new FormData();
    formData.append(this.selectedFile.name, this.selectedFile);
    this.http.post(
      `api/upload`,
      formData, {
      headers: new HttpHeaders({ 'FileUploadPath': 'xlsPIT' }),
      reportProgress: true,
      observe: 'events'
    }).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
        this.message = this.progress.toString();
      }
      else if (event.type === HttpEventType.Response) {
        if (event.ok) {
          this.message = event.body.toString();
          setTimeout(() => {
            this.importingData = true;
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
          this.importingData = false;
        }
      }
    )
  }

}
