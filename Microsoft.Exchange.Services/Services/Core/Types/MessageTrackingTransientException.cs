using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MessageTrackingTransientException : ServicePermanentException
	{
		public MessageTrackingTransientException() : base((CoreResources.IDs)3399410586U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
