using System;
using System.Collections.Generic;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class LocatorServiceClientWriter : LocatorServiceClientAdapter, IGlobalLocatorServiceWriter
	{
		public static IGlobalLocatorServiceWriter Create(GlsCallerId glsCallerId)
		{
			return new LocatorServiceClientWriter(glsCallerId);
		}

		public static IGlobalLocatorServiceWriter Create(GlsCallerId glsCallerId, LocatorService serviceProxy)
		{
			return new LocatorServiceClientWriter(glsCallerId, serviceProxy);
		}

		private LocatorServiceClientWriter(GlsCallerId glsCallerId) : base(glsCallerId)
		{
		}

		private LocatorServiceClientWriter(GlsCallerId glsCallerId, LocatorService serviceProxy) : base(glsCallerId, serviceProxy)
		{
		}

		public void SaveTenant(Guid tenantId, KeyValuePair<TenantProperty, PropertyValue>[] properties)
		{
			SaveTenantRequest request = LocatorServiceClientWriter.ConstructSaveTenantRequest(tenantId, properties);
			LocatorService proxy = this.AcquireServiceProxy();
			GLSLogger.LoggingWrapper<SaveTenantResponse>(this, tenantId.ToString(), proxy.GetHashCode().ToString(), () => proxy.SaveTenant(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
		}

		public void SaveDomain(SmtpDomain domain, bool isInitialDomain, Guid tenantId, KeyValuePair<DomainProperty, PropertyValue>[] properties)
		{
			SaveDomainRequest request = LocatorServiceClientWriter.ConstructSaveDomainRequest(domain, null, tenantId, properties);
			request.DomainKeyType = (isInitialDomain ? DomainKeyType.InitialDomain : DomainKeyType.CustomDomain);
			LocatorService proxy = this.AcquireServiceProxy();
			GLSLogger.LoggingWrapper<SaveDomainResponse>(this, domain.ToString(), proxy.GetHashCode().ToString(), () => proxy.SaveDomain(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
		}

		public void SaveDomain(SmtpDomain domain, string domainKey, Guid tenantId, KeyValuePair<DomainProperty, PropertyValue>[] properties)
		{
			SaveDomainRequest request = LocatorServiceClientWriter.ConstructSaveDomainRequest(domain, domainKey, tenantId, properties);
			LocatorService proxy = this.AcquireServiceProxy();
			GLSLogger.LoggingWrapper<SaveDomainResponse>(this, domain.ToString(), proxy.GetHashCode().ToString(), () => proxy.SaveDomain(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
		}

		public void DeleteTenant(Guid tenantId, Namespace ns)
		{
			DeleteTenantRequest request = LocatorServiceClientWriter.ConstructDeleteTenantRequest(tenantId, ns);
			LocatorService proxy = this.AcquireServiceProxy();
			GLSLogger.LoggingWrapper<DeleteTenantResponse>(this, tenantId.ToString(), proxy.GetHashCode().ToString(), () => proxy.DeleteTenant(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
		}

		public void DeleteDomain(SmtpDomain domain, Guid tenantId, Namespace ns)
		{
			DeleteDomainRequest request = LocatorServiceClientWriter.ConstructDeleteDomainRequest(domain, tenantId, ns);
			LocatorService proxy = this.AcquireServiceProxy();
			GLSLogger.LoggingWrapper<DeleteDomainResponse>(this, domain.ToString(), proxy.GetHashCode().ToString(), () => proxy.DeleteDomain(this.requestIdentity, request));
			base.ReleaseServiceProxy(proxy);
		}

		public IAsyncResult BeginSaveTenant(Guid tenantId, KeyValuePair<TenantProperty, PropertyValue>[] properties, AsyncCallback callback, object asyncState)
		{
			SaveTenantRequest saveTenantRequest = LocatorServiceClientWriter.ConstructSaveTenantRequest(tenantId, properties);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginSaveTenant(this.requestIdentity, saveTenantRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public IAsyncResult BeginSaveDomain(SmtpDomain domain, string domainKey, Guid tenantId, KeyValuePair<DomainProperty, PropertyValue>[] properties, AsyncCallback callback, object asyncState)
		{
			SaveDomainRequest saveDomainRequest = LocatorServiceClientWriter.ConstructSaveDomainRequest(domain, domainKey, tenantId, properties);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginSaveDomain(this.requestIdentity, saveDomainRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public IAsyncResult BeginDeleteTenant(Guid tenantId, Namespace ns, AsyncCallback callback, object asyncState)
		{
			DeleteTenantRequest deleteTenantRequest = LocatorServiceClientWriter.ConstructDeleteTenantRequest(tenantId, ns);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginDeleteTenant(this.requestIdentity, deleteTenantRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public IAsyncResult BeginDeleteDomain(SmtpDomain domain, Guid tenantId, Namespace ns, AsyncCallback callback, object asyncState)
		{
			DeleteDomainRequest deleteDomainRequest = LocatorServiceClientWriter.ConstructDeleteDomainRequest(domain, tenantId, ns);
			LocatorService locatorService = this.AcquireServiceProxy();
			IAsyncResult internalAsyncResult = locatorService.BeginDeleteDomain(this.requestIdentity, deleteDomainRequest, new AsyncCallback(LocatorServiceClientAdapter.OnWebServiceRequestCompleted), new GlsAsyncState(callback, asyncState, locatorService));
			return new GlsAsyncResult(callback, asyncState, locatorService, internalAsyncResult);
		}

		public void EndSaveTenant(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			glsAsyncResult.ServiceProxy.EndSaveTenant(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
		}

		public void EndSaveDomain(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			glsAsyncResult.ServiceProxy.EndSaveDomain(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
		}

		public void EndDeleteTenant(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			glsAsyncResult.ServiceProxy.EndDeleteTenant(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
		}

		public void EndDeleteDomain(IAsyncResult externalAR)
		{
			GlsAsyncResult glsAsyncResult = (GlsAsyncResult)externalAR;
			glsAsyncResult.ServiceProxy.EndDeleteDomain(glsAsyncResult.InternalAsyncResult);
			base.ReleaseServiceProxy(glsAsyncResult.ServiceProxy);
			glsAsyncResult.Dispose();
		}

		internal static SaveTenantRequest ConstructSaveTenantRequest(Guid tenantId, KeyValuePair<TenantProperty, PropertyValue>[] properties)
		{
			LocatorServiceClientAdapter.ThrowIfEmptyGuid(tenantId, "tenantId");
			LocatorServiceClientAdapter.ThrowIfNull(properties, "properties");
			foreach (KeyValuePair<TenantProperty, PropertyValue> keyValuePair in properties)
			{
				if (keyValuePair.Key.DataType != keyValuePair.Value.DataType)
				{
					throw new ArgumentException("key and value have different data types!", "properties");
				}
			}
			return new SaveTenantRequest
			{
				TenantInfo = new TenantInfo
				{
					TenantId = tenantId,
					Properties = LocatorServiceClientWriter.GetPropertyValues(properties)
				}
			};
		}

		internal static SaveDomainRequest ConstructSaveDomainRequest(SmtpDomain domain, string domainKey, Guid tenantId, KeyValuePair<DomainProperty, PropertyValue>[] properties)
		{
			LocatorServiceClientAdapter.ThrowIfEmptyGuid(tenantId, "tenantId");
			LocatorServiceClientAdapter.ThrowIfNull(domain, "domain");
			LocatorServiceClientAdapter.ThrowIfNull(properties, "properties");
			foreach (KeyValuePair<DomainProperty, PropertyValue> keyValuePair in properties)
			{
				if (keyValuePair.Key.DataType != keyValuePair.Value.DataType)
				{
					throw new ArgumentException("key and value have different data types!", "properties");
				}
			}
			return new SaveDomainRequest
			{
				TenantId = tenantId,
				DomainInfo = new DomainInfo
				{
					DomainName = domain.Domain,
					DomainKey = domainKey,
					Properties = LocatorServiceClientWriter.GetPropertyValues(properties)
				}
			};
		}

		internal static DeleteTenantRequest ConstructDeleteTenantRequest(Guid tenantId, Namespace ns)
		{
			LocatorServiceClientAdapter.ThrowIfEmptyGuid(tenantId, "tenantId");
			LocatorServiceClientAdapter.ThrowIfInvalidNamespace(ns);
			return new DeleteTenantRequest
			{
				Tenant = new TenantQuery
				{
					TenantId = tenantId,
					PropertyNames = new string[]
					{
						NamespaceUtil.NamespaceWildcard(ns)
					}
				}
			};
		}

		internal static DeleteDomainRequest ConstructDeleteDomainRequest(SmtpDomain domain, Guid tenantId, Namespace ns)
		{
			return LocatorServiceClientWriter.ConstructDeleteDomainRequest(domain, tenantId, ns, false);
		}

		internal static DeleteDomainRequest ConstructDeleteDomainRequest(SmtpDomain domain, Guid tenantId, Namespace ns, bool skipTenantCheck)
		{
			LocatorServiceClientAdapter.ThrowIfNull(domain, "domain");
			if (!skipTenantCheck)
			{
				LocatorServiceClientAdapter.ThrowIfEmptyGuid(tenantId, "tenantId");
			}
			LocatorServiceClientAdapter.ThrowIfInvalidNamespace(ns);
			return new DeleteDomainRequest
			{
				TenantId = tenantId,
				Domain = new DomainQuery
				{
					DomainName = domain.Domain,
					PropertyNames = new string[]
					{
						NamespaceUtil.NamespaceWildcard(ns)
					}
				}
			};
		}

		private static KeyValuePair<string, string>[] GetPropertyValues(KeyValuePair<TenantProperty, PropertyValue>[] properties)
		{
			KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[properties.Length];
			for (int i = 0; i < properties.Length; i++)
			{
				GlsProperty key = properties[i].Key;
				PropertyValue value = properties[i].Value;
				array[i] = new KeyValuePair<string, string>(key.Name, value.ToString());
			}
			return array;
		}

		private static KeyValuePair<string, string>[] GetPropertyValues(KeyValuePair<DomainProperty, PropertyValue>[] properties)
		{
			KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[properties.Length];
			for (int i = 0; i < properties.Length; i++)
			{
				GlsProperty key = properties[i].Key;
				PropertyValue value = properties[i].Value;
				array[i] = new KeyValuePair<string, string>(key.Name, value.ToString());
			}
			return array;
		}
	}
}
