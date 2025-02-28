

using MintPlayer.AspNetCore.Hsts; 
using MintPlayer.AspNetCore.SpaServices.Extensions; // SPA service extensions  
using MintPlayer.AspNetCore.SpaServices.Prerendering; // SPA prerendering support  
using MintPlayer.AspNetCore.SpaServices.Routing; // SPA routing services  
using System.Text.RegularExpressions;  
using WebMarkupMin.AspNetCoreLatest;   

var builder = WebApplication.CreateSlimBuilder(args); // Creates a lightweight web app builder  

// Configure URLs and HTTPS settings  
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
builder.WebHost.UseKestrelHttpsConfiguration(); // Configures Kestrel for HTTPS  

// Register services  
builder.Services.AddControllers(); // Adds MVC controllers  
builder.Services.AddSpaStaticFilesImproved(config => config.RootPath = "ClientApp/dist"); // Serves SPA static files  
builder.Services.AddSpaPrerenderingService<demopractice.Services.SpaPrerenderingService>(); // Adds SPA prerendering  

// Configure HTML minification and compression  
builder.Services.AddWebMarkupMin()
    .AddHttpCompression() // Enables HTTP compression  
    .AddHtmlMinification(); // Enables HTML minification  

// WebMarkupMin settings  
builder.Services
    .Configure<WebMarkupMinOptions>(options =>
    {
        options.DisablePoweredByHttpHeaders = true; // Hides "powered by" headers  
        options.AllowMinificationInDevelopmentEnvironment = true; // Enables minification in dev  
        options.AllowCompressionInDevelopmentEnvironment = true; // Enables compression in dev  
        options.DisablePoweredByHttpHeaders = false; // Allows "powered by" headers  
    })
    .Configure<HtmlMinificationOptions>(options =>
    {
        options.MinificationSettings.RemoveEmptyAttributes = true; // Removes empty attributes  
        options.MinificationSettings.RemoveRedundantAttributes = true; // Removes redundant attributes  
        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true; // Removes "http://" from URLs  
        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = false; // Keeps "https://"  
        options.MinificationSettings.MinifyInlineJsCode = true; // Minifies inline JS  
        options.MinificationSettings.MinifyEmbeddedJsCode = true; // Minifies embedded JS  
        options.MinificationSettings.MinifyEmbeddedJsonData = true; // Minifies JSON data  
        options.MinificationSettings.WhitespaceMinificationMode = WebMarkupMin.Core.WhitespaceMinificationMode.Aggressive; // Aggressive whitespace removal  
    });

var app = builder.Build(); // Builds the app  

// Configure middleware  
app.UseImprovedHsts(); // Enforces HTTP Strict Transport Security  
app.UseHttpsRedirection(); // Redirects HTTP to HTTPS  
app.UseStaticFiles(); // Serves static files  

if (!builder.Environment.IsDevelopment())
{
    app.UseSpaStaticFilesImproved(); // Serves SPA static files in production  
}

app.UseWebMarkupMin(); // Applies HTML minification and compression  
app.UseRouting(); // Enables request routing  
app.UseAuthentication(); // Enables authentication  
app.UseAuthorization(); // Enables authorization  

// Define API routes  
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}"); // Default route pattern  
});

// Configure SPA hosting and prerendering  
app.UseSpaImproved(spa =>
{
    spa.Options.SourcePath = "ClientApp"; // Sets the source directory for the SPA  
    spa.Options.CliRegexes = new Regex[]
    {
        new Regex(@"Local\:\s+(?<openbrowser>https?\:\/\/(.+))") // Extracts local dev URLs  
    };

    spa.UseSpaPrerendering(options =>
    {
        options.BootModuleBuilder = builder.Environment.IsDevelopment()
            ? new AngularPrerendererBuilder(npmScript: "build:ssr") // Uses Angular SSR in dev  
            : null;
        options.BootModulePath = $"{spa.Options.SourcePath}/dist/server/main.mjs"; // Path to SSR entry point  
        options.ExcludeUrls = new[] { "/sockjs-node" }; // Excludes WebSocket-related URLs  
    });

    if (builder.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "build:ssr"); // Uses Angular CLI server in dev  
    }
});

app.Run(); // Starts the application  

