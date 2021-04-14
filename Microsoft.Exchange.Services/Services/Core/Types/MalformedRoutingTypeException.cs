using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class MalformedRoutingTypeException : ServicePermanentExceptionWithPropertyPath
	{
		public MalformedRoutingTypeException(PropertyPath propertyPath, Exception innerException) : base((CoreResources.IDs)4103342537U, propertyPath, innerException)
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
