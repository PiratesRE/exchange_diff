using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidPropertyRequestException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidPropertyRequestException(PropertyPath propertyPath) : base((CoreResources.IDs)3673396595U, propertyPath)
		{
		}

		public InvalidPropertyRequestException(LocalizedString message, PropertyPath propertyPath) : base(ResponseCodeType.ErrorInvalidPropertyRequest, message, propertyPath)
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
