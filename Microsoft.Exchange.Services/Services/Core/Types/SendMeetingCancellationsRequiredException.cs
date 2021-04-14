using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class SendMeetingCancellationsRequiredException : ServicePermanentException
	{
		public SendMeetingCancellationsRequiredException() : base(CoreResources.IDs.ErrorSendMeetingCancellationsRequired)
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
