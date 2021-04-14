using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Data.Directory
{
	internal class GlsDirectorySession : IGlobalDirectorySession
	{
		internal static GlsCacheServiceMode GlsCacheServiceMode
		{
			get
			{
				return ConfigBase<AdDriverConfigSchema>.GetConfig<GlsCacheServiceMode>("GlsCacheServiceMode");
			}
		}

		private static bool TryGetOverrideTenant(Guid tenantGuid, out GlobalLocatorServiceTenant tenantOverride)
		{
			tenantOverride = null;
			if (GlsDirectorySession.glsTenantOverridesNextRefresh < DateTime.UtcNow)
			{
				GlsDirectorySession.glsTenantOverridesNextRefresh = DateTime.UtcNow + GlsDirectorySession.ADConfigurationSettingsRefreshPeriod;
				try
				{
					GlsDirectorySession.glsTenantOverrides = (ConfigBase<AdDriverConfigSchema>.GetConfig<GlsOverrideCollection>("GlsTenantOverrides") ?? new GlsOverrideCollection(null));
				}
				catch (ConfigurationSettingsException ex)
				{
					ExTraceGlobals.GLSTracer.TraceWarning<string>(0L, "Unable to refresh GLS overrides after getting configuration exception:{0}", ex.Message);
				}
			}
			return GlsDirectorySession.glsTenantOverrides.TryGetTenantOverride(tenantGuid, out tenantOverride);
		}

		private static DirectoryServiceProxyPool<LocatorService> ServiceProxyPool
		{
			get
			{
				if (GlsDirectorySession.serviceProxyPool == null)
				{
					lock (GlsDirectorySession.proxyPoolLockObj)
					{
						if (GlsDirectorySession.serviceProxyPool == null)
						{
							GlsDirectorySession.serviceProxyPool = GlsDirectorySession.CreateServiceProxyPool();
						}
					}
				}
				return GlsDirectorySession.serviceProxyPool;
			}
		}

		private static DirectoryServiceProxyPool<LocatorService> OfflineServiceProxyPool
		{
			get
			{
				if (GlsDirectorySession.offlineServiceProxyPool == null)
				{
					lock (GlsDirectorySession.proxyPoolLockObj)
					{
						if (GlsDirectorySession.offlineServiceProxyPool == null)
						{
							GlsDirectorySession.offlineServiceProxyPool = GlsDirectorySession.CreateOfflineServiceProxyPool();
						}
					}
				}
				return GlsDirectorySession.offlineServiceProxyPool;
			}
		}

		private static ServiceEndpoint LoadServiceEndpoint()
		{
			ServiceEndpoint endpoint = LocatorServiceClientConfiguration.Instance.Endpoint;
			if (endpoint != null)
			{
				return endpoint;
			}
			ServiceEndpoint endpoint2;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 291, "LoadServiceEndpoint", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\GlsDirectorySession.cs");
				ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
				endpoint2 = endpointContainer.GetEndpoint("GlobalLocatorService");
			}
			catch (ADTransientException ex)
			{
				throw new GlsTransientException(ex.LocalizedString, ex);
			}
			catch (ADExternalException ex2)
			{
				throw new GlsPermanentException(ex2.LocalizedString, ex2);
			}
			return endpoint2;
		}

		private static Exception GetTransientWrappedException(Exception wcfException, string targetInfo)
		{
			if (wcfException is TimeoutException)
			{
				return new GlsTransientException(DirectoryStrings.TimeoutGlsError(wcfException.Message), wcfException);
			}
			if (wcfException is EndpointNotFoundException)
			{
				return new GlsTransientException(DirectoryStrings.GlsEndpointNotFound(targetInfo, wcfException.ToString()), wcfException);
			}
			return new GlsTransientException(DirectoryStrings.TransientGlsError(wcfException.Message), wcfException);
		}

		private static Exception GetPermanentWrappedException(Exception wcfException, string targetInfo)
		{
			return new GlsPermanentException(DirectoryStrings.PermanentGlsError(wcfException.Message), wcfException);
		}

		private static DirectoryServiceProxyPool<LocatorService> CreateServiceProxyPool()
		{
			List<WSHttpBinding> list = new List<WSHttpBinding>();
			foreach (TimeSpan sendTimeout in GlsDirectorySession.SendTimeouts)
			{
				WSHttpBinding wshttpBinding = new WSHttpBinding(SecurityMode.Transport)
				{
					ReceiveTimeout = TimeSpan.FromSeconds(30.0),
					SendTimeout = sendTimeout,
					MaxBufferPoolSize = 524288L,
					MaxReceivedMessageSize = 65536L
				};
				wshttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
				list.Add(wshttpBinding);
			}
			ServiceEndpoint serviceEndpoint = GlsDirectorySession.LoadServiceEndpoint();
			GlsDirectorySession.endpointHostName = serviceEndpoint.Uri.Host;
			return DirectoryServiceProxyPool<LocatorService>.CreateDirectoryServiceProxyPool("GlobalLocatorService", serviceEndpoint, ExTraceGlobals.GLSTracer, 150, list, new GetWrappedExceptionDelegate(GlsDirectorySession.GetTransientWrappedException), new GetWrappedExceptionDelegate(GlsDirectorySession.GetPermanentWrappedException), DirectoryEventLogConstants.Tuple_CannotContactGLS, false);
		}

		private static DirectoryServiceProxyPool<LocatorService> CreateOfflineServiceProxyPool()
		{
			return DirectoryServiceProxyPool<LocatorService>.CreateDirectoryServiceProxyPool("GlsCacheService", new ServiceEndpoint(new Uri("net.pipe://localhost/GlsCacheService/service.svc")), ExTraceGlobals.GLSTracer, 150, new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new GetWrappedExceptionDelegate(GlsDirectorySession.GetTransientWrappedException), new GetWrappedExceptionDelegate(GlsDirectorySession.GetPermanentWrappedException), DirectoryEventLogConstants.Tuple_CannotContactGLS, false);
		}

		public static GlsOverrideFlag GetTenantOverrideStatus(string externalDirectoryOrganizationId)
		{
			GlsOverrideFlag glsOverrideFlag = GlsOverrideFlag.None;
			Guid guid;
			GlobalLocatorServiceTenant globalLocatorServiceTenant;
			if (!string.IsNullOrWhiteSpace(externalDirectoryOrganizationId) && Guid.TryParse(externalDirectoryOrganizationId, out guid) && GlsDirectorySession.TryGetOverrideTenant(guid, out globalLocatorServiceTenant))
			{
				glsOverrideFlag = GlsOverrideFlag.OverrideIsSet;
				GlobalLocatorServiceTenant globalLocatorServiceTenant2;
				if (new GlsDirectorySession().TryGetTenantInfoByOrgGuid(guid, out globalLocatorServiceTenant2, GlsDirectorySession.GlsCacheServiceMode, true))
				{
					if (string.Compare(globalLocatorServiceTenant2.ResourceForest, globalLocatorServiceTenant.ResourceForest, CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase) != 0)
					{
						glsOverrideFlag |= (GlsOverrideFlag.GlsRecordMismatch | GlsOverrideFlag.ResourceForestMismatch);
					}
					if (string.Compare(globalLocatorServiceTenant2.AccountForest, globalLocatorServiceTenant.AccountForest, CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase) != 0)
					{
						glsOverrideFlag |= (GlsOverrideFlag.GlsRecordMismatch | GlsOverrideFlag.AccountForestMismatch);
					}
				}
			}
			return glsOverrideFlag;
		}

		internal GlsDirectorySession(GlsCallerId glsCallerId, GlsAPIReadFlag globalLocaterServiceReadFlag)
		{
			this.glsCallerId = glsCallerId;
			this.glsReadFlag = globalLocaterServiceReadFlag;
		}

		internal GlsDirectorySession(GlsAPIReadFlag globalLocaterServiceReadFlag) : this(GlsCallerId.Exchange, globalLocaterServiceReadFlag)
		{
		}

		internal GlsDirectorySession(GlsCallerId glsCallerId) : this(glsCallerId, GlsAPIReadFlag.Default)
		{
		}

		internal GlsDirectorySession() : this(GlsCallerId.Exchange)
		{
		}

		public string GetRedirectServer(string memberName)
		{
			string result;
			if (!this.TryGetRedirectServer(memberName, out result))
			{
				throw new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(memberName));
			}
			return result;
		}

		public bool TryGetRedirectServer(string memberName, out string fqdn)
		{
			string empty = string.Empty;
			bool result = this.TryGetRedirectServer(memberName, GlsDirectorySession.GlsCacheServiceMode, out empty);
			fqdn = empty;
			return result;
		}

		public bool TryGetRedirectServer(string memberName, GlsCacheServiceMode glsCacheServiceMode, out string fqdn)
		{
			fqdn = string.Empty;
			SmtpAddress smtpAddress = GlsDirectorySession.ParseMemberName(memberName);
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(new SmtpDomain(smtpAddress.Domain), GlsDirectorySession.noDomainProperties, GlsDirectorySession.resourceForestProperty, this.glsReadFlag);
			GlsLoggerContext glsLoggerContext;
			FindDomainResponse response = this.ExecuteWithRetry<FindDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindDomain(requestIdentity, request), "TryGetRedirectServer", "FindDomain", smtpAddress.Domain, true, glsCacheServiceMode, out glsLoggerContext);
			FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsLoggerContext, Namespace.Exo);
			if (findDomainResult == null)
			{
				return false;
			}
			fqdn = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOResourceForest).GetStringValue();
			return this.IsValidForestName(memberName, fqdn, glsLoggerContext);
		}

		public string GetRedirectServer(Guid orgGuid)
		{
			string result;
			if (!this.TryGetRedirectServer(orgGuid, out result))
			{
				throw new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(orgGuid.ToString()));
			}
			return result;
		}

		public bool TryGetRedirectServer(Guid orgGuid, out string fqdn)
		{
			fqdn = string.Empty;
			FindTenantRequest request = this.ConstructFindTenantRequest(orgGuid, GlsDirectorySession.resourceForestProperty);
			GlsLoggerContext glsLoggerContext;
			FindTenantResponse response = this.ExecuteWithRetry<FindTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindTenant(requestIdentity, request), "TryGetRedirectServer", "FindTenant", orgGuid, true, out glsLoggerContext);
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(response, glsLoggerContext, Namespace.Exo);
			if (findTenantResult == null)
			{
				return false;
			}
			fqdn = findTenantResult.GetPropertyValue(TenantProperty.EXOResourceForest).GetStringValue();
			return this.IsValidForestName(orgGuid.ToString(), fqdn, glsLoggerContext);
		}

		public IAsyncResult BeginGetNextHop(SmtpDomain domain, object clientAsyncState, AsyncCallback clientCallback)
		{
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(domain, GlsDirectorySession.noDomainProperties, GlsDirectorySession.exoNextHopProperty, this.glsReadFlag);
			return this.BeginExecuteWithRetry(delegate(LocatorService proxy, RequestIdentity requestIdentity, GlsAsyncResult asyncResult)
			{
				IAsyncResult internalAsyncResult = proxy.BeginFindDomain(requestIdentity, request, new AsyncCallback(this.OnAsyncRequestCompleted), asyncResult);
				asyncResult.InternalAsyncResult = internalAsyncResult;
			}, clientCallback, clientAsyncState, "BeginFindDomain", "FindDomain", domain, true);
		}

		public bool TryEndGetNextHop(IAsyncResult asyncResult, out string nextHop, out Guid tenantId)
		{
			nextHop = null;
			tenantId = Guid.Empty;
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)asyncResult;
			FindDomainResponse response = this.EndExecuteWithRetry<FindDomainResponse>(glsAsyncResult, "EndGetNextHop", (LocatorService proxy) => proxy.EndFindDomain(glsAsyncResult.InternalAsyncResult));
			FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsAsyncResult.LoggerContext, Namespace.Exo);
			if (findDomainResult == null)
			{
				return false;
			}
			nextHop = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOSmtpNextHopDomain).GetStringValue();
			tenantId = findDomainResult.TenantId;
			return true;
		}

		public bool DomainExists(string domainFqdn, Namespace[] namespaceArray)
		{
			SmtpDomain domain = this.ParseDomain(domainFqdn);
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(domain, GlsDirectorySession.ffoExoDomainProperties, GlsDirectorySession.noTenantProperties, this.glsReadFlag);
			GlsLoggerContext glsLoggerContext;
			FindDomainResponse response = this.ExecuteWithRetry<FindDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindDomain(requestIdentity, request), "DomainExists", "FindDomain", domainFqdn, true, out glsLoggerContext);
			foreach (Namespace namespaceToCheck in namespaceArray)
			{
				FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsLoggerContext, namespaceToCheck);
				if (findDomainResult != null && !string.IsNullOrEmpty(findDomainResult.Domain))
				{
					return true;
				}
			}
			return false;
		}

		internal bool DomainExists(string domainFqdn)
		{
			return this.DomainExists(domainFqdn, new Namespace[]
			{
				this.glsCallerId.DefaultNamespace
			});
		}

		public bool TryGetDomainFlag(string domainFqdn, GlsDomainFlags flag, out bool value)
		{
			SmtpDomain smtpDomain = this.ParseDomain(domainFqdn);
			BitVector32 bitVector;
			Guid guid;
			if (!this.TryGetExoDomainFlags(smtpDomain, out bitVector, out guid))
			{
				value = false;
				return false;
			}
			value = bitVector[(int)flag];
			return true;
		}

		public void SetDomainFlag(string domainFqdn, GlsDomainFlags flag, bool value)
		{
			SmtpDomain smtpDomain = this.ParseDomain(domainFqdn);
			BitVector32 bitVector;
			Guid externalDirectoryOrganizationId;
			if (!this.TryGetExoDomainFlags(smtpDomain, out bitVector, out externalDirectoryOrganizationId))
			{
				throw new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(domainFqdn));
			}
			bitVector[(int)flag] = value;
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ExoFlags, new PropertyValue(bitVector.Data));
			this.SaveDomain("SetDomainFlag", externalDirectoryOrganizationId, smtpDomain, DomainKeyType.UseExisting, new KeyValuePair<DomainProperty, PropertyValue>[]
			{
				keyValuePair
			});
		}

		public bool TryGetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags flag, out bool value)
		{
			BitVector32 bitVector;
			if (!this.TryGetExoTenantFlags(externalDirectoryOrganizationId, out bitVector))
			{
				value = false;
				return false;
			}
			value = bitVector[(int)flag];
			return true;
		}

		public void SetTenantFlag(Guid externalDirectoryOrganizationId, GlsTenantFlags tenantFlags, bool value)
		{
			BitVector32 bitVector;
			if (!this.TryGetExoTenantFlags(externalDirectoryOrganizationId, out bitVector))
			{
				throw new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(externalDirectoryOrganizationId.ToString()));
			}
			bitVector[(int)tenantFlags] = value;
			KeyValuePair<TenantProperty, PropertyValue> keyValuePair = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOTenantFlags, new PropertyValue(bitVector.Data));
			this.SaveTenant("SetTenantFlag", externalDirectoryOrganizationId, new KeyValuePair<TenantProperty, PropertyValue>[]
			{
				keyValuePair
			});
		}

		public void AddTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN)
		{
			this.AddTenant(externalDirectoryOrganizationId, resourceForestFqdn, accountForestFqdn, smtpNextHopDomain, tenantFlags, tenantContainerCN, resourceForestFqdn);
		}

		public void AddTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN, string primarySite)
		{
			if (tenantContainerCN != null && (tenantContainerCN == string.Empty || tenantContainerCN.Length > 64))
			{
				throw new ArgumentException(tenantContainerCN);
			}
			List<KeyValuePair<TenantProperty, PropertyValue>> list = new List<KeyValuePair<TenantProperty, PropertyValue>>();
			list.Add(new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOResourceForest, new PropertyValue(resourceForestFqdn)));
			list.Add(new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOAccountForest, new PropertyValue(accountForestFqdn)));
			list.Add(new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOSmtpNextHopDomain, new PropertyValue(smtpNextHopDomain)));
			list.Add(new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOPrimarySite, new PropertyValue(primarySite)));
			list.Add(new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOTenantFlags, new PropertyValue((int)tenantFlags)));
			if (tenantContainerCN != null)
			{
				list.Add(new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOTenantContainerCN, new PropertyValue(tenantContainerCN)));
			}
			this.SaveTenant("AddTenant", externalDirectoryOrganizationId, list.ToArray());
		}

		public void AddMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId)
		{
			this.SaveMSAUser("AddUser", msaUserNetID, msaUserMemberName, externalDirectoryOrganizationId);
		}

		public void AddTenant(Guid externalDirectoryOrganizationId, CustomerType tenantType, string ffoRegion, string ffoVersion)
		{
			KeyValuePair<TenantProperty, PropertyValue> keyValuePair = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.CustomerType, new PropertyValue((int)tenantType));
			KeyValuePair<TenantProperty, PropertyValue> keyValuePair2 = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.Region, new PropertyValue(ffoRegion));
			KeyValuePair<TenantProperty, PropertyValue> keyValuePair3 = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.Version, new PropertyValue(ffoVersion));
			this.SaveTenant("AddTenant", externalDirectoryOrganizationId, new KeyValuePair<TenantProperty, PropertyValue>[]
			{
				keyValuePair,
				keyValuePair2,
				keyValuePair3
			});
		}

		public void UpdateTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN)
		{
			this.UpdateTenant(externalDirectoryOrganizationId, resourceForestFqdn, accountForestFqdn, smtpNextHopDomain, tenantFlags, tenantContainerCN, resourceForestFqdn);
		}

		public void UpdateTenant(Guid externalDirectoryOrganizationId, string resourceForestFqdn, string accountForestFqdn, string smtpNextHopDomain, GlsTenantFlags tenantFlags, string tenantContainerCN, string primarySite)
		{
			this.AddTenant(externalDirectoryOrganizationId, resourceForestFqdn, accountForestFqdn, smtpNextHopDomain, tenantFlags, tenantContainerCN, primarySite);
		}

		public void UpdateMSAUser(string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId)
		{
			this.SaveMSAUser("UpdateUser", msaUserNetID, msaUserMemberName, externalDirectoryOrganizationId);
		}

		public void RemoveTenant(Guid externalDirectoryOrganizationId)
		{
			DeleteTenantRequest request = LocatorServiceClientWriter.ConstructDeleteTenantRequest(externalDirectoryOrganizationId, this.glsCallerId.DefaultNamespace);
			this.ExecuteWriteWithRetry<DeleteTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.DeleteTenant(requestIdentity, request), "RemoveTenant", "DeleteTenant", externalDirectoryOrganizationId.ToString());
		}

		public void RemoveMSAUser(string msaUserNetID)
		{
			DeleteUserRequest request = this.ConstructDeleteUserRequest(msaUserNetID);
			this.ExecuteWriteWithRetry<DeleteUserResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.DeleteUser(requestIdentity, request), "RemoveUser", "DeleteUser", msaUserNetID);
		}

		public bool TryGetTenantType(Guid externalDirectoryOrganizationId, out CustomerType tenantType)
		{
			FindTenantRequest request = this.ConstructFindTenantRequest(externalDirectoryOrganizationId, GlsDirectorySession.customerTypeProperty);
			GlsLoggerContext glsLoggerContext;
			FindTenantResponse response = this.ExecuteWithRetry<FindTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindTenant(requestIdentity, request), "TryGetTenantType", "FindTenant", externalDirectoryOrganizationId, true, out glsLoggerContext);
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(response, glsLoggerContext, Namespace.Ffo);
			if (findTenantResult == null)
			{
				tenantType = CustomerType.None;
				return false;
			}
			tenantType = (CustomerType)findTenantResult.GetPropertyValue(TenantProperty.CustomerType).GetIntValue();
			return true;
		}

		public bool TryGetTenantForestsByDomain(string domainFqdn, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string smtpNextHopDomain, out string tenantContainerCN)
		{
			bool flag;
			return this.TryGetTenantForestsByDomain(domainFqdn, out externalDirectoryOrganizationId, out resourceForestFqdn, out accountForestFqdn, out smtpNextHopDomain, out tenantContainerCN, out flag);
		}

		public bool TryGetTenantForestsByDomain(string domainFqdn, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string smtpNextHopDomain, out string tenantContainerCN, out bool dataFromOfflineService)
		{
			GlobalLocatorServiceTenant globalLocatorServiceTenant;
			if (!this.TryGetTenantInfoByDomain(domainFqdn, out globalLocatorServiceTenant))
			{
				resourceForestFqdn = null;
				accountForestFqdn = null;
				externalDirectoryOrganizationId = Guid.Empty;
				smtpNextHopDomain = null;
				tenantContainerCN = null;
				dataFromOfflineService = false;
				return false;
			}
			resourceForestFqdn = globalLocatorServiceTenant.ResourceForest;
			accountForestFqdn = globalLocatorServiceTenant.AccountForest;
			smtpNextHopDomain = globalLocatorServiceTenant.SmtpNextHopDomain.Domain;
			tenantContainerCN = globalLocatorServiceTenant.TenantContainerCN;
			externalDirectoryOrganizationId = globalLocatorServiceTenant.ExternalDirectoryOrganizationId;
			dataFromOfflineService = globalLocatorServiceTenant.IsOfflineData;
			return true;
		}

		public bool TryGetTenantInfoByDomain(string domainFqdn, out GlobalLocatorServiceTenant glsTenant)
		{
			return this.TryGetTenantInfoByDomain(domainFqdn, out glsTenant, GlsDirectorySession.GlsCacheServiceMode);
		}

		public bool TryGetTenantInfoByDomain(string domainFqdn, out GlobalLocatorServiceTenant glsTenant, GlsCacheServiceMode glsCacheServiceMode)
		{
			SmtpDomain domain = this.ParseDomain(domainFqdn);
			glsTenant = new GlobalLocatorServiceTenant();
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(domain, GlsDirectorySession.AllExoDomainProperties, GlsDirectorySession.AllExoTenantProperties, this.glsReadFlag);
			GlsLoggerContext glsLoggerContext;
			FindDomainResponse response = this.ExecuteWithRetry<FindDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindDomain(requestIdentity, request), "TryGetTenantInfoByDomain", "FindDomain", domainFqdn, true, glsCacheServiceMode, out glsLoggerContext);
			FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsLoggerContext, Namespace.Exo);
			if (findDomainResult == null || !findDomainResult.HasTenantProperties())
			{
				return false;
			}
			glsTenant.ResourceForest = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOResourceForest).GetStringValue();
			glsTenant.AccountForest = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOAccountForest).GetStringValue();
			glsTenant.SmtpNextHopDomain = new SmtpDomain(findDomainResult.GetTenantPropertyValue(TenantProperty.EXOSmtpNextHopDomain).GetStringValue());
			glsTenant.TenantContainerCN = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOTenantContainerCN).GetStringValue();
			glsTenant.ResumeCache = findDomainResult.GetTenantPropertyValue(TenantProperty.GlobalResumeCache).GetStringValue();
			glsTenant.PrimarySite = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOPrimarySite).GetStringValue();
			glsTenant.TenantFlags = (GlsTenantFlags)findDomainResult.GetTenantPropertyValue(TenantProperty.EXOTenantFlags).GetIntValue();
			glsTenant.ExternalDirectoryOrganizationId = findDomainResult.TenantId;
			glsTenant.IsOfflineData = this.IsDataReturnedFromOfflineService(glsLoggerContext);
			GlobalLocatorServiceTenant globalLocatorServiceTenant;
			if (GlsDirectorySession.TryGetOverrideTenant(glsTenant.ExternalDirectoryOrganizationId, out globalLocatorServiceTenant))
			{
				glsTenant = globalLocatorServiceTenant;
				return true;
			}
			return this.ValidateMandatoryTenantProperties(glsTenant, glsLoggerContext);
		}

		public bool TryGetTenantForestsByOrgGuid(Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN)
		{
			bool flag;
			return this.TryGetTenantForestsByOrgGuid(externalDirectoryOrganizationId, out resourceForestFqdn, out accountForestFqdn, out tenantContainerCN, out flag);
		}

		public bool TryGetTenantForestsByOrgGuid(Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN, out bool dataFromOfflineService)
		{
			GlobalLocatorServiceTenant globalLocatorServiceTenant;
			if (!this.TryGetTenantInfoByOrgGuid(externalDirectoryOrganizationId, out globalLocatorServiceTenant))
			{
				resourceForestFqdn = null;
				accountForestFqdn = null;
				tenantContainerCN = null;
				dataFromOfflineService = false;
				return false;
			}
			resourceForestFqdn = globalLocatorServiceTenant.ResourceForest;
			accountForestFqdn = globalLocatorServiceTenant.AccountForest;
			tenantContainerCN = globalLocatorServiceTenant.TenantContainerCN;
			dataFromOfflineService = globalLocatorServiceTenant.IsOfflineData;
			return true;
		}

		public bool TryGetTenantInfoByOrgGuid(Guid externalDirectoryOrganizationId, out GlobalLocatorServiceTenant glsTenant)
		{
			return this.TryGetTenantInfoByOrgGuid(externalDirectoryOrganizationId, out glsTenant, GlsDirectorySession.GlsCacheServiceMode);
		}

		public bool TryGetTenantInfoByOrgGuid(Guid externalDirectoryOrganizationId, out GlobalLocatorServiceTenant glsTenant, GlsCacheServiceMode glsCacheServiceMode)
		{
			return this.TryGetTenantInfoByOrgGuid(externalDirectoryOrganizationId, out glsTenant, glsCacheServiceMode, false);
		}

		public bool TryGetTenantForestsByMSAUserNetID(string userNetID, out Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN)
		{
			GlobalLocatorServiceTenant globalLocatorServiceTenant;
			if (!this.TryGetTenantInfoByMSAUserNetID(userNetID, out globalLocatorServiceTenant))
			{
				resourceForestFqdn = null;
				accountForestFqdn = null;
				tenantContainerCN = null;
				externalDirectoryOrganizationId = Guid.Empty;
				return false;
			}
			externalDirectoryOrganizationId = globalLocatorServiceTenant.ExternalDirectoryOrganizationId;
			resourceForestFqdn = globalLocatorServiceTenant.ResourceForest;
			accountForestFqdn = globalLocatorServiceTenant.AccountForest;
			tenantContainerCN = globalLocatorServiceTenant.TenantContainerCN;
			return true;
		}

		public bool TryGetTenantInfoByMSAUserNetID(string msaUserNetID, out GlobalLocatorServiceTenant glsTenant)
		{
			glsTenant = new GlobalLocatorServiceTenant();
			FindUserRequest request = this.ConstructFindUserRequest(msaUserNetID, GlsDirectorySession.AllExoTenantProperties);
			GlsLoggerContext glsLoggerContext;
			FindUserResponse response = this.ExecuteWithRetry<FindUserResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindUser(requestIdentity, request), "TryGetTenantInfoByMSAUserNetID", "FindUser", msaUserNetID, true, out glsLoggerContext);
			FindUserResult findUserResult = this.ConstructFindUserResult(response, glsLoggerContext);
			if (findUserResult == null)
			{
				return false;
			}
			glsTenant.ResourceForest = findUserResult.GetTenantPropertyValue(TenantProperty.EXOResourceForest).GetStringValue();
			glsTenant.AccountForest = findUserResult.GetTenantPropertyValue(TenantProperty.EXOAccountForest).GetStringValue();
			glsTenant.TenantContainerCN = findUserResult.GetTenantPropertyValue(TenantProperty.EXOTenantContainerCN).GetStringValue();
			glsTenant.ResumeCache = findUserResult.GetTenantPropertyValue(TenantProperty.GlobalResumeCache).GetStringValue();
			glsTenant.PrimarySite = findUserResult.GetTenantPropertyValue(TenantProperty.EXOPrimarySite).GetStringValue();
			glsTenant.SmtpNextHopDomain = new SmtpDomain(findUserResult.GetTenantPropertyValue(TenantProperty.EXOSmtpNextHopDomain).GetStringValue());
			glsTenant.TenantFlags = (GlsTenantFlags)findUserResult.GetTenantPropertyValue(TenantProperty.EXOTenantFlags).GetIntValue();
			glsTenant.ExternalDirectoryOrganizationId = findUserResult.TenantId;
			return this.ValidateMandatoryTenantProperties(glsTenant, glsLoggerContext);
		}

		public void SetTenantEntryResumeCacheTime(Guid externalDirectoryOrganizationId, string dateTimeString)
		{
			if (string.IsNullOrEmpty(dateTimeString))
			{
				throw new ArgumentNullException(dateTimeString);
			}
			KeyValuePair<TenantProperty, PropertyValue>[] tenantProperties = new KeyValuePair<TenantProperty, PropertyValue>[]
			{
				new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.GlobalResumeCache, new PropertyValue(dateTimeString))
			};
			this.SaveTenant("SetTenantEntryResumeCacheTime", externalDirectoryOrganizationId, tenantProperties);
		}

		public void SetAccountForest(Guid externalDirectoryOrganizationId, string value, string tenantContainerCN = null)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException(value);
			}
			KeyValuePair<TenantProperty, PropertyValue> keyValuePair = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOAccountForest, new PropertyValue(value));
			KeyValuePair<TenantProperty, PropertyValue>[] tenantProperties;
			if (string.IsNullOrEmpty(tenantContainerCN))
			{
				tenantProperties = new KeyValuePair<TenantProperty, PropertyValue>[]
				{
					keyValuePair
				};
			}
			else
			{
				KeyValuePair<TenantProperty, PropertyValue> keyValuePair2 = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOTenantContainerCN, new PropertyValue(tenantContainerCN));
				tenantProperties = new KeyValuePair<TenantProperty, PropertyValue>[]
				{
					keyValuePair,
					keyValuePair2
				};
			}
			this.SaveTenant("SetAccountForest", externalDirectoryOrganizationId, tenantProperties);
		}

		public void SetResourceForest(Guid externalDirectoryOrganizationId, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException(value);
			}
			KeyValuePair<TenantProperty, PropertyValue> keyValuePair = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.EXOResourceForest, new PropertyValue(value));
			this.SaveTenant("SetResourceForest", externalDirectoryOrganizationId, new KeyValuePair<TenantProperty, PropertyValue>[]
			{
				keyValuePair
			});
		}

		public void SetTenantVersion(Guid externalDirectoryOrganizationId, string newTenantVersion)
		{
			if (string.IsNullOrEmpty(newTenantVersion))
			{
				throw new ArgumentNullException(newTenantVersion);
			}
			KeyValuePair<TenantProperty, PropertyValue> keyValuePair = new KeyValuePair<TenantProperty, PropertyValue>(TenantProperty.Version, new PropertyValue(newTenantVersion));
			this.SaveTenant("SetTenantVersion", externalDirectoryOrganizationId, new KeyValuePair<TenantProperty, PropertyValue>[]
			{
				keyValuePair
			});
		}

		public bool TryGetTenantDomains(Guid externalDirectoryOrganizationId, out string[] acceptedDomainFqdns)
		{
			throw new NotImplementedException();
		}

		public bool TryGetTenantDomainFromDomainFqdn(string domainFqdn, out GlobalLocatorServiceDomain glsDomain)
		{
			return this.TryGetTenantDomainFromDomainFqdn(domainFqdn, out glsDomain, GlsDirectorySession.GlsCacheServiceMode);
		}

		public bool TryGetTenantDomainFromDomainFqdn(string domainFqdn, out GlobalLocatorServiceDomain glsDomain, GlsCacheServiceMode glsCacheServiceMode)
		{
			return this.TryGetTenantDomainFromDomainFqdn(domainFqdn, out glsDomain, false, glsCacheServiceMode);
		}

		public bool TryGetTenantDomainFromDomainFqdn(string domainFqdn, out GlobalLocatorServiceDomain glsDomain, bool skipTenantCheck)
		{
			return this.TryGetTenantDomainFromDomainFqdn(domainFqdn, out glsDomain, skipTenantCheck, GlsDirectorySession.GlsCacheServiceMode);
		}

		public bool TryGetTenantDomainFromDomainFqdn(string domainFqdn, out GlobalLocatorServiceDomain glsDomain, bool skipTenantCheck, GlsCacheServiceMode glsCacheServiceMode)
		{
			SmtpDomain domain = this.ParseDomain(domainFqdn);
			glsDomain = new GlobalLocatorServiceDomain();
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(domain, GlsDirectorySession.AllExoDomainProperties, GlsDirectorySession.AllExoTenantProperties, this.glsReadFlag);
			GlsLoggerContext glsLoggerContext;
			FindDomainResponse response = this.ExecuteWithRetry<FindDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindDomain(requestIdentity, request), "TryGetTenantDomainFromDomainFqdn", "FindDomain", domainFqdn, true, glsCacheServiceMode, out glsLoggerContext);
			FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsLoggerContext, Namespace.Exo, Namespace.Exo, skipTenantCheck);
			if (findDomainResult == null)
			{
				return false;
			}
			glsDomain.DomainName = new SmtpDomain(findDomainResult.Domain);
			glsDomain.DomainInUse = findDomainResult.GetDomainPropertyValue(DomainProperty.ExoDomainInUse).GetBoolValue();
			PropertyValue domainPropertyValue = findDomainResult.GetDomainPropertyValue(DomainProperty.ExoFlags);
			if (domainPropertyValue != null && Enum.IsDefined(typeof(GlsDomainFlags), domainPropertyValue.GetIntValue()))
			{
				glsDomain.DomainFlags = new GlsDomainFlags?((GlsDomainFlags)domainPropertyValue.GetIntValue());
			}
			else
			{
				glsDomain.DomainFlags = null;
			}
			glsDomain.ExternalDirectoryOrganizationId = findDomainResult.TenantId;
			return true;
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain)
		{
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, isInitialDomain, false, false);
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, string ffoRegion, string ffoServiceVersion)
		{
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.Region, new PropertyValue(ffoRegion));
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair2 = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ServiceVersion, new PropertyValue(ffoServiceVersion));
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair3 = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.DomainInUse, new PropertyValue(true));
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, isInitialDomain ? DomainKeyType.InitialDomain : DomainKeyType.CustomDomain, new KeyValuePair<DomainProperty, PropertyValue>[]
			{
				keyValuePair,
				keyValuePair2,
				keyValuePair3
			});
		}

		public void UpdateAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn)
		{
			SmtpDomain smtpDomain = this.ParseDomain(domainFqdn);
			BitVector32 bitVector;
			Guid guid;
			if (!this.TryGetExoDomainFlags(smtpDomain, out bitVector, out guid))
			{
				throw new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(domainFqdn));
			}
			if (!bitVector[1])
			{
				return;
			}
			bool nego2Enabled = false;
			bool oauth2ClientProfileEnabled = bitVector[2];
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, DomainKeyType.UseExisting, nego2Enabled, oauth2ClientProfileEnabled);
		}

		public void UpdateAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, KeyValuePair<DomainProperty, PropertyValue>[] properties)
		{
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, DomainKeyType.UseExisting, properties);
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, bool nego2Enabled, bool oauth2ClientProfileEnabled)
		{
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, isInitialDomain ? DomainKeyType.InitialDomain : DomainKeyType.CustomDomain, nego2Enabled, oauth2ClientProfileEnabled);
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool isInitialDomain, KeyValuePair<DomainProperty, PropertyValue>[] properties)
		{
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, isInitialDomain ? DomainKeyType.InitialDomain : DomainKeyType.CustomDomain, properties);
		}

		public void GetFfoTenantSettingsByDomain(string domain, out Guid tenantId, out string region, out string version, out CustomerType tenantType)
		{
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(new SmtpDomain(domain), GlsDirectorySession.ffoDomainProperties, GlsDirectorySession.customerTypeProperty, this.glsReadFlag);
			GlsLoggerContext glsLoggerContext;
			FindDomainResponse response = this.ExecuteWithRetry<FindDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindDomain(requestIdentity, request), "GetFfoTenantSettingsByDomain", "FindDomain", domain, true, out glsLoggerContext);
			FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsLoggerContext, Namespace.Ffo);
			if (findDomainResult == null)
			{
				region = null;
				version = null;
				tenantId = Guid.Empty;
				tenantType = CustomerType.None;
				throw new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(domain));
			}
			tenantId = findDomainResult.TenantId;
			tenantType = (CustomerType)findDomainResult.GetTenantPropertyValue(TenantProperty.CustomerType).GetIntValue();
			region = findDomainResult.GetDomainPropertyValue(DomainProperty.Region).GetStringValue();
			version = findDomainResult.GetDomainPropertyValue(DomainProperty.ServiceVersion).GetStringValue();
		}

		public string GetFfoTenantRegionByOrgGuid(Guid orgGuid)
		{
			string result = null;
			FindTenantRequest request = this.ConstructFindTenantRequest(orgGuid, GlsDirectorySession.ffoTenantRegionProperty);
			GlsLoggerContext glsLoggerContext;
			FindTenantResponse findTenantResponse = this.ExecuteWithRetry<FindTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindTenant(requestIdentity, request), "GetFfoTenantRegionByOrgGuid", "FindTenant", orgGuid, true, out glsLoggerContext);
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(findTenantResponse, glsLoggerContext, Namespace.Ffo);
			if (findTenantResult == null && findTenantResponse.TenantInfo == null)
			{
				throw new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(orgGuid.ToString()));
			}
			if (findTenantResult != null)
			{
				result = findTenantResult.GetPropertyValue(TenantProperty.Region).GetStringValue();
			}
			return result;
		}

		public IEnumerable<string> GetDomainNamesProvisionedByEXO(IEnumerable<SmtpDomain> domains)
		{
			if (domains == null)
			{
				throw new ArgumentNullException("domains");
			}
			List<string> list = new List<string>(domains.Count<SmtpDomain>());
			FindDomainsRequest request = LocatorServiceClientReader.ConstructFindDomainsRequest(domains, GlsDirectorySession.exoDomainInUseProperty, GlsDirectorySession.noTenantProperties, this.glsReadFlag);
			GlsLoggerContext glsLoggerContext;
			FindDomainsResponse findDomainsResponse = this.ExecuteWithRetry<FindDomainsResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindDomains(requestIdentity, request), "GetDomainNamesProvisionedByEXO", "FindDomains", string.Join<SmtpDomain>(",", domains), true, out glsLoggerContext);
			foreach (FindDomainResponse findDomainResponse in findDomainsResponse.DomainsResponse)
			{
				findDomainResponse.TransactionID = findDomainsResponse.TransactionID;
				FindDomainResult findDomainResult = this.ConstructFindDomainResult(findDomainResponse, glsLoggerContext, Namespace.Exo);
				if (findDomainResult != null && findDomainResult.GetDomainPropertyValue(DomainProperty.ExoDomainInUse).GetBoolValue())
				{
					list.Add(findDomainResult.Domain);
				}
			}
			return list;
		}

		public bool TenantExists(Guid externalDirectoryOrganizationId, Namespace namespaceToCheck)
		{
			FindTenantRequest request = this.ConstructFindTenantRequest(externalDirectoryOrganizationId, GlsDirectorySession.ffoExoTenantProperties);
			GlsLoggerContext glsLoggerContext;
			FindTenantResponse response = this.ExecuteWithRetry<FindTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindTenant(requestIdentity, request), "TenantExists", "FindTenant", externalDirectoryOrganizationId, true, out glsLoggerContext);
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(response, glsLoggerContext, namespaceToCheck);
			return findTenantResult != null;
		}

		public bool MSAUserExists(string msaUserNetID)
		{
			FindUserRequest request = this.ConstructFindUserRequest(msaUserNetID, GlsDirectorySession.AllExoTenantProperties);
			GlsLoggerContext glsLoggerContext;
			FindUserResponse response = this.ExecuteWithRetry<FindUserResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindUser(requestIdentity, request), "MSAUserExists", "FindUser", msaUserNetID, true, out glsLoggerContext);
			FindUserResult findUserResult = this.ConstructFindUserResult(response, glsLoggerContext);
			return null != findUserResult;
		}

		public bool TryGetMSAUserMemberName(string msaUserNetID, out string msaUserMemberName)
		{
			FindUserRequest request = this.ConstructFindUserRequest(msaUserNetID, GlsDirectorySession.AllExoTenantProperties);
			GlsLoggerContext glsLoggerContext;
			FindUserResponse response = this.ExecuteWithRetry<FindUserResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindUser(requestIdentity, request), "TryGetMSAUserMemberName", "FindUser", msaUserNetID, true, out glsLoggerContext);
			FindUserResult findUserResult = this.ConstructFindUserResult(response, glsLoggerContext);
			if (findUserResult == null)
			{
				msaUserMemberName = null;
				return false;
			}
			msaUserMemberName = findUserResult.MSAUserMemberName;
			return true;
		}

		public bool TryGetFfoTenantProvisioningProperties(Guid externalDirectoryOrganizationId, out string version, out CustomerType tenantType, out string region)
		{
			FindTenantRequest request = this.ConstructFindTenantRequest(externalDirectoryOrganizationId, GlsDirectorySession.ffoTenantProperties);
			if (GlsCallerId.IsForwardSyncCallerID(this.glsCallerId))
			{
				request.ReadFlag = 2;
			}
			GlsLoggerContext glsLoggerContext;
			FindTenantResponse response = this.ExecuteWithRetry<FindTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindTenant(requestIdentity, request), "TryGetFfoTenantProvisioningProperties", "FindTenant", externalDirectoryOrganizationId, true, out glsLoggerContext);
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(response, glsLoggerContext, Namespace.Ffo);
			if (findTenantResult == null)
			{
				version = null;
				tenantType = CustomerType.None;
				region = null;
				return false;
			}
			tenantType = (CustomerType)findTenantResult.GetPropertyValue(TenantProperty.CustomerType).GetIntValue();
			version = findTenantResult.GetPropertyValue(TenantProperty.Version).GetStringValue();
			region = findTenantResult.GetPropertyValue(TenantProperty.Region).GetStringValue();
			return true;
		}

		public IAsyncResult BeginGetFfoTenantAttributionPropertiesByOrgId(Guid externalDirectoryOrganizationId, object clientAsyncState, AsyncCallback clientCallback)
		{
			FindTenantRequest request = this.ConstructFindTenantRequest(externalDirectoryOrganizationId, GlsDirectorySession.customerAttributionTenantProperties);
			return this.BeginExecuteWithRetry(delegate(LocatorService proxy, RequestIdentity requestIdentity, GlsAsyncResult asyncResult)
			{
				IAsyncResult internalAsyncResult = proxy.BeginFindTenant(requestIdentity, request, new AsyncCallback(this.OnAsyncRequestCompleted), asyncResult);
				asyncResult.InternalAsyncResult = internalAsyncResult;
			}, clientCallback, clientAsyncState, "BeginGetFfoTenantAttributionPropertiesByOrgId", "FindTenant", externalDirectoryOrganizationId, true);
		}

		public bool TryEndGetFfoTenantAttributionPropertiesByOrgId(IAsyncResult asyncResult, out string ffoRegion, out string exoNextHop, out CustomerType tenantType, out string exoResourceForest, out string exoAccountForest, out string exoTenantContainer)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)asyncResult;
			FindTenantResponse response = this.EndExecuteWithRetry<FindTenantResponse>(glsAsyncResult, "TryEndGetFfoTenantAttributionPropertiesByOrgId", (LocatorService proxy) => proxy.EndFindTenant(glsAsyncResult.InternalAsyncResult));
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(response, glsAsyncResult.LoggerContext, Namespace.Ffo);
			if (findTenantResult == null)
			{
				ffoRegion = null;
				exoNextHop = null;
				tenantType = CustomerType.None;
				exoResourceForest = null;
				exoAccountForest = null;
				exoTenantContainer = null;
				return false;
			}
			ffoRegion = findTenantResult.GetPropertyValue(TenantProperty.Region).GetStringValue();
			exoNextHop = findTenantResult.GetPropertyValue(TenantProperty.EXOSmtpNextHopDomain).GetStringValue();
			tenantType = (CustomerType)findTenantResult.GetPropertyValue(TenantProperty.CustomerType).GetIntValue();
			exoResourceForest = findTenantResult.GetPropertyValue(TenantProperty.EXOResourceForest).GetStringValue();
			exoAccountForest = findTenantResult.GetPropertyValue(TenantProperty.EXOAccountForest).GetStringValue();
			exoTenantContainer = findTenantResult.GetPropertyValue(TenantProperty.EXOTenantContainerCN).GetStringValue();
			return true;
		}

		public IAsyncResult BeginGetFfoTenantAttributionPropertiesByDomain(SmtpDomain domain, object clientAsyncState, AsyncCallback clientCallback)
		{
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(domain, GlsDirectorySession.customerAttributionDomainProperties, GlsDirectorySession.customerAttributionTenantProperties, this.glsReadFlag);
			return this.BeginExecuteWithRetry(delegate(LocatorService proxy, RequestIdentity requestIdentity, GlsAsyncResult asyncResult)
			{
				IAsyncResult internalAsyncResult = proxy.BeginFindDomain(requestIdentity, request, new AsyncCallback(this.OnAsyncRequestCompleted), asyncResult);
				asyncResult.InternalAsyncResult = internalAsyncResult;
			}, clientCallback, clientAsyncState, "BeginGetFfoTenantAttributionPropertiesByDomain", "FindDomain", domain, true);
		}

		public bool TryEndGetFfoTenantAttributionPropertiesByDomain(IAsyncResult asyncResult, out string ffoRegion, out string ffoVersion, out Guid externalDirectoryOrganizationId, out string exoNextHop, out CustomerType tenantType, out DomainIPv6State ipv6Enabled, out string exoResourceForest, out string exoAccountForest, out string exoTenantContainer)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)asyncResult;
			FindDomainResponse response = this.EndExecuteWithRetry<FindDomainResponse>(glsAsyncResult, "TryEndGetFfoTenantAttributionPropertiesByDomain", (LocatorService proxy) => proxy.EndFindDomain(glsAsyncResult.InternalAsyncResult));
			FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsAsyncResult.LoggerContext, Namespace.IgnoreComparison, Namespace.Ffo, false);
			if (findDomainResult == null)
			{
				ffoRegion = null;
				ffoVersion = null;
				externalDirectoryOrganizationId = Guid.Empty;
				exoNextHop = null;
				tenantType = CustomerType.None;
				ipv6Enabled = DomainIPv6State.Unknown;
				exoResourceForest = null;
				exoAccountForest = null;
				exoTenantContainer = null;
				return false;
			}
			ffoRegion = findDomainResult.GetDomainPropertyValue(DomainProperty.Region).GetStringValue();
			ffoVersion = findDomainResult.GetDomainPropertyValue(DomainProperty.ServiceVersion).GetStringValue();
			externalDirectoryOrganizationId = findDomainResult.TenantId;
			exoNextHop = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOSmtpNextHopDomain).GetStringValue();
			tenantType = (CustomerType)findDomainResult.GetTenantPropertyValue(TenantProperty.CustomerType).GetIntValue();
			ipv6Enabled = (DomainIPv6State)findDomainResult.GetDomainPropertyValue(DomainProperty.IPv6).GetIntValue();
			exoResourceForest = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOResourceForest).GetStringValue();
			exoAccountForest = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOAccountForest).GetStringValue();
			exoTenantContainer = findDomainResult.GetTenantPropertyValue(TenantProperty.EXOTenantContainerCN).GetStringValue();
			return true;
		}

		private void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, DomainKeyType domainKeyType, bool nego2Enabled, bool oauth2ClientProfileEnabled)
		{
			BitVector32 bitVector = default(BitVector32);
			bitVector[1] = nego2Enabled;
			bitVector[2] = oauth2ClientProfileEnabled;
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ExoFlags, new PropertyValue(bitVector.Data));
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair2 = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ExoDomainInUse, new PropertyValue(true));
			this.AddAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, domainKeyType, new KeyValuePair<DomainProperty, PropertyValue>[]
			{
				keyValuePair,
				keyValuePair2
			});
		}

		public void AddAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, DomainKeyType domainKeyType, KeyValuePair<DomainProperty, PropertyValue>[] properties)
		{
			SmtpDomain smtpDomain = this.ParseDomain(domainFqdn);
			this.SaveDomain("AddAcceptedDomain", externalDirectoryOrganizationId, smtpDomain, domainKeyType, properties);
		}

		public void RemoveAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn)
		{
			this.RemoveAcceptedDomain(externalDirectoryOrganizationId, domainFqdn, false);
		}

		public void RemoveAcceptedDomain(Guid externalDirectoryOrganizationId, string domainFqdn, bool skipTenantCheck)
		{
			SmtpDomain domain = this.ParseDomain(domainFqdn);
			DeleteDomainRequest request = LocatorServiceClientWriter.ConstructDeleteDomainRequest(domain, externalDirectoryOrganizationId, this.glsCallerId.DefaultNamespace, skipTenantCheck);
			this.ExecuteWriteWithRetry<DeleteDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.DeleteDomain(requestIdentity, request), "RemoveAcceptedDomain", "DeleteDomain", domainFqdn);
		}

		public void SetDomainVersion(Guid externalDirectoryOrganizationId, string domainFqdn, string newDomainVersion)
		{
			if (string.IsNullOrEmpty(domainFqdn))
			{
				throw new ArgumentNullException(domainFqdn);
			}
			if (string.IsNullOrEmpty(newDomainVersion))
			{
				throw new ArgumentNullException(newDomainVersion);
			}
			SmtpDomain smtpDomain = this.ParseDomain(domainFqdn);
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.ServiceVersion, new PropertyValue(newDomainVersion));
			KeyValuePair<DomainProperty, PropertyValue> keyValuePair2 = new KeyValuePair<DomainProperty, PropertyValue>(DomainProperty.DomainInUse, new PropertyValue(true));
			this.SaveDomain("SetDomainVersion", externalDirectoryOrganizationId, smtpDomain, DomainKeyType.UseExisting, new KeyValuePair<DomainProperty, PropertyValue>[]
			{
				keyValuePair,
				keyValuePair2
			});
		}

		internal static SmtpAddress ParseMemberName(string memberName)
		{
			if (string.IsNullOrEmpty(memberName))
			{
				throw new ArgumentNullException("memberName");
			}
			if (!SmtpAddress.IsValidSmtpAddress(memberName))
			{
				throw new ArgumentException("memberName");
			}
			return SmtpAddress.Parse(memberName);
		}

		internal static void ThrowIfInvalidNetID(string netID, string parameterName)
		{
			NetID netID2;
			if (!NetID.TryParse(netID, out netID2))
			{
				throw new ArgumentException(parameterName);
			}
		}

		internal static void ThrowIfInvalidSmtpAddress(string smtpAddress, string parameterName)
		{
			if (!SmtpAddress.IsValidSmtpAddress(smtpAddress))
			{
				throw new ArgumentException(parameterName);
			}
		}

		internal static void ThrowIfEmptyGuid(Guid argument, string parameterName)
		{
			if (argument == Guid.Empty)
			{
				throw new ArgumentException(parameterName);
			}
		}

		internal static void ThrowIfNull(object argument, string parameterName)
		{
			if (argument == null)
			{
				throw new ArgumentException(parameterName);
			}
		}

		private bool TryGetTenantInfoByOrgGuid(Guid externalDirectoryOrganizationId, out GlobalLocatorServiceTenant glsTenant, GlsCacheServiceMode glsCacheServiceMode, bool skipOverrideCheck)
		{
			if (!skipOverrideCheck && GlsDirectorySession.TryGetOverrideTenant(externalDirectoryOrganizationId, out glsTenant))
			{
				return true;
			}
			glsTenant = new GlobalLocatorServiceTenant();
			FindTenantRequest request = this.ConstructFindTenantRequest(externalDirectoryOrganizationId, GlsDirectorySession.AllExoTenantProperties);
			GlsLoggerContext glsLoggerContext;
			FindTenantResponse response = this.ExecuteWithRetry<FindTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindTenant(requestIdentity, request), "TryGetTenantInfoByOrgGuid", "FindTenant", externalDirectoryOrganizationId, true, glsCacheServiceMode, out glsLoggerContext);
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(response, glsLoggerContext, Namespace.Exo);
			if (findTenantResult == null)
			{
				return false;
			}
			glsTenant.ResourceForest = findTenantResult.GetPropertyValue(TenantProperty.EXOResourceForest).GetStringValue();
			glsTenant.AccountForest = findTenantResult.GetPropertyValue(TenantProperty.EXOAccountForest).GetStringValue();
			glsTenant.TenantContainerCN = findTenantResult.GetPropertyValue(TenantProperty.EXOTenantContainerCN).GetStringValue();
			glsTenant.ResumeCache = findTenantResult.GetPropertyValue(TenantProperty.GlobalResumeCache).GetStringValue();
			glsTenant.PrimarySite = findTenantResult.GetPropertyValue(TenantProperty.EXOPrimarySite).GetStringValue();
			glsTenant.SmtpNextHopDomain = new SmtpDomain(findTenantResult.GetPropertyValue(TenantProperty.EXOSmtpNextHopDomain).GetStringValue());
			glsTenant.TenantFlags = (GlsTenantFlags)findTenantResult.GetPropertyValue(TenantProperty.EXOTenantFlags).GetIntValue();
			glsTenant.ExternalDirectoryOrganizationId = externalDirectoryOrganizationId;
			glsTenant.IsOfflineData = this.IsDataReturnedFromOfflineService(glsLoggerContext);
			return this.ValidateMandatoryTenantProperties(glsTenant, glsLoggerContext);
		}

		private void SaveTenant(string operation, Guid externalDirectoryOrganizationId, KeyValuePair<TenantProperty, PropertyValue>[] tenantProperties)
		{
			SaveTenantRequest request = LocatorServiceClientWriter.ConstructSaveTenantRequest(externalDirectoryOrganizationId, tenantProperties);
			this.ExecuteWriteWithRetry<SaveTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.SaveTenant(requestIdentity, request), operation, "SaveTenant", externalDirectoryOrganizationId.ToString());
		}

		private void SaveMSAUser(string operation, string msaUserNetID, string msaUserMemberName, Guid externalDirectoryOrganizationId)
		{
			SaveUserRequest request = this.ConstructSaveUserRequest(msaUserNetID, msaUserMemberName, externalDirectoryOrganizationId);
			this.ExecuteWriteWithRetry<SaveUserResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.SaveUser(requestIdentity, request), operation, "SaveUser", msaUserNetID);
		}

		private void SaveDomain(string operation, Guid externalDirectoryOrganizationId, SmtpDomain smtpDomain, DomainKeyType domainKeyType, KeyValuePair<DomainProperty, PropertyValue>[] domainProperties)
		{
			SaveDomainRequest request = LocatorServiceClientWriter.ConstructSaveDomainRequest(smtpDomain, null, externalDirectoryOrganizationId, domainProperties);
			request.DomainKeyType = domainKeyType;
			this.ExecuteWriteWithRetry<SaveDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.SaveDomain(requestIdentity, request), operation, "SaveDomain", smtpDomain.ToString());
		}

		private bool TryGetExoDomainFlags(SmtpDomain smtpDomain, out BitVector32 flags, out Guid tenantId)
		{
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(smtpDomain, GlsDirectorySession.exoDomainFlagsProperty, GlsDirectorySession.noTenantProperties, this.glsReadFlag);
			GlsLoggerContext glsLoggerContext;
			FindDomainResponse response = this.ExecuteWithRetry<FindDomainResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindDomain(requestIdentity, request), "TryGetExoDomainFlags", "FindDomain", smtpDomain.Domain, true, out glsLoggerContext);
			FindDomainResult findDomainResult = this.ConstructFindDomainResult(response, glsLoggerContext, Namespace.Exo);
			if (findDomainResult == null)
			{
				tenantId = Guid.Empty;
				flags = new BitVector32(0);
				return false;
			}
			flags = new BitVector32(findDomainResult.GetDomainPropertyValue(DomainProperty.ExoFlags).GetIntValue());
			tenantId = findDomainResult.TenantId;
			return true;
		}

		private bool TryGetExoTenantFlags(Guid tenantId, out BitVector32 flags)
		{
			FindTenantRequest request = this.ConstructFindTenantRequest(tenantId, GlsDirectorySession.exoTenantFlagsProperty);
			GlsLoggerContext glsLoggerContext;
			FindTenantResponse response = this.ExecuteWithRetry<FindTenantResponse>((LocatorService proxy, RequestIdentity requestIdentity) => proxy.FindTenant(requestIdentity, request), "TryGetExoTenantFlags", "FindTenant", tenantId, true, out glsLoggerContext);
			FindTenantResult findTenantResult = this.ConstructFindTenantResult(response, glsLoggerContext, Namespace.Exo);
			if (findTenantResult == null)
			{
				flags = new BitVector32(0);
				return false;
			}
			flags = new BitVector32(findTenantResult.GetPropertyValue(TenantProperty.EXOTenantFlags).GetIntValue());
			return true;
		}

		private bool ValidateMandatoryTenantProperties(GlobalLocatorServiceTenant glsTenant, GlsLoggerContext loggerContext)
		{
			if (string.IsNullOrEmpty(glsTenant.ResourceForest) && string.IsNullOrEmpty(glsTenant.AccountForest))
			{
				GLSLogger.LogException(loggerContext, new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(glsTenant.ExternalDirectoryOrganizationId.ToString())));
				return false;
			}
			if (string.IsNullOrEmpty(glsTenant.ResourceForest) || string.IsNullOrEmpty(glsTenant.AccountForest))
			{
				GLSLogger.LogException(loggerContext, new GlsTenantNotFoundException(DirectoryStrings.InvalidTenantRecordInGls(glsTenant.ExternalDirectoryOrganizationId, glsTenant.ResourceForest, glsTenant.AccountForest)));
				return false;
			}
			return this.IsValidForestName(glsTenant.ExternalDirectoryOrganizationId.ToString(), glsTenant.ResourceForest, loggerContext) && this.IsValidForestName(glsTenant.ExternalDirectoryOrganizationId.ToString(), glsTenant.AccountForest, loggerContext);
		}

		private bool IsValidForestName(string tenant, string forest, GlsLoggerContext loggerContext)
		{
			PartitionId partitionId;
			Exception ex;
			if (!PartitionId.TryParse(forest, out partitionId, out ex))
			{
				GLSLogger.LogException(loggerContext, new GlsPermanentException(DirectoryStrings.InvalidForestFqdnInGls(forest, tenant, ex.Message)));
				return false;
			}
			return true;
		}

		private SmtpDomain ParseDomain(string domainFqdn)
		{
			if (string.IsNullOrEmpty(domainFqdn))
			{
				throw new ArgumentNullException("domainFqdn");
			}
			SmtpDomain result;
			if (!SmtpDomain.TryParse(domainFqdn, out result))
			{
				throw new ArgumentException("domainFqdn");
			}
			return result;
		}

		private void OnAsyncRequestCompleted(IAsyncResult internalAR)
		{
			GLSLogger.FaultInjectionDelayTraceForAsync();
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)internalAR.AsyncState;
			glsAsyncResult.InternalAsyncResult = internalAR;
			glsAsyncResult.IsCompleted = true;
		}

		private TResult ExecuteWriteWithRetry<TResult>(Func<LocatorService, RequestIdentity, TResult> method, string methodName, string glsApiName, object parameterValue) where TResult : ResponseBase
		{
			GlsLoggerContext context;
			TResult tresult = this.ExecuteWithRetry<TResult>(method, methodName, glsApiName, parameterValue, false, out context);
			GLSLogger.LogResponse(context, GLSLogger.StatusCode.WriteSuccess, tresult, null);
			return tresult;
		}

		private TResult ExecuteWithRetry<TResult>(Func<LocatorService, RequestIdentity, TResult> method, string methodName, string glsApiName, object parameterValue, bool isRead, out GlsLoggerContext glsLoggerContext) where TResult : ResponseBase
		{
			GlsLoggerContext glsLoggerContext2;
			TResult tresult = this.ExecuteWithRetry<TResult>(method, methodName, glsApiName, parameterValue, isRead, GlsDirectorySession.GlsCacheServiceMode, out glsLoggerContext2);
			glsLoggerContext = glsLoggerContext2;
			GLSLogger.LogResponse(glsLoggerContext2, isRead ? GLSLogger.StatusCode.Found : GLSLogger.StatusCode.WriteSuccess, tresult, null);
			return tresult;
		}

		private TResult ExecuteWithRetry<TResult>(Func<LocatorService, RequestIdentity, TResult> method, string methodName, string glsApiName, object parameterValue, bool isRead, GlsCacheServiceMode glsCacheServiceMode, out GlsLoggerContext glsLoggerContext) where TResult : ResponseBase
		{
			TResult response = default(TResult);
			GlsLoggerContext loggerContext = null;
			Exception ex = null;
			string endpointHostNameForLogging;
			DirectoryServiceProxyPool<LocatorService> directoryServiceProxyPool;
			int num;
			switch (glsCacheServiceMode)
			{
			case GlsCacheServiceMode.CacheDisabled:
				directoryServiceProxyPool = GlsDirectorySession.ServiceProxyPool;
				num = 0;
				endpointHostNameForLogging = GlsDirectorySession.endpointHostName;
				break;
			case GlsCacheServiceMode.CacheAsExceptionFallback:
				directoryServiceProxyPool = GlsDirectorySession.ServiceProxyPool;
				num = 1;
				endpointHostNameForLogging = GlsDirectorySession.endpointHostName;
				break;
			default:
				if (glsCacheServiceMode != GlsCacheServiceMode.CacheOnly)
				{
					throw new ArgumentException("Unsupported mode");
				}
				directoryServiceProxyPool = GlsDirectorySession.OfflineServiceProxyPool;
				num = 0;
				endpointHostNameForLogging = "localhost";
				break;
			}
			do
			{
				try
				{
					directoryServiceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<LocatorService> proxy)
					{
						RequestIdentity uniqueRequestIdentity = this.GetUniqueRequestIdentity();
						loggerContext = new GlsLoggerContext(glsApiName, parameterValue, endpointHostNameForLogging, isRead, uniqueRequestIdentity.RequestTrackingGuid);
						try
						{
							GLSLogger.FaultInjectionTrace();
							response = method(proxy.Client, uniqueRequestIdentity);
							string text = GlsDirectorySession.ExtractMachineNameFromDiagnostics(response.Diagnostics);
							if (!string.IsNullOrEmpty(text) && !string.Equals(proxy.Tag, text))
							{
								proxy.Tag = text;
							}
							loggerContext.ConnectionId = proxy.Client.GetHashCode().ToString();
						}
						catch (Exception ex3)
						{
							loggerContext.ConnectionId = proxy.Client.GetHashCode().ToString();
							GLSLogger.LogException(loggerContext, ex3);
							throw;
						}
					}, methodName, 3);
				}
				catch (Exception ex2)
				{
					if (num > 0)
					{
						if (glsApiName.Equals("SaveDomain", StringComparison.OrdinalIgnoreCase) || glsApiName.Equals("SaveTenant", StringComparison.OrdinalIgnoreCase))
						{
							throw;
						}
						directoryServiceProxyPool = GlsDirectorySession.OfflineServiceProxyPool;
						ex = ex2;
						endpointHostNameForLogging = "localhost";
						ExTraceGlobals.GLSTracer.TraceWarning<string>(0L, "Falling back to offline GLS after getting exception from the pool:{0}", ex2.Message);
					}
					else
					{
						if (ex != null)
						{
							ExTraceGlobals.GLSTracer.TraceError<string>(0L, "Both online and offline GLS failed, rethrowing the original exception:{0}", ex.Message);
							throw ex;
						}
						throw;
					}
				}
			}
			while (num-- > 0);
			glsLoggerContext = loggerContext;
			return response;
		}

		private IAsyncResult BeginExecuteWithRetry(Action<LocatorService, RequestIdentity, GlsAsyncResult> method, AsyncCallback clientCallback, object clientAsyncState, string methodName, string glsApiName, object parameterValue, bool isRead)
		{
			Exception ex = null;
			GlsAsyncResult glsAsyncResult = new GlsAsyncResult(clientCallback, clientAsyncState, null, null);
			GlsCacheServiceMode glsCacheServiceMode = GlsDirectorySession.GlsCacheServiceMode;
			string endpointHostNameForLogging;
			DirectoryServiceProxyPool<LocatorService> directoryServiceProxyPool;
			int num;
			switch (glsCacheServiceMode)
			{
			case GlsCacheServiceMode.CacheDisabled:
				directoryServiceProxyPool = GlsDirectorySession.ServiceProxyPool;
				num = 0;
				glsAsyncResult.IsOfflineGls = false;
				endpointHostNameForLogging = GlsDirectorySession.endpointHostName;
				break;
			case GlsCacheServiceMode.CacheAsExceptionFallback:
				directoryServiceProxyPool = GlsDirectorySession.ServiceProxyPool;
				num = 1;
				glsAsyncResult.IsOfflineGls = false;
				endpointHostNameForLogging = GlsDirectorySession.endpointHostName;
				break;
			default:
				if (glsCacheServiceMode != GlsCacheServiceMode.CacheOnly)
				{
					throw new ArgumentException("GlsCacheServiceMode");
				}
				directoryServiceProxyPool = GlsDirectorySession.OfflineServiceProxyPool;
				num = 0;
				glsAsyncResult.IsOfflineGls = true;
				endpointHostNameForLogging = "localhost";
				break;
			}
			do
			{
				try
				{
					directoryServiceProxyPool.CallServiceWithRetryAsyncBegin(delegate(IPooledServiceProxy<LocatorService> proxy)
					{
						RequestIdentity uniqueRequestIdentity = this.GetUniqueRequestIdentity();
						GlsLoggerContext glsLoggerContext = new GlsLoggerContext(glsApiName, parameterValue, endpointHostNameForLogging, isRead, uniqueRequestIdentity.RequestTrackingGuid);
						glsAsyncResult.LoggerContext = glsLoggerContext;
						glsAsyncResult.PooledProxy = proxy;
						try
						{
							GLSLogger.FaultInjectionTrace();
							method(proxy.Client, uniqueRequestIdentity, glsAsyncResult);
							glsLoggerContext.ConnectionId = proxy.Client.GetHashCode().ToString();
						}
						catch (Exception ex3)
						{
							glsLoggerContext.ConnectionId = proxy.Client.GetHashCode().ToString();
							GLSLogger.LogException(glsLoggerContext, ex3);
							glsAsyncResult.LoggerContext = null;
							glsAsyncResult.PooledProxy = null;
							throw;
						}
					}, methodName, 3);
				}
				catch (Exception ex2)
				{
					if (num > 0)
					{
						directoryServiceProxyPool = GlsDirectorySession.OfflineServiceProxyPool;
						glsAsyncResult.IsOfflineGls = true;
						ex = ex2;
						endpointHostNameForLogging = "localhost";
						ExTraceGlobals.GLSTracer.TraceWarning<string>(0L, "Falling back to offline GLS after getting exception from the pool:{0}", ex2.Message);
					}
					else if (ex != null)
					{
						ExTraceGlobals.GLSTracer.TraceError<string>(0L, "Both online and offline GLS failed, rethrowing the original exception:{0}", ex.Message);
						glsAsyncResult.AsyncException = ex;
						glsAsyncResult.IsCompleted = true;
					}
					else
					{
						glsAsyncResult.AsyncException = ex2;
						glsAsyncResult.IsCompleted = true;
					}
				}
			}
			while (num-- > 0);
			return glsAsyncResult;
		}

		private TResult EndExecuteWithRetry<TResult>(GlsAsyncResult glsAsyncResult, string methodName, Func<LocatorService, TResult> method) where TResult : ResponseBase
		{
			TResult response = default(TResult);
			if (!glsAsyncResult.IsCompleted)
			{
				glsAsyncResult.AsyncWaitHandle.WaitOne(GlsDirectorySession.AsyncWaitTimeout);
				if (!glsAsyncResult.IsCompleted)
				{
					Exception ex = new GlsTransientException(DirectoryStrings.AsyncTimeout((int)GlsDirectorySession.AsyncWaitTimeout.TotalSeconds), new TimeoutException());
					GLSLogger.LogException(glsAsyncResult.LoggerContext, ex);
					throw ex;
				}
			}
			glsAsyncResult.CheckExceptionAndEnd();
			DirectoryServiceProxyPool<LocatorService> directoryServiceProxyPool = glsAsyncResult.IsOfflineGls ? GlsDirectorySession.OfflineServiceProxyPool : GlsDirectorySession.ServiceProxyPool;
			directoryServiceProxyPool.CallServiceWithRetryAsyncEnd(glsAsyncResult.PooledProxy, delegate(IPooledServiceProxy<LocatorService> proxy)
			{
				try
				{
					GLSLogger.FaultInjectionTraceForAsync();
					response = method(glsAsyncResult.ServiceProxy);
					string text = GlsDirectorySession.ExtractMachineNameFromDiagnostics(response.Diagnostics);
					if (!string.IsNullOrEmpty(text) && !string.Equals(proxy.Tag, text))
					{
						proxy.Tag = text;
					}
				}
				catch (Exception ex2)
				{
					GLSLogger.LogException(glsAsyncResult.LoggerContext, ex2);
					throw;
				}
			}, methodName);
			return response;
		}

		private RequestIdentity GetUniqueRequestIdentity()
		{
			return new RequestIdentity
			{
				CallerId = this.glsCallerId.CallerIdString,
				TrackingGuid = this.glsCallerId.TrackingGuid,
				RequestTrackingGuid = Guid.NewGuid()
			};
		}

		private bool IsNotFound(string[] nonExistentNamespaces, Namespace namespaceToCheck)
		{
			if (nonExistentNamespaces == null || nonExistentNamespaces.Length == 0 || namespaceToCheck == Namespace.IgnoreComparison)
			{
				return false;
			}
			foreach (string text in nonExistentNamespaces)
			{
				if (text.Equals(NamespaceUtil.NamespaceToString(namespaceToCheck), StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private DeleteUserRequest ConstructDeleteUserRequest(string msaUserNetID)
		{
			GlsDirectorySession.ThrowIfInvalidNetID(msaUserNetID, "msaUserNetID");
			return new DeleteUserRequest
			{
				User = new UserQuery
				{
					UserKey = msaUserNetID
				}
			};
		}

		private FindTenantResult ConstructFindTenantResult(FindTenantResponse response, GlsLoggerContext glsLoggerContext, Namespace namespaceToCheck)
		{
			FindTenantResult result;
			try
			{
				GlsRawResponse glsRawResponse = new GlsRawResponse();
				glsRawResponse.Populate(response.TenantInfo);
				if (response.TenantInfo == null || this.IsNotFound(response.TenantInfo.NoneExistNamespaces, namespaceToCheck))
				{
					ADProviderPerf.IncrementNotFoundCounter();
					GLSLogger.LogResponse(glsLoggerContext, GLSLogger.StatusCode.NotFound, response, glsRawResponse);
					result = null;
				}
				else
				{
					IDictionary<TenantProperty, PropertyValue> properties = (response.TenantInfo != null && response.TenantInfo.Properties != null) ? LocatorServiceClientReader.ConstructTenantPropertyDictionary(response.TenantInfo.Properties) : new Dictionary<TenantProperty, PropertyValue>();
					GLSLogger.LogResponse(glsLoggerContext, GLSLogger.StatusCode.Found, response, glsRawResponse);
					result = new FindTenantResult(properties);
				}
			}
			catch (ArgumentException ex)
			{
				throw new GlsPermanentException(new LocalizedString(ex.Message), ex);
			}
			return result;
		}

		private FindUserResult ConstructFindUserResult(FindUserResponse response, GlsLoggerContext glsLoggerContext)
		{
			FindUserResult result;
			try
			{
				GlsRawResponse glsRawResponse = new GlsRawResponse();
				glsRawResponse.Populate(response.UserInfo);
				glsRawResponse.Populate(response.TenantInfo);
				if (response.UserInfo == null)
				{
					ADProviderPerf.IncrementNotFoundCounter();
					GLSLogger.LogResponse(glsLoggerContext, GLSLogger.StatusCode.NotFound, response, glsRawResponse);
					result = null;
				}
				else
				{
					string msaUserMemberName = null;
					if (response.UserInfo != null)
					{
						string userKey = response.UserInfo.UserKey;
						msaUserMemberName = response.UserInfo.MSAUserName;
					}
					Guid tenantId = (response.TenantInfo != null) ? response.TenantInfo.TenantId : Guid.Empty;
					IDictionary<TenantProperty, PropertyValue> tenantProperties = (response.TenantInfo != null && response.TenantInfo.Properties != null) ? LocatorServiceClientReader.ConstructTenantPropertyDictionary(response.TenantInfo.Properties) : new Dictionary<TenantProperty, PropertyValue>();
					GLSLogger.LogResponse(glsLoggerContext, GLSLogger.StatusCode.Found, response, glsRawResponse);
					result = new FindUserResult(msaUserMemberName, tenantId, tenantProperties);
				}
			}
			catch (ArgumentException ex)
			{
				throw new GlsPermanentException(new LocalizedString(ex.Message), ex);
			}
			return result;
		}

		private FindDomainResult ConstructFindDomainResult(FindDomainResponse response, GlsLoggerContext glsLoggerContext, Namespace namespaceToCheck)
		{
			return this.ConstructFindDomainResult(response, glsLoggerContext, namespaceToCheck, namespaceToCheck, false);
		}

		private FindDomainResult ConstructFindDomainResult(FindDomainResponse response, GlsLoggerContext glsLoggerContext, Namespace domainNamespaceToCheck, Namespace tenantNamespaceToCheck, bool skipTenantCheck)
		{
			FindDomainResult result;
			try
			{
				GlsRawResponse glsRawResponse = new GlsRawResponse();
				glsRawResponse.Populate(response.TenantInfo);
				glsRawResponse.Populate(response.DomainInfo);
				if ((!skipTenantCheck && (response.TenantInfo == null || this.IsNotFound(response.TenantInfo.NoneExistNamespaces, tenantNamespaceToCheck))) || response.DomainInfo == null || this.IsNotFound(response.DomainInfo.NoneExistNamespaces, domainNamespaceToCheck))
				{
					ADProviderPerf.IncrementNotFoundCounter();
					GLSLogger.LogResponse(glsLoggerContext, GLSLogger.StatusCode.NotFound, response, glsRawResponse);
					result = null;
				}
				else
				{
					IDictionary<DomainProperty, PropertyValue> domainProperties = (response.DomainInfo != null && response.DomainInfo.Properties != null) ? LocatorServiceClientReader.ConstructDomainPropertyDictionary(response.DomainInfo.Properties) : new Dictionary<DomainProperty, PropertyValue>();
					IDictionary<TenantProperty, PropertyValue> tenantProperties = (response.TenantInfo != null && response.TenantInfo.Properties != null) ? LocatorServiceClientReader.ConstructTenantPropertyDictionary(response.TenantInfo.Properties) : new Dictionary<TenantProperty, PropertyValue>();
					Guid tenantId = (response.TenantInfo != null) ? response.TenantInfo.TenantId : Guid.Empty;
					GLSLogger.LogResponse(glsLoggerContext, GLSLogger.StatusCode.Found, response, glsRawResponse);
					result = new FindDomainResult((response.DomainInfo == null) ? null : response.DomainInfo.DomainName, tenantId, tenantProperties, domainProperties);
				}
			}
			catch (ArgumentException ex)
			{
				throw new GlsPermanentException(new LocalizedString(ex.Message), ex);
			}
			return result;
		}

		private FindTenantRequest ConstructFindTenantRequest(Guid tenantId, TenantProperty[] tenantProperties)
		{
			return new FindTenantRequest
			{
				ReadFlag = (int)this.glsReadFlag,
				Tenant = new TenantQuery
				{
					TenantId = tenantId,
					PropertyNames = GlsDirectorySession.GetPropertyNames(tenantProperties)
				}
			};
		}

		private FindUserRequest ConstructFindUserRequest(string userNetID, TenantProperty[] tenantProperties)
		{
			GlsDirectorySession.ThrowIfInvalidNetID(userNetID, "userNetID");
			GlsDirectorySession.ThrowIfNull(tenantProperties, "tenantProperties");
			return new FindUserRequest
			{
				ReadFlag = (int)this.glsReadFlag,
				User = new UserQuery
				{
					UserKey = userNetID
				},
				Tenant = new TenantQuery
				{
					PropertyNames = GlsDirectorySession.GetPropertyNames(tenantProperties)
				}
			};
		}

		private FindDomainRequest ConstructFindDomainRequest(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties)
		{
			return new FindDomainRequest
			{
				ReadFlag = (int)this.glsReadFlag,
				Domain = new DomainQuery
				{
					DomainName = domain.Domain,
					PropertyNames = GlsDirectorySession.GetPropertyNames(domainProperties)
				},
				Tenant = new TenantQuery
				{
					PropertyNames = GlsDirectorySession.GetPropertyNames(tenantProperties)
				}
			};
		}

		private FindDomainsRequest ConstructFindDomainsRequest(IEnumerable<SmtpDomain> domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties)
		{
			FindDomainsRequest findDomainsRequest = new FindDomainsRequest();
			findDomainsRequest.ReadFlag = (int)this.glsReadFlag;
			findDomainsRequest.DomainsName = (from domain in domains
			select domain.Domain).ToArray<string>();
			findDomainsRequest.DomainPropertyNames = GlsDirectorySession.GetPropertyNames(domainProperties);
			findDomainsRequest.TenantPropertyNames = GlsDirectorySession.GetPropertyNames(tenantProperties);
			return findDomainsRequest;
		}

		private SaveUserRequest ConstructSaveUserRequest(string msaUserNetID, string msaUserMemberName, Guid tenantId)
		{
			GlsDirectorySession.ThrowIfInvalidNetID(msaUserNetID, "msaUserNetID");
			GlsDirectorySession.ThrowIfInvalidSmtpAddress(msaUserMemberName, "msaUserMemberName");
			GlsDirectorySession.ThrowIfEmptyGuid(tenantId, "tenantId");
			return new SaveUserRequest
			{
				UserInfo = new UserInfo
				{
					UserKey = msaUserNetID,
					MSAUserName = msaUserMemberName
				},
				TenantId = tenantId
			};
		}

		private static string[] GetPropertyNames(GlsProperty[] properties)
		{
			return (from property in properties
			select property.Name).ToArray<string>();
		}

		private static string ExtractMachineNameFromDiagnostics(string diagnostics)
		{
			if (string.IsNullOrEmpty(diagnostics))
			{
				return string.Empty;
			}
			int num = diagnostics.IndexOf("<Machine>", StringComparison.OrdinalIgnoreCase);
			if (num < 0)
			{
				return string.Empty;
			}
			num += "<Machine>".Length;
			int num2 = diagnostics.IndexOf("</Machine>", num, StringComparison.OrdinalIgnoreCase) - 1;
			if (num2 < 0 || num2 <= num)
			{
				return string.Empty;
			}
			return diagnostics.Substring(num, num2 - num + 1);
		}

		private bool IsDataReturnedFromOfflineService(GlsLoggerContext loggerContext)
		{
			return string.Equals(loggerContext.ResolveEndpointToIpAddress(false), "localhost", StringComparison.OrdinalIgnoreCase);
		}

		private const int MaxRetries = 3;

		private const string GlsEndpoint = "GlobalLocatorService";

		private const string GlsCacheEndpoint = "GlsCacheService";

		private const string offlineGlsUrl = "net.pipe://localhost/GlsCacheService/service.svc";

		private const string MachineTagStart = "<Machine>";

		private const string MachineTagEnd = "</Machine>";

		private const int MaxNumberOfClientProxies = 150;

		private static readonly TimeSpan AsyncWaitTimeout = TimeSpan.FromSeconds(60.0);

		private static readonly TimeSpan ADConfigurationSettingsRefreshPeriod = TimeSpan.FromMinutes(15.0);

		private static readonly List<TimeSpan> SendTimeouts = new List<TimeSpan>
		{
			TimeSpan.FromSeconds(15.0),
			TimeSpan.FromSeconds(7.0),
			TimeSpan.FromSeconds(3.0)
		};

		private static readonly TenantProperty[] accountResourceForestProperty = new TenantProperty[]
		{
			TenantProperty.EXOAccountForest,
			TenantProperty.EXOResourceForest,
			TenantProperty.EXOSmtpNextHopDomain,
			TenantProperty.EXOTenantContainerCN
		};

		private static readonly TenantProperty[] resourceForestProperty = new TenantProperty[]
		{
			TenantProperty.EXOResourceForest
		};

		private static readonly TenantProperty[] customerTypeProperty = new TenantProperty[]
		{
			TenantProperty.CustomerType
		};

		private static readonly TenantProperty[] customerAttributionTenantProperties = new TenantProperty[]
		{
			TenantProperty.CustomerType,
			TenantProperty.Region,
			TenantProperty.EXOSmtpNextHopDomain,
			TenantProperty.EXOAccountForest,
			TenantProperty.EXOTenantContainerCN,
			TenantProperty.EXOResourceForest
		};

		private static readonly DomainProperty[] customerAttributionDomainProperties = new DomainProperty[]
		{
			DomainProperty.Region,
			DomainProperty.ServiceVersion,
			DomainProperty.IPv6
		};

		private static readonly DomainProperty[] exoDomainFlagsProperty = new DomainProperty[]
		{
			DomainProperty.ExoFlags
		};

		private static readonly DomainProperty[] exoDomainInUseProperty = new DomainProperty[]
		{
			DomainProperty.ExoDomainInUse
		};

		private static readonly DomainProperty[] ffoExoDomainProperties = new DomainProperty[]
		{
			DomainProperty.Region,
			DomainProperty.ExoFlags
		};

		private static readonly TenantProperty[] ffoExoTenantProperties = new TenantProperty[]
		{
			TenantProperty.EXOResourceForest,
			TenantProperty.CustomerType
		};

		private static readonly TenantProperty[] exoTenantFlagsProperty = new TenantProperty[]
		{
			TenantProperty.EXOTenantFlags
		};

		private static readonly TenantProperty[] exoNextHopProperty = new TenantProperty[]
		{
			TenantProperty.EXOSmtpNextHopDomain
		};

		private static readonly DomainProperty[] ffoDomainProperties = new DomainProperty[]
		{
			DomainProperty.Region,
			DomainProperty.ServiceVersion,
			DomainProperty.IPv6
		};

		private static readonly TenantProperty[] ffoTenantProperties = new TenantProperty[]
		{
			TenantProperty.Version,
			TenantProperty.CustomerType,
			TenantProperty.Region
		};

		private static readonly TenantProperty[] ffoTenantRegionProperty = new TenantProperty[]
		{
			TenantProperty.Region
		};

		private static readonly DomainProperty[] noDomainProperties = GlsDirectorySession.exoDomainFlagsProperty;

		private static readonly TenantProperty[] noTenantProperties = GlsDirectorySession.resourceForestProperty;

		private static DirectoryServiceProxyPool<LocatorService> serviceProxyPool;

		private static DirectoryServiceProxyPool<LocatorService> offlineServiceProxyPool;

		private static GlsOverrideCollection glsTenantOverrides = new GlsOverrideCollection(null);

		private static DateTime glsTenantOverridesNextRefresh = DateTime.MinValue;

		private static object proxyPoolLockObj = new object();

		private readonly GlsCallerId glsCallerId;

		private static string endpointHostName;

		private readonly GlsAPIReadFlag glsReadFlag;

		internal static readonly DomainProperty[] AllExoDomainProperties = new DomainProperty[]
		{
			DomainProperty.ExoDomainInUse,
			DomainProperty.ExoFlags
		};

		internal static readonly TenantProperty[] AllExoTenantProperties = new TenantProperty[]
		{
			TenantProperty.EXOAccountForest,
			TenantProperty.EXOResourceForest,
			TenantProperty.EXOPrimarySite,
			TenantProperty.EXOSmtpNextHopDomain,
			TenantProperty.EXOTenantFlags,
			TenantProperty.EXOTenantContainerCN,
			TenantProperty.GlobalResumeCache
		};
	}
}
