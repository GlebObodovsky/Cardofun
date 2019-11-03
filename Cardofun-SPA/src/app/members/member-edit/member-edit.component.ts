import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { City } from 'src/app/_models/City';
import { CityService } from 'src/app/_services/city/city.service';
import { Subject, of, concat } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError } from 'rxjs/operators';
import { Language } from 'src/app/_models/language';
import { LanguageService } from 'src/app/_services/language/language.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user/user.service';
import { AuthService } from 'src/app/_services/auth/auth.service';
import { BsDatepickerConfig } from 'ngx-bootstrap';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;
  bsConfig: Partial<BsDatepickerConfig>;

  levelsOfSpeaking: any[] = [
    'beginner',
    'intermediate',
    'conversational',
    'fluent',
    'native'
  ];

  citiesLoading = false;
  cities$: Observable<City[]>;
  citiesInput$ = new Subject<string>();

  speakingLanguagesLoading = false;
  selectedSpeakingLanguage: Language;
  speakingLanguages$: Observable<Language[]>;
  speakingLanguagesInput$ = new Subject<string>();

  learningLanguagesLoading = false;
  selectedLearningLanguage: Language;
  learningLanguages$: Observable<Language[]>;
  learningLanguagesInput$ = new Subject<string>();

  constructor(private route: ActivatedRoute, private cityService: CityService,
    private languageService: LanguageService, private alertifyService: AlertifyService,
    private userService: UserService, private authService: AuthService) { }
  @ViewChild('editForm', {static: true}) editForm: NgForm;
  @HostListener('window:beforeunload', ['$event'])

  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-default',
      dateInputFormat: 'YYYY-MM-DD'
    };

    this.route.data.subscribe(data => {
      this.user = data['user'];
    });

    this.authService.currentPhotoUrl
      .subscribe(photoUrl => {
        this.user.photoUrl = photoUrl;
      });

    this.loadCities();
    this.loadSpeakingLanguages();
    this.loadLearningLanguages();
  }

  updateUser() {
    this.userService.putUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertifyService.success('Profile updated successfully');
      this.editForm.reset(this.user);
      this.authService.currentUser = this.user;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, error => {
      this.alertifyService.error(error);
    });
  }

  addSpeakingLanguage() {
    const current = this.user.languagesTheUserSpeaks.find(l => l.code === this.selectedSpeakingLanguage.code);
    if (current) {
      current.levelOfSpeaking = this.selectedSpeakingLanguage.levelOfSpeaking;
      return;
    }
    this.user.languagesTheUserSpeaks.push(this.selectedSpeakingLanguage);
    this.editForm.form.markAsDirty();
  }

  addLearningLanguage() {
    const current = this.user.languagesTheUserLearns.find(l => l.code === this.selectedLearningLanguage.code);
    if (current) {
      current.levelOfSpeaking = this.selectedLearningLanguage.levelOfSpeaking;
      return;
    }
    this.user.languagesTheUserLearns.push(this.selectedLearningLanguage);
    this.editForm.form.markAsDirty();
  }

  removeSpeaking(language: Language) {
    const index = this.user.languagesTheUserSpeaks.indexOf(language, 0);
    if (index > -1) {
      this.user.languagesTheUserSpeaks.splice(index, 1);
    }
    this.editForm.form.markAsDirty();
  }

  removeLearning(language: Language) {
    const index = this.user.languagesTheUserLearns.indexOf(language, 0);
    if (index > -1) {
      this.user.languagesTheUserLearns.splice(index, 1);
    }
    this.editForm.form.markAsDirty();
  }

  isSpeakingSelected() {
    return this.selectedSpeakingLanguage != null;
  }

  isSpeakingLevelSelected() {
    return this.isSpeakingSelected() && this.selectedSpeakingLanguage.levelOfSpeaking != null;
  }

  isLearningSelected() {
    return this.selectedLearningLanguage != null;
  }

  isLearningLevelSelected() {
    return this.isLearningSelected() && this.selectedLearningLanguage.levelOfSpeaking != null;
  }

  modelChanged() {
    this.editForm.form.markAsDirty();
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

  private loadSpeakingLanguages() {
    this.speakingLanguages$ = concat(
        of([]), // default items
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

  private loadLearningLanguages() {
    this.learningLanguages$ = concat(
        of([]), // default items
        this.learningLanguagesInput$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            tap(() => this.learningLanguagesLoading = true),
            switchMap(term => this.getLanguages(term).pipe(
                catchError(() => of([])), // empty list on error
                tap(() => this.learningLanguagesLoading = false)
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
}
