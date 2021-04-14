using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal class AsyncCallStateObject
	{
		public AsyncCallStateObject(object callerStateObject, IPooledServiceProxy<IPolicySyncWebserviceClient> proxyToUse, TenantCookie tenantCookie = null)
		{
			ArgumentValidator.ThrowIfNull("proxyToUse", proxyToUse);
			this.CallerStateObject = callerStateObject;
			this.ProxyToUse = proxyToUse;
			this.TenantCookie = tenantCookie;
		}

		public object CallerStateObject { get; private set; }

		internal IPooledServiceProxy<IPolicySyncWebserviceClient> ProxyToUse { get; private set; }

		internal TenantCookie TenantCookie { get; private set; }
	}
}
