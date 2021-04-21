import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonComponent } from './samples/common/common.component';
import { MeetingComponent } from './samples/meeting/meeting.component';
import { SortV2Component } from './samples/sort-v2/sort-v2.component';
import { SortComponent } from './samples/sort/sort.component';
import { WelcomeComponent } from './samples/welcome/welcome.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/common' },
  { path: 'common', pathMatch: 'full', component:CommonComponent },
  { path: 'meeting', pathMatch: 'full', component:MeetingComponent },
  { path: 'sort', pathMatch: 'full', component:SortComponent },
  { path: 'sortv2', pathMatch: 'full', component:SortV2Component },
  { path: 'welcome', pathMatch: 'full', component:WelcomeComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
