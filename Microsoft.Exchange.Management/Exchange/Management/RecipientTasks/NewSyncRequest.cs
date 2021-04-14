using System;
using System.Management.Automation;
using System.Net;
using System.Security;
using Microsoft.Exchange.Configuration.Authorization;
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
	[Cmdlet("New", "SyncRequest", SupportsShouldProcess = true, DefaultParameterSetName = "AutoDetect")]
	public sealed class NewSyncRequest : NewRequest<SyncRequest>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Pop", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Eas", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Imap", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect", ValueFromPipeline = true)]
		public Guid AggregatedMailboxGuid
		{
			get
			{
				return (Guid)(base.Fields["AggregatedMailboxGuid"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["AggregatedMailboxGuid"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Eas", ValueFromPipeline = true)]
		[Parameter(Mandatory = true, ParameterSetName = "Olc", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Imap", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Pop", ValueFromPipeline = true)]
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

		[Parameter(Mandatory = true, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Olc")]
		[Parameter(Mandatory = true, ParameterSetName = "Imap")]
		[Parameter(Mandatory = true, ParameterSetName = "Eas")]
		public Fqdn RemoteServerName
		{
			get
			{
				return (Fqdn)base.Fields["RemoteServerName"];
			}
			set
			{
				base.Fields["RemoteServerName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "Olc")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		public int RemoteServerPort
		{
			get
			{
				return (int)(base.Fields["RemoteServerPort"] ?? 0);
			}
			set
			{
				base.Fields["RemoteServerPort"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		public Fqdn SmtpServerName
		{
			get
			{
				return (Fqdn)base.Fields["SmtpServerName"];
			}
			set
			{
				base.Fields["SmtpServerName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		public int SmtpServerPort
		{
			get
			{
				return (int)(base.Fields["SmtpServerPort"] ?? 0);
			}
			set
			{
				base.Fields["SmtpServerPort"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Pop")]
		[Parameter(Mandatory = true, ParameterSetName = "Eas")]
		[Parameter(Mandatory = true, ParameterSetName = "AutoDetect")]
		[Parameter(Mandatory = true, ParameterSetName = "Imap")]
		public SmtpAddress RemoteEmailAddress
		{
			get
			{
				return (SmtpAddress)(base.Fields["RemoteEmailAddress"] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields["RemoteEmailAddress"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		public string UserName
		{
			get
			{
				return (string)base.Fields["UserName"];
			}
			set
			{
				base.Fields["UserName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AutoDetect")]
		[Parameter(Mandatory = true, ParameterSetName = "Eas")]
		[Parameter(Mandatory = true, ParameterSetName = "Imap")]
		[Parameter(Mandatory = true, ParameterSetName = "Pop")]
		public SecureString Password
		{
			get
			{
				return (SecureString)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		public AuthenticationMethod Authentication
		{
			get
			{
				return (AuthenticationMethod)(base.Fields["Authentication"] ?? AuthenticationMethod.Basic);
			}
			set
			{
				base.Fields["Authentication"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		public IMAPSecurityMechanism Security
		{
			get
			{
				return (IMAPSecurityMechanism)(base.Fields["Security"] ?? IMAPSecurityMechanism.None);
			}
			set
			{
				base.Fields["Security"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		[Parameter(Mandatory = false, ParameterSetName = "Olc")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Imap")]
		public SwitchParameter Imap
		{
			get
			{
				return (SwitchParameter)(base.Fields["Imap"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Imap"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Pop")]
		public SwitchParameter Pop
		{
			get
			{
				return (SwitchParameter)(base.Fields["Pop"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Pop"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Eas")]
		public SwitchParameter Eas
		{
			get
			{
				return (SwitchParameter)(base.Fields["Eas"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Eas"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Olc")]
		public SwitchParameter Olc
		{
			get
			{
				return (SwitchParameter)(base.Fields["Olc"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Olc"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Olc")]
		public long? Puid
		{
			get
			{
				return (long?)base.Fields["Puid"];
			}
			set
			{
				base.Fields["Puid"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Olc")]
		public int? DGroup
		{
			get
			{
				return (int?)base.Fields["DGroup"];
			}
			set
			{
				base.Fields["DGroup"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AutoDetect")]
		[Parameter(Mandatory = true, ParameterSetName = "Eas")]
		[Parameter(Mandatory = true, ParameterSetName = "Pop")]
		[Parameter(Mandatory = true, ParameterSetName = "Imap")]
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

		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		public new DateTime StartAfter
		{
			get
			{
				return (DateTime)base.Fields["StartAfter"];
			}
			set
			{
				base.Fields["StartAfter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect")]
		public DateTime CompleteAfter
		{
			get
			{
				return (DateTime)base.Fields["CompleteAfter"];
			}
			set
			{
				base.Fields["CompleteAfter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect")]
		public TimeSpan IncrementalSyncInterval
		{
			get
			{
				return (TimeSpan)(base.Fields["IncrementalSyncInterval"] ?? TimeSpan.FromHours(1.0));
			}
			set
			{
				base.Fields["IncrementalSyncInterval"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Pop")]
		[Parameter(Mandatory = false, ParameterSetName = "Olc")]
		[Parameter(Mandatory = false, ParameterSetName = "Eas")]
		[Parameter(Mandatory = false, ParameterSetName = "AutoDetect")]
		[Parameter(Mandatory = false, ParameterSetName = "Imap")]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter ForestWideDomainControllerAffinityByExecutingUser
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForestWideDomainControllerAffinityByExecutingUser"] ?? false);
			}
			set
			{
				base.Fields["ForestWideDomainControllerAffinityByExecutingUser"] = value;
			}
		}

		protected override RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(this.targetUser.Id);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewSyncRequest((this.DataObject == null) ? base.RequestName : this.DataObject.ToString());
			}
		}

		protected override void CreateIndexEntries(TransactionalRequestJob dataObject)
		{
			RequestIndexEntryProvider.CreateAndPopulateRequestIndexEntries(dataObject, this.DefaultRequestIndexId);
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new NewSyncRequestTaskModuleFactory();
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (!this.Olc)
			{
				return base.CreateSession();
			}
			base.CreateSession();
			ADSessionSettings sessionSettings = ADSessionSettings.FromConsumerOrganization();
			base.GCSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 572, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\SyncRequest\\NewSyncRequest.cs");
			base.RecipSession = DirectorySessionFactory.NonCacheSessionFactory.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, sessionSettings, 578, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\SyncRequest\\NewSyncRequest.cs");
			base.CurrentOrganizationId = base.RecipSession.SessionSettings.CurrentOrganizationId;
			base.RJProvider.IndexProvider.RecipientSession = base.RecipSession;
			return base.RJProvider;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.Mailbox == null)
				{
					ADObjectId adObjectId;
					if (!base.TryGetExecutingUserId(out adObjectId))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					this.Mailbox = new MailboxIdParameter(adObjectId);
				}
				this.targetUser = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.Mailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
				base.RecipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(base.RecipSession, this.targetUser.OrganizationId, true);
				base.CurrentOrganizationId = this.targetUser.OrganizationId;
				base.RJProvider.IndexProvider.RecipientSession = base.RecipSession;
				if (this.targetUser.Database == null)
				{
					base.WriteError(new MailboxLacksDatabasePermanentException(this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
				}
				if (this.AggregatedMailboxGuid != Guid.Empty)
				{
					MultiValuedProperty<Guid> multiValuedProperty = this.targetUser.AggregatedMailboxGuids ?? new MultiValuedProperty<Guid>();
					if (!multiValuedProperty.Contains(this.AggregatedMailboxGuid))
					{
						base.WriteError(new AggregatedMailboxNotFoundPermanentException(this.AggregatedMailboxGuid, this.targetUser.ToString()), ErrorCategory.InvalidArgument, this.AggregatedMailboxGuid);
					}
				}
				this.DataObject.DomainControllerToUpdate = this.targetUser.OriginatingServer;
				bool wildcardedSearch = false;
				if (!string.IsNullOrEmpty(this.Name))
				{
					base.ValidateName();
					base.RequestName = this.Name;
				}
				else if (this.Olc)
				{
					base.RequestName = "OlcSync";
				}
				else
				{
					wildcardedSearch = true;
					base.RequestName = "Sync";
				}
				base.RescopeToOrgId(this.targetUser.OrganizationId);
				ADObjectId mdbId = null;
				ADObjectId mdbServerSite = null;
				RequestFlags requestFlags = this.LocateAndChooseMdb(null, this.targetUser.Database, null, this.Mailbox, this.Mailbox, out mdbId, out mdbServerSite);
				base.MdbId = mdbId;
				base.MdbServerSite = mdbServerSite;
				base.Flags = (RequestFlags.CrossOrg | requestFlags);
				base.RequestName = this.CheckRequestNameAvailability(base.RequestName, this.targetUser.Id, true, MRSRequestType.Sync, this.Mailbox, wildcardedSearch);
				if (this.Imap == true)
				{
					this.syncProtocol = SyncProtocol.Imap;
				}
				else if (this.Eas == true)
				{
					this.syncProtocol = SyncProtocol.Eas;
				}
				else if (this.Pop == true)
				{
					this.syncProtocol = SyncProtocol.Pop;
				}
				else if (this.Olc == true)
				{
					this.syncProtocol = SyncProtocol.Olc;
				}
				else
				{
					base.WriteError(new SyncProtocolNotSpecifiedPermanentException(), ErrorCategory.InvalidArgument, this.syncProtocol);
				}
				if (base.IsFieldSet("IncrementalSyncInterval"))
				{
					RequestTaskHelper.ValidateIncrementalSyncInterval(this.IncrementalSyncInterval, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				DateTime utcNow = DateTime.UtcNow;
				if (base.IsFieldSet("StartAfter"))
				{
					RequestTaskHelper.ValidateStartAfterTime(this.StartAfter.ToUniversalTime(), new Task.TaskErrorLoggingDelegate(base.WriteError), utcNow);
				}
				if (base.IsFieldSet("StartAfter") && base.IsFieldSet("CompleteAfter"))
				{
					RequestTaskHelper.ValidateStartAfterComesBeforeCompleteAfter(new DateTime?(this.StartAfter.ToUniversalTime()), new DateTime?(this.CompleteAfter.ToUniversalTime()), new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
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
			if (this.Eas)
			{
				dataObject.JobType = MRSJobType.RequestJobE15_SubType;
			}
			dataObject.RequestType = MRSRequestType.Sync;
			if (dataObject.WorkloadType == RequestWorkloadType.None)
			{
				dataObject.WorkloadType = RequestWorkloadType.SyncAggregation;
			}
			dataObject.RemoteHostName = this.RemoteServerName;
			dataObject.RemoteHostPort = this.RemoteServerPort;
			dataObject.SmtpServerName = this.SmtpServerName;
			dataObject.SmtpServerPort = this.SmtpServerPort;
			dataObject.EmailAddress = this.RemoteEmailAddress;
			dataObject.AuthenticationMethod = new AuthenticationMethod?(this.Authentication);
			dataObject.SecurityMechanism = this.Security;
			dataObject.SyncProtocol = this.syncProtocol;
			dataObject.IncrementalSyncInterval = this.IncrementalSyncInterval;
			if (this.Olc)
			{
				dataObject.AllowLargeItems = true;
				dataObject.UserPuid = this.Puid;
				dataObject.OlcDGroup = this.DGroup;
			}
			if (base.IsFieldSet("StartAfter"))
			{
				RequestTaskHelper.SetStartAfter(new DateTime?(this.StartAfter), dataObject, null);
			}
			else
			{
				RequestTaskHelper.SetStartAfter(new DateTime?(DateTime.MinValue), dataObject, null);
			}
			if (base.IsFieldSet("CompleteAfter"))
			{
				RequestTaskHelper.SetCompleteAfter(new DateTime?(this.CompleteAfter), dataObject, null);
			}
			else
			{
				dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.CompleteAfter, new DateTime?(DateTime.MaxValue));
			}
			if (string.IsNullOrEmpty(this.UserName))
			{
				this.UserName = this.RemoteEmailAddress.ToString();
			}
			if (!string.IsNullOrEmpty(this.TargetRootFolder))
			{
				dataObject.TargetRootFolder = this.TargetRootFolder;
			}
			dataObject.RemoteCredential = new NetworkCredential(this.UserName, this.Password);
			if (this.AggregatedMailboxGuid != Guid.Empty)
			{
				dataObject.Flags |= RequestFlags.TargetIsAggregatedMailbox;
			}
			if (this.targetUser != null)
			{
				dataObject.TargetUserId = this.targetUser.Id;
				dataObject.TargetUser = this.targetUser;
				dataObject.TargetExchangeGuid = ((this.AggregatedMailboxGuid != Guid.Empty) ? this.AggregatedMailboxGuid : this.targetUser.ExchangeGuid);
				dataObject.TargetDatabase = ADObjectIdResolutionHelper.ResolveDN(this.targetUser.Database);
				dataObject.TargetAlias = this.targetUser.Alias;
			}
		}

		protected override SyncRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new SyncRequest(dataObject.IndexEntries[0]);
			}
			return null;
		}

		public const string DefaultSyncName = "Sync";

		public const string DefaultOlcSyncName = "OlcSync";

		private const string ParameterForestWideDomainControllerAffinityByExecutingUser = "ForestWideDomainControllerAffinityByExecutingUser";

		public const string TaskNoun = "SyncRequest";

		public const string ParameterAggregatedMailboxGuid = "AggregatedMailboxGuid";

		public const string ParameterMailbox = "Mailbox";

		public const string ParameterRemoteEmailAddress = "RemoteEmailAddress";

		public const string ParameterRemoteServerName = "RemoteServerName";

		public const string ParameterRemoteServerPort = "RemoteServerPort";

		public const string ParameterSmtpServerName = "SmtpServerName";

		public const string ParameterSmtpServerPort = "SmtpServerPort";

		public const string ParameterUserName = "UserName";

		public const string ParameterPassword = "Password";

		public const string ParameterSecurity = "Security";

		public const string ParameterAuthentication = "Authentication";

		public const string ParameterForce = "Force";

		public const string ParameterImap = "Imap";

		public const string ParameterPop = "Pop";

		public const string ParameterEas = "Eas";

		public const string ParameterOlc = "Olc";

		public const string ParameterPuid = "Puid";

		public const string ParameterDGroup = "DGroup";

		public const string ParameterIncrementalSyncInterval = "IncrementalSyncInterval";

		public const string ParameterSetAutoDetect = "AutoDetect";

		public const string ParameterSetImap = "Imap";

		public const string ParameterSetPop = "Pop";

		public const string ParameterSetEas = "Eas";

		public const string ParameterSetOlc = "Olc";

		public const string ParameterTargetRootFolder = "TargetRootFolder";

		private ADUser targetUser;

		private SyncProtocol syncProtocol;
	}
}
