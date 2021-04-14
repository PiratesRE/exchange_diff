using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	public class TenantCookie : BaseCookie
	{
		public TenantCookie()
		{
		}

		public TenantCookie(ADObjectId tenantId, string serviceInstance, byte[] data, bool complete = false) : base(serviceInstance, data)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("tenantId");
			}
			this.TenantId = tenantId;
			base.Complete = complete;
		}

		public TenantCookie(string contextId, string serviceInstance, byte[] data, bool complete = false) : this(new ADObjectId(Guid.Parse(contextId)), serviceInstance, data, complete)
		{
		}

		public ADObjectId TenantId
		{
			get
			{
				return this[TenantCookieSchema.TenantIdProp] as ADObjectId;
			}
			set
			{
				this[TenantCookieSchema.TenantIdProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return TenantCookie.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TenantCookie.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly TenantCookieSchema schema = ObjectSchema.GetInstance<TenantCookieSchema>();

		private static string mostDerivedClass = "TenantCookie";
	}
}
