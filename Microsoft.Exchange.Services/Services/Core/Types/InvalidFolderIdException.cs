using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidFolderIdException : InvalidStoreIdException
	{
		public InvalidFolderIdException(Enum messageId) : base(messageId)
		{
		}

		public InvalidFolderIdException(Enum messageId, Exception innerException) : base(messageId, innerException)
		{
		}

		public InvalidFolderIdException(ResponseCodeType responseCode, Enum messageId) : base(responseCode, messageId)
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
