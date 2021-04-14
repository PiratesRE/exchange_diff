using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class EmailAddressMismatchException : ServicePermanentException
	{
		public EmailAddressMismatchException() : base(CoreResources.IDs.ErrorEmailAddressMismatch)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
