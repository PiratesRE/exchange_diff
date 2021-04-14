using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class SentMeetingRequestUpdateException : ServicePermanentException
	{
		public SentMeetingRequestUpdateException() : base((CoreResources.IDs)3080514177U)
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
