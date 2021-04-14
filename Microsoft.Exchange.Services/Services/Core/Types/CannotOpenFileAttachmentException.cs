using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotOpenFileAttachmentException : ServicePermanentException
	{
		public CannotOpenFileAttachmentException(Exception innerException) : base(CoreResources.IDs.ErrorCannotOpenFileAttachment, innerException)
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
