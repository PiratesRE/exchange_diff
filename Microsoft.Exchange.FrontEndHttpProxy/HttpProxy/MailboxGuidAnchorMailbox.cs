using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy
{
	internal class MailboxGuidAnchorMailbox : ArchiveSupportedAnchorMailbox
	{
		public MailboxGuidAnchorMailbox(Guid mailboxGuid, string domain, IRequestContext requestContext) : base(AnchorSource.MailboxGuid, mailboxGuid, requestContext)
		{
			this.Domain = domain;
			base.NotFoundExceptionCreator = delegate()
			{
				base.UpdateNegativeCache(new NegativeAnchorMailboxCacheEntry
				{
					ErrorCode = HttpStatusCode.NotFound,
					SubErrorCode = HttpProxySubErrorCode.MailboxGuidWithDomainNotFound,
					SourceObject = this.ToCacheKey()
				});
				string message = string.Format("Cannot find mailbox {0} with domain {1}.", this.MailboxGuid, this.Domain);
				return new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.MailboxGuidWithDomainNotFound, message);
			};
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)base.SourceObject;
			}
		}

		public string Domain { get; private set; }

		public string FallbackSmtp { get; set; }

		public override string GetOrganizationNameForLogging()
		{
			string organizationNameForLogging = base.GetOrganizationNameForLogging();
			if (string.IsNullOrEmpty(organizationNameForLogging) && !string.IsNullOrEmpty(this.Domain))
			{
				return this.Domain;
			}
			return organizationNameForLogging;
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			IRecipientSession session = null;
			if (!string.IsNullOrEmpty(this.Domain) && SmtpAddress.IsValidDomain(this.Domain))
			{
				try
				{
					session = DirectoryHelper.GetRecipientSessionFromDomain(base.RequestContext.LatencyTracker, this.Domain, false);
					goto IL_95;
				}
				catch (CannotResolveTenantNameException)
				{
					base.UpdateNegativeCache(new NegativeAnchorMailboxCacheEntry
					{
						ErrorCode = HttpStatusCode.NotFound,
						SubErrorCode = HttpProxySubErrorCode.DomainNotFound,
						SourceObject = this.ToCacheKey()
					});
					throw;
				}
			}
			session = DirectoryHelper.GetRootOrgRecipientSession();
			IL_95:
			ADRawEntry adrawEntry;
			if (base.IsArchive != null)
			{
				adrawEntry = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => session.FindByExchangeGuidIncludingAlternate(this.MailboxGuid, this.PropertySet));
			}
			else
			{
				adrawEntry = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => session.FindByExchangeGuidIncludingAlternate(this.MailboxGuid, MailboxGuidAnchorMailbox.ADRawEntryWithArchivePropertySet));
				if (adrawEntry != null && ((Guid)adrawEntry[ADUserSchema.ArchiveGuid]).Equals(this.MailboxGuid))
				{
					base.IsArchive = new bool?(true);
				}
			}
			if (adrawEntry == null && !string.IsNullOrEmpty(this.FallbackSmtp) && SmtpAddress.IsValidSmtpAddress(this.FallbackSmtp))
			{
				adrawEntry = new SmtpAnchorMailbox(this.FallbackSmtp, base.RequestContext)
				{
					IsArchive = base.IsArchive,
					NotFoundExceptionCreator = null
				}.GetADRawEntry();
			}
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(adrawEntry);
		}

		protected override string ToCacheKey()
		{
			return this.MailboxGuid.ToString();
		}

		protected override IRoutingKey GetRoutingKey()
		{
			return new MailboxGuidRoutingKey(this.MailboxGuid, this.Domain);
		}

		protected static readonly ADPropertyDefinition[] ADRawEntryWithArchivePropertySet = new ADPropertyDefinition[]
		{
			ADObjectSchema.OrganizationId,
			ADUserSchema.ArchiveGuid,
			ADMailboxRecipientSchema.Database,
			ADUserSchema.ArchiveDatabase,
			ADRecipientSchema.PrimarySmtpAddress
		};
	}
}
