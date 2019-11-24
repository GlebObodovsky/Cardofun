import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { UserService } from 'src/app/_services/user/user.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { UserFilterParams } from 'src/app/_models/userFilterParams';
import { Language } from 'src/app/_models/language';
import { Observable, Subject, concat, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError, map } from 'rxjs/operators';
import { LanguageService } from 'src/app/_services/language/language.service';
import { City } from 'src/app/_models/City';
import { CityService } from 'src/app/_services/city/city.service';
import { Country } from 'src/app/_models/country';
import { CountryService } from 'src/app/_services/country/country.service';
import { LocalStorageService } from 'src/app/_services/local-storage/local-storage.service';
import { FriendService } from 'src/app/_services/friend/friend.service';
import { FriendshipStatus } from 'src/app/_models/enums/friendshipStatus';
import { SupscriptionState } from 'src/app/_models/enums/supscriptionState';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  currentPath: string;
  friendPath = 'friends';

  users: User[];
  user: User = this.localStorageService.getUser();
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];
  userParams: UserFilterParams = {};
  pagination: Pagination;

  countries$: Observable<Country[]>;
  countriesInput$ = new Subject<string>();
  countriesLoading = false;

  cities$: Observable<City[]>;
  citiesInput$ = new Subject<string>();
  citiesLoading = false;

  speakingLanguages$: Observable<Language[]>;
  speakingLanguagesInput$ = new Subject<string>();
  speakingLanguagesLoading = false;

  friendshipStatuses = FriendshipStatus;

  constructor(private route: ActivatedRoute, private userService: UserService,
    private friendService: FriendService, private alertifyService: AlertifyService,
    private languageService: LanguageService, private countryService: CountryService,
    private cityService: CityService, private localStorageService: LocalStorageService) { }

  ngOnInit() {
    this.route.url.subscribe(params => {
      this.currentPath = params[0].path;
      if (params[1]) {
        this.userParams.subscriptionState = SupscriptionState[params[1].path];
      }
    });

    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.resetFilters();
    this.loadCountries();
    this.loadCities();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
    .subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertifyService.error(error);
    });
  }

  resetFilters(applyFilters?: boolean) {
    this.userParams.sex = null;
    this.userParams.ageMin = null;
    this.userParams.ageMax = null;
    this.userParams.cityId = null;
    this.userParams.countryIsoCode = null;
    this.userParams.languageLearningCode = null;
    this.userParams.languageSpeakingCode = this.user.languagesTheUserLearns.length === 1
      ? this.user.languagesTheUserLearns[0].code
      : null;

    this.loadSpeakingLanguages();

    if (applyFilters) {
      this.loadUsers();
    }
  }

  excludedFromList(friend: User) {
    if (this.currentPath === this.friendPath) {
      const index = this.users.indexOf(friend, 0);
      if (index > -1) {
        this.users.splice(index, 1);
      }
    }
  }

  addedToFriendlist(friend: User) {
    if (this.userParams.subscriptionState === SupscriptionState.followers) {
      this.excludedFromList(friend);
    }
  }

  private loadSpeakingLanguages() {
    this.speakingLanguages$ = concat(
        of(this.user.languagesTheUserLearns), // default items
        this.speakingLanguagesInput$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            tap(() => this.speakingLanguagesLoading = true),
            switchMap(term => this.getLanguages(term).pipe(
                catchError(() => of([])), // empty list on error
                tap(() => this.speakingLanguagesLoading = false)
            ))
        )
    );
  }

  private getLanguages(term: string): Observable<Language[]> {
    if (term == null) {
      return of([]);
    }
    return this.languageService.getLanguages(term);
  }

  private loadCountries() {
    this.countries$ = concat(
        of([]), // default items
        this.countriesInput$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            tap(() => this.countriesLoading = true),
            switchMap(term => this.getCountries(term).pipe(
                catchError(() => of([])), // empty list on error
                tap(() => this.countriesLoading = false)
            ))
        )
    );
  }

  private getCountries(term: string): Observable<Country[]> {
    if (term == null) {
      return of([]);
    }
    return this.countryService.getCountries(term);
  }

  private loadCities() {
    this.cities$ = concat(
        of([]), // default items
        this.citiesInput$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            tap(() => this.citiesLoading = true),
            switchMap(term => this.getCities(term).pipe(
                catchError(() => of([])), // empty list on error
                tap(() => this.citiesLoading = false)
            ))
        )
    );
  }

  private getCities(term: string): Observable<City[]> {
    if (term == null) {
      return of([]);
    }
    return this.cityService.getCities(term);
  }

  private locationChanged(location: string) {
    if (location === 'city') {
      this.userParams.countryIsoCode = null;
    } else {
      this.userParams.cityId = null;
    }
  }
}
