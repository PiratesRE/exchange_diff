using System;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsConfigurationException : AdfsIdentityException
	{
		public AdfsConfigErrorReason Reason { get; private set; }

		public AdfsConfigurationException(AdfsConfigErrorReason reason, string message) : base(message)
		{
			this.Reason = reason;
		}
	}
}
