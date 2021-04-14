using System;
using System.Globalization;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class Parse821
	{
		internal static bool TryParseAddressLine(string protocolText, out RoutingAddress routingAddress, out string tail)
		{
			tail = null;
			routingAddress = RoutingAddress.Empty;
			try
			{
				routingAddress = Parse821.ParseAddressLine(protocolText, out tail);
				return true;
			}
			catch (FormatException)
			{
			}
			return false;
		}

		internal static RoutingAddress ParseAddressLine(string protocolText, out string tail)
		{
			string address = SmtpToken.SplitString(protocolText, ' ', out tail);
			address = Parse821.TrimRouteAddresses(address);
			if (tail != null)
			{
				tail = tail.TrimStart(new char[0]);
			}
			return new RoutingAddress(address);
		}

		internal static string FormatAddressLine(RoutingAddress smtpAddress, string tail)
		{
			return Parse821.FormatAddressLine(smtpAddress.ToString(), tail);
		}

		internal static string FormatAddressLine(string smtpAddress, string tail)
		{
			if (tail != null)
			{
				return string.Format("{0} {1}", smtpAddress, tail, CultureInfo.InvariantCulture);
			}
			return smtpAddress;
		}

		internal static string TrimRouteAddresses(string address)
		{
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "ExtractCanonicalAddress");
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length == 0)
			{
				return string.Empty;
			}
			if (address[0] == '<')
			{
				if (address[address.Length - 1] != '>')
				{
					return string.Empty;
				}
				address = address.Substring(1, address.Length - 2).Trim();
			}
			if (((string)RoutingAddress.NullReversePath).Equals(address, StringComparison.Ordinal))
			{
				return string.Empty;
			}
			if (address.Length == 0)
			{
				return (string)RoutingAddress.NullReversePath;
			}
			while (address.Length != 0 && address[0] == '@')
			{
				string text = null;
				SmtpToken.SplitString(address, ',', out text);
				if (text == null)
				{
					SmtpToken.SplitString(address, ':', out text);
					if (text == null)
					{
						return string.Empty;
					}
					address = text.TrimStart(null);
					break;
				}
				else
				{
					address = text.TrimStart(null);
				}
			}
			if (address.Length > 1 && address[address.Length - 1] == '.')
			{
				string text2 = null;
				SmtpToken.SplitString(address, '@', out text2);
				if (text2 != null)
				{
					address = address.Substring(0, address.Length - 1);
				}
			}
			ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Extracted \"{0}\"", address);
			return address;
		}
	}
}
