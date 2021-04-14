using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Get", "PublicFolderStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderStatistics : GetTenantADObjectWithIdentityTaskBase<PublicFolderIdParameter, PublicFolderStatistics>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override PublicFolderIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["MailboxInformation"];
			}
			set
			{
				base.Fields["MailboxInformation"] = value;
			}
		}

		[Parameter]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return new Unlimited<uint>(10000U);
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity != null)
				{
					return this.Identity.PublicFolderId;
				}
				return null;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return this.Identity == null;
			}
		}

		protected sealed override IConfigDataProvider CreateSession()
		{
			OrganizationIdParameter organization = null;
			if (MapiTaskHelper.IsDatacenter)
			{
				organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(this.Organization, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			if (this.publicFolderStatisticsDataProvider == null || base.CurrentOrganizationId != this.publicFolderStatisticsDataProvider.CurrentOrganizationId)
			{
				if (this.publicFolderStatisticsDataProvider != null)
				{
					this.publicFolderStatisticsDataProvider.Dispose();
					this.publicFolderStatisticsDataProvider = null;
				}
				Guid mailboxGuid = Guid.Empty;
				if (base.Fields.IsModified("MailboxInformation"))
				{
					ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())), ExchangeErrorCategory.Client);
					if (aduser == null || aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
					{
						base.WriteError(new ObjectNotFoundException(Strings.PublicFolderMailboxNotFound), ExchangeErrorCategory.Client, aduser);
					}
					mailboxGuid = aduser.ExchangeGuid;
				}
				try
				{
					this.publicFolderStatisticsDataProvider = new PublicFolderStatisticsDataProvider(this.ConfigurationSession, "Get-PublicFolderStatistics", mailboxGuid);
				}
				catch (AccessDeniedException exception)
				{
					base.WriteError(exception, ErrorCategory.PermissionDenied, this.Identity);
				}
			}
			return this.publicFolderStatisticsDataProvider;
		}

		protected override void InternalProcessRecord()
		{
			if (this.Identity != null)
			{
				base.GetDataObject<PublicFolder>(this.Identity, this.publicFolderStatisticsDataProvider.PublicFolderDataProvider, null, new LocalizedString?(Strings.ErrorPublicFolderNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorPublicFolderNotUnique(this.Identity.ToString())));
			}
			base.InternalProcessRecord();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.publicFolderStatisticsDataProvider != null)
			{
				this.publicFolderStatisticsDataProvider.Dispose();
				this.publicFolderStatisticsDataProvider = null;
			}
		}

		private const string MailboxInformation = "MailboxInformation";

		private PublicFolderStatisticsDataProvider publicFolderStatisticsDataProvider;
	}
}
