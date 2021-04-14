using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface INativeMethodProvider
	{
		string GetSiteName(string server);

		uint GetAccessCheck(byte[] ntsd, string listString);

		uint GetAccessCheck(string ntsdString, string listString);

		bool TokenMembershipCheck(string sid);

		bool IsCoreServer();
	}
}
