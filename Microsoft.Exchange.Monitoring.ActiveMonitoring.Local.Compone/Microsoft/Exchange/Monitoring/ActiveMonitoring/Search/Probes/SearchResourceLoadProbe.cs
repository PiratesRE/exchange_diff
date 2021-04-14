using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchResourceLoadProbe : SearchProbeBase
	{
		protected override bool SkipOnNonActiveDatabase
		{
			get
			{
				return true;
			}
		}

		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			string[] array = new string[]
			{
				VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.MailboxReplicationService.Classification.ToString(),
				VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.MailboxReplicationServiceHighPriority.Classification.ToString()
			};
			string targetResource = base.Definition.TargetResource;
			string text = null;
			string moveJob = Strings.SearchInformationNotAvailable;
			string resource = Strings.SearchInformationNotAvailable;
			string text2 = Strings.SearchInformationNotAvailable;
			string text3 = Strings.SearchInformationNotAvailable;
			int @int = base.AttributeHelper.GetInt("OscillationDetectionWindowMinute", false, 60, null, null);
			XmlNodeList moveJobs = MrsDiagnosticInfoHelper.GetMoveJobs();
			if (moveJobs == null)
			{
				base.Result.StateAttribute1 = "null";
				return;
			}
			foreach (object obj in moveJobs)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNode xmlNode2 = xmlNode.SelectSingleNode("CurrentlyThrottledResource");
				if (xmlNode2 != null && !string.IsNullOrEmpty(xmlNode2.InnerText))
				{
					string innerText = xmlNode2.InnerText;
					foreach (string arg in SearchResourceLoadProbe.ciResources)
					{
						string text4 = string.Format("{0}({1})", arg, targetResource);
						if (innerText.Equals(text4, StringComparison.OrdinalIgnoreCase))
						{
							base.Result.StateAttribute1 = xmlNode.OuterXml;
							moveJob = SearchResourceLoadProbe.SimpleFormatXml(xmlNode);
							text = text4;
							break;
						}
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			bool flag = false;
			string classification = string.Empty;
			foreach (string text5 in array)
			{
				XmlNode resource2 = MrsDiagnosticInfoHelper.GetResource(text, text5);
				if (resource2 != null)
				{
					base.Result.StateAttribute2 = resource2.OuterXml;
					if (resource2.OuterXml.IndexOf(ResourceLoadState.Underloaded.ToString(), StringComparison.OrdinalIgnoreCase) == -1)
					{
						flag = true;
						classification = text5;
						resource = SearchResourceLoadProbe.SimpleFormatXml(resource2);
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			List<XmlNode> list = new List<XmlNode>();
			XmlNodeList history = MrsDiagnosticInfoHelper.GetHistory(text, classification);
			if (history != null)
			{
				foreach (object obj2 in history)
				{
					XmlNode item = (XmlNode)obj2;
					list.Add(item);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int k = list.Count - 1; k >= 0; k--)
			{
				XmlNode xmlNode3 = list[k];
				string innerText2 = xmlNode3.SelectSingleNode("Load").InnerText;
				string innerText3 = xmlNode3.SelectSingleNode("DateTime").InnerText;
				stringBuilder.AppendFormat("DateTime: {0}, Load: '{1}'\n", innerText3, innerText2);
				DateTime dateTime;
				if (innerText2 != null && (innerText2.IndexOf(ResourceLoadState.Underloaded.ToString(), StringComparison.OrdinalIgnoreCase) >= 0 || innerText2.IndexOf(ResourceLoadState.Full.ToString(), StringComparison.OrdinalIgnoreCase) >= 0) && DateTime.TryParse(innerText3, out dateTime) && dateTime.AddMinutes((double)@int) > DateTime.UtcNow)
				{
					base.Result.StateAttribute5 = "NormalOscillationFound";
					base.Result.StateAttribute4 = stringBuilder.ToString();
					return;
				}
			}
			if (stringBuilder.Length > 0)
			{
				text3 = stringBuilder.ToString();
				base.Result.StateAttribute4 = text3;
			}
			List<CopyStatusClientCachedEntry> cachedDatabaseCopyStatus = SearchMonitoringHelper.GetCachedDatabaseCopyStatus(targetResource);
			if (cachedDatabaseCopyStatus != null)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in cachedDatabaseCopyStatus)
				{
					RpcDatabaseCopyStatus2 copyStatus = copyStatusClientCachedEntry.CopyStatus;
					if (copyStatus != null)
					{
						string copyName = string.Format("{0}\\{1}", copyStatus.DBName, copyStatus.MailboxServer);
						stringBuilder2.AppendLine(Strings.SearchIndexCopyBacklogStatus(copyName, copyStatus.CopyStatus.ToString(), copyStatus.ContentIndexStatus.ToString(), copyStatus.ContentIndexBacklog.ToString(), copyStatus.ContentIndexRetryQueueSize.ToString()));
					}
				}
				if (stringBuilder2.Length > 0)
				{
					text2 = stringBuilder2.ToString();
					base.Result.StateAttribute3 = text2;
				}
			}
			throw new SearchProbeFailureException(Strings.SearchResourceLoadUnhealthy(targetResource, text2, moveJob, resource, text3));
		}

		private static string SimpleFormatXml(XmlNode xmlNode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if (!string.IsNullOrEmpty(xmlNode2.Name) && !string.IsNullOrEmpty(xmlNode2.InnerText))
				{
					stringBuilder.AppendFormat("{0}: {1}\n", xmlNode2.Name, xmlNode2.InnerText);
				}
			}
			return stringBuilder.ToString();
		}

		private const string ResourceKeyTemplate = "{0}({1})";

		private static string[] ciResources = new string[]
		{
			"CiAgeOfLastNotification",
			"CiRetryQueueSize"
		};
	}
}
