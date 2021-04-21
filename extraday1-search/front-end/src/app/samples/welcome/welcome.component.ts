import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/service/common.service';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.scss']
})
export class WelcomeComponent implements OnInit {

  tokenValue:string;
  isSpinning:boolean = false;
  constructor(private commonService:CommonService) { }

  ngOnInit(): void {
  }

  setToken(): void {
    this.commonService.setMockToken(this.tokenValue);
    alert("succeed");
  }
  
}
