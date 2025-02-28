/**
 * Import necessary types for configuring server-side routing.
 */
import { RenderMode, ServerRoute } from '@angular/ssr';

/**
 * Defines server-side routes for Angular SSR.
 *
 * - `path: '**'` → Matches all routes (wildcard route).
 * - `renderMode: RenderMode.Prerender` → Uses prerendering to generate static HTML during build time.
 */
export const serverRoutes: ServerRoute[] = [
  {
    path: '**', // Wildcard route to match any path
    renderMode: RenderMode.Prerender, // Enables prerendering for improved performance and SEO
  },
];
