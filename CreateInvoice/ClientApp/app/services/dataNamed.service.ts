import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response } from '@angular/http';
import { NamedIdObject } from '../models/NamedIdObject.model';
import 'rxjs/Rx';

@Injectable()

export class DataNamedService {
    private actionUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.actionUrl = baseUrl + '/api/';
    }

    getAll(apiName:string): Observable<NamedIdObject[]> {
        return this.http.get(this.actionUrl + apiName + '/getAll')
            .map((response: Response) => {
                return <NamedIdObject[]>response.json();
        });
    }
}

