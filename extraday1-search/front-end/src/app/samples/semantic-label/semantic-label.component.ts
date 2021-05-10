import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/service/common.service';

@Component({
  selector: 'app-semantic-label',
  templateUrl: './semantic-label.component.html',
  styleUrls: ['./semantic-label.component.scss']
})
export class SemanticLabelComponent implements OnInit {
  isSpinning = false;
  entityTypes = ['list', 'driveItem'];

  constructor(private commonService: CommonService) {}

  ngOnInit(): void {}

  loading = false;

  searchInput1 = '';

  showConfiguration = false;

  data: any;

  encodeUri(input: string): string {
    return encodeURI(input);
  }

  executeSearch(input: string) {
    if (this.searchInput1 == '') {
      alert('Search term cannot be empty');
      return;
    }
    this.isSpinning = true;
    this.commonService
      .Search(this.searchInput1, this.entityTypes)
      .subscribe((data) => {
        this.data = data;
        this.isSpinning = false;
      }, error=>{
        this.isSpinning = false;
        alert(error["message"]);
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

  getMockData(){}
}
