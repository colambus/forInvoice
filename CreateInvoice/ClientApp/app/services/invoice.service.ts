import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, RequestOptions } from '@angular/http';
import { InvoiceModel } from '../models/invoice.model';
import 'rxjs/Rx';
import { Headers, URLSearchParams } from '@angular/http';

@Injectable()
export class InvoiceService {
    private actionUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.actionUrl = baseUrl + '/api/Invoice/';
    }

    getAll(): Observable<InvoiceModel[]> {
        return this.http.get(this.actionUrl + 'GetAll')
            .map((response: Response) => {
                return <InvoiceModel[]>response.json();
            });
    }

    create(): Observable<InvoiceModel> {
        return this.http.get(this.actionUrl + 'CreateNewInvoice')
            .map((response: Response) => {
                return <InvoiceModel>response.json();
            });
    }

    deleteItem(id: number) {
        return this.http.delete(this.actionUrl + '/' + id);
    };
}