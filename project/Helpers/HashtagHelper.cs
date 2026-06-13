using System.Text.RegularExpressions;
using System.Net;

namespace project.Helpers;

public static class HashtagHelper
{
    public static string FormatHashtags(string? content)
    {
        if (string.IsNullOrEmpty(content)) return string.Empty;

        // 1. Decode any HTML entities first so &#128293; -> 🔥, &#39; -> '
        //    This normalises whatever is stored in the DB before we process it.
        var decoded = WebUtility.HtmlDecode(content);

        // 2. HTML-encode to prevent XSS.
        //    WebUtility.HtmlEncode only encodes: < > & " '
        //    Emojis and other Unicode pass through untouched.
        var encoded = WebUtility.HtmlEncode(decoded);

        // 3. Replace hashtags with themed clickable links.
        //    Pattern: (?<!&)#[A-Za-z]\w*
        //      (?<!&) – negative lookbehind: skip # that follows & (i.e. HTML entity &#39;)
        //      [A-Za-z] – first char must be a letter, so numeric entities like #39 are skipped
        //      \w*     – followed by any word chars
        var formatted = Regex.Replace(
            encoded,
            @"(?<!&)#[A-Za-z]\w*",
            m =>
            {
                var tag = m.Value;
                var urlTag = WebUtility.UrlEncode(tag.ToLower());
                return $"<a href=\"/Post?hashtag={urlTag}\" class=\"post-hashtag\" " +
                       $"style=\"color:var(--accent);text-decoration:none;font-weight:600;\">{tag}</a>";
            }
        );

        return formatted;
    }
}
