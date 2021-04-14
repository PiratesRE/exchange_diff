using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AttachmentNestLevelLimitExceededException : ServicePermanentException
	{
		public AttachmentNestLevelLimitExceededException() : base(CoreResources.IDs.ErrorAttachmentNestLevelLimitExceeded)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010SP2;
			}
		}
	}
}
