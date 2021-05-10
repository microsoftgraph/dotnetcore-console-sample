import { stringify } from '@angular/compiler/src/util';
import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/service/common.service';

@Component({
  selector: 'app-speller',
  templateUrl: './speller.component.html',
  styleUrls: ['./speller.component.scss']
})
export class SpellerComponent implements OnInit {
  isSpinning = false;
  entityTypes = ['message'];

  constructor(private commonService: CommonService) {}

  ngOnInit(): void {}

  loading = false;

  showCode = false;

  searchInput = '';

  spellerSuggestion = "";

  showCorrect = false;

  enableModification = true;

  showConfiguration = false;

  data: any;

  encodeUri(input: string): string {
    if(input == 'undefined' || input == ""  ) return null;
    return encodeURI(input);
  }

  executeSearch(input: string) {
    if (this.searchInput == '') {
      alert('Search term cannot be empty');
      return;
    }
    this.isSpinning = true;
    this.commonService
      .SearchWithSpeller(this.searchInput, this.entityTypes, this.enableModification)
      .subscribe((data) => {
        this.data = data;
        this.isSpinning = false;
        console.log(data);
        this.spellerSuggestion = this.data["queryAlterationResponse"]["queryAlteration"]["alteredQueryString"];
        if(this.spellerSuggestion.toLocaleLowerCase() != this.searchInput.toLocaleLowerCase()){
          this.showCorrect = true;
        }else{
          this.showCorrect = false;
        }

      }, error=>{
        this.isSpinning = false;
        alert(error["message"]);
      });
  }

  excuteRawSearch(){
    this.enableModification = false;
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

  codeOpen(): void {
    this.showCode = true;
  }

  codeClose(): void {
    this.showCode = false;
  }



}
