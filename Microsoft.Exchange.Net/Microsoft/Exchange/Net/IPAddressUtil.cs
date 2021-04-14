using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal static class IPAddressUtil
	{
		internal static bool IsIntranetAddress(Uri serverUrl)
		{
			if (serverUrl == null || string.IsNullOrEmpty(serverUrl.Host))
			{
				IPAddressUtil.Tracer.TraceError(0L, "IPAddressUtil.IsIntranetAddress(): Input parameter is invalid. Server name is either null or empty");
				throw new ArgumentNullException("serverUrl");
			}
			if (serverUrl.HostNameType != UriHostNameType.IPv4 && serverUrl.HostNameType != UriHostNameType.IPv6 && serverUrl.HostNameType != UriHostNameType.Dns)
			{
				IPAddressUtil.Tracer.TraceError<string>(0L, "IPAddressUtil.IsIntranetAddress(): The HostNameType of the Uri is of unsupported type. This API supports only IPV4, IPV6 and DNS type hostnametype, got {0}", serverUrl.HostNameType.ToString());
				throw new ArgumentOutOfRangeException("serverUrl");
			}
			return IPAddressUtil.IsIntranetAddress(serverUrl.Host);
		}

		internal static bool IsIntranetAddress(string hostName)
		{
			IPAddress item = null;
			List<IPAddress> list = new List<IPAddress>();
			if (string.IsNullOrEmpty(hostName))
			{
				IPAddressUtil.Tracer.TraceError(0L, "IPAddressUtil.IsIntranetAddress(): Input parameter is invalid. Hostname is either null or empty");
				throw new ArgumentNullException("hostName");
			}
			if (!IPAddress.TryParse(hostName, out item))
			{
				IPAddress[] array = null;
				try
				{
					array = Dns.GetHostAddresses(hostName);
				}
				catch (Exception ex)
				{
					IPAddressUtil.Tracer.TraceError<string, string>(0L, "IPAddressUtil.IsIntranetAddress(): Unknown exception has occured while trying to find IP address of server name '{0}. The exception is {1}", hostName, ex.ToString());
					throw;
				}
				if (array != null)
				{
					foreach (IPAddress ipaddress in array)
					{
						if (ipaddress.AddressFamily == AddressFamily.InterNetwork || ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
						{
							list.Add(ipaddress);
						}
					}
				}
				if (list.Count == 0)
				{
					IPAddressUtil.Tracer.TraceError(0L, "IPAddressUtil.IsIntranetAddress(): Did not get an IPAddress from DNS");
					return false;
				}
			}
			else
			{
				list.Add(item);
			}
			bool result = false;
			foreach (IPAddress ipaddress2 in list)
			{
				if (ipaddress2.AddressFamily == AddressFamily.InterNetwork)
				{
					uint num = IPAddressUtil.CovertIPAdressToInt(ipaddress2.ToString());
					if (IPAddress.IsLoopback(ipaddress2))
					{
						result = true;
						break;
					}
					if (num >= 167772160U && num <= 184549375U)
					{
						result = true;
						break;
					}
					if (num >= 2886729728U && num <= 2887778303U)
					{
						result = true;
						break;
					}
					if (num >= 3232235520U && num <= 3232301055U)
					{
						result = true;
						break;
					}
					if (num >= 3758096384U && num <= 4026531839U)
					{
						result = true;
						break;
					}
				}
				else if (ipaddress2.AddressFamily == AddressFamily.InterNetworkV6)
				{
					byte[] addressBytes = ipaddress2.GetAddressBytes();
					if (addressBytes[0] == 252 || addressBytes[0] == 253)
					{
						result = true;
						break;
					}
					if (addressBytes[0] == 255)
					{
						result = true;
						break;
					}
					if (IPAddress.IsLoopback(ipaddress2))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private static uint CovertIPAdressToInt(string ipV4AddressStr)
		{
			IPAddress ipaddress = IPAddress.Parse(ipV4AddressStr);
			if (ipaddress.AddressFamily != AddressFamily.InterNetwork)
			{
				throw new ArgumentOutOfRangeException("ipV4AddressStr");
			}
			byte[] addressBytes = ipaddress.GetAddressBytes();
			uint num = (uint)((uint)addressBytes[0] << 24);
			num += (uint)((uint)addressBytes[1] << 16);
			num += (uint)((uint)addressBytes[2] << 8);
			return num + (uint)addressBytes[3];
		}

		private static readonly Trace Tracer = ExTraceGlobals.CommonTracer;
	}
}
