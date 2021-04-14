using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class HttpProxyBackEndHelper
	{
		internal static bool IsPartnerHostedOnly
		{
			get
			{
				return HttpProxyBackEndHelper.isPartnerHostedOnly.Member;
			}
		}

		internal static ADUser GetDefaultOrganizationMailbox(OrganizationId organizationId, string anchorMailboxKey = null)
		{
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 126, "GetDefaultOrganizationMailbox", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs");
			return HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(tenantOrRootOrgRecipientSession, anchorMailboxKey);
		}

		internal static ADUser GetDefaultOrganizationMailbox(IRecipientSession recipientSession, string anchorMailboxKey = null)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			ADUser aduser = HttpProxyBackEndHelper.GetOrganizationMailbox(recipientSession, HttpProxyBackEndHelper.DefaultOrganizationCapability, anchorMailboxKey);
			if (aduser == null)
			{
				ADUser[] array = recipientSession.FindADUser(null, QueryScope.SubTree, HttpProxyBackEndHelper.E15OrganizationMailboxFilter, null, 1);
				if (array.Length == 0)
				{
					ExTraceGlobals.CafeTracer.TraceError<OrganizationId>(0L, "[HttpProxyBackEndHelper.GetDefaultOrganizationMailbox] Unable to find E15 organization mailbox by name for organization {0}.", recipientSession.SessionSettings.CurrentOrganizationId);
					array = recipientSession.FindADUser(null, QueryScope.SubTree, HttpProxyBackEndHelper.E14EDiscoveryMailboxFilter, null, 1);
					if (array.Length == 0)
					{
						ExTraceGlobals.CafeTracer.TraceError<OrganizationId>(0L, "[HttpProxyBackEndHelper.GetDefaultOrganizationMailbox] Unable to find E14 eDiscovery mailbox by name for organization {0}.", recipientSession.SessionSettings.CurrentOrganizationId);
					}
					else
					{
						aduser = array[HttpProxyBackEndHelper.ComputeIndex(anchorMailboxKey, array.Length)];
						ExTraceGlobals.CafeTracer.TraceDebug<ObjectId, OrganizationId>(0L, "[HttpProxyBackEndHelper.GetDefaultOrganizationMailbox] Found E14 eDiscovery mailbox {0} by name for organization {1}.", aduser.Identity, recipientSession.SessionSettings.CurrentOrganizationId);
					}
				}
				else
				{
					aduser = array[HttpProxyBackEndHelper.ComputeIndex(anchorMailboxKey, array.Length)];
					ExTraceGlobals.CafeTracer.TraceDebug<ObjectId, OrganizationId>(0L, "[HttpProxyBackEndHelper.GetDefaultOrganizationMailbox] Found E15 eDiscovery mailbox {0} by name for organization {1}.", aduser.Identity, recipientSession.SessionSettings.CurrentOrganizationId);
				}
			}
			return aduser;
		}

		internal static int ComputeIndex(string identifier, int arrayLength)
		{
			int hashCode = HttpProxyBackEndHelper.GetHashCode(identifier);
			int num = hashCode % arrayLength;
			ExTraceGlobals.CafeTracer.TraceDebug<string, int, int>(0L, "[HttpProxyBackEndHelper.ComputeIndex] AnchorMailboxKey: {0}, HashPivot {1}, Index {2}.", identifier ?? string.Empty, hashCode, num);
			return num;
		}

		internal static int GetHashCode(string key)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(key))
			{
				result = Math.Abs(key.ToUpper().GetHashCode());
			}
			return result;
		}

		internal static ADUser GetE14EDiscoveryMailbox(IRecipientSession recipientSession)
		{
			ADUser result = null;
			ADUser[] array = recipientSession.FindADUser(null, QueryScope.SubTree, HttpProxyBackEndHelper.E14EDiscoveryMailboxFilter, null, 1);
			if (array.Length == 0)
			{
				ExTraceGlobals.CafeTracer.TraceError<OrganizationId>(0L, "[HttpProxyBackEndHelper.GetE14EDiscoveryMailbox] Unable to find E14 eDiscovery mailbox by name for organization {0}.", recipientSession.SessionSettings.CurrentOrganizationId);
			}
			else
			{
				result = array[0];
			}
			return result;
		}

		internal static ADUser GetOrganizationMailbox(OrganizationId organizationId, OrganizationCapability capability, string anchorMailboxKey = null)
		{
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 269, "GetOrganizationMailbox", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs");
			return HttpProxyBackEndHelper.GetOrganizationMailbox(tenantOrRootOrgRecipientSession, capability, anchorMailboxKey);
		}

		internal static ADUser GetOrganizationMailboxInClosestSite(OrganizationId organizationId, OrganizationCapability capability)
		{
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 291, "GetOrganizationMailboxInClosestSite", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs");
			return HttpProxyBackEndHelper.GetOrganizationMailboxInClosestSite(tenantOrRootOrgRecipientSession, capability);
		}

		internal static BackEndServer GetAnyBackEndServer()
		{
			return HttpProxyBackEndHelper.GetAnyBackEndServer(VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.ServersCache.Enabled);
		}

		internal static BackEndServer GetAnyBackEndServer(bool useServersCache)
		{
			if (useServersCache)
			{
				MiniServer anyBackEndServerWithMinVersion = ServersCache.GetAnyBackEndServerWithMinVersion(Server.E15MinVersion);
				return new BackEndServer(anyBackEndServerWithMinVersion.Fqdn, anyBackEndServerWithMinVersion.VersionNumber);
			}
			return HttpProxyBackEndHelper.GetAnyBackEndServerForVersion<WebServicesService>(new ServerVersion(Server.E15MinVersion), false, ClientAccessType.InternalNLBBypass, false);
		}

		internal static BackEndServer GetAnyBackEndServerForVersion<ServiceType>(ServerVersion serverVersion, bool exactVersionMatch, ClientAccessType clientAccessType, bool currentSiteOnly = false) where ServiceType : HttpService
		{
			if (serverVersion == null)
			{
				throw new ArgumentNullException("version");
			}
			ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetAnyBackEndServerForVersion", 349);
			string empty = string.Empty;
			int version = 0;
			ServerRole serverRoleToLookFor = (serverVersion.Major < 15) ? ServerRole.ClientAccess : ServerRole.Mailbox;
			if (currentServiceTopology.TryGetRandomServerFromCurrentSite(serverRoleToLookFor, serverVersion.ToInt(), ServiceTopology.RandomServerSearchType.ExactVersion, out empty, out version, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetAnyBackEndServerForVersion", 358))
			{
				return new BackEndServer(empty, version);
			}
			if (!exactVersionMatch)
			{
				ServerVersion serverVersion2 = new ServerVersion(serverVersion.Major, 0, 0, 0);
				if (currentServiceTopology.TryGetRandomServerFromCurrentSite(serverRoleToLookFor, serverVersion2.ToInt(), ServiceTopology.RandomServerSearchType.MinimumVersionMatchMajor, out empty, out version, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetAnyBackEndServerForVersion", 374))
				{
					return new BackEndServer(empty, version);
				}
			}
			if (!currentSiteOnly)
			{
				HttpService httpService = currentServiceTopology.FindAny<ServiceType>(clientAccessType, (ServiceType service) => service != null && !service.IsFrontEnd && (service.ServerRole & serverRoleToLookFor) == serverRoleToLookFor && !service.IsOutOfService && service.AdminDisplayVersionNumber == serverVersion, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetAnyBackEndServerForVersion", 389);
				if (httpService != null)
				{
					return new BackEndServer(httpService.ServerFullyQualifiedDomainName, httpService.ServerVersionNumber);
				}
				if (!exactVersionMatch)
				{
					new ServerVersion(serverVersion.Major, 0, 0, 0);
					httpService = currentServiceTopology.FindAny<ServiceType>(clientAccessType, (ServiceType service) => service != null && !service.IsFrontEnd && (service.ServerRole & serverRoleToLookFor) == serverRoleToLookFor && !service.IsOutOfService && service.ServerVersionNumber > serverVersion.ToInt() && new ServerVersion(service.ServerVersionNumber).Major == serverVersion.Major, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetAnyBackEndServerForVersion", 408);
					if (httpService != null)
					{
						return new BackEndServer(httpService.ServerFullyQualifiedDomainName, httpService.ServerVersionNumber);
					}
				}
			}
			if (!exactVersionMatch)
			{
				ServerVersion higherVersion = new ServerVersion(serverVersion.Major + 1, 0, 0, 0);
				ServerRole higherVersionServerRoleToLookFor = (higherVersion.Major < 15) ? ServerRole.ClientAccess : ServerRole.Mailbox;
				if (currentServiceTopology.TryGetRandomServerFromCurrentSite(higherVersionServerRoleToLookFor, higherVersion.ToInt(), ServiceTopology.RandomServerSearchType.MinimumVersion, out empty, out version, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetAnyBackEndServerForVersion", 431))
				{
					return new BackEndServer(empty, version);
				}
				if (!currentSiteOnly)
				{
					HttpService httpService2 = currentServiceTopology.FindAny<ServiceType>(clientAccessType, (ServiceType service) => service != null && !service.IsFrontEnd && (service.ServerRole & higherVersionServerRoleToLookFor) == higherVersionServerRoleToLookFor && !service.IsOutOfService && service.ServerVersionNumber > higherVersion.ToInt(), "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetAnyBackEndServerForVersion", 444);
					if (httpService2 != null)
					{
						return new BackEndServer(httpService2.ServerFullyQualifiedDomainName, httpService2.ServerVersionNumber);
					}
				}
			}
			string message = string.Format("Unable to find any backend server for version {0}.", serverVersion);
			throw new ServerNotFoundException(message, serverVersion.ToString());
		}

		internal static BackEndServer GetDeterministicBackEndServer<ServiceType>(BackEndServer mailboxServer, string identifier, ClientAccessType clientAccessType) where ServiceType : HttpService
		{
			if (mailboxServer == null)
			{
				throw new ArgumentNullException("mailboxServer");
			}
			if (string.IsNullOrEmpty(identifier))
			{
				throw new ArgumentNullException("identifier");
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.ServersCache.Enabled)
			{
				MiniServer deterministicBackEndServerFromSameSite = ServersCache.GetDeterministicBackEndServerFromSameSite(mailboxServer.Fqdn, Server.E15MinVersion, identifier, false);
				return new BackEndServer(deterministicBackEndServerFromSameSite.Fqdn, deterministicBackEndServerFromSameSite.VersionNumber);
			}
			HttpProxyBackEndHelper.TopologyWithSites completeServiceTopologyWithSites = HttpProxyBackEndHelper.GetCompleteServiceTopologyWithSites(mailboxServer.Fqdn);
			HttpService[] array = (HttpService[])HttpProxyBackEndHelper.FindAcceptableBackEndService<ServiceType>(completeServiceTopologyWithSites, clientAccessType, (int x) => x >= Server.E15MinVersion);
			if (array.Length > 0)
			{
				int num = HttpProxyBackEndHelper.ComputeIndex(identifier, array.Length);
				ExTraceGlobals.CafeTracer.TraceDebug<int, string, int>(0L, "[HttpProxyBackEndHelper.GetDeterministicBackEndServer] Buckets: {0} Identifier: {1} Index: {2}", array.Length, identifier, num);
				return new BackEndServer(array[num].ServerFullyQualifiedDomainName, array[num].ServerVersionNumber);
			}
			throw new ServerNotFoundException("Unable to find proper HTTP service.");
		}

		internal static Uri GetE12ExternalUrl<ServiceType>(BackEndServer mailboxServer) where ServiceType : HttpService
		{
			if (mailboxServer == null)
			{
				throw new ArgumentNullException("mailboxServer");
			}
			if (mailboxServer.Version >= Server.E14MinVersion)
			{
				throw new ArgumentException("Mailbox server version is higher then E12", "mailboxServer");
			}
			HttpProxyBackEndHelper.TopologyWithSites legacyServiceTopologyWithSites = HttpProxyBackEndHelper.GetLegacyServiceTopologyWithSites(mailboxServer.Fqdn);
			HttpService httpService = HttpProxyBackEndHelper.GetBestBackEndServiceForVersion<ServiceType>(legacyServiceTopologyWithSites, ClientAccessType.External, (int x) => x >= Server.E2007MinVersion && x < Server.E14MinVersion);
			return httpService.Url;
		}

		internal static Uri GetBackEndServiceUrlByServer<ServiceType>(BackEndServer backEndServer) where ServiceType : HttpService
		{
			if (backEndServer.IsE15OrHigher)
			{
				return GlobalServiceUrls.GetInternalUrl<ServiceType>(backEndServer.Fqdn);
			}
			HttpProxyBackEndHelper.TopologyWithSites legacyServiceTopologyWithSites = HttpProxyBackEndHelper.GetLegacyServiceTopologyWithSites(backEndServer.Fqdn);
			ServiceType serviceType = legacyServiceTopologyWithSites.ServiceTopology.FindAny<ServiceType>(ClientAccessType.InternalNLBBypass, (ServiceType service) => service != null && !service.IsFrontEnd && service.ServerFullyQualifiedDomainName.Equals(backEndServer.Fqdn, StringComparison.OrdinalIgnoreCase), "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetBackEndServiceUrlByServer", 568);
			if (serviceType != null)
			{
				ExTraceGlobals.CafeTracer.TraceDebug<BackEndServer>(0L, "[HttpProxyBackEndHelper.GetBackEndServiceUrlByServer] Found HTTP service on the specified back end server {0}.", backEndServer);
				return serviceType.Url;
			}
			serviceType = HttpProxyBackEndHelper.GetBestBackEndServiceForVersion<ServiceType>(legacyServiceTopologyWithSites, ClientAccessType.InternalNLBBypass, (int x) => new ServerVersion(x).Major == new ServerVersion(backEndServer.Version).Major);
			ExTraceGlobals.CafeTracer.TraceDebug<string, int>(0L, "[HttpProxyBackEndHelper.GetBackEndServiceByServer] Found HTTP service {0} with version {1}.", serviceType.ServerFullyQualifiedDomainName, serviceType.ServerVersionNumber);
			if (serviceType != null)
			{
				return serviceType.Url;
			}
			throw new ServerNotFoundException("Unable to find proper HTTP service.");
		}

		internal static bool TryGetBackEndWebServicesUrlFromSmtp(string smtpString, Func<SmtpAddress, IRecipientSession> createRecipientSession, out Uri ewsUri)
		{
			ewsUri = null;
			ProxyAddress proxyAddress;
			if (!ProxyAddress.TryParse(smtpString, out proxyAddress))
			{
				ExTraceGlobals.CafeTracer.TraceDebug<string>(0L, "[HttpProxyBackEndHelper.TryGetBackEndWebServicesUrlFromSmtp] The smtp address was invalid: {0}", smtpString);
				return false;
			}
			if (proxyAddress.Prefix != ProxyAddressPrefix.Smtp)
			{
				ExTraceGlobals.CafeTracer.TraceDebug<string>(0L, "[HttpProxyBackEndHelper.TryGetBackEndWebServicesUrlFromSmtp] Non-SMTP address is not supported: {0}", smtpString);
				return false;
			}
			SmtpAddress arg = new SmtpAddress(proxyAddress.AddressString);
			IRecipientSession session = createRecipientSession(arg);
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromProxyAddress(session, smtpString);
				ewsUri = BackEndLocator.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo);
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.CafeTracer.TraceDebug<string, string>(0L, "[HttpProxyBackEndHelper.TryGetBackEndWebServicesUrlFromSmtp] Couldn't find exchange principal for smtp address: {0}. Exception {1}", smtpString, ex.ToString());
				return false;
			}
			catch (BackEndLocatorException ex2)
			{
				ExTraceGlobals.CafeTracer.TraceDebug<string, string>(0L, "[HttpProxyBackEndHelper.TryGetBackEndWebServicesUrlFromSmtp] Couldn't find exchange services url for smtp address: {0}. Exception: {1}", smtpString, ex2.ToString());
				return false;
			}
			return true;
		}

		private static ADUser GetOrganizationMailbox(IRecipientSession recipientSession, OrganizationCapability capability, string anchorMailboxKey)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(recipientSession, capability);
			if (organizationMailboxesByCapability == null || organizationMailboxesByCapability.Count == 0)
			{
				ExTraceGlobals.CafeTracer.TraceError<OrganizationCapability, OrganizationId>(0L, "[HttpProxyBackEndHelper.GetOrganizationMailbox] Unable to find organization mailbox with capability {0} for organization {1}.", capability, recipientSession.SessionSettings.CurrentOrganizationId);
				return null;
			}
			ADUser aduser;
			if (!string.IsNullOrEmpty(anchorMailboxKey) && organizationMailboxesByCapability.Count > 1)
			{
				aduser = organizationMailboxesByCapability[HttpProxyBackEndHelper.ComputeIndex(anchorMailboxKey, organizationMailboxesByCapability.Count)];
			}
			else
			{
				aduser = organizationMailboxesByCapability[0];
			}
			ExTraceGlobals.CafeTracer.TraceDebug<ObjectId, OrganizationCapability, OrganizationId>(0L, "[HttpProxyBackEndHelper.GetOrganizationMailbox] Find organization mailbox {0} with capability {1} for organization {2}.", aduser.Identity, capability, recipientSession.SessionSettings.CurrentOrganizationId);
			return aduser;
		}

		private static ADUser GetOrganizationMailboxInClosestSite(IRecipientSession recipientSession, OrganizationCapability capability)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(recipientSession, capability);
			if (organizationMailboxesByCapability == null || organizationMailboxesByCapability.Count == 0)
			{
				ExTraceGlobals.CafeTracer.TraceError<OrganizationCapability, OrganizationId>(0L, "[HttpProxyBackEndHelper.GetOrganizationMailbox] Unable to find organization mailbox with capability {0} for organization {1}.", capability, recipientSession.SessionSettings.CurrentOrganizationId);
				return null;
			}
			ADUser aduser;
			if (organizationMailboxesByCapability.Count == 1)
			{
				aduser = organizationMailboxesByCapability[0];
			}
			else
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetOrganizationMailboxInClosestSite", 753);
				Site site = currentServiceTopology.GetSite(LocalServerCache.LocalServerFqdn, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetOrganizationMailboxInClosestSite", 754);
				List<ADUser> list = new List<ADUser>(5);
				int num = int.MaxValue;
				foreach (ADUser aduser2 in organizationMailboxesByCapability)
				{
					ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(recipientSession.SessionSettings, aduser2, RemotingOptions.AllowCrossSite);
					Site site2 = currentServiceTopology.GetSite(exchangePrincipal.MailboxInfo.Location.ServerFqdn, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetOrganizationMailboxInClosestSite", 761);
					if (list.Count == 0)
					{
						list.Add(aduser2);
						currentServiceTopology.TryGetConnectionCost(site, site2, out num, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetOrganizationMailboxInClosestSite", 768);
					}
					else
					{
						int maxValue = int.MaxValue;
						currentServiceTopology.TryGetConnectionCost(site, site2, out maxValue, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetOrganizationMailboxInClosestSite", 773);
						if (maxValue == num)
						{
							list.Add(aduser2);
						}
						else if (maxValue < num)
						{
							list.Clear();
							list.Add(aduser2);
							num = maxValue;
						}
					}
				}
				if (list.Count == 1)
				{
					aduser = list[0];
				}
				else
				{
					aduser = list[HttpProxyBackEndHelper.random.Next(list.Count)];
				}
			}
			ExTraceGlobals.CafeTracer.TraceDebug<ObjectId, OrganizationCapability, OrganizationId>(0L, "[HttpProxyBackEndHelper.GetOrganizationMailbox] Find organization mailbox {0} with capability {1} for organization {2}.", aduser.Identity, capability, recipientSession.SessionSettings.CurrentOrganizationId);
			return aduser;
		}

		private static ServiceType GetBestBackEndServiceForVersion<ServiceType>(HttpProxyBackEndHelper.TopologyWithSites topology, ClientAccessType clientAccessType, Predicate<int> versionNumberCondition) where ServiceType : HttpService
		{
			if (topology == null)
			{
				throw new ArgumentNullException("topology");
			}
			if (versionNumberCondition == null)
			{
				throw new ArgumentNullException("versionNumberCondition");
			}
			ServiceType serviceType = default(ServiceType);
			Site lookupSite = (topology.TargetSite != null) ? topology.TargetSite : topology.CurrentSite;
			if (lookupSite != null)
			{
				serviceType = topology.ServiceTopology.FindAny<ServiceType>(clientAccessType, (ServiceType service) => service != null && service.Site.Equals(lookupSite) && !service.IsOutOfService && versionNumberCondition(service.ServerVersionNumber), "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetBestBackEndServiceForVersion", 847);
				if (serviceType == null)
				{
					ExTraceGlobals.CafeTracer.TraceError<Site>(0L, "[HttpProxyBackEndHelper.GetBestBackEndServiceForVersion] Could not find server for site {0}", lookupSite);
				}
			}
			if (serviceType == null && topology.CurrentSite != null)
			{
				serviceType = topology.ServiceTopology.FindAny<ServiceType>(clientAccessType, (ServiceType service) => service != null && service.Site.Equals(topology.CurrentSite) && !service.IsOutOfService && versionNumberCondition(service.ServerVersionNumber), "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetBestBackEndServiceForVersion", 870);
				if (serviceType == null)
				{
					ExTraceGlobals.CafeTracer.TraceError<Site>(0L, "[HttpProxyBackEndHelper.GetBestBackEndServiceForVersion] Could not find server for current site {0}!", topology.CurrentSite);
				}
			}
			if (serviceType == null)
			{
				serviceType = topology.ServiceTopology.FindAny<ServiceType>(clientAccessType, (ServiceType service) => service != null && !service.IsOutOfService && versionNumberCondition(service.ServerVersionNumber), "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetBestBackEndServiceForVersion", 893);
				if (serviceType == null)
				{
					ExTraceGlobals.CafeTracer.TraceError(0L, "[HttpProxyBackEndHelper.GetBestBackEndServiceForVersion] Could not find any applicable CAS server!  (Last chance - returning null.");
				}
			}
			if (serviceType == null)
			{
				throw new ServerNotFoundException("Unable to find proper HTTP service.");
			}
			return serviceType;
		}

		private static ServiceType[] FindAcceptableBackEndService<ServiceType>(HttpProxyBackEndHelper.TopologyWithSites topology, ClientAccessType clientAccessType, Predicate<int> versionNumberCondition) where ServiceType : HttpService
		{
			if (topology == null)
			{
				throw new ArgumentNullException("topology");
			}
			if (versionNumberCondition == null)
			{
				throw new ArgumentNullException("versionNumberCondition");
			}
			List<ServiceType> tempServices = new List<ServiceType>();
			Site lookupSite = (topology.TargetSite != null) ? topology.TargetSite : topology.CurrentSite;
			if (lookupSite != null)
			{
				topology.ServiceTopology.ForEach<ServiceType>(delegate(ServiceType service)
				{
					if ((service.ServerRole & ServerRole.Mailbox) == ServerRole.Mailbox && service.ClientAccessType == clientAccessType && !service.IsOutOfService && service.Site.Equals(lookupSite) && versionNumberCondition(service.ServerVersionNumber))
					{
						tempServices.Add(service);
					}
				}, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "FindAcceptableBackEndService", 948);
				if (tempServices.Count == 0)
				{
					ExTraceGlobals.CafeTracer.TraceError<Site>(0L, "[HttpProxyBackEndHelper.FindAcceptableBackEndService] Could not find BE server for site {0}", lookupSite);
				}
			}
			if (tempServices.Count == 0 && topology.CurrentSite != null)
			{
				topology.ServiceTopology.ForEach<ServiceType>(delegate(ServiceType service)
				{
					if ((service.ServerRole & ServerRole.Mailbox) == ServerRole.Mailbox && service.ClientAccessType == clientAccessType && !service.IsOutOfService && service.Site.Equals(topology.CurrentSite) && versionNumberCondition(service.ServerVersionNumber))
					{
						tempServices.Add(service);
					}
				}, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "FindAcceptableBackEndService", 974);
				if (tempServices.Count == 0)
				{
					ExTraceGlobals.CafeTracer.TraceError<Site>(0L, "[HttpProxyBackEndHelper.FindAcceptableBackEndService] Could not find BE server for current site {0}", topology.CurrentSite);
				}
			}
			if (tempServices.Count == 0)
			{
				topology.ServiceTopology.ForEach<ServiceType>(delegate(ServiceType service)
				{
					if ((service.ServerRole & ServerRole.Mailbox) == ServerRole.Mailbox && service.ClientAccessType == clientAccessType && !service.IsOutOfService && versionNumberCondition(service.ServerVersionNumber))
					{
						tempServices.Add(service);
					}
				}, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "FindAcceptableBackEndService", 999);
				if (tempServices.Count == 0)
				{
					ExTraceGlobals.CafeTracer.TraceError(0L, "[HttpProxyBackEndHelper.FindAcceptableBackEndService] Could not find any applicable BE server");
				}
			}
			return tempServices.ToArray();
		}

		private static HttpProxyBackEndHelper.TopologyWithSites GetLegacyServiceTopologyWithSites(string serverFqdn)
		{
			ServiceTopology currentLegacyServiceTopology = ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetLegacyServiceTopologyWithSites", 1028);
			return HttpProxyBackEndHelper.GetServiceTopologyWithSites(serverFqdn, currentLegacyServiceTopology);
		}

		private static HttpProxyBackEndHelper.TopologyWithSites GetCompleteServiceTopologyWithSites(string serverFqdn)
		{
			ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetCompleteServiceTopologyWithSites", 1040);
			return HttpProxyBackEndHelper.GetServiceTopologyWithSites(serverFqdn, currentServiceTopology);
		}

		private static HttpProxyBackEndHelper.TopologyWithSites GetServiceTopologyWithSites(string serverFqdn, ServiceTopology topology)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			HttpProxyBackEndHelper.TopologyWithSites topologyWithSites = new HttpProxyBackEndHelper.TopologyWithSites
			{
				ServiceTopology = topology
			};
			try
			{
				topologyWithSites.TargetSite = topology.GetSite(serverFqdn, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\HttpProxyBackEndHelper.cs", "GetServiceTopologyWithSites", 1065);
			}
			catch (ServerNotInSiteException arg)
			{
				ExTraceGlobals.CafeTracer.TraceError<string, ServerNotInSiteException>(0L, "[HttpProxyBackEndHelper.GetServiceTopologyWithSites] Could not find site for server {0} - ServerNotInSiteException {1}", serverFqdn, arg);
			}
			catch (ServerNotFoundException arg2)
			{
				ExTraceGlobals.CafeTracer.TraceError<string, ServerNotFoundException>(0L, "[HttpProxyBackEndHelper.GetServiceTopologyWithSites] Could not find site for server {0} - ServerNotFoundException {1}", serverFqdn, arg2);
			}
			try
			{
				topologyWithSites.CurrentSite = HttpProxyBackEndHelper.localSite.Member;
			}
			catch (CannotGetSiteInfoException arg3)
			{
				ExTraceGlobals.CafeTracer.TraceError<string, CannotGetSiteInfoException>(0L, "[HttpProxyBackEndHelper.GetServiceTopologyWithSites] Could not find site for current server {0} - CannotGetSiteInfoException {1}", LocalServerCache.LocalServerFqdn, arg3);
			}
			return topologyWithSites;
		}

		internal static readonly OrganizationCapability DefaultOrganizationCapability = OrganizationCapability.UMGrammar;

		internal static readonly string E15OrganizationMailbox = "SystemMailbox{bb558c35-97f1-4cb9-8ff7-d53741dc928c}";

		internal static readonly string E14EDiscoveryMailbox = "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}";

		private static readonly QueryFilter E15OrganizationMailboxFilter = new AndFilter(new QueryFilter[]
		{
			new TextFilter(ADObjectSchema.Name, HttpProxyBackEndHelper.E15OrganizationMailbox, MatchOptions.FullString, MatchFlags.IgnoreCase),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
		});

		private static readonly QueryFilter E14EDiscoveryMailboxFilter = new AndFilter(new QueryFilter[]
		{
			new TextFilter(ADObjectSchema.Name, HttpProxyBackEndHelper.E14EDiscoveryMailbox, MatchOptions.FullString, MatchFlags.IgnoreCase),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
		});

		private static readonly Random random = new Random();

		private static LazyMember<bool> isPartnerHostedOnly = new LazyMember<bool>(delegate()
		{
			try
			{
				if (Datacenter.IsPartnerHostedOnly(true))
				{
					return true;
				}
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			return false;
		});

		private static readonly LazyMember<Site> localSite = new LazyMember<Site>(() => new Site(new TopologySite(LocalSiteCache.LocalSite)));

		private class TopologyWithSites
		{
			public ServiceTopology ServiceTopology { get; set; }

			public Site CurrentSite { get; set; }

			public Site TargetSite { get; set; }
		}
	}
}
