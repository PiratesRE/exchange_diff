using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal static class Parse
	{
		internal static TimeSpan? ParseFromMilliseconds(string value)
		{
			TimeSpan? result = null;
			int num;
			if (!string.IsNullOrEmpty(value) && int.TryParse(value, out num))
			{
				Exception ex = null;
				try
				{
					result = new TimeSpan?(TimeSpan.FromMilliseconds((double)num));
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				catch (OverflowException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<Exception>(0, "Error decoding timeout property: {0}", ex);
				}
			}
			return result;
		}

		internal static bool IsSMSRecipient(string recipient)
		{
			ProxyAddress proxyAddress;
			return SmtpProxyAddress.TryDeencapsulate(recipient, out proxyAddress) && string.Equals(proxyAddress.PrefixString, "MOBILE", StringComparison.OrdinalIgnoreCase);
		}

		internal static string RemoveControlChars(string s)
		{
			StringBuilder stringBuilder = null;
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (char.IsControl(s, i))
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(s.Length);
						stringBuilder.Append(s, 0, i);
					}
				}
				else if (stringBuilder != null)
				{
					stringBuilder.Append(s[i]);
				}
			}
			if (stringBuilder != null)
			{
				return stringBuilder.ToString();
			}
			return s;
		}
	}
}
