using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidPropertySetException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidPropertySetException(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorInvalidPropertySet, propertyPath)
		{
		}

		public InvalidPropertySetException(Enum messageId, PropertyPath propertyPath) : base(ResponseCodeType.ErrorInvalidPropertySet, messageId, propertyPath)
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
