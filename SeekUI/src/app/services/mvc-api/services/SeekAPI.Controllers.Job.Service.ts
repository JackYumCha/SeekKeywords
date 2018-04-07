/** 
 * Auto Generated Code
 * Please do not modify this file manually 
 * Assembly Name: "SeekAPI"
 * Source Type: "D:\VSTS\Repos\Machine Learning Lecture\Projects\Seek\SeekAPI\SeekAPI\bin\Debug\netcoreapp2.0\SeekAPI.dll"
 * Generated At: 2018-04-07 14:36:33.684
 */
import { JobAnalysis } from '../datatypes/JobModel.Entities.JobAnalysis';
import { JobAnalysisEntry } from '../datatypes/JobModel.Entities.JobAnalysisEntry';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
@Injectable()
export class JobService {
	constructor(private $httpClient: HttpClient) {{}}
	public $baseURL: string = '<SeekAPI>';
	public ListJobAnalysis(): Observable<JobAnalysis[]> {
		return this.$httpClient.post<JobAnalysis[]>(this.$baseURL + 'Job/ListJobAnalysis', null, {});
	}
	public GetJobAnalysis(jobAnalysisKey: string): Observable<JobAnalysis> {
		let $data = new FormData();
		$data.append('jobAnalysisKey', jobAnalysisKey);
		return this.$httpClient.post<JobAnalysis>(this.$baseURL + 'Job/GetJobAnalysis', $data, {});
	}
	public ListJobAnalysisEntries(jobAnalysisKey: string): Observable<JobAnalysisEntry[]> {
		let $data = new FormData();
		$data.append('jobAnalysisKey', jobAnalysisKey);
		return this.$httpClient.post<JobAnalysisEntry[]>(this.$baseURL + 'Job/ListJobAnalysisEntries', $data, {});
	}
	public GetJobAnalysisEntry(jobAnalysisEntryKey: string): Observable<JobAnalysisEntry> {
		let $data = new FormData();
		$data.append('jobAnalysisEntryKey', jobAnalysisEntryKey);
		return this.$httpClient.post<JobAnalysisEntry>(this.$baseURL + 'Job/GetJobAnalysisEntry', $data, {});
	}
}
