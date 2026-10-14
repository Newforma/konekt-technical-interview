// Runs the questions in eval/questions.json through RagService and prints a score card.
//   dotnet run --project src/eval            (offline mode, no network)
//   Llm__Mode=http Llm__BaseUrl=... Llm__ApiKey=... Llm__Model=... dotnet run --project src/eval
// Exit code is always 0: the score is a compass, not a gate.
using System.Text.Json;
using Backend.Rag;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var root = FindRepoRoot();
var config = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(root, "src/backend/appsettings.json"))
    .AddEnvironmentVariables()
    .Build();

var llmOptions = new LlmOptions();
config.GetSection("Llm").Bind(llmOptions);
ILlmClient llm = llmOptions.Mode.Equals("http", StringComparison.OrdinalIgnoreCase)
    ? new OpenAiCompatibleLlmClient(new HttpClient { Timeout = TimeSpan.FromSeconds(60) }, Options.Create(llmOptions))
    : new OfflineLlmClient();

var rag = new RagService(CorpusLoader.Load(Path.Combine(root, "corpus")), llm);
var cases = JsonSerializer.Deserialize<List<EvalCase>>(
    File.ReadAllText(Path.Combine(root, "eval/questions.json")),
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

int answerOk = 0, answerTotal = 0, citeOk = 0, citeTotal = 0, abstainOk = 0, abstainTotal = 0, safeOk = 0, safeTotal = 0;
Console.WriteLine($"Mode: {llmOptions.Mode}\n");
Console.WriteLine("id    answer  cites  abstain  safe");

foreach (var c in cases)
{
    AskResponse r;
    try { r = await rag.AskAsync(c.Question); }
    catch (Exception e) { Console.WriteLine($"{c.Id}   ERROR {e.Message}"); continue; }

    string answer = "-", cites = "-", abstain = "-", safe = "-";

    if (c.AnswerContainsAny is { Count: > 0 })
    {
        var ok = c.AnswerContainsAny.Any(s => r.Answer.Contains(s, StringComparison.OrdinalIgnoreCase));
        answer = Mark(ok); answerTotal++; answerOk += ok ? 1 : 0;
    }
    if (c.ExpectedSources is { Count: > 0 })
    {
        var ok = c.ExpectedSources.All(s => r.Sources.Contains(s)) && r.Sources.Count <= (c.MaxSources ?? int.MaxValue);
        cites = Mark(ok); citeTotal++; citeOk += ok ? 1 : 0;
    }
    if (c.ExpectAbstain)
    {
        var ok = r.Abstained;
        abstain = Mark(ok); abstainTotal++; abstainOk += ok ? 1 : 0;
    }
    if (c.MustNotContain is { Count: > 0 })
    {
        var ok = !c.MustNotContain.Any(s => r.Answer.Contains(s, StringComparison.OrdinalIgnoreCase));
        safe = Mark(ok); safeTotal++; safeOk += ok ? 1 : 0;
    }
    Console.WriteLine($"{c.Id}   {answer,-7} {cites,-6} {abstain,-8} {safe}");
}

Console.WriteLine();
Console.WriteLine($"Answer correctness : {answerOk}/{answerTotal}");
Console.WriteLine($"Valid citations    : {citeOk}/{citeTotal}");
Console.WriteLine($"Abstention         : {abstainOk}/{abstainTotal}");
Console.WriteLine($"Injection safety   : {safeOk}/{safeTotal}");
var total = answerOk + citeOk + abstainOk + safeOk;
var max = answerTotal + citeTotal + abstainTotal + safeTotal;
Console.WriteLine($"TOTAL              : {total}/{max}");

static string Mark(bool ok) => ok ? "PASS" : "FAIL";

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "corpus"))) dir = dir.Parent;
    return dir?.FullName ?? throw new InvalidOperationException("Repo root (with corpus/) not found.");
}

record EvalCase(string Id, string Question, List<string>? AnswerContainsAny, List<string>? ExpectedSources,
    bool ExpectAbstain, List<string>? MustNotContain, int? MaxSources);
