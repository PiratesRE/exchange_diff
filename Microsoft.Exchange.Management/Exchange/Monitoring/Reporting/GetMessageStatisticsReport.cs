using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	[Cmdlet("Get", "MessageStatisticsReport", DefaultParameterSetName = "ReportingPeriod")]
	public sealed class GetMessageStatisticsReport : TransportReportingTask
	{
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			base.ValidateSProcExists("[Exchange2010].[TransportReporting_GetMessageStatistics]");
			TaskLogger.LogExit();
		}

		protected override void WriteStatistics(ADObjectId tenantId)
		{
			TaskLogger.LogEnter();
			SqlDataReader sqlDataReader = null;
			List<MessageStatisticsReport> list = new List<MessageStatisticsReport>(1);
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(base.GetSqlConnectionString()))
				{
					sqlConnection.Open();
					SqlCommand sqlCommand = new SqlCommand("[Exchange2010].[TransportReporting_GetMessageStatistics]", sqlConnection);
					sqlCommand.CommandType = CommandType.StoredProcedure;
					if (tenantId != null && tenantId.ObjectGuid != Guid.Empty)
					{
						sqlCommand.Parameters.Add("@TenantGuid", SqlDbType.UniqueIdentifier).Value = tenantId.ObjectGuid;
					}
					else if (base.Fields.IsModified("AdSite"))
					{
						sqlCommand.Parameters.Add("@AdSite", SqlDbType.NVarChar).Value = base.AdSite.ToString();
					}
					sqlCommand.Parameters.Add("@DailyStatistics", SqlDbType.TinyInt).Value = (base.DailyStatistics ? 1 : 0);
					if (base.ParameterSetName == "ReportingPeriod")
					{
						if (!base.Fields.IsModified("ReportingPeriod"))
						{
							base.ReportingPeriod = ReportingPeriod.LastMonth;
						}
						ExDateTime startDate;
						ExDateTime endDate;
						Utils.GetStartEndDateForReportingPeriod(base.ReportingPeriod, out startDate, out endDate);
						base.StartDate = startDate;
						base.EndDate = endDate;
					}
					sqlCommand.Parameters.Add("@IntervalStartDateTime", SqlDbType.DateTime).Value = (DateTime)base.StartDate;
					sqlCommand.Parameters.Add("@IntervalEndDateTime", SqlDbType.DateTime).Value = (DateTime)base.EndDate;
					sqlDataReader = sqlCommand.ExecuteReader();
					while (sqlDataReader.Read())
					{
						MessageStatisticsReport messageStatisticsReport = new MessageStatisticsReport();
						if (tenantId != null)
						{
							messageStatisticsReport.Identity = tenantId;
						}
						if (base.DailyStatistics)
						{
							DateTime dateTime = (DateTime)sqlDataReader["AggregatedDateTime"];
							if (new ExDateTime(ExTimeZone.UtcTimeZone, dateTime) < base.StartDate)
							{
								base.WriteError(new ArgumentException(Strings.InvalidAggregatedDateTime), (ErrorCategory)1002, null);
								return;
							}
							messageStatisticsReport.StartDate = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime.Date);
							messageStatisticsReport.EndDate = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime.AddDays(1.0).Subtract(TimeSpan.FromMinutes(1.0)));
						}
						else
						{
							messageStatisticsReport.StartDate = base.StartDate;
							messageStatisticsReport.EndDate = base.EndDate;
						}
						messageStatisticsReport.TotalMessagesSent = (DBNull.Value.Equals(sqlDataReader["TotalMessagesSent"]) ? 0 : ((int)sqlDataReader["TotalMessagesSent"]));
						messageStatisticsReport.TotalMessagesReceived = (DBNull.Value.Equals(sqlDataReader["TotalMessagesReceived"]) ? 0 : ((int)sqlDataReader["TotalMessagesReceived"]));
						messageStatisticsReport.TotalMessagesSentToForeign = (DBNull.Value.Equals(sqlDataReader["TotalMessagesSentToForeign"]) ? 0 : ((int)sqlDataReader["TotalMessagesSentToForeign"]));
						messageStatisticsReport.TotalMessagesReceivedFromForeign = (DBNull.Value.Equals(sqlDataReader["TotalMessagesReceivedFromForeign"]) ? 0 : ((int)sqlDataReader["TotalMessagesReceivedFromForeign"]));
						list.Add(messageStatisticsReport);
					}
				}
			}
			catch (SqlException ex)
			{
				if (!Datacenter.IsMultiTenancyEnabled() && ex.Number == 53)
				{
					base.WriteError(new InvalidOperationException(Strings.ScomMayNotBeInstalled(ex.Message)), (ErrorCategory)1001, null);
				}
				else
				{
					base.WriteError(ex, (ErrorCategory)1002, null);
				}
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
			}
			if (base.DailyStatistics)
			{
				IEnumerable<StartEndDateTimePair> allDaysInGivenRange = Utils.GetAllDaysInGivenRange(base.StartDate, base.EndDate);
				int num = 0;
				using (IEnumerator<StartEndDateTimePair> enumerator = allDaysInGivenRange.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StartEndDateTimePair startEndDateTimePair = enumerator.Current;
						if (num < list.Count && startEndDateTimePair.StartDate.Date == list[num].StartDate.Date)
						{
							base.WriteResult(list[num]);
							num++;
						}
						else
						{
							MessageStatisticsReport messageStatisticsReport2 = new MessageStatisticsReport();
							messageStatisticsReport2.StartDate = startEndDateTimePair.StartDate;
							messageStatisticsReport2.EndDate = startEndDateTimePair.EndDate;
							if (tenantId != null)
							{
								messageStatisticsReport2.Identity = tenantId;
							}
							base.WriteResult(messageStatisticsReport2);
						}
					}
					goto IL_491;
				}
			}
			foreach (MessageStatisticsReport dataObject in list)
			{
				base.WriteResult(dataObject);
			}
			IL_491:
			TaskLogger.LogExit();
		}

		private const string CmdletNoun = "MessageStatisticsReport";

		private const string SPName = "[Exchange2010].[TransportReporting_GetMessageStatistics]";

		private const ReportingPeriod DefualtReportingPeriod = ReportingPeriod.LastMonth;
	}
}
