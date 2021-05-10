import { DatePipe } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CommonService {
  headers: any;
  searchUri = 'https://localhost:44383/api/Search';
  constructor(private http: HttpClient, private datePipe: DatePipe) {}

  // search meeting
  Search(
    query: string,
    entityTypes: string[],
    sortASCPropertiesList?: string[],
    sortDESCPropertiesList?: string[]
  ) {
    /*
    User Token, get a test token from Graph Explorer https://developer.microsoft.com/en-us/graph/graph-explorer,
    For client's application, please refer https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/auth-oidc
    */
    this.headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Custom-Token': this.getMockToken(),
    });

    var requestBody: any = {
      requests: [
        {
          entityTypes: entityTypes,
          query: {
            queryString: query,
          },
          size: 10,
        },
      ],
    };

    if (
      (sortASCPropertiesList && sortASCPropertiesList.length > 0) ||
      (sortDESCPropertiesList && sortDESCPropertiesList.length > 0)
    ) {
      var sortProperties: string[] = this.buildSortProperties(
        sortASCPropertiesList,
        sortDESCPropertiesList
      );
      requestBody = {
        requests: [
          {
            entityTypes: entityTypes,
            query: {
              queryString: query,
            },
            size: 10,
            sortProperties: sortProperties,
          },
        ],
      };
    }

    return this.http.post(this.searchUri, requestBody, {
      headers: this.headers,
    });
  }

  // search meeting
  SearchMeeting(query: string, startDate?: Date, endDate?: Date) {
    this.headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Custom-Token': this.getMockToken(),
    });

    var requestBody: any = {
      requests: [
        {
          entityTypes: ['event'],
          query: {
            queryString: query,
          },
          size: 10,
        },
      ],
    };

    if (startDate || endDate) {
      requestBody = {
        requests: [
          {
            entityTypes: ['event'],
            query: {
              queryString: query,
            },
            size: 10,
            Filter: this.buildDateRange(startDate, endDate),
          },
        ],
      };
    }

    return this.http.post(this.searchUri, requestBody, {
      headers: this.headers,
    });
  }


// Speller Request
SearchWithSpeller(query:string, entityTypes: string[], enableModification:boolean){
  this.headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Custom-Token': this.getMockToken(),
  });

  var requestBody: any = {
    requests: [
      {
        entityTypes: entityTypes,
        query: {
          queryString: query,
        },
        size: 10,
      },
    ],
    "queryAlterationOptions":{
      "enableSuggestion":true,
      "enableModification":enableModification
    }
  };

  console.log("request body:",requestBody);

  return this.http.post(
    this.searchUri,
     requestBody, {
    headers: this.headers,
  });
}












  // get meeting details
  getMeetingDetails(eventId: String) {
    eventId = eventId.replace(new RegExp('/', 'g'), '-');
    return this.http.get(
      'https://localhost:44383/api/Event?eventId=' + eventId,
      { headers: this.headers }
    );
  }

  // set Mock token
  // only for test
  setMockToken(token: string) {
    localStorage.setItem('token', token);
  }

  getMockToken(): string {
    return localStorage.getItem('token');
  }

  buildSortProperties(
    ascProperties: string[],
    descProperties: string[]
  ): string[] {
    var result: any[] = [];

    if (ascProperties)
      for (let item of ascProperties) {
        result.push({
          name: item,
          isDescending: false,
        });
      }

    if (descProperties)
      for (let item of descProperties) {
        result.push({
          name: item,
          isDescending: true,
        });
      }

    return result;
  }

  buildDateRange(startDate: Date, endDate: Date): any {
    var dateRange = [];
    if (startDate) {
      dateRange.push({
        Range: {
          StartTime: {
            gte: this.getYearMonthDay(startDate),
          },
        },
      });
    }

    if (endDate) {
      dateRange.push({
        Range: {
          EndTime: {
            lte: this.getYearMonthDay(startDate),
          },
        },
      });
    }

    return {
      And: dateRange,
    };
  }

  getYearMonthDay(date: Date): string {
    return this.datePipe.transform(date, 'yyyy-MM-dd');
  }

  //get sort result
  GetSortResult(
    query: string,
    entityTypes: string[],
    selectedSortPropertiesList: string[]
  ) {
    this.headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Custom-Token': this.getMockToken(),
    });
    var requestBody: any;
    if (selectedSortPropertiesList && selectedSortPropertiesList.length > 0) {
      var sortProperties: string[] = this.buildSortPropertiesFromList(
        selectedSortPropertiesList
      );
      requestBody = {
        requests: [
          {
            entityTypes: entityTypes,
            query: {
              queryString: query,
            },
            size: 10,
            sortProperties: sortProperties,
          },
        ],
      };
    } else {
      requestBody = {
        requests: [
          {
            entityTypes: entityTypes,
            query: {
              queryString: query,
            },
            size: 10,
          },
        ],
      };
    }
    return this.http.post(
      'https://graph.microsoft.com/beta/search/query',
      requestBody,
      { headers: this.headers }
    );
  }

  buildSortPropertiesFromList(sortPropertiesList: string[]): string[] {
    var result: any[] = [];

    if (sortPropertiesList)
      for (let sortItem of sortPropertiesList) {
        var itemArray = sortItem.split(':');
        var isDescending = itemArray[1].includes('descending') ? true : false;
        result.push({
          name: itemArray[0],
          isDescending: isDescending,
        });
      }
    return result;
  }
}
