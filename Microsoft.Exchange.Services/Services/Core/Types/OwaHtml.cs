using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class OwaHtml : ISanitizingPolicy
	{
		public string Sanitize(string str)
		{
			return WebUtility.HtmlEncode(str);
		}

		public void Sanitize(TextWriter writer, string str)
		{
			WebUtility.HtmlEncode(str, writer);
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

		private SanitizingFormatProvider<OwaHtml> owaFormatProvider;
	}
}
