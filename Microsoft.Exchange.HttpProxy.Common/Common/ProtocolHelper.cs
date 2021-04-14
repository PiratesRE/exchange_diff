using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class ProtocolHelper
	{
		public static string GetExplicitLogonNode(string applicationPath, string filePath, ExplicitLogonNode node, out bool selectedNodeIsLast)
		{
			ArgumentValidator.ThrowIfNull("applicationPath", applicationPath);
			ArgumentValidator.ThrowIfNull("filePath", filePath);
			selectedNodeIsLast = false;
			string text = null;
			bool flag = false;
			int num = applicationPath.Length;
			if (num < filePath.Length && filePath[num] == '/')
			{
				num++;
			}
			int num2 = filePath.IndexOf('/', num);
			string text2 = (num2 == -1) ? filePath.Substring(num) : filePath.Substring(num, num2 - num);
			bool flag2 = num2 == -1;
			if (!flag2 && node == ExplicitLogonNode.Third)
			{
				int num3 = filePath.IndexOf('/', num2 + 1);
				text = ((num3 == -1) ? filePath.Substring(num2 + 1) : filePath.Substring(num2 + 1, num3 - num2 - 1));
				flag = (num3 == -1);
			}
			string result;
			switch (node)
			{
			case ExplicitLogonNode.Second:
				result = text2;
				selectedNodeIsLast = flag2;
				break;
			case ExplicitLogonNode.Third:
				result = text;
				selectedNodeIsLast = flag;
				break;
			default:
				throw new InvalidOperationException("somebody expanded ExplicitLogonNode and didn't tell TryGetExplicitLogonNode!");
			}
			return result;
		}

		public static bool TryGetValidNormalizedExplicitLogonAddress(string explicitLogonAddress, out string normalizedAddress)
		{
			return ProtocolHelper.TryGetValidNormalizedExplicitLogonAddress(explicitLogonAddress, false, out normalizedAddress);
		}

		public static bool TryGetValidNormalizedExplicitLogonAddress(string explicitLogonAddress, bool selectedNodeIsLast, out string normalizedAddress)
		{
			normalizedAddress = null;
			if (string.IsNullOrEmpty(explicitLogonAddress))
			{
				return false;
			}
			if (selectedNodeIsLast)
			{
				return false;
			}
			normalizedAddress = explicitLogonAddress.Replace("...", ".@").Replace("..", "@");
			return SmtpAddress.IsValidSmtpAddress(normalizedAddress);
		}

		public static Uri GetClientUrlForProxy(Uri url, string explicitLogonAddress)
		{
			ArgumentValidator.ThrowIfNull("url", url);
			ArgumentValidator.ThrowIfNull("explicitLogonAddress", explicitLogonAddress);
			string text = "/" + explicitLogonAddress;
			string text2 = url.ToString();
			int num = text2.IndexOf(text);
			if (num != -1)
			{
				text2 = text2.Substring(0, num) + text2.Substring(num + text.Length);
			}
			return new Uri(text2);
		}

		public static bool IsODataRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.StartsWith("/odata/", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsOAuthMetadataRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.IndexOf("/metadata/", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool IsAutodiscoverV2Request(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return ProtocolHelper.IsAutodiscoverV2Version1Request(path) || ProtocolHelper.IsAutodiscoverV2PreviewRequest(path);
		}

		public static bool IsAutodiscoverV2Version1Request(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.IndexOf("/autodiscover.json/v1.0", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool IsAutodiscoverV2PreviewRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.IndexOf("/autodiscover.json", StringComparison.OrdinalIgnoreCase) >= 0 && path.IndexOf("/autodiscover.json/v1.0", StringComparison.OrdinalIgnoreCase) == -1;
		}

		public static bool IsEwsODataRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.StartsWith("/ews/odata/", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsEwsGetUserPhotoRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.StartsWith("/ews/exchange.asmx/s/GetUserPhoto", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsAnyWsSecurityRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return ProtocolHelper.IsWsSecurityRequest(path) || ProtocolHelper.IsPartnerAuthRequest(path) || ProtocolHelper.IsX509CertAuthRequest(path);
		}

		public static bool IsWsSecurityRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.EndsWith("wssecurity", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsPartnerAuthRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.EndsWith("wssecurity/symmetrickey", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsX509CertAuthRequest(string path)
		{
			ArgumentValidator.ThrowIfNull("path", path);
			return path.EndsWith("wssecurity/x509cert", StringComparison.OrdinalIgnoreCase);
		}
	}
}
