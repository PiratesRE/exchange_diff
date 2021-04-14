using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class TenantCookieFilter : BaseCookieFilter
	{
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

		public bool Completed
		{
			get
			{
				return (bool)this[BaseCookieSchema.CompleteProp];
			}
			set
			{
				this[BaseCookieSchema.CompleteProp] = value;
			}
		}

		public static readonly TenantCookieFilter Default = new TenantCookieFilter();
	}
}
