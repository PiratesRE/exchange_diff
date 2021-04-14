using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DelegateSaveFailedException : ServicePermanentException
	{
		public DelegateSaveFailedException(CoreResources.IDs errorCode) : base(errorCode)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
