using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningReconciliation
{
	internal static class ProvisioningReconciliationHelper
	{
		internal static ReconciliationCookie GetReconciliationCookie(ProvisioningReconciliationConfig provisioningReconciliationConfig, Task.TaskErrorLoggingDelegate errorLogger)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 42, "GetReconciliationCookie", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\ProvisioningReconciliationHelper.cs");
			ReconciliationCookie reconciliationCookie = null;
			if (provisioningReconciliationConfig.ReconciliationCookies != null && provisioningReconciliationConfig.ReconciliationCookies.Count > 0)
			{
				string empty = string.Empty;
				bool flag = false;
				using (MultiValuedProperty<ReconciliationCookie>.Enumerator enumerator = provisioningReconciliationConfig.ReconciliationCookies.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ReconciliationCookie reconciliationCookie2 = enumerator.Current;
						if (reconciliationCookie2.Version == ProvisioningReconciliationHelper.CurrentCookieVersion && ProvisioningReconciliationHelper.IsServerSuitable(reconciliationCookie2.InvocationId, topologyConfigurationSession, out empty, out flag))
						{
							reconciliationCookie = new ReconciliationCookie(reconciliationCookie2.Version, empty, reconciliationCookie2.InvocationId, reconciliationCookie2.HighestCommittedUsn);
							if (flag)
							{
								break;
							}
						}
					}
					goto IL_E8;
				}
			}
			Fqdn[] domainControllersInLocalSite = ProvisioningReconciliationHelper.GetDomainControllersInLocalSite(errorLogger);
			if (domainControllersInLocalSite != null)
			{
				foreach (Fqdn fqdn in domainControllersInLocalSite)
				{
					reconciliationCookie = ProvisioningReconciliationHelper.GetReconciliationCookieForDomainController(fqdn, topologyConfigurationSession, errorLogger);
					if (reconciliationCookie != null)
					{
						break;
					}
				}
			}
			IL_E8:
			if (reconciliationCookie != null)
			{
				return reconciliationCookie;
			}
			errorLogger(new TaskException(Strings.ErrorNoActiveDCForProvisioningReconciliationCookie), (ErrorCategory)1001, null);
			return null;
		}

		internal static MultiValuedProperty<ReconciliationCookie> GetReconciliationCookiesForNextCycle(string dc, Task.TaskErrorLoggingDelegate errorLogger)
		{
			MultiValuedProperty<ReconciliationCookie> multiValuedProperty = new MultiValuedProperty<ReconciliationCookie>();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(dc, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromRootOrgScopeSet(), 126, "GetReconciliationCookiesForNextCycle", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\ProvisioningReconciliationHelper.cs");
			topologyConfigurationSession.UseConfigNC = false;
			MultiValuedProperty<ReplicationCursor> multiValuedProperty2 = topologyConfigurationSession.ReadReplicationCursors(ADSession.GetDomainNamingContextForLocalForest());
			topologyConfigurationSession.UseConfigNC = true;
			foreach (ReplicationCursor replicationCursor in multiValuedProperty2)
			{
				if (replicationCursor.SourceDsa != null)
				{
					ADServer adserver = topologyConfigurationSession.FindDCByInvocationId(replicationCursor.SourceInvocationId);
					if (adserver != null)
					{
						string dnsHostName = adserver.DnsHostName;
						ReconciliationCookie item = new ReconciliationCookie(ProvisioningReconciliationHelper.CurrentCookieVersion, dnsHostName, replicationCursor.SourceInvocationId, replicationCursor.UpToDatenessUsn);
						multiValuedProperty.Add(item);
					}
				}
			}
			return multiValuedProperty;
		}

		private static ReconciliationCookie GetReconciliationCookieForDomainController(Fqdn fqdn, ITopologyConfigurationSession configSession, Task.TaskErrorLoggingDelegate errorLogger)
		{
			ADServer adserver = configSession.FindDCByFqdn(fqdn);
			if (adserver != null)
			{
				LocalizedString empty = LocalizedString.Empty;
				string text;
				if (SuitabilityVerifier.IsServerSuitableIgnoreExceptions(adserver.DnsHostName, true, null, out text, out empty))
				{
					ITopologyConfigurationSession sessionForDC = ProvisioningReconciliationHelper.GetSessionForDC(adserver);
					RootDse rootDse = sessionForDC.GetRootDse();
					Guid invocationIdByDC = sessionForDC.GetInvocationIdByDC(adserver);
					return new ReconciliationCookie(ProvisioningReconciliationHelper.CurrentCookieVersion, adserver.DnsHostName, invocationIdByDC, rootDse.HighestCommittedUSN);
				}
			}
			return null;
		}

		private static Fqdn[] GetDomainControllersInLocalSite(Task.TaskErrorLoggingDelegate errorLogger)
		{
			ADForest localForest = ADForest.GetLocalForest();
			if (localForest == null)
			{
				errorLogger(new TaskException(Strings.ErrorCannotRetrieveLocalForest), (ErrorCategory)1001, null);
			}
			List<ADServer> list = localForest.FindAllGlobalCatalogsInLocalSite();
			if (list == null || list.Count == 0)
			{
				errorLogger(new TaskException(Strings.ErrorNoDCInLocalSite), (ErrorCategory)1001, null);
			}
			Fqdn[] array = new Fqdn[list.Count];
			int num = 0;
			foreach (ADServer adserver in list)
			{
				array[num] = new Fqdn(adserver.DnsHostName);
				num++;
			}
			return array;
		}

		private static ITopologyConfigurationSession GetSessionForDC(ADServer dc)
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(dc.DnsHostName, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 246, "GetSessionForDC", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\ProvisioningReconciliationHelper.cs");
		}

		private static bool IsServerSuitable(Guid invocationId, ITopologyConfigurationSession session, out string dnsHostName, out bool isInLocalSite)
		{
			dnsHostName = string.Empty;
			isInLocalSite = false;
			ADServer adserver = session.FindDCByInvocationId(invocationId);
			if (adserver == null)
			{
				return false;
			}
			dnsHostName = adserver.DnsHostName;
			isInLocalSite = adserver.IsInLocalSite;
			LocalizedString empty = LocalizedString.Empty;
			string text;
			return SuitabilityVerifier.IsServerSuitableIgnoreExceptions(adserver.DnsHostName, true, null, out text, out empty);
		}

		public static readonly int CurrentCookieVersion = 2;
	}
}
