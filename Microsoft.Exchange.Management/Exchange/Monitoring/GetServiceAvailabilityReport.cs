using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Get", "ServiceAvailabilityReport", DefaultParameterSetName = "StartEndDateSet")]
	public sealed class GetServiceAvailabilityReport : GetAvailabilityReport<OrganizationIdParameter>
	{
		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			base.ValidateSProcExists("[Exchange2010].[SLAReportTenantDataGetByTenantGuid]");
			TaskLogger.LogExit();
		}

		protected override void GetTenantAvailabilityReport(ADObjectId tenantId)
		{
			SqlDataReader sqlDataReader = null;
			string text = (tenantId != null) ? tenantId.Name : "<all>";
			base.TraceInfo("Getting service availability report for tenant: {0}", new object[]
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
					SqlCommand sqlCommand = new SqlCommand("[Exchange2010].[SLAReportTenantDataGetByTenantGuid]", sqlConnection);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = (DateTime)this.utcStartDateTime;
					sqlCommand.Parameters.Add("@EndTime", SqlDbType.DateTime).Value = (DateTime)this.utcEndDateTime;
					if (tenantId != null)
					{
						sqlCommand.Parameters.Add("@TenantGuid", SqlDbType.UniqueIdentifier).Value = tenantId.ObjectGuid;
					}
					base.TraceInfo("Executing stored procedure: {0}", new object[]
					{
						"[Exchange2010].[SLAReportTenantDataGetByTenantGuid]"
					});
					sqlDataReader = sqlCommand.ExecuteReader();
					base.TraceInfo("Processing service availability data for tenant: {0}", new object[]
					{
						text
					});
					this.ProcessServiceAvailabilityReportData(tenantId, sqlDataReader);
					base.TraceInfo("Finished processing service availability data for tenant: {0}", new object[]
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

		private void ProcessServiceAvailabilityReportData(ADObjectId tenantId, SqlDataReader reader)
		{
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			while (reader.Read())
			{
				string tenantNameFromReportingDB = (string)reader["TenantName"];
				Guid guid = (Guid)reader["TenantGuid"];
				bool flag = (int)reader["IsOrganizationConfig"] == 1;
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, ((DateTime)reader["UtcDate"]).Date);
				double availabilityPercentage = Convert.ToDouble(reader["UpTime"]);
				ServiceAvailabilityReport serviceAvailabilityReport;
				if (hashtable.Contains(guid))
				{
					serviceAvailabilityReport = (ServiceAvailabilityReport)hashtable[guid];
				}
				else
				{
					serviceAvailabilityReport = new ServiceAvailabilityReport();
					ADObjectId adobjectId;
					if (flag)
					{
						adobjectId = base.RootOrgContainerId;
					}
					else
					{
						adobjectId = base.ResolveTenantIdentity(tenantNameFromReportingDB, guid, tenantId, ref hashtable2);
					}
					if (adobjectId == null)
					{
						continue;
					}
					serviceAvailabilityReport.Identity = adobjectId;
					hashtable.Add(guid, serviceAvailabilityReport);
				}
				DailyServiceAvailability dailyServiceAvailability = new DailyServiceAvailability((DateTime)exDateTime);
				dailyServiceAvailability.AvailabilityPercentage = availabilityPercentage;
				if (!serviceAvailabilityReport.DailyStatistics.Contains(dailyServiceAvailability))
				{
					serviceAvailabilityReport.DailyStatistics.Add(dailyServiceAvailability);
				}
			}
			foreach (object obj in hashtable.Values)
			{
				ServiceAvailabilityReport serviceAvailabilityReport2 = (ServiceAvailabilityReport)obj;
				serviceAvailabilityReport2.StartDate = (DateTime)this.utcStartDateTime;
				serviceAvailabilityReport2.EndDate = (DateTime)this.utcEndDateTime;
				serviceAvailabilityReport2.DailyStatistics.Sort();
				double num = 0.0;
				int count = serviceAvailabilityReport2.DailyStatistics.Count;
				foreach (DailyAvailability dailyAvailability in serviceAvailabilityReport2.DailyStatistics)
				{
					DailyServiceAvailability dailyServiceAvailability2 = (DailyServiceAvailability)dailyAvailability;
					num += dailyServiceAvailability2.AvailabilityPercentage;
				}
				num /= (double)count;
				serviceAvailabilityReport2.AvailabilityPercentage = num;
				if (!base.DailyStatistics.IsPresent)
				{
					serviceAvailabilityReport2.DailyStatistics = null;
				}
				this.WriteReport(serviceAvailabilityReport2);
			}
		}

		private const string CmdletNoun = "ServiceAvailabilityReport";

		private const string SPName = "[Exchange2010].[SLAReportTenantDataGetByTenantGuid]";
	}
}
