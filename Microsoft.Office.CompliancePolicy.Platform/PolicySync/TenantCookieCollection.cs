using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class TenantCookieCollection : IEnumerable<TenantCookie>, IEnumerable
	{
		public TenantCookieCollection(Workload workload, ConfigurationObjectType objectType)
		{
			this.Workload = workload;
			this.ObjectType = objectType;
		}

		[DataMember]
		public Workload Workload { get; private set; }

		[DataMember]
		public ConfigurationObjectType ObjectType { get; private set; }

		public TenantCookie this[Guid tenantId]
		{
			get
			{
				return this.tenantCookies[tenantId];
			}
			set
			{
				this.tenantCookies[tenantId] = value;
			}
		}

		public bool TryGetCookie(Guid tenantId, out TenantCookie tenantCookie)
		{
			return this.tenantCookies.TryGetValue(tenantId, out tenantCookie);
		}

		IEnumerator<TenantCookie> IEnumerable<TenantCookie>.GetEnumerator()
		{
			return this.tenantCookies.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TenantCookie>)this).GetEnumerator();
		}

		[DataMember]
		private Dictionary<Guid, TenantCookie> tenantCookies = new Dictionary<Guid, TenantCookie>();
	}
}
