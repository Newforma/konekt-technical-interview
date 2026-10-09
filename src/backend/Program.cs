using Backend.Data;
using Backend.Rag;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// RAG: the corpus is loaded once at startup. LLM mode is configured in appsettings ("Llm" section).
builder.Services.Configure<LlmOptions>(builder.Configuration.GetSection("Llm"));
var corpusPath = Path.GetFullPath(builder.Configuration["Rag:CorpusPath"] ?? "../../corpus", builder.Environment.ContentRootPath);
builder.Services.AddSingleton(_ => CorpusLoader.Load(corpusPath));
if (string.Equals(builder.Configuration["Llm:Mode"], "http", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpClient<ILlmClient, OpenAiCompatibleLlmClient>();
}
else
{
    builder.Services.AddSingleton<ILlmClient, OfflineLlmClient>();
}
builder.Services.AddSingleton<RagService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("*")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
