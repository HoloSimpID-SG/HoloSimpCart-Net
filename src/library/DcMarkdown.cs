using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuTakingTooLong.src.library
{
    public static partial class MoLibrary
    {
        public static string Header1(this string text) => $"# {text}";
        public static string Header2(this string text) => $"## {text}";
        public static string Header3(this string text) => $"### {text}";
        public static string SubText(this string text) => $"-# {text}";
        public static string Bold(this string text) => $"**{text}**";
        public static string Italic(this string text) => $"*{text}*";
        public static string Underline(this string text) => $"__{text}__";
        public static string Strikethrough(this string text) => $"--{text}--";
        public static string Hyperlink(this string text, string link)
        {
            if (link.IsNullOrEmpty())
                return text;
            if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                return $"[{text}]({link})";
            else
                return $"[{text}](https://{link})";
        }
        public static string CodeBlock(this string text) => $"```{text}```";
        public static string BlockQuote(this string text) => $"> {text}";
        public static string BlockQuoteMultiLine(this string text) => $">>> {text}";
        public static string ListAll(this IDictionary<object, object> dict, int indent = 0)
        {
            StringBuilder strResult = new();
            foreach (var kvp in dict)
            {
                object first = kvp.Key;
                object second = kvp.Value;

                strResult.Append(Enumerable.Repeat(' ', indent));
                strResult.Append($"- {first}, ");

                if (second is IEnumerable<object> subList && !(second is string))
                    strResult.AppendLine($"{subList.ListAll(indent + 1)}");
                else
                    strResult.AppendLine($"{second}");
            }
            return strResult.ToString();
        }
        public static string ListAll(this IEnumerable<object> list, int indent = 0)
        {
            StringBuilder strResult = new();
            foreach (var item in list)
            {
                strResult.Append(Enumerable.Repeat(' ', indent));
                strResult.Append("- ");
                if (item is IEnumerable<object> subList && !(item is string))
                    strResult.AppendLine($"{subList.ListAll(indent + 1)}");
                else
                    strResult.AppendLine($"{item}");
            }
            return strResult.ToString();
        }
    }
}