using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class FolderNotFoundException : ServicePermanentException
	{
		public FolderNotFoundException() : base((CoreResources.IDs)3395659933U)
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
