namespace Backend.Rag;

public record AskRequest(string Question);

/// <summary>Contract of POST /api/ask. The front end and the eval both rely on it.</summary>
/// <param name="Answer">Answer text shown to the user.</param>
/// <param name="Sources">File names (e.g. "rfi-014-office-tower-a.md") that support the answer.</param>
/// <param name="Abstained">True when the system declines to answer because the corpus has no answer.</param>
public record AskResponse(string Answer, IReadOnlyList<string> Sources, bool Abstained);

public record Chunk(string Source, string Text);

public record ScoredChunk(Chunk Chunk, double Score);
