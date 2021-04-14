using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ObjectSavePropertyErrorException : ServicePermanentExceptionWithPropertyPath
	{
		public ObjectSavePropertyErrorException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorItemSavePropertyError, messageId, innerException)
		{
		}

		public ObjectSavePropertyErrorException(PropertyPath[] properties, Exception innerException, bool useItemError) : base(useItemError ? CoreResources.IDs.ErrorItemSavePropertyError : CoreResources.IDs.ErrorFolderSavePropertyError, properties, innerException)
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
