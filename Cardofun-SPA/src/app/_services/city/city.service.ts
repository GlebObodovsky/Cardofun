import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { City } from 'src/app/_models/City';

@Injectable({
  providedIn: 'root'
})
export class CityService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCities(searchPattern: string): Observable<City[]> {
    return this.http.get<City[]>(this.baseUrl + 'cities/' + searchPattern);
  }

}
