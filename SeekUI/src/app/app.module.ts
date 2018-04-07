import { JobInterceptor } from './services/mvc-api/job.interceptor';
import { JobService } from './services/mvc-api/services/SeekAPI.Controllers.Job.Service';
import { AppRouterModule } from './app.router';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { JobAnalysisListComponent } from './job-analysis-list/job-analysis-list.component';
import { JobAnalysisEntryComponent } from './job-analysis-entry/job-analysis-entry.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BarChartComponent } from './bar-chart/bar-chart.component';

@NgModule({
  declarations: [
    AppComponent,
    JobAnalysisListComponent,
    JobAnalysisEntryComponent,
    BarChartComponent
  ],
  imports: [
    AppRouterModule, BrowserModule, HttpClientModule, FormsModule, RouterModule
  ],
  providers: [
    JobService,
    // interceptor for job api
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JobInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
