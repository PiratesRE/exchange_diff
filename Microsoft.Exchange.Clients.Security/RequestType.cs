using System;

namespace Microsoft.Exchange.Clients.Security
{
	internal enum RequestType
	{
		NotSet,
		Regular,
		EcpByoidAdmin,
		EcpDelegatedAdmin,
		EcpDelegatedAdminTargetForest
	}
}
