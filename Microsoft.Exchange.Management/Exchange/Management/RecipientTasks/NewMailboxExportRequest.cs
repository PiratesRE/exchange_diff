using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MailboxExportRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MailboxExportRequest")]
	public sealed class NewMailboxExportRequest : NewRequest<MailboxExportRequest>
	{
		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MailboxExportRequest", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public MailboxOrMailUserIdParameter Mailbox
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MailboxExportRequest")]
		[ValidateNotNull]
		public LongPath FilePath
		{
			get
			{
				return (LongPath)base.Fields["FilePath"];
			}
			set
			{
				base.Fields["FilePath"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public string SourceRootFolder
		{
			get
			{
				return (string)base.Fields["SourceRootFolder"];
			}
			set
			{
				base.Fields["SourceRootFolder"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public string TargetRootFolder
		{
			get
			{
				return (string)base.Fields["TargetRootFolder"];
			}
			set
			{
				base.Fields["TargetRootFolder"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public SwitchParameter IsArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsArchive"] ?? false);
			}
			set
			{
				base.Fields["IsArchive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new PSCredential RemoteCredential
		{
			get
			{
				return base.RemoteCredential;
			}
			set
			{
				base.RemoteCredential = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new Fqdn RemoteHostName
		{
			get
			{
				return base.RemoteHostName;
			}
			set
			{
				base.RemoteHostName = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new string ContentFilter
		{
			get
			{
				return base.ContentFilter;
			}
			set
			{
				base.ContentFilter = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new CultureInfo ContentFilterLanguage
		{
			get
			{
				return base.ContentFilterLanguage;
			}
			set
			{
				base.ContentFilterLanguage = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new string[] IncludeFolders
		{
			get
			{
				return base.IncludeFolders;
			}
			set
			{
				base.IncludeFolders = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new string[] ExcludeFolders
		{
			get
			{
				return base.ExcludeFolders;
			}
			set
			{
				base.ExcludeFolders = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new SwitchParameter ExcludeDumpster
		{
			get
			{
				return base.ExcludeDumpster;
			}
			set
			{
				base.ExcludeDumpster = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new ConflictResolutionOption ConflictResolutionOption
		{
			get
			{
				return base.ConflictResolutionOption;
			}
			set
			{
				base.ConflictResolutionOption = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxExportRequest")]
		public new FAICopyOption AssociatedMessagesCopyOption
		{
			get
			{
				return base.AssociatedMessagesCopyOption;
			}
			set
			{
				base.AssociatedMessagesCopyOption = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxExportRequest((this.DataObject == null) ? base.RequestName : this.DataObject.ToString());
			}
		}

		protected override KeyValuePair<string, LocalizedString>[] ExtendedAttributes
		{
			get
			{
				return new KeyValuePair<string, LocalizedString>[]
				{
					new KeyValuePair<string, LocalizedString>("FilePath", new LocalizedString(this.FilePath.ToString())),
					new KeyValuePair<string, LocalizedString>("Mailbox", new LocalizedString(this.user.DisplayName))
				};
			}
		}

		protected override void InternalStateReset()
		{
			this.user = null;
			base.InternalStateReset();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider result;
			using (new ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler())
			{
				result = base.CreateSession();
			}
			return result;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.ValidateRootFolders(this.SourceRootFolder, this.TargetRootFolder);
				if (!this.FilePath.IsUnc)
				{
					base.WriteError(new NonUNCFilePathPermanentException(this.FilePath.PathName), ErrorCategory.InvalidArgument, this.FilePath);
				}
				this.user = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.Mailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
				bool wildcardedSearch = false;
				if (!string.IsNullOrEmpty(base.Name))
				{
					base.ValidateName();
					base.RequestName = base.Name;
				}
				else
				{
					wildcardedSearch = true;
					base.RequestName = "MailboxExport";
				}
				if (base.ParameterSetName.Equals("MailboxExportRequest"))
				{
					this.DisallowExportFromPublicFolderMailbox();
					if (this.user.RecipientType != RecipientType.UserMailbox)
					{
						base.WriteError(new InvalidRecipientTypePermanentException(this.user.ToString(), this.user.RecipientType.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
					}
					if (this.IsArchive && (this.user.ArchiveGuid == Guid.Empty || this.user.ArchiveDatabase == null))
					{
						base.WriteError(new MailboxLacksArchivePermanentException(this.user.ToString()), ErrorCategory.InvalidArgument, this.IsArchive);
					}
					if (!this.IsArchive && this.user.Database == null)
					{
						base.WriteError(new MailboxLacksDatabasePermanentException(this.user.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
					}
					base.RescopeToOrgId(this.user.OrganizationId);
					ADObjectId mdbId = null;
					ADObjectId mdbServerSite = null;
					this.LocateAndChooseMdb(this.IsArchive ? this.user.ArchiveDatabase : this.user.Database, null, this.Mailbox, null, this.Mailbox, out mdbId, out mdbServerSite);
					base.MdbId = mdbId;
					base.MdbServerSite = mdbServerSite;
					base.Flags = (((this.RemoteHostName == null) ? RequestFlags.IntraOrg : RequestFlags.CrossOrg) | RequestFlags.Push);
				}
				base.RequestName = this.CheckRequestNameAvailability(base.RequestName, this.user.Id, true, MRSRequestType.MailboxExport, this.Mailbox, wildcardedSearch);
				base.InternalValidate();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void SetRequestProperties(TransactionalRequestJob dataObject)
		{
			base.SetRequestProperties(dataObject);
			dataObject.RequestType = MRSRequestType.MailboxExport;
			if (dataObject.WorkloadType == RequestWorkloadType.None)
			{
				if (string.IsNullOrEmpty(this.RemoteHostName))
				{
					dataObject.WorkloadType = RequestWorkloadType.Local;
				}
				else
				{
					dataObject.WorkloadType = RequestWorkloadType.RemotePstExport;
				}
			}
			dataObject.FilePath = this.FilePath.PathName;
			if (this.user != null)
			{
				dataObject.SourceUserId = this.user.Id;
				dataObject.SourceUser = this.user;
			}
			if (base.ParameterSetName.Equals("MailboxExportRequest"))
			{
				if (!string.IsNullOrEmpty(this.SourceRootFolder))
				{
					dataObject.SourceRootFolder = this.SourceRootFolder;
				}
				if (!string.IsNullOrEmpty(this.TargetRootFolder))
				{
					dataObject.TargetRootFolder = this.TargetRootFolder;
				}
				if (this.IsArchive)
				{
					dataObject.SourceIsArchive = true;
					dataObject.SourceExchangeGuid = this.user.ArchiveGuid;
					dataObject.SourceDatabase = ADObjectIdResolutionHelper.ResolveDN(this.user.ArchiveDatabase);
				}
				else
				{
					dataObject.SourceIsArchive = false;
					dataObject.SourceExchangeGuid = this.user.ExchangeGuid;
					dataObject.SourceDatabase = ADObjectIdResolutionHelper.ResolveDN(this.user.Database);
				}
				dataObject.SourceAlias = this.user.Alias;
				if (this.RemoteCredential != null)
				{
					dataObject.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, null);
				}
				if (!string.IsNullOrEmpty(this.RemoteHostName))
				{
					dataObject.RemoteHostName = this.RemoteHostName;
				}
			}
		}

		protected override MailboxExportRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new MailboxExportRequest(dataObject.IndexEntries[0]);
			}
			base.WriteError(new RequestIndexEntriesAbsentPermanentException(dataObject.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
			return null;
		}

		private void DisallowExportFromPublicFolderMailbox()
		{
			if (this.user != null && this.user.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorCannotExportPstFromPublicFolderMailbox(this.user.Name)), ErrorCategory.InvalidArgument, this.Mailbox);
			}
		}

		public const string DefaultMailboxExportName = "MailboxExport";

		public const string TaskNoun = "MailboxExportRequest";

		public const string ParameterSetMailboxExport = "MailboxExportRequest";

		private ADUser user;
	}
}
