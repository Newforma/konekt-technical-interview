namespace Backend.Rag;

public static class Chunker
{
    public const int ChunkSize = 300;

    public static IEnumerable<Chunk> Split(string source, string text)
    {
        for (var i = 0; i < text.Length; i += ChunkSize)
        {
            var length = Math.Min(ChunkSize, text.Length - i);
            yield return new Chunk(source, text.Substring(i, length));
        }
    }
}
