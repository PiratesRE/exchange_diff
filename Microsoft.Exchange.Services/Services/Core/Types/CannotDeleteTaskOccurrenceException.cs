using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotDeleteTaskOccurrenceException : ServicePermanentException
	{
		public CannotDeleteTaskOccurrenceException() : base((CoreResources.IDs)3049158008U)
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
