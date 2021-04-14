using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.Probes
{
	public sealed class ForwardSyncMonopolizedProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, string targetResource, int recurrenceInterval)
		{
			return new ProbeDefinition
			{
				AssemblyPath = ForwardSyncMonopolizedProbe.AssemblyPath,
				TypeName = ForwardSyncMonopolizedProbe.TypeName,
				Name = name,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = recurrenceInterval / 2,
				TargetResource = targetResource
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): starting", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 130);
			this.MeasurementStart = DateTime.UtcNow;
			if (ExEnvironment.IsTest)
			{
				ForwardSyncMonopolizedProbe.HighAvgObjectThrpt = new Dictionary<string, int>(2)
				{
					{
						"User",
						3
					},
					{
						"Group",
						15
					}
				};
				ForwardSyncMonopolizedProbe.MonopolyCheckWindow = new TimeSpan(0, 4, 0);
			}
			this.TimeStartAnalysis = this.MeasurementStart - ForwardSyncMonopolizedProbe.MonopolyCheckWindow;
			try
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): Get ForwardSync arbitration events", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 145);
				string query = string.Format("<QueryList>  <Query Id=\"0\" Path=\"{0}\">    <Select Path=\"{0}\">        *[System[Provider[@Name='{1}'] and        (EventID={2}) and        TimeCreated[timediff(@SystemTime) &lt;= {3}]]]    </Select>  </Query></QueryList>", new object[]
				{
					"ForwardSync",
					"MSExchangeForwardSync",
					5012,
					2400000
				});
				using (EventLogReader eventLogReader = new EventLogReader(new EventLogQuery("ForwardSync", PathType.LogName, query)
				{
					ReverseDirection = true
				}))
				{
					for (;;)
					{
						using (EventRecord eventRecord = eventLogReader.ReadEvent())
						{
							if (eventRecord != null)
							{
								if (eventRecord.Properties.Count >= 3)
								{
									WTFDiagnostics.TraceInformation<DateTime, string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): ForwardSync Arbitration event: Time: {0}, Status: {1}, ServiceInstanceName:{2}", eventRecord.TimeCreated.Value.ToUniversalTime(), eventRecord.Properties[1].Value.ToString(), eventRecord.Properties[2].Value.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 174);
									if (string.Equals(eventRecord.Properties[1].Value.ToString(), "Active", StringComparison.InvariantCultureIgnoreCase))
									{
										string text = eventRecord.Properties[2].Value.ToString();
										ForwardSyncMonopolizedProbe.ServiceInstanceName = text.Replace("_", "/");
										break;
									}
								}
								continue;
							}
						}
						break;
					}
				}
				if (string.IsNullOrEmpty(ForwardSyncMonopolizedProbe.ServiceInstanceName) || ForwardSyncMonopolizedProbe.ServiceInstanceName.EndsWith("2"))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): This server is Passive or not serving BPOS_S", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 196);
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): This server is Active for service instance {0}", ForwardSyncMonopolizedProbe.ServiceInstanceName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 203);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): Get and parse Receive Log for service instance: {0}", ForwardSyncMonopolizedProbe.ServiceInstanceName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 209);
					this.AnalyzeReceiveLogForMonopolizingOrg();
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): Completed Get and parse Receive Log for service instance: {0}", ForwardSyncMonopolizedProbe.ServiceInstanceName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 217);
				}
			}
			finally
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoWork(): ending", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 226);
			}
		}

		private void AnalyzeReceiveLogForMonopolizingOrg()
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): starting", null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 238);
			try
			{
				string text = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\ForwardSyncLogs\\ReceiveLog");
				string[] files = Directory.GetFiles(text, "ForwardSyncReceive*");
				string text2 = null;
				foreach (string text3 in files)
				{
					if (Directory.GetLastWriteTimeUtc(text3) >= this.TimeStartAnalysis)
					{
						text2 = text3;
						WTFDiagnostics.TraceInformation<string, DateTime, DateTime>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Analyzing Receive Log {0}, last written: {1} > TimeStartAnalysis: {2}, less than 20 min ago.Will be parsed.", text2, Directory.GetLastWriteTimeUtc(text3), this.TimeStartAnalysis, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 256);
						break;
					}
					WTFDiagnostics.TraceInformation<string, DateTime, DateTime>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Older Receive Log {0}, last written: {1} < TimeStartAnalysis: {2}, more than 20 min ago. Won't be parsed.", text2, Directory.GetLastWriteTimeUtc(text3), this.TimeStartAnalysis, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 264);
				}
				if (text2 == null)
				{
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Couldn't find a Receive log in: {0}, written in the last 20 min.", text, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 273);
				}
				else
				{
					Dictionary<string, int> dictionary2;
					Dictionary<string, List<int>> dictionary = this.DoParseReceiveLog(text2, out dictionary2);
					WTFDiagnostics.TraceInformation<double, double>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Thresholds:  MonopolyCheckWindow {0} (min), HighPercentage: {1}", ForwardSyncMonopolizedProbe.MonopolyCheckWindow.TotalMinutes, ForwardSyncMonopolizedProbe.HighPercentage, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 289);
					foreach (string text4 in ForwardSyncMonopolizedProbe.HighAvgObjectThrpt.Keys)
					{
						WTFDiagnostics.TraceInformation<string, int, double>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Thresholds:  ObjectType: {0}, AvgThroughput (obj/min): {1}, HighObjectCount in 20 min: {2}", text4, ForwardSyncMonopolizedProbe.HighAvgObjectThrpt[text4], (double)ForwardSyncMonopolizedProbe.HighAvgObjectThrpt[text4] * ForwardSyncMonopolizedProbe.MonopolyCheckWindow.TotalMinutes, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 297);
					}
					WTFDiagnostics.TraceInformation<int>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Result Of Parse ReceivLogs Summary row count: {0}", dictionary.Count, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 304);
					foreach (string text5 in dictionary.Keys)
					{
						string[] array2 = text5.Split(new char[]
						{
							'~'
						});
						string text6 = array2[0];
						string text7 = array2[1];
						WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): tenantObjectType: {0}, tenantID: {1}, objectType: {2}", text5, text6, text7, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 315);
						double num = (double)ForwardSyncMonopolizedProbe.HighAvgObjectThrpt[text7] * ForwardSyncMonopolizedProbe.MonopolyCheckWindow.TotalMinutes;
						if ((double)dictionary[text5][0] >= num && (double)dictionary[text5][1] >= ForwardSyncMonopolizedProbe.HighPercentage)
						{
							WTFDiagnostics.TraceWarning<string, string, int, int>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Tenant: {0}, ObjectType: {1}, Objects in 20 min: {2}, Percentage: {3}", text6, text7, dictionary[text5][0], dictionary[text5][1], null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 324);
							WTFDiagnostics.TraceWarning<int, double, int, double>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): IS MONOPOLIZING. Objects in 20 min: {0} >= {1} and Percentage: {2} >= {3}", dictionary[text5][0], num, dictionary[text5][1], ForwardSyncMonopolizedProbe.HighPercentage, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 329);
							string message = string.Format("Tenant: {0} IS MONOPOLIZING the DeltaSync stream, ObjectType: {1}, Objects count in 20 min: {2}, Percentage: {3}", new object[]
							{
								text6,
								text7,
								dictionary[text5][0],
								dictionary[text5][1]
							});
							base.Result.StateAttribute1 = text6;
							base.Result.StateAttribute2 = text7;
							base.Result.StateAttribute3 = ForwardSyncMonopolizedProbe.ServiceInstanceName;
							base.Result.StateAttribute6 = num;
							base.Result.StateAttribute7 = ForwardSyncMonopolizedProbe.HighPercentage;
							base.Result.StateAttribute8 = (double)dictionary[text5][0];
							base.Result.StateAttribute9 = (double)dictionary[text5][1];
							WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, message, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 348);
							throw new Exception(message);
						}
						WTFDiagnostics.TraceInformation<string, string, int, int>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Tenant: {0}, ObjectType: {1}, Objects in 20 min: {2}, Percentage: {3}", text6, text7, dictionary[text5][0], dictionary[text5][1], null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 356);
						WTFDiagnostics.TraceInformation<int, double, int, double>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Is NOT monopolizing. Objects in 20 min: {0} < {1} or Percentage: {2} < {3}", dictionary[text5][0], num, dictionary[text5][1], ForwardSyncMonopolizedProbe.HighPercentage, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 361);
						base.Result.StateAttribute1 = text6;
						base.Result.StateAttribute2 = text7;
						base.Result.StateAttribute3 = ForwardSyncMonopolizedProbe.ServiceInstanceName;
						base.Result.StateAttribute6 = num;
						base.Result.StateAttribute7 = ForwardSyncMonopolizedProbe.HighPercentage;
						base.Result.StateAttribute8 = (double)dictionary[text5][0];
						base.Result.StateAttribute9 = (double)dictionary[text5][1];
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string, Exception>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): Exception: {0}, {1}", ex.Message, ex, null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 379);
				throw;
			}
			finally
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: AnalyzeReceiveLogForMonopolizingOrg(): ending", null, "AnalyzeReceiveLogForMonopolizingOrg", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 387);
			}
		}

		private Dictionary<string, List<int>> DoParseReceiveLog(string recLog, out Dictionary<string, int> totalObjectsForType)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoParseReceiveLog(): starting", null, "DoParseReceiveLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 402);
			Dictionary<string, List<int>> dictionary = new Dictionary<string, List<int>>();
			totalObjectsForType = new Dictionary<string, int>();
			totalObjectsForType.Add("User", 0);
			totalObjectsForType.Add("Group", 0);
			try
			{
				using (FileStream fileStream = new FileStream(recLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (StreamReader streamReader = new StreamReader(fileStream))
					{
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							string[] array = text.Split(new char[]
							{
								','
							});
							DateTime dateTime;
							if (array != null && array.Length >= 21 && string.IsNullOrEmpty(array[5]) && DateTime.TryParse(array[0], out dateTime))
							{
								dateTime = dateTime.ToUniversalTime();
								if (dateTime >= this.TimeStartAnalysis && array[4] == "GetChangesRequest")
								{
									WTFDiagnostics.TraceInformation<DateTime, string, string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoParseReceiveLog(): dt: {0}, gc: {1}, r19: {3}, r17: {2}", dateTime, array[4], array[17], array[19], null, "DoParseReceiveLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 435);
									if (array[19] == "User" || array[19] == "Contact")
									{
										string key = string.Format("{0}~{1}", array[18], "User");
										if (dictionary.ContainsKey(key))
										{
											List<int> list;
											(list = dictionary[key])[0] = list[0] + 1;
										}
										else
										{
											List<int> value = new List<int>(2)
											{
												1,
												0
											};
											dictionary.Add(key, value);
										}
										Dictionary<string, int> dictionary2;
										(dictionary2 = totalObjectsForType)["User"] = dictionary2["User"] + 1;
									}
									else if (array[19] == "Group" || array[22] == "LinkObject")
									{
										string key2 = string.Format("{0}~{1}", array[18], "Group");
										if (dictionary.ContainsKey(key2))
										{
											List<int> list2;
											(list2 = dictionary[key2])[0] = list2[0] + 1;
										}
										else
										{
											List<int> value2 = new List<int>(2)
											{
												1,
												0
											};
											dictionary.Add(key2, value2);
										}
										Dictionary<string, int> dictionary3;
										(dictionary3 = totalObjectsForType)["Group"] = dictionary3["Group"] + 1;
									}
									WTFDiagnostics.TraceInformation<DateTime, int, int>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoParseReceiveLog(): dt: {0}, Users: {1}, Groups: {2}", dateTime, totalObjectsForType["User"], totalObjectsForType["Group"], null, "DoParseReceiveLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 474);
								}
							}
						}
					}
				}
				WTFDiagnostics.TraceInformation<int, int>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoParseReceiveLog(): ReceiveLogs Summary for last 20 minutes, Users and Contacts: {0}, Groups and Links: {1}", totalObjectsForType["User"], totalObjectsForType["Group"], null, "DoParseReceiveLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 484);
				foreach (string text2 in dictionary.Keys)
				{
					if (text2.EndsWith("User"))
					{
						dictionary[text2][1] = Convert.ToInt32(dictionary[text2][0] * 100 / totalObjectsForType["User"]);
					}
					else
					{
						dictionary[text2][1] = Convert.ToInt32(dictionary[text2][0] * 100 / totalObjectsForType["Group"]);
					}
					WTFDiagnostics.TraceInformation<string, int, int>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoParseReceiveLog(): Tenant~ObjectType: {0}, Objects in 20 min: {1}, Percentage: {2}", text2, dictionary[text2][0], dictionary[text2][1], null, "DoParseReceiveLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 502);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string, Exception>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoParseReceiveLog(): Exception reading file: {0}, {1}", ex.Message, ex, null, "DoParseReceiveLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 510);
				throw;
			}
			finally
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncMonopolizedProbe:: DoParseReceiveLog(): ending", null, "DoParseReceiveLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\MonopolizingTenantProbe.cs", 518);
			}
			return dictionary;
		}

		private const string LogName = "ForwardSync";

		private const string EventSource = "MSExchangeForwardSync";

		private const int EventId = 5012;

		private const int EventIntervalMilliSeconds = 2400000;

		private const string QueryStringFormat = "<QueryList>  <Query Id=\"0\" Path=\"{0}\">    <Select Path=\"{0}\">        *[System[Provider[@Name='{1}'] and        (EventID={2}) and        TimeCreated[timediff(@SystemTime) &lt;= {3}]]]    </Select>  </Query></QueryList>";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ForwardSyncMonopolizedProbe).FullName;

		private static TimeSpan MonopolyCheckWindow = new TimeSpan(0, 20, 0);

		private static Dictionary<string, int> HighAvgObjectThrpt = new Dictionary<string, int>(2)
		{
			{
				"User",
				300
			},
			{
				"Group",
				1500
			}
		};

		private static readonly double HighPercentage = 50.0;

		private DateTime MeasurementStart = DateTime.UtcNow;

		private DateTime TimeStartAnalysis;

		private static string ServiceInstanceName = string.Empty;
	}
}
