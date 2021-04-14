using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidSendItemSaveSettingsException : ServicePermanentException
	{
		public InvalidSendItemSaveSettingsException() : base((CoreResources.IDs)3825363766U)
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
