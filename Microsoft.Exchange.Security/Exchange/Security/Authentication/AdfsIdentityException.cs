using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsIdentityException : LocalizedException
	{
		public AdfsIdentityException(string message) : base(new LocalizedString(message))
		{
		}
	}
}
