using System;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Webstore.Probes
{
	public class WebstoreReplicationLatencyProbe : ProbeWorkItem
	{
		private int LatencyThresholdInSeconds { get; set; }

		private string WebstoreServer { get; set; }

		public static ProbeDefinition CreateDefinition(string webstoreServer, int latencyThresholdInSeconds)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = typeof(WebstoreReplicationLatencyProbe).Assembly.Location;
			probeDefinition.TypeName = typeof(WebstoreReplicationLatencyProbe).FullName;
			probeDefinition.Name = typeof(WebstoreReplicationLatencyProbe).Name;
			probeDefinition.ServiceName = ExchangeComponent.FfoWebstore.Name;
			probeDefinition.RecurrenceIntervalSeconds = 900;
			probeDefinition.TimeoutSeconds = 300;
			probeDefinition.MaxRetryAttempts = 2;
			probeDefinition.TargetResource = Environment.MachineName;
			probeDefinition.Attributes["WebstoreServer"] = webstoreServer;
			probeDefinition.Attributes["LatencyThresholdInSeconds"] = latencyThresholdInSeconds.ToString();
			return probeDefinition;
		}

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			this.WebstoreServer = attributeHelper.GetString("WebstoreServer", false, ".");
			this.LatencyThresholdInSeconds = attributeHelper.GetInt("LatencyThresholdInSeconds", true, 60, null, null);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("WebstoreReplicationLatencyProbe started at {0}.\r\n", DateTime.UtcNow);
			this.InitializeAttributes(null);
			SqlConnection sqlConnection = new SqlConnection();
			using (sqlConnection)
			{
				sqlConnection.ConnectionString = string.Format("Server={0};Database=WstConfig;Integrated Security=SSPI", this.WebstoreServer);
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("Connecting to WstConfig.  ConnectionString:\"{0}\"\r\n", sqlConnection.ConnectionString);
				sqlConnection.Open();
				string cmdText = "\r\n                                    SELECT \r\n                                       TOP 20 \r\n                                        SourceServer.ServerName + ':' + SourceDb.SQLDatabaseName AS SourceDB, \r\n                                        DestServer.ServerName   + ':' + DestDb.SQLDatabaseName   AS DestinationDB, \r\n                                        l.Latency                                                AS LatencyInSeconds, \r\n                                        l.LatencyBucket                                          AS LatencyBucket, \r\n                                        l.LastUpdate                                             AS LatencyRecordLastUpdate, \r\n                                        DATEDIFF(MINUTE,LastUpdate,GETDATE())                    AS LatencyRecordAgeInMinutes \r\n                                    FROM SQLFSSLatency l \r\n                                        INNER JOIN SQLDatabase SourceDb     ON SourceDb.SQLDatabaseUID    = l.SourceSQLDBUID \r\n                                        INNER JOIN SQLDatabase DestDb       ON DestDb.SQLDatabaseUID      = l.DestSQLDBUID \r\n                                        INNER JOIN DataServer  SourceServer ON SourceServer.DataServerUID = SourceDb.DataServerUID \r\n                                        INNER JOIN DataServer  DestServer   ON DestServer.DataServerUID   = DestDb.DataServerUID \r\n                                    WHERE Latency >= @LatencyThresholdInSeconds OR Latency = -1 \r\n                                       OR DATEDIFF(SECOND,LastUpdate,GETDATE()) > @LatencyThresholdInSeconds \r\n                                    ORDER BY Latency DESC";
				using (SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection))
				{
					sqlCommand.Parameters.Add(new SqlParameter("@LatencyThresholdInSeconds", this.LatencyThresholdInSeconds));
					ProbeResult result2 = base.Result;
					result2.ExecutionContext += string.Format("About to query latency data from WstConfig.  LatencyThresholdInSeconds:{0}\r\n", this.LatencyThresholdInSeconds);
					using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
					{
						bool hasRows = sqlDataReader.HasRows;
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine(WebstoreReplicationLatencyProbe.ProbeErrorMessage);
						while (sqlDataReader.Read())
						{
							object[] array = new object[sqlDataReader.FieldCount];
							int values = sqlDataReader.GetValues(array);
							for (int i = 0; i < values; i++)
							{
								stringBuilder.AppendFormat("{0}: {1}", sqlDataReader.GetName(i), array[i]);
								stringBuilder.AppendLine();
							}
							stringBuilder.AppendLine();
						}
						if (hasRows)
						{
							base.Result.FailureContext = stringBuilder.ToString();
							base.Result.Error = WebstoreReplicationLatencyProbe.ProbeErrorMessage;
							throw new Exception(WebstoreReplicationLatencyProbe.ProbeErrorMessage);
						}
						ProbeResult result3 = base.Result;
						result3.ExecutionContext += "No replication latency issues were found.\r\n";
					}
				}
			}
			ProbeResult result4 = base.Result;
			result4.ExecutionContext += string.Format("WebstoreReplicationLatencyProbe finished at {0}.", DateTime.UtcNow);
		}

		public static readonly string ProbeErrorMessage = "Webstore replication latencies are not within configured thresholds, uninitialized, or stale.";

		internal static class AttributeNames
		{
			internal const string WebstoreServer = "WebstoreServer";

			internal const string LatencyThresholdInSeconds = "LatencyThresholdInSeconds";
		}

		internal static class DefaultValues
		{
			internal const string WebstoreServer = ".";

			internal const int LatencyThresholdInSeconds = 60;
		}
	}
}
