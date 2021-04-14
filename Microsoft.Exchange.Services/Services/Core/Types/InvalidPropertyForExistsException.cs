using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidPropertyForExistsException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidPropertyForExistsException(PropertyPath propertyPath) : base((CoreResources.IDs)3788524313U, propertyPath)
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
