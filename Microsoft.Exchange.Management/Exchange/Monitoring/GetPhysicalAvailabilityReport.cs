using System;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Get", "PhysicalAvailabilityReport", DefaultParameterSetName = "StartEndDateSet")]
	public sealed class GetPhysicalAvailabilityReport : GetAvailabilityReport<OrganizationIdParameter>
	{
		[Parameter(ParameterSetName = "ReportingPeriodSet", Mandatory = false, Position = 0, ValueFromPipeline = true)]
		[Parameter(ParameterSetName = "StartEndDateSet", Mandatory = false, Position = 0, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(ParameterSetName = "StartEndDateSet", Mandatory = false, Position = 0, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "ReportingPeriodSet", Mandatory = false, Position = 0, ValueFromPipeline = true)]
		public ServerIdParameter ExchangeServer
		{
			get
			{
				return (ServerIdParameter)base.Fields["ExchangeServer"];
			}
			set
			{
				base.Fields["ExchangeServer"] = value;
			}
		}

		private new OrganizationIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			base.ValidateSProcExists("[Exchange2010].[SLAReportDataGetByDatabaseOrExchangeServer]");
			if (this.Database != null && this.ExchangeServer == null)
			{
				this.database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())));
			}
			else if (this.ExchangeServer != null && this.Database == null)
			{
				this.exchangeServer = (Server)base.GetDataObject<Server>(this.ExchangeServer, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.ExchangeServer.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.ExchangeServer.ToString())));
			}
			else if (this.ExchangeServer == null && this.Database == null)
			{
				this.database = null;
				this.exchangeServer = null;
			}
			else
			{
				base.WriteError(new ArgumentException(Strings.AmbiguousDatabaseAndExchangeServerParameters), (ErrorCategory)1000, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.database = null;
			this.exchangeServer = null;
		}

		protected override void GetTenantAvailabilityReport(ADObjectId tenantId)
		{
			if (!base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				base.WriteError(new NotSupportedException(Strings.TenantExecutionNotSupported), (ErrorCategory)1000, null);
			}
			SqlDataReader sqlDataReader = null;
			string text = (tenantId != null) ? tenantId.Name : "<all>";
			base.TraceInfo("Getting physical availability report for tenant: {0}", new object[]
			{
				text
			});
			try
			{
				string sqlConnectionString = base.GetSqlConnectionString();
				using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
				{
					base.TraceInfo("Opening SQL connecttion: {0}", new object[]
					{
						sqlConnectionString
					});
					sqlConnection.Open();
					SqlCommand sqlCommand = new SqlCommand("[Exchange2010].[SLAReportDataGetByDatabaseOrExchangeServer]", sqlConnection);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = (DateTime)this.utcStartDateTime;
					sqlCommand.Parameters.Add("@EndTime", SqlDbType.DateTime).Value = (DateTime)this.utcEndDateTime;
					if (this.database != null && this.exchangeServer == null)
					{
						sqlCommand.Parameters.Add("@MdbName", SqlDbType.NVarChar).Value = this.database.Name;
					}
					else if (this.exchangeServer != null && this.database == null)
					{
						sqlCommand.Parameters.Add("@ExchangeServerFqdn", SqlDbType.NVarChar).Value = this.exchangeServer.Fqdn;
						sqlCommand.Parameters.Add("@ADSite", SqlDbType.NVarChar).Value = this.exchangeServer.ServerSite.Name;
					}
					base.TraceInfo("Executing stored procedure: {0}", new object[]
					{
						"[Exchange2010].[SLAReportDataGetByDatabaseOrExchangeServer]"
					});
					sqlDataReader = sqlCommand.ExecuteReader();
					base.TraceInfo("Processing physical availability data for tenant: {0}", new object[]
					{
						text
					});
					this.ProcessPhysicalAvailabilityReportData(sqlDataReader);
					base.TraceInfo("Finished processing physical availability data for tenant: {0}", new object[]
					{
						text
					});
				}
			}
			catch (SqlException ex)
			{
				base.WriteError(new SqlReportingConnectionException(ex.Message), (ErrorCategory)1000, null);
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
			}
		}

		private void ProcessPhysicalAvailabilityReportData(SqlDataReader reader)
		{
			PhysicalAvailabilityReport physicalAvailabilityReport = new PhysicalAvailabilityReport();
			while (reader.Read())
			{
				string siteName = (string)reader["SiteName"];
				string text = (string)reader["Name"];
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, ((DateTime)reader["UtcDate"]).Date);
				double availabilityPercentage = Convert.ToDouble(reader["UpTime"]);
				double rawAvailabilityPercentage = Convert.ToDouble(reader["RawUpTime"]);
				physicalAvailabilityReport.SiteName = siteName;
				DailyPhysicalAvailability dailyPhysicalAvailability = new DailyPhysicalAvailability((DateTime)exDateTime);
				dailyPhysicalAvailability.AvailabilityPercentage = availabilityPercentage;
				dailyPhysicalAvailability.RawAvailabilityPercentage = rawAvailabilityPercentage;
				if (!physicalAvailabilityReport.DailyStatistics.Contains(dailyPhysicalAvailability))
				{
					physicalAvailabilityReport.DailyStatistics.Add(dailyPhysicalAvailability);
				}
			}
			if (physicalAvailabilityReport.DailyStatistics.Count > 0)
			{
				physicalAvailabilityReport.StartDate = (DateTime)this.utcStartDateTime;
				physicalAvailabilityReport.EndDate = (DateTime)this.utcEndDateTime;
				physicalAvailabilityReport.Database = ((this.database != null) ? this.database.Id : null);
				physicalAvailabilityReport.ExchangeServer = ((this.exchangeServer != null) ? this.exchangeServer.Id : null);
				physicalAvailabilityReport.DailyStatistics.Sort();
				double num = 0.0;
				double num2 = 0.0;
				int count = physicalAvailabilityReport.DailyStatistics.Count;
				foreach (DailyAvailability dailyAvailability in physicalAvailabilityReport.DailyStatistics)
				{
					DailyPhysicalAvailability dailyPhysicalAvailability2 = (DailyPhysicalAvailability)dailyAvailability;
					num += dailyPhysicalAvailability2.AvailabilityPercentage;
					num2 += dailyPhysicalAvailability2.RawAvailabilityPercentage;
				}
				num /= (double)count;
				num2 /= (double)count;
				physicalAvailabilityReport.AvailabilityPercentage = num;
				physicalAvailabilityReport.RawAvailabilityPercentage = num2;
				if (!base.DailyStatistics.IsPresent)
				{
					physicalAvailabilityReport.DailyStatistics = null;
				}
				this.WriteReport(physicalAvailabilityReport);
			}
		}

		private const string CmdletNoun = "PhysicalAvailabilityReport";

		private const string SPName = "[Exchange2010].[SLAReportDataGetByDatabaseOrExchangeServer]";

		private MailboxDatabase database;

		private Server exchangeServer;
	}
}
