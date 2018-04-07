import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-bar-chart',
  templateUrl: './bar-chart.component.html',
  styleUrls: ['./bar-chart.component.scss']
})
export class BarChartComponent implements OnInit {

  entries: IEntry[] = [];

  padding: number = 12;
  rowHeight: number = 32;
  rowWidth: number = 720;
  maxValue: number = 1;

  constructor() { }

  ngOnInit() {
  }

  /** accept data from parent component */
  @Input('data') set data(value: {[key:string]:number}) {
    this.entries = [];
    for(let key in value){
      this.entries.push({
        key: key,
        value: value[key]
      });
    }
    this.entries = this.entries.sort((a, b)=> b.value - a.value);
    console.log('entries:', this.entries);
  }

  @Input('max') set max(value: number) {
    this.maxValue = value;
    console.log('max:', this.max);
  }

  get width(): number {
    return this.padding * 2 + this.rowWidth;
  }

  get heigth(): number {
    return this.padding * 2 + this.rowHeight * this.entries.length;
  }

  get viewBox(): string {
    return `0,0,${this.width},${this.heigth}`;
  }

  get viewHeight():string {
    return `${this.heigth}px`;
  }

  get viewWidth():string {
    return `${this.width}px`;
  }

  get bodyOffset(): string{
    return `translate(${this.padding},${this.padding})`;
  }

  entryOffset(entryIndex: number) {
    return `translate(${0},${entryIndex * this.rowHeight})`;
  }

  get rectHeight(): number {
    return this.rowHeight * 0.8;
  }

  get rectTop(): number {
    return this.rowHeight * 0.1;
  }

  rectWidth(entry: IEntry):number {
    return this.rowWidth * entry.value / this.maxValue;
  }

  get textTop(): number {
    return this.rowHeight * 0.5;
  }
}

interface IEntry {key: string, value: number};