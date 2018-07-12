import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, RequestOptions, ResponseContentType } from '@angular/http';
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

    print(invoice: InvoiceModel) {
        let request_data = new URLSearchParams();
        request_data.append('id', invoice.id.toString());
        let request_option = new RequestOptions();
        request_option.search = request_data;
        request_option.responseType = ResponseContentType.Blob;

        return this.http.get(this.actionUrl + 'Download', request_option)
            .map(result => {
                return {
                    filename: 'Invoice.xlsx',
                    data: result.blob()
                }
            });
    }
}