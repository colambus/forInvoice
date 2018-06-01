import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, RequestOptions } from '@angular/http';
import { InvoiceProductModel } from '../models/invoiceProduct.model';
import 'rxjs/Rx';
import { Headers, URLSearchParams } from '@angular/http';
import { stringify } from '@angular/core/src/util';

@Injectable()
export class InvoiceProductService {
    private actionUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.actionUrl = baseUrl + '/api/InvoiceProduct/';
    }

    getAll(invoiceId: number): Observable<InvoiceProductModel[]> {
        let request_data = new URLSearchParams();
        request_data.append('id', String(invoiceId));
        let request_option = new RequestOptions();
        request_option.search = request_data;
        return this.http.get(this.actionUrl + 'GetAll', request_option)
            .map((response: Response) => {
                return <InvoiceProductModel[]>response.json();
            });
    }

    add(product: InvoiceProductModel): Observable<InvoiceProductModel> {
        return this.http.put(this.actionUrl + 'Add', product)
            .map((response: Response) => {
                return <InvoiceProductModel>response.json();
            });
    }

    save(product: InvoiceProductModel, isNew: boolean): Observable<InvoiceProductModel> {
        if (isNew) {
            return this.http.put(this.actionUrl + 'Add', product)
                .map((response: Response) => {
                    return <InvoiceProductModel>response.json();
                });
        } else {
            return this.http.post(this.actionUrl + 'Save', product)
                .map((response: Response) => {
                    return <InvoiceProductModel>response.json();
                });
        }   
    }

    remove(id: number) {
        return this.http.delete(this.actionUrl + '/' + id);
    }
}