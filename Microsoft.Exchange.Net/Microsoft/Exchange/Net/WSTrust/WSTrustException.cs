using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal abstract class WSTrustException : LocalizedException
	{
		public WSTrustException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public WSTrustException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}
	}
}
