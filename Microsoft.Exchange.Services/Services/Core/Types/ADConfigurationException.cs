using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ADConfigurationException : ServicePermanentException
	{
		public ADConfigurationException() : base(CoreResources.IDs.ErrorMailboxConfiguration)
		{
		}

		public ADConfigurationException(Exception innerException) : base(CoreResources.IDs.ErrorMailboxConfiguration, innerException)
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
