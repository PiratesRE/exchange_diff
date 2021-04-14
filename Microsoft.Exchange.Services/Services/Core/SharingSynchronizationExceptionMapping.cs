using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class SharingSynchronizationExceptionMapping : StaticExceptionMapping
	{
		public SharingSynchronizationExceptionMapping() : base(typeof(SharingSynchronizationException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorSharingSynchronizationFailed, (CoreResources.IDs)3469371317U)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			SharingSynchronizationException ex = base.VerifyExceptionType<SharingSynchronizationException>(exception);
			return new Dictionary<string, string>
			{
				{
					"ErrorDetails",
					ex.ErrorDetails
				}
			};
		}

		private const string ErrorDetails = "ErrorDetails";
	}
}
