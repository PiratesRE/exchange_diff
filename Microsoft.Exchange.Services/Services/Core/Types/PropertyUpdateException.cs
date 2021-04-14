using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class PropertyUpdateException : ServicePermanentExceptionWithPropertyPath
	{
		public PropertyUpdateException(PropertyPath[] properties, Exception innerException) : base(CoreResources.IDs.ErrorPropertyUpdate, properties, innerException)
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
