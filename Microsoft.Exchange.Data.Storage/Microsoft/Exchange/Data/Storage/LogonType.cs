using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum LogonType
	{
		Owner,
		Admin,
		Delegated,
		Transport,
		SystemService,
		BestAccess,
		DelegatedAdmin
	}
}
