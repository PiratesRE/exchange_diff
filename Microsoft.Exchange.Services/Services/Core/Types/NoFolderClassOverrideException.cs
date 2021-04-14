using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class NoFolderClassOverrideException : ServicePermanentException
	{
		public NoFolderClassOverrideException() : base((CoreResources.IDs)3753602229U)
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
