using System.Text.RegularExpressions;

namespace MediaPlayer.Domain.Utilities
{
  public class StringUtils
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
  }
}