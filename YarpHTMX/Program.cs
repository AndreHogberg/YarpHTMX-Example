using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Zitadel.Authentication;
using Zitadel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", false);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(ZitadelDefaults.AuthenticationScheme)
    .AddZitadel(opt =>
    {
        opt.SignInScheme = IdentityConstants.ExternalScheme;
        
        opt.Authority = builder.Configuration["Zitadel:Authority"];
        opt.ClientId = builder.Configuration["Zitadel:ClientId"];
        
        opt.GetClaimsFromUserInfoEndpoint = true;
        
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = opt.Authority,
            ValidAudience = opt.ClientId
        };
    })
    .AddExternalCookie()
    .Configure(opt =>
    {
        opt.AccessDeniedPath = "/login";
        opt.LoginPath = "/login";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SameSite = SameSiteMode.None;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.UseHttpsRedirection();

app.MapGet("/login", async context =>
{
    await context.ChallengeAsync(ZitadelDefaults.AuthenticationScheme, new() { RedirectUri = "/app2" });
});

app.Run();
