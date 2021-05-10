import { Component, OnInit } from '@angular/core';
import { CommonService } from 'src/app/service/common.service';
import * as ACData from "adaptivecards-templating";
import * as AdaptiveCards from "adaptivecards";
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';


@Component({
  selector: 'app-display-template',
  templateUrl: './display-template.component.html',
  styleUrls: ['./display-template.component.scss']
})
export class DisplayTemplateComponent implements OnInit {

  
  isSpinning = false;
  showCode = false;
  entityTypes = ['list', 'driveItem'];

  constructor(private commonService: CommonService, private sanitizer: DomSanitizer) {}

  ngOnInit(): void {}

  loading = false;

  searchInput1 = '';

  showConfiguration = false;

  data: any;

  test:any;

  templateDictionary:any;

  renderTemplates:any[] = [];

  encodeUri(input: string): string {
    return encodeURI(input);
  }
  // key: template Id  value:template Body
  renderedACTList = new Map<string, string>();

  executeSearch(input: string) {

    /*if (this.searchInput1 == '') {
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
      });*/

      this.data = this.getMockData();
      this.renderTemplates = [];
      this.templateDictionary = this.data["value"][0]["resultTemplates"];
      for(let hit of this.data["value"][0]["hitsContainers"]["0"]["hits"]){
        this.renderTemplates.push(this.renderACT(hit));
      }
      console.log(this.renderTemplates);
  }


