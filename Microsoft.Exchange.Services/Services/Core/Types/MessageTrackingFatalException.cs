using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MessageTrackingFatalException : ServicePermanentException
	{
		public MessageTrackingFatalException() : base(CoreResources.IDs.ErrorMessageTrackingPermanentError)
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
