using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class MissingUserIdInformationException : ServicePermanentException
	{
		public MissingUserIdInformationException() : base(CoreResources.IDs.ErrorMissingUserIdInformation)
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
