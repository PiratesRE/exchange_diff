using System;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Net.XropService
{
	internal interface IAuthorizationManager
	{
		bool CheckAccess(TokenValidationResults validationResults);
	}
}
