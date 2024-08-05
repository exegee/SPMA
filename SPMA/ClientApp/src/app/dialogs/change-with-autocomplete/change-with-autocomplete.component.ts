import { AfterViewInit, Component, Inject, OnInit, Pipe, PipeTransform, ViewEncapsulation } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { OptimaItem } from '../../models/optima/optimaitem.model';
import { OptimaMag } from '../../models/optima/optimamag.model';
import { OptimaService } from '../../services/optima/optima.service';


@Component({
  selector: 'app-change-with-autocomplete',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './change-with-autocomplete.component.html',
  styleUrls: ['./change-with-autocomplete.component.css' ]
})
export class ChangeWithAutocompleteComponent implements OnInit {

  materialNameInput=new FormControl('');
  isLoading: boolean = false;
  enableConfirmButton: boolean = false;
  filteredOptimaItems: OptimaItem[];
  selectedMag: OptimaMag = new OptimaMag(0, "WSZYSTKIE", "WSZYSTKIE");
  



  constructor(public dialogRef: MatDialogRef<ChangeWithAutocompleteComponent>,
   private optimaService: OptimaService) { }

  ngOnInit() {

    this.materialNameInput.valueChanges
      .pipe(
        debounceTime(300),
        tap((value) => {
          if (value.length >= 3) {
            this.isLoading = true;
          }
        }
        ),
        switchMap(value => value.length >= 3 ? this.optimaService.searchItem(value, this.selectedMag).pipe(finalize(() => {
          this.isLoading = false

        })) : []),
        finalize(() => {
          this.isLoading = false
        })
      ).subscribe(items => {
        this.filteredOptimaItems = items;
        //this.filteredOptimaItems.length == 0 ? this.enableConfirmButton = false : this.enableConfirmButton = true;
        //console.log(this.enableConfirmButton);
      }, () => { });

  }



  // Optima standard component display
  searchOptimaWarehouseDisplay(item: OptimaItem) {
    if (item) { return item.twr_Kod; }
  }

  onSelectedOptimaItem(event) {
    //console.log(event.target.value);
  }


  onYesClick(): void {
    console.log(this.materialNameInput.value);
    this.dialogRef.close(this.materialNameInput.value);
  }

}


// Pipes
@Pipe({ name: 'highlight' })
export class HighlightPipe implements PipeTransform {
  transform(value: string, search: string, uppercase: boolean): string {
    const pattern = new RegExp(search, 'gi');

    if (value != null)
      var result = value.replace(pattern, uppercase ? `<b>${search.toUpperCase()}</b>` : `<b>${search}</b>`);
    else
      result = '';

    return result;
  }
}
