using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "PublicFolderMigrationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MigrationLocalPublicFolder")]
	public sealed class NewPublicFolderMigrationRequest : NewRequest<PublicFolderMigrationRequest>
	{
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationLocalPublicFolder")]
		public DatabaseIdParameter SourceDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["SourceDatabase"];
			}
			set
			{
				base.Fields["SourceDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public Stream CSVStream { get; set; }

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public byte[] CSVData
		{
			get
			{
				return (byte[])base.Fields["CSVData"];
			}
			set
			{
				base.Fields["CSVData"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywherePublicFolder")]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywherePublicFolder")]
		public string RemoteMailboxLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteMailboxLegacyDN"];
			}
			set
			{
				base.Fields["RemoteMailboxLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywherePublicFolder")]
		[ValidateNotNullOrEmpty]
		public string RemoteMailboxServerLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteMailboxServerLegacyDN"];
			}
			set
			{
				base.Fields["RemoteMailboxServerLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywherePublicFolder")]
		[ValidateNotNull]
		public Fqdn OutlookAnywhereHostName
		{
			get
			{
				return (Fqdn)base.Fields["OutlookAnywhereHostName"];
			}
			set
			{
				base.Fields["OutlookAnywhereHostName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationOutlookAnywherePublicFolder")]
		public AuthenticationMethod AuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)(base.Fields["AuthenticationMethod"] ?? AuthenticationMethod.Basic);
			}
			set
			{
				base.Fields["AuthenticationMethod"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywherePublicFolder")]
		[ValidateNotNull]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewPublicFolderMigrationRequest((this.DataObject == null) ? base.RequestName : this.DataObject.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.CSVData == null && this.CSVStream == null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorAtleastOneOfCSVInput), ExchangeErrorCategory.Client, null);
			}
			if (this.CSVData != null && this.CSVStream != null)
			{
				base.WriteError(new RecipientTaskException(Strings.MigrationCsvParameterInvalid), ExchangeErrorCategory.Client, null);
			}
		}

		protected override void InternalStateReset()
		{
			this.sourceDatabase = null;
			base.InternalStateReset();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				int num = 0;
				ADObjectId adobjectId = null;
				if (base.ParameterSetName.Equals("MigrationOutlookAnywherePublicFolder"))
				{
					IConfigurationSession session = RequestTaskHelper.CreateOrganizationFindingSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
					if (this.Organization == null)
					{
						this.Organization = new OrganizationIdParameter(base.CurrentOrganizationId.OrganizationalUnit);
					}
					ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, session, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
					base.RescopeToOrgId(adorganizationalUnit.OrganizationId);
					base.Flags = (RequestFlags.CrossOrg | RequestFlags.Pull);
				}
				else if (base.ParameterSetName.Equals("MigrationLocalPublicFolder"))
				{
					base.OrganizationId = OrganizationId.ForestWideOrgId;
					base.Flags = (RequestFlags.IntraOrg | RequestFlags.Pull);
					PublicFolderDatabase publicFolderDatabase = base.CheckDatabase<PublicFolderDatabase>(this.SourceDatabase, NewRequest<PublicFolderMigrationRequest>.DatabaseSide.Source, this.SourceDatabase, out empty, out empty2, out adobjectId, out num);
					this.sourceDatabase = publicFolderDatabase.Id;
				}
				if (base.WorkloadType == RequestWorkloadType.None)
				{
					base.WorkloadType = (((base.Flags & RequestFlags.IntraOrg) == RequestFlags.IntraOrg) ? RequestWorkloadType.Local : RequestWorkloadType.Onboarding);
				}
				if (!string.IsNullOrEmpty(this.Name))
				{
					base.ValidateName();
					base.RequestName = this.Name;
				}
				else
				{
					base.RequestName = "PublicFolderMigration";
				}
				ADObjectId adObjectId = this.AutoSelectRequestQueueForPFRequest(base.OrganizationId);
				DatabaseIdParameter databaseIdParameter = new DatabaseIdParameter(adObjectId);
				ADObjectId mdbServerSite = null;
				MailboxDatabase mailboxDatabase = base.CheckDatabase<MailboxDatabase>(databaseIdParameter, NewRequest<PublicFolderMigrationRequest>.DatabaseSide.RequestStorage, this.Organization, out empty, out empty2, out mdbServerSite, out num);
				MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.CurrentOrgConfigSession.SessionSettings, mailboxDatabase, new Task.ErrorLoggerDelegate(base.WriteError));
				base.MdbId = mailboxDatabase.Id;
				base.MdbServerSite = mdbServerSite;
				this.CheckRequestNameAvailability(null, null, false, MRSRequestType.PublicFolderMigration, this.Organization, false);
				if (base.CheckRequestOfTypeExists(MRSRequestType.PublicFolderMailboxMigration))
				{
					base.WriteError(new MultiplePublicFolderMigrationTypesNotAllowedException(), ExchangeErrorCategory.Client, this.Organization);
				}
				this.publicFolderConfiguration = TenantPublicFolderConfigurationCache.Instance.GetValue(base.OrganizationId);
				if (this.publicFolderConfiguration.HeuristicsFlags.HasFlag(HeuristicsFlags.PublicFolderMigrationComplete))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorPublicFolderMigrationCompletedPreviously), ExchangeErrorCategory.Client, null);
				}
				this.ValidateCSV();
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
			dataObject.JobType = MRSJobType.RequestJobE15_CreatePublicFoldersUnderParentInSecondary;
			dataObject.RequestType = MRSRequestType.PublicFolderMigration;
			dataObject.WorkloadType = base.WorkloadType;
			dataObject.AuthenticationMethod = new AuthenticationMethod?(this.AuthenticationMethod);
			dataObject.RemoteMailboxLegacyDN = this.RemoteMailboxLegacyDN;
			dataObject.RemoteMailboxServerLegacyDN = this.RemoteMailboxServerLegacyDN;
			dataObject.OutlookAnywhereHostName = this.OutlookAnywhereHostName;
			dataObject.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, new AuthenticationMethod?(this.AuthenticationMethod));
			dataObject.PreserveMailboxSignature = false;
			dataObject.PreventCompletion = true;
			dataObject.SuspendWhenReadyToComplete = true;
			dataObject.AllowedToFinishMove = false;
			dataObject.SourceDatabase = this.sourceDatabase;
			dataObject.TargetDatabase = base.MdbId;
			dataObject.ExchangeGuid = this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
			dataObject.SourceExchangeGuid = dataObject.ExchangeGuid;
			dataObject.TargetExchangeGuid = dataObject.ExchangeGuid;
			dataObject.FolderToMailboxMap = this.folderToMailboxMapping;
			dataObject.AllowLargeItems = false;
			dataObject.SkipFolderPromotedProperties = true;
			dataObject.SkipFolderViews = true;
			dataObject.SkipFolderRestrictions = true;
			dataObject.SkipContentVerification = true;
		}

		protected override PublicFolderMigrationRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new PublicFolderMigrationRequest(dataObject.IndexEntries[0]);
			}
			base.WriteError(new RequestIndexEntriesAbsentPermanentException(dataObject.ToString()), ErrorCategory.InvalidArgument, this.Organization);
			return null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is CsvValidationException;
		}

		private void ValidateCSV()
		{
			Stream stream = null;
			try
			{
				Stream stream2;
				if (this.CSVStream != null)
				{
					stream2 = this.CSVStream;
				}
				else
				{
					stream2 = new MemoryStream(this.CSVData);
					stream = stream2;
				}
				PublicFolderMigrationRequestCsvSchema publicFolderMigrationRequestCsvSchema = new PublicFolderMigrationRequestCsvSchema();
				publicFolderMigrationRequestCsvSchema.PropertyValidationError += delegate(CsvRow row, string columnName, PropertyValidationError error)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorParsingCSV(row.Index, columnName, error.Description)), ExchangeErrorCategory.Client, null);
				};
				Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>();
				Dictionary<string, Guid> dictionary2 = null;
				bool flag = false;
				foreach (CsvRow csvRow in publicFolderMigrationRequestCsvSchema.Read(stream2))
				{
					if (csvRow.Index != 0)
					{
						if (!flag)
						{
							dictionary2 = this.CreateMailboxNameToGuidMap();
							flag = true;
						}
						string text = csvRow["FolderPath"];
						string identity = csvRow["TargetMailbox"];
						MailboxIdParameter mailboxIdParameter = MailboxIdParameter.Parse(identity);
						ADUser aduser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, base.RecipSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
						if (dictionary.ContainsKey(text))
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorParsingCSV(csvRow.Index, "FolderPath", "DuplicateFolderPathEntry")), ExchangeErrorCategory.Client, null);
						}
						else if (!dictionary2.ContainsValue(aduser.ExchangeGuid))
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorParsingCSV(csvRow.Index, "TargetMailbox", "InvalidPublicFolderMailbox")), ExchangeErrorCategory.Client, null);
						}
						dictionary.Add(text, aduser.ExchangeGuid);
						this.folderToMailboxMapping.Add(new FolderToMailboxMapping(text, aduser.ExchangeGuid));
					}
				}
				if (!flag)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorParsingCSV(1, "FolderPath", "NoInputDataFound")), ExchangeErrorCategory.Client, null);
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
			}
		}

		private Dictionary<string, Guid> CreateMailboxNameToGuidMap()
		{
			Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
			foreach (PublicFolderRecipient publicFolderRecipient in this.publicFolderConfiguration.GetAllMailboxRecipients())
			{
				dictionary.Add(publicFolderRecipient.MailboxName, publicFolderRecipient.MailboxGuid);
			}
			return dictionary;
		}

		private const string DefaultPublicFolderMigrationName = "PublicFolderMigration";

		private const string ParameterSetLocalPublicFolderMigration = "MigrationLocalPublicFolder";

		internal const string ParameterSetOutlookAnywherePublicFolderMigration = "MigrationOutlookAnywherePublicFolder";

		internal const string TaskNoun = "PublicFolderMigrationRequest";

		internal const string ParameterSourceDatabase = "SourceDatabase";

		internal const string ParameterCSVData = "CSVData";

		internal const string ParameterCSVStream = "CSVStream";

		internal const string ParameterOrganization = "Organization";

		internal const string ParameterRemoteMailboxLegacyDN = "RemoteMailboxLegacyDN";

		internal const string ParameterRemoteMailboxServerLegacyDN = "RemoteMailboxServerLegacyDN";

		internal const string ParameterOutlookAnywhereHostName = "OutlookAnywhereHostName";

		internal const string ParameterAuthenticationMethod = "AuthenticationMethod";

		private ADObjectId sourceDatabase;

		private TenantPublicFolderConfiguration publicFolderConfiguration;

		private List<FolderToMailboxMapping> folderToMailboxMapping = new List<FolderToMailboxMapping>();
	}
}
