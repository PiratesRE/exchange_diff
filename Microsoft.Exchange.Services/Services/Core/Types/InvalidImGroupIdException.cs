using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidImGroupIdException : ServicePermanentException
	{
		public InvalidImGroupIdException() : base(CoreResources.IDs.ErrorInvalidImGroupId)
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
