using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionInvalidAttributeValue : ServicePermanentExceptionWithPropertyPath
	{
		public CalendarExceptionInvalidAttributeValue(PropertyPath propertyPath) : base((CoreResources.IDs)2961161516U, propertyPath)
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
