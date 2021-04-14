using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class ReportContextFactory : IReportContextFactory
	{
		public Type ReportType { get; set; }

		public string ViewName { get; set; }

		public DataMartType DataMartType { get; set; }

		public IDbConnection CreateConnection(bool createBackupConnection = false)
		{
			string connectionString = DataMart.Instance.GetConnectionString(this.DataMartType, createBackupConnection);
			if (!string.IsNullOrEmpty(connectionString))
			{
				return new SqlConnection(connectionString);
			}
			return null;
		}

		public IReportContext CreateReportContext(IDbConnection connection)
		{
			ReportContext reportContext = new ReportContext(connection);
			reportContext.ChangeViewName(this.ReportType, this.ViewName);
			return reportContext;
		}
	}
}
