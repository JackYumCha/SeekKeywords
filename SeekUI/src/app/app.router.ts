import { JobAnalysisListComponent } from './job-analysis-list/job-analysis-list.component';
import { Routes, RouterModule } from "@angular/router";
import { JobAnalysisEntryComponent } from './job-analysis-entry/job-analysis-entry.component';

const appRoutes:Routes = [
    {
        path: '',
        redirectTo: 'job-analysis-list',
        pathMatch: 'full'
    },
    {
        path: 'job-analysis-list',
        component: JobAnalysisListComponent
    },
    {
        path: 'job-analysis-entry/:analysis/:entry',
        component: JobAnalysisEntryComponent
    }
];

export var AppRouterModule = RouterModule.forRoot(appRoutes);