using System;
using System.IO;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Probes
{
	public class DiagnosticsAggregationLocalSnapshotProbe : ProbeWorkItem
	{
		private static ConfigurationLoader<TransportServerConfiguration, TransportServerConfiguration.Builder> LocalServer
		{
			get
			{
				if (DiagnosticsAggregationLocalSnapshotProbe.localServer == null)
				{
					lock (DiagnosticsAggregationLocalSnapshotProbe.locker)
					{
						if (DiagnosticsAggregationLocalSnapshotProbe.localServer == null)
						{
							ConfigurationLoader<TransportServerConfiguration, TransportServerConfiguration.Builder> configurationLoader = new ConfigurationLoader<TransportServerConfiguration, TransportServerConfiguration.Builder>(TimeSpan.Zero);
							configurationLoader.Load();
							DiagnosticsAggregationLocalSnapshotProbe.localServer = configurationLoader;
						}
					}
				}
				return DiagnosticsAggregationLocalSnapshotProbe.localServer;
			}
		}

		private string QueueLogPath
		{
			get
			{
				if (!(DiagnosticsAggregationLocalSnapshotProbe.LocalServer.Cache.TransportServer.QueueLogPath == null))
				{
					return DiagnosticsAggregationLocalSnapshotProbe.LocalServer.Cache.TransportServer.QueueLogPath.PathName;
				}
				return null;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(base.Definition.ExtensionAttributes);
			TimeSpan t = TimeSpan.FromMinutes(30.0);
			XmlAttribute xmlAttribute = (XmlAttribute)xmlDocument.SelectSingleNode("//ExtensionAttributes/@MaximumFileAgeMinutes");
			int num;
			if (xmlAttribute != null && int.TryParse(xmlAttribute.Value, out num))
			{
				t = TimeSpan.FromMinutes((double)num);
			}
			base.Result.StateAttribute3 = t.ToString("c");
			if (!string.IsNullOrEmpty(this.QueueLogPath))
			{
				string text = Path.Combine(this.QueueLogPath, "QueueSnapShot.xml");
				base.Result.StateAttribute1 = text;
				TimeSpan t2 = DateTime.UtcNow - File.GetLastWriteTimeUtc(text);
				base.Result.StateAttribute2 = File.GetLastWriteTimeUtc(text).ToString("U");
				base.Result.StateAttribute4 = t2.ToString("c");
				if (t2 > t)
				{
					throw new ApplicationException("File age exceeds maximum");
				}
			}
		}

		private static object locker = new object();

		private static ConfigurationLoader<TransportServerConfiguration, TransportServerConfiguration.Builder> localServer;
	}
}
