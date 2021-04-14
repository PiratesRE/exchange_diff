using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidPropertyDeleteException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidPropertyDeleteException(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorInvalidPropertyDelete, propertyPath)
		{
		}

		public InvalidPropertyDeleteException(Enum messageId, PropertyPath propertyPath) : base(ResponseCodeType.ErrorInvalidPropertyDelete, messageId, propertyPath)
		{
		}

		public InvalidPropertyDeleteException(PropertyPath propertyPath, Exception innerException) : base(CoreResources.IDs.ErrorInvalidPropertyDelete, propertyPath, innerException)
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
