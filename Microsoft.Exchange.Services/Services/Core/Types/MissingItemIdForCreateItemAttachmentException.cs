using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MissingItemIdForCreateItemAttachmentException : ServicePermanentException
	{
		public MissingItemIdForCreateItemAttachmentException() : base(CoreResources.IDs.ErrorMissingItemForCreateItemAttachment)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
