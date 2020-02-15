import * as signalR from '@aspnet/signalr';
import { Subject, Observer, Subscription } from 'rxjs';
import { isDevMode } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AppInjector } from '../../_helpers/app-injector';
import { LocalStorageService } from '../local-storage/local-storage.service';

export abstract class SignalrBaseService {
    private baseUrl = environment.baseServerUrl;

    protected isConnectionInProgress = false;

    protected hubConnection: signalR.HubConnection;
    protected abstract subjects: Subject<any>[];

    constructor(url: string) {
        const localStorageService = AppInjector.get(LocalStorageService);
        const logLevel = isDevMode() ? signalR.LogLevel.Debug : signalR.LogLevel.None;
        this.hubConnection = new signalR.HubConnectionBuilder()
            .configureLogging(logLevel)
            .withUrl(this.baseUrl + url, { accessTokenFactory: () => localStorageService.getToken() })
            .build();
    }

    //#region Generic methods fur subscription / unsubscription
    protected checkStateAndConnect() {
        // check the connections state. If it's disconnected - start the connection
        if (this.hubConnection.state === signalR.HubConnectionState.Disconnected && !this.isConnectionInProgress) {
        this.isConnectionInProgress = true;
        this.hubConnection.start()
            .then(() => { this.isConnectionInProgress = false; })
            .catch(err => { this.isConnectionInProgress = false; });
        }
    }

    protected checkStateAndDisonnect() {
        // check the connection state. If it's connected but there's no one
        // who'd listen for the events - disconnect the hub
        if (this.hubConnection.state === signalR.HubConnectionState.Disconnected
            || this.subjects.some(s => s.observers.length > 0)) {
            return;
        }
        this.hubConnection.stop();
    }

    protected subscribeOnEvent<TSubject>(observer: Observer<TSubject>, nameOfHubMethod: string,
        subject: Subject<TSubject>): Subscription {
        // checking that the hub is connected and if it's not - connecting
        this.checkStateAndConnect();

        if (subject.observers.length === 0) {
            this.hubConnection.on(nameOfHubMethod, (object: TSubject) => {
                subject.next(object);
            });
        }
        return subject.subscribe(observer);
    }

    protected unsubscribeFromEvent<TSubject>(subscriprion: Subscription, nameOfHubMethod: string,
        subject: Subject<TSubject>) {
        if (!subscriprion) {
            return;
        }

        subscriprion.unsubscribe();
        if (subject.observers.length === 0) {
            this.hubConnection.off(nameOfHubMethod);
        }
        // checking if there's no observers left and if there isn't - close the hub connection
        this.checkStateAndDisonnect();
    }
    //#endregion Generic methods fur subscription / unsubscription
}
