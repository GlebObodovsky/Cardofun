import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { City } from 'src/app/_models/City';
import { CityService } from 'src/app/_services/city/city.service';
import { Subject, of, concat } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;

  // citiesLoading = false;
  // selectedCity: City;
  // cities$: Observable<City[]>;
  // citiesInput$ = new Subject<string>();

  constructor(private route: ActivatedRoute, private cityService: CityService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });

    // this.loadCities();
  }

  // private loadCities() {
  //   this.cities$ = concat(
  //       of([]), // default items
  //       this.citiesInput$.pipe(
  //           debounceTime(200),
  //           distinctUntilChanged(),
  //           tap(() => this.citiesLoading = true),
  //           switchMap(term => this.cityService.getCities(term).pipe(
  //               catchError(() => of([])), // empty list on error
  //               tap(() => this.citiesLoading = false)
  //           ))
  //       )
  //   );
}
