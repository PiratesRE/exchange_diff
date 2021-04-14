using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionInvalidPropertyValue : ServicePermanentExceptionWithPropertyPath
	{
		public CalendarExceptionInvalidPropertyValue(PropertyPath propertyPath) : base((CoreResources.IDs)3349192959U, propertyPath)
		{
		}

		public CalendarExceptionInvalidPropertyValue(PropertyPath propertyPath, Exception innerException) : base((CoreResources.IDs)3349192959U, propertyPath, innerException)
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
