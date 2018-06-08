import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response, ResponseContentType } from '@angular/http';
import { CerticateModel } from '../models/certificate.model';
import 'rxjs/Rx';

@Injectable()

export class CertificateService {
    private actionUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.actionUrl = baseUrl + '/api/Certificate/';
    }

    getAll(): Observable<CerticateModel[]> {
        return this.http.get(this.actionUrl + '/getAll')
            .map((response: Response) => {
                let result = <CerticateModel[]>response.json();
                result.forEach(function (item) {
                    item.endDate = new Date(item.endDate);
                    item.startDate = new Date(item.startDate);
                });
                return result;
            });
    }

    deleteItem(id: number) {
        return this.http.delete(this.actionUrl + '/' + id);
    };

    save(certificate: CerticateModel, isNew: boolean): Observable<CerticateModel> {
        if (isNew) {
            return this.http.put(this.actionUrl + 'Add', certificate)
                .map((response: Response) => {
                    return <CerticateModel>response.json();
                });
        } else {
            return this.http.post(this.actionUrl + 'Save', certificate)
                .map((response: Response) => {
                    return <CerticateModel>response.json();
                });
        }
    };

    getImportTemplate() {
        return this.http.get(this.actionUrl + 'DownloadTemplate', {
            responseType: ResponseContentType.Blob
        }).map(result => {
            return {
                filename: 'Certificate template.xlsx',
                data: result.blob()
            }
        });
    }
}