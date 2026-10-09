using System.Text.RegularExpressions;

namespace Backend.Rag;

/// <summary>Small in-memory BM25 index over text chunks. No stemming, no stop words.</summary>
public partial class Retriever
{
    private const double K1 = 1.5;
    private const double B = 0.75;

    private readonly List<Chunk> _chunks = [];
    private readonly List<Dictionary<string, int>> _termCounts = [];
    private readonly List<int> _lengths = [];
    private readonly Dictionary<string, int> _docFrequency = [];
    private double _avgLength;

    public int Count => _chunks.Count;

    public static IEnumerable<string> Tokenize(string text) =>
        TokenRegex().Matches(text.ToLowerInvariant()).Select(m => m.Value);

    public void Add(IEnumerable<Chunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            var counts = new Dictionary<string, int>();
            var length = 0;
            foreach (var token in Tokenize(chunk.Text))
            {
                counts[token] = counts.GetValueOrDefault(token) + 1;
                length++;
            }
            foreach (var term in counts.Keys)
            {
                _docFrequency[term] = _docFrequency.GetValueOrDefault(term) + 1;
            }
            _chunks.Add(chunk);
            _termCounts.Add(counts);
            _lengths.Add(length);
        }
        _avgLength = _lengths.Count == 0 ? 0 : _lengths.Average();
    }

    public IReadOnlyList<ScoredChunk> Search(string query, int k)
    {
        var terms = Tokenize(query).Distinct().ToList();
        var results = new List<ScoredChunk>();
        for (var i = 0; i < _chunks.Count; i++)
        {
            double score = 0;
            foreach (var term in terms)
            {
                if (!_termCounts[i].TryGetValue(term, out var tf)) continue;
                var df = _docFrequency[term];
                var idf = Math.Log(1 + (_chunks.Count - df + 0.5) / (df + 0.5));
                score += idf * tf * (K1 + 1) / (tf + K1 * (1 - B + B * _lengths[i] / _avgLength));
            }
            if (score > 0) results.Add(new ScoredChunk(_chunks[i], score));
        }
        return results.OrderByDescending(r => r.Score).Take(k).ToList();
    }

    [GeneratedRegex(@"[\p{L}\p{N}]+")]
    private static partial Regex TokenRegex();
}
