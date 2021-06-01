using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MediaPlayer.Domain.Utilities
{
  public static class StringUtils
  {

    //Split Camel Case function from Stackoverflow: 
    //https://stackoverflow.com/a/5796793
    public static string SplitCamelCase(string input)
    {
      return Regex.Replace(
        Regex.Replace(
          input,
          @"(\P{Ll})(\P{Ll}\p{Ll})", 
            "$1 $2" 
        ), 
        @"(\p{Ll})(\P{Ll})", 
        "$1 $2" 
      );
    }

    //Order By Alphanumeric method from SO post:
    //https://stackoverflow.com/questions/248603/natural-sort-order-in-c-sharp
    public static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
    {
        int max = source.SelectMany(i => Regex.Matches(selector(i), @"\d+")
                .Cast<Match>().Select(m => (int?)m.Value.Length)).Max() ?? 0;
        return source.OrderBy(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0')));
    }
  }
}