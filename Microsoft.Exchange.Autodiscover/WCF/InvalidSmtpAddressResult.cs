using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class InvalidSmtpAddressResult : ResultBase
	{
		internal InvalidSmtpAddressResult(UserResultMapping userResultMapping) : base(userResultMapping)
		{
		}

		internal override UserResponse CreateResponse(IBudget budget)
		{
			return base.CreateInvalidUserResponse();
		}
	}
}
