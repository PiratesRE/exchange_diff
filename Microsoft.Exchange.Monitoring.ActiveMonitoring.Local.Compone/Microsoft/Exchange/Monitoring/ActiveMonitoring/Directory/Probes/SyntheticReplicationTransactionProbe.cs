using System;
using System.DirectoryServices;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class SyntheticReplicationTransactionProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.SyntheticReplicationTransaction, delegate
			{
				string arg = new ServerIdParameter().ToString();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting SyntheticReplicationTransactionProbe on DC: {0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationTransactionProbe.cs", 59);
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				string path = string.Empty;
				string arg2 = string.Empty;
				DateTime utcNow = DateTime.UtcNow;
				string text = string.Empty;
				string text2 = utcNow.ToString("yyyy-MM-ddTHH:mm:ss");
				string text3 = string.Empty;
				string text4 = string.Empty;
				try
				{
					using (DirectoryEntry directoryEntry = new DirectoryEntry())
					{
						using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
						{
							arg2 = "OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString();
							path = string.Format("LDAP://{0}/CN={0},{1}", Environment.MachineName, arg2);
							using (DirectoryEntry directoryEntry3 = new DirectoryEntry(path))
							{
								string domainControllerSite = DirectoryUtils.GetDomainControllerSite(directoryEntry3);
								if (directoryEntry3.Properties.Contains("adminDescription"))
								{
									DateTime.FromFileTimeUtc(Convert.ToInt64(directoryEntry3.Properties["adminDescription"].Value.ToString()));
								}
								int num = 0;
								if (directoryEntry3.Properties.Contains("msExchProvisioningFlags"))
								{
									num = (int)directoryEntry3.Properties["msExchProvisioningFlags"].Value;
								}
								foreach (object obj in directoryEntry2.Children)
								{
									DirectoryEntry directoryEntry4 = (DirectoryEntry)obj;
									try
									{
										if (directoryEntry4.Name != directoryEntry3.Name)
										{
											text3 = directoryEntry4.Properties["cn"].Value.ToString();
											DateTime value;
											if (directoryEntry4.Properties.Contains("adminDescription"))
											{
												value = DateTime.FromFileTimeUtc(Convert.ToInt64(directoryEntry4.Properties["adminDescription"].Value.ToString()));
											}
											else
											{
												value = this.staleDataLimitDate;
											}
											double num2 = Math.Round(DateTime.UtcNow.Subtract(value).TotalMinutes, 2);
											string domainControllerSite2 = DirectoryUtils.GetDomainControllerSite(directoryEntry4);
											DateTime dateTime = (DateTime)directoryEntry4.Properties["whenChanged"].Value;
											text4 = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
											double num3 = num2;
											if (num2 < 5.0)
											{
												num3 = Math.Round(dateTime.Subtract(value).TotalMinutes, 2);
											}
											if (num3 < 48000.0)
											{
												stringBuilder2.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} \n", new object[]
												{
													text3,
													domainControllerSite2,
													text2,
													Environment.MachineName,
													domainControllerSite,
													text4,
													num3,
													num
												});
											}
										}
									}
									catch (Exception ex)
									{
										WTFDiagnostics.TraceWarning<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Got exception for DC: {0}.  \n{1}", arg, ex.Message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationTransactionProbe.cs", 150);
									}
								}
								if (!string.IsNullOrEmpty(stringBuilder2.ToString()))
								{
									this.LogReplicationDataToFile(stringBuilder2.ToString());
								}
								utcNow = DateTime.UtcNow;
								text = utcNow.ToFileTimeUtc().ToString();
								if (!directoryEntry3.Properties.Contains("adminDescription"))
								{
									directoryEntry3.Properties["adminDescription"].Add(text);
								}
								else
								{
									directoryEntry3.Properties["adminDescription"][0] = text;
								}
								directoryEntry3.CommitChanges();
							}
						}
					}
					stringBuilder.AppendFormat("SyntheticReplicationTransactionProbe::DoWork: Successfully updated time stamp {0} ({1}) for DC {2}", text, utcNow.ToString(), Environment.MachineName);
				}
				catch (Exception ex2)
				{
					flag = true;
					stringBuilder.Append(ex2.Message);
					base.Result.Error = stringBuilder.ToString();
				}
				base.Result.StateAttribute1 = stringBuilder.ToString();
				WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", !flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationTransactionProbe.cs", 192);
				if (flag)
				{
					throw new Exception(stringBuilder.ToString());
				}
			});
		}

		private bool CheckFreeSpaceAvailable()
		{
			bool result = true;
			DriveInfo[] drives = DriveInfo.GetDrives();
			try
			{
				DriveInfo[] array = drives;
				int i = 0;
				while (i < array.Length)
				{
					DriveInfo driveInfo = array[i];
					if (driveInfo.Name.StartsWith("D:"))
					{
						long availableFreeSpace = driveInfo.AvailableFreeSpace;
						long totalSize = driveInfo.TotalSize;
						long num = 100L * availableFreeSpace / totalSize;
						if (num > 1L)
						{
							result = true;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Got exception when checking for free space.  \n{0}", ex.Message, null, "CheckFreeSpaceAvailable", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationTransactionProbe.cs", 243);
			}
			return result;
		}

		private void LogReplicationDataToFile(string logString)
		{
			try
			{
				if (!string.IsNullOrEmpty(logString))
				{
					string text = string.Empty;
					StringBuilder stringBuilder = new StringBuilder();
					if (Directory.Exists("D:"))
					{
						text = Path.Combine("D:", "Exchange", "Logs");
						if (!Directory.Exists(text))
						{
							Directory.CreateDirectory(text);
						}
						if (!Directory.Exists(text))
						{
							WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Could not create the folder {0}.  Continuing withoug logging.", text, null, "LogReplicationDataToFile", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationTransactionProbe.cs", 278);
						}
						else
						{
							this.CleanUpDataOlderThanDays(text, 3);
							stringBuilder.AppendFormat("{0}\\ADReplication_{1}-1.log", text, DateTime.UtcNow.ToString("yyyyMMdd"));
							if (!File.Exists(stringBuilder.ToString()))
							{
								logString = "SourceDC, SourceSite, LogTime, TargetDC, TargetSite, TargetWhenChanged, DeltaInMinutes, TargetDcInMMFlag\n" + logString;
							}
							new FileInfo(stringBuilder.ToString());
							using (StreamWriter streamWriter = File.AppendText(stringBuilder.ToString()))
							{
								streamWriter.Write(logString);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Got exception when logging the replication latency information.  Continuing withoug logging.  \n{0}", ex.Message, null, "LogReplicationDataToFile", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationTransactionProbe.cs", 307);
			}
		}

		private void CleanUpDataOlderThanDays(string folderPathStr, int numberOfDays = 3)
		{
			if (!string.IsNullOrEmpty(folderPathStr) && Directory.Exists(folderPathStr))
			{
				string[] files = Directory.GetFiles(folderPathStr, "*.log");
				foreach (string fileName in files)
				{
					FileInfo fileInfo = new FileInfo(fileName);
					if (fileInfo.LastWriteTimeUtc < DateTime.UtcNow.AddDays((double)(-(double)numberOfDays)))
					{
						fileInfo.Delete();
					}
				}
			}
		}

		public const int StaleDataLimitInDays = 32;

		private const string LogFileHeaderString = "SourceDC, SourceSite, LogTime, TargetDC, TargetSite, TargetWhenChanged, DeltaInMinutes, TargetDcInMMFlag\n";

		private const string LogDrive = "D:";

		private readonly DateTime staleDataLimitDate = DateTime.UtcNow.AddDays(-32.0);
	}
}
