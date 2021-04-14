using System;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes
{
	public class RwsDatamartAvailabilityProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			string text = string.Empty;
			try
			{
				text = this.GetConnectionString();
				using (SqlConnection sqlConnection = new SqlConnection(text))
				{
					sqlConnection.Open();
					base.Result.StateAttribute21 = string.Format("Successfully connected to SQL Server {0}, Database {1}.", sqlConnection.DataSource, "CDM-TenantDS");
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("Exception when try to open connection to datamart. Exception: {0}. The connection string is {1}.", ex.Message, text), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartAvailabilityProbe.cs", 69);
				base.Result.StateAttribute21 = text;
				throw;
			}
		}

		private string GetConnectionString()
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
			string connectionString;
			try
			{
				sqlConnectionStringBuilder.DataSource = base.Definition.Endpoint;
				sqlConnectionStringBuilder.InitialCatalog = "CDM-TenantDS";
				sqlConnectionStringBuilder.ConnectTimeout = 30;
				sqlConnectionStringBuilder.IntegratedSecurity = true;
				connectionString = sqlConnectionStringBuilder.ConnectionString;
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("Failed to set the connection string, exception: {0}, ConnectionString: {1}. ", ex.Message, sqlConnectionStringBuilder.ConnectionString), null, "GetConnectionString", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDatamartAvailabilityProbe.cs", 98);
				throw;
			}
			return connectionString;
		}

		private const string ConnectionStringFormat = "Server={0};Database={1};Integrated Security=SSPI;Connection Timeout={2}";

		private const string TenantsDatamartDatabaseName = "CDM-TenantDS";

		private const int TenantsDatamartConnectionTimeoutSeconds = 30;
	}
}
