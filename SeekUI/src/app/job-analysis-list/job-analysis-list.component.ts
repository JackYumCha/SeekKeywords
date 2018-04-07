import { JobAnalysis } from './../services/mvc-api/datatypes/JobModel.Entities.JobAnalysis';
import { Router, NavigationEnd } from '@angular/router';
import { RouterEndComponent } from './../shared/router-end-component';
import { Component, OnInit } from '@angular/core';
import { JobService } from '../services/mvc-api/services/SeekAPI.Controllers.Job.Service';

@Component({
  selector: 'app-job-analysis-list',
  templateUrl: './job-analysis-list.component.html',
  styleUrls: ['./job-analysis-list.component.scss']
})
export class JobAnalysisListComponent extends RouterEndComponent {

  list: JobAnalysis[];

  constructor(router: Router, private jobService: JobService) {
    super(router);
  }

  onNavigationEnd = (e: NavigationEnd) => {
    this.jobService.ListJobAnalysis().takeUntil(this.$destroy).subscribe(this.onJobAnalysisList);
  }

  onJobAnalysisList = (list:JobAnalysis[]) => {
    this.list = list;
    console.log(this.list);
  }
  
}
