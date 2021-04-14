using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidPropertyForOperationException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidPropertyForOperationException(PropertyPath propertyPath) : base((CoreResources.IDs)2517173182U, propertyPath)
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
