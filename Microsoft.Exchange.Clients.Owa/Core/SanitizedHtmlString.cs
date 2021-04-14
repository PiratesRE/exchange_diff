using System;
using System.Globalization;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class SanitizedHtmlString : SanitizedStringBase<OwaHtml>
	{
		public SanitizedHtmlString()
		{
		}

		public SanitizedHtmlString(string rawValue) : base(rawValue)
		{
		}

		public static SanitizedHtmlString Empty
		{
			get
			{
				return SanitizedHtmlString.empty;
			}
		}

		public static SanitizedHtmlString Format(string format, params object[] args)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString();
			sanitizedHtmlString.UntrustedValue = StringSanitizer<OwaHtml>.SanitizeFormat(CultureInfo.InvariantCulture, format, args);
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		public static SanitizedHtmlString Format(IFormatProvider provider, string format, params object[] args)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString();
			sanitizedHtmlString.UntrustedValue = StringSanitizer<OwaHtml>.SanitizeFormat(provider, format, args);
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		public static SanitizedHtmlString FromStringId(Strings.IDs id)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(LocalizedStrings.GetHtmlEncoded(id));
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		public static SanitizedHtmlString FromStringId(Strings.IDs id, CultureInfo userCulture)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(LocalizedStrings.GetHtmlEncoded(id, userCulture));
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		public static SanitizedHtmlString GetNonEncoded(Strings.IDs id)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(LocalizedStrings.GetNonEncoded(id));
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		public static SanitizedHtmlString GetSanitizedStringWithoutEncoding(string s)
		{
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(s);
			sanitizedHtmlString.DecreeToBeTrusted();
			return sanitizedHtmlString;
		}

		private static readonly SanitizedHtmlString empty = new SanitizedHtmlString(string.Empty);
	}
}
