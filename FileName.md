# ASP.NET Core Angular SSR Setup

This README explains how to enable server-side rendering (SSR) in an ASP.NET Core application that serves an Angular Single Page Application (SPA). The setup leverages various middleware components to enhance performance, security, and SEO.

---

## Overview

This solution integrates several key features:
- **HTTPS and HSTS**: Enforces secure connections.
- **Static File Serving**: Serves SPA assets from the Angular build.
- **WebMarkupMin**: Optimizes HTML output via minification and compression.
- **SPA Middleware**: Integrates Angular SSR with prerendering and Angular CLI server support (in development).
- **API Controllers & Routing**: Handles API requests alongside serving the Angular app.

---

## Key Files and Their Roles

### 1. `Program.cs` (Main Application Setup)

This file configures and builds the ASP.NET Core application. It registers services, configures middleware, and integrates the Angular SPA with SSR.

```csharp
using MintPlayer.AspNetCore.Hsts; // Enables HSTS for improved security
using MintPlayer.AspNetCore.SpaServices.Extensions; // SPA service extensions
using MintPlayer.AspNetCore.SpaServices.Prerendering; // Enables SPA prerendering
using MintPlayer.AspNetCore.SpaServices.Routing; // Provides server routing for SPA
using System.Text.RegularExpressions; // Supports regular expressions (used in SPA CLI regex)
using WebMarkupMin.AspNetCoreLatest; // Provides HTML minification and HTTP compression

var builder = WebApplication.CreateSlimBuilder(args); // Creates a lightweight ASP.NET Core builder

// Configure URLs and HTTPS settings
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
builder.WebHost.UseKestrelHttpsConfiguration(); // Configures Kestrel to support HTTPS

// Register services
builder.Services.AddControllers(); // Adds MVC controller support
builder.Services.AddSpaStaticFilesImproved(configuration => configuration.RootPath = "ClientApp/dist"); // Serves static SPA files
builder.Services.AddSpaPrerenderingService<demopractice.Services.SpaPrerenderingService>(); // Registers the SPA prerendering service

// Add and configure HTML minification and compression
builder.Services.AddWebMarkupMin()
    .AddHttpCompression() // Enables HTTP compression
    .AddHtmlMinification(); // Enables HTML minification

// Configure WebMarkupMin options for both development and production environments
builder.Services
    .Configure<WebMarkupMinOptions>(options =>
    {
        options.DisablePoweredByHttpHeaders = true; // Option to hide "powered by" headers
        options.AllowMinificationInDevelopmentEnvironment = true; // Enable minification in development
        options.AllowCompressionInDevelopmentEnvironment = true; // Enable compression in development
        options.DisablePoweredByHttpHeaders = false; // Here you can choose whether to show "powered by" header
    })
    .Configure<HtmlMinificationOptions>(options =>
    {
        options.MinificationSettings.RemoveEmptyAttributes = true; // Removes empty attributes from HTML
        options.MinificationSettings.RemoveRedundantAttributes = true; // Removes redundant attributes
        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true; // Removes "http://" from URLs
        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = false; // Keeps "https://" intact
        options.MinificationSettings.MinifyInlineJsCode = true; // Minifies inline JavaScript
        options.MinificationSettings.MinifyEmbeddedJsCode = true; // Minifies embedded JavaScript
        options.MinificationSettings.MinifyEmbeddedJsonData = true; // Minifies embedded JSON data
        options.MinificationSettings.WhitespaceMinificationMode = WebMarkupMin.Core.WhitespaceMinificationMode.Aggressive; // Aggressive whitespace removal
    });

var app = builder.Build(); // Builds the application

// Configure the middleware pipeline
app.UseImprovedHsts(); // Enforces HTTP Strict Transport Security (HSTS)
app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS
app.UseStaticFiles(); // Serves static files (CSS, JS, images, etc.)

if (!builder.Environment.IsDevelopment())
{
    app.UseSpaStaticFilesImproved(); // Serves SPA static files in non-development environments
}

app.UseWebMarkupMin(); // Applies HTML minification and compression middleware
app.UseRouting(); // Enables routing
app.UseAuthentication(); // Enables authentication middleware
app.UseAuthorization(); // Enables authorization middleware

// Set up API routes
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}"); // Default API routing pattern
});

// Configure SPA middleware; this should be the last middleware in the pipeline
app.UseSpaImproved(spa =>
{
    spa.Options.SourcePath = "ClientApp"; // Defines the source path for the Angular application
    spa.Options.CliRegexes = new Regex[]
    {
        new Regex(@"Local\:\s+(?<openbrowser>https?\:\/\/(.+))") // Regex to extract dev server URLs
    };

    // Configure SPA prerendering
    spa.UseSpaPrerendering(options =>
    {
        options.BootModuleBuilder = builder.Environment.IsDevelopment()
            ? new AngularPrerendererBuilder(npmScript: "build:ssr") // Uses Angular CLI SSR build in development
            : null;
        options.BootModulePath = $"{spa.Options.SourcePath}/dist/server/main.mjs"; // Path to the server-side entry module
        options.ExcludeUrls = new[] { "/sockjs-node" }; // Excludes specific URLs from prerendering
    });

    if (builder.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "build:ssr"); // Launches Angular CLI server for SSR in development
    }
});

app.Run(); // Runs the application
