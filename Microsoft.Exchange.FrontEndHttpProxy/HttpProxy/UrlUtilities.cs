using System;
using System.Linq;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class UrlUtilities
	{
		public static bool IsEcpUrl(string urlString)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return false;
			}
			if (urlString.Equals("/ecp", StringComparison.OrdinalIgnoreCase) || urlString.StartsWith("/ecp/", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			Uri uri = null;
			return Uri.TryCreate(urlString, UriKind.Absolute, out uri) && uri != null && (uri.AbsolutePath.Equals("/ecp", StringComparison.OrdinalIgnoreCase) || uri.AbsolutePath.StartsWith("/ecp/", StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsEacUrl(string urlString)
		{
			if (!UrlUtilities.IsEcpUrl(urlString))
			{
				return false;
			}
			int num = urlString.IndexOf('?');
			if (num > 0)
			{
				string[] source = urlString.Substring(num + 1).Split(new char[]
				{
					'&'
				});
				return !source.Contains("rfr=owa") && !source.Contains("rfr=olk");
			}
			return true;
		}

		public static bool IsIntegratedAuthUrl(Uri url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			return url.AbsolutePath.IndexOf(Constants.IntegratedAuthPathWithTrailingSlash, StringComparison.OrdinalIgnoreCase) != -1 || url.AbsolutePath.EndsWith(Constants.IntegratedAuthPath, StringComparison.OrdinalIgnoreCase);
		}

		public static Uri FixIntegratedAuthUrlForBackEnd(Uri url)
		{
			if (!UrlUtilities.IsIntegratedAuthUrl(url))
			{
				return url;
			}
			UriBuilder uriBuilder = new UriBuilder(url);
			string absolutePath = url.AbsolutePath;
			int num = url.AbsolutePath.IndexOf(Constants.IntegratedAuthPath, StringComparison.OrdinalIgnoreCase);
			uriBuilder.Path = absolutePath.Substring(0, num) + absolutePath.Substring(num + Constants.IntegratedAuthPath.Length);
			return uriBuilder.Uri;
		}

		public static Uri FixDFPOWAVdirUrlForBackEnd(Uri url, string dfpOwaVdir)
		{
			if (string.IsNullOrEmpty(dfpOwaVdir))
			{
				return url;
			}
			UriBuilder uriBuilder = new UriBuilder(url);
			string absolutePath = url.AbsolutePath;
			int num = url.AbsolutePath.IndexOf("/owa", StringComparison.OrdinalIgnoreCase);
			uriBuilder.Path = absolutePath.Substring(0, num + 1) + dfpOwaVdir;
			int num2 = num + "/owa".Length;
			if (absolutePath.Length > num2)
			{
				uriBuilder.Path += absolutePath.Substring(num2);
			}
			return uriBuilder.Uri;
		}

		internal static bool IsOwaMiniUrl(Uri url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			return url.AbsolutePath.EndsWith(Constants.OMAPath, StringComparison.OrdinalIgnoreCase) || url.AbsolutePath.IndexOf(Constants.OMAPath + "/", StringComparison.OrdinalIgnoreCase) != -1;
		}

		internal static bool IsCmdWebPart(HttpRequest request)
		{
			string text = request.QueryString["cmd"];
			return !string.IsNullOrEmpty(text) && string.Equals(text, "contents", StringComparison.OrdinalIgnoreCase);
		}

		private const string Command = "cmd";

		private const string CommandValue = "contents";
	}
}
