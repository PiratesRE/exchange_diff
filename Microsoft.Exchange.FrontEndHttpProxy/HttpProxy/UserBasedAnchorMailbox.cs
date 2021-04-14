using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class UserBasedAnchorMailbox : DatabaseBasedAnchorMailbox
	{
		protected UserBasedAnchorMailbox(AnchorSource anchorSource, object sourceObject, IRequestContext requestContext) : base(anchorSource, sourceObject, requestContext)
		{
		}

		public Func<ADRawEntry, ADObjectId> MissingDatabaseHandler { get; set; }

		public string CacheKeyPostfix { get; set; }

		protected virtual ADPropertyDefinition[] PropertySet
		{
			get
			{
				return UserBasedAnchorMailbox.ADRawEntryPropertySet;
			}
		}

		protected virtual ADPropertyDefinition DatabaseProperty
		{
			get
			{
				return ADMailboxRecipientSchema.Database;
			}
		}

		public ADRawEntry GetADRawEntry()
		{
			if (!this.activeDirectoryRawEntryLoaded)
			{
				this.loadedADRawEntry = this.LoadADRawEntry();
				if (this.loadedADRawEntry == null)
				{
					base.RequestContext.Logger.AppendString(HttpProxyMetadata.RoutingHint, "-NoUser");
				}
				ExTraceGlobals.VerboseTracer.TraceDebug<ADRawEntry, UserBasedAnchorMailbox>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::GetADRawEntry]: LoadADRawEntry() resturns {0} for anchor mailbox {1}.", this.loadedADRawEntry, this);
				this.activeDirectoryRawEntryLoaded = true;
			}
			return this.loadedADRawEntry;
		}

		public string GetDomainName()
		{
			return base.GetCacheEntry().DomainName;
		}

		public override string GetOrganizationNameForLogging()
		{
			if (this.activeDirectoryRawEntryLoaded && this.GetADRawEntry() != null)
			{
				return ((OrganizationId)this.GetADRawEntry()[ADObjectSchema.OrganizationId]).GetFriendlyName();
			}
			return base.GetOrganizationNameForLogging();
		}

		public override BackEndCookieEntryBase BuildCookieEntryForTarget(BackEndServer routingTarget, bool proxyToDownLevel, bool useResourceForest)
		{
			if (routingTarget == null)
			{
				throw new ArgumentNullException("routingTarget");
			}
			if (!proxyToDownLevel && !base.UseServerCookie)
			{
				ADObjectId database = this.GetDatabase();
				if (database != null)
				{
					if (useResourceForest)
					{
						return new BackEndDatabaseResourceForestCookieEntry(database.ObjectGuid, this.GetDomainName(), database.PartitionFQDN);
					}
					return new BackEndDatabaseCookieEntry(database.ObjectGuid, this.GetDomainName());
				}
			}
			return base.BuildCookieEntryForTarget(routingTarget, proxyToDownLevel, useResourceForest);
		}

		public override IRoutingEntry GetRoutingEntry()
		{
			IRoutingKey routingKey = this.GetRoutingKey();
			DatabaseGuidRoutingDestination databaseGuidRoutingDestination = this.GetRoutingDestination() as DatabaseGuidRoutingDestination;
			if (routingKey != null && databaseGuidRoutingDestination != null)
			{
				return new SuccessfulMailboxRoutingEntry(routingKey, databaseGuidRoutingDestination, 0L);
			}
			return base.GetRoutingEntry();
		}

		protected abstract ADRawEntry LoadADRawEntry();

		protected override AnchorMailboxCacheEntry RefreshCacheEntry()
		{
			ADRawEntry adrawEntry = this.GetADRawEntry();
			if (adrawEntry == null)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<UserBasedAnchorMailbox>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::RefreshCacheEntry]: Anchor mailbox {0} has no AD object. Will use random server.", this);
				return new AnchorMailboxCacheEntry();
			}
			string domainNameFromADRawEntry = UserBasedAnchorMailbox.GetDomainNameFromADRawEntry(adrawEntry);
			ExTraceGlobals.VerboseTracer.TraceDebug<UserBasedAnchorMailbox, string>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::RefreshCacheEntry]: The domain name of anchor mailbox {0} is {1}.", this, domainNameFromADRawEntry);
			ADObjectId adobjectId = (ADObjectId)adrawEntry[this.DatabaseProperty];
			if (adobjectId == null && this.MissingDatabaseHandler != null)
			{
				adobjectId = this.MissingDatabaseHandler(adrawEntry);
			}
			if (adobjectId == null)
			{
				base.RequestContext.Logger.AppendString(HttpProxyMetadata.RoutingHint, "-NoDatabase");
				OrganizationId organizationId = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
				ADUser defaultOrganizationMailbox = HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(organizationId, (string)adrawEntry[ADObjectSchema.DistinguishedName]);
				if (defaultOrganizationMailbox == null || defaultOrganizationMailbox.Database == null)
				{
					if ((Utilities.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled) && OrganizationId.ForestWideOrgId.Equals(organizationId))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::RefreshCacheEntry]: Cannot find organization mailbox for datacenter FirstOrg user {0}. Will use random server.", adrawEntry.Id);
						return new AnchorMailboxCacheEntry
						{
							DomainName = domainNameFromADRawEntry
						};
					}
					string text = string.Format("Unable to find organization mailbox for organization {0}", organizationId);
					ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::RefreshCacheEntry]: {0}", text);
					throw new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.OrganizationMailboxNotFound, text);
				}
				else
				{
					adobjectId = defaultOrganizationMailbox.Database;
					ExTraceGlobals.VerboseTracer.TraceDebug<ADObjectId, ObjectId, ADObjectId>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::RefreshCacheEntry]: Anchor mailbox user {0} has no mailbox. Will use organization mailbox {1} with database {2}", adrawEntry.Id, defaultOrganizationMailbox.Identity, adobjectId);
				}
			}
			return new AnchorMailboxCacheEntry
			{
				Database = adobjectId,
				DomainName = domainNameFromADRawEntry
			};
		}

		protected override AnchorMailboxCacheEntry LoadCacheEntryFromIncomingCookie()
		{
			BackEndDatabaseCookieEntry backEndDatabaseCookieEntry = base.IncomingCookieEntry as BackEndDatabaseCookieEntry;
			if (backEndDatabaseCookieEntry != null)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<UserBasedAnchorMailbox, BackEndDatabaseCookieEntry>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::LoadCacheEntryFromIncomingCookie]: Anchor mailbox {0} using cookie entry {1} as cache entry.", this, backEndDatabaseCookieEntry);
				BackEndDatabaseResourceForestCookieEntry backEndDatabaseResourceForestCookieEntry = base.IncomingCookieEntry as BackEndDatabaseResourceForestCookieEntry;
				return new AnchorMailboxCacheEntry
				{
					Database = new ADObjectId(backEndDatabaseCookieEntry.Database, (backEndDatabaseResourceForestCookieEntry == null) ? null : backEndDatabaseResourceForestCookieEntry.ResourceForest),
					DomainName = backEndDatabaseCookieEntry.Domain
				};
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<UserBasedAnchorMailbox>((long)this.GetHashCode(), "[UserBasedAnchorMailbox::LoadCacheEntryFromCookie]: Anchor mailbox {0} had no BackEndDatabaseCookie.", this);
			return null;
		}

		protected override string ToCacheKey()
		{
			if (!string.IsNullOrEmpty(this.CacheKeyPostfix))
			{
				return base.ToCacheKey() + this.CacheKeyPostfix;
			}
			return base.ToCacheKey();
		}

		protected virtual IRoutingKey GetRoutingKey()
		{
			return null;
		}

		private static string GetDomainNameFromADRawEntry(ADRawEntry activeDirectoryRawEntry)
		{
			OrganizationId organizationId = (OrganizationId)activeDirectoryRawEntry[ADObjectSchema.OrganizationId];
			if (organizationId == null || organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				return null;
			}
			SmtpAddress smtpAddress = (SmtpAddress)activeDirectoryRawEntry[ADRecipientSchema.PrimarySmtpAddress];
			if (!string.IsNullOrEmpty(smtpAddress.Domain))
			{
				return smtpAddress.Domain;
			}
			SmtpAddress smtpAddress2 = (SmtpAddress)activeDirectoryRawEntry[ADRecipientSchema.WindowsLiveID];
			if (!string.IsNullOrEmpty(smtpAddress2.Domain))
			{
				return smtpAddress2.Domain;
			}
			return organizationId.ConfigurationUnit.Parent.Name;
		}

		private IRoutingDestination GetRoutingDestination()
		{
			string domainName = this.GetDomainName();
			if (!string.IsNullOrEmpty(domainName))
			{
				ADObjectId database = this.GetDatabase();
				return new DatabaseGuidRoutingDestination(database.ObjectGuid, domainName, database.PartitionFQDN);
			}
			return null;
		}

		public static readonly ADPropertyDefinition[] ADRawEntryPropertySet = new ADPropertyDefinition[]
		{
			ADObjectSchema.ExchangeVersion,
			ADObjectSchema.OrganizationId,
			ADMailboxRecipientSchema.Database,
			ADMailboxRecipientSchema.Sid,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.ExternalEmailAddress
		};

		private ADRawEntry loadedADRawEntry;

		private bool activeDirectoryRawEntryLoaded;
	}
}
