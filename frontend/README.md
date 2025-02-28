# Angular SSR Setup with Server-Side Rendering (SSR)

This README explains how to enable server-side rendering (SSR) in your Angular application using the provided configuration and code files. The setup leverages Angular Universal/SSR techniques along with prerendering and custom server routing.

Below are the key files and their roles in enabling SSR:

---

## 1. `app.config.server.ts`

This file merges the base Angular application configuration with server-specific providers needed for SSR. It enables server rendering and custom server routing.

```typescript
import { mergeApplicationConfig, ApplicationConfig } from "@angular/core";
import { provideServerRendering } from "@angular/platform-server";
import { provideServerRouting } from "@angular/ssr";
import { appConfig } from "./app.config";
import { serverRoutes } from "./app.routes.server";

const serverConfig: ApplicationConfig = {
  providers: [provideServerRendering(), provideServerRouting(serverRoutes)],
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
```

## Explanation:

    provideServerRendering(): Activates server-side rendering.
    provideServerRouting(serverRoutes): Configures the application to use server-specific routes (defined in the next file).
    mergeApplicationConfig: Combines the base configuration with these server settings.

## app.routes.server.ts

This file defines the server routes for the application and specifies that all routes should use prerendering.

```typescript
import { RenderMode, ServerRoute } from "@angular/ssr";

export const serverRoutes: ServerRoute[] = [
  {
    path: "**", // Wildcard route that matches any path
    renderMode: RenderMode.Prerender, // Uses prerendering for generating static HTML at build time
  },
];
```

Explanation:

The wildcard route ('\*\*') ensures that all paths are matched.
RenderMode.Prerender tells Angular to prerender the HTML, which improves performance and SEO.

## main.server.ts

```typescript
This is the entry point for the server-side version of your Angular application. It bootstraps the application using the server configuration.
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { config } from './app/app.config.server';

const bootstrap = () => bootstrapApplication(AppComponent, config);

export default bootstrap;
```

## tsconfig.server.json

This TypeScript configuration file is used to compile your server-side code. It extends your base tsconfig.app.json while adding server-specific compiler options.

```json
{
  "extends": "./tsconfig.app.json",
  "compilerOptions": {
    "outDir": "dist/server", // Output directory for the compiled server bundle
    "module": "ES2022", // Uses ES2022 module system
    "target": "ES2022", // Compiles to ES2022 JavaScript
    "types": ["node"], // Includes Node.js type definitions
    "emitDecoratorMetadata": true, // Required for Angular decorators
    "experimentalDecorators": true
  },
  "files": [
    "src/main.server.ts" // Entry point for the server-side build
  ],
  "angularCompilerOptions": {
    "enableIvy": false // Disables Ivy for the server build (if not required)
  }
}
```

Explanation:

The configuration ensures that the server code is compiled with the appropriate ECMAScript settings.
It includes Node.js types, which are necessary for server-side execution.
The server entry point is specified to allow proper bundling.

## How to Enable SSR in Your Angular Application

Set Up Your Angular Application:
Ensure that your Angular project has a base configuration (e.g., app.config.ts) and that it works correctly on the client side.

## Add SSR-Specific Files:

Add the app.config.server.ts file to merge server-specific settings.
Add the app.routes.server.ts file to define server routes with prerendering.
Create main.server.ts as the bootstrap file for the server-side application.
Include the tsconfig.server.json file for compiling your server bundle.
Build the Server Bundle:
Use Angular CLI to compile the server-side code. For example:

```json
 "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "build": "ng build",
    "watch": "ng build --watch --configuration development",
    "test": "ng test",
    "dev:ssr": "ng run ClientApp:serve-ssr",
    "serve:ssr": "node dist/server/server.mjs",
    "build:ssr": "ng build && node dist/server/server.mjs",
    "build:ssr:prod": "ng build --configuration=production && ng run ClientApp:server:production"
  },

```

Integrate with Your Server:
If you are using a server (for example, an ASP.NET Core backend with MintPlayer SPA Services), configure it to use the generated server bundle. The ASP.NET Core middleware should point to the prerendered bundle for SSR.

Run Your Application:
Start your server. When you access your application, the server will prerender the HTML and serve it to clients, improving load times and SEO.
