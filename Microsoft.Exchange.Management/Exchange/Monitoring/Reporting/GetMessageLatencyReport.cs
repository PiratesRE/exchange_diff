using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	[Cmdlet("Get", "MessageLatencyReport", DefaultParameterSetName = "ReportingPeriod")]
	public sealed class GetMessageLatencyReport : TransportReportingTask
	{
		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan SlaTargetTimespan
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["SlaTargetTimespan"];
			}
			set
			{
				base.Fields["SlaTargetTimespan"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			base.ValidateSProcExists("[Exchange2010].[TransportReporting_GetMessageLatency]");
			if (base.Fields.IsModified("SlaTargetTimespan") && (this.SlaTargetTimespan > EnhancedTimeSpan.FromMinutes(5.0) || this.SlaTargetTimespan < EnhancedTimeSpan.FromSeconds(1.0)))
			{
				base.WriteError(new ArgumentException(Strings.OutOfRangeSlaTaget(EnhancedTimeSpan.FromSeconds(1.0).ToString(), EnhancedTimeSpan.FromMinutes(5.0).ToString())), (ErrorCategory)1000, null);
			}
			TaskLogger.LogExit();
		}

		protected override void WriteStatistics(ADObjectId tenantId)
		{
			TaskLogger.LogEnter();
			SqlDataReader sqlDataReader = null;
			List<MessageLatencyReport> list = new List<MessageLatencyReport>(1);
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(base.GetSqlConnectionString()))
				{
					sqlConnection.Open();
					SqlCommand sqlCommand = new SqlCommand("[Exchange2010].[TransportReporting_GetMessageLatency]", sqlConnection);
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
					sqlCommand.Parameters.Add("@Target", SqlDbType.SmallInt).Value = this.GetSlaTargetTimeSpanInSeconds();
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
						MessageLatencyReport messageLatencyReport = new MessageLatencyReport();
						if (tenantId != null)
						{
							messageLatencyReport.Identity = tenantId;
						}
						messageLatencyReport.SlaTargetInSeconds = this.GetSlaTargetTimeSpanInSeconds();
						if (base.DailyStatistics)
						{
							DateTime dateTime = (DateTime)sqlDataReader["AggregatedDateTime"];
							if (new ExDateTime(ExTimeZone.UtcTimeZone, dateTime).Date < base.StartDate)
							{
								base.WriteError(new ArgumentException(Strings.InvalidAggregatedDateTime), (ErrorCategory)1002, null);
							}
							messageLatencyReport.StartDate = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime.Date);
							messageLatencyReport.EndDate = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime.AddDays(1.0).Subtract(TimeSpan.FromMinutes(1.0)));
						}
						else
						{
							messageLatencyReport.StartDate = base.StartDate;
							messageLatencyReport.EndDate = base.EndDate;
						}
						if (DBNull.Value.Equals(sqlDataReader["PercentageOfMessagesMeetingTarget"]) || DBNull.Value.Equals(sqlDataReader["PercentageOfMessagesMeetingTarget"]))
						{
							messageLatencyReport.PercentOfMessageInGivenSla = 100m;
						}
						else
						{
							messageLatencyReport.PercentOfMessageInGivenSla = (decimal)sqlDataReader["PercentageOfMessagesMeetingTarget"];
						}
						list.Add(messageLatencyReport);
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
							MessageLatencyReport messageLatencyReport2 = new MessageLatencyReport();
							messageLatencyReport2.StartDate = startEndDateTimePair.StartDate;
							messageLatencyReport2.EndDate = startEndDateTimePair.EndDate;
							messageLatencyReport2.PercentOfMessageInGivenSla = 100m;
							if (tenantId != null)
							{
								messageLatencyReport2.Identity = tenantId;
							}
							messageLatencyReport2.SlaTargetInSeconds = this.GetSlaTargetTimeSpanInSeconds();
							base.WriteResult(messageLatencyReport2);
						}
					}
					goto IL_470;
				}
			}
			foreach (MessageLatencyReport dataObject in list)
			{
				base.WriteResult(dataObject);
			}
			IL_470:
			TaskLogger.LogExit();
		}

		private short GetSlaTargetTimeSpanInSeconds()
		{
			if (base.Fields.IsModified("SlaTargetTimespan"))
			{
				return (short)this.SlaTargetTimespan.TotalSeconds;
			}
			return (short)GetMessageLatencyReport.DefaultSlaTargetTimeSpan.TotalSeconds;
		}

		private const string CmdletNoun = "MessageLatencyReport";

		private const string SPName = "[Exchange2010].[TransportReporting_GetMessageLatency]";

		private const ReportingPeriod DefualtReportingPeriod = ReportingPeriod.LastMonth;

		private static readonly EnhancedTimeSpan DefaultSlaTargetTimeSpan = EnhancedTimeSpan.FromMinutes(1.0) + EnhancedTimeSpan.FromSeconds(30.0);
	}
}
