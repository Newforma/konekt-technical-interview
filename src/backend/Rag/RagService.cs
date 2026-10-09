using System.Text;

namespace Backend.Rag;

public class RagService(Retriever retriever, ILlmClient llm)
{
    private const int TopK = 2;

    private const string SystemPrompt = "You are a helpful assistant. Answer the question using the context.";

    public async Task<AskResponse> AskAsync(string question, CancellationToken ct = default)
    {
        var hits = retriever.Search(question, TopK);

        var context = new StringBuilder();
        foreach (var hit in hits)
        {
            context.AppendLine(hit.Chunk.Text);
        }

        var userPrompt = $"Context:\n{context}\n\nQuestion: {question}";
        var answer = await llm.CompleteAsync(SystemPrompt, userPrompt, ct);

        return new AskResponse(answer, [], false);
    }
}

public static class CorpusLoader
{
    public static Retriever Load(string corpusPath)
    {
        var retriever = new Retriever();
        foreach (var file in Directory.GetFiles(corpusPath, "*.md").OrderBy(f => f, StringComparer.Ordinal))
        {
            retriever.Add(Chunker.Split(Path.GetFileName(file), File.ReadAllText(file)));
        }
        return retriever;
    }
}
