using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidExtendedPropertyException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidExtendedPropertyException(ExtendedPropertyUri offendingProperty) : base(CoreResources.IDs.ErrorInvalidExtendedProperty, offendingProperty)
		{
		}

		public InvalidExtendedPropertyException(ExtendedPropertyUri offendingProperty, Exception innerException) : base(CoreResources.IDs.ErrorInvalidExtendedProperty, offendingProperty, innerException)
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
