using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AttachmentSizeLimitExceededException : ServicePermanentException
	{
		public AttachmentSizeLimitExceededException() : base(CoreResources.IDs.ErrorAttachmentSizeLimitExceeded)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		internal const int AttachmentDataSizeLimit = 2147483647;
	}
}
