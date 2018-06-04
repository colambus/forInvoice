import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response } from '@angular/http';
import { CountryOfOriginModel } from '../models/countryOfOrigin.model';
import 'rxjs/Rx';

@Injectable()

export class CountryService {
    private actionUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.actionUrl = baseUrl + '/api/Country';
    }

    getAll(): Observable<CountryOfOriginModel[]> {
        return this.http.get(this.actionUrl + '/getAll')
            .map((response: Response) => {
                return <CountryOfOriginModel[]>response.json();
            });
    }

    deleteItem(id: number) {
        return this.http.delete(this.actionUrl + '/' + id);
    };

    save(certificate: CountryOfOriginModel, isNew: boolean): Observable<CountryOfOriginModel> {
        if (isNew) {
            return this.http.put(this.actionUrl + 'Add', certificate)
                .map((response: Response) => {
                    return <CountryOfOriginModel>response.json();
                });
        } else {
            return this.http.post(this.actionUrl + 'Save', certificate)
                .map((response: Response) => {
                    return <CountryOfOriginModel>response.json();
                });
        }
    }


}