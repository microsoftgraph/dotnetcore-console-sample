import { DatePipe } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommonService {
  accessToken= "eyJ0eXAiOiJKV1QiLCJub25jZSI6InNCd2xxcUs5UUZYWGIzMEdId2IzYWU3c05lYWhLU1lnTU83Qm1PemdFTFEiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNjE4NDkwMjI3LCJuYmYiOjE2MTg0OTAyMjcsImV4cCI6MTYxODQ5NDEyNywiYWNjdCI6MCwiYWNyIjoiMSIsImFjcnMiOlsidXJuOnVzZXI6cmVnaXN0ZXJzZWN1cml0eWluZm8iLCJ1cm46bWljcm9zb2Z0OnJlcTIiLCJ1cm46bWljcm9zb2Z0OnJlcTMiLCJjMSIsImMyIiwiYzMiLCJjNCIsImM1IiwiYzYiLCJjNyIsImM4IiwiYzkiLCJjMTAiLCJjMTEiLCJjMTIiLCJjMTMiLCJjMTQiLCJjMTUiLCJjMTYiLCJjMTciLCJjMTgiLCJjMTkiLCJjMjAiLCJjMjEiLCJjMjIiLCJjMjMiLCJjMjQiLCJjMjUiXSwiYWlvIjoiQVVRQXUvOFRBQUFBTytkclZvTTVRRW8yM0Zjd0N0TkMxNUgvSHJzR0hlTWpjblAvdjl1N0g5K1I4UGNuWUp2U1EvUVJjMDA3VDNXcTdsTXVLZUlmb053Q0ZOaExHU0w2T0E9PSIsImFtciI6WyJyc2EiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiR3JhcGggZXhwbG9yZXIgKG9mZmljaWFsIHNpdGUpIiwiYXBwaWQiOiJkZThiYzhiNS1kOWY5LTQ4YjEtYThhZC1iNzQ4ZGE3MjUwNjQiLCJhcHBpZGFjciI6IjAiLCJjb250cm9scyI6WyJhcHBfcmVzIl0sImNvbnRyb2xzX2F1ZHMiOlsiZGU4YmM4YjUtZDlmOS00OGIxLWE4YWQtYjc0OGRhNzI1MDY0IiwiMDAwMDAwMDMtMDAwMC0wMDAwLWMwMDAtMDAwMDAwMDAwMDAwIiwiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIl0sImRldmljZWlkIjoiY2M2ZjQ1OWUtNWQ5OS00N2YzLWJlYTEtMDQzMTIxNWQ3NTI3IiwiZmFtaWx5X25hbWUiOiJXYW5nIChNU0FJKSIsImdpdmVuX25hbWUiOiJZaXdlbiIsImlkdHlwIjoidXNlciIsImlwYWRkciI6IjE2Ny4yMjAuMjMyLjQyIiwibmFtZSI6Illpd2VuIFdhbmcgKE1TQUkpIiwib2lkIjoiNDk3YjdhMmEtOWUxYS00OGQ3LTgwZTgtMjk2NWQyZmMzYTgxIiwib25wcmVtX3NpZCI6IlMtMS01LTIxLTIxNDY3NzMwODUtOTAzMzYzMjg1LTcxOTM0NDcwNy0yNjkxMjgwIiwicGxhdGYiOiIzIiwicHVpZCI6IjEwMDMyMDAxMDYxQTY3NzIiLCJyaCI6IjAuQVFFQXY0ajVjdkdHcjBHUnF5MTgwQkhiUjdYSWk5NzUyYkZJcUsyM1NOcHlVR1FhQUQ0LiIsInNjcCI6IkNhbGVuZGFycy5SZWFkV3JpdGUgQ29udGFjdHMuUmVhZFdyaXRlIERldmljZU1hbmFnZW1lbnRBcHBzLlJlYWRXcml0ZS5BbGwgRGV2aWNlTWFuYWdlbWVudENvbmZpZ3VyYXRpb24uUmVhZC5BbGwgRGV2aWNlTWFuYWdlbWVudENvbmZpZ3VyYXRpb24uUmVhZFdyaXRlLkFsbCBEZXZpY2VNYW5hZ2VtZW50TWFuYWdlZERldmljZXMuUHJpdmlsZWdlZE9wZXJhdGlvbnMuQWxsIERldmljZU1hbmFnZW1lbnRNYW5hZ2VkRGV2aWNlcy5SZWFkLkFsbCBEZXZpY2VNYW5hZ2VtZW50TWFuYWdlZERldmljZXMuUmVhZFdyaXRlLkFsbCBEZXZpY2VNYW5hZ2VtZW50UkJBQy5SZWFkLkFsbCBEZXZpY2VNYW5hZ2VtZW50UkJBQy5SZWFkV3JpdGUuQWxsIERldmljZU1hbmFnZW1lbnRTZXJ2aWNlQ29uZmlnLlJlYWQuQWxsIERldmljZU1hbmFnZW1lbnRTZXJ2aWNlQ29uZmlnLlJlYWRXcml0ZS5BbGwgRGlyZWN0b3J5LkFjY2Vzc0FzVXNlci5BbGwgRGlyZWN0b3J5LlJlYWRXcml0ZS5BbGwgRmlsZXMuUmVhZFdyaXRlLkFsbCBHcm91cC5SZWFkV3JpdGUuQWxsIElkZW50aXR5Umlza0V2ZW50LlJlYWQuQWxsIE1haWwuUmVhZFdyaXRlIE1haWxib3hTZXR0aW5ncy5SZWFkV3JpdGUgTm90ZXMuUmVhZFdyaXRlLkFsbCBvcGVuaWQgUGVvcGxlLlJlYWQgUHJlc2VuY2UuUmVhZCBQcmVzZW5jZS5SZWFkLkFsbCBwcm9maWxlIFJlcG9ydHMuUmVhZC5BbGwgU2l0ZXMuUmVhZFdyaXRlLkFsbCBUYXNrcy5SZWFkV3JpdGUgVXNlci5SZWFkIFVzZXIuUmVhZEJhc2ljLkFsbCBVc2VyLlJlYWRXcml0ZSBVc2VyLlJlYWRXcml0ZS5BbGwgZW1haWwiLCJzaWduaW5fc3RhdGUiOlsiZHZjX21uZ2QiLCJkdmNfY21wIiwiZHZjX2RtamQiLCJpbmtub3dubnR3ayIsImttc2kiXSwic3ViIjoiRldqb014STdJcm9uUlVsX0hqNXVSbk9wWWs5ZGJvYWpJeGpwYVNpN3hzNCIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJXVyIsInRpZCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsInVuaXF1ZV9uYW1lIjoieWl3ZW53YW5nQG1pY3Jvc29mdC5jb20iLCJ1cG4iOiJ5aXdlbndhbmdAbWljcm9zb2Z0LmNvbSIsInV0aSI6IlVYaThPc2J1NkUtakdIdlJpeElCQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfc3QiOnsic3ViIjoieDNzS1FBOWtCRkZBVXdsYy1sbXpxNExCWTJibzFuSGk3ZzAydTR3bzU1WSJ9LCJ4bXNfdGNkdCI6MTI4OTI0MTU0N30.dwdNLHb5kmF-sioZif0KLRb1WjN_wc72qlqN0IBM71FysjmGx43efOMwYxAvbkJbR1mVGwvyIK_zYB8C8LpIkqOfCdtVsN1Iw_giwIDYrrhd5TrypAVNX_tnNcSdac_aIrf4DsqwfStzSLDNKIbzimnnmfUpRCT7Ko3FXIsdGNbrOznfzHhePAhUXdM2cjqLZp9_MwuGi7r56E459j3M_d4NnYncTxa-CsT6pDkiu3CWwP8ZANN58Y60tKiEN6MB8smblH-T7xvYyaIU0vZ2XLFG_u-1GarYsAum5GtsQOb1Erhl2QGtmzAb4Ks_X7e8ISxgMDM6UFei2Ou45se6yw";
  headers:any;
  authHeader:any;
  searchUri = "https://graph.microsoft.com/beta/search/query";
  constructor(private http:HttpClient, private datePipe: DatePipe) { 
    this.headers = new HttpHeaders({'Content-Type': 'application/json'});
    this.authHeader =  new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization':'Bearer '+this.accessToken,
      "Access-Control-Allow-Origin":"*"
    });
  }

  // search meeting
  Search(query:string, entityTypes:string[], sortASCPropertiesList?:string[], sortDESCPropertiesList?:string[]){
    var requestBody:any = {
      "requests": [
          {
              "entityTypes": entityTypes,
              "query": {
                  "queryString": query
              },
              "size": 10
          }
      ]
  }

  if(   (sortASCPropertiesList && sortASCPropertiesList.length > 0)    ||   (sortDESCPropertiesList && sortDESCPropertiesList.length > 0) ){
     var sortProperties:string[] = this.buildSortProperties(sortASCPropertiesList,sortDESCPropertiesList);
    requestBody = {
      "requests": [
          {
              "entityTypes": entityTypes,
              "query": {
                  "queryString": query
              },
              "size": 10,
              "sortProperties":sortProperties
          }
      ]
  }
  }

  console.log("requestBody:",requestBody);
 
    return this.http.post(
      this.searchUri,
      requestBody,
      {headers:this.authHeader}
    );

  }

   // search meeting
   SearchMeeting(query:string, startDate?:Date, endDate?:Date){
    var requestBody:any = {
      "requests": [
          {
              "entityTypes": [
                  "event"
              ],
              "query": {
                  "queryString": query
              },
              "size": 10
          }
      ]
  }

  if(startDate || endDate){


    requestBody = {
      "requests": [
          {
              "entityTypes": [
                  "event"
              ],
              "query": {
                  "queryString": query
              },
              "size": 10,
              "Filter":this.buildDateRange(startDate,endDate)
          }
      ]
  }

  }
 
    return this.http.post(
      this.searchUri,
      requestBody,
      {headers:this.authHeader}
    );

  }
