using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionOutOfRange : ServicePermanentExceptionWithPropertyPath
	{
		public CalendarExceptionOutOfRange(PropertyPath propertyPath) : base((CoreResources.IDs)3773356320U, propertyPath)
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
