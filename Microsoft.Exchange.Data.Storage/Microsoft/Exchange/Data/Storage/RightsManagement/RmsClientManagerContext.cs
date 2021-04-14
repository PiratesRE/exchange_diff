using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RmsClientManagerContext
	{
		public RmsClientManagerContext(OrganizationId orgId, IRmsLatencyTracker latencyTracker = null) : this(orgId, RmsClientManagerContext.ContextId.None, null, Guid.NewGuid(), null, latencyTracker)
		{
		}

		public RmsClientManagerContext(OrganizationId orgId, RmsClientManagerContext.ContextId contextId, string contextValue, string publishingLicense = null) : this(orgId, contextId, contextValue, Guid.NewGuid(), null, null)
		{
			if (!string.IsNullOrEmpty(publishingLicense))
			{
				this.orgId = RmsClientManagerUtils.OrgIdFromPublishingLicenseOrDefault(publishingLicense, orgId, out this.externalDirectoryOrgId);
			}
		}

		public RmsClientManagerContext(OrganizationId orgId, RmsClientManagerContext.ContextId contextId, string contextValue, Guid transactionId, Guid externalDirectoryOrgId) : this(orgId, contextId, contextValue, transactionId, null, null)
		{
			this.externalDirectoryOrgId = externalDirectoryOrgId;
		}

		public RmsClientManagerContext(OrganizationId orgId, RmsClientManagerContext.ContextId contextId, string contextValue, IADRecipientCache recipientCache, IRmsLatencyTracker latencyTracker, string publishingLicense, Guid externalDirectoryOrgId) : this(orgId, contextId, contextValue, recipientCache, latencyTracker, publishingLicense)
		{
			this.externalDirectoryOrgId = externalDirectoryOrgId;
		}

		public RmsClientManagerContext(OrganizationId orgId, RmsClientManagerContext.ContextId contextId, string contextValue, IADRecipientCache recipientCache, IRmsLatencyTracker latencyTracker, string publishingLicense = null) : this(orgId, contextId, contextValue, Guid.NewGuid(), recipientCache, latencyTracker)
		{
			if (!string.IsNullOrEmpty(publishingLicense))
			{
				this.orgId = RmsClientManagerUtils.OrgIdFromPublishingLicenseOrDefault(publishingLicense, orgId, out this.externalDirectoryOrgId);
			}
		}

		private RmsClientManagerContext(OrganizationId orgId, RmsClientManagerContext.ContextId contextId, string contextValue, Guid transactionId, IADRecipientCache recipientCache, IRmsLatencyTracker latencyTracker)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			this.orgId = orgId;
			this.tenantId = RmsClientManagerUtils.GetTenantGuidFromOrgId(this.orgId);
			this.contextId = contextId;
			this.contextValue = contextValue;
			this.transactionId = transactionId;
			this.recipientCache = recipientCache;
			this.latencyTracker = (latencyTracker ?? NoopRmsLatencyTracker.Instance);
			if (this.recipientCache != null && this.recipientCache.ADSession != null)
			{
				this.recipientSession = this.recipientCache.ADSession;
				return;
			}
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, this.orgId.Equals(OrganizationId.ForestWideOrgId) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 248, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\rightsmanagement\\RmsClientManagerContext.cs");
		}

		public Guid SystemProbeId { get; set; }

		public OrganizationId OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		public Guid ExternalDirectoryOrgId
		{
			get
			{
				return this.externalDirectoryOrgId;
			}
		}

		public Guid TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		public IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
		}

		public RmsClientManagerContext.ContextId ContextID
		{
			get
			{
				return this.contextId;
			}
		}

		public string ContextValue
		{
			get
			{
				return this.contextValue;
			}
		}

		public string ContextStringForm
		{
			get
			{
				if (this.contextId == RmsClientManagerContext.ContextId.None || string.IsNullOrEmpty(this.contextValue))
				{
					return null;
				}
				if (string.IsNullOrEmpty(this.contextStringForm))
				{
					this.contextStringForm = this.contextId.ToString() + ":" + this.contextValue;
				}
				return this.contextStringForm;
			}
		}

		public Guid TransactionId
		{
			get
			{
				return this.transactionId;
			}
		}

		public IRmsLatencyTracker LatencyTracker
		{
			get
			{
				return this.latencyTracker;
			}
		}

		public ADRawEntry ResolveRecipient(string recipient)
		{
			ArgumentValidator.ThrowIfNull("recipient", recipient);
			if (this.recipientCache == null)
			{
				this.recipientCache = new ADRecipientCache<ADRawEntry>(RmsClientManagerContext.propertyDefinitions, 1, this.orgId);
			}
			return this.recipientCache.FindAndCacheRecipient(new SmtpProxyAddress(recipient, true)).Data;
		}

		private static readonly ADPropertyDefinition[] propertyDefinitions = new ADPropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientTypeDetails
		};

		private readonly RmsClientManagerContext.ContextId contextId;

		private readonly string contextValue;

		private readonly Guid transactionId;

		private readonly Guid tenantId;

		private readonly IRecipientSession recipientSession;

		private readonly IRmsLatencyTracker latencyTracker;

		private readonly OrganizationId orgId;

		private readonly Guid externalDirectoryOrgId;

		private IADRecipientCache recipientCache;

		private string contextStringForm;

		internal enum ContextId
		{
			None,
			MessageId,
			MailboxGuid,
			AttachmentFileName
		}
	}
}
