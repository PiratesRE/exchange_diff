using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public sealed class RecipientOrOrganizationIdParameter
	{
		public SyncObjectId ResolvedSyncObjectId { get; set; }

		public string ResolvedServiceInstanceId { get; set; }

		public RecipientIdParameter RecipientParameter { get; set; }

		public OrganizationIdParameter OrganizationParameter { get; set; }

		public RecipientOrOrganizationIdParameter(string identity)
		{
			this.RecipientParameter = new RecipientIdParameter(identity);
			this.OrganizationParameter = new OrganizationIdParameter(identity);
		}

		public RecipientOrOrganizationIdParameter(ADObjectId adIdentity)
		{
			this.RecipientParameter = new RecipientIdParameter(adIdentity);
			this.OrganizationParameter = new OrganizationIdParameter(adIdentity);
		}

		public RecipientOrOrganizationIdParameter(INamedIdentity namedIdentity)
		{
			this.RecipientParameter = new RecipientIdParameter(namedIdentity);
			this.OrganizationParameter = new OrganizationIdParameter(namedIdentity);
		}

		public RecipientOrOrganizationIdParameter(ADObject recipient)
		{
			this.RecipientParameter = new RecipientIdParameter(recipient);
		}

		public RecipientOrOrganizationIdParameter(Microsoft.Exchange.Data.Directory.Management.User user)
		{
			this.RecipientParameter = new UserIdParameter(user);
		}

		public RecipientOrOrganizationIdParameter(MailUser mailUser)
		{
			this.RecipientParameter = new MailUserIdParameter(mailUser);
		}

		public RecipientOrOrganizationIdParameter(MailboxEntry storeMailboxEntry)
		{
			this.RecipientParameter = new MailboxIdParameter(storeMailboxEntry);
		}

		public RecipientOrOrganizationIdParameter(MailboxId storeMailboxId)
		{
			this.RecipientParameter = new MailboxIdParameter(storeMailboxId);
		}

		public RecipientOrOrganizationIdParameter(Mailbox mailbox)
		{
			this.RecipientParameter = new MailboxIdParameter(mailbox);
		}

		public RecipientOrOrganizationIdParameter(Microsoft.Exchange.Data.Directory.Management.Contact contact)
		{
			this.RecipientParameter = new ContactIdParameter(contact);
		}

		public RecipientOrOrganizationIdParameter(MailContact mailContact)
		{
			this.RecipientParameter = new MailContactIdParameter(mailContact);
		}

		public RecipientOrOrganizationIdParameter(WindowsGroup group)
		{
			this.RecipientParameter = new GroupIdParameter(group);
		}

		public RecipientOrOrganizationIdParameter(DistributionGroup group)
		{
			this.RecipientParameter = new GroupIdParameter(group);
		}

		public RecipientOrOrganizationIdParameter(TenantOrganizationPresentationObject tenant)
		{
			this.ResolvedSyncObjectId = new SyncObjectId(tenant.ExternalDirectoryOrganizationId, tenant.ExternalDirectoryOrganizationId, DirectoryObjectClass.Company);
			this.ResolvedServiceInstanceId = tenant.DirSyncServiceInstance;
		}

		public RecipientOrOrganizationIdParameter(OrganizationId organizationId)
		{
			this.OrganizationParameter = new OrganizationIdParameter(organizationId);
		}
	}
}
