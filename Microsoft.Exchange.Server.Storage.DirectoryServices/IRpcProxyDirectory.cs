using System;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public interface IRpcProxyDirectory
	{
		SecurityDescriptor GetDatabaseSecurityDescriptor(IExecutionContext context, Guid databaseGuid);

		SecurityDescriptor GetServerSecurityDescriptor(IExecutionContext context);

		void RefreshDatabaseInfo(IExecutionContext context, Guid databaseGuid);

		void RefreshServerInfo(IExecutionContext context);

		int? GetMaximumRpcThreadCount(IExecutionContext context);

		DatabaseInfo GetDatabaseInfo(IExecutionContext context, Guid databaseGuid);
	}
}
