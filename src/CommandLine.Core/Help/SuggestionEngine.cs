using System.Collections.Generic;
using System.Linq;

namespace SandlotWizards.CommandLineParser.Help;
public static class SuggestionEngine
{
    public static string SuggestClosest(string input, IEnumerable<string> options)
    {
        return options
            .OrderBy(opt => LevenshteinDistance(input, opt))
            .FirstOrDefault() ?? string.Empty;
    }

    private static int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        var d = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) d[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                d[i, j] = new[] {
                    d[i - 1, j] + 1,
                    d[i, j - 1] + 1,
                    d[i - 1, j - 1] + cost
                }.Min();
            }
        }

        return d[s.Length, t.Length];
    }
}
