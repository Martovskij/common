using System;
using System.Security.Cryptography;

namespace CommonLibrary
{
  /// <summary>
  /// Расширения для сериализации RSAParameters.
  /// </summary>
  public static class RsaExtensions
  {
    public static string ToXmlString(this RSAParameters clientKey)
    {     
      using (var csProvider = new RSACryptoServiceProvider())
      {
        csProvider.ImportParameters(clientKey);
        return csProvider.ToXmlString(false);
      }
    }

    public static RSAParameters FromXmlString(this string clientKey)
    {
      using (var csProvider = new RSACryptoServiceProvider())
      {
        csProvider.FromXmlString(clientKey);
        return csProvider.ExportParameters(false);
      }
    }
  }
}
