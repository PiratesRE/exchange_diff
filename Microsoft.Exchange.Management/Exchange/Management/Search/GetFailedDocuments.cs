using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Engine;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Search
{
	[Cmdlet("Get", "FailedContentIndexDocuments", SupportsShouldProcess = true, DefaultParameterSetName = "mailbox")]
	public sealed class GetFailedDocuments : GetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		[Alias(new string[]
		{
			"mailbox"
		})]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "mailbox", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "server", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "database", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public DatabaseIdParameter MailboxDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["MailboxDatabase"];
			}
			set
			{
				base.Fields["MailboxDatabase"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "mailbox")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FailureMode FailureMode
		{
			get
			{
				return (FailureMode)(base.Fields["FailureMode"] ?? FailureMode.Permanent);
			}
			set
			{
				base.Fields["FailureMode"] = value;
			}
		}

		[ValidateRange(1, EvaluationErrors.MaxFailureId)]
		[Parameter(Mandatory = false)]
		public int? ErrorCode
		{
			get
			{
				return (int?)base.Fields["ErrorCode"];
			}
			set
			{
				base.Fields["ErrorCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)base.Fields["StartDate"];
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? EndDate
		{
			get
			{
				return (DateTime?)base.Fields["EndDate"];
			}
			set
			{
				base.Fields["EndDate"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is RecipientTaskException;
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null)
			{
				base.InternalValidate();
				this.mailbox = (ADUser)base.GetDataObject<ADUser>(this.Identity, base.DataSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.Identity)), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.Identity)));
				if (!this.Archive)
				{
					MailboxDatabase item = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(new DatabaseIdParameter(this.mailbox.Database), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.mailbox.Database.Name)), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.mailbox.Database.Name)));
					this.databases = new List<MailboxDatabase>(1);
					this.databases.Add(item);
					return;
				}
				if (this.mailbox.ArchiveDatabase != null)
				{
					if (this.mailbox.ArchiveGuid != Guid.Empty)
					{
						if (this.mailbox.ArchiveDomain != null)
						{
							base.WriteError(new MdbAdminTaskException(Strings.ErrorRemoteArchiveNoStats(this.mailbox.ToString())), (ErrorCategory)1003, this.Identity);
						}
					}
					else
					{
						base.WriteError(new MdbAdminTaskException(Strings.ErrorArchiveNotEnabled(this.mailbox.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					}
					MailboxDatabase item2 = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(new DatabaseIdParameter(this.mailbox.ArchiveDatabase), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.mailbox.ArchiveDatabase.Name)), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.mailbox.ArchiveDatabase.Name)));
					this.databases = new List<MailboxDatabase>(1);
					this.databases.Add(item2);
					return;
				}
				base.WriteError(new MdbAdminTaskException(Strings.ErrorArchiveNotEnabled(this.mailbox.ToString())), ErrorCategory.InvalidArgument, this.Identity);
				return;
			}
			else
			{
				if (this.MailboxDatabase != null)
				{
					MailboxDatabase item3 = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.MailboxDatabase, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.MailboxDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.MailboxDatabase.ToString())));
					this.databases = new List<MailboxDatabase>(1);
					this.databases.Add(item3);
					return;
				}
				if (this.Server != null)
				{
					Server server = (Server)base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server)), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server)));
					this.databases = new List<MailboxDatabase>(server.GetMailboxDatabases());
				}
				return;
			}
		}

		protected override void InternalProcessRecord()
		{
			this.InitSubjectRetrievalEnabled();
			int num = 0;
			foreach (MailboxDatabase mdb in this.databases)
			{
				num += this.WriteFailures(mdb, this.ErrorCode, this.FailureMode, (ExDateTime?)this.StartDate, (ExDateTime?)this.EndDate);
			}
			if (this.Identity != null)
			{
				if (num == 0)
				{
					base.WriteVerbose(Strings.FailedDocumentsNoResultsMailbox(this.Identity.ToString()));
					return;
				}
				base.WriteVerbose(Strings.FailedDocumentsResultsMailbox(this.Identity.ToString(), num));
				return;
			}
			else
			{
				if (this.MailboxDatabase == null)
				{
					if (this.Server != null)
					{
						if (num == 0)
						{
							base.WriteVerbose(Strings.FailedDocumentsNoResultsServer(this.Server.ToString()));
							return;
						}
						base.WriteVerbose(Strings.FailedDocumentsResultsServer(this.Server.ToString(), num));
					}
					return;
				}
				if (num == 0)
				{
					base.WriteVerbose(Strings.FailedDocumentsNoResultsDatabase(this.MailboxDatabase.ToString()));
					return;
				}
				base.WriteVerbose(Strings.FailedDocumentsResultsDatabase(this.MailboxDatabase.ToString(), num));
				return;
			}
		}

		private int WriteFailures(MailboxDatabase mdb, int? errorcode, FailureMode failureMode, ExDateTime? startDate, ExDateTime? endDate)
		{
			int num = 0;
			StoreSession storeSession = null;
			ADUser aduser = null;
			Guid? mailboxGuid = null;
			if (this.Identity != null)
			{
				mailboxGuid = new Guid?(this.Archive ? this.mailbox.ArchiveGuid : this.mailbox.ExchangeGuid);
				aduser = this.mailbox;
			}
			MdbInfo mdbInfo = new MdbInfo(mdb);
			mdbInfo.EnableOwningServerUpdate = true;
			mdbInfo.UpdateDatabaseLocationInfo();
			Guid? guid = null;
			try
			{
				using (IFailedItemStorage failedItemStorage = Factory.Current.CreateFailedItemStorage(Factory.Current.CreateSearchServiceConfig(mdbInfo.Guid), mdbInfo.IndexSystemName, mdbInfo.OwningServer))
				{
					FailedItemParameters parameters = new FailedItemParameters(failureMode, FieldSet.Default)
					{
						MailboxGuid = mailboxGuid,
						ErrorCode = errorcode,
						StartDate = startDate,
						EndDate = endDate
					};
					foreach (IFailureEntry failureEntry in failedItemStorage.GetFailedItems(parameters))
					{
						if (failureEntry.MailboxGuid != guid)
						{
							guid = new Guid?(failureEntry.MailboxGuid);
							if (storeSession != null)
							{
								storeSession.Dispose();
								storeSession = null;
							}
							if (mailboxGuid == null)
							{
								aduser = this.GetADUser(guid.Value);
							}
							if (aduser != null && aduser.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
							{
								storeSession = this.OpenPublicFolderMailboxSession(mdbInfo.Guid, aduser);
							}
							else
							{
								storeSession = this.OpenMailboxSession(mdbInfo.Guid, guid.Value);
							}
						}
						if (storeSession != null)
						{
							int num2 = (int)storeSession.Mailbox.TryGetProperty(MailboxSchema.MailboxNumber);
							if (num2 != ((MdbItemIdentity)failureEntry.ItemId).MailboxNumber)
							{
								continue;
							}
						}
						string subject = this.GetSubject(failureEntry, storeSession);
						this.WriteResult(new FailedDocument(failureEntry, subject, mdbInfo.Name, aduser));
						num++;
					}
				}
			}
			catch (ComponentException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			finally
			{
				if (storeSession != null)
				{
					storeSession.Dispose();
				}
			}
			return num;
		}

		private ADUser GetADUser(Guid mailboxGuid)
		{
			string text = mailboxGuid.ToString();
			try
			{
				return (ADUser)base.GetDataObject<ADUser>(new MailboxIdParameter(text), base.DataSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(text)), new LocalizedString?(Strings.ErrorRecipientNotUnique(text)));
			}
			catch (ManagementObjectNotFoundException)
			{
				return null;
			}
			catch (ManagementObjectAmbiguousException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			return null;
		}

		private string GetSubject(IFailureEntry failure, StoreSession session)
		{
			if (this.subjectRetrievalEnabled && session != null)
			{
				try
				{
					using (Item item = Item.Bind(session, StoreObjectId.Deserialize(failure.EntryId)))
					{
						return (item.TryGetProperty(ItemSchema.Subject) as string) ?? string.Empty;
					}
				}
				catch (StoragePermanentException ex)
				{
					base.WriteWarning(ex.Message);
					return string.Empty;
				}
				catch (StorageTransientException ex2)
				{
					base.WriteWarning(ex2.Message);
					return string.Empty;
				}
			}
			return string.Empty;
		}

		private StoreSession OpenMailboxSession(Guid mdbGuid, Guid mailboxGuid)
		{
			if (mailboxGuid == Guid.Empty)
			{
				return null;
			}
			try
			{
				ExchangePrincipal mailboxOwner = ExchangePrincipal.FromMailboxData(mailboxGuid, mdbGuid, GetFailedDocuments.EmptyCultureInfoCollection, RemotingOptions.AllowCrossSite);
				return MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.InvariantCulture, "Client=Management;Action=GetFailedDocuments");
			}
			catch (StoragePermanentException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			catch (StorageTransientException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, null);
			}
			return null;
		}

		private StoreSession OpenPublicFolderMailboxSession(Guid mdbGuid, ADUser adUser)
		{
			try
			{
				return PublicFolderSession.OpenAsAdmin(adUser.OrganizationId, null, adUser.ExchangeGuid, null, CultureInfo.InvariantCulture, "Client=Management;Action=GetFailedDocuments", null);
			}
			catch (StoragePermanentException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			catch (StorageTransientException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, null);
			}
			return null;
		}

		private void InitSubjectRetrievalEnabled()
		{
			if (base.NeedSuppressingPiiData)
			{
				this.subjectRetrievalEnabled = false;
				return;
			}
			if (this.Identity != null)
			{
				this.subjectRetrievalEnabled = true;
				return;
			}
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ContentIndex\\"))
				{
					if (registryKey != null)
					{
						string value = registryKey.GetValue("SubjectEnabled") as string;
						bool flag;
						if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out flag))
						{
							this.subjectRetrievalEnabled = flag;
							return;
						}
					}
				}
			}
			catch (SecurityException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			this.subjectRetrievalEnabled = false;
		}

		private const string ServerParam = "Server";

		private const string MailboxDatabaseIDParam = "MailboxDatabase";

		private const string ContentIndexKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ContentIndex\\";

		private const string SubjectEnabledKeyName = "SubjectEnabled";

		private const string ParameterIsArchive = "Archive";

		private const string ParameterErrorCode = "ErrorCode";

		private const string ParameterFailureMode = "FailureMode";

		private const string ParameterStartDate = "StartDate";

		private const string ParameterEndDate = "EndDate";

		private static readonly ICollection<CultureInfo> EmptyCultureInfoCollection = new CultureInfo[0];

		private List<MailboxDatabase> databases;

		private bool subjectRetrievalEnabled;

		private ADUser mailbox;
	}
}
