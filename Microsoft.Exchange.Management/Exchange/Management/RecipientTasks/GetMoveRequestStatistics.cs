using System;
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
	[Cmdlet("Get", "MoveRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMoveRequestStatistics : GetTaskBase<MoveRequestStatistics>
	{
		public GetMoveRequestStatistics()
		{
			this.fromMdb = null;
			this.gcSession = null;
			this.configSession = null;
			this.mrProvider = null;
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNull]
		public MoveRequestIdParameter Identity
		{
			get
			{
				return (MoveRequestIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeReport
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeReport"] ?? false);
			}
			set
			{
				base.Fields["IncludeReport"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationMoveRequestQueue")]
		public DatabaseIdParameter MoveRequestQueue
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["MoveRequestQueue"];
			}
			set
			{
				base.Fields["MoveRequestQueue"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MigrationMoveRequestQueue")]
		public Guid MailboxGuid
		{
			get
			{
				return (Guid)(base.Fields["MailboxGuid"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["MailboxGuid"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Diagnostic
		{
			get
			{
				return (SwitchParameter)(base.Fields["Diagnostic"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Diagnostic"] = value;
			}
		}

		[ValidateLength(1, 1048576)]
		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public string DiagnosticArgument
		{
			get
			{
				return (string)base.Fields["DiagnosticArgument"];
			}
			set
			{
				base.Fields["DiagnosticArgument"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
				{
					return new RequestJobQueryFilter(this.MailboxGuid, this.fromMdb.ObjectGuid, MRSRequestType.Move);
				}
				return null;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null);
			ADSessionSettings adsessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			adsessionSettings = ADSessionSettings.RescopeToSubtree(adsessionSettings);
			if (MapiTaskHelper.IsDatacenter || MapiTaskHelper.IsDatacenterDedicated)
			{
				adsessionSettings.IncludeSoftDeletedObjects = true;
			}
			this.gcSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 273, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\GetMoveRequestStatistics.cs");
			this.adSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 280, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\GetMoveRequestStatistics.cs");
			this.configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 286, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\MoveRequest\\GetMoveRequestStatistics.cs");
			if (this.mrProvider != null)
			{
				this.mrProvider.Dispose();
				this.mrProvider = null;
			}
			if (base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
			{
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.MoveRequestQueue, this.configSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.MoveRequestQueue.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.MoveRequestQueue.ToString())));
				this.mrProvider = new RequestJobProvider(mailboxDatabase.Guid);
			}
			else
			{
				this.mrProvider = new RequestJobProvider(this.gcSession, this.configSession);
			}
			this.mrProvider.LoadReport = this.IncludeReport;
			return this.mrProvider;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.mrProvider != null)
			{
				this.mrProvider.Dispose();
				this.mrProvider = null;
			}
			base.Dispose(disposing);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (base.ParameterSetName.Equals("Identity"))
				{
					ADUser aduser = (ADUser)RecipientTaskHelper.ResolveDataObject<ADUser>(this.adSession, this.gcSession, base.ServerSettings, this.Identity, null, base.OptionalIdentityData, this.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
					if (aduser.MailboxMoveStatus == RequestStatus.None)
					{
						base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotBeingMoved(aduser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					}
					else
					{
						MoveRequestStatistics moveRequestStatistics = (MoveRequestStatistics)this.mrProvider.Read<MoveRequestStatistics>(new RequestJobObjectId(aduser));
						if (moveRequestStatistics == null || moveRequestStatistics.Status == RequestStatus.None)
						{
							base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotBeingMoved(aduser.ToString())), ErrorCategory.InvalidArgument, this.Identity);
						}
						else
						{
							this.WriteResult(moveRequestStatistics);
						}
					}
				}
				else if (base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
				{
					if (this.MoveRequestQueue != null)
					{
						MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.MoveRequestQueue, this.configSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.MoveRequestQueue.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.MoveRequestQueue.ToString())));
						this.fromMdb = mailboxDatabase.Id;
					}
					this.mrProvider.AllowInvalid = true;
					base.InternalProcessRecord();
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return SetMoveRequestBase.IsKnownExceptionHandler(exception, new WriteVerboseDelegate(base.WriteVerbose)) || base.IsKnownException(exception);
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			LocalizedException ex = SetMoveRequestBase.TranslateExceptionHandler(e);
			if (ex == null)
			{
				ErrorCategory errorCategory;
				base.TranslateException(ref e, out errorCategory);
				category = errorCategory;
				return;
			}
			e = ex;
			category = ErrorCategory.ResourceUnavailable;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject
			});
			MoveRequestStatistics moveRequestStatistics = (MoveRequestStatistics)dataObject;
			try
			{
				RequestTaskHelper.GetUpdatedMRSRequestInfo(moveRequestStatistics, this.Diagnostic, this.DiagnosticArgument);
				if (moveRequestStatistics.Status == RequestStatus.Queued)
				{
					moveRequestStatistics.PositionInQueue = this.mrProvider.ComputePositionInQueue(moveRequestStatistics.ExchangeGuid);
				}
				base.WriteResult(moveRequestStatistics);
				string identity;
				if (moveRequestStatistics.UserId != null)
				{
					identity = moveRequestStatistics.UserId.ToString();
				}
				else if (moveRequestStatistics.Identity != null)
				{
					identity = moveRequestStatistics.Identity.ToString();
				}
				else
				{
					identity = moveRequestStatistics.ExchangeGuid.ToString();
				}
				if (moveRequestStatistics.ValidationResult != RequestJobBase.ValidationResultEnum.Valid)
				{
					this.WriteWarning(Strings.ErrorInvalidMoveRequest(identity, moveRequestStatistics.ValidationMessage));
				}
				if (moveRequestStatistics.PoisonCount > 5)
				{
					this.WriteWarning(Strings.WarningJobIsPoisoned(identity, moveRequestStatistics.PoisonCount));
				}
				if (base.ParameterSetName.Equals("MigrationMoveRequestQueue"))
				{
					base.WriteVerbose(Strings.RawRequestJobDump(CommonUtils.ConfigurableObjectToString(moveRequestStatistics)));
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		public const string ParameterIncludeReport = "IncludeReport";

		public const string ParameterMoveRequestQueue = "MoveRequestQueue";

		public const string ParameterMailboxGuid = "MailboxGuid";

		public const string ParameterIdentity = "Identity";

		public const string MoveRequestQueueSet = "MigrationMoveRequestQueue";

		public const string ParameterDiagnostic = "Diagnostic";

		public const string ParameterDiagnosticArgument = "DiagnosticArgument";

		private ADObjectId fromMdb;

		private IRecipientSession adSession;

		private IRecipientSession gcSession;

		private ITopologyConfigurationSession configSession;

		private RequestJobProvider mrProvider;
	}
}
