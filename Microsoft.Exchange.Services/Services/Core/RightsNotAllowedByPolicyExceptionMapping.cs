using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class RightsNotAllowedByPolicyExceptionMapping : StaticExceptionMapping
	{
		public RightsNotAllowedByPolicyExceptionMapping() : base(typeof(RightsNotAllowedByPolicyException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorPermissionNotAllowedByPolicy, CoreResources.IDs.ErrorPermissionNotAllowedByPolicy)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException localizedException)
		{
			RightsNotAllowedByPolicyException ex = base.VerifyExceptionType<RightsNotAllowedByPolicyException>(localizedException);
			return new Dictionary<string, string>
			{
				{
					"UserId",
					ex.RightsNotAllowedRecipients[0].Principal.ToString()
				}
			};
		}

		private const string UserId = "UserId";
	}
}
