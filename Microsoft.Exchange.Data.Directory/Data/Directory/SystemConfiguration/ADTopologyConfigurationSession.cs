using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.Diagnostics;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ADTopologyConfigurationSession : ADConfigurationSession, ITopologyConfigurationSession, IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		private static QueryFilter FqdnFilterForServer(string serverFqdn)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.NetworkAddress, "ncacn_ip_tcp:" + serverFqdn);
		}

		public ADTopologyConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, true, consistencyMode, null, sessionSettings)
		{
		}

		public ADTopologyConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, null, sessionSettings)
		{
		}

		public ADTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.DomainController = domainController;
		}

		public ADTopologyConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope) : this(domainController, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			if (ConfigScopes.TenantSubTree != configScope)
			{
				throw new NotSupportedException("Only ConfigScopes.TenantSubTree is supported by this constructor");
			}
			if (ConfigScopes.TenantSubTree == configScope)
			{
				base.ConfigScope = configScope;
			}
		}

		private static bool IsValidTrustedHoster(string item)
		{
			if (item.StartsWith("*.", StringComparison.OrdinalIgnoreCase))
			{
				string domain = item.Substring(2);
				if (SmtpAddress.IsValidDomain(domain))
				{
					return true;
				}
			}
			else if (SmtpAddress.IsValidDomain(item))
			{
				return true;
			}
			return false;
		}

		private static void ReadMultipleADLegacyObjectsHashInserter<TResult>(Hashtable hash, TResult entry) where TResult : ADLegacyVersionableObject
		{
			string key = ((string)entry.propertyBag[ADObjectSchema.Name]).ToLowerInvariant();
			if (!hash.ContainsKey(key))
			{
				hash[key] = new Result<TResult>(entry, null);
			}
			key = ((string)entry.propertyBag[ADObjectSchema.DistinguishedName]).ToLowerInvariant();
			if (!hash.ContainsKey(key))
			{
				hash[key] = new Result<TResult>(entry, null);
			}
		}

		private static Result<T> ReadMultipleADLegacyObjectsHashLookup<T>(Hashtable hash, string key) where T : ADLegacyVersionableObject
		{
			if (!string.IsNullOrEmpty(key))
			{
				object obj = hash[key.ToLowerInvariant()];
				if (obj != null)
				{
					return (Result<T>)obj;
				}
			}
			return new Result<T>(default(T), ProviderError.NotFound);
		}

		private static QueryFilter ReadMultipleADLegacyObjectsQueryFilterFromObjectName(string nameOrDN)
		{
			if (nameOrDN == null)
			{
				throw new ArgumentNullException("nameOrDN");
			}
			return new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, nameOrDN),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, nameOrDN)
			});
		}

		public ADCrossRef[] FindADCrossRefByDomainId(ADObjectId domainNc)
		{
			return this.InvokeWithAPILogging<ADCrossRef[]>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADCrossRefSchema.NCName, domainNc);
				return this.InternalFind<ADCrossRef>(null, QueryScope.SubTree, filter, null, 0, null);
			}, "FindADCrossRefByDomainId");
		}

		public ADCrossRef[] FindADCrossRefByNetBiosName(string domain)
		{
			return this.InvokeWithAPILogging<ADCrossRef[]>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADCrossRefSchema.NetBiosName, domain);
				return this.InternalFind<ADCrossRef>(null, QueryScope.SubTree, filter, null, 0, null);
			}, "FindADCrossRefByNetBiosName");
		}

		public AccountPartition[] FindAllAccountPartitions()
		{
			return this.InvokeWithAPILogging<AccountPartition[]>(delegate
			{
				ADPagedReader<AccountPartition> adpagedReader = base.FindPaged<AccountPartition>(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().GetChildId(AccountPartition.AccountForestContainerName), QueryScope.OneLevel, null, null, 0);
				return adpagedReader.ReadAllPages();
			}, "FindAllAccountPartitions");
		}

		public ADSite[] FindAllADSites()
		{
			return this.InvokeWithAPILogging<ADSite[]>(delegate
			{
				ADPagedReader<ADSite> adpagedReader = base.FindPaged<ADSite>(base.ConfigurationNamingContext, QueryScope.SubTree, null, null, 0);
				return adpagedReader.ReadAllPages();
			}, "FindAllADSites");
		}

		public IList<PublicFolderDatabase> FindAllPublicFolderDatabaseOfCurrentVersion()
		{
			return this.InvokeWithAPILogging<IList<PublicFolderDatabase>>(delegate
			{
				PublicFolderDatabase[] array = base.Find<PublicFolderDatabase>(base.GetOrgContainerId(), QueryScope.SubTree, null, null, 10000);
				IList<PublicFolderDatabase> list = new List<PublicFolderDatabase>(array.Length);
				if (array.Length != 0)
				{
					QueryFilter[] array2 = new QueryFilter[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, array[i].Server);
					}
					QueryFilter filter = new OrFilter(array2);
					Server[] array3 = base.Find<Server>(base.GetOrgContainerId(), QueryScope.SubTree, filter, null, 10000);
					foreach (Server server in array3)
					{
						if (server.MajorVersion == 15)
						{
							foreach (PublicFolderDatabase publicFolderDatabase in array)
							{
								if (server.Id.Equals(publicFolderDatabase.Server))
								{
									list.Add(publicFolderDatabase);
									break;
								}
							}
						}
					}
				}
				return list;
			}, "FindAllPublicFolderDatabaseOfCurrentVersion");
		}

		public ADPagedReader<Server> FindAllServersWithExactVersionNumber(int versionNumber)
		{
			return this.InvokeWithAPILogging<ADPagedReader<Server>>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.VersionNumber, versionNumber);
				return this.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
			}, "FindAllServersWithExactVersionNumber");
		}

		public ADPagedReader<Server> FindAllServersWithVersionNumber(int versionNumber)
		{
			return this.InvokeWithAPILogging<ADPagedReader<Server>>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, versionNumber);
				return this.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
			}, "FindAllServersWithVersionNumber");
		}

		public ADPagedReader<MiniServer> FindAllServersWithExactVersionNumber(int versionNumber, QueryFilter additionalFilter, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<ADPagedReader<MiniServer>>(delegate
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.VersionNumber, versionNumber);
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					additionalFilter
				});
				return this.FindPaged<MiniServer>(null, QueryScope.SubTree, filter, null, 0, properties);
			}, "FindAllServersWithExactVersionNumber");
		}

		public ADPagedReader<MiniServer> FindAllServersWithVersionNumber(int versionNumber, QueryFilter additionalFilter, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<ADPagedReader<MiniServer>>(delegate
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, versionNumber);
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					additionalFilter
				});
				return this.FindPaged<MiniServer>(null, QueryScope.SubTree, filter, null, 0, properties);
			}, "FindAllServersWithVersionNumber");
		}

		public CmdletExtensionAgent[] FindCmdletExtensionAgents(bool enabledOnly, bool sortByPriority)
		{
			return this.InvokeWithAPILogging<CmdletExtensionAgent[]>(delegate
			{
				QueryFilter filter = null;
				if (enabledOnly)
				{
					filter = new BitMaskAndFilter(CmdletExtensionAgentSchema.CmdletExtensionFlags, 1UL);
				}
				ADPagedReader<CmdletExtensionAgent> adpagedReader = this.FindPaged<CmdletExtensionAgent>(null, QueryScope.SubTree, filter, null, 0);
				List<CmdletExtensionAgent> list = new List<CmdletExtensionAgent>();
				foreach (CmdletExtensionAgent item in adpagedReader)
				{
					list.Add(item);
				}
				if (sortByPriority)
				{
					list.Sort();
				}
				return list.ToArray();
			}, "FindCmdletExtensionAgents");
		}

		public ADComputer FindComputerByHostName(string hostName)
		{
			return this.FindComputerByHostName(null, hostName);
		}

		public ADComputer FindComputerByHostName(ADObjectId domainId, string hostName)
		{
			return this.InvokeWithAPILogging<ADComputer>(delegate
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADComputerSchema.ServicePrincipalName, "HOST/" + hostName),
					new NotFilter(new BitMaskOrFilter(ADComputerSchema.UserAccountControl, 2UL))
				});
				ADComputer[] array = this.Find<ADComputer>(domainId, QueryScope.SubTree, filter, null, 2);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				if (array.Length > 1)
				{
					throw new ADOperationException(DirectoryStrings.HostNameMatchesMultipleComputers(hostName, array[0].DistinguishedName, array[1].DistinguishedName));
				}
				return array[0];
			}, "FindComputerByHostName");
		}

		public ADComputer FindComputerBySid(SecurityIdentifier sid)
		{
			return this.InvokeWithAPILogging<ADComputer>(delegate
			{
				if (sid.IsWellKnown(WellKnownSidType.LocalSystemSid) || sid.IsWellKnown(WellKnownSidType.NetworkServiceSid))
				{
					ExTraceGlobals.ADTopologyTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "The Sid {0} is LocalSystemSid or NetworkServiceSid, finding local computer without using Sid", sid);
					return this.FindLocalComputer();
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADComputerSchema.Sid, sid);
				ADComputer[] array = this.Find<ADComputer>(null, QueryScope.SubTree, filter, null, 2);
				if (array == null)
				{
					return null;
				}
				switch (array.Length)
				{
				case 0:
					return null;
				case 1:
					return array[0];
				default:
					throw new ADOperationException(DirectoryStrings.ErrorNonUniqueSid(sid.ToString()));
				}
			}, "FindComputerBySid");
		}

		public TDatabase FindDatabaseByGuid<TDatabase>(Guid dbGuid) where TDatabase : Database, new()
		{
			return this.InvokeWithAPILogging<TDatabase>(delegate
			{
				if (dbGuid == Guid.Empty)
				{
					throw new ArgumentException("dbGuid cannot be Empty.");
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, dbGuid);
				TDatabase[] array = this.Find<TDatabase>(null, QueryScope.SubTree, filter, null, 1);
				if (array == null || array.Length <= 0)
				{
					return default(TDatabase);
				}
				return array[0];
			}, "FindDatabaseByGuid");
		}

		public TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new()
		{
			return this.InvokeWithAPILogging<TResult[]>(() => this.InternalFind<TResult>(rootId, scope, filter, sortBy, maxResults, properties), "Find");
		}

		public ADServer FindDCByFqdn(string dnsHostName)
		{
			return this.InvokeWithAPILogging<ADServer>(delegate
			{
				if (string.IsNullOrEmpty(dnsHostName))
				{
					throw new ArgumentNullException("dnsHostName");
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADServerSchema.DnsHostName, dnsHostName);
				ADServer[] array = this.Find<ADServer>(this.GetSitesContainer().Id, QueryScope.SubTree, filter, null, 1);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindDCByFqdn");
		}

		public ADServer FindDCByInvocationId(Guid invocationId)
		{
			return this.InvokeWithAPILogging<ADServer>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, NtdsDsaSchema.InvocationId, invocationId);
				NtdsDsa[] array = this.Find<NtdsDsa>(null, QueryScope.SubTree, filter, null, 1);
				if (array != null && array.Length > 0)
				{
					return this.Read<ADServer>(array[0].Id.Parent);
				}
				return null;
			}, "FindDCByInvocationId");
		}

		public UMDialPlan[] FindDialPlansForServer(Server server)
		{
			return this.InvokeWithAPILogging<UMDialPlan[]>(delegate
			{
				if (server == null)
				{
					throw new ArgumentNullException("server");
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, UMDialPlanSchema.UMServers, server.Id);
				ADPagedReader<UMDialPlan> adpagedReader = this.FindPaged<UMDialPlan>(null, QueryScope.SubTree, filter, null, 0);
				List<UMDialPlan> list = new List<UMDialPlan>();
				foreach (UMDialPlan item in adpagedReader)
				{
					list.Add(item);
				}
				return list.ToArray();
			}, "FindDialPlansForServer");
		}

		public ELCFolder FindElcFolderByName(string name)
		{
			return this.InvokeWithAPILogging<ELCFolder>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, name);
				ELCFolder[] array = this.Find<ELCFolder>(null, QueryScope.SubTree, filter, null, 2);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindElcFolderByName");
		}

		public ADComputer FindLocalComputer()
		{
			return this.InvokeWithAPILogging<ADComputer>(delegate
			{
				string hostName = Dns.GetHostEntry("localhost").HostName;
				return this.FindComputerByHostName(null, hostName);
			}, "FindLocalComputer");
		}

		public Server FindLocalServer()
		{
			return this.InvokeWithAPILogging<Server>(delegate
			{
				string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(true);
				Server server = this.FindServerByFqdn(localComputerFqdn);
				if (server != null)
				{
					return server;
				}
				throw new LocalServerNotFoundException(localComputerFqdn);
			}, "FindLocalServer");
		}

		public MailboxDatabase FindMailboxDatabaseByNameAndServer(string databaseName, Server server)
		{
			return this.InvokeWithAPILogging<MailboxDatabase>(delegate
			{
				int num = -1;
				MailboxDatabase[] mailboxDatabases = server.GetMailboxDatabases();
				if (mailboxDatabases != null)
				{
					for (int i = 0; i < mailboxDatabases.Length; i++)
					{
						if (string.Compare(mailboxDatabases[i].Name, databaseName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							num = i;
							break;
						}
					}
				}
				if (num <= -1)
				{
					return null;
				}
				return mailboxDatabases[num];
			}, "FindMailboxDatabaseByNameAndServer");
		}

		public MesoContainer FindMesoContainer(ADDomain dom)
		{
			return this.InvokeWithAPILogging<MesoContainer>(delegate
			{
				string domainController = this.DomainController;
				MesoContainer result;
				try
				{
					this.DomainController = null;
					MesoContainer[] array = this.Find<MesoContainer>(dom.Id, QueryScope.OneLevel, null, null, 1);
					if (array == null || array.Length == 0)
					{
						result = null;
					}
					else
					{
						result = array[0];
					}
				}
				finally
				{
					this.DomainController = domainController;
				}
				return result;
			}, "FindMesoContainer");
		}

		public MiniClientAccessServerOrArray[] FindMiniClientAccessServerOrArray(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<MiniClientAccessServerOrArray[]>(() => this.InternalFind<MiniClientAccessServerOrArray>(rootId, scope, filter, sortBy, maxResults, properties), "FindMiniClientAccessServerOrArray");
		}

		public MiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByFqdn(string serverFqdn, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<MiniClientAccessServerOrArray>(delegate
			{
				if (string.IsNullOrEmpty(serverFqdn))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				QueryFilter filter = ADTopologyConfigurationSession.FqdnFilterForServer(serverFqdn);
				MiniClientAccessServerOrArray[] array = this.FindMiniClientAccessServerOrArray(null, QueryScope.SubTree, filter, null, 2, properties);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindMiniClientAccessServerOrArrayByFqdn");
		}

		public MiniServer[] FindMiniServer(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<MiniServer[]>(() => this.InternalFind<MiniServer>(rootId, scope, filter, sortBy, maxResults, properties), "FindMiniServer");
		}

		public MiniServer FindMiniServerByFqdn(string serverFqdn, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<MiniServer>(delegate
			{
				if (string.IsNullOrEmpty(serverFqdn))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				QueryFilter filter = ADTopologyConfigurationSession.FqdnFilterForServer(serverFqdn);
				MiniServer[] array = this.FindMiniServer(null, QueryScope.SubTree, filter, null, 2, properties);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindMiniServerByFqdn");
		}

		public MiniServer FindMiniServerByName(string serverName, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<MiniServer>(delegate
			{
				if (string.IsNullOrEmpty(serverName))
				{
					throw new ArgumentNullException("serverName");
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serverName);
				MiniServer[] array = this.FindMiniServer(null, QueryScope.SubTree, filter, null, 2, properties);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindMiniServerByName");
		}

		public ADOabVirtualDirectory[] FindOABVirtualDirectoriesForLocalServer()
		{
			return this.InvokeWithAPILogging<ADOabVirtualDirectory[]>(() => this.FindVirtualDirectoriesForLocalServer<ADOabVirtualDirectory>(), "FindOABVirtualDirectoriesForLocalServer");
		}

		public ADOwaVirtualDirectory[] FindOWAVirtualDirectoriesForLocalServer()
		{
			return this.InvokeWithAPILogging<ADOwaVirtualDirectory[]>(() => this.FindVirtualDirectoriesForLocalServer<ADOwaVirtualDirectory>(), "FindOWAVirtualDirectoriesForLocalServer");
		}

		public ADO365SuiteServiceVirtualDirectory[] FindO365SuiteServiceVirtualDirectoriesForLocalServer()
		{
			return this.FindVirtualDirectoriesForLocalServer<ADO365SuiteServiceVirtualDirectory>();
		}

		public ADSnackyServiceVirtualDirectory[] FindSnackyServiceVirtualDirectoriesForLocalServer()
		{
			return this.FindVirtualDirectoriesForLocalServer<ADSnackyServiceVirtualDirectory>();
		}

		public MiniVirtualDirectory[] FindMiniVirtualDirectories(ADObjectId serverId)
		{
			return this.InvokeWithAPILogging<MiniVirtualDirectory[]>(delegate
			{
				ArgumentValidator.ThrowIfNull("serverId", serverId);
				return this.Find<MiniVirtualDirectory>(serverId, QueryScope.SubTree, null, null, 0);
			}, "FindMiniVirtualDirectories");
		}

		public ADPagedReader<MiniServer> FindPagedMiniServer(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<ADPagedReader<MiniServer>>(() => this.FindPaged<MiniServer>(rootId, scope, filter, sortBy, pageSize, properties), "FindPagedMiniServer");
		}

		public MiniServer FindMiniServerByFqdn(string serverFqdn)
		{
			return this.InvokeWithAPILogging<MiniServer>(delegate
			{
				if (string.IsNullOrEmpty(serverFqdn))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				QueryFilter filter = ADTopologyConfigurationSession.FqdnFilterForServer(serverFqdn);
				MiniServer[] array = this.Find<MiniServer>(null, QueryScope.SubTree, filter, null, 1);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindMiniServerByFqdn");
		}

		public Server FindServerByFqdn(string serverFqdn)
		{
			return this.InvokeWithAPILogging<Server>(delegate
			{
				if (string.IsNullOrEmpty(serverFqdn))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				QueryFilter filter = ADTopologyConfigurationSession.FqdnFilterForServer(serverFqdn);
				Server[] array = this.Find<Server>(null, QueryScope.SubTree, filter, null, 2);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindServerByFqdn");
		}

		public Server FindServerByLegacyDN(string legacyExchangeDN)
		{
			return this.InvokeWithAPILogging<Server>(() => (from result in this.FindByExchangeLegacyDNs<Server>(new string[]
			{
				legacyExchangeDN
			}, null)
			select result.Data).Single<Server>(), "FindServerByLegacyDN");
		}

		public Server FindServerByName(string serverName)
		{
			return this.InvokeWithAPILogging<Server>(delegate
			{
				if (string.IsNullOrEmpty(serverName))
				{
					throw new ArgumentNullException("serverName");
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serverName);
				Server[] array = this.Find<Server>(null, QueryScope.SubTree, filter, null, 2);
				if (array == null || array.Length <= 0)
				{
					return null;
				}
				return array[0];
			}, "FindServerByName");
		}

		public ReadOnlyCollection<ADServer> FindServerWithNtdsdsa(string domainDN, bool gcOnly, bool includingRodc)
		{
			return this.InvokeWithAPILogging<ReadOnlyCollection<ADServer>>(delegate
			{
				ADObjectId id = this.GetSitesContainer().Id;
				QueryFilter queryFilter = null;
				if (!string.IsNullOrEmpty(domainDN))
				{
					queryFilter = new ComparisonFilter(ComparisonOperator.Equal, NtdsDsaSchema.MasterNCs, domainDN);
					if (includingRodc)
					{
						queryFilter = new OrFilter(new QueryFilter[]
						{
							queryFilter,
							new ComparisonFilter(ComparisonOperator.Equal, NtdsDsaSchema.FullReplicaNCs, domainDN)
						});
					}
				}
				if (gcOnly)
				{
					QueryFilter queryFilter2 = new BitMaskAndFilter(NtdsDsaSchema.Options, 1UL);
					queryFilter = ((queryFilter == null) ? queryFilter2 : new AndFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					}));
				}
				if (!includingRodc)
				{
					QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, NtdsDsaSchema.DsIsRodc, false);
					queryFilter = ((queryFilter == null) ? queryFilter3 : new AndFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter3
					}));
				}
				ADPagedReader<NtdsDsa> adpagedReader = this.FindPaged<NtdsDsa>(id, QueryScope.SubTree, queryFilter, null, 0);
				Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				foreach (NtdsDsa ntdsDsa in adpagedReader)
				{
					ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Found matching ntdsdsa under {0}", ntdsDsa.Id.Parent.Name);
					string distinguishedName = ntdsDsa.Id.Parent.DistinguishedName;
					dictionary[distinguishedName] = distinguishedName;
				}
				QueryFilter filter = new ExistsFilter(ADServerSchema.DnsHostName);
				ADPagedReader<ADServer> adpagedReader2 = this.FindPaged<ADServer>(id, QueryScope.SubTree, filter, null, 0);
				Dictionary<string, ADServer> dictionary2 = new Dictionary<string, ADServer>(StringComparer.OrdinalIgnoreCase);
				foreach (ADServer adserver in adpagedReader2)
				{
					if (dictionary.ContainsKey(adserver.DistinguishedName))
					{
						ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Found AD server {0} that has matching ntdsdsa object", adserver.DnsHostName);
						try
						{
							dictionary2.Add(adserver.DnsHostName, adserver);
							continue;
						}
						catch (ArgumentException)
						{
							ADServer adserver2 = null;
							if (!dictionary2.TryGetValue(adserver.DnsHostName, out adserver2))
							{
								throw;
							}
							DateTime? whenCreated = adserver2.WhenCreated;
							DateTime? whenCreated2 = adserver.WhenCreated;
							if (whenCreated != null && whenCreated2 != null && whenCreated.Value < whenCreated2.Value)
							{
								dictionary2[adserver.DnsHostName] = adserver;
								Globals.LogExchangeTopologyEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DUPLICATED_SERVER, adserver.DistinguishedName, new object[]
								{
									adserver.DistinguishedName,
									adserver2.DistinguishedName
								});
							}
							else
							{
								Globals.LogExchangeTopologyEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DUPLICATED_SERVER, adserver.DistinguishedName, new object[]
								{
									adserver.DistinguishedName,
									adserver2.DistinguishedName
								});
							}
							continue;
						}
					}
					ExTraceGlobals.ADTopologyTracer.TraceWarning<string>((long)this.GetHashCode(), "Found non-DC AD server or an AD server that doesn't match with ntdsdsa object {0}", adserver.Name);
				}
				List<ADServer> list = new List<ADServer>(dictionary2.Values);
				return new ReadOnlyCollection<ADServer>(list);
			}, "FindServerWithNtdsdsa");
		}

		public TResult FindUnique<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter) where TResult : ADConfigurationObject, new()
		{
			return this.InvokeWithAPILogging<TResult>(delegate
			{
				TResult[] array = this.InternalFind<TResult>(rootId, scope, filter, null, 2, null);
				if (array.Length == 1)
				{
					return array[0];
				}
				if (array.Length == 2)
				{
					QueryFilter[] array2 = new QueryFilter[2];
					QueryFilter[] array3 = array2;
					int num = 0;
					TResult tresult = Activator.CreateInstance<TResult>();
					array3[num] = tresult.ImplicitFilter;
					array2[1] = filter;
					throw new ADResultsNotUniqueException(QueryFilter.AndTogether(array2).ToString());
				}
				return default(TResult);
			}, "FindUnique");
		}

		public TPolicy[] FindWorkloadManagementChildPolicies<TPolicy>(ADObjectId wlmPolicy) where TPolicy : ADConfigurationObject, new()
		{
			return this.InvokeWithAPILogging<TPolicy[]>(() => this.FindWorkloadManagementChildPolicies<TPolicy>(wlmPolicy, null), "FindWorkloadManagementChildPolicies");
		}

		public AdministrativeGroup GetAdministrativeGroup()
		{
			return this.InvokeWithAPILogging<AdministrativeGroup>(delegate
			{
				AdministrativeGroup[] array = base.Find<AdministrativeGroup>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, AdministrativeGroup.DefaultName), null, 1);
				if (array == null || array.Length == 0)
				{
					throw new DefaultAdministrativeGroupNotFoundException(AdministrativeGroup.DefaultName);
				}
				return array[0];
			}, "GetAdministrativeGroup");
		}

		public ADObjectId GetAdministrativeGroupId()
		{
			return this.InvokeWithAPILogging<ADObjectId>(delegate
			{
				if (this.adminGroupId == null)
				{
					AdministrativeGroup administrativeGroup = this.GetAdministrativeGroup();
					this.adminGroupId = administrativeGroup.Id;
				}
				return this.adminGroupId;
			}, "GetAdministrativeGroupId");
		}

		public ADPagedReader<ExtendedRight> GetAllExtendedRights()
		{
			return this.InvokeWithAPILogging<ADPagedReader<ExtendedRight>>(() => base.FindPaged<ExtendedRight>(base.ConfigurationNamingContext.GetChildId("Extended-Rights"), QueryScope.OneLevel, null, null, 0), "GetAllExtendedRights");
		}

		public ADObjectId GetAutoDiscoverGlobalContainerId()
		{
			return this.InvokeWithAPILogging<ADObjectId>(delegate
			{
				ADObjectId childId = base.ConfigurationNamingContext.GetChildId("Services");
				return childId.GetChildId("Microsoft Exchange Autodiscover");
			}, "GetAutoDiscoverGlobalContainerId");
		}

		public string[] GetAutodiscoverTrustedHosters()
		{
			return this.InvokeWithAPILogging<string[]>(delegate
			{
				ADServiceConnectionPoint[] array = base.Find<ADServiceConnectionPoint>(this.GetAutoDiscoverGlobalContainerId(), QueryScope.SubTree, ExchangeScpObjects.AutodiscoverTrustedHosterKeyword.Filter, null, 0);
				if (array == null || array.Length == 0)
				{
					return null;
				}
				List<string> list = new List<string>(array.Length);
				foreach (ADServiceConnectionPoint adserviceConnectionPoint in array)
				{
					if (adserviceConnectionPoint.ServiceBindingInformation != null && adserviceConnectionPoint.ServiceBindingInformation.Count > 0)
					{
						foreach (string text in adserviceConnectionPoint.ServiceBindingInformation)
						{
							if (ADTopologyConfigurationSession.IsValidTrustedHoster(text))
							{
								list.Add(text);
							}
							else
							{
								ExTraceGlobals.ClientThrottlingTracer.TraceError<string>((long)this.GetHashCode(), "[ADTopologyConfigurationSession::GetAutodiscoverTrustedHosters] Ignoring invalid trusted hoster value '{0}'.", text);
							}
						}
					}
				}
				if (list.Count == 0)
				{
					return null;
				}
				return list.ToArray();
			}, "GetAutodiscoverTrustedHosters");
		}

		public ADObjectId GetClientAccessContainerId()
		{
			return this.InvokeWithAPILogging<ADObjectId>(() => base.GetOrgContainerId().GetDescendantId(new ADObjectId("CN=Client Access")), "GetClientAccessContainerId");
		}

		public DatabaseAvailabilityGroupContainer GetDatabaseAvailabilityGroupContainer()
		{
			return this.InvokeWithAPILogging<DatabaseAvailabilityGroupContainer>(delegate
			{
				DatabaseAvailabilityGroupContainer[] array = base.Find<DatabaseAvailabilityGroupContainer>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, DatabaseAvailabilityGroupContainer.DefaultName), null, 1);
				if (array == null || array.Length == 0)
				{
					throw new DefaultDatabaseAvailabilityGroupContainerNotFoundException(DatabaseAvailabilityGroupContainer.DefaultName);
				}
				return array[0];
			}, "GetDatabaseAvailabilityGroupContainer");
		}

		public ADObjectId GetDatabaseAvailabilityGroupContainerId()
		{
			return this.InvokeWithAPILogging<ADObjectId>(delegate
			{
				if (this.databaseAvailabilityGroupContainerId == null)
				{
					DatabaseAvailabilityGroupContainer databaseAvailabilityGroupContainer = this.GetDatabaseAvailabilityGroupContainer();
					this.databaseAvailabilityGroupContainerId = databaseAvailabilityGroupContainer.Id;
				}
				return this.databaseAvailabilityGroupContainerId;
			}, "GetDatabaseAvailabilityGroupContainerId");
		}

		public DatabasesContainer GetDatabasesContainer()
		{
			return this.InvokeWithAPILogging<DatabasesContainer>(delegate
			{
				DatabasesContainer[] array = base.Find<DatabasesContainer>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, DatabasesContainer.DefaultName), null, 1);
				if (array == null || array.Length == 0)
				{
					throw new DefaultDatabaseContainerNotFoundException(DatabasesContainer.DefaultName);
				}
				return array[0];
			}, "GetDatabasesContainer");
		}

		public ADObjectId GetDatabasesContainerId()
		{
			return this.InvokeWithAPILogging<ADObjectId>(delegate
			{
				if (this.databasesContainerId == null)
				{
					DatabasesContainer databasesContainer = this.GetDatabasesContainer();
					this.databasesContainerId = databasesContainer.Id;
				}
				return this.databasesContainerId;
			}, "GetDatabasesContainerId");
		}

		public ServiceEndpointContainer GetEndpointContainer()
		{
			return this.InvokeWithAPILogging<ServiceEndpointContainer>(delegate
			{
				ServiceEndpointContainer[] array = base.Find<ServiceEndpointContainer>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, ServiceEndpointContainer.DefaultName), null, 1);
				if (array == null || array.Length == 0)
				{
					throw new EndpointContainerNotFoundException(ServiceEndpointContainer.DefaultName);
				}
				return array[0];
			}, "GetEndpointContainer");
		}

		public ThrottlingPolicy GetGlobalThrottlingPolicy()
		{
			return this.InvokeWithAPILogging<ThrottlingPolicy>(() => this.InteralGetGlobalThrottlingPolicy(false), "GetGlobalThrottlingPolicy");
		}

		public ThrottlingPolicy GetGlobalThrottlingPolicy(bool throwError)
		{
			return this.InvokeWithAPILogging<ThrottlingPolicy>(() => this.InteralGetGlobalThrottlingPolicy(throwError), "GetGlobalThrottlingPolicy");
		}

		public Guid GetInvocationIdByDC(ADServer dc)
		{
			return this.InvokeWithAPILogging<Guid>(() => this.InternalGetInvocationIdByDC(dc), "GetInvocationIdByDC");
		}

		public Guid GetInvocationIdByFqdn(string serverFqdn)
		{
			return this.InvokeWithAPILogging<Guid>(delegate
			{
				ADServer adserver = this.FindDCByFqdn(serverFqdn);
				if (adserver == null)
				{
					throw new ADOperationException(DirectoryStrings.ErrorDCNotFound(serverFqdn));
				}
				return this.InternalGetInvocationIdByDC(adserver);
			}, "GetInvocationIdByFqdn");
		}

		public ADSite GetLocalSite()
		{
			return this.InvokeWithAPILogging<ADSite>(delegate
			{
				string siteName = NativeHelpers.GetSiteName(true);
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, siteName);
				ADSite[] array = base.Find<ADSite>(base.ConfigurationNamingContext, QueryScope.SubTree, filter, null, 1);
				if (array == null || array.Length == 0)
				{
					ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "No site was found with the name {0}", siteName);
					throw new CannotGetSiteInfoException(DirectoryStrings.CannotGetUsefulSiteInfo);
				}
				ExTraceGlobals.ADTopologyTracer.TraceDebug((long)this.GetHashCode(), "Local site found");
				return array[0];
			}, "GetLocalSite");
		}

		public MsoMainStreamCookieContainer GetMsoMainStreamCookieContainer(string serviceInstanceName)
		{
			return this.InvokeWithAPILogging<MsoMainStreamCookieContainer>(delegate
			{
				ADObjectId serviceInstanceObjectId = SyncServiceInstance.GetServiceInstanceObjectId(serviceInstanceName);
				MsoMainStreamCookieContainer msoMainStreamCookieContainer = this.Read<MsoMainStreamCookieContainer>(serviceInstanceObjectId);
				if (msoMainStreamCookieContainer == null)
				{
					throw new ServiceInstanceContainerNotFoundException(serviceInstanceName);
				}
				return msoMainStreamCookieContainer;
			}, "GetMsoMainStreamCookieContainer");
		}

		public PooledLdapConnection GetNotifyConnection()
		{
			return this.InvokeWithAPILogging<PooledLdapConnection>(delegate
			{
				string accountOrResourceForestFqdn = base.SessionSettings.GetAccountOrResourceForestFqdn();
				return ConnectionPoolManager.GetConnection(ConnectionType.ConfigDCNotification, accountOrResourceForestFqdn);
			}, "GetNotifyConnection");
		}

		public Server GetParentServer(ADObjectId entryId, ADObjectId originalId)
		{
			return this.InvokeWithAPILogging<Server>(delegate
			{
				ADObjectId adobjectId = entryId.DescendantDN(8);
				if (originalId != null)
				{
					ADObjectId id = originalId.DescendantDN(8);
					if (!adobjectId.Equals(id))
					{
						throw new NotSupportedException(string.Format("Moving object '{0}' from server '{1}' to '{2}' is not supported.", entryId.ToString(), originalId.ToString(), adobjectId.ToString()));
					}
				}
				return this.Read<Server>(adobjectId);
			}, "GetParentServer");
		}

		public ProvisioningReconciliationConfig GetProvisioningReconciliationConfig()
		{
			return this.InvokeWithAPILogging<ProvisioningReconciliationConfig>(delegate
			{
				ProvisioningReconciliationConfig[] array = base.Find<ProvisioningReconciliationConfig>(null, QueryScope.SubTree, null, null, 1);
				if (array != null && array.Length == 1)
				{
					return array[0];
				}
				return null;
			}, "GetProvisioningReconciliationConfig");
		}

		public string GetRootDomainNamingContextFromCurrentReadConnection()
		{
			return this.InvokeWithAPILogging<string>(delegate
			{
				ADObjectId adobjectId = null;
				PooledLdapConnection readConnection = base.GetReadConnection(null, ref adobjectId);
				string rootDomainNC;
				try
				{
					rootDomainNC = readConnection.ADServerInfo.RootDomainNC;
				}
				finally
				{
					readConnection.ReturnToPool();
				}
				return rootDomainNC;
			}, "GetRootDomainNamingContextFromCurrentReadConnection");
		}

		public RootDse GetRootDse()
		{
			return this.InvokeWithAPILogging<RootDse>(delegate
			{
				if (string.IsNullOrEmpty(base.DomainController))
				{
					throw new InvalidOperationException(DirectoryStrings.GetRootDseRequiresDomainController);
				}
				RootDse[] array = base.Find<RootDse>(new ADObjectId(string.Empty), QueryScope.Base, null, null, 0);
				if (array.Length != 1)
				{
					throw new ADOperationException(DirectoryStrings.InvalidRootDse(base.LastUsedDc));
				}
				return array[0];
			}, "GetRootDse");
		}

		public RoutingGroup GetRoutingGroup()
		{
			return this.InvokeWithAPILogging<RoutingGroup>(delegate
			{
				RoutingGroup[] array = base.Find<RoutingGroup>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, RoutingGroup.DefaultName), null, 1);
				if (array == null || array.Length == 0)
				{
					throw new DefaultRoutingGroupNotFoundException(RoutingGroup.DefaultName);
				}
				return array[0];
			}, "GetRoutingGroup");
		}

		public ADObjectId GetRoutingGroupId()
		{
			return this.InvokeWithAPILogging<ADObjectId>(delegate
			{
				if (this.routingGroupId == null)
				{
					RoutingGroup routingGroup = this.GetRoutingGroup();
					this.routingGroupId = routingGroup.Id;
				}
				return this.routingGroupId;
			}, "GetRoutingGroupId");
		}

		public string GetSchemaMasterDC()
		{
			return this.InvokeWithAPILogging<string>(delegate
			{
				if (TopologyProvider.IsAdamTopology())
				{
					return "localhost";
				}
				SchemaContainer[] array = this.InternalFind<SchemaContainer>(base.GetSchemaNamingContext(), QueryScope.Base, null, null, 1, null);
				if (array == null || array.Length == 0)
				{
					throw new SchemaMasterDCNotFoundException(DirectoryStrings.ExceptionNoSchemaContainerObject);
				}
				ADObjectId fsmoRoleOwner = array[0].FsmoRoleOwner;
				if (fsmoRoleOwner == null)
				{
					throw new SchemaMasterDCNotFoundException(DirectoryStrings.ExceptionNoFsmoRoleOwnerAttribute);
				}
				ADObjectId parent = fsmoRoleOwner.Parent;
				ADServer[] array2 = this.InternalFind<ADServer>(parent, QueryScope.Base, null, null, 1, null);
				if (array2 == null || array2.Length == 0)
				{
					throw new SchemaMasterDCNotFoundException(DirectoryStrings.ExceptionNoSchemaMasterServerObject(parent.DistinguishedName ?? string.Empty));
				}
				return array2[0].DnsHostName;
			}, "GetSchemaMasterDC");
		}

		public ServicesContainer GetServicesContainer()
		{
			return this.InvokeWithAPILogging<ServicesContainer>(delegate
			{
				ServicesContainer[] array = base.Find<ServicesContainer>(null, QueryScope.OneLevel, new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADConfigurationObjectSchema.SystemFlags, SystemFlagsEnum.Indispensable),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, ServicesContainer.DefaultName)
				}), null, 1);
				if (array == null || array.Length == 0)
				{
					throw new ServicesContainerNotFoundException(DirectoryStrings.ServicesContainerNotFound);
				}
				return array[0];
			}, "GetServicesContainer");
		}

		public SitesContainer GetSitesContainer()
		{
			return this.InvokeWithAPILogging<SitesContainer>(delegate
			{
				SitesContainer[] array = base.Find<SitesContainer>(null, QueryScope.SubTree, null, null, 1);
				if (array == null || array.Length == 0)
				{
					throw new SitesContainerNotFoundException(DirectoryStrings.SitesContainerNotFound);
				}
				return array[0];
			}, "GetSitesContainer");
		}

		public StampGroupContainer GetStampGroupContainer()
		{
			return this.InvokeWithAPILogging<StampGroupContainer>(delegate
			{
				StampGroupContainer[] array = base.Find<StampGroupContainer>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, StampGroupContainer.DefaultName), null, 1);
				if (array == null || array.Length == 0)
				{
					throw new DefaultDatabaseAvailabilityGroupContainerNotFoundException(StampGroupContainer.DefaultName);
				}
				return array[0];
			}, "GetStampGroupContainer");
		}

		public ADObjectId GetStampGroupContainerId()
		{
			if (this.stampGroupContainerId == null)
			{
				StampGroupContainer stampGroupContainer = this.GetStampGroupContainer();
				this.stampGroupContainerId = stampGroupContainer.Id;
			}
			return this.stampGroupContainerId;
		}

		public bool HasAnyServer()
		{
			return this.InvokeWithAPILogging<bool>(() => base.Find<Server>(null, QueryScope.SubTree, null, null, 1).Any<Server>(), "HasAnyServer");
		}

		public bool IsInE12InteropMode()
		{
			return this.InvokeWithAPILogging<bool>(delegate
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.E2007MinVersion),
					new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E14MinVersion)
				});
				Server[] array = base.Find<Server>(null, QueryScope.SubTree, filter, null, 1);
				return array.Length > 0;
			}, "IsInE12InteropMode");
		}

		public bool IsInPreE12InteropMode()
		{
			return this.InvokeWithAPILogging<bool>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E2007MinVersion);
				Server[] array = base.Find<Server>(null, QueryScope.SubTree, filter, null, 1);
				return array.Length > 0;
			}, "IsInPreE12InteropMode");
		}

		public bool IsInPreE14InteropMode()
		{
			return this.InvokeWithAPILogging<bool>(delegate
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E14MinVersion);
				Server[] array = base.Find<Server>(null, QueryScope.SubTree, filter, null, 1);
				return array.Length > 0;
			}, "IsInPreE14InteropMode");
		}

		public Server ReadLocalServer()
		{
			string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(false);
			if (localComputerFqdn != null)
			{
				return this.FindServerByFqdn(localComputerFqdn);
			}
			return null;
		}

		public MiniClientAccessServerOrArray ReadMiniClientAccessServerOrArray(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<MiniClientAccessServerOrArray>(() => this.InternalRead<MiniClientAccessServerOrArray>(entryId, properties), "ReadMiniClientAccessServerOrArray");
		}

		public MiniServer ReadMiniServer(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return this.InvokeWithAPILogging<MiniServer>(() => this.InternalRead<MiniServer>(entryId, properties), "ReadMiniServer");
		}

		public Result<TResult>[] ReadMultipleLegacyObjects<TResult>(string[] objectNames) where TResult : ADLegacyVersionableObject, new()
		{
			return this.InvokeWithAPILogging<Result<TResult>[]>(() => this.ReadMultiple<string, TResult>(objectNames, new Converter<string, QueryFilter>(ADTopologyConfigurationSession.ReadMultipleADLegacyObjectsQueryFilterFromObjectName), new ADDataSession.HashInserter<TResult>(ADTopologyConfigurationSession.ReadMultipleADLegacyObjectsHashInserter<TResult>), new ADDataSession.HashLookup<string, TResult>(ADTopologyConfigurationSession.ReadMultipleADLegacyObjectsHashLookup<TResult>), null), "ReadMultipleLegacyObjects");
		}

		public Result<Server>[] ReadMultipleServers(string[] serverNames)
		{
			return this.ReadMultipleLegacyObjects<Server>(serverNames);
		}

		public ManagementScope ReadRootOrgManagementScopeByName(string scopeName)
		{
			return this.InvokeWithAPILogging<ManagementScope>(() => this.Read<ManagementScope>(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().GetDescendantId(ManagementScope.RdnScopesContainerToOrganization).GetChildId(scopeName)), "ReadRootOrgManagementScopeByName");
		}

		private bool TryFindByExchangeLegacyDN<TData>(string legacyExchangeDN, IEnumerable<PropertyDefinition> properties, out TData data) where TData : ADRawEntry, new()
		{
			data = (from result in base.FindByExchangeLegacyDNs<TData>(new string[]
			{
				legacyExchangeDN
			}, properties)
			select result.Data).Single<TData>();
			return data != null;
		}

		public bool TryFindByExchangeLegacyDN(string legacyExchangeDN, IEnumerable<PropertyDefinition> properties, out MiniClientAccessServerOrArray miniClientAccessServerOrArray)
		{
			return this.TryFindByExchangeLegacyDN<MiniClientAccessServerOrArray>(legacyExchangeDN, properties, out miniClientAccessServerOrArray);
		}

		public bool TryFindByExchangeLegacyDN(string legacyExchangeDN, IEnumerable<PropertyDefinition> properties, out MiniServer miniServer)
		{
			return this.TryFindByExchangeLegacyDN<MiniServer>(legacyExchangeDN, properties, out miniServer);
		}

		public bool TryGetDefaultAdQueryPolicy(out ADQueryPolicy queryPolicy)
		{
			ADQueryPolicy[] array = base.Find<ADQueryPolicy>(ADSession.GetConfigurationNamingContextForLocalForest().GetChildId("Services").GetChildId("Windows NT"), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, ADQueryPolicy.ADDefaultQueryPolicyName), null, 1);
			if (array != null && array.Length == 1)
			{
				queryPolicy = array[0];
				return true;
			}
			queryPolicy = null;
			return false;
		}

		public void UpdateGwartLastModified()
		{
			LegacyGwart[] array = base.Find<LegacyGwart>(this.GetAdministrativeGroupId(), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, LegacyGwart.DefaultName), null, 1);
			if (array == null || array.Length == 0)
			{
				throw new LegacyGwartNotFoundException(LegacyGwart.DefaultName, AdministrativeGroup.DefaultName);
			}
			DateTime? dateTime = new DateTime?(array[0].GwartLastModified ?? DateTime.UtcNow);
			array[0].GwartLastModified = new DateTime?(dateTime.Value.AddMinutes(8.0));
			base.Save(array[0]);
		}

		private TResult[] InternalFind<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new()
		{
			return base.Find<TResult>(rootId, scope, filter, sortBy, maxResults, properties, false);
		}

		private TPolicy[] FindWorkloadManagementChildPolicies<TPolicy>(ADObjectId wlmPolicy, QueryFilter filter) where TPolicy : ADConfigurationObject, new()
		{
			if (wlmPolicy == null)
			{
				throw new ArgumentNullException("wlmPolicy");
			}
			return base.Find<TPolicy>(wlmPolicy, QueryScope.OneLevel, filter, null, 0);
		}

		private T[] FindVirtualDirectoriesForLocalServer<T>() where T : ExchangeVirtualDirectory, new()
		{
			Server server = null;
			try
			{
				server = this.FindLocalServer();
			}
			catch (LocalServerNotFoundException)
			{
			}
			if (server == null)
			{
				return new T[0];
			}
			return this.FindVirtualDirectories<T>(server);
		}

		private T[] FindVirtualDirectories<T>(Server server) where T : ExchangeVirtualDirectory, new()
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			T[] array = base.Find<T>(server.Id, QueryScope.SubTree, null, null, 0);
			if (array.Length == 0)
			{
				return new T[0];
			}
			return array;
		}

		private ThrottlingPolicy InteralGetGlobalThrottlingPolicy(bool throwError)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ThrottlingPolicySchema.ThrottlingPolicyScope, ThrottlingPolicyScopeType.Global);
			ADObjectId descendantId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().GetDescendantId(new ADObjectId("CN=Global Settings"));
			ThrottlingPolicy[] array = base.Find<ThrottlingPolicy>(descendantId, QueryScope.OneLevel, filter, null, 2);
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceError((long)this.GetHashCode(), "[ADTopologyConfiguartionSession::GetGlobalThrottlingPolicy] No global policy found in first org.");
				Globals.LogExchangeTopologyEvent(DirectoryEventLogConstants.Tuple_GlobalThrottlingPolicyMissing, descendantId.DistinguishedName, new object[0]);
				if (throwError)
				{
					throw new GlobalThrottlingPolicyNotFoundException();
				}
				return null;
			}
			else
			{
				if (array.Length == 1)
				{
					return array[0];
				}
				ExTraceGlobals.ClientThrottlingTracer.TraceError<int>((long)this.GetHashCode(), "[ADTopologyConfiguartionSession::GetGlobalThrottlingPolicy] '{0}' global policies found in first org.", array.Length);
				Globals.LogExchangeTopologyEvent(DirectoryEventLogConstants.Tuple_MoreThanOneGlobalThrottlingPolicy, descendantId.DistinguishedName, new object[]
				{
					array.Length
				});
				if (throwError)
				{
					throw new GlobalThrottlingPolicyAmbiguousException();
				}
				return null;
			}
		}

		private Guid InternalGetInvocationIdByDC(ADServer dc)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("DC cannot be null");
			}
			NtdsDsa[] array = base.Find<NtdsDsa>(dc.Id, QueryScope.OneLevel, null, null, 1);
			if (array.Length != 1)
			{
				throw new ADOperationException(DirectoryStrings.InvalidNtds(dc.DnsHostName));
			}
			return array[0].InvocationId;
		}

		private T InvokeWithAPILogging<T>(Func<T> action, [CallerMemberName] string memberName = null)
		{
			return ADScenarioLog.InvokeWithAPILog<T>(DateTime.UtcNow, memberName, default(Guid), ADTopologyConfigurationSession.ClassName, "", () => action(), () => base.LastUsedDc);
		}

		private static string ClassName = "ADTopologyConfigurationSession";

		[NonSerialized]
		private ADObjectId adminGroupId;

		[NonSerialized]
		private ADObjectId databasesContainerId;

		[NonSerialized]
		private ADObjectId databaseAvailabilityGroupContainerId;

		[NonSerialized]
		private ADObjectId stampGroupContainerId;

		[NonSerialized]
		private ADObjectId routingGroupId;
	}
}
