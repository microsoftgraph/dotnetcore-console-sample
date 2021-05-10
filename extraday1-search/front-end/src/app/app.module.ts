import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import { registerLocaleData, DatePipe } from '@angular/common';
import en from '@angular/common/locales/en';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { IconsProviderModule } from './icons-provider.module';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { MeetingComponent } from './samples/meeting/meeting.component';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { SortComponent } from './samples/sort/sort.component';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { CommonComponent } from './samples/common/common.component';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzTransferModule } from 'ng-zorro-antd/transfer';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { SortV2Component } from './samples/sort-v2/sort-v2.component';
import { WelcomeComponent } from './samples/welcome/welcome.component';
import { SpellerComponent } from './samples/speller/speller.component';
import { SemanticLabelComponent } from './samples/semantic-label/semantic-label.component';
import { DisplayTemplateComponent } from './samples/display-template/display-template.component';
import { IconDefinition } from '@ant-design/icons-angular';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { MarkdownModule } from 'ngx-markdown';
import { SecurityContext } from '@angular/core';

// import icons
import * as AllIcons from '@ant-design/icons-angular/icons';
const antDesignIcons = AllIcons as {
   [key: string]: IconDefinition;
 };
const icons: IconDefinition[] = Object.keys(antDesignIcons).map(key => antDesignIcons[key])

registerLocaleData(en);

@NgModule({
  declarations: [
    AppComponent,
    MeetingComponent,
    SortComponent,
    CommonComponent,
    SortV2Component,
    WelcomeComponent,
    SpellerComponent,
    SemanticLabelComponent,
    DisplayTemplateComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    IconsProviderModule,
    NzLayoutModule,
    NzMenuModule,
    NzInputModule,
    NzGridModule,
    NzIconModule.forRoot(icons),
    NzTabsModule,
    NzButtonModule,
    NzCardModule,
    NzDrawerModule,
    NzSpinModule,
    NzListModule,
    NzCheckboxModule,
    NzTransferModule,
    NzSelectModule,
    NzDatePickerModule,
    NzSelectModule,
    NzSwitchModule,
    MarkdownModule.forRoot({ 
      loader: HttpClient,
      sanitize: SecurityContext.NONE 
    }),
  ],
  providers: [{ provide: NZ_I18N, useValue: en_US }, DatePipe],
  bootstrap: [AppComponent],
})
export class AppModule {}
