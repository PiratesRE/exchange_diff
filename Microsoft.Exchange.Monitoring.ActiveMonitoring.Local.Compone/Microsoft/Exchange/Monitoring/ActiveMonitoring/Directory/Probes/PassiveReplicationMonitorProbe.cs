using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class PassiveReplicationMonitorProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey("ReplicationThresholdInMins"))
			{
				pDef.Attributes["ReplicationThresholdInMins"] = propertyBag["ReplicationThresholdInMins"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value forReplicationThresholdInMins");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.PassiveReplicationMonitor, delegate
			{
				this.DomainControllerName = new ServerIdParameter().ToString();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting PassiveReplicationMonitorProbe on DC: {0}", this.DomainControllerName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveReplicationMonitorProbe.cs", 98);
				string empty = string.Empty;
				string defaultNC = DirectoryGeneralUtils.GetDefaultNC(Environment.MachineName);
				string partition = string.Format("CN=Configuration,{0}", defaultNC);
				bool flag = false;
				string text = string.Empty;
				try
				{
					this.CheckReplicationForPartition(Environment.MachineName, defaultNC, "");
				}
				catch (Exception ex)
				{
					flag = true;
					text = string.Format("Passive replication probe failed for Domain NC partition: \n\n{0}. StackTrace: {1}.\n\n", ex.Message, ex.StackTrace);
				}
				try
				{
					this.CheckReplicationForPartition(Environment.MachineName, partition, "");
				}
				catch (Exception ex2)
				{
					flag = true;
					text = string.Format("{0}Passive replication probe failed for Config NC partition: \n\n{1}.  StackTrace: {2}", text, ex2.Message, ex2.StackTrace);
				}
				if (flag)
				{
					throw new Exception(text);
				}
			});
		}

		private PassiveReplicationMonitorProbe.ReplicationResult CheckReplicationForPartition(string targetDC, string partition, string linkCheckDomainController = "")
		{
			if (!string.IsNullOrEmpty(linkCheckDomainController))
			{
				base.Result.StateAttribute3 = string.Format("Calling replication check on a remote DC: {0}", targetDC);
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			List<string> list = new List<string>();
			"LDAP://" + targetDC + "/" + partition;
			string domainControllerOUFormatString = DirectoryUtils.GetDomainControllerOUFormatString(targetDC);
			string path = string.Format(domainControllerOUFormatString, targetDC);
			if (!base.Definition.Attributes.ContainsKey("ReplicationThresholdInMins"))
			{
				throw new ArgumentException("ReplicationThresholdInMins");
			}
			int num = int.Parse(base.Definition.Attributes["ReplicationThresholdInMins"]);
			string text = string.Empty;
			string domainControllerSite;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
				{
					domainControllerSite = DirectoryUtils.GetDomainControllerSite(directoryEntry);
				}
				text = DirectoryUtils.GetReplicationXml(targetDC, partition);
				if (string.IsNullOrEmpty(text))
				{
					return PassiveReplicationMonitorProbe.ReplicationResult.GREEN;
				}
			}
			catch (Exception ex)
			{
				if (string.IsNullOrEmpty(linkCheckDomainController))
				{
					throw new Exception(string.Format("Could not make an LDAP connection to the Domain Controller.  Got exception {0}", ex.ToString()));
				}
				if (string.IsNullOrEmpty(base.Result.StateAttribute4))
				{
					base.Result.StateAttribute4 = string.Format("Found the following Remote DCs down:  {0} ", targetDC);
				}
				else
				{
					ProbeResult result = base.Result;
					result.StateAttribute4 += string.Format("{0} ", targetDC);
				}
				return PassiveReplicationMonitorProbe.ReplicationResult.GREEN;
			}
			bool flag = true;
			string text2 = string.Empty;
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(text)))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(xmlReader);
				XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("REPL");
				if (elementsByTagName.Item(0).ChildNodes.Count == 0)
				{
					return PassiveReplicationMonitorProbe.ReplicationResult.GREEN;
				}
				XmlNodeList childNodes = elementsByTagName.Item(0).ChildNodes;
				string value = string.Empty;
				DateTime utcNow = DateTime.UtcNow;
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string text3 = xmlNode["pszSourceDsaDN"].OuterXml;
					text3 = text3.Split(new char[]
					{
						','
					})[1].Substring(3);
					value = xmlNode["ftimeLastSyncSuccess"].InnerText;
					DateTime value2 = Convert.ToDateTime(value);
					double totalMinutes = utcNow.Subtract(value2).TotalMinutes;
					string replicationSpan = this.GetReplicationSpan(domainControllerSite, text3);
					stringBuilder2.AppendFormat("{0},{1},{2}\n", text3, replicationSpan, totalMinutes);
					if (value2.Year > 2000)
					{
						if (totalMinutes > (double)num)
						{
							stringBuilder.AppendFormat("{0},{1},{2}\n", text3, replicationSpan, totalMinutes);
							list.Add(text3);
						}
						else
						{
							if (string.Compare(linkCheckDomainController, text3) == 0)
							{
								return PassiveReplicationMonitorProbe.ReplicationResult.REMOTE_LINKS_CHECK_ALERT;
							}
							flag = false;
						}
					}
				}
			}
			base.Result.StateAttribute1 = string.Format("All links for the Domain Controller:\n\nSourceDC,ReplMode,SyncDeltaInMinutes\n{0}", stringBuilder2);
			if (!string.IsNullOrEmpty(linkCheckDomainController))
			{
				if (flag)
				{
					return PassiveReplicationMonitorProbe.ReplicationResult.REMOTE_LINKS_CHECK_GREEN;
				}
				return PassiveReplicationMonitorProbe.ReplicationResult.REMOTE_LINKS_CHECK_RED;
			}
			else
			{
				if (flag)
				{
					text2 = string.Format("All replication links for partition {0} are slow: \n\nSourceDC,ReplMode,SyncDeltaInMinutes\n{1}", partition, stringBuilder);
					base.Result.StateAttribute2 = text2;
					throw new Exception(text2);
				}
				if (list.Count != 0)
				{
					foreach (string text4 in list)
					{
						PassiveReplicationMonitorProbe.ReplicationResult replicationResult = this.CheckReplicationForPartition(text4, partition, Environment.MachineName);
						if (replicationResult != PassiveReplicationMonitorProbe.ReplicationResult.REMOTE_LINKS_CHECK_GREEN)
						{
							if (replicationResult == PassiveReplicationMonitorProbe.ReplicationResult.REMOTE_LINKS_CHECK_ALERT)
							{
								text2 = string.Format("Some of the replication links for partition {0} are slow: \n\nSourceDC,ReplMode,SyncDeltaInMinutes\n{1}", partition, stringBuilder);
								base.Result.StateAttribute2 = text2;
								throw new Exception(text2);
							}
							if (replicationResult == PassiveReplicationMonitorProbe.ReplicationResult.REMOTE_LINKS_CHECK_RED && string.Compare(Environment.MachineName, text4, true) < 0)
							{
								text2 = string.Format("Some of the replication links for partition {0} are slow: \n\nSourceDC,ReplMode,SyncDeltaInMinutes\n{1}", partition, stringBuilder);
								base.Result.StateAttribute2 = text2;
								throw new Exception(text2);
							}
						}
					}
				}
				return PassiveReplicationMonitorProbe.ReplicationResult.GREEN;
			}
			PassiveReplicationMonitorProbe.ReplicationResult result2;
			return result2;
		}

		private string GetReplicationSpan(string currentDCSiteName, string remoteDCName)
		{
			string result = "InterSite";
			try
			{
				string text = DirectoryUtils.GetDomainControllerOUFormatString(Environment.MachineName);
				text = string.Format(text, remoteDCName);
				string domainControllerSite;
				using (DirectoryEntry directoryEntry = new DirectoryEntry(text))
				{
					domainControllerSite = DirectoryUtils.GetDomainControllerSite(directoryEntry);
				}
				if (string.Compare(currentDCSiteName, domainControllerSite, true) == 0)
				{
					result = "IntraSite";
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Could not determine replication span from current site to remote DC: {0}.  Will consider it as Intersite.  This is due to Exception: {1}", remoteDCName, ex.Message, null, "GetReplicationSpan", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveReplicationMonitorProbe.cs", 370);
			}
			return result;
		}

		public string DomainControllerName = string.Empty;

		public enum ReplicationResult
		{
			GREEN,
			REMOTE_LINKS_CHECK_GREEN,
			REMOTE_LINKS_CHECK_RED,
			REMOTE_LINKS_CHECK_ALERT
		}
	}
}
