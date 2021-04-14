using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotUseItemIdForFolderIdException : ServicePermanentException
	{
		public CannotUseItemIdForFolderIdException() : base((CoreResources.IDs)2423603834U)
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
