using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class PrincipalNotAllowedByPolicyExceptionMapping : StaticExceptionMapping
	{
		public PrincipalNotAllowedByPolicyExceptionMapping() : base(typeof(PrincipalNotAllowedByPolicyException), ExchangeVersion.Exchange2010, ResponseCodeType.ErrorUserNotAllowedByPolicy, CoreResources.IDs.ErrorUserNotAllowedByPolicy)
		{
		}

		protected override IDictionary<string, string> GetConstantValues(LocalizedException localizedException)
		{
			PrincipalNotAllowedByPolicyException ex = base.VerifyExceptionType<PrincipalNotAllowedByPolicyException>(localizedException);
			return new Dictionary<string, string>
			{
				{
					"UserId",
					ex.Principal.ToString()
				}
			};
		}

		private const string UserId = "UserId";
	}
}
