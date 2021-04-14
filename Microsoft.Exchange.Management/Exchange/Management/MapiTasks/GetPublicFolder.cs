using System;
using System.Collections.Generic;
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
	[Cmdlet("Get", "PublicFolder", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolder : GetTenantADObjectWithIdentityTaskBase<PublicFolderIdParameter, PublicFolder>
	{
		[Parameter(ParameterSetName = "GetChildren", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(ParameterSetName = "Recurse", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		[Parameter(Mandatory = true, ParameterSetName = "Recurse")]
		public SwitchParameter Recurse
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recurse"] ?? false);
			}
			set
			{
				base.Fields["Recurse"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "GetChildren")]
		public SwitchParameter GetChildren
		{
			get
			{
				return (SwitchParameter)(base.Fields["GetChildren"] ?? false);
			}
			set
			{
				base.Fields["GetChildren"] = value;
			}
		}

		[Parameter(ParameterSetName = "Recurse")]
		[Parameter(ParameterSetName = "GetChildren")]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter ResidentFolders
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResidentFolders"] ?? false);
			}
			set
			{
				base.Fields["ResidentFolders"] = value;
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

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter internalFilter = base.InternalFilter;
				if (this.inputFilter == null)
				{
					return internalFilter;
				}
				if (internalFilter != null)
				{
					return new AndFilter(new QueryFilter[]
					{
						internalFilter,
						this.inputFilter
					});
				}
				return this.inputFilter;
			}
		}

		protected sealed override IConfigDataProvider CreateSession()
		{
			OrganizationIdParameter organization = null;
			if (MapiTaskHelper.IsDatacenter)
			{
				organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(this.Organization, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			this.rootId = null;
			if ((this.Recurse.IsPresent || this.GetChildren.IsPresent) && this.Identity != null)
			{
				this.rootId = this.Identity.PublicFolderId;
				this.Identity = null;
			}
			if (this.publicFolderDataProvider == null || base.CurrentOrganizationId != this.publicFolderDataProvider.CurrentOrganizationId)
			{
				if (this.publicFolderDataProvider != null)
				{
					this.publicFolderDataProvider.Dispose();
					this.publicFolderDataProvider = null;
				}
				Guid mailboxGuid = Guid.Empty;
				if (base.Fields.IsModified("Mailbox"))
				{
					ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())), ExchangeErrorCategory.Client);
					this.VerifyIsPublicFolderMailbox(aduser);
					mailboxGuid = aduser.ExchangeGuid;
				}
				try
				{
					this.publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, "Get-PublicFolder", mailboxGuid);
				}
				catch (AccessDeniedException exception)
				{
					base.WriteError(exception, ErrorCategory.PermissionDenied, this.Identity);
				}
			}
			if (this.ResidentFolders.IsPresent)
			{
				this.contentMailboxGuid = this.publicFolderDataProvider.PublicFolderSession.MailboxGuid;
				this.inputFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.ReplicaListBinary, this.contentMailboxGuid.ToByteArray());
			}
			return this.publicFolderDataProvider;
		}

		protected override void InternalProcessRecord()
		{
			if (this.rootId != null)
			{
				base.GetDataObject<PublicFolder>(new PublicFolderIdParameter(this.rootId), base.DataSession, null, new LocalizedString?(Strings.ErrorPublicFolderNotFound(this.rootId.ToString())), new LocalizedString?(Strings.ErrorPublicFolderNotUnique(this.rootId.ToString())));
			}
			base.InternalProcessRecord();
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
				return this.rootId;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return this.Recurse.IsPresent;
			}
		}

		protected override void InternalStateReset()
		{
			if (this.Identity == null)
			{
				this.Identity = new PublicFolderIdParameter(new PublicFolderId(MapiFolderPath.IpmSubtreeRoot));
			}
			this.contentMailboxGuid = Guid.Empty;
			this.inputFilter = null;
			base.InternalStateReset();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.publicFolderDataProvider != null)
			{
				this.publicFolderDataProvider.Dispose();
				this.publicFolderDataProvider = null;
			}
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			if (dataObjects != null && this.contentMailboxGuid != Guid.Empty)
			{
				base.WriteResult<T>(this.FilterRootPublicFolder<T>(dataObjects));
				return;
			}
			base.WriteResult<T>(dataObjects);
		}

		private IEnumerable<T> FilterRootPublicFolder<T>(IEnumerable<T> dataObjects)
		{
			using (IEnumerator<T> enumerator = dataObjects.GetEnumerator())
			{
				if (enumerator.MoveNext() && this.contentMailboxGuid.Equals((enumerator.Current as PublicFolder).ContentMailboxGuid))
				{
					yield return enumerator.Current;
				}
				while (enumerator.MoveNext())
				{
					!0 ! = enumerator.Current;
					yield return !;
				}
			}
			yield break;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PublicFolder publicFolder = (PublicFolder)dataObject;
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(base.CurrentOrganizationId);
			if (value != null)
			{
				if (value.GetLocalMailboxRecipient(publicFolder.ContentMailboxGuid) == null)
				{
					TenantPublicFolderConfigurationCache.Instance.RemoveValue(base.CurrentOrganizationId);
					value = TenantPublicFolderConfigurationCache.Instance.GetValue(base.CurrentOrganizationId);
				}
				PublicFolderRecipient localMailboxRecipient = value.GetLocalMailboxRecipient(publicFolder.ContentMailboxGuid);
				publicFolder.ContentMailboxName = ((localMailboxRecipient != null) ? localMailboxRecipient.MailboxName : string.Empty);
				if (base.NeedSuppressingPiiData)
				{
					publicFolder.ContentMailboxName = SuppressingPiiData.Redact(publicFolder.ContentMailboxName);
				}
				publicFolder.ResetChangeTracking();
			}
			base.WriteResult(publicFolder);
		}

		private void VerifyIsPublicFolderMailbox(ADUser adUser)
		{
			if (adUser == null || adUser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
			{
				base.WriteError(new ObjectNotFoundException(Strings.PublicFolderMailboxNotFound), ExchangeErrorCategory.Client, adUser);
			}
		}

		private const string ParameterRecurse = "Recurse";

		private const string ParameterGetChildren = "GetChildren";

		private const string ParameterSetRecurse = "Recurse";

		private const string ParameterSetGetChildren = "GetChildren";

		private const string MailboxFieldName = "Mailbox";

		private const string ParameterResidentFolders = "ResidentFolders";

		private QueryFilter inputFilter;

		private Guid contentMailboxGuid = Guid.Empty;

		private PublicFolderId rootId;

		private PublicFolderDataProvider publicFolderDataProvider;
	}
}
