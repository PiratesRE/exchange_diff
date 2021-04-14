using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class SyntheticReplicationMonitorProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.SyntheticReplicationMonitor, delegate
			{
				if (!DirectoryUtils.IsRidMaster())
				{
					base.Result.StateAttribute5 = "This DC is not a RID master.  Probe will be skipped.";
					return;
				}
				string arg = new ServerIdParameter().ToString();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting SyntheticReplicationMonitorProbe on DC: {0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationMonitorProbe.cs", 80);
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				new StringBuilder();
				DateTime dateTime = DateTime.UtcNow.AddMinutes(-30.0);
				DateTime t = dateTime;
				DateTime t2 = this.staleDataLimitDate;
				DateTime domainControllerLastUpdateTime = this.staleDataLimitDate;
				string text = string.Empty;
				bool flag = false;
				try
				{
					using (DirectoryEntry directoryEntry = new DirectoryEntry())
					{
						if (directoryEntry.Properties.Contains("distinguishedName") && directoryEntry.Properties["distinguishedName"].Value != null)
						{
							using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
							{
								string text2 = "OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString();
								string path = string.Format("LDAP://{0}/CN={0},{1}", Environment.MachineName, text2);
								using (DirectoryEntry directoryEntry3 = new DirectoryEntry(path))
								{
									if (directoryEntry3.Properties.Contains("adminDescription"))
									{
										t2 = DateTime.FromFileTimeUtc(Convert.ToInt64(directoryEntry3.Properties["adminDescription"].Value.ToString()));
										t = t2.AddMinutes(-30.0);
									}
								}
								foreach (object obj in directoryEntry2.Children)
								{
									DirectoryEntry directoryEntry4 = (DirectoryEntry)obj;
									try
									{
										text = directoryEntry4.Properties["cn"].Value.ToString();
										if (directoryEntry4.Properties["cn"].Value.ToString() != Environment.MachineName)
										{
											WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Processing for DC: {0}", text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationMonitorProbe.cs", 135);
											DateTime dateTime2 = this.staleDataLimitDate;
											if (directoryEntry4.Properties.Contains("adminDescription"))
											{
												dateTime2 = DateTime.FromFileTimeUtc(Convert.ToInt64(directoryEntry4.Properties["adminDescription"].Value.ToString()));
											}
											string domainControllerSite = DirectoryUtils.GetDomainControllerSite(directoryEntry4);
											string text3 = dateTime2.ToString("yyyy-MM-ddTHH:mm:ss");
											int num = 0;
											int num2 = -1;
											if (directoryEntry4.Properties.Contains("msExchProvisioningFlags"))
											{
												num = (int)directoryEntry4.Properties["msExchProvisioningFlags"].Value;
												num2 = num;
											}
											double num3 = Math.Round(DateTime.UtcNow.Subtract(dateTime2).TotalMinutes, 2);
											if (dateTime2 > this.staleDataLimitDate && dateTime > dateTime2)
											{
												if (num == 0)
												{
													flag = true;
												}
												stringBuilder.AppendFormat("{0}, {1}, {2}, {3}, IN, {4}\n", new object[]
												{
													text,
													Environment.MachineName,
													text3,
													num3.ToString(),
													num
												});
											}
											bool flag2 = this.IsOutboundCheckRequiredForSite(domainControllerSite, directoryEntry4);
											if (flag2 && t2 > this.staleDataLimitDate && num2 == 0)
											{
												WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Checking for outbound link in site: {0}", domainControllerSite, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationMonitorProbe.cs", 184);
												this.outboundSitesChecked.Add(domainControllerSite);
												domainControllerLastUpdateTime = this.GetDomainControllerLastUpdateTime(text, Environment.MachineName, text2);
												if (domainControllerLastUpdateTime > this.staleDataLimitDate)
												{
													num3 = Math.Round(DateTime.UtcNow.Subtract(domainControllerLastUpdateTime).TotalMinutes, 2);
													if (t > domainControllerLastUpdateTime)
													{
														flag = true;
														stringBuilder2.AppendFormat("{0}, {1}, {2}, {3}, OUT, 0\n", new object[]
														{
															Environment.MachineName,
															text,
															domainControllerLastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
															num3
														});
													}
												}
											}
										}
									}
									catch (Exception ex)
									{
										string text4 = string.Format("Got an unexpected exception when processing DC {0}.\n{1}\n<<< End of Exception >>>\n\n", text, ex.Message);
										WTFDiagnostics.TraceWarning<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, text4, text, ex.Message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationMonitorProbe.cs", 212);
										ProbeResult result = base.Result;
										result.StateAttribute3 += text4;
									}
								}
							}
						}
					}
				}
				catch (Exception ex2)
				{
					string text5 = string.Format("Got an unexpected exception when running synthetic replication monitor probe. \n{0}\n<<< End of Exception >>>\n\n", ex2.Message);
					WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, text5, ex2.Message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationMonitorProbe.cs", 228);
					ProbeResult result2 = base.Result;
					result2.StateAttribute3 += text5;
				}
				if (stringBuilder.Length > 0 || stringBuilder2.Length > 0)
				{
					base.Result.Error = "Based on the synthetic replication monitor, RID Master has not received updates from following Domain controllers for more than 30 minutes: \n\n";
					ProbeResult result3 = base.Result;
					result3.Error += "SourceDC, TargetDC, LastUpdated, DeltaInMinutes, AtRIDMaster, MMFlag\n";
					ProbeResult result4 = base.Result;
					result4.Error += stringBuilder.ToString();
					ProbeResult result5 = base.Result;
					result5.Error += stringBuilder2.ToString();
					base.Result.StateAttribute2 = base.Result.Error;
				}
				WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", !flag, base.Result.SampleValue, base.Result.StateAttribute2, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SyntheticReplicationMonitorProbe.cs", 245);
				if (flag)
				{
					throw new Exception(base.Result.Error);
				}
				base.Result.StateAttribute1 = "SyntheticReplicationMonitorProbe: No replication issues found in the forest.";
			});
		}

		private DateTime GetDomainControllerLastUpdateTime(string remoteDcName, string dcName, string ouString)
		{
			string path = string.Format("LDAP://{0}/CN={1},{2}", remoteDcName, Environment.MachineName, ouString);
			DateTime result = this.staleDataLimitDate;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
				{
					if (directoryEntry != null)
					{
						result = DateTime.FromFileTimeUtc(Convert.ToInt64(directoryEntry.Properties["adminDescription"].Value.ToString()));
					}
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		private bool IsOutboundCheckRequiredForSite(string siteName, DirectoryEntry dcEntry)
		{
			bool result = true;
			string item = dcEntry.Properties["cn"].Value.ToString();
			if (this.outboundSitesChecked.Contains(item))
			{
				result = false;
			}
			return result;
		}

		public const int StaleDataLimitInDays = 32;

		public const int MaxLatencyInMinutes = 30;

		private readonly DateTime staleDataLimitDate = DateTime.UtcNow.AddDays(-32.0);

		private List<string> outboundSitesChecked = new List<string>();
	}
}
