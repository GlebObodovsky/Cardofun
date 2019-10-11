import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Language } from 'src/app/_models/language';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCities(searchPattern: string): Observable<Language[]> {
    return this.http.get<Language[]>(this.baseUrl + 'languages/' + searchPattern);
  }
}
