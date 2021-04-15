
import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/service/common.service';

@Component({
  selector: 'app-common',
  templateUrl: './common.component.html',
  styleUrls: ['./common.component.scss']
})
export class CommonComponent implements OnInit {
  isSpinning = false;
  entityTypes = ["list","driveItem"];

  constructor(private commonService:CommonService) { }

  ngOnInit(): void {
  }

  loading = false;

  searchInput1="";

  showConfiguration = false;

  
  data:any;

encodeUri(input:string):string{
  return encodeURI(input);
}

executeSearch1(input:string){
  if(this.searchInput1 == ""){
    alert("Search term cannot be empty");
    return;
  }

  console.log("entity types:",this.entityTypes);
  this.isSpinning = true;
  this.commonService.Search(this.searchInput1, this.entityTypes).subscribe(data=>{
    this.data = data;
    this.isSpinning = false;
    console.log(data);
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

}


