import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { UserService } from 'src/app/_services/user/user.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { UserFilterParams } from 'src/app/_models/userFilterParams';
import { Language } from 'src/app/_models/language';
import { Observable, Subject, concat, of, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError, map } from 'rxjs/operators';
import { LanguageService } from 'src/app/_services/language/language.service';
import { City } from 'src/app/_models/City';
import { CityService } from 'src/app/_services/city/city.service';
import { Country } from 'src/app/_models/country';
import { CountryService } from 'src/app/_services/country/country.service';
import { LocalStorageService } from 'src/app/_services/local-storage/local-storage.service';
import { FriendshipStatus } from 'src/app/_models/enums/friendshipStatus';
import { SubscriptionState as SubscriptionState } from 'src/app/_models/enums/subscriptionState';
import { SignalrFriendService } from 'src/app/_services/signalr/signalr-friend/signalr-friend.service';
import { FriendshipRequestStatus } from 'src/app/_models/friendship-request-status';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit, OnDestroy {
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

  friendshipRequestSub: Subscription;
  friendshipStatusSub: Subscription;

  constructor(private route: ActivatedRoute, private userService: UserService,
    private alertifyService: AlertifyService, private languageService: LanguageService,
    private countryService: CountryService, private cityService: CityService,
    private localStorageService: LocalStorageService, private signalrFriendService: SignalrFriendService) { }

  ngOnInit() {
    this.route.url.subscribe(params => {
      this.currentPath = params[0].path;

      if (this.currentPath === SubscriptionState.friends && !params[1]) {
        this.userParams.subscriptionState = SubscriptionState.friends;
      } else if (params[1]) {
        this.userParams.subscriptionState = SubscriptionState[params[1].path];
      }
      this.subscribeOnFrinedshipChanges();
    });

    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.resetFilters();
    this.loadCountries();
    this.loadCities();
  }

  ngOnDestroy() {
    if (this.friendshipStatusSub) {
      this.signalrFriendService.unsubscribeFromFriendshipStatusReceived(this.friendshipStatusSub);
    }
    if (this.friendshipRequestSub) {
      this.signalrFriendService.unsubscribeFromFriendshipRequestReceived(this.friendshipRequestSub);
    }
  }

  private subscribeOnFrinedshipChanges() {
    // We always have to be notified about the changes, despite at which view state we are
    if (!this.friendshipStatusSub) {
      this.friendshipStatusSub = this.signalrFriendService.subscribeOnFriendshipStatusReceived({
        next: (requestStatus: FriendshipRequestStatus) => {
          const ourUserId = this.user.id;
          const theirUserId =
              requestStatus.fromUserId === ourUserId
              ? requestStatus.toUserId
              : requestStatus.fromUserId;
          const friendshipToChange = this.users.find(u => u.id === theirUserId);
          // if there's nothing to change in the list - leave
          if (!friendshipToChange) {
            return;
          }
          // if we're in the general member view - change the status of existing friendship request
          // if we're anywhere else (friends / subscribtions / followers) - remove the changed one or add the new one
          if (this.userParams.subscriptionState == null) {
            if (requestStatus.isDeleted) {
              friendshipToChange.friendship = null;
            } else {
              friendshipToChange.friendship = {
                isOwner: requestStatus.fromUserId === theirUserId,
                status: requestStatus.status
              };
            }
          } else {
            const index = this.users.indexOf(friendshipToChange, 0);
            if (index > -1) {
              this.users.splice(index, 1);
            }
          }
        },
        error: null,
        complete: null
      });
    }

    if (this.userParams.subscriptionState === SubscriptionState.subscriptions) {
      // ToDo: Subscribe to new friendship requests...
    }
    // ToDo: Unsubscribe from SignalR events if there are subscriptions
    // ToDo: Subscribe to events...
  }

  public pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  public loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
    .subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertifyService.error(error);
    });
  }

  public resetFilters(applyFilters?: boolean) {
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

  public excludedFromList(friend: User) {
    if (this.currentPath === this.friendPath) {
      const index = this.users.indexOf(friend, 0);
      if (index > -1) {
        this.users.splice(index, 1);
      }
    }
  }

  public addedToFriendlist(friend: User) {
    if (this.userParams.subscriptionState === SubscriptionState.followers) {
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

  public locationChanged(location: string) {
    if (location === 'city') {
      this.userParams.countryIsoCode = null;
    } else {
      this.userParams.cityId = null;
    }
  }
}
