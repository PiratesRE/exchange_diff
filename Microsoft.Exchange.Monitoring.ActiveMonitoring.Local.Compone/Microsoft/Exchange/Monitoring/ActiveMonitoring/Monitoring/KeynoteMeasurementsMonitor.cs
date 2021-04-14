using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Monitoring
{
	public class KeynoteMeasurementsMonitor : MonitorWorkItem
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, int failureCount, bool enabled)
		{
			return new MonitorDefinition
			{
				Name = name,
				SampleMask = sampleMask,
				ServiceName = serviceName,
				Component = component,
				Enabled = enabled,
				AssemblyPath = KeynoteMeasurementsMonitor.AssemblyPath,
				TypeName = typeof(KeynoteMeasurementsMonitor).FullName
			};
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			int num = this.ReadAttribute("aggregationLevel", 2);
			int num2 = this.ReadAttribute("minISPCountThreshold", 2);
			double num3 = this.ReadAttribute("minAvailabilityThreshold", 50.0);
			int num4 = this.ReadAttribute("LookBackMinutes", 120);
			this.endpoint = this.ReadAttribute("Endpoint", null);
			string targetResource = base.Definition.TargetResource;
			base.Result.StateAttribute4 = targetResource;
			base.Result.StateAttribute5 = this.endpoint;
			base.Result.StateAttribute6 = (double)num;
			base.Result.StateAttribute7 = num3;
			base.Result.StateAttribute8 = (double)num2;
			base.Result.StateAttribute9 = (double)num4;
			if (string.IsNullOrEmpty(targetResource))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.MonitoringTracer, base.TraceContext, "The target and scope to monitor is not defined.  Existing without performing monitoring.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteMeasurementsMonitor.cs", 93);
				throw new Exception("The target and scope to monitor is not defined.  Please set target for the monitor");
			}
			DataTable failedMeasurementsForKeynote = this.GetFailedMeasurementsForKeynote(targetResource, num, num2, num3, num4);
			if (failedMeasurementsForKeynote != null && failedMeasurementsForKeynote.Rows.Count > 0)
			{
				string text = this.BuildFailureReport(failedMeasurementsForKeynote);
				base.Result.StateAttribute3 = text;
				WTFDiagnostics.TraceError<TracingContext>(ExTraceGlobals.MonitoringTracer, base.TraceContext, string.Format("Keynote measurement probe failure(s) were detected.  Failure detail: {0}", text), base.TraceContext, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteMeasurementsMonitor.cs", 105);
				base.Result.IsAlert = true;
				base.Result.HealthState = ServiceHealthStatus.Unhealthy;
			}
		}

		private DataTable GetFailedMeasurementsForKeynote(string target, int queryAggregationScope, int minIspCount, double minAvailability, int lookbackMinutes)
		{
			DataSet dataSet = new DataSet();
			try
			{
				DateTime executionStartTime = base.Result.ExecutionStartTime;
				DateTime dateTime = executionStartTime.AddMinutes((double)(-(double)lookbackMinutes));
				base.Result.StateAttribute2 = dateTime.ToString();
				base.Result.StateAttribute3 = executionStartTime.ToString();
				using (SqlConnection sqlConnection = new SqlConnection(this.endpoint))
				{
					SqlCommand sqlCommand = new SqlCommand("GetFailedKeynoteMeasurements", sqlConnection);
					sqlCommand.CommandText = "GetFailedKeynoteMeasurements";
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime)).Value = dateTime;
					sqlCommand.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime)).Value = executionStartTime;
					sqlCommand.Parameters.Add(new SqlParameter("@QueryAggregationScope", SqlDbType.Int)).Value = queryAggregationScope;
					sqlCommand.Parameters.Add(new SqlParameter("@MinISPCount", SqlDbType.Int)).Value = minIspCount;
					sqlCommand.Parameters.Add(new SqlParameter("@MinAvailability", SqlDbType.Float)).Value = minAvailability;
					sqlCommand.Parameters.Add(new SqlParameter("@Target", SqlDbType.NVarChar)).Value = target;
					using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
					{
						sqlDataAdapter.Fill(dataSet);
					}
				}
			}
			catch (SqlException innerException)
			{
				throw new Exception(string.Format("Failed to query database: {0}", this.endpoint), innerException);
			}
			if (dataSet.Tables.Count > 0)
			{
				return dataSet.Tables[0];
			}
			return null;
		}

		private string BuildFailureReport(DataTable failureReport)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("The keynote monitor [{0}] failed.  Please see details below for measurement failure details", base.Definition.Name));
			foreach (object obj in failureReport.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				foreach (object obj2 in failureReport.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj2;
					stringBuilder.AppendLine(string.Format("{0}: {1}", dataColumn.ColumnName, dataRow[dataColumn]));
				}
				stringBuilder.AppendLine("---------------------------------------------------------------");
			}
			return stringBuilder.ToString();
		}

		private const string FailedMeasurementFetchStoredProc = "GetFailedKeynoteMeasurements";

		private static string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private string endpoint;
	}
}