 renderACT(hit){
var templateId = hit["resultTemplateId"];
  // Define a template payload
var templatePayload = this.templateDictionary[templateId]["body"];
var template = new ACData.Template(templatePayload);

// Expand the template with your `$root` data object.
// This binds it to the data and produces the final Adaptive Card payload
var context = new ACData.EvaluationContext();
context.$root = hit.resource;
var card = template.expand(context);

// OPTIONAL: Render the card (requires that the adaptivecards library be loaded)
var adaptiveCard = new AdaptiveCards.AdaptiveCard();
adaptiveCard.parse(card);
return this.sanitizer.bypassSecurityTrustHtml(adaptiveCard.render().outerHTML);
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



  getMockData(){
    return {
      "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#Collection(microsoft.graph.searchResponse)",
      "value": [
          {
              "searchTerms": [],
              "hitsContainers": [
                  {
                      "total": 1201701,
                      "moreResultsAvailable": true,
                      "hits": [
                          {
                              "hitId": "85437765-b430-434f-a945-38eceead5b93",
                              "rank": 1,
                              "summary": "",
                              "resultTemplateId": "1603900360618_5XCBK2OXG",
                              "resource": {
                                  "@odata.type": "#microsoft.graph.externalItem",
                                  "id": "B5B6E9C7-152C-4478-BCCB-CEF053F17397",
                                  "Title": "[SM00186] Number of tests - Liquid",
                                  "URL": "https://liquid.microsoft.com/Web/Object/Read/scanningtoolwarnings/Requirements/CodeQL.SM00186"
                              }
                          },
                          {
                              "hitId": "85437765-5430-434f-a945-38eceead5b94",
                              "rank": 2,
                              "summary": "",
                              "resultTemplateId": "1603900360618_5XCBK2OXP",
                              "resource": {
                                  "@odata.type": "#microsoft.graph.externalItem",
                                  "title": "Publish Adaptive Card Schema",
                                  "description": "Now that we have defined the main rules and features of the format, we need to produce a schema and publish it to GitHub. The schema will be the starting point of our reference documentation.",
                                  "creator": {
                                      "name": "Matt Hidinger",
                                      "profileImage": "https://pbs.twimg.com/profile_images/3647943215/d7f12830b3c17a5a9e4afcc370e3a37e_400x400.jpeg"
                                  },
                                  "createdUtc": "2017-02-14T06:08:39Z",
                                  "viewUrl": "https://adaptivecards.io",
                                  "properties": [
                                      {
                                          "key": "Board",
                                          "value": "Adaptive Cards"
                                      },
                                      {
                                          "key": "List",
                                          "value": "Backlog"
                                      },
                                      {
                                          "key": "Assigned to",
                                          "value": "Matt Hidinger"
                                      },
                                      {
                                          "key": "Due date",
                                          "value": "Not set"
                                      }
                                  ]
                              }
                          },
                          {
                              "hitId": "85437765-b430-434f-a945-38eceead5b95",
                              "rank": 3,
                              "summary": "",
                              "resultTemplateId": "1603900360618_5XCBK2OXG",
                              "resource": {
                                  "@odata.type": "#microsoft.graph.externalItem",
                                  "id": "B5B6E9C7-152C-4478-BCCB-CEF053F17399",
                                  "Title": "[SM00186] Number of tests - Liquid 3",
                                  "URL": "https://liquid.microsoft.com/Web/Object/Read/scanningtoolwarnings/Requirements/CodeQL.SM00186"
                              }
                          }
                      ]
                  }
              ],
              "resultTemplates": {
                  "1603900360618_5XCBK2OXG": {
                      "displayName": "Liquid-3",
                      "body": {
                          "type": "AdaptiveCard",
                          "version": "1.0",
                          "body": [
                              {
                                  "type": "ColumnSet",
                                  "columns": [
                                      {
                                          "type": "Column",
                                          "width": "auto",
                                          "items": [
                                              {
                                                  "type": "Image",
                                                  "url": "https://searchuxcdn.azureedge.net/designerapp/images/LiquidLogo.png",
                                                  "horizontalAlignment": "Center",
                                                  "size": "Small"
                                              }
                                          ],
                                          "horizontalAlignment": "Center"
                                      },
                                      {
                                          "type": "Column",
                                          "width": 10,
                                          "items": [
                                              {
                                                  "type": "TextBlock",
                                                  "text": "[{Title}]({URL})",
                                                  "weight": "Bolder",
                                                  "color": "Accent",
                                                  "size": "Medium",
                                                  "maxLines": 3
                                              },
                                              {
                                                  "type": "TextBlock",
                                                  "text": "{ResultSnippet}",
                                                  "maxLines": 3,
                                                  "wrap": true
                                              }
                                          ],
                                          "spacing": "Medium"
                                      }
                                  ]
                              }
                          ],
                          "$schema": "http://adaptivecards.io/schemas/adaptive-card.json"
                      }
                  },
                  "1603900360618_5XCBK2OXP": {
                      "displayName": "Liquid-2",
                      "body": {
                        "type": "AdaptiveCard",
                        "body": [
                            {
                                "type": "TextBlock",
                                "size": "Medium",
                                "weight": "Bolder",
                                "text": "{title}"
                            },
                            {
                                "type": "ColumnSet",
                                "columns": [
                                    {
                                        "type": "Column",
                                        "items": [
                                            {
                                                "type": "Image",
                                                "style": "Person",
                                                "url": "{creator.profileImage}",
                                                "size": "Small"
                                            }
                                        ],
                                        "width": "auto"
                                    },
                                    {
                                        "type": "Column",
                                        "items": [
                                            {
                                                "type": "TextBlock",
                                                "weight": "Bolder",
                                                "text": "{creator.name}",
                                                "wrap": true
                                            },
                                            {
                                                "type": "TextBlock",
                                                "spacing": "None",
                                                "text": "Created {{DATE({createdUtc},SHORT)}}",
                                                "isSubtle": true,
                                                "wrap": true
                                            }
                                        ],
                                        "width": "stretch"
                                    }
                                ]
                            },
                            {
                                "type": "TextBlock",
                                "text": "{description}",
                                "wrap": true
                            },
                            {
                                "type": "FactSet",
                                "facts": [
                                    {
                                        "$data": "{properties}",
                                        "title": "{key}:",
                                        "value": "{value}"
                                    }
                                ]
                            }
                        ],
                        "actions": [
                            {
                                "type": "Action.ShowCard",
                                "title": "Set due date",
                                "card": {
                                    "type": "AdaptiveCard",
                                    "body": [
                                        {
                                            "type": "Input.Date",
                                            "id": "dueDate"
                                        },
                                        {
                                            "type": "Input.Text",
                                            "id": "comment",
                                            "placeholder": "Add a comment",
                                            "isMultiline": true
                                        }
                                    ],
                                    "actions": [
                                        {
                                            "type": "Action.Submit",
                                            "title": "OK"
                                        }
                                    ],
                                    "$schema": "http://adaptivecards.io/schemas/adaptive-card.json"
                                }
                            },
                            {
                                "type": "Action.OpenUrl",
                                "title": "View",
                                "url": "{viewUrl}"
                            }
                        ],
                        "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
                        "version": "1.3"
                    }
                  }
              }
          }
      ]
  };

  }
}
