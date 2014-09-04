using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace CommonLibrary
{
  /// <summary>
  /// Класс для работы с сетевыми интерфейсами.
  /// </summary>
  public static class NetworkInterfaces
  {
    /// <summary>
    /// Словарь сетевых интерфейсов, индексированных по внутреннему номеру.
    /// </summary>
    private static readonly IDictionary<uint, NetworkInterface> networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
        .Select(ni => new { NetworkInterface = ni, indexField = ni.GetType().GetField("index", BindingFlags.Instance | BindingFlags.NonPublic) })
        .Where(item => item.indexField != null)
        .Select(item => new { item.NetworkInterface, Index = (uint)item.indexField.GetValue(item.NetworkInterface) })
        .ToDictionary(item => item.Index, item => item.NetworkInterface);

    /// <summary>
    /// Получить сетевой интерфейс, через котрый будет идти обращение к указанному адресу.
    /// </summary>
    /// <param name="address">IP-адрес.</param>
    /// <returns>Сетевой интерфейс.</returns>
    /// <remarks>По мотивам http://pastebin.com/u9159Ys8 .</remarks>
    public static NetworkInterface GetBestInterface(IPAddress address)
    {
      byte[] addressBytes = address.GetAddressBytes();
      uint ipaddr = BitConverter.ToUInt32(addressBytes, 0);
      uint interfaceIndex;
      int error = NativeMethods.GetBestInterface(ipaddr, out interfaceIndex);

      if (error != 0)
        return null;

      NetworkInterface result;
      return networkInterfaces.TryGetValue(interfaceIndex, out result) ? result : null;
    }
  }
}
