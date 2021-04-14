using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidIdXmlException : ServicePermanentException
	{
		public InvalidIdXmlException() : base((CoreResources.IDs)3852956793U)
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
