using System;
using System.Data;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal interface IReportContextFactory
	{
		IDbConnection CreateConnection(bool createBackupConnection = false);

		IReportContext CreateReportContext(IDbConnection connection);
	}
}
