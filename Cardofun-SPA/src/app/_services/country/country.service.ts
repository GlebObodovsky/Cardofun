import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Country } from 'src/app/_models/country';

@Injectable({
  providedIn: 'root'
})
export class CountryService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCountries(searchPattern: string): Observable<Country[]> {
    return this.http.get<Country[]>(this.baseUrl + 'countries/' + searchPattern);
  }
}
