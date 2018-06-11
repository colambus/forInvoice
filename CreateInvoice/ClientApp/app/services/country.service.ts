import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, ResponseContentType } from '@angular/http';
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

    save(countryOfOrigin: CountryOfOriginModel, isNew: boolean): Observable<CountryOfOriginModel> {
        if (isNew) {
            return this.http.put(this.actionUrl + '/Add', countryOfOrigin)
                .map((response: Response) => {
                    return <CountryOfOriginModel>response.json();
                });
        } else {
            return this.http.post(this.actionUrl + '/Save', countryOfOrigin)
                .map((response: Response) => {
                    return <CountryOfOriginModel>response.json();
                });
        }
    }

    getImportTemplate() {
        return this.http.get(this.actionUrl + 'DownloadTemplate', {
            responseType: ResponseContentType.Blob
        }).map(result => {
            return {
                filename: 'Country template.xlsx',
                data: result.blob()
            }
        });
    }
}