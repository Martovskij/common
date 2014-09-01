﻿using System.Collections.Generic;
using System.Globalization;

namespace Utils
{
    /// <summary>
    /// Локализованная строка.
    /// </summary>
    public class LocalizedString 
    {
      private readonly Dictionary<CultureInfo, string> localizedValues = new Dictionary<CultureInfo, string>();

      /// <summary>
      /// Значение строки в зависимости от установленной культуры.
      /// </summary>
      /// Remarks: если нет значения для текущей культуры - возвращается значение для en-US
      public string Value
      {
        get
        {
          return this.localizedValues.ContainsKey(CultureInfo.CurrentCulture)
            ? this.localizedValues[CultureInfo.CurrentCulture]
            : this.localizedValues[CultureInfo.CreateSpecificCulture("en-US")];
        }
      }

      /// <summary>
      /// Конструктор для одной англоязычной строки.
      /// </summary>
      /// <param name="value"></param>
      public LocalizedString(string value)
      {
        if (value == null)
          value = string.Empty;
        this.localizedValues.Add(CultureInfo.CreateSpecificCulture("en-US"), value);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="ruValue"></param>
      /// <param name="enValue"></param>
      public LocalizedString(string ruValue, string enValue)
      {
        if (ruValue == null)
          ruValue = string.Empty;
        if (enValue == null)
          enValue = string.Empty;
        this.localizedValues.Add(CultureInfo.CreateSpecificCulture("en-US"), enValue);
        this.localizedValues.Add(CultureInfo.CreateSpecificCulture("ru-RU"), ruValue);
      }
    }
}
