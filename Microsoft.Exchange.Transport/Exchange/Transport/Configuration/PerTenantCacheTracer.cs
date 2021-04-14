using System;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class PerTenantCacheTracer : DefaultCacheTracer<OrganizationId>
	{
		public PerTenantCacheTracer(Trace tracer, string cacheName) : base(tracer, cacheName)
		{
		}

		protected override string GetKeyString(OrganizationId key)
		{
			if (key == null)
			{
				return string.Empty;
			}
			if (key.ConfigurationUnit != null)
			{
				return key.ConfigurationUnit.DistinguishedName;
			}
			return "FirstOrg";
		}
	}
}
