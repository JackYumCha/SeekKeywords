import { JobAnalysis } from './../services/mvc-api/datatypes/JobModel.Entities.JobAnalysis';
import { JobService } from './../services/mvc-api/services/SeekAPI.Controllers.Job.Service';
import { Router, NavigationEnd } from '@angular/router';
import { RouterEndComponent } from './../shared/router-end-component';
import { Component, OnInit } from '@angular/core';
import { RouterExtensions } from '../shared/router-extensions';
import { JobAnalysisEntry } from '../services/mvc-api/datatypes/JobModel.Entities.JobAnalysisEntry';

@Component({
  selector: 'app-job-analysis-entry',
  templateUrl: './job-analysis-entry.component.html',
  styleUrls: ['./job-analysis-entry.component.scss']
})
export class JobAnalysisEntryComponent extends RouterEndComponent {

  analysisKey: string;
  entryKey: string;
  list: JobAnalysisEntry[];
  jobAnalysis: JobAnalysis;
  jobAnalysisEntry: JobAnalysisEntry;

  constructor(router: Router, private jobService: JobService) {
    super(router);
  }

  onNavigationEnd = (e: NavigationEnd) => {
    let sections = RouterExtensions.getSections(e);
    this.analysisKey = sections[1];
    this.entryKey = sections[0];
    this.jobService.GetJobAnalysis(this.analysisKey).takeUntil(this.$destroy).subscribe(this.onJobAnalysis);
    this.jobService.ListJobAnalysisEntries(this.analysisKey).takeUntil(this.$destroy).subscribe(this.onJobAnalysisEntries);
  }

  onJobAnalysis = (jobAnalysis: JobAnalysis) =>{
    this.jobAnalysis = jobAnalysis;
  }

  onJobAnalysisEntries = (list: JobAnalysisEntry[])=>{
    this.list = list;
    console.log('list:', this.list);
    let key;
    if(this.list.length == 0) 
    {
      return;
    }
    if(this.entryKey == 'latest') {
      key = this.list[0]._key;
    }
    else{
      let found = this.list.filter(entry => entry._key == this.entryKey);
      if(found.length > 0 ){
        key = found[0]._key;
      }
      else{
        key = this.list[0]._key;
      }
    }
    this.jobService.GetJobAnalysisEntry(key).takeUntil(this.$destroy).subscribe(this.onJobAnalysisEntry);
  }

  onJobAnalysisEntry = (jobAnalysisEntry: JobAnalysisEntry)=>{
    this.jobAnalysisEntry = jobAnalysisEntry;
    console.log('jobAnalysisEntry:', this.jobAnalysisEntry);
  }
}
