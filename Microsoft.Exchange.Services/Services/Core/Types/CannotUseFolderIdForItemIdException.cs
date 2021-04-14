using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotUseFolderIdForItemIdException : ServicePermanentException
	{
		public CannotUseFolderIdForItemIdException() : base((CoreResources.IDs)2770848984U)
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
