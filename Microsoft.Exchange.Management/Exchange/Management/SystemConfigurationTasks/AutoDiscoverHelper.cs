using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class AutoDiscoverHelper
	{
		public static void DiscoverBegin(string componentId, string emailAddress, CredentialsImpersonator credentialsImpersonator, ITopologyConfigurationSession configSession, Task.TaskVerboseLoggingDelegate verboseDelegate, AsyncCallback asyncCallback, params string[] optionalHeaders)
		{
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("componentId");
			}
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			if (credentialsImpersonator == null)
			{
				throw new ArgumentNullException("credentialsImpersonator");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("verboseDelegate");
			}
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			string autoDiscoverEndpoint = AutoDiscoverHelper.GetAutoDiscoverEndpoint(emailAddress, configSession, verboseDelegate);
			verboseDelegate(Strings.TowsAutodiscoverUrl(autoDiscoverEndpoint));
			if (!string.IsNullOrEmpty(autoDiscoverEndpoint))
			{
				AutoDiscoverClient autoDiscoverClient = new AutoDiscoverClient(componentId, verboseDelegate, credentialsImpersonator, emailAddress, autoDiscoverEndpoint, true, optionalHeaders);
				autoDiscoverClient.BeginInvoke(asyncCallback);
			}
		}

		public static AutoDiscoverResponseXML DiscoverEnd(IAsyncResult asyncResult, out string url)
		{
			return AutoDiscoverClient.EndInvoke(asyncResult, out url);
		}

		public static string GetAutoDiscoverEndpoint(string emailAddress, ITopologyConfigurationSession configSession, Task.TaskVerboseLoggingDelegate verboseDelegate)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			if (verboseDelegate == null)
			{
				throw new ArgumentNullException("verboseDelegate");
			}
			SmtpAddress smtpAddress = new SmtpAddress(emailAddress);
			string domain = smtpAddress.Domain;
			string text = AutoDiscoverHelper.GetUrlFromScp(configSession, domain, verboseDelegate);
			if (!string.IsNullOrEmpty(text) && text.StartsWith("LDAP", StringComparison.OrdinalIgnoreCase))
			{
				verboseDelegate(Strings.TowsXForest(emailAddress, text));
				text = null;
			}
			string text2 = "AutoDiscover." + domain;
			if (string.IsNullOrEmpty(text) && AutoDiscoverHelper.ValidateDns(text2))
			{
				text = string.Format("https://{0}/AutoDiscover/AutoDiscover.xml", text2);
			}
			if (string.IsNullOrEmpty(text) && AutoDiscoverHelper.ValidateDns(domain))
			{
				text = string.Format("https://{0}/AutoDiscover/AutoDiscover.xml", domain);
			}
			return text;
		}

		private static string GetUrlFromScp(ITopologyConfigurationSession configSession, string domainName, Task.TaskVerboseLoggingDelegate verboseDelegate)
		{
			QueryFilter filter = ExchangeScpObjects.AutodiscoverUrlKeyword.Filter;
			IConfigurable[] array = configSession.Find<ADServiceConnectionPoint>(filter, null, true, null);
			string empty = string.Empty;
			if (array != null && array.Length > 0)
			{
				string item = "Domain=" + domainName;
				ADServiceConnectionPoint adserviceConnectionPoint = null;
				foreach (IConfigurable configurable in array)
				{
					ADServiceConnectionPoint adserviceConnectionPoint2 = configurable as ADServiceConnectionPoint;
					if (adserviceConnectionPoint2.Keywords.Contains("67661d7F-8FC4-4fa7-BFAC-E1D7794C1F68") && AutoDiscoverHelper.IsE14OrLater(configSession, adserviceConnectionPoint2))
					{
						verboseDelegate(Strings.TowsFoundScpByDomain(adserviceConnectionPoint2.Identity.ToString(), domainName, adserviceConnectionPoint2.ServiceBindingInformation[0]));
						if (adserviceConnectionPoint2.Keywords.Contains(item))
						{
							return adserviceConnectionPoint2.ServiceBindingInformation[0];
						}
					}
					else if (adserviceConnectionPoint2.Keywords.Count == 1 && adserviceConnectionPoint == null)
					{
						adserviceConnectionPoint = adserviceConnectionPoint2;
					}
				}
				string siteName = NativeHelpers.GetSiteName(false);
				string item2 = "Site=" + siteName;
				ADServiceConnectionPoint adserviceConnectionPoint3 = null;
				foreach (IConfigurable configurable2 in array)
				{
					ADServiceConnectionPoint adserviceConnectionPoint4 = configurable2 as ADServiceConnectionPoint;
					if (adserviceConnectionPoint4.Keywords.Contains("77378F46-2C66-4aa9-A6A6-3E7A48B19596") && adserviceConnectionPoint4.Keywords.Contains(item2) && AutoDiscoverHelper.IsE14OrLater(configSession, adserviceConnectionPoint4))
					{
						verboseDelegate(Strings.TowsFoundScpBySite(adserviceConnectionPoint4.Identity.ToString(), siteName, adserviceConnectionPoint4.ServiceBindingInformation[0]));
						return adserviceConnectionPoint4.ServiceBindingInformation[0];
					}
					if (adserviceConnectionPoint3 == null)
					{
						adserviceConnectionPoint3 = adserviceConnectionPoint4;
					}
				}
				if (adserviceConnectionPoint3 != null)
				{
					return adserviceConnectionPoint3.ServiceBindingInformation[0];
				}
				if (adserviceConnectionPoint != null)
				{
					return adserviceConnectionPoint.ServiceBindingInformation[0];
				}
			}
			return null;
		}

		private static bool IsE14OrLater(ITopologyConfigurationSession configSession, ADServiceConnectionPoint scp)
		{
			Server server = configSession.FindServerByName(scp.ServiceDnsName);
			return server != null && server.IsE14OrLater;
		}

		private static bool ValidateDns(string domainName)
		{
			IPHostEntry iphostEntry = null;
			try
			{
				iphostEntry = Dns.GetHostEntry(domainName);
			}
			catch (SocketException)
			{
			}
			return iphostEntry != null && iphostEntry.AddressList.Length != 0;
		}
	}
}
