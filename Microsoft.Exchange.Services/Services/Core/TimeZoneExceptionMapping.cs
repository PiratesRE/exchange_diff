using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class TimeZoneExceptionMapping : StaticExceptionMapping
	{
		public TimeZoneExceptionMapping() : base(typeof(TimeZoneException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorTimeZone, CoreResources.IDs.ErrorTimeZone)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			TimeZoneException ex = base.VerifyExceptionType<TimeZoneException>(exception);
			if (ex.ConstantValues.Count != 0)
			{
				return new Dictionary<string, string>(ex.ConstantValues);
			}
			return null;
		}

		public override LocalizedString GetLocalizedMessage(LocalizedException exception)
		{
			return exception.LocalizedString;
		}
	}
}
