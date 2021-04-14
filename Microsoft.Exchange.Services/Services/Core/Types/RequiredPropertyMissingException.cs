using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class RequiredPropertyMissingException : ServicePermanentExceptionWithPropertyPath
	{
		public RequiredPropertyMissingException(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorRequiredPropertyMissing, propertyPath)
		{
		}

		public RequiredPropertyMissingException(ResponseCodeType responseCodeType) : base(responseCodeType, CoreResources.IDs.ErrorRequiredPropertyMissing)
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
