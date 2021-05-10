import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonComponent } from './samples/common/common.component';
import { DisplayTemplateComponent } from './samples/display-template/display-template.component';
import { MeetingComponent } from './samples/meeting/meeting.component';
import { SemanticLabelComponent } from './samples/semantic-label/semantic-label.component';
import { SortV2Component } from './samples/sort-v2/sort-v2.component';
import { SortComponent } from './samples/sort/sort.component';
import { SpellerComponent } from './samples/speller/speller.component';
import { WelcomeComponent } from './samples/welcome/welcome.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'welcome' },
  { path: 'common', pathMatch: 'full', component: CommonComponent },
  { path: 'meeting', pathMatch: 'full', component: MeetingComponent },
  //{ path: 'sort', pathMatch: 'full', component:SortComponent },
  { path: 'sort', pathMatch: 'full', component: SortV2Component },
  { path: 'welcome', pathMatch: 'full', component: WelcomeComponent },
  { path: 'speller', pathMatch: 'full', component: SpellerComponent },
  { path: 'semanticLabel', pathMatch: 'full', component: SemanticLabelComponent },
  { path: 'displayTemplate', pathMatch: 'full', component: DisplayTemplateComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
