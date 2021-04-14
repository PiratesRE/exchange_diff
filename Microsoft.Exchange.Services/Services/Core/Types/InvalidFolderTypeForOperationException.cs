using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidFolderTypeForOperationException : ServicePermanentException
	{
		public InvalidFolderTypeForOperationException(Enum messageId) : base(ResponseCodeType.ErrorInvalidFolderTypeForOperation, messageId)
		{
		}

		public InvalidFolderTypeForOperationException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidFolderTypeForOperation, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
