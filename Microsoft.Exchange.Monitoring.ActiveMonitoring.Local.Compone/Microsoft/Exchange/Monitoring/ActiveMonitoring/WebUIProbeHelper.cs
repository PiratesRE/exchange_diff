using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class WebUIProbeHelper : ProbeDefinitionHelper
	{
		internal override List<ProbeDefinition> CreateDefinition()
		{
			List<ProbeDefinition> list = new List<ProbeDefinition>();
			XmlNode xmlNode = base.DefinitionNode.SelectSingleNode("ExtensionAttributes");
			if (xmlNode == null)
			{
				string message = "The required node ExtensionAttributes was not provided for the WebUIProbe.";
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message, null, "CreateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\WebUIProbeHelper.cs", 74);
				throw new InvalidOperationException(message);
			}
			ProbeDefinition probeDefinition = base.CreateProbeDefinition();
			probeDefinition.ExtensionAttributes = xmlNode.OuterXml;
			string optionalXmlAttribute = DefinitionHelperBase.GetOptionalXmlAttribute<string>(base.DiscoveryContext.ContextNode, "FeatureTag", string.Empty, base.TraceContext);
			if (optionalXmlAttribute != string.Empty)
			{
				WebUIProbeHelper.WorkDefinitionUser workDefinitionUser = this.GetWorkDefinitionUser(optionalXmlAttribute);
				string newValue = string.Empty;
				string newValue2 = string.Empty;
				string newValue3 = string.Empty;
				WebUIProbeHelper.DatacenterEnvironment datacenterEnvironment = this.GetDatacenterEnvironment();
				if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
				{
					switch (datacenterEnvironment)
					{
					case WebUIProbeHelper.DatacenterEnvironment.FfoDogfood:
						newValue = WebUIProbeHelper.UmcUrl.FfoDogfood;
						newValue2 = WebUIProbeHelper.FfoRwsUrl.FfoDogfood;
						newValue3 = WebUIProbeHelper.UccUrl.FfoDogfood;
						goto IL_168;
					case WebUIProbeHelper.DatacenterEnvironment.FfoProduction:
						newValue = WebUIProbeHelper.UmcUrl.FfoProduction;
						newValue2 = WebUIProbeHelper.FfoRwsUrl.FfoProduction;
						newValue3 = WebUIProbeHelper.UccUrl.FfoProduction;
						goto IL_168;
					}
					newValue = WebUIProbeHelper.UmcUrl.FfoTest;
					newValue2 = WebUIProbeHelper.FfoRwsUrl.FfoTest;
					newValue3 = WebUIProbeHelper.UccUrl.FfoTest;
				}
				else
				{
					switch (datacenterEnvironment)
					{
					case WebUIProbeHelper.DatacenterEnvironment.ExoDogFood:
						newValue = WebUIProbeHelper.UmcUrl.ExoDogfood;
						newValue2 = WebUIProbeHelper.FfoRwsUrl.ExoDogfood;
						goto IL_168;
					case WebUIProbeHelper.DatacenterEnvironment.ExoProduction:
						newValue = WebUIProbeHelper.UmcUrl.ExoProduction;
						newValue2 = WebUIProbeHelper.FfoRwsUrl.ExoProduction;
						goto IL_168;
					}
					newValue = WebUIProbeHelper.UmcUrl.ExoTest;
					newValue2 = WebUIProbeHelper.FfoRwsUrl.ExoTest;
				}
				IL_168:
				string text = probeDefinition.ExtensionAttributes;
				text = text.Replace(WebUIProbeHelper.ReplacementTokens.FfoRwsUrl, newValue2);
				text = text.Replace(WebUIProbeHelper.ReplacementTokens.UmcUrl, newValue);
				text = text.Replace(WebUIProbeHelper.ReplacementTokens.UccUrl, newValue3);
				text = text.Replace(WebUIProbeHelper.ReplacementTokens.Tenant, workDefinitionUser.TenantName);
				text = text.Replace(WebUIProbeHelper.ReplacementTokens.User, workDefinitionUser.Username);
				text = text.Replace(WebUIProbeHelper.ReplacementTokens.Password, workDefinitionUser.Password);
				probeDefinition.ExtensionAttributes = text;
			}
			list.Add(probeDefinition);
			return list;
		}

		private WebUIProbeHelper.DatacenterEnvironment GetDatacenterEnvironment()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
				if (!string.IsNullOrWhiteSpace(ipproperties.DnsSuffix))
				{
					string text = ipproperties.DnsSuffix.ToLower();
					WebUIProbeHelper.DatacenterEnvironment result;
					if (text.Contains("protection.gbl"))
					{
						result = WebUIProbeHelper.DatacenterEnvironment.FfoProduction;
					}
					else if (text.Contains("ffo.gbl"))
					{
						result = WebUIProbeHelper.DatacenterEnvironment.FfoDogfood;
					}
					else if (text.Contains("sdf.exchangelabs.com"))
					{
						result = WebUIProbeHelper.DatacenterEnvironment.ExoDogFood;
					}
					else
					{
						if (!text.Contains("prod.outlook.com") && !text.Contains("prod.exchangelabs.com"))
						{
							goto IL_8C;
						}
						result = WebUIProbeHelper.DatacenterEnvironment.ExoProduction;
					}
					return result;
				}
				IL_8C:;
			}
			return WebUIProbeHelper.DatacenterEnvironment.Test;
		}

		private WebUIProbeHelper.WorkDefinitionUser GetWorkDefinitionUser(string featureTag)
		{
			GlobalConfigSession globalConfigSession = new GlobalConfigSession();
			IEnumerable<ProbeOrganizationInfo> probeOrganizations = globalConfigSession.GetProbeOrganizations(featureTag);
			if (probeOrganizations.Count<ProbeOrganizationInfo>() == 0)
			{
				throw new InvalidOperationException(string.Format("Cannot find any tenant for feature tag \"{0}\".", featureTag));
			}
			int count = WebUIProbeHelper.random.Next(0, probeOrganizations.Count<ProbeOrganizationInfo>());
			ProbeOrganizationInfo probeOrganizationInfo = probeOrganizations.Skip(count).First<ProbeOrganizationInfo>();
			ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromExternalDirectoryOrganizationId(probeOrganizationInfo.ProbeOrganizationId.ObjectGuid), 219, "GetWorkDefinitionUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\WebUIProbeHelper.cs");
			IEnumerable<ADUser> enumerable = tenantRecipientSession.Find<ADUser>(null, QueryScope.SubTree, null, null, int.MaxValue);
			if (enumerable == null || !enumerable.Any<ADUser>())
			{
				throw new InvalidOperationException(string.Format("Cannot find any user for feature tag \"{0}\".", featureTag));
			}
			ADUser aduser = enumerable.FirstOrDefault((ADUser adUser) => adUser.WindowsLiveID.ToString().ToLower().StartsWith("admin@")) ?? enumerable.ElementAt(WebUIProbeHelper.random.Next(enumerable.Count<ADUser>()));
			return new WebUIProbeHelper.WorkDefinitionUser(probeOrganizationInfo.OrganizationName, aduser.WindowsLiveID.ToString(), probeOrganizationInfo.LoginPassword);
		}

		private const string DefaultMonitoringAccount = "admin@";

		private static Random random = new Random();

		private enum DatacenterEnvironment
		{
			FfoDogfood,
			FfoProduction,
			ExoDogFood,
			ExoProduction,
			Test
		}

		private static class ReplacementTokens
		{
			internal static readonly string User = "{$User}";

			internal static readonly string Password = "{$Password}";

			internal static readonly string Tenant = "{$Tenant}";

			internal static readonly string UmcUrl = "{$UmcUrl}";

			internal static readonly string FfoRwsUrl = "{$FfoRwsUrl}";

			internal static readonly string UccUrl = "{$UccUrl}";
		}

		private static class UmcUrl
		{
			internal static readonly string ExoTest = "exchangelabs.live-int.com/ecp/?Realm={$Tenant}";

			internal static readonly string ExoProduction = "www.outlook.com/ecp/?exsvurl=1&amp;Realm={$Tenant}";

			internal static readonly string ExoDogfood = "sdfpilot.outlook.com/ecp/?exsvurl=1&amp;Realm={$Tenant}";

			internal static readonly string FfoTest = "admin.o365filtering-int.com/ecp/?Realm={$Tenant}";

			internal static readonly string FfoProduction = "admin.protection.outlook.com/ecp/?exsvurl=1&amp;Realm={$Tenant}";

			internal static readonly string FfoDogfood = "admin.o365filtering.com/ecp/?exsvurl=1&amp;Realm={$Tenant}";
		}

		private static class UccUrl
		{
			internal static readonly string FfoTest = "compliance.o365filtering-int.com:446/Ucc/?Realm={$Tenant}";

			internal static readonly string FfoProduction = "compliance.protection.outlook.com/Ucc/?exsvurl=1&amp;Realm={$Tenant}";

			internal static readonly string FfoDogfood = "compliance.o365filtering.com/Ucc/?exsvurl=1&amp;Realm={$Tenant}";
		}

		private static class FfoRwsUrl
		{
			internal static readonly string ExoTest = "exchangelabs.live-int.com";

			internal static readonly string FfoTest = "admin.o365filtering-int.com";

			internal static readonly string ExoDogfood = "sdfpilot.outlook.com";

			internal static readonly string FfoDogfood = "admin.o365filtering.com";

			internal static readonly string ExoProduction = "reports.office365.com";

			internal static readonly string FfoProduction = "admin.protection.outlook.com";
		}

		private class WorkDefinitionUser
		{
			public WorkDefinitionUser(string tenantName, string username, SecureString password)
			{
				this.TenantName = tenantName;
				this.Username = username;
				IntPtr ptr = Marshal.SecureStringToBSTR(password);
				try
				{
					this.Password = Marshal.PtrToStringBSTR(ptr);
				}
				finally
				{
					Marshal.FreeBSTR(ptr);
				}
			}

			public string TenantName { get; private set; }

			public string Username { get; private set; }

			public string Password { get; private set; }
		}
	}
}
