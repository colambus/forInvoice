import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, RequestOptions, ResponseContentType } from '@angular/http';
import { ProductModel } from '../models/product.model';
import 'rxjs/Rx';
import { Headers, URLSearchParams } from '@angular/http';
import { forEach } from '@angular/router/src/utils/collection';

@Injectable()
export class ProductService {
    private actionUrl: string;
    
    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.actionUrl = baseUrl + '/api/Product/';
    }

    getBySeq(seq: string): Observable<ProductModel[]> {
 
        let request_data = new URLSearchParams();
        request_data.append('queryStr', seq);
        let request_option = new RequestOptions();
        request_option.search = request_data;
        return this.http.get(this.actionUrl + 'GetBySeq',
            request_option)
            .map((response: Response) => {
                let result = <ProductModel[]>response.json();
                result.forEach(function (item) {
                    item.descriptionEn = item.id + " " + item.descriptionEn;
                });    
                return result;
            });
    }

    getAll(skip?: number, take?: number): Observable<ProductModel[]> {
        let request_data = new URLSearchParams();
        if (skip != undefined)
            request_data.append('skip', skip.toString()); 
        if (take != undefined)
            request_data.append('take', take.toString());
        let request_option = new RequestOptions();
        request_option.search = request_data;
        return this.http.get(this.actionUrl + 'GetAll', request_option)
            .map((response: Response) => {
                return <ProductModel[]>response.json();
            });
    }

    save(product: ProductModel, isNew: boolean): Observable<ProductModel> {
        if (isNew) {
            return this.http.put(this.actionUrl + 'Add', product)
                .map((response: Response) => {
                    return <ProductModel>response.json();
                });
        } else {
            return this.http.post(this.actionUrl + 'Save', product)
                .map((response: Response) => {
                    return <ProductModel>response.json();
                });
        }   
    }

    getProductsCount() {
        return this.http.get(this.actionUrl + "GetProductsCount")
            .map(result => {
                return Number(result.text());
            });
    }

    deleteItem(id: number) {
        return this.http.delete(this.actionUrl + '/' + id);
    };

    getImportTemplate() {
        return this.http.get(this.actionUrl + 'DownloadTemplate', {
            responseType: ResponseContentType.Blob
        }).map(result => {
            return {
                filename: 'Products template.xlsx',
                data: result.blob()
            }
        });
    }
}