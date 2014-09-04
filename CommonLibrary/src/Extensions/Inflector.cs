using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CommonLibrary
{
  public static class Inflector
  {
    #region Default Rules

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
      Justification = "Инициализация сложная и перенести ее inline не представляется возможным, поэтому оставляем в конструкторе.")]
    static Inflector()
    {
      plurals = new List<Rule>();
      singulars = new List<Rule>();
      uncountables = new List<string>();

      AddPlural("$", "s");
      AddPlural("(ax|test)is$", "$1es");
      AddPlural("(octop|vir|alumn|fung)us$", "$1i");
      AddPlural("(alias|status)$", "$1es");
      AddPlural("(bu)s$", "$1ses");
      AddPlural("(buffal|tomat|volcan)o$", "$1oes");
      AddPlural("([ti])um$", "$1a");
      AddPlural("sis$", "ses");
      AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
      AddPlural("(hive)$", "$1s");
      AddPlural("([^aeiouy]|qu)y$", "$1ies");
      AddPlural("(s|x|ch|ss|sh)$", "$1es");
      AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
      AddPlural("([m|l])ouse$", "$1ice");
      AddPlural("^(ox)$", "$1en");
      AddPlural("(quiz)$", "$1zes");

      AddSingular("s$", string.Empty);
      AddSingular("(n)ews$", "$1ews");
      AddSingular("([ti])a$", "$1um");
      AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
      AddSingular("(^analy)ses$", "$1sis");
      AddSingular("([^f])ves$", "$1fe");
      AddSingular("(hive)s$", "$1");
      AddSingular("(tive)s$", "$1");
      AddSingular("([lr])ves$", "$1f");
      AddSingular("([^aeiouy]|qu)ies$", "$1y");
      AddSingular("(s)eries$", "$1eries");
      AddSingular("(m)ovies$", "$1ovie");
      AddSingular("(x|ch|ss|sh)es$", "$1");
      AddSingular("([m|l])ice$", "$1ouse");
      AddSingular("(bus)es$", "$1");
      AddSingular("(o)es$", "$1");
      AddSingular("(shoe)s$", "$1");
      AddSingular("(cris|ax|test)es$", "$1is");
      AddSingular("(octop|vir|alumn|fung)i$", "$1us");
      AddSingular("(alias|status)es$", "$1");
      AddSingular("^(ox)en", "$1");
      AddSingular("(vert|ind)ices$", "$1ex");
      AddSingular("(matr)ices$", "$1ix");
      AddSingular("(quiz)zes$", "$1");

      AddIrregular("person", "people");
      AddIrregular("man", "men");
      AddIrregular("child", "children");
      AddIrregular("sex", "sexes");
      AddIrregular("move", "moves");
      AddIrregular("goose", "geese");
      AddIrregular("alumna", "alumnae");

      AddUncountable("equipment");
      AddUncountable("information");
      AddUncountable("rice");
      AddUncountable("money");
      AddUncountable("species");
      AddUncountable("series");
      AddUncountable("fish");
      AddUncountable("sheep");
      AddUncountable("deer");
      AddUncountable("aircraft");
    }

    #endregion

    private class Rule
    {
      private readonly Regex regex;
      private readonly string replacement;

      public Rule(string pattern, string replacement)
      {
        this.regex = new Regex(pattern, RegexOptions.IgnoreCase);
        this.replacement = replacement;
      }

      public string Apply(string word)
      {
        if (!this.regex.IsMatch(word))
        {
          return null;
        }

        return this.regex.Replace(word, this.replacement);
      }
    }

    private static void AddIrregular(string singular, string plural)
    {
      AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
      AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
    }

    private static void AddUncountable(string word)
    {
      uncountables.Add(word.ToLower(CultureInfo.CurrentCulture));
    }

    private static void AddPlural(string rule, string replacement)
    {
      plurals.Add(new Rule(rule, replacement));
    }

    private static void AddSingular(string rule, string replacement)
    {
      singulars.Add(new Rule(rule, replacement));
    }

    private static readonly List<Rule> plurals;
    private static readonly List<Rule> singulars;
    private static readonly List<string> uncountables;

    public static string Pluralize(this string word)
    {
      return ApplyRules(plurals, word);
    }

    public static string Singularize(this string word)
    {
      return ApplyRules(singulars, word);
    }

    private static string ApplyRules(List<Rule> rules, string word)
    {
      string result = word;

      if (!uncountables.Contains(word.ToLower(CultureInfo.CurrentCulture)))
      {
        for (int i = rules.Count - 1; i >= 0; i--)
        {
          if ((result = rules[i].Apply(word)) != null)
          {
            break;
          }
        }
      }

      return result;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Titleize",
      Justification = "Название верное и соответствует выполняемым функциям")]
    public static string Titleize(this string word)
    {
      return Regex.Replace(Humanize(Underscore(word)), @"\b([a-z])",
                           delegate(Match match)
                           {
                             return match.Captures[0].Value.ToUpper(CultureInfo.CurrentCulture);
                           });
    }

    public static string Humanize(this string lowercaseAndUnderscoredWord)
    {
      return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pascalize",
      Justification = "Название верное и соответствует выполняемым функциям")]
    public static string Pascalize(this string lowercaseAndUnderscoredWord)
    {
      return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)",
                           delegate(Match match)
                           {
                             return match.Groups[1].Value.ToUpper(CultureInfo.CurrentCulture);
                           });
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Camelize",
      Justification = "Название верное и соответствует выполняемым функциям")]
    public static string Camelize(this string lowercaseAndUnderscoredWord)
    {
      return Uncapitalize(Pascalize(lowercaseAndUnderscoredWord));
    }

    public static string Underscore(this string pascalCasedWord)
    {
      return Regex.Replace(
          Regex.Replace(
              Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
              "$1_$2"), @"[-\s]", "_").ToLower(CultureInfo.CurrentCulture);
    }

    public static string Capitalize(this string word)
    {
      return word.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + word.Substring(1).ToLower(CultureInfo.CurrentCulture);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uncapitalize",
      Justification = "Название верное и соответствует выполняемым функциям")]
    public static string Uncapitalize(this string word)
    {
      return word.Substring(0, 1).ToLower(CultureInfo.CurrentCulture) + word.Substring(1);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ordinalize",
      Justification = "Название верное и соответствует выполняемым функциям")]
    public static string Ordinalize(this string value)
    {
      return Ordanize(int.Parse(value), value);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ordinalize",
      Justification = "Название верное и соответствует выполняемым функциям")]
    public static string Ordinalize(this int number)
    {
      return Ordanize(number, number.ToString());
    }

    private static string Ordanize(int number, string numberString)
    {
      int nMod100 = number % 100;

      if (nMod100 >= 11 && nMod100 <= 13)
      {
        return numberString + "th";
      }

      switch (number % 10)
      {
        case 1:
          return numberString + "st";
        case 2:
          return numberString + "nd";
        case 3:
          return numberString + "rd";
        default:
          return numberString + "th";
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dasherize",
      Justification = "Название верное и соответствует выполняемым функциям")]
    public static string Dasherize(this string underscoredWord)
    {
      return underscoredWord.Replace('_', '-');
    }
  }
}