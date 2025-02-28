/**
 * Import necessary modules for bootstrapping the Angular application on the server.
 */
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { config } from './app/app.config.server';

/**
 * Initializes and bootstraps the Angular application using server-side configuration.
 *
 * - `bootstrapApplication(AppComponent, config)`:
 *   - Bootstraps the root `AppComponent` with the provided server configuration.
 *   - Ensures proper setup for server-side rendering (SSR).
 */
const bootstrap = () => bootstrapApplication(AppComponent, config);

export default bootstrap;
