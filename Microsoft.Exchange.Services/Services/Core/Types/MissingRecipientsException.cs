using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MissingRecipientsException : ServicePermanentException
	{
		public MissingRecipientsException() : base((CoreResources.IDs)2985674644U)
		{
		}

		public MissingRecipientsException(Exception innerException) : base((CoreResources.IDs)2985674644U, innerException)
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
