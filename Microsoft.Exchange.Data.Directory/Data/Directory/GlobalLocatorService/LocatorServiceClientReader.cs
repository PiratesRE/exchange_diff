using System;
using System.Collections.Generic;
using System.Linq;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class LocatorServiceClientReader : LocatorServiceClientAdapter, IGlobalLocatorServiceReader
	{
		internal static IGlobalLocatorServiceReader Create(GlsCallerId glsCallerId)
		{
			if (AppConfigGlsReader.AppConfigOverrideExists())
			{
				return new AppConfigGlsReader();
			}
			return new LocatorServiceClientReader(glsCallerId);
		}

		internal static IGlobalLocatorServiceReader Create(GlsCallerId glsCallerId, LocatorService serviceProxy)
		{
			if (AppConfigGlsReader.AppConfigOverrideExists())
			{
				return new AppConfigGlsReader();
			}
			return new LocatorServiceClientReader(glsCallerId, serviceProxy);
		}

		private LocatorServiceClientReader(GlsCallerId glsCallerId, GlsAPIReadFlag readFlag) : base(glsCallerId)
		{
			this.glsReadFlag = readFlag;
		}

		private LocatorServiceClientReader(GlsCallerId glsCallerId) : base(glsCallerId)
		{
		}

		private LocatorServiceClientReader(GlsCallerId glsCallerId, LocatorService serviceProxy) : base(glsCallerId, serviceProxy)
		{
		}

		public bool TenantExists(Guid tenantId, Namespace[] ns)
		{
			FindTenantRequest request = LocatorServiceClientReader.ConstructTenantExistsRequest(tenantId, ns, this.glsReadFlag);
			LocatorService proxy = this.AcquireServiceProxy();
			FindTenantResponse findTenantResponse = GLSLogger.LoggingWrapper<FindTenantResponse>(this, tenantId.ToString(), proxy.GetHashCode().ToString(), () => proxy.FindTenant(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
			return findTenantResponse.TenantInfo != null;
		}

		public bool DomainExists(SmtpDomain domain, Namespace[] ns)
		{
			FindDomainRequest request = LocatorServiceClientReader.ConstructDomainExistsRequest(domain, ns, this.glsReadFlag);
			LocatorService proxy = this.AcquireServiceProxy();
			FindDomainResponse findDomainResponse = GLSLogger.LoggingWrapper<FindDomainResponse>(this, domain.ToString(), proxy.GetHashCode().ToString(), () => proxy.FindDomain(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
			return findDomainResponse.DomainInfo != null;
		}

		public FindTenantResult FindTenant(Guid tenantId, TenantProperty[] tenantProperties)
		{
			FindTenantRequest request = LocatorServiceClientReader.ConstructFindTenantRequest(tenantId, tenantProperties, this.glsReadFlag);
			LocatorService proxy = this.AcquireServiceProxy();
			FindTenantResponse response = GLSLogger.LoggingWrapper<FindTenantResponse>(this, tenantId.ToString(), proxy.GetHashCode().ToString(), () => proxy.FindTenant(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
			return LocatorServiceClientReader.ConstructFindTenantResult(response);
		}

		public FindDomainResult FindDomain(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties)
		{
			FindDomainRequest request = LocatorServiceClientReader.ConstructFindDomainRequest(domain, domainProperties, tenantProperties, this.glsReadFlag);
			LocatorService proxy = this.AcquireServiceProxy();
			FindDomainResponse response = GLSLogger.LoggingWrapper<FindDomainResponse>(this, domain.ToString(), proxy.GetHashCode().ToString(), () => proxy.FindDomain(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
			return LocatorServiceClientReader.ConstructFindDomainResult(response);
		}

		public FindDomainsResult FindDomains(SmtpDomain[] domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties)
		{
			FindDomainsRequest request = LocatorServiceClientReader.ConstructFindDomainsRequest(domains, domainProperties, tenantProperties, this.glsReadFlag);
			LocatorService proxy = this.AcquireServiceProxy();
			FindDomainsResponse response = GLSLogger.LoggingWrapper<FindDomainsResponse>(this, domains[0].ToString(), proxy.GetHashCode().ToString(), () => proxy.FindDomains(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
			return LocatorServiceClientReader.ConstructFindDomainsResult(response);
		}

		public IAsyncResult BeginTenantExists(Guid tenantId, Namespace[] ns, AsyncCallback callback, object asyncState)
		{
			FindTenantRequest findTenantRequest = LocatorServiceClientReader.ConstructTenantExistsRequest(tenantId, ns, this.glsReadFlag);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginFindTenant(this.requestIdentity, findTenantRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public IAsyncResult BeginDomainExists(SmtpDomain domain, Namespace[] ns, AsyncCallback callback, object asyncState)
		{
			FindDomainRequest findDomainRequest = LocatorServiceClientReader.ConstructDomainExistsRequest(domain, ns, this.glsReadFlag);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginFindDomain(this.requestIdentity, findDomainRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public IAsyncResult BeginFindTenant(Guid tenantId, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState)
		{
			FindTenantRequest findTenantRequest = LocatorServiceClientReader.ConstructFindTenantRequest(tenantId, tenantProperties, this.glsReadFlag);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginFindTenant(this.requestIdentity, findTenantRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public IAsyncResult BeginFindDomain(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState)
		{
			FindDomainRequest findDomainRequest = LocatorServiceClientReader.ConstructFindDomainRequest(domain, domainProperties, tenantProperties, this.glsReadFlag);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginFindDomain(this.requestIdentity, findDomainRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public IAsyncResult BeginFindDomains(SmtpDomain[] domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, AsyncCallback callback, object asyncState)
		{
			FindDomainsRequest findDomainsRequest = LocatorServiceClientReader.ConstructFindDomainsRequest(domains, domainProperties, tenantProperties, this.glsReadFlag);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginFindDomains(this.requestIdentity, findDomainsRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public bool EndTenantExists(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			FindTenantResponse findTenantResponse = glsAsyncResult.ServiceProxy.EndFindTenant(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
			return findTenantResponse.TenantInfo != null;
		}

		public bool EndDomainExists(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			FindDomainResponse findDomainResponse = glsAsyncResult.ServiceProxy.EndFindDomain(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
			return findDomainResponse.DomainInfo != null;
		}

		public FindTenantResult EndFindTenant(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			FindTenantResponse response = glsAsyncResult.ServiceProxy.EndFindTenant(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
			return LocatorServiceClientReader.ConstructFindTenantResult(response);
		}

		public FindDomainResult EndFindDomain(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			FindDomainResponse response = glsAsyncResult.ServiceProxy.EndFindDomain(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
			return LocatorServiceClientReader.ConstructFindDomainResult(response);
		}

		public FindDomainsResult EndFindDomains(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			FindDomainsResponse response = glsAsyncResult.ServiceProxy.EndFindDomains(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
			return LocatorServiceClientReader.ConstructFindDomainsResult(response);
		}

		internal static FindTenantRequest ConstructTenantExistsRequest(Guid tenantId, Namespace[] ns, GlsAPIReadFlag readFlag)
		{
			LocatorServiceClientAdapter.ThrowIfEmptyGuid(tenantId, "tenantId");
			LocatorServiceClientAdapter.ThrowIfInvalidNamespace(ns);
			FindTenantRequest findTenantRequest = new FindTenantRequest();
			findTenantRequest.ReadFlag = (int)readFlag;
			string[] propertyNames = (from item in ns
			select NamespaceUtil.NamespaceWildcard(item)).ToArray<string>();
			findTenantRequest.Tenant = new TenantQuery
			{
				TenantId = tenantId,
				PropertyNames = propertyNames
			};
			return findTenantRequest;
		}

		internal static FindDomainRequest ConstructDomainExistsRequest(SmtpDomain domain, Namespace[] ns, GlsAPIReadFlag readFlag)
		{
			LocatorServiceClientAdapter.ThrowIfNull(domain, "domain");
			LocatorServiceClientAdapter.ThrowIfInvalidNamespace(ns);
			FindDomainRequest findDomainRequest = new FindDomainRequest();
			findDomainRequest.ReadFlag = (int)readFlag;
			string[] propertyNames = (from item in ns
			select NamespaceUtil.NamespaceWildcard(item)).ToArray<string>();
			findDomainRequest.Domain = new DomainQuery
			{
				DomainName = domain.Domain,
				PropertyNames = propertyNames
			};
			return findDomainRequest;
		}

		internal static FindTenantRequest ConstructFindTenantRequest(Guid tenantId, TenantProperty[] tenantProperties, GlsAPIReadFlag readFlag)
		{
			LocatorServiceClientAdapter.ThrowIfEmptyGuid(tenantId, "tenantId");
			LocatorServiceClientAdapter.ThrowIfNull(tenantProperties, "tenantProperties");
			return new FindTenantRequest
			{
				ReadFlag = (int)readFlag,
				Tenant = new TenantQuery
				{
					TenantId = tenantId,
					PropertyNames = LocatorServiceClientReader.GetPropertyNames(tenantProperties)
				}
			};
		}

		internal static FindDomainRequest ConstructFindDomainRequest(SmtpDomain domain, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, GlsAPIReadFlag readFlag)
		{
			LocatorServiceClientAdapter.ThrowIfNull(domain, "domain");
			LocatorServiceClientAdapter.ThrowIfNull(domainProperties, "domainProperties");
			LocatorServiceClientAdapter.ThrowIfNull(tenantProperties, "tenantProperties");
			return new FindDomainRequest
			{
				ReadFlag = (int)readFlag,
				Domain = new DomainQuery
				{
					DomainName = domain.Domain,
					PropertyNames = LocatorServiceClientReader.GetPropertyNames(domainProperties)
				},
				Tenant = new TenantQuery
				{
					PropertyNames = LocatorServiceClientReader.GetPropertyNames(tenantProperties)
				}
			};
		}

		internal static FindDomainsRequest ConstructFindDomainsRequest(IEnumerable<SmtpDomain> domains, DomainProperty[] domainProperties, TenantProperty[] tenantProperties, GlsAPIReadFlag readFlag)
		{
			LocatorServiceClientAdapter.ThrowIfNull(domains, "domains");
			LocatorServiceClientAdapter.ThrowIfNull(domainProperties, "domainProperties");
			LocatorServiceClientAdapter.ThrowIfNull(tenantProperties, "tenantProperties");
			FindDomainsRequest findDomainsRequest = new FindDomainsRequest();
			findDomainsRequest.ReadFlag = (int)readFlag;
			findDomainsRequest.DomainsName = (from domain in domains
			select domain.Domain).ToArray<string>();
			findDomainsRequest.DomainPropertyNames = LocatorServiceClientReader.GetPropertyNames(domainProperties);
			findDomainsRequest.TenantPropertyNames = LocatorServiceClientReader.GetPropertyNames(tenantProperties);
			return findDomainsRequest;
		}

		internal static FindTenantResult ConstructFindTenantResult(FindTenantResponse response)
		{
			IDictionary<TenantProperty, PropertyValue> properties = (response.TenantInfo != null) ? LocatorServiceClientReader.ConstructTenantPropertyDictionary(response.TenantInfo.Properties) : new Dictionary<TenantProperty, PropertyValue>();
			return new FindTenantResult(properties);
		}

		internal static FindDomainResult ConstructFindDomainResult(FindDomainResponse response)
		{
			IDictionary<DomainProperty, PropertyValue> domainProperties = (response.DomainInfo != null && response.DomainInfo.Properties != null) ? LocatorServiceClientReader.ConstructDomainPropertyDictionary(response.DomainInfo.Properties) : new Dictionary<DomainProperty, PropertyValue>();
			IDictionary<TenantProperty, PropertyValue> tenantProperties = (response.TenantInfo != null && response.TenantInfo.Properties != null) ? LocatorServiceClientReader.ConstructTenantPropertyDictionary(response.TenantInfo.Properties) : new Dictionary<TenantProperty, PropertyValue>();
			Guid tenantId = (response.TenantInfo != null) ? response.TenantInfo.TenantId : Guid.Empty;
			return new FindDomainResult((response.DomainInfo == null) ? null : response.DomainInfo.DomainName, tenantId, tenantProperties, domainProperties);
		}

		internal static FindDomainsResult ConstructFindDomainsResult(FindDomainsResponse response)
		{
			FindDomainResult[] findDomainResults = new FindDomainResult[0];
			if (response.DomainsResponse != null)
			{
				findDomainResults = (from findDomainResponse in response.DomainsResponse
				select LocatorServiceClientReader.ConstructFindDomainResult(findDomainResponse)).ToArray<FindDomainResult>();
			}
			return new FindDomainsResult(findDomainResults);
		}

		private static string[] GetPropertyNames(GlsProperty[] properties)
		{
			return (from property in properties
			select property.Name).ToArray<string>();
		}

		internal static IDictionary<TenantProperty, PropertyValue> ConstructTenantPropertyDictionary(KeyValuePair<string, string>[] properties)
		{
			IDictionary<TenantProperty, PropertyValue> dictionary = new Dictionary<TenantProperty, PropertyValue>();
			foreach (KeyValuePair<string, string> keyValuePair in properties)
			{
				TenantProperty tenantProperty = TenantProperty.Get(keyValuePair.Key);
				PropertyValue value = PropertyValue.Create(keyValuePair.Value, tenantProperty);
				dictionary.Add(tenantProperty, value);
			}
			return dictionary;
		}

		internal static IDictionary<DomainProperty, PropertyValue> ConstructDomainPropertyDictionary(KeyValuePair<string, string>[] properties)
		{
			IDictionary<DomainProperty, PropertyValue> dictionary = new Dictionary<DomainProperty, PropertyValue>();
			foreach (KeyValuePair<string, string> keyValuePair in properties)
			{
				DomainProperty domainProperty = DomainProperty.Get(keyValuePair.Key);
				PropertyValue value = PropertyValue.Create(keyValuePair.Value, domainProperty);
				dictionary.Add(domainProperty, value);
			}
			return dictionary;
		}

		private readonly GlsAPIReadFlag glsReadFlag;
	}
}
