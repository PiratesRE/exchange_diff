using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MissingItemForCreateItemAttachmentException : ServicePermanentException
	{
		public MissingItemForCreateItemAttachmentException() : base(CoreResources.IDs.ErrorMissingItemForCreateItemAttachment)
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
