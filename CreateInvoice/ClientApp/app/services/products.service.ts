﻿import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, RequestOptions } from '@angular/http';
import { ProductModel } from '../models/product.model';
import 'rxjs/Rx';
import { Headers, URLSearchParams } from '@angular/http';

@Injectable()
export class ProductService {
    private actionUrl: string;
    
    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.actionUrl = baseUrl + '/api/Product/';
    }

    getBySeq(seq: string): Observable<ProductModel[]> {

        let headers = new Headers({
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        });

        let request_data = new URLSearchParams();
        request_data.set('queryStr', seq);
        let request_option = new RequestOptions({ headers: headers });
        request_option.params = request_data;
        return this.http.get(this.actionUrl + 'GetBySeq',
            { params: request_option })
            .map((response: Response) => {
                return <ProductModel[]>response.json();
            });
    }
}