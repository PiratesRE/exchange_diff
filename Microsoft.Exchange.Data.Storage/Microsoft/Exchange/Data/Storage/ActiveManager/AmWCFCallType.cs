using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum AmWCFCallType
	{
		LocalServer,
		RemoteServerSameDomainSameSite,
		RemoteServerSameDomainDifferentSite,
		RemoteServerDifferentDomain,
		Unknown
	}
}
