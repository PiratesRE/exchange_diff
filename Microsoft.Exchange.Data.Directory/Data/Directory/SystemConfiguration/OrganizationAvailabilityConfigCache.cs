using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationAvailabilityConfigCache : OrganizationBaseCache
	{
		public OrganizationAvailabilityConfigCache(OrganizationId organizationId, IConfigurationSession session, IRecipientSession recipientSession) : base(organizationId, session)
		{
			this.recipientSession = recipientSession;
		}

		public AvailabilityConfig AvailabilityConfig
		{
			get
			{
				this.PopulateCacheIfNeeded();
				return this.config;
			}
		}

		public ADRecipient GetPerUserAccountObject()
		{
			this.PopulateAccessInformationIfNeeded();
			return this.perUserAccountObject;
		}

		public ADRecipient GetOrgWideAccountObject()
		{
			this.PopulateAccessInformationIfNeeded();
			return this.orgWideAccountObject;
		}

		private void PopulateAccessInformationIfNeeded()
		{
			if (!this.accountInformationCached)
			{
				if (this.AvailabilityConfig != null)
				{
					if (this.config.PerUserAccount != null)
					{
						this.perUserAccountObject = this.recipientSession.Read(this.config.PerUserAccount);
					}
					else
					{
						this.perUserAccountObject = null;
						OrganizationBaseCache.Tracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Null PerUserAccount for get availability configuration for: {0}", base.OrganizationId);
					}
					if (this.config.OrgWideAccount != null)
					{
						this.orgWideAccountObject = this.recipientSession.Read(this.config.OrgWideAccount);
					}
					else
					{
						this.orgWideAccountObject = null;
						OrganizationBaseCache.Tracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Null OrgWideAccount for get availability configuration for: {0}", base.OrganizationId);
					}
				}
				this.accountInformationCached = true;
			}
		}

		private void PopulateCacheIfNeeded()
		{
			if (!this.cached)
			{
				OrganizationBaseCache.Tracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Cache miss, get the Availability Configuration for: {0}", base.OrganizationId);
				this.config = base.Session.GetAvailabilityConfig();
				OrganizationBaseCache.Tracer.TraceDebug<string, OrganizationId>((long)this.GetHashCode(), "{0} to get availability configuration for: {1}", (this.config == null) ? "Unable" : "Able", base.OrganizationId);
				this.cached = true;
			}
		}

		private AvailabilityConfig config;

		private ADRecipient perUserAccountObject;

		private ADRecipient orgWideAccountObject;

		private bool cached;

		private bool accountInformationCached;

		private IRecipientSession recipientSession;
	}
}