// get meeting details
  getMeetingDetails(eventId:String){

    eventId = eventId.replace(new RegExp("\/","g"),"-");
    return this.http.get(
      "https://graph.microsoft.com/v1.0/me/events/"+eventId+"?useICalUId=false&eventType=&eventStartDateTime=",
      {headers:this.authHeader}
    );
  }

  buildSortProperties(ascProperties:string[], descProperties:string[]):string[]{

    var result:any[] = [];

    if(ascProperties)
    for(let item of ascProperties){
      result.push({
        "name":item,
        "isDescending":false
      });
    }

    if(descProperties)
    for(let item of descProperties){
      result.push({
        "name":item,
        "isDescending":true
      });
    }

    return result;
  }

  buildDateRange(startDate:Date, endDate:Date):any{
    var dateRange = [];
    if(startDate){
      dateRange.push({
        "Range":{
          "StartTime":{
            "gte":this.getYearMonthDay(startDate)
          }
        }
      });
    }

    if(endDate){

      dateRange.push({
        "Range":{
          "EndTime":{
            "lte":this.getYearMonthDay(startDate)
          }
        }
      });
    }

    return {
      "And":dateRange
    };

  }

  getYearMonthDay(date:Date):string{
    return this.datePipe.transform(date, "yyyy-MM-dd");
  }
  
  
}
