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
	[Cmdlet("New", "PublicFolderMailboxMigrationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MailboxMigrationLocalPublicFolder")]
	public sealed class NewPublicFolderMailboxMigrationRequest : NewRequest<PublicFolderMailboxMigrationRequest>
	{
		[Parameter(Mandatory = true, ParameterSetName = "MailboxMigrationLocalPublicFolder")]
		[ValidateNotNull]
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

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
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
		[Parameter(Mandatory = true, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
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

		[Parameter(Mandatory = true, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
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

		[Parameter(Mandatory = true, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
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

		[Parameter(Mandatory = false, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
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

		[ValidateNotNull]
		[Parameter(Mandatory = true)]
		public MailboxIdParameter TargetMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["TargetMailbox"];
			}
			set
			{
				base.Fields["TargetMailbox"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MailboxMigrationOutlookAnywherePublicFolder")]
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

		internal ADUser TargetMailboxUser { get; set; }

		internal TenantPublicFolderConfiguration PublicFolderConfiguration { get; set; }

		private new string Name
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewPublicFolderMailboxMigrationRequest((this.DataObject == null) ? base.RequestName : this.DataObject.ToString());
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
				base.OrganizationId = OrganizationId.ForestWideOrgId;
				if (base.ParameterSetName.Equals("MailboxMigrationOutlookAnywherePublicFolder"))
				{
					IConfigurationSession session = RequestTaskHelper.CreateOrganizationFindingSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
					if (this.Organization == null && base.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
					{
						this.Organization = new OrganizationIdParameter(base.CurrentOrganizationId.OrganizationalUnit);
					}
					if (this.Organization != null)
					{
						ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, session, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
						base.OrganizationId = adorganizationalUnit.OrganizationId;
					}
					base.RescopeToOrgId(base.OrganizationId);
					base.Flags = (RequestFlags.CrossOrg | RequestFlags.Pull);
				}
				else if (base.ParameterSetName.Equals("MailboxMigrationLocalPublicFolder"))
				{
					base.Flags = (RequestFlags.IntraOrg | RequestFlags.Pull);
					PublicFolderDatabase publicFolderDatabase = base.CheckDatabase<PublicFolderDatabase>(this.SourceDatabase, NewRequest<PublicFolderMailboxMigrationRequest>.DatabaseSide.Source, this.SourceDatabase, out empty, out empty2, out adobjectId, out num);
					this.sourceDatabase = publicFolderDatabase.Id;
				}
				this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.OrganizationId), 408, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\PublicFolderMailboxMigrationRequest\\NewPublicFolderMailboxMigrationRequest.cs");
				this.TargetMailboxUser = (ADUser)base.GetDataObject<ADUser>(this.TargetMailbox, this.recipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.TargetMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.TargetMailbox.ToString())));
				TenantPublicFolderConfigurationCache.Instance.RemoveValue(base.OrganizationId);
				this.PublicFolderConfiguration = TenantPublicFolderConfigurationCache.Instance.GetValue(base.OrganizationId);
				if (this.PublicFolderConfiguration.HeuristicsFlags.HasFlag(HeuristicsFlags.PublicFolderMigrationComplete))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorPublicFolderMigrationCompletedPreviously), ExchangeErrorCategory.Client, null);
				}
				if (this.PublicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid == Guid.Empty)
				{
					base.WriteError(new RecipientTaskException(MrsStrings.PublicFolderMailboxesNotProvisionedForMigration), ExchangeErrorCategory.ServerOperation, null);
				}
				if (this.PublicFolderConfiguration.GetLocalMailboxRecipient(this.TargetMailboxUser.ExchangeGuid) == null)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotMigratePublicFolderIntoNonPublicFolderMailbox), ExchangeErrorCategory.Client, this.TargetMailboxUser);
				}
				if (base.WorkloadType == RequestWorkloadType.None)
				{
					base.WorkloadType = (((base.Flags & RequestFlags.IntraOrg) == RequestFlags.IntraOrg) ? RequestWorkloadType.Local : RequestWorkloadType.Onboarding);
				}
				base.RequestName = "PublicFolderMailboxMigration" + this.TargetMailboxUser.ExchangeGuid;
				ADObjectId requestQueueForMailboxMigrationRequest = this.GetRequestQueueForMailboxMigrationRequest();
				DatabaseIdParameter databaseIdParameter = new DatabaseIdParameter(requestQueueForMailboxMigrationRequest);
				ADObjectId mdbServerSite = null;
				MailboxDatabase mailboxDatabase = base.CheckDatabase<MailboxDatabase>(databaseIdParameter, NewRequest<PublicFolderMailboxMigrationRequest>.DatabaseSide.RequestStorage, this.Organization, out empty, out empty2, out mdbServerSite, out num);
				MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.CurrentOrgConfigSession.SessionSettings, mailboxDatabase, new Task.ErrorLoggerDelegate(base.WriteError));
				base.MdbId = mailboxDatabase.Id;
				base.MdbServerSite = mdbServerSite;
				base.RequestName = this.CheckRequestNameAvailability(base.RequestName, null, false, MRSRequestType.PublicFolderMailboxMigration, this.TargetMailbox, false);
				if (base.CheckRequestOfTypeExists(MRSRequestType.PublicFolderMigration))
				{
					base.WriteError(new MultiplePublicFolderMigrationTypesNotAllowedException(), ExchangeErrorCategory.Client, this.Organization);
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
			dataObject.RequestType = MRSRequestType.PublicFolderMailboxMigration;
			dataObject.WorkloadType = base.WorkloadType;
			if (base.ParameterSetName.Equals("MailboxMigrationOutlookAnywherePublicFolder"))
			{
				dataObject.AuthenticationMethod = new AuthenticationMethod?(this.AuthenticationMethod);
				dataObject.OutlookAnywhereHostName = this.OutlookAnywhereHostName;
				dataObject.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, new AuthenticationMethod?(this.AuthenticationMethod));
				dataObject.RemoteMailboxLegacyDN = this.RemoteMailboxLegacyDN;
				dataObject.RemoteMailboxServerLegacyDN = this.RemoteMailboxServerLegacyDN;
			}
			else
			{
				dataObject.SourceDatabase = this.sourceDatabase;
				dataObject.SourceExchangeGuid = this.sourceDatabase.ObjectGuid;
			}
			dataObject.PreserveMailboxSignature = false;
			dataObject.PreventCompletion = true;
			dataObject.SuspendWhenReadyToComplete = true;
			dataObject.AllowedToFinishMove = false;
			dataObject.SourceDatabase = this.sourceDatabase;
			dataObject.TargetDatabase = base.MdbId;
			dataObject.TargetUserId = this.TargetMailboxUser.Id;
			dataObject.ExchangeGuid = this.TargetMailboxUser.ExchangeGuid;
			dataObject.TargetExchangeGuid = this.TargetMailboxUser.ExchangeGuid;
			dataObject.FolderToMailboxMap = this.folderToMailboxMapping;
			dataObject.AllowLargeItems = false;
			dataObject.SkipFolderPromotedProperties = true;
			dataObject.SkipFolderViews = true;
			dataObject.SkipFolderRestrictions = true;
			dataObject.SkipContentVerification = true;
		}

		protected override PublicFolderMailboxMigrationRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new PublicFolderMailboxMigrationRequest(dataObject.IndexEntries[0]);
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
						ADUser aduser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, this.recipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
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
			foreach (PublicFolderRecipient publicFolderRecipient in this.PublicFolderConfiguration.GetAllMailboxRecipients())
			{
				dictionary.Add(publicFolderRecipient.MailboxName, publicFolderRecipient.MailboxGuid);
			}
			return dictionary;
		}

		private ADObjectId GetRequestQueueForMailboxMigrationRequest()
		{
			PublicFolderRecipient localMailboxRecipient = this.PublicFolderConfiguration.GetLocalMailboxRecipient(this.TargetMailboxUser.ExchangeGuid);
			if (localMailboxRecipient == null)
			{
				return null;
			}
			return localMailboxRecipient.Database;
		}

		private const string DefaultPublicFolderMailboxMigrationName = "PublicFolderMailboxMigration";

		private const string ParameterSetLocalPublicFolderMailboxMigration = "MailboxMigrationLocalPublicFolder";

		private const string ParameterSetOutlookAnywherePublicFolderMailboxMigration = "MailboxMigrationOutlookAnywherePublicFolder";

		private const string TaskNoun = "PublicFolderMailboxMigrationRequest";

		private const string ParameterSourceDatabase = "SourceDatabase";

		private const string ParameterCSVData = "CSVData";

		private const string ParameterCSVStream = "CSVStream";

		private const string ParameterOrganization = "Organization";

		private const string ParameterTargetMailbox = "TargetMailbox";

		private const string ParameterRemoteMailboxLegacyDN = "RemoteMailboxLegacyDN";

		private const string ParameterRemoteMailboxServerLegacyDN = "RemoteMailboxServerLegacyDN";

		private const string ParameterOutlookAnywhereHostName = "OutlookAnywhereHostName";

		private const string ParameterAuthenticationMethod = "AuthenticationMethod";

		private ADObjectId sourceDatabase;

		private List<FolderToMailboxMapping> folderToMailboxMapping = new List<FolderToMailboxMapping>();

		private IRecipientSession recipientSession;
	}
}
