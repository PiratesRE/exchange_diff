using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class DeleteItemsException : ServicePermanentException
	{
		public DeleteItemsException(Exception innerException) : base(ResponseCodeType.ErrorCannotDeleteObject, (CoreResources.IDs)3912965805U, innerException)
		{
		}

		public DeleteItemsException(Enum messageId) : base(ResponseCodeType.ErrorCannotDeleteObject, messageId)
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
