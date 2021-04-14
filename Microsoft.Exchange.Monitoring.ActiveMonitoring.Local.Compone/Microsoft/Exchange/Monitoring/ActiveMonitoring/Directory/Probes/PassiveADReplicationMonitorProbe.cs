using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class PassiveADReplicationMonitorProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (!propertyBag.ContainsKey("ReplicationThresholdInMins"))
			{
				throw new ArgumentException("Please specify value forReplicationThresholdInMins");
			}
			pDef.Attributes["ReplicationThresholdInMins"] = propertyBag["ReplicationThresholdInMins"].ToString().Trim();
			if (propertyBag.ContainsKey("PercentageOfDCsThresholdExcludedForADHealth"))
			{
				pDef.Attributes["PercentageOfDCsThresholdExcludedForADHealth"] = propertyBag["PercentageOfDCsThresholdExcludedForADHealth"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value forPercentageOfDCsThresholdExcludedForADHealth");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			bool flag = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
			{
				flag = PassiveADReplicationMonitorProbe.GetRegistryBool(registryKey, "DisablePassiveADReplicationMonitorProbe", true);
			}
			base.Result.StateAttribute12 = "Disabled = " + flag.ToString();
			WTFDiagnostics.TraceInformation<bool>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "PassiveADReplicationMonitor is disabled or not: {0}", flag, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 120);
			if (!flag)
			{
				DirectoryUtils.Logger(this, StxLogType.PassiveADReplicationMonitor, delegate
				{
					string arg = new ServerIdParameter().ToString();
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting PassiveADReplicationMonitor on DC: {0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 135);
					AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
					PassiveADReplicationMonitorProbe.replicationThresholdInMins = attributeHelper.GetInt("ReplicationThresholdInMins", true, 0, null, null);
					PassiveADReplicationMonitorProbe.percentageOfDCsThresholdExcludedForADHealth = attributeHelper.GetDouble("PercentageOfDCsThresholdExcludedForADHealth", true, 0.0, null, null);
					List<string> provisionedDCNames = this.GetProvisionedDCNames();
					KeyValuePair<double, string> result = this.CalculateADHealth(provisionedDCNames);
					this.WriteADHealthResult(result);
					if (result.Key > (double)PassiveADReplicationMonitorProbe.replicationThresholdInMins)
					{
						if (!string.IsNullOrWhiteSpace(result.Value))
						{
							if (string.Compare(result.Value, Environment.MachineName, true) == 0)
							{
								base.Result.Error = string.Format("The shortest inbound replication delay of DC '{0}' is {1}, which is over replication threshold {2} in minutes.", result.Value, result.Key, PassiveADReplicationMonitorProbe.replicationThresholdInMins);
								return;
							}
							base.Result.StateAttribute5 = string.Format("The shortest inbound replication delay of DC '{0}' is {1}, which is over replication threshold {2} in minutes.", result.Value, result.Key, PassiveADReplicationMonitorProbe.replicationThresholdInMins);
							return;
						}
						else
						{
							if (string.Compare(this.lastDC, Environment.MachineName, true) == 0)
							{
								base.Result.Error = this.errorMsg;
								return;
							}
							base.Result.StateAttribute5 = this.errorMsg;
						}
					}
				});
			}
		}

		public KeyValuePair<double, string> CalculateADHealth(List<string> list)
		{
			if (list == null || list.Count == 0)
			{
				this.errorMsg = "[PassiveADReplicationMonitorProbe:CalculateADHealth] All DCs are in MM state now!";
				WTFDiagnostics.TraceError(ExTraceGlobals.DirectoryTracer, base.TraceContext, "All DCs are in MM state now！", null, "CalculateADHealth", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 199);
				return new KeyValuePair<double, string>(double.MaxValue, string.Empty);
			}
			List<KeyValuePair<double, string>> listOfShortestReplicationDelay = new List<KeyValuePair<double, string>>(0);
			double key = double.MaxValue;
			string value = string.Empty;
			string domainNCPartitionName = DirectoryGeneralUtils.GetDefaultNC(Environment.MachineName);
			CancellationTokenSource cts = new CancellationTokenSource();
			using (new Timer(delegate(object _)
			{
				cts.Cancel();
			}, null, 120000, -1))
			{
				try
				{
					Parallel.ForEach<string>(list, new ParallelOptions
					{
						CancellationToken = cts.Token
					}, delegate(string item)
					{
						this.GetShortestInboundReplicationDelay(item, domainNCPartitionName, listOfShortestReplicationDelay);
					});
				}
				catch (OperationCanceledException ex)
				{
					base.Result.StateAttribute11 = string.Format("[PassiveADReplicationMonitorProbe:CalculateADHealth] Timed out trying to read shortest replication delay due to the exception: {0}", ex.Message);
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Error when trying to read shortest replication delay from DCs in the forest. This is due to Exception: {0}", ex.Message, null, "CalculateADHealth", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 237);
				}
			}
			base.Result.StateAttribute1 = this.shortestReplicationDelayLinksString.ToString();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Shortest replication delays: {0}", this.shortestReplicationDelayLinksString.ToString(), null, "CalculateADHealth", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 248);
			listOfShortestReplicationDelay.Sort((KeyValuePair<double, string> x, KeyValuePair<double, string> y) => y.Key.CompareTo(x.Key));
			int num = (int)(PassiveADReplicationMonitorProbe.percentageOfDCsThresholdExcludedForADHealth * this.numberOfAllDCs);
			int num2 = num - this.numberOfDCsInMM;
			int num3 = -1;
			if (num2 < 0)
			{
				if (listOfShortestReplicationDelay.Count != 0)
				{
					num3 = 0;
				}
			}
			else if (listOfShortestReplicationDelay.Count >= num2 + 1)
			{
				num3 = num2;
			}
			else
			{
				num3 = list.Count - 1;
			}
			if (num3 != -1)
			{
				key = listOfShortestReplicationDelay[num3].Key;
				value = listOfShortestReplicationDelay[num3].Value;
			}
			return new KeyValuePair<double, string>(key, value);
		}

		private List<string> GetProvisionedDCNames()
		{
			List<string> list = new List<string>(0);
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry())
				{
					if (directoryEntry.Properties.Contains("distinguishedName") && directoryEntry.Properties["distinguishedName"].Value != null)
					{
						using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
						{
							"OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString();
							foreach (object obj in directoryEntry2.Children)
							{
								DirectoryEntry directoryEntry3 = (DirectoryEntry)obj;
								string item = directoryEntry3.Properties["cn"].Value.ToString();
								this.lastDC = item;
								int num;
								if (directoryEntry3.Properties.Contains("msExchProvisioningFlags"))
								{
									num = (int)directoryEntry3.Properties["msExchProvisioningFlags"].Value;
									if (num != 0)
									{
										this.numberOfDCsInMM++;
									}
								}
								else
								{
									num = 31000;
									this.numberOfDCsInMM++;
								}
								if (num == 0)
								{
									list.Add(item);
								}
								this.numberOfAllDCs += 1.0;
								directoryEntry3.Dispose();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute4 = string.Format("[PassiveADReplicationMonitorProbe:GetProvisionedDCNames] Could not get the list of all domain controllers in this forest from the machine {0}. This is due to Exception: {1}", Environment.MachineName, ex.Message);
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Could not get the list of all domain controllers in this forest from the machine {0}. This is due to Exception: {1}", Environment.MachineName, ex.Message, null, "GetProvisionedDCNames", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 353);
			}
			return list;
		}

		private void GetShortestInboundReplicationDelay(string targetDC, string partition, List<KeyValuePair<double, string>> listOfReplicationDelays)
		{
			string domainControllerOUFormatString = DirectoryUtils.GetDomainControllerOUFormatString(targetDC);
			string.Format(domainControllerOUFormatString, targetDC);
			KeyValuePair<double, string> item = default(KeyValuePair<double, string>);
			string text = string.Empty;
			try
			{
				text = DirectoryUtils.GetReplicationXml(targetDC, partition);
				if (string.IsNullOrEmpty(text))
				{
					lock (PassiveADReplicationMonitorProbe.lockForList)
					{
						item = new KeyValuePair<double, string>(0.0, targetDC);
						listOfReplicationDelays.Add(item);
						return;
					}
				}
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute5 = ex.Message;
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Did not successfully get replication xml from the DC '{0}'.  This is due to Exception: {1}", targetDC, ex.Message, null, "GetShortestInboundReplicationDelay", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 401);
				lock (PassiveADReplicationMonitorProbe.lockForList)
				{
					item = new KeyValuePair<double, string>((double)PassiveADReplicationMonitorProbe.replicationThresholdInMins, targetDC);
					listOfReplicationDelays.Add(item);
				}
			}
			string text2 = string.Empty;
			double num = double.MaxValue;
			string arg = string.Empty;
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(text)))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(xmlReader);
				XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("REPL");
				if (elementsByTagName.Item(0).ChildNodes.Count == 0)
				{
					lock (PassiveADReplicationMonitorProbe.lockForList)
					{
						item = new KeyValuePair<double, string>(0.0, targetDC);
						listOfReplicationDelays.Add(item);
					}
				}
				XmlNodeList childNodes = elementsByTagName.Item(0).ChildNodes;
				string value = string.Empty;
				DateTime utcNow = DateTime.UtcNow;
				foreach (object obj4 in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj4;
					text2 = xmlNode["pszSourceDsaDN"].OuterXml;
					text2 = text2.Split(new char[]
					{
						','
					})[1].Substring(3);
					value = xmlNode["ftimeLastSyncSuccess"].InnerText;
					DateTime value2 = Convert.ToDateTime(value);
					double totalMinutes = utcNow.Subtract(value2).TotalMinutes;
					if (totalMinutes < num)
					{
						num = totalMinutes;
						arg = text2;
					}
				}
				this.shortestReplicationDelayLinksString.AppendFormat("{0}, {1}, {2}.  \n", arg, targetDC, num);
				lock (PassiveADReplicationMonitorProbe.lockForList)
				{
					item = new KeyValuePair<double, string>(num, targetDC);
					listOfReplicationDelays.Add(item);
				}
			}
		}

		private void WriteADHealthResult(KeyValuePair<double, string> result)
		{
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry())
				{
					using (new DirectoryEntry("LDAP://OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
					{
						string arg = "OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString();
						string path = string.Format("LDAP://{0}/CN={0},{1}", Environment.MachineName, arg);
						using (DirectoryEntry directoryEntry3 = new DirectoryEntry(path))
						{
							DateTime utcNow = DateTime.UtcNow;
							string text = utcNow.ToFileTimeUtc().ToString();
							string value = string.Concat(new string[]
							{
								result.Key.ToString(),
								"@",
								result.Value,
								"@",
								text
							});
							if (!directoryEntry3.Properties.Contains("msExchExtensionAttribute45"))
							{
								directoryEntry3.Properties["msExchExtensionAttribute45"].Add(value);
							}
							else
							{
								directoryEntry3.Properties["msExchExtensionAttribute45"][0] = value;
							}
							directoryEntry3.CommitChanges();
							base.Result.StateAttribute2 = string.Format("PassiveADReplicationMonitorProbe::WriteADHealthResult: Successfully updated time stamp and AD Health result {0} ({1}): {2} in minutes", text, utcNow.ToString(), result);
						}
					}
				}
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute3 = ex.Message;
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Could not write AD health result successfully to the DC '{0}'.  This is due to Exception: {1}", result.Value, ex.Message, null, "WriteADHealthResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveADReplicationMonitorProbe.cs", 517);
			}
		}

		internal static bool GetRegistryBool(RegistryKey regkey, string key, bool defaultValue)
		{
			int? num = null;
			if (regkey != null)
			{
				num = (regkey.GetValue(key) as int?);
			}
			if (num == null)
			{
				return defaultValue;
			}
			return Convert.ToBoolean(num.Value);
		}

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters";

		private const string DisablePassiveADReplicationMonitorProbeRegKeyName = "DisablePassiveADReplicationMonitorProbe";

		private const bool DefaultDisabled = true;

		private static readonly object lockForList = new object();

		private static int replicationThresholdInMins = -1;

		private static double percentageOfDCsThresholdExcludedForADHealth = -1.0;

		private int numberOfDCsInMM;

		private double numberOfAllDCs;

		private StringBuilder shortestReplicationDelayLinksString = new StringBuilder();

		private string errorMsg = string.Empty;

		private string lastDC = string.Empty;
	}
}
