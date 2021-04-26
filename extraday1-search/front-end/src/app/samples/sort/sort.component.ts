import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/service/common.service';

@Component({
  selector: 'app-sort',
  templateUrl: './sort.component.html',
  styleUrls: ['./sort.component.scss'],
})
export class SortComponent implements OnInit {
  isSpinning = false;
  entityTypes = ['list', 'driveItem'];

  constructor(private commonService: CommonService) {}

  ngOnInit(): void {}

  loading = false;

  searchInput1 = '';

  showConfiguration = false;

  sortPropertiesList = [
    { value: 'Created', label: 'Created' },
    { value: 'Summary', label: 'Summary' },
    { value: 'displayName', label: 'displayName' },
    { value: 'id', label: 'id' },
    { value: 'lastModifiedDateTime', label: 'lastModifiedDateTime' },
  ];

  selectedASCSortPropertiesList: string[] = ['Created', 'Summary'];

  selectedDESCSortPropertiesList: string[] = [];

  data: any;

  encodeUri(input: string): string {
    return encodeURI(input);
  }

  executeSearch1(input: string) {
    if (this.searchInput1 == '') {
      alert('Search term cannot be empty');
      return;
    }

    var intersection: string[] = this.selectedASCSortPropertiesList.filter(
      (x) => this.selectedDESCSortPropertiesList.indexOf(x) > -1
    );

    if (intersection.length > 0) {
      alert('Fields cannot included in desc and asc list at same time');
      return;
    }

    this.isSpinning = true;
    this.commonService
      .Search(
        this.searchInput1,
        this.entityTypes,
        this.selectedASCSortPropertiesList
      )
      .subscribe((data) => {
        this.data = data;
        this.isSpinning = false;
      });
  }

  setEntityTypes(value: string[]): void {
    this.entityTypes = value;
  }

  configOpen(): void {
    this.showConfiguration = true;
  }

  configClose(): void {
    this.showConfiguration = false;
  }

  select(ret: {}): void {
    console.log('nzSelectChange', ret);
  }

  change(ret: {}): void {
    console.log('nzChange', ret);
  }
}
