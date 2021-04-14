using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedRecurrenceException : ServicePermanentException
	{
		public UnsupportedRecurrenceException() : base((CoreResources.IDs)3322365201U)
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
