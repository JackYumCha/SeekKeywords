import { OnInit, OnDestroy } from "@angular/core";
import { Router, NavigationEnd } from '@angular/router';
import { Subject } from "rxjs/Subject";
import { isFunction } from "util";
import { InitDestroyComponent } from "./init-destroy-component";
import 'rxjs/add/operator/takeUntil';
import 'rxjs/add/operator/filter';

export class RouterEndComponent extends InitDestroyComponent {

    $destroy: Subject<boolean> = new Subject<boolean>();

    constructor(private router: Router){
        super();
        this.router.events.takeUntil(this.$destroy).filter(e => e instanceof NavigationEnd).subscribe(e => this.onNavigationEnd(<any>e));
    }
    
    onNavigationEnd = (e: NavigationEnd) => {
        console.warn('Please implement your onNavigationEnd for RouterEndComponent; or else you should use InitDestroyComponent.');
    }
}