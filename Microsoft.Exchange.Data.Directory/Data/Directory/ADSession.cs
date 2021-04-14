using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationCache;
using Microsoft.Exchange.Data.Directory.ValidationRules;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal static class ADSession
	{
		public static byte[] EncodePasswordForLdap(SecureString password)
		{
			byte[] array = new byte[(password.Length + 2) * 2];
			int num = 34;
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
				int num2 = 0;
				array[num2++] = (byte)(num & 255);
				array[num2++] = (byte)(num >> 8 & 255);
				Marshal.Copy(intPtr, array, num2, password.Length * 2);
				num2 += password.Length * 2;
				array[num2++] = (byte)(num & 255);
				array[num2++] = (byte)(num >> 8 & 255);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
				}
				gchandle.Free();
			}
			return array;
		}

		public static bool IsBoundToAdam
		{
			get
			{
				return TopologyProvider.IsAdamTopology();
			}
		}

		public static void SetAdminTopologyMode()
		{
			if (!ADSession.isAdminModeEnabled)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug(0L, "A process tried to set the Topology Mode to Admin, but this mode has been disabled");
				return;
			}
			TopologyProvider.SetProcessTopologyMode(true, true);
			if (ADSession.IsBoundToAdam)
			{
				ADSessionSettings.ClearProcessADContext();
				ConnectionPoolManager.ForceRebuild();
			}
		}

		public static void DisableAdminTopologyMode()
		{
			if (TopologyProvider.IsAdminMode)
			{
				ExTraceGlobals.ADTopologyTracer.TraceError(0L, "A process tried to disable Admin Topology Mode after that mode was already set");
				throw new ADOperationException(DirectoryStrings.ExceptionUnableToDisableAdminTopologyMode);
			}
			ADSession.isAdminModeEnabled = false;
		}

		public static string GetConfigDC(string partitionFqdn, string serverName)
		{
			if (!Globals.IsDatacenter)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>(0L, "ADSession::GetConfigDC '{0}' will use local forest '{1}'", partitionFqdn, TopologyProvider.LocalForestFqdn);
				partitionFqdn = TopologyProvider.LocalForestFqdn;
			}
			string fqdn;
			using (ServiceTopologyProvider serviceTopologyProvider = new ServiceTopologyProvider(serverName))
			{
				ADServerInfo configDCInfo = serviceTopologyProvider.GetConfigDCInfo(partitionFqdn, false);
				if (configDCInfo == null)
				{
					throw new ADTransientException(DirectoryStrings.ExceptionADTopologyUnexpectedError(serverName, string.Empty));
				}
				fqdn = configDCInfo.Fqdn;
			}
			return fqdn;
		}

		public static string GetSharedConfigDC()
		{
			string configDC;
			using (ServiceTopologyProvider serviceTopologyProvider = new ServiceTopologyProvider())
			{
				configDC = serviceTopologyProvider.GetConfigDC(false);
			}
			return configDC;
		}

		public static void SetSharedConfigDC(string partitionFqdn, string serverName, int port)
		{
			using (ServiceTopologyProvider serviceTopologyProvider = new ServiceTopologyProvider())
			{
				serviceTopologyProvider.SetConfigDC(partitionFqdn, serverName, port);
			}
		}

		public static void SetCurrentConfigDC(string serverName, string partitionFqdn)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentNullException("serverName");
			}
			if (string.IsNullOrEmpty(partitionFqdn))
			{
				throw new ArgumentNullException("partitionFqdn");
			}
			TopologyProvider instance = TopologyProvider.GetInstance();
			instance.SetConfigDC(partitionFqdn, serverName, instance.DefaultDCPort);
		}

		public static string GetCurrentConfigDCForLocalForest()
		{
			return ADSession.GetCurrentConfigDC(TopologyProvider.LocalForestFqdn);
		}

		public static string GetCurrentConfigDC(string partitionFqdn)
		{
			ADSessionSettings adsessionSettings;
			if (PartitionId.IsLocalForestPartition(partitionFqdn))
			{
				adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			else
			{
				adsessionSettings = ADSessionSettings.FromAccountPartitionRootOrgScopeSet(new PartitionId(partitionFqdn));
			}
			Fqdn fqdn = adsessionSettings.ServerSettings.ConfigurationDomainController(partitionFqdn);
			if (fqdn != null)
			{
				return fqdn.ToString();
			}
			return TopologyProvider.GetInstance().GetConfigDC(partitionFqdn);
		}

		public static string GetCurrentConfigDC(PartitionId partitionId)
		{
			if (partitionId != null)
			{
				return ADSession.GetCurrentConfigDC(partitionId.ForestFQDN);
			}
			return ADSession.GetCurrentConfigDCForLocalForest();
		}

		private static void CopySettableSessionProperties(IDirectorySession oldSession, IDirectorySession newSession)
		{
			if (oldSession != null)
			{
				newSession.UseConfigNC = oldSession.UseConfigNC;
				newSession.UseGlobalCatalog = oldSession.UseGlobalCatalog;
				newSession.EnforceDefaultScope = oldSession.EnforceDefaultScope;
				newSession.SkipRangedAttributes = oldSession.SkipRangedAttributes;
				if (object.Equals(oldSession.SessionSettings.PartitionId, newSession.SessionSettings.PartitionId))
				{
					newSession.LinkResolutionServer = oldSession.LinkResolutionServer;
				}
			}
		}

		internal static void CopySettableSessionPropertiesAndSettings(IDirectorySession oldSession, IDirectorySession newSession)
		{
			if (oldSession != null)
			{
				ADSession.CopySettableSessionProperties(oldSession, newSession);
				ADSessionSettings.CloneSettableProperties(oldSession.SessionSettings, newSession.SessionSettings);
			}
		}

		internal static IDirectorySession RescopeSessionToTenantSubTree(IDirectorySession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (session.ConfigScope == ConfigScopes.TenantLocal)
			{
				return ADSession.CreateScopedSession(session, ADSessionSettings.RescopeToSubtree(session.SessionSettings));
			}
			return session;
		}

		internal static IDirectorySession CreateScopedSession(IDirectorySession session, ADSessionSettings underSessionSettings)
		{
			bool flag = object.Equals(session.SessionSettings.PartitionId, underSessionSettings.PartitionId);
			IConfigurationSession configurationSession = session as IConfigurationSession;
			IDirectorySession directorySession;
			if (configurationSession != null)
			{
				directorySession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(flag ? configurationSession.DomainController : null, configurationSession.ReadOnly, configurationSession.ConsistencyMode, flag ? configurationSession.NetworkCredential : null, underSessionSettings, 395, "CreateScopedSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADSession.cs");
			}
			else
			{
				IRecipientSession recipientSession = session as IRecipientSession;
				if (recipientSession.SessionSettings.IncludeSoftDeletedObjects)
				{
					underSessionSettings.IncludeSoftDeletedObjects = true;
					directorySession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(flag ? recipientSession.DomainController : null, flag ? recipientSession.SearchRoot : null, recipientSession.Lcid, recipientSession.ReadOnly, recipientSession.ConsistencyMode, flag ? recipientSession.NetworkCredential : null, underSessionSettings, recipientSession.ConfigScope, 410, "CreateScopedSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADSession.cs");
				}
				else
				{
					directorySession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(flag ? recipientSession.DomainController : null, flag ? recipientSession.SearchRoot : null, recipientSession.Lcid, recipientSession.ReadOnly, recipientSession.ConsistencyMode, flag ? recipientSession.NetworkCredential : null, underSessionSettings, 422, "CreateScopedSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADSession.cs");
				}
				if (recipientSession.IsReducedRecipientSession())
				{
					directorySession = DirectorySessionFactory.Default.GetReducedRecipientSession((IRecipientSession)directorySession, 434, "CreateScopedSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADSession.cs");
				}
			}
			ADSession.CopySettableSessionProperties(session, directorySession);
			return directorySession;
		}

		internal static bool IsTenantIdentity(ADObjectId id, string partitionFqdn)
		{
			if (ADSession.IsBoundToAdam || id.DomainId == null)
			{
				return false;
			}
			if (!string.Equals(id.GetPartitionId().ForestFQDN, partitionFqdn, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Object partition FQDN doesn't match partitionFqdn parameter.");
			}
			ADObjectId configurationNamingContext = ADSession.GetConfigurationNamingContext(partitionFqdn);
			if (id.Equals(configurationNamingContext))
			{
				return false;
			}
			ADObjectId domainNamingContext = ADSession.GetDomainNamingContext(partitionFqdn);
			if (id.Equals(domainNamingContext))
			{
				return false;
			}
			ADObjectId configurationUnitsRoot = ADSession.GetConfigurationUnitsRoot(partitionFqdn);
			if (id.IsDescendantOf(configurationUnitsRoot))
			{
				return true;
			}
			ADObjectId hostedOrganizationsRoot = ADSession.GetHostedOrganizationsRoot(partitionFqdn);
			return id.IsDescendantOf(hostedOrganizationsRoot) && !id.Equals(hostedOrganizationsRoot);
		}

		internal static bool IsLdapFilterError(ADOperationException ex)
		{
			return ex.InnerException != null && ex.InnerException is LdapException && ((LdapException)ex.InnerException).ErrorCode == 87;
		}

		public static bool TryVerifyIsWithinScopes(ADRawEntry obj, ADScope readScope, IList<ADScopeCollection> writeScopes, ADScopeCollection exclusiveScopes, IList<ValidationRule> validationRules, bool emptyObjectSessionOnException, out ADScopeException exception)
		{
			return ADSession.TryVerifyIsWithinScopes(obj, readScope, writeScopes, exclusiveScopes, validationRules, emptyObjectSessionOnException, ConfigScopes.None, out exception);
		}

		internal static bool TryVerifyIsWithinScopes(ADRawEntry obj, ADScope readScope, IList<ADScopeCollection> writeScopes, ADScopeCollection exclusiveScopes, IList<ValidationRule> validationRules, bool emptyObjectSessionOnException, ConfigScopes sessionScopeHint, out ADScopeException exception)
		{
			if (readScope == null)
			{
				throw new ArgumentNullException("readScope");
			}
			if (writeScopes == null)
			{
				throw new ArgumentNullException("writeScopes");
			}
			exception = null;
			bool flag;
			if (!ADSession.IsWithinScope(obj, readScope, out flag))
			{
				if (!flag || sessionScopeHint != ConfigScopes.RootOrg || ADSession.IsTenantIdentity(obj.Id, obj.Id.GetPartitionId().ForestFQDN))
				{
					if (obj is ADObject && emptyObjectSessionOnException)
					{
						((ADObject)obj).m_Session = null;
					}
					exception = new ADScopeException(DirectoryStrings.ErrorNotInReadScope(obj.Id.ToString()));
					return false;
				}
				ExTraceGlobals.ScopeVerificationTracer.TraceDebug<ADObjectId>(0L, "ADSession::TryVerifyIsWithinScopes Allowing unfilterable object '{0}' in RootOrg-scoped session to bypass filter verification", obj.Id);
			}
			bool flag2 = false;
			if (exclusiveScopes != null)
			{
				foreach (ADScope scope in exclusiveScopes)
				{
					if (ADSession.IsWithinScope(obj, scope))
					{
						flag2 = true;
						break;
					}
				}
			}
			foreach (ADScopeCollection adscopeCollection in writeScopes)
			{
				bool flag3 = false;
				foreach (ADScope adscope in adscopeCollection)
				{
					bool flag4 = false;
					bool flag5 = false;
					bool flag6 = false;
					if (adscope is RbacScope)
					{
						RbacScope rbacScope = (RbacScope)adscope;
						flag4 = rbacScope.Exclusive;
						flag5 = rbacScope.IsFromEndUserRole;
						flag6 = (rbacScope.ScopeType == ScopeType.Self);
					}
					if (!flag2 && flag4)
					{
						ExTraceGlobals.ScopeVerificationTracer.TraceDebug(0L, "ADSession::TryVerifyIsWithinScopes Ignoring scope ScopeRoot '{0}', ScopeFilter '{1}', IsWithinExclusiveScope '{2}', IsExclusive '{3}'", new object[]
						{
							(adscope.Root == null) ? "<null>" : adscope.Root.ToDNString(),
							(adscope.Filter == null) ? "<null>" : adscope.Filter.ToString(),
							flag2,
							flag4
						});
					}
					else
					{
						ADScope adscope2 = adscope;
						if (flag2 && !flag4)
						{
							if (!flag5)
							{
								ExTraceGlobals.ScopeVerificationTracer.TraceDebug(0L, "ADSession::TryVerifyIsWithinScopes Ignoring scope ScopeRoot '{0}', ScopeFilter '{1}', IsWithinExclusiveScope '{2}', IsExclusive '{3}'", new object[]
								{
									(adscope2.Root == null) ? "<null>" : adscope2.Root.ToDNString(),
									(adscope2.Filter == null) ? "<null>" : adscope2.Filter.ToString(),
									flag2,
									flag4
								});
								continue;
							}
							if (!flag6)
							{
								if (((RbacScope)adscope2).SelfFilter == null)
								{
									exception = new ADScopeException(DirectoryStrings.ExArgumentNullException("RbacScope.SelfFilter"));
									return false;
								}
								adscope2 = new RbacScope(ScopeType.Self)
								{
									Root = ((RbacScope)adscope2).SelfRoot,
									Filter = ((RbacScope)adscope2).SelfFilter
								};
							}
						}
						if (ADSession.IsWithinScope(obj, adscope2))
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					if (obj is ADObject && emptyObjectSessionOnException)
					{
						((ADObject)obj).m_Session = null;
					}
					exception = new ADScopeException(DirectoryStrings.ErrorNoWriteScope(obj.Id.ToString()));
					return false;
				}
			}
			if (validationRules != null)
			{
				RuleValidationException ex = null;
				foreach (ValidationRule validationRule in validationRules)
				{
					if (!validationRule.TryValidate(obj, out ex))
					{
						exception = ex;
						return false;
					}
				}
			}
			return true;
		}

		public static bool TryVerifyIsWithinScopes(ADRawEntry obj, ADScope readScope, IList<ADScopeCollection> writeScopes, ADScopeCollection exclusiveScopes, bool emptyObjectSessionOnException, out ADScopeException exception)
		{
			return ADSession.TryVerifyIsWithinScopes(obj, readScope, writeScopes, exclusiveScopes, null, emptyObjectSessionOnException, out exception);
		}

		public static string StringFromWkGuid(Guid wkGuid, string containerDN)
		{
			return string.Format("<WKGUID={0},{1}>", HexConverter.ByteArrayToHexString(wkGuid.ToByteArray()), containerDN);
		}

		public static void VerifyIsWithinScopes(ADRawEntry obj, ADScope readScope, IList<ADScopeCollection> writeScopes, ADScopeCollection invalidScopes, IList<ValidationRule> validationRules, bool emptyObjectSessionOnException)
		{
			ADScopeException ex;
			if (!ADSession.TryVerifyIsWithinScopes(obj, readScope, writeScopes, invalidScopes, validationRules, emptyObjectSessionOnException, out ex))
			{
				throw ex;
			}
		}

		public static void VerifyIsWithinScopes(ADRawEntry obj, ADScope readScope, IList<ADScopeCollection> writeScopes, ADScopeCollection invalidScopes, bool emptyObjectSessionOnException)
		{
			ADScopeException ex;
			if (!ADSession.TryVerifyIsWithinScopes(obj, readScope, writeScopes, invalidScopes, emptyObjectSessionOnException, out ex))
			{
				throw ex;
			}
		}

		internal static bool IsWithinScope(ADRawEntry obj, ADScope scope)
		{
			bool flag;
			return ADSession.IsWithinScope(obj, scope, out flag);
		}

		internal static bool IsWithinScope(ADRawEntry obj, ADScope scope, out bool outOfScopeBecauseOfFilter)
		{
			return ADDataSession.IsWithinScope(obj, scope, out outOfScopeBecauseOfFilter);
		}

		internal static string GetAttributeNameWithRange(string ldapDisplayName, string lowerRange, string upperRange)
		{
			return string.Format("{0};Range={1}-{2}", ldapDisplayName, lowerRange, upperRange);
		}

		internal static ADObjectId GetDomainNamingContext(string domainController, NetworkCredential credential)
		{
			return ADSession.GetNamingContext(ADSession.ADNamingContext.Domain, domainController, credential);
		}

		internal static ADObjectId GetSchemaNamingContext(string domainController, NetworkCredential credential)
		{
			return ADSession.GetNamingContext(ADSession.ADNamingContext.Schema, domainController, credential);
		}

		internal static ADObjectId GetConfigurationNamingContext(string domainController, NetworkCredential credential)
		{
			return ADSession.GetNamingContext(ADSession.ADNamingContext.Config, domainController, credential);
		}

		internal static ADObjectId GetRootDomainNamingContext(string domainController, NetworkCredential credential)
		{
			return ADSession.GetNamingContext(ADSession.ADNamingContext.RootDomain, domainController, credential);
		}

		private static ADObjectId GetNamingContext(ADSession.ADNamingContext context, string domainController, NetworkCredential credential)
		{
			PooledLdapConnection pooledLdapConnection = null;
			ADObjectId result = null;
			try
			{
				string partitionFqdn = Globals.IsMicrosoftHostedOnly ? ADServerSettings.GetPartitionFqdnFromADServerFqdn(domainController) : TopologyProvider.LocalForestFqdn;
				pooledLdapConnection = ConnectionPoolManager.GetConnection(ConnectionType.DomainController, partitionFqdn, credential, domainController, 389);
				switch (context)
				{
				case ADSession.ADNamingContext.RootDomain:
					result = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.RootDomainNC);
					break;
				case ADSession.ADNamingContext.Domain:
					result = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.WritableNC);
					break;
				case (ADSession.ADNamingContext)3:
					break;
				case ADSession.ADNamingContext.Config:
					result = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.ConfigNC);
					break;
				default:
					if (context == ADSession.ADNamingContext.Schema)
					{
						result = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.SchemaNC);
					}
					break;
				}
			}
			finally
			{
				if (pooledLdapConnection != null)
				{
					pooledLdapConnection.ReturnToPool();
				}
			}
			return result;
		}

		internal static ADObjectId GetConfigurationNamingContextForLocalForest()
		{
			return ADSession.GetConfigurationNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal static ADObjectId GetConfigurationNamingContext(string partitionFqdn)
		{
			if (!Globals.IsDatacenter)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>(0L, "ADSession::GetConfigurationNamingContext '{0}' will use local forest '{1}'", partitionFqdn, TopologyProvider.LocalForestFqdn);
				partitionFqdn = TopologyProvider.LocalForestFqdn;
			}
			return TopologyProvider.GetInstance().GetConfigurationNamingContext(partitionFqdn);
		}

		internal static ADObjectId GetSchemaNamingContextForLocalForest()
		{
			return ADSession.GetSchemaNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal static ADObjectId GetSchemaNamingContext(string partitionFqdn)
		{
			if (!Globals.IsDatacenter)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>(0L, "ADSession::GetSchemaNamingContext '{0}' will use local forest '{1}'", partitionFqdn, TopologyProvider.LocalForestFqdn);
				partitionFqdn = TopologyProvider.LocalForestFqdn;
			}
			return TopologyProvider.GetInstance().GetSchemaNamingContext(partitionFqdn);
		}

		internal static ADObjectId GetDomainNamingContextForLocalForest()
		{
			return ADSession.GetDomainNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal static ADObjectId GetDomainNamingContext(string partitionFqdn)
		{
			if (!Globals.IsDatacenter)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>(0L, "ADSession::GetDomainNamingContext '{0}' will use local forest '{1}'", partitionFqdn, TopologyProvider.LocalForestFqdn);
				partitionFqdn = TopologyProvider.LocalForestFqdn;
			}
			return TopologyProvider.GetInstance().GetDomainNamingContext(partitionFqdn);
		}

		internal static ADObjectId GetRootDomainNamingContext(string partitionFqdn)
		{
			if (!Globals.IsDatacenter)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>(0L, "ADSession::GetRootDomainNamingContext '{0}' will use local forest '{1}'", partitionFqdn, TopologyProvider.LocalForestFqdn);
				partitionFqdn = TopologyProvider.LocalForestFqdn;
			}
			return TopologyProvider.GetInstance().GetRootDomainNamingContext(partitionFqdn);
		}

		internal static ADObjectId GetRootDomainNamingContextForLocalForest()
		{
			return ADSession.GetRootDomainNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal static ADObjectId GetDeletedObjectsContainer(ADObjectId namingContextId)
		{
			if (namingContextId == null)
			{
				throw new ArgumentNullException("namingContextId");
			}
			return namingContextId.GetChildId("Deleted Objects");
		}

		internal static ADObjectId GetHostedOrganizationsRootForLocalForest()
		{
			return ADSession.GetHostedOrganizationsRoot(TopologyProvider.LocalForestFqdn);
		}

		internal static ADObjectId GetHostedOrganizationsRoot(string partitionFqdn)
		{
			return ADSession.GetRootDomainNamingContext(partitionFqdn).GetChildId("OU", "Microsoft Exchange Hosted Organizations");
		}

		internal static ADObjectId GetMicrosoftExchangeRoot(ADObjectId configNC)
		{
			if (configNC == null)
			{
				throw new ArgumentNullException("configNC");
			}
			return configNC.GetDescendantId(new ADObjectId("CN=Microsoft Exchange,CN=Services"));
		}

		public static object[][] ConvertToView(ADRawEntry[] recipients, IEnumerable<PropertyDefinition> properties)
		{
			if (recipients == null)
			{
				return null;
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>(properties);
			PropertyDefinition[] propertyDefinitions = list.ToArray();
			object[][] array = new object[recipients.Length][];
			for (int i = 0; i < recipients.Length; i++)
			{
				array[i] = recipients[i].GetProperties(propertyDefinitions);
			}
			return array;
		}

		internal static bool IsTenantConfigObjectInCorrectNC(ADObjectId tenantObjectId)
		{
			return tenantObjectId == null || tenantObjectId.DomainId == null || ADSessionSettings.IsForefrontObject(tenantObjectId) || tenantObjectId.ToDNString().IndexOf("cn=configuration,dc=", StringComparison.OrdinalIgnoreCase) < 0 || !ADSession.IsTenantConfigInDomainNC(tenantObjectId.GetPartitionId().ForestFQDN);
		}

		internal static bool Diag_GetRegistryBool(RegistryKey regkey, string key, bool defaultValue)
		{
			int? num = null;
			if (regkey != null)
			{
				num = (regkey.GetValue(key) as int?);
			}
			if (num == null)
			{
				return defaultValue;
			}
			return Convert.ToBoolean(num.Value);
		}

		internal static ADObjectId GetConfigurationUnitsRootForLocalForest()
		{
			return ADSession.GetConfigurationUnitsRoot(TopologyProvider.LocalForestFqdn);
		}

		internal static ADObjectId GetConfigurationUnitsRoot(string partitionFqdn)
		{
			ADObjectId adobjectId = ADSession.IsTenantConfigInDomainNC(partitionFqdn) ? ADSession.GetRootDomainNamingContext(partitionFqdn) : ADSession.GetMicrosoftExchangeRoot(ADSession.GetConfigurationNamingContext(partitionFqdn));
			return adobjectId.GetChildId("CN", ADObject.ConfigurationUnits);
		}

		internal static bool IsTenantConfigInDomainNC(string partitionFqdn)
		{
			TenantCULocation tenantCULocation = InternalDirectoryRootOrganizationCache.GetTenantCULocation(partitionFqdn);
			if (tenantCULocation == TenantCULocation.Undefined)
			{
				if (Globals.IsDatacenter)
				{
					ADSystemConfigurationSession.GetRootOrgContainer(partitionFqdn, null, null);
					tenantCULocation = InternalDirectoryRootOrganizationCache.GetTenantCULocation(partitionFqdn);
				}
				else
				{
					tenantCULocation = TenantCULocation.ConfigNC;
					if (PartitionId.IsLocalForestPartition(partitionFqdn))
					{
						InternalDirectoryRootOrganizationCache.InitializeForestModeFlagForSetup(partitionFqdn, tenantCULocation);
					}
				}
			}
			return tenantCULocation == TenantCULocation.DomainNC;
		}

		internal static void InitializeForestModeFlagForLocalForest()
		{
			string localForestFqdn = TopologyProvider.LocalForestFqdn;
			try
			{
				ADSession.IsTenantConfigInDomainNC(localForestFqdn);
			}
			catch (OrgContainerNotFoundException)
			{
				if (Globals.IsDatacenter)
				{
					if (Globals.IsMicrosoftHostedOnly)
					{
						InternalDirectoryRootOrganizationCache.InitializeForestModeFlagForSetup(localForestFqdn, TenantCULocation.DomainNC);
					}
					else
					{
						InternalDirectoryRootOrganizationCache.InitializeForestModeFlagForSetup(localForestFqdn, TenantCULocation.ConfigNC);
					}
				}
				else
				{
					InternalDirectoryRootOrganizationCache.InitializeForestModeFlagForSetup(localForestFqdn, TenantCULocation.ConfigNC);
				}
			}
			finally
			{
				InternalDirectoryRootOrganizationCache.GetTenantCULocation(localForestFqdn);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldFilterSoftDeleteObject(ADSessionSettings sessionSettings, ADObjectId id)
		{
			return !sessionSettings.IncludeSoftDeletedObjects && !sessionSettings.IncludeInactiveMailbox && -1 != id.DistinguishedName.IndexOf(",OU=Soft Deleted Objects,", StringComparison.OrdinalIgnoreCase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldFilterCNFObject(ADSessionSettings sessionSettings, ADObjectId id)
		{
			return !sessionSettings.IncludeCNFObject && ADSession.IsCNFObject(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCNFObject(ADObjectId id)
		{
			return id.DistinguishedName.IndexOf("\\0ACNF", StringComparison.OrdinalIgnoreCase) > 0;
		}

		private const string CollisionObjectSig = "\\0ACNF";

		private static bool isAdminModeEnabled = true;

		internal enum ADNamingContext
		{
			RootDomain = 1,
			Domain,
			Config = 4,
			Schema = 8
		}
	}
}
