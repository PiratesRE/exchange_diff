using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADForest
	{
		private ADForest(string domainController, string globalCatalog) : this(ADForest.LocalForestName, domainController, globalCatalog, null, true)
		{
		}

		private ADForest(string fqdn, string domainController, string globalCatalog, NetworkCredential networkCredential, bool knownForest)
		{
			this.fqdn = fqdn;
			this.domainController = domainController;
			this.globalCatalog = globalCatalog;
			this.networkCredential = networkCredential;
			this.partitionId = (Datacenter.IsMicrosoftHostedOnly(true) ? new PartitionId(fqdn) : PartitionId.LocalForest);
			this.isKnownForest = knownForest;
			this.isLocalForest = ADForest.IsLocalForestFqdn(fqdn);
		}

		private static string LocalForestName
		{
			get
			{
				if (ADForest.localForestName == null)
				{
					ADForest.localForestName = NativeHelpers.GetForestName();
				}
				return ADForest.localForestName;
			}
		}

		private static ADForest LocalForestInstance
		{
			get
			{
				if (ADForest.localForestInstance == null)
				{
					ADForest.localForestInstance = new ADForest(null, null);
				}
				return ADForest.localForestInstance;
			}
		}

		private static bool IsDcAlsoGcAndAvailable(string domainController, NetworkCredential credentials)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, true, ConsistencyMode.FullyConsistent, credentials, ADSessionSettings.FromRootOrgScopeSet(), 131, "IsDcAlsoGcAndAvailable", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADForest.cs");
			tenantOrTopologyConfigurationSession.UseGlobalCatalog = true;
			return tenantOrTopologyConfigurationSession.IsReadConnectionAvailable();
		}

		private ITopologyConfigurationSession CreateConfigNcSession()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.domainController, true, ConsistencyMode.FullyConsistent, this.networkCredential, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.partitionId), 144, "CreateConfigNcSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADForest.cs");
			topologyConfigurationSession.UseGlobalCatalog = false;
			return topologyConfigurationSession;
		}

		private IConfigurationSession CreateDomainNcSession()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.globalCatalog, true, ConsistencyMode.FullyConsistent, this.networkCredential, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.partitionId), 156, "CreateDomainNcSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADForest.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			tenantOrTopologyConfigurationSession.UseGlobalCatalog = true;
			tenantOrTopologyConfigurationSession.EnforceDefaultScope = false;
			return tenantOrTopologyConfigurationSession;
		}

		internal static bool IsLocalForestFqdn(string fqdn)
		{
			return string.Equals(fqdn, ADForest.LocalForestName, StringComparison.OrdinalIgnoreCase);
		}

		public static ADForest GetLocalForest()
		{
			return ADForest.GetLocalForest(null);
		}

		public static ADForest GetLocalForest(string domainController)
		{
			if (string.IsNullOrEmpty(domainController))
			{
				return ADForest.LocalForestInstance;
			}
			string text = null;
			if (ADForest.IsDcAlsoGcAndAvailable(domainController, null))
			{
				text = domainController;
			}
			return new ADForest(domainController, text);
		}

		public static ADForest GetForest(string forestFqdn, NetworkCredential credentials)
		{
			if (string.IsNullOrEmpty(forestFqdn))
			{
				throw new ArgumentNullException("forestFqdn");
			}
			if (ADForest.IsLocalForestFqdn(forestFqdn))
			{
				if (credentials != null)
				{
					throw new ArgumentException("The use of credentials is not supported for the local forest '" + forestFqdn + "'");
				}
				return ADForest.GetLocalForest();
			}
			else
			{
				PartitionId partitionId = new PartitionId(forestFqdn);
				if (ADAccountPartitionLocator.IsKnownPartition(partitionId))
				{
					return ADForest.GetForest(partitionId);
				}
				ADServerInfo remoteServerFromDomainFqdn = TopologyProvider.GetInstance().GetRemoteServerFromDomainFqdn(forestFqdn, credentials);
				return new ADForest(forestFqdn, remoteServerFromDomainFqdn.Fqdn, null, credentials, false);
			}
		}

		public static ADForest GetForest(PartitionId partitionId)
		{
			if (null == partitionId)
			{
				throw new ArgumentNullException("partitionId");
			}
			if (ADForest.IsLocalForestFqdn(partitionId.ForestFQDN))
			{
				return ADForest.GetLocalForest();
			}
			IList<ADServerInfo> serversForRole = TopologyProvider.GetInstance().GetServersForRole(partitionId.ForestFQDN, new List<string>(0), ADServerRole.DomainController, 1, false);
			IList<ADServerInfo> serversForRole2 = TopologyProvider.GetInstance().GetServersForRole(partitionId.ForestFQDN, new List<string>(0), ADServerRole.GlobalCatalog, 1, false);
			if (serversForRole == null || serversForRole2 == null || serversForRole.Count == 0 || serversForRole2.Count == 0)
			{
				throw new ADOperationException(DirectoryStrings.CannotGetForestInfo(partitionId.ForestFQDN));
			}
			return new ADForest(partitionId.ForestFQDN, serversForRole[0].Fqdn, serversForRole2[0].Fqdn, null, true);
		}

		public static ADDomain FindExternalDomain(string fqdn, NetworkCredential credential)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			if (credential == null)
			{
				throw new ArgumentNullException("credential");
			}
			if (string.IsNullOrEmpty(credential.UserName))
			{
				throw new ArgumentException("User name must be provided in the credential argument to perform this operation.");
			}
			ADServerInfo remoteServerFromDomainFqdn = TopologyProvider.GetInstance().GetRemoteServerFromDomainFqdn(fqdn, credential);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(remoteServerFromDomainFqdn.Fqdn, true, ConsistencyMode.FullyConsistent, credential, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(Datacenter.IsMicrosoftHostedOnly(true) ? new PartitionId(remoteServerFromDomainFqdn.Fqdn) : PartitionId.LocalForest), 316, "FindExternalDomain", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADForest.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			tenantOrTopologyConfigurationSession.EnforceDefaultScope = false;
			ADDomain[] array = tenantOrTopologyConfigurationSession.Find<ADDomain>(new ADObjectId(NativeHelpers.DistinguishedNameFromCanonicalName(fqdn)), QueryScope.Base, null, null, 1);
			if (array == null || array.Length <= 0)
			{
				throw new ADExternalException(DirectoryStrings.ExceptionADTopologyNoSuchDomain(fqdn));
			}
			return array[0];
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		public bool IsLocalForest
		{
			get
			{
				return this.isLocalForest;
			}
		}

		public ADCrossRef[] GetDomainPartitions()
		{
			return this.GetDomainPartitions(false);
		}

		public ADCrossRef[] GetDomainPartitions(bool topLevelOnly)
		{
			return this.GetDomainPartitions(topLevelOnly, new ExistsFilter(ADCrossRefSchema.DnsRoot));
		}

		private ADCrossRef[] GetDomainPartitions(bool topLevelOnly, QueryFilter userFilter)
		{
			ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "FindDomains with topLevelOnly = {0}", topLevelOnly ? "true" : "false");
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("GetDomainPartitions is not supported for forest " + this.Fqdn);
			}
			QueryFilter queryFilter = ADForest.domainFilter;
			if (topLevelOnly)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					new NotFilter(new ExistsFilter(ADCrossRefSchema.TrustParent))
				});
			}
			if (userFilter != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					userFilter
				});
			}
			IConfigurationSession configurationSession = this.CreateConfigNcSession();
			ADCrossRef[] array = configurationSession.Find<ADCrossRef>(null, QueryScope.SubTree, queryFilter, null, 0);
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug((long)this.GetHashCode(), "No CrossRef objects were found");
			}
			return array;
		}

		public ADCrossRef GetLocalDomainPartition()
		{
			string domainName = NativeHelpers.GetDomainName();
			return this.FindDomainPartitionByFqdn(domainName);
		}

		public ReadOnlyCollection<ADDomain> FindDomains()
		{
			return this.FindDomains(false);
		}

		public ReadOnlyCollection<ADDomain> FindTopLevelDomains()
		{
			return this.FindDomains(true);
		}

		private ReadOnlyCollection<ADDomain> FindDomains(bool topLevelOnly)
		{
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindDomains is not supported for forest " + this.Fqdn);
			}
			ADCrossRef[] domainPartitions = this.GetDomainPartitions(topLevelOnly);
			List<ADDomain> list = new List<ADDomain>();
			if (domainPartitions != null)
			{
				ADObjectId[] array = new ADObjectId[domainPartitions.Length];
				IConfigurationSession configurationSession = this.CreateDomainNcSession();
				for (int i = 0; i < domainPartitions.Length; i++)
				{
					array[i] = domainPartitions[i].NCName;
				}
				Result<ADDomain>[] array2 = configurationSession.ReadMultiple<ADDomain>(array);
				for (int j = 0; j < array2.Length; j++)
				{
					Result<ADDomain> result = array2[j];
					if (result.Data == null)
					{
						ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Domain not found: {0}", array[j].ToDNString());
					}
					else
					{
						ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Domain found: {0}", array[0].ToDNString());
						list.Add(result.Data);
					}
				}
			}
			return new ReadOnlyCollection<ADDomain>(list);
		}

		public ADDomain FindDomainByFqdn(string fqdn)
		{
			if (fqdn == null)
			{
				throw new ArgumentNullException("fqdn");
			}
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentException(DirectoryStrings.ExEmptyStringArgumentException("fqdn"), "fqdn");
			}
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindDomainByFqdn is not supported for forest " + this.Fqdn);
			}
			ADCrossRef adcrossRef = this.FindDomainPartitionByFqdn(fqdn);
			if (adcrossRef == null)
			{
				return null;
			}
			IConfigurationSession configurationSession = this.CreateDomainNcSession();
			ADDomain addomain = configurationSession.Read<ADDomain>(adcrossRef.NCName);
			if (addomain == null)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "No Domain found at {0}", adcrossRef.NCName);
				return null;
			}
			ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Domain found: {0}", addomain.Name);
			return addomain;
		}

		public ADCrossRef FindDomainPartitionByFqdn(string fqdn)
		{
			if (fqdn == null)
			{
				throw new ArgumentNullException("fqdn");
			}
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentException(DirectoryStrings.ExEmptyStringArgumentException("fqdn"), "fqdn");
			}
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindDomainPartitionByFqdn is not supported for forest " + this.Fqdn);
			}
			QueryFilter userFilter = new ComparisonFilter(ComparisonOperator.Equal, ADCrossRefSchema.DnsRoot, fqdn);
			ADCrossRef[] domainPartitions = this.GetDomainPartitions(false, userFilter);
			if (domainPartitions == null || domainPartitions.Length == 0)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "No CrossRef found using fqdn {0}", fqdn ?? "<null>");
				return null;
			}
			return domainPartitions[0];
		}

		public ADDomain FindLocalDomain()
		{
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindLocalDomain is not supported for forest " + this.Fqdn);
			}
			string domainName = NativeHelpers.GetDomainName();
			return this.FindDomainByFqdn(domainName);
		}

		public ADDomain FindRootDomain()
		{
			return this.FindRootDomain(false);
		}

		public ADDomain FindRootDomain(bool readFromDc)
		{
			ExTraceGlobals.ADTopologyTracer.TraceDebug((long)this.GetHashCode(), "FindRootDomain");
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindRootDomain is not supported for forest " + this.Fqdn);
			}
			IConfigurationSession configurationSession;
			if (readFromDc)
			{
				configurationSession = this.CreateConfigNcSession();
				configurationSession.UseConfigNC = false;
			}
			else
			{
				configurationSession = this.CreateDomainNcSession();
			}
			ADObjectId rootDomainNamingContext = configurationSession.GetRootDomainNamingContext();
			if (rootDomainNamingContext == null)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug((long)this.GetHashCode(), "GetRootDomainNamingContext returned null");
				return null;
			}
			ADDomain[] array = null;
			try
			{
				array = configurationSession.Find<ADDomain>(rootDomainNamingContext, QueryScope.Base, null, null, 1);
			}
			catch (ADReferralException innerException)
			{
				if (!readFromDc || string.IsNullOrEmpty(this.domainController))
				{
					throw;
				}
				throw new ADOperationException(DirectoryStrings.ExceptionReferralWhenBoundToDomainController(rootDomainNamingContext.ToString(), this.domainController), innerException);
			}
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "No domain found at {0}", rootDomainNamingContext.DistinguishedName);
				return null;
			}
			ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Domain found: {0}", array[0].Name);
			return array[0];
		}

		public ReadOnlyCollection<ADServer> FindAllGlobalCatalogs()
		{
			return this.FindAllGlobalCatalogs(true);
		}

		public ReadOnlyCollection<ADServer> FindAllGlobalCatalogs(bool includingRODC)
		{
			ITopologyConfigurationSession topologyConfigurationSession = this.CreateConfigNcSession();
			return topologyConfigurationSession.FindServerWithNtdsdsa(null, true, includingRODC);
		}

		public List<ADServer> FindAllGlobalCatalogsInLocalSite()
		{
			ITopologyConfigurationSession topologyConfigurationSession = this.CreateConfigNcSession();
			ADSite localSite = topologyConfigurationSession.GetLocalSite();
			List<ADServer> list = new List<ADServer>();
			if (localSite != null)
			{
				ReadOnlyCollection<ADServer> readOnlyCollection = this.FindAllGlobalCatalogs(false);
				foreach (ADServer adserver in readOnlyCollection)
				{
					if (adserver.Site.Equals(localSite.Id))
					{
						list.Add(adserver);
					}
				}
			}
			return list;
		}

		public ADDomainTrustInfo[] FindAllLocalDomainTrustedDomains()
		{
			return this.FindTrustedDomains(NativeHelpers.GetDomainName());
		}

		public ADDomainTrustInfo[] FindTrustedDomains(string domainFqdn)
		{
			if (string.IsNullOrEmpty(domainFqdn))
			{
				throw new ArgumentNullException("domainFqdn");
			}
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindAllLocalDomainTrustedDomains is only supported for local forest and account forests");
			}
			if (this.FindDomainByFqdn(domainFqdn) == null)
			{
				throw new ADExternalException(DirectoryStrings.ExceptionADTopologyNoSuchDomain(domainFqdn));
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ADDomainTrustInfoSchema.TrustDirection, 2UL),
				new ComparisonFilter(ComparisonOperator.Equal, ADDomainTrustInfoSchema.TrustType, TrustTypeFlag.Uplevel),
				new NotFilter(new BitMaskAndFilter(ADDomainTrustInfoSchema.TrustAttributes, 32UL)),
				new NotFilter(new BitMaskAndFilter(ADDomainTrustInfoSchema.TrustAttributes, 8UL))
			});
			return this.FindTrustRelationships(domainFqdn, filter);
		}

		public ADDomainTrustInfo[] FindAllTrustedForests()
		{
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindAllLocalDomainTrustedDomains is only supported for the local forest and account forest.");
			}
			return this.FindTrustRelationships(this.fqdn, ADForest.forestTrustFilter);
		}

		public ADDomainTrustInfo[] FindTrustRelationshipsForDomain(string trustedDomainFqnd)
		{
			if (!this.isKnownForest)
			{
				throw new NotSupportedException("FindAllLocalDomainTrustedDomains is only supported for the local forest and account forest.");
			}
			return this.FindTrustRelationships(this.fqdn, QueryFilter.AndTogether(new QueryFilter[]
			{
				ADForest.forestTrustFilter,
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, trustedDomainFqnd)
			}));
		}

		private ADDomainTrustInfo[] FindTrustRelationships(string fqdn, QueryFilter filter)
		{
			ADObjectId rootId = new ADObjectId("CN=System," + NativeHelpers.DistinguishedNameFromCanonicalName(fqdn));
			IConfigurationSession configurationSession = this.CreateDomainNcSession();
			return configurationSession.Find<ADDomainTrustInfo>(rootId, QueryScope.SubTree, filter, null, 0);
		}

		private static readonly QueryFilter domainFilter = new BitMaskAndFilter(ADConfigurationObjectSchema.SystemFlags, 3UL);

		private static readonly QueryFilter forestTrustFilter = new AndFilter(new QueryFilter[]
		{
			new BitMaskAndFilter(ADDomainTrustInfoSchema.TrustDirection, 2UL),
			new ComparisonFilter(ComparisonOperator.Equal, ADDomainTrustInfoSchema.TrustType, TrustTypeFlag.Uplevel),
			new BitMaskAndFilter(ADDomainTrustInfoSchema.TrustAttributes, 8UL)
		});

		private readonly bool isKnownForest;

		private readonly bool isLocalForest;

		private static ADForest localForestInstance;

		private static string localForestName;

		private string fqdn;

		private string domainController;

		private string globalCatalog;

		private PartitionId partitionId;

		private NetworkCredential networkCredential;
	}
}
