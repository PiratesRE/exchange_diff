using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class EmptyFolderException : ServicePermanentException
	{
		public EmptyFolderException(Exception innerException) : base(ResponseCodeType.ErrorCannotEmptyFolder, (CoreResources.IDs)2838198776U, innerException)
		{
		}

		public EmptyFolderException(Enum messageId) : base(ResponseCodeType.ErrorCannotEmptyFolder, messageId)
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
