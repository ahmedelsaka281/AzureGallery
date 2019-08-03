import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpRequest } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class AzureService {

  private azureUrl = environment.baseUrl + '/gallery/';

  constructor(private http: HttpClient) { }

  getAllFiles(): Observable<string[]> {
    return this.http.get<string[]>(this.azureUrl + 'getAllFiles');
  }

  deleteFile(name: string): Observable<any> {
    return this.http.delete(this.azureUrl + 'deleteFile/' + name + '', { responseType: 'text' });
  }

  downloadFile(name: string): Observable<any> {
    return this.http.post(this.azureUrl + 'downloadFile/' + name + '', null, { responseType: 'text' });
  }

  uploadFile(formData: FormData): Observable<any> {
    const uploadReq = new HttpRequest('POST', `${this.azureUrl}UploadFile`, formData, { reportProgress: true, responseType: 'text' });
    return this.http.request(uploadReq);
  }

  deleteAllFiles(): Observable<any> {
    return this.http.delete(this.azureUrl + 'deleteAllFiles', { responseType: 'text' });
  }
}
