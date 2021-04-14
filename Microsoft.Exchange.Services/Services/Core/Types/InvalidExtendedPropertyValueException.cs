using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidExtendedPropertyValueException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidExtendedPropertyValueException(ExtendedPropertyUri propertyPath) : base((CoreResources.IDs)3635256568U, propertyPath)
		{
		}

		public InvalidExtendedPropertyValueException(ExtendedPropertyUri propertyPath, Exception innerException) : base((CoreResources.IDs)3635256568U, propertyPath, innerException)
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
