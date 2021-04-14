using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ImGroupDisplayNameAlreadyExistsException : ServicePermanentException
	{
		public ImGroupDisplayNameAlreadyExistsException() : base((CoreResources.IDs)3809605342U)
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
