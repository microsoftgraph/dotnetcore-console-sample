import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/service/common.service';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-sort-v2',
  templateUrl: './sort-v2.component.html',
  styleUrls: ['./sort-v2.component.scss'],
})
export class SortV2Component implements OnInit {
  entityTypes = ['list', 'driveItem'];

  isSpinning = false;
  data: any;
  showConfiguration = false;
  showCode = false;

  visible = false;

  selectedSortPropertiesList: string[] = [];

  sortPropertiesList: Array<{ value: string; label: string }> = [];

  validateForm!: FormGroup;
  listOfControl: Array<{ id: number; controlInstance: string }> = [];

  constructor(private commonService: CommonService) {}

  ngOnInit(): void {}

  searchInput1 = '';
  loading = false;
  hitObjects: any;
  response = '';
  isDescending = false;
  fieldInput = '';
  token = '';

  executeSearch(input: string) {
    if (this.searchInput1 == '') {
      alert('Search term cannot be empty');
      return;
    }

    this.isSpinning = true;
    this.commonService
      .GetSortResult(
        this.searchInput1,
        this.entityTypes,
        this.selectedSortPropertiesList
      )
      .subscribe(
        (data) => {
          this.data = data;
          this.isSpinning = false;
        },
        (error) => {
          this.isSpinning = false;
          alert(error['message']);
        }
      );
  }

  encodeUri(uri: string) {
    return encodeURI(uri);
  }

  submitAndGetSortResult() {
    this.visible = false;
    this.executeSearch(this.searchInput1);
  }

  formatEmptyOrNullString(resourceProperty: string) {
    if (resourceProperty == null || resourceProperty == '') {
      return 'null';
    }
    return resourceProperty;
  }

  returnWebUrlIfNameIsnull(hitObject: any) {
    var name = hitObject.resource.name;
    if (name == '' || name == null) {
      return 'null';
    }
    return name;
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

  setEntityTypes(value: string[]): void {
    this.entityTypes = value;
  }

  AddToselectedSortPropertiesList() {
    if (this.fieldInput != '') {
      var sortOrder = 'ascending';
      if (String(this.isDescending).includes('true')) {
        sortOrder = 'descending';
      }
      var sortProperty = this.fieldInput + ': ' + sortOrder;
      if (!this.selectedSortPropertiesList.includes(sortProperty)) {
        this.selectedSortPropertiesList.push(sortProperty);
      }
    }
    if (this.selectedSortPropertiesList.length > 0)
      this.sortPropertiesList = this.selectedSortPropertiesList.map((item) => {
        return {
          value: item,
          label: item,
        };
      });
  }
  CSharp_Code = `
  GraphServiceClient graphClient = new GraphServiceClient( authProvider );

  var requests = new List<SearchRequestObject>()
  {
    new SearchRequestObject
    {
      EntityTypes = new List<EntityType>()
      {
        EntityType.DriveItem
      },
      Query = new SearchQuery
      {
        QueryString = "*"
      },
      SortProperties = new List<SortProperty>()
      {
        new SortProperty
        {
          Name = "lastModifiedDateTime",
          IsDescending = true
        }
      }
    }
  };
  
  await graphClient.Search
    .Query(requests,null)
    .Request()
    .PostAsync();
 `;

  highlight() {
    var renderCSharpCode = this.CSharp_Code.replace(
      new RegExp('var|await|null|new'),
      (match) => {
        return '<span class="highlightText">' + match + '</span>';
      }
    );
    return renderCSharpCode;
  }
}
