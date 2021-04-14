using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class ApplyConversationActionException : ServicePermanentException
	{
		public ApplyConversationActionException() : base(CoreResources.IDs.ErrorApplyConversationActionFailed)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010SP1;
			}
		}
	}
}
