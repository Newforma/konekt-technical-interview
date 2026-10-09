using Backend.Rag;
using Xunit;

namespace Tests;

public class RagTests
{
    [Fact]
    public void Tokenize_lowercases_and_splits_on_punctuation()
    {
        Assert.Equal(["fire", "rated", "90", "minutes"], Retriever.Tokenize("Fire-rated, 90 Minutes!"));
    }

    [Fact]
    public void Chunker_covers_the_whole_text()
    {
        var text = new string('a', Chunker.ChunkSize * 2 + 10);
        var chunks = Chunker.Split("x.md", text).ToList();
        Assert.Equal(3, chunks.Count);
        Assert.Equal(text, string.Concat(chunks.Select(c => c.Text)));
    }

    [Fact]
    public void Search_ranks_the_matching_chunk_first()
    {
        var retriever = new Retriever();
        retriever.Add([new Chunk("a.md", "crane inspection failed"), new Chunk("b.md", "wind speed limit")]);
        var hits = retriever.Search("crane", 2);
        Assert.Single(hits);
        Assert.Equal("a.md", hits[0].Chunk.Source);
    }

    [Fact]
    public async Task Ask_returns_an_answer_in_offline_mode()
    {
        var retriever = new Retriever();
        retriever.Add([new Chunk("a.md", "The glazing needs 90 minutes of fire resistance.")]);
        var response = await new RagService(retriever, new OfflineLlmClient()).AskAsync("fire resistance glazing");
        Assert.Contains("90 minutes", response.Answer);
    }
}
