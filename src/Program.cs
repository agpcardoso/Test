using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using SantanderTest.Clients;
using SantanderTest.Services;
using Microsoft.Extensions.DependencyInjection;
using SantanderTest.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GeneralSettings>(
    builder.Configuration.GetSection("GeneralSettings"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Redis
builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.Configuration = builder.Configuration["REDIS_CONNECTION_STRING"];
    opts.InstanceName = "hn-api:";
});


// Polly policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<TimeoutRejectedException>()
    .WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1) });
}


static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}


// Typed client
builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(c =>
{
    c.BaseAddress = new Uri("https://hacker-news.firebaseio.com/");
    c.Timeout = TimeSpan.FromSeconds(5);
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy())
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5)));


// DI
builder.Services.AddScoped<IBestStoriesService, BestStoriesService>();



var app = builder.Build();



    app.UseSwagger();
    app.UseSwaggerUI();



app.MapControllers();
app.Run();