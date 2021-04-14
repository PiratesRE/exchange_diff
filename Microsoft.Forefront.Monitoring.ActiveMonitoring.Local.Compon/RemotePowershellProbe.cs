using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Exchange.Hygiene.Migration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class RemotePowershellProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				this.LogResult("RemotePowershellProbe started. ", new object[0]);
				this.InitializeFromXml(base.Definition.ExtensionAttributes);
				using (IRemotePowershellDataProvider dataProvider = this.GetDataProvider())
				{
					dataProvider.Find<TenantInboundConnector>(null, null, false, null).Cast<TenantInboundConnector>();
				}
			}
			catch (Exception ex)
			{
				this.LogResult("Exception: {0} ", new object[]
				{
					ex
				});
				throw;
			}
			finally
			{
				stopwatch.Stop();
				this.LogResult("RemotePowershellProbe finished in: {0}", new object[]
				{
					stopwatch.Elapsed
				});
			}
		}

		private void InitializeFromXml(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
			{
				throw new ArgumentException("XML is null or whitespace");
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(xml);
			XmlElement xmlElement = Utils.CheckXmlElement(safeXmlDocument.SelectSingleNode("//Configuration"), "//Configuration");
			string attribute = xmlElement.GetAttribute("FeatureTag", string.Empty);
			if (string.IsNullOrWhiteSpace(attribute))
			{
				throw new ArgumentException("Did not find valid FeatureTag configuration attribute");
			}
			IEnumerable<ProbeOrganizationInfo> probeOrganizations = new GlobalConfigSession().GetProbeOrganizations(attribute);
			if (probeOrganizations == null || !probeOrganizations.Any<ProbeOrganizationInfo>())
			{
				throw new ArgumentException(string.Format("Did not find any probe organizations for feature tag '{0}'", attribute));
			}
			this.probeOrganizationInfo = probeOrganizations.ElementAt(new Random().Next(0, probeOrganizations.Count<ProbeOrganizationInfo>()));
		}

		private IRemotePowershellDataProvider GetDataProvider()
		{
			return RemotePowershellProbe.providerFactory.CreateProviderForTenant("RemotePowershellProbe", new ADObjectId("DN=" + this.probeOrganizationInfo.OrganizationName), this.probeOrganizationInfo.CustomerType);
		}

		private void LogResult(string format, params object[] objs)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format(format, objs);
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += Environment.NewLine;
		}

		private const string CallerId = "RemotePowershellProbe";

		private static PowershellDataProviderFactory providerFactory = new PowershellDataProviderFactory();

		private ProbeOrganizationInfo probeOrganizationInfo;
	}
}
