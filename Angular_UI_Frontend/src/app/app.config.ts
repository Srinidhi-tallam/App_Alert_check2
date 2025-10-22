import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { routes } from './app.routes'; // ✅ fixed path (remove './app/')

export const appConfig: ApplicationConfig = {
  providers: [
    // Modern way to provide HTTP client (replaces deprecated HttpClientModule)
    provideHttpClient(withInterceptorsFromDi()),

    // Optional: if you use FormsModule globally
    // importProvidersFrom(FormsModule),

    provideRouter(routes)
  ]
};
