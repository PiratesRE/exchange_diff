using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class OrganizationBaseCache
	{
		public OrganizationBaseCache(OrganizationId organizationId, IConfigurationSession session)
		{
			this.organizationId = organizationId;
			this.session = session;
		}

		protected IConfigurationSession Session
		{
			get
			{
				return this.session;
			}
		}

		protected OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		private OrganizationId organizationId;

		private IConfigurationSession session;

		protected static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;
	}
}
