import { NavigationEnd } from '@angular/router';

export module RouterExtensions {
    export function getSections(e: NavigationEnd) {
        var sections = e.url.split('/');
        return sections.reverse();
    }
}