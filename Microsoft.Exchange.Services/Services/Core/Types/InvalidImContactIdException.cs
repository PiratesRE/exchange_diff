using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidImContactIdException : ServicePermanentException
	{
		public InvalidImContactIdException() : base((CoreResources.IDs)3485828594U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
