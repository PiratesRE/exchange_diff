using System;
using System.IO;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaHtml : ISanitizingPolicy
	{
		public string Sanitize(string str)
		{
			return Utilities.HtmlEncode(str);
		}

		public void Sanitize(TextWriter writer, string str)
		{
			Utilities.HtmlEncode(str, writer);
		}

		public string SanitizeFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			SanitizingFormatProvider<OwaHtml> sanitizingFormatProvider = this.owaFormatProvider;
			if (sanitizingFormatProvider == null || sanitizingFormatProvider.InnerFormatProvider != formatProvider)
			{
				sanitizingFormatProvider = (this.owaFormatProvider = new SanitizingFormatProvider<OwaHtml>(formatProvider));
			}
			return string.Format(sanitizingFormatProvider, format, args);
		}

		public string EscapeJScript(string rawValue)
		{
			return Utilities.JavascriptEncode(rawValue);
		}

		private SanitizingFormatProvider<OwaHtml> owaFormatProvider;
	}
}
