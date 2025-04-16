﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BlogApi.Application.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string phrase)
    {
        string str = phrase.ToLowerInvariant();
        str = RemoveDiacritics(str);
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", "-").Trim('-');
        return str;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
