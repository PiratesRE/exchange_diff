using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes
{
	public class RwsInstrumentationProbe : ProbeWorkItem
	{
		internal RemotePowerShell RPS
		{
			get
			{
				if (this.remotePowerShell == null)
				{
					this.remotePowerShell = this.CreateRunspace();
				}
				return this.remotePowerShell;
			}
		}

		private string GetSummaryTableStatus(string connectionString, DateTime lastUpdateTimeStamp)
		{
			string result = string.Empty;
			string arg = string.Format(base.Definition.Attributes["TargetServer"], Environment.MachineName.Substring(0, 3));
			string arg2 = base.Definition.Attributes["TargetDatabase"];
			string arg3 = base.Definition.Attributes["TargetSummaryTable"];
			string value = base.Definition.Attributes["TargetReportName"];
			string value2 = base.Definition.Attributes["OutputFieldSeperator"];
			string cmdText = string.Format("SELECT top 1 [DataUnitId]\r\n                                  ,[ServerName]\r\n                                  ,[TableName]\r\n                                  ,[DataTimeStamp]\r\n                                  ,[DataSetTimeStamp]\r\n                                  ,[DataUnitTimeStamp]\r\n                                  ,[PumpStartTimeStamp]\r\n                                  ,[PumpEndTimeStamp]\r\n                                  ,[CreateIndexEndTimeStamp]\r\n                                  ,[DataReadyTimeStamp]\r\n                                  ,[Length]\r\n                                  ,[Path]\r\n                                  ,[Status]\r\n                                  ,[LastUpdateTimeStamp]\r\n                        FROM {0}.{1}\r\n                        WHERE [TableName] = @TargetReportName\r\n                            AND [Status] = @Status\r\n                            AND [LastUpdateTimeStamp] > @LastUpdateTimeStamp\r\n                        ORDER BY [LastUpdateTimeStamp] ASC\r\n                        ", arg2, arg3);
			SqlParameter value3 = new SqlParameter("@TargetReportName", value);
			SqlParameter value4 = new SqlParameter("@Status", "Ready");
			SqlParameter value5 = new SqlParameter("@LastUpdateTimeStamp", lastUpdateTimeStamp);
			using (SqlConnection sqlConnection = new SqlConnection(string.Format(connectionString, arg)))
			{
				using (SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection))
				{
					sqlCommand.Parameters.Add(value3);
					sqlCommand.Parameters.Add(value4);
					sqlCommand.Parameters.Add(value5);
					sqlConnection.Open();
					using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
					{
						if (sqlDataReader.Read())
						{
							this.serverName = (string)sqlDataReader["ServerName"];
							this.dumpingDataUnitId = (long)sqlDataReader["DataUnitId"];
							this.dataTimeStamp = (DateTime)sqlDataReader["DataTimeStamp"];
							this.lastUpdateTimeStamp = (DateTime)sqlDataReader["LastUpdateTimeStamp"];
							int capacity = 256;
							StringBuilder stringBuilder = new StringBuilder(capacity);
							for (int i = 0; i < sqlDataReader.FieldCount; i++)
							{
								if (i != 0)
								{
									stringBuilder.Append(value2);
								}
								stringBuilder.Append(sqlDataReader[i].ToString());
							}
							result = stringBuilder.ToString();
						}
					}
				}
			}
			return result;
		}

		private string GetFileName(string reportName, long dumpingDataUnitId)
		{
			string empty = string.Empty;
			return string.Format("{0}_{1}.log", reportName, dumpingDataUnitId.ToString());
		}

		private DateTime GetFileDateTime(RwsInstrumentationProbe.ReportType reportType, DateTime dataTime, DateTime lastUpdateTimeStamp)
		{
			DateTime result = dataTime;
			DateTime dateTime = lastUpdateTimeStamp.AddDays(-1.0);
			switch (reportType)
			{
			case RwsInstrumentationProbe.ReportType.Weekly:
				if (dataTime > dateTime.Date.AddDays((double)((DayOfWeek)(-1) - dateTime.DayOfWeek)))
				{
					result = dateTime;
				}
				break;
			case RwsInstrumentationProbe.ReportType.Monthly:
				if (dataTime > dateTime.Date.AddDays((double)(-(double)dateTime.Day)))
				{
					result = dateTime;
				}
				break;
			case RwsInstrumentationProbe.ReportType.Yearly:
				if (dataTime > dateTime.Date.AddDays((double)(-(double)dateTime.DayOfYear)))
				{
					result = dateTime;
				}
				break;
			}
			return result;
		}

		private RwsInstrumentationProbe.ReportType GetReportType(string reportName)
		{
			RwsInstrumentationProbe.ReportType result = RwsInstrumentationProbe.ReportType.Daily;
			if (reportName.Contains("Weekly"))
			{
				result = RwsInstrumentationProbe.ReportType.Weekly;
			}
			else if (reportName.Contains("Monthly"))
			{
				result = RwsInstrumentationProbe.ReportType.Monthly;
			}
			else if (reportName.Contains("Yearly"))
			{
				result = RwsInstrumentationProbe.ReportType.Yearly;
			}
			return result;
		}

		private void DumpingSummaryData(string line, string outputPath, DateTime targetDate)
		{
			if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			}
			using (FileStream fileStream = new FileStream(outputPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
			{
				File.SetCreationTimeUtc(outputPath, targetDate);
				using (StreamWriter streamWriter = new StreamWriter(fileStream))
				{
					streamWriter.WriteLine(line);
				}
			}
		}

		private void DumpingDetailData(string outputPath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			}
			string text = string.Format(base.Definition.Attributes["TargetServer"], Environment.MachineName.Substring(0, 3));
			string text2 = base.Definition.Attributes["TargetDatabase"];
			string text3 = base.Definition.Attributes["TargetDetailTable"];
			string fileName = base.Definition.Attributes["ExeFileName"];
			string format = base.Definition.Attributes["ExeArguments"];
			string text4 = base.Definition.Attributes["OutputFieldSeperator"];
			string format2 = "\r\n                        SELECT '{0}' AS [ServerName], [DataUnitId], [TENANTGUID], [COUNT] \r\n                        FROM {1}.{2}\r\n                        WHERE [DataUnitId] = {3};\r\n                        ";
			string text5 = string.Format(format2, new object[]
			{
				this.serverName,
				text2,
				text3,
				this.dumpingDataUnitId
			});
			using (Process process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = string.Format(format, new object[]
					{
						text5,
						outputPath,
						text,
						text4
					}),
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			})
			{
				process.Start();
				long num = -1L;
				string pattern = base.Definition.Attributes["ExeSuccessCopyNumberPattern"];
				Regex regex = new Regex(pattern);
				while (!process.StandardOutput.EndOfStream)
				{
					string input = process.StandardOutput.ReadLine();
					Match match = regex.Match(input);
					if (match.Success && match.Groups[1].Success)
					{
						num = Convert.ToInt64(match.Groups[1].Value);
					}
				}
				if (num < 0L)
				{
					throw new Exception("BCP.exe dumping data error");
				}
			}
		}

		public void SendEmail(string alertSource, string toAddresses, string ccAddresses, string fromAddress, string subject, string body)
		{
			if (string.IsNullOrWhiteSpace(toAddresses))
			{
				throw new ArgumentException("toAddresses");
			}
			if (string.IsNullOrWhiteSpace(ccAddresses))
			{
				throw new ArgumentException("ccAddresses");
			}
			if (string.IsNullOrWhiteSpace(fromAddress))
			{
				throw new ArgumentException("fromAddress");
			}
			if (string.IsNullOrWhiteSpace(subject))
			{
				throw new ArgumentException("subject");
			}
			if (string.IsNullOrWhiteSpace(body))
			{
				throw new ArgumentException("body");
			}
			PSCommand pscommand = new PSCommand();
			bool flag = false;
			pscommand.AddCommand("New-OnCallEmail");
			pscommand.AddParameter("Subject", subject);
			pscommand.AddParameter("Body", body);
			pscommand.AddParameter("Urgent", flag);
			pscommand.AddParameter("ToAddresses", toAddresses);
			pscommand.AddParameter("CCAddresses", ccAddresses);
			pscommand.AddParameter("FromAddress", fromAddress);
			pscommand.AddParameter("Source", alertSource);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(pscommand.Commands[0].CommandText);
			foreach (CommandParameter commandParameter in pscommand.Commands[0].Parameters)
			{
				stringBuilder.AppendFormat(" -{0}:{1}", commandParameter.Name, commandParameter.Value.ToString());
			}
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "RwsInstrumentationProbe: Emailing job status via command '{0}'...", stringBuilder.ToString(), null, "SendEmail", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 474);
			try
			{
				this.RPS.InvokePSCommand(pscommand);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "RwsInstrumentationProbe.SendEmail: Successfully emailed job status '{0}'.", stringBuilder.ToString(), null, "SendEmail", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 478);
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("RwsInstrumentationProbe.SendEmail: Unexpected failure when emailing job status via command '{0}': {1}", stringBuilder.ToString(), ex.ToString()));
			}
		}

		private RemotePowerShell CreateRunspace()
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Setting probe definition account value to {0} Settings.RemotePowershellCertSubject", null, "CreateRunspace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 493);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Setting probe definition endpoint value to {0} Settings.RemotePowershellCertSubject", null, "CreateRunspace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 496);
			RemotePowerShell remotePowerShell;
			if (!string.IsNullOrWhiteSpace(base.Definition.AccountPassword))
			{
				remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(base.Definition.Endpoint), base.Definition.Account, base.Definition.AccountPassword, false);
			}
			else if (base.Definition.Endpoint.Contains(";"))
			{
				remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(base.Definition.Endpoint.Split(new char[]
				{
					';'
				}), base.Definition.Account, false);
				if (remotePowerShell == null)
				{
					remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(base.Definition.Endpoint.Split(new char[]
					{
						';'
					}), base.Definition.Account, true);
				}
			}
			else
			{
				remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(new Uri(base.Definition.Endpoint), base.Definition.Account, false);
			}
			return remotePowerShell;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: start", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 556);
			string path = base.Definition.Attributes["ConfigurationDir"];
			string text = base.Definition.Attributes["TargetReportName"];
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: read configuration from file", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 566);
			int dumpRecoveryDays = Convert.ToInt32(base.Definition.Attributes["DumpRecoveryDays"]);
			string path2 = Path.Combine(path, text) + ".conf";
			RwsInstrumentationProbe.Configuration configuration = new RwsInstrumentationProbe.Configuration();
			configuration.DumpRecoveryDays = dumpRecoveryDays;
			configuration.LoadConfiguration(path2);
			string connectionString = base.Definition.Attributes["SummaryConnectionString"];
			string summaryTableStatus = this.GetSummaryTableStatus(connectionString, configuration.LastUpdateTimeStamp);
			if (!string.IsNullOrEmpty(summaryTableStatus))
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: dumping detail data", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 582);
				string path3 = base.Definition.Attributes["UploaderDetailFileDir"];
				string text2 = this.GetFileName(text, this.dumpingDataUnitId);
				text2 = Path.Combine(path3, text2);
				if (File.Exists(text2))
				{
					throw new Exception(string.Format("{0} is exist.", text2));
				}
				string text3 = string.Format("{0}.{1}", text2, "tmp");
				try
				{
					this.DumpingDetailData(text3);
					RwsInstrumentationProbe.ReportType reportType = this.GetReportType(text);
					File.SetCreationTimeUtc(text3, this.GetFileDateTime(reportType, this.dataTimeStamp, this.lastUpdateTimeStamp));
					File.Move(text3, text2);
				}
				catch (Exception ex)
				{
					if (File.Exists(text3))
					{
						File.Delete(text3);
					}
					throw ex;
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: dumping summary data", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 618);
				string text4 = base.Definition.Attributes["UploaderSummaryFilePath"];
				DateTime targetDate = this.lastUpdateTimeStamp.Date.AddDays(-1.0);
				text4 = text4.Replace(".log", targetDate.ToString("_yyyy_MM_dd") + ".log");
				this.DumpingSummaryData(summaryTableStatus, text4, targetDate);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: successfully complete dumping data", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 633);
				configuration.SaveConfiguration(this.lastUpdateTimeStamp, DateTime.UtcNow, configuration.LastSendEmailTimeStamp);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: save configuration to file", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 643);
			}
			else
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: no table need dump to file", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 652);
				string value = base.Definition.Attributes["CheckDataExistHour"];
				if (configuration.LastDumpSuccessTime < DateTime.Today && configuration.LastSendEmailTimeStamp < DateTime.Today && DateTime.UtcNow.Hour >= Convert.ToInt32(value))
				{
					string alertSource = base.Definition.Attributes["AlertSource"];
					string toAddresses = base.Definition.Attributes["ToAddresses"];
					string ccAddresses = base.Definition.Attributes["CCAddresses"];
					string fromAddress = base.Definition.Attributes["FromAddress"];
					string subject = string.Format(base.Definition.Attributes["AlertSubject"], text);
					string body = string.Format(base.Definition.Attributes["AlertBody"], new object[]
					{
						DateTime.UtcNow.ToString(),
						Environment.MachineName,
						configuration.LastUpdateTimeStamp.ToString(),
						configuration.LastDumpSuccessTime.ToString()
					});
					try
					{
						this.SendEmail(alertSource, toAddresses, ccAddresses, fromAddress, subject, body);
					}
					catch (Exception ex2)
					{
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: Unexpected failure when emailing job status, error is {1}", base.Definition.Name, ex2.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 685);
					}
					configuration.SaveConfiguration(configuration.LastUpdateTimeStamp, configuration.LastDumpSuccessTime, DateTime.UtcNow);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: data ready time-out, and send email to owner", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 697);
				}
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsInstrumentationDiscovery.{0}: end", base.Definition.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsInstrumentationProbe.cs", 705);
		}

		private string serverName = string.Empty;

		private long dumpingDataUnitId = long.MinValue;

		private DateTime dataTimeStamp = DateTime.MinValue;

		private DateTime lastUpdateTimeStamp = DateTime.MinValue;

		private RemotePowerShell remotePowerShell;

		private enum ReportType
		{
			Daily,
			Weekly,
			Monthly,
			Yearly
		}

		private class Configuration
		{
			public int DumpRecoveryDays
			{
				set
				{
					this.dumpRecoveryDays = value;
				}
			}

			public DateTime LastUpdateTimeStamp
			{
				get
				{
					return this.lastUpdateTimeStamp;
				}
			}

			public DateTime LastDumpSuccessTime
			{
				get
				{
					return this.lastDumpSuccessTime;
				}
			}

			public DateTime LastSendEmailTimeStamp
			{
				get
				{
					return this.lastSendEmailTimeStamp;
				}
			}

			public void LoadConfiguration(string path)
			{
				string location = Assembly.GetExecutingAssembly().Location;
				string directoryName = Path.GetDirectoryName(location);
				this.path = Path.Combine(directoryName, path);
				if (File.Exists(this.path))
				{
					StreamReader streamReader = new StreamReader(this.path);
					string text = streamReader.ReadLine();
					streamReader.Close();
					string[] array = text.Split(new char[]
					{
						','
					});
					this.lastUpdateTimeStamp = new DateTime(Convert.ToInt64(array[0].Trim()));
					this.lastDumpSuccessTime = new DateTime(Convert.ToInt64(array[1].Trim()));
					this.lastSendEmailTimeStamp = new DateTime(Convert.ToInt64(array[2].Trim()));
					return;
				}
				if (!Directory.Exists(Path.GetDirectoryName(this.path)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(this.path));
				}
				DateTime date = DateTime.UtcNow.AddDays((double)(-(double)this.dumpRecoveryDays)).Date;
				this.lastUpdateTimeStamp = date;
				this.lastDumpSuccessTime = date;
				this.lastSendEmailTimeStamp = date;
			}

			public void SaveConfiguration(DateTime lastUpdateTimeStamp, DateTime lastDumpSuccessTime, DateTime lastSendEmailTimeStamp)
			{
				this.lastUpdateTimeStamp = lastUpdateTimeStamp;
				this.lastDumpSuccessTime = lastDumpSuccessTime;
				this.lastSendEmailTimeStamp = lastSendEmailTimeStamp;
				using (StreamWriter streamWriter = new StreamWriter(this.path))
				{
					streamWriter.WriteLine(string.Format("{1}{0}{2}{0}{3}", new object[]
					{
						',',
						lastUpdateTimeStamp.Ticks,
						lastDumpSuccessTime.Ticks,
						lastSendEmailTimeStamp.Ticks
					}));
				}
			}

			private const char seperator = ',';

			private string path;

			private int dumpRecoveryDays;

			private DateTime lastUpdateTimeStamp;

			private DateTime lastDumpSuccessTime;

			private DateTime lastSendEmailTimeStamp;
		}
	}
}
