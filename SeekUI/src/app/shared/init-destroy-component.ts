import { OnInit, OnDestroy } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { isFunction } from "util";

export class InitDestroyComponent implements OnInit, OnDestroy{

    $destroy: Subject<boolean> = new Subject<boolean>();

    constructor(){
    }

    ngOnInit(){
        this.onInit();
    }

    onInit(){

    }

    ngOnDestroy(){
        this.$destroy.next(true);
        this.onDestroy();
    }

    onDestroy() {

    }
}