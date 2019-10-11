import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { City } from 'src/app/_models/City';
import { CityService } from 'src/app/_services/city/city.service';
import { Subject, of, concat } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError } from 'rxjs/operators';
import { Language } from 'src/app/_models/language';
import { LanguageService } from 'src/app/_services/language/language.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;

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
    private languageService: LanguageService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });

    this.loadCities();
    this.loadSpeakingLanguages();
    this.loadLearningLanguages();
  }

  addSpeakingLanguage() {
    const current = this.user.languagesTheUserSpeaks.find(l => l.code === this.selectedSpeakingLanguage.code);
    if (current) {
      current.levelOfSpeaking = this.selectedSpeakingLanguage.levelOfSpeaking;
      return;
    }
    this.user.languagesTheUserSpeaks.push(this.selectedSpeakingLanguage);
  }

  addLearningLanguage() {
    const current = this.user.languagesTheUserLearns.find(l => l.code === this.selectedLearningLanguage.code);
    if (current) {
      current.levelOfSpeaking = this.selectedLearningLanguage.levelOfSpeaking;
      return;
    }
    this.user.languagesTheUserLearns.push(this.selectedLearningLanguage);
  }

  removeSpeaking(language: Language) {
    const index = this.user.languagesTheUserSpeaks.indexOf(language, 0);
    if (index > -1) {
      this.user.languagesTheUserSpeaks.splice(index, 1);
    }
  }

  removeLearning(language: Language) {
    const index = this.user.languagesTheUserLearns.indexOf(language, 0);
    if (index > -1) {
      this.user.languagesTheUserLearns.splice(index, 1);
    }
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

  private loadCities() {
    this.cities$ = concat(
        of([]), // default items
        this.citiesInput$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            tap(() => this.citiesLoading = true),
            switchMap(term => this.cityService.getCities(term != null ? term : '').pipe(
                catchError(() => of([])), // empty list on error
                tap(() => this.citiesLoading = false)
            ))
        )
    );
  }

  private loadSpeakingLanguages() {
    this.speakingLanguages$ = concat(
        of([]), // default items
        this.speakingLanguagesInput$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            tap(() => this.speakingLanguagesLoading = true),
            switchMap(term => this.languageService.getCities(term != null ? term : '').pipe(
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
            switchMap(term => this.languageService.getCities(term != null ? term : '').pipe(
                catchError(() => of([])), // empty list on error
                tap(() => this.learningLanguagesLoading = false)
            ))
        )
    );
  }
}
