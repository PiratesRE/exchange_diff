using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidPropertyAppendException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidPropertyAppendException(PropertyPath propertyPath) : base((CoreResources.IDs)3619206730U, propertyPath)
		{
		}

		public InvalidPropertyAppendException(Enum messageId, PropertyPath propertyPath) : base(ResponseCodeType.ErrorInvalidPropertyAppend, messageId, propertyPath)
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
