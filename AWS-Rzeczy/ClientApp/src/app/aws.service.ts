import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { S3RquestBody } from './home/S3RquestBody';

@Injectable({
    providedIn: 'root'
})
export class AWSService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    public CreateBucket(body: S3RquestBody): Observable<any> {
        return this.http.post<string>(this.baseUrl + 'api/s3/create', body);
    }

    public DeleteFromBucket(body: S3RquestBody): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/s3/delete', body);
    }
    public GetListBucket(body: S3RquestBody): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/s3/getlist', body);
    }

    public GetFromBucket(body: S3RquestBody): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/s3/get', body);
    }

    public UploadToBucket(body: S3RquestBody): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'api/s3/upload', body);
    }

}
