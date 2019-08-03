import { Observable } from 'rxjs';
import { AuthService } from './../_services/auth.service';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';


@Injectable()
export class TokenInterceptor implements HttpInterceptor {
    constructor(public auth: AuthService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${this.auth.getToken()}`
            }
        });
        return next.handle(request);
    }
}
