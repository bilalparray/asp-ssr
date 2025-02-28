import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { provideServerRouting } from '@angular/ssr';
import { appConfig } from './app.config';
import { serverRoutes } from './app.routes.server';

/**
 * Defines the server-specific application configuration.
 *
 * - `provideServerRendering()`: Enables server-side rendering (SSR) for the application.
 * - `provideServerRouting(serverRoutes)`: Configures server-side routing with predefined server-specific routes.
 */
const serverConfig: ApplicationConfig = {
  providers: [provideServerRendering(), provideServerRouting(serverRoutes)],
};

/**
 * Merges the general application configuration (`appConfig`) with the server-specific configuration (`serverConfig`).
 * This ensures that both client and server settings are combined appropriately.
 */
export const config = mergeApplicationConfig(appConfig, serverConfig);
