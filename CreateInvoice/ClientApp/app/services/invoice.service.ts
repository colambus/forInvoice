import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, RequestOptions } from '@angular/http';
import { InvoiceModel } from '../models/invoice.model';
import 'rxjs/Rx';
import { Headers, URLSearchParams } from '@angular/http';
import { forEach } from '@angular/router/src/utils/collection';

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
                let result = response.json();
                if (result.date)
                    result.date = new Date(result.date);
                return <InvoiceModel>result;
            });
    }

    deleteItem(id: number) {
        return this.http.delete(this.actionUrl + '/' + id);
    };

    save(invoice: InvoiceModel): Observable<InvoiceModel> {
        return this.http.put(this.actionUrl + 'Save', invoice)
            .map((response: Response) => {
                return <InvoiceModel>response.json();
            });
    }

    getById(invoice: InvoiceModel): Observable<InvoiceModel> {
        return this.http.post(this.actionUrl + 'GetById', invoice)
            .map((response: Response) => {
                let result = response.json();
                if (result.date)
                    result.date = new Date(result.date);
                return <InvoiceModel>result;
            });
    }
}