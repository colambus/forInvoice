import { Component } from '@angular/core';
import { Data } from '@angular/router/src/config';
import { getToday } from '@progress/kendo-angular-dateinputs/dist/es2015/util';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    invoiceDate: Date = getToday();
    public organizations: Array<string> = [
        'Baseball', 'Basketball', 'Cricket', 'Field Hockey',
        'Football', 'Table Tennis', 'Tennis', 'Volleyball'
    ];
    paymentIdentification: Array<string>;

}
