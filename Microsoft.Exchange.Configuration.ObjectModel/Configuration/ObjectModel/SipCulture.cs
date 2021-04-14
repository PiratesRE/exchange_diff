using System;
using System.Globalization;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal struct SipCulture
	{
		internal SipCulture(CultureInfo parentCulture, string languageCode)
		{
			this.parentCulture = parentCulture;
			this.languageCode = languageCode;
		}

		private CultureInfo parentCulture;

		private string languageCode;
	}
}
