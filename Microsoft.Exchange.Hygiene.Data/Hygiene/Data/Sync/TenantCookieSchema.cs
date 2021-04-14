using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class TenantCookieSchema : BaseCookieSchema
	{
		public static readonly HygienePropertyDefinition TenantIdProp = new HygienePropertyDefinition("TenantId", typeof(ADObjectId));
	}
}
