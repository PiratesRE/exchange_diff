using System;
using System.Web;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal interface IUserTokenParser
	{
		bool TryParseUserToken(HttpContext context, out string userToken);
	}
}
