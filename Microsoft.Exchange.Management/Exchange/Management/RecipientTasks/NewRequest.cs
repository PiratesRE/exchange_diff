using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class NewRequest<TRequest> : NewTaskBase<TransactionalRequestJob> where TRequest : RequestBase, new()
	{
		protected NewRequest()
		{
			this.MRSClient = null;
			this.RecipSession = null;
			this.GCSession = null;
			this.RJProvider = null;
			this.OrganizationId = null;
			this.MdbId = null;
			this.MdbServerSite = null;
			this.UnreachableMrsServers = new List<string>();
			this.NormalizedContentFilter = null;
			this.GeneralReportEntries = new List<ReportEntry>();
			this.PerRecordReportEntries = new List<ReportEntry>();
			this.ConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 95, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\NewRequest.cs");
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> BadItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["BadItemLimit"] ?? RequestTaskHelper.UnlimitedZero);
			}
			set
			{
				base.Fields["BadItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> LargeItemLimit
		{
			get
			{
				return (Unlimited<int>)(base.Fields["LargeItemLimit"] ?? RequestTaskHelper.UnlimitedZero);
			}
			set
			{
				base.Fields["LargeItemLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AcceptLargeDataLoss
		{
			get
			{
				return (SwitchParameter)(base.Fields["AcceptLargeDataLoss"] ?? false);
			}
			set
			{
				base.Fields["AcceptLargeDataLoss"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BatchName
		{
			get
			{
				return (string)base.Fields["BatchName"];
			}
			set
			{
				base.Fields["BatchName"] = value;
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
		public SwitchParameter Suspend
		{
			get
			{
				return (SwitchParameter)(base.Fields["Suspend"] ?? false);
			}
			set
			{
				base.Fields["Suspend"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SuspendComment
		{
			get
			{
				return (string)base.Fields["SuspendComment"];
			}
			set
			{
				base.Fields["SuspendComment"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RequestPriority Priority
		{
			get
			{
				return (RequestPriority)(base.Fields["Priority"] ?? RequestPriority.Normal);
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RequestWorkloadType WorkloadType
		{
			get
			{
				return (RequestWorkloadType)(base.Fields["WorkloadType"] ?? RequestWorkloadType.None);
			}
			set
			{
				base.Fields["WorkloadType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> CompletedRequestAgeLimit
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)(base.Fields["CompletedRequestAgeLimit"] ?? RequestTaskHelper.DefaultCompletedRequestAgeLimit);
			}
			set
			{
				base.Fields["CompletedRequestAgeLimit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SkippableMergeComponent[] SkipMerging
		{
			get
			{
				return (SkippableMergeComponent[])(base.Fields["SkipMerging"] ?? null);
			}
			set
			{
				base.Fields["SkipMerging"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InternalMrsFlag[] InternalFlags
		{
			get
			{
				return (InternalMrsFlag[])(base.Fields["InternalFlags"] ?? null);
			}
			set
			{
				base.Fields["InternalFlags"] = value;
			}
		}

		public PSCredential RemoteCredential
		{
			get
			{
				return (PSCredential)base.Fields["RemoteCredential"];
			}
			set
			{
				base.Fields["RemoteCredential"] = value;
			}
		}

		public SwitchParameter AllowLegacyDNMismatch
		{
			get
			{
				return (SwitchParameter)(base.Fields["AllowLegacyDNMismatch"] ?? false);
			}
			set
			{
				base.Fields["AllowLegacyDNMismatch"] = value;
			}
		}

		public string ContentFilter
		{
			get
			{
				return (string)base.Fields["ContentFilter"];
			}
			set
			{
				base.Fields["ContentFilter"] = value;
			}
		}

		public CultureInfo ContentFilterLanguage
		{
			get
			{
				return (CultureInfo)(base.Fields["ContentFilterLanguage"] ?? CultureInfo.InvariantCulture);
			}
			set
			{
				base.Fields["ContentFilterLanguage"] = value;
			}
		}

		public string[] IncludeFolders
		{
			get
			{
				return (string[])base.Fields["IncludeFolders"];
			}
			set
			{
				base.Fields["IncludeFolders"] = value;
			}
		}

		public string[] ExcludeFolders
		{
			get
			{
				return (string[])base.Fields["ExcludeFolders"];
			}
			set
			{
				base.Fields["ExcludeFolders"] = value;
			}
		}

		public SwitchParameter ExcludeDumpster
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExcludeDumpster"] ?? false);
			}
			set
			{
				base.Fields["ExcludeDumpster"] = value;
			}
		}

		public ConflictResolutionOption ConflictResolutionOption
		{
			get
			{
				return (ConflictResolutionOption)(base.Fields["ConflictResolutionOption"] ?? ConflictResolutionOption.KeepSourceItem);
			}
			set
			{
				base.Fields["ConflictResolutionOption"] = value;
			}
		}

		public FAICopyOption AssociatedMessagesCopyOption
		{
			get
			{
				return (FAICopyOption)(base.Fields["AssociatedMessagesCopyOption"] ?? FAICopyOption.Copy);
			}
			set
			{
				base.Fields["AssociatedMessagesCopyOption"] = value;
			}
		}

		public Fqdn RemoteHostName
		{
			get
			{
				return (Fqdn)base.Fields["RemoteHostName"];
			}
			set
			{
				base.Fields["RemoteHostName"] = value;
			}
		}

		public DateTime StartAfter
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

		internal MailboxReplicationServiceClient MRSClient { get; private set; }

		internal RequestJobProvider RJProvider { get; private set; }

		internal List<string> UnreachableMrsServers { get; private set; }

		internal ADObjectId MdbId { get; set; }

		internal ADObjectId MdbServerSite { get; set; }

		internal IRecipientSession GCSession { get; set; }

		internal IRecipientSession RecipSession { get; set; }

		internal IConfigurationSession CurrentOrgConfigSession { get; set; }

		internal ITopologyConfigurationSession ConfigSession { get; set; }

		internal string NormalizedContentFilter { get; private set; }

		internal string ExecutingUserIdentity
		{
			get
			{
				return base.ExecutingUserIdentityName;
			}
		}

		internal List<ReportEntry> GeneralReportEntries { get; private set; }

		internal List<ReportEntry> PerRecordReportEntries { get; private set; }

		internal OrganizationId OrganizationId { get; set; }

		internal RequestFlags Flags { get; set; }

		internal string RequestName { get; set; }

		protected virtual KeyValuePair<string, LocalizedString>[] ExtendedAttributes
		{
			get
			{
				return null;
			}
		}

		internal virtual RequestFlags LocateAndChooseMdb(ADObjectId sourceDatabaseId, ADObjectId targetDatabaseId, object sourceErrorObject, object targetErrorObject, object storageErrorObject, out ADObjectId mdb, out ADObjectId serverSite)
		{
			bool flag = false;
			bool flag2 = false;
			ADObjectId adobjectId = null;
			ADObjectId adobjectId2 = null;
			int num = 0;
			int num2 = 0;
			mdb = null;
			serverSite = null;
			if (sourceDatabaseId != null)
			{
				string text;
				string text2;
				this.CheckDatabase<MailboxDatabase>(new DatabaseIdParameter(sourceDatabaseId), NewRequest<TRequest>.DatabaseSide.Source, sourceErrorObject, out text, out text2, out adobjectId, out num);
				flag = this.IsSupportedDatabaseVersion(num, NewRequest<TRequest>.DatabaseSide.RequestStorage);
			}
			if (targetDatabaseId != null)
			{
				string text3;
				string text4;
				this.CheckDatabase<MailboxDatabase>(new DatabaseIdParameter(targetDatabaseId), NewRequest<TRequest>.DatabaseSide.Target, targetErrorObject, out text3, out text4, out adobjectId2, out num2);
				flag2 = this.IsSupportedDatabaseVersion(num2, NewRequest<TRequest>.DatabaseSide.RequestStorage);
			}
			if (!flag && !flag2)
			{
				base.WriteError(new MailboxDatabaseVersionUnsupportedPermanentException(Strings.ErrorStorageMailboxDatabaseVersionUnsupported), ErrorCategory.InvalidArgument, storageErrorObject);
				return RequestFlags.None;
			}
			if (flag && !flag2)
			{
				mdb = sourceDatabaseId;
				serverSite = adobjectId;
				return RequestFlags.Push;
			}
			if (!flag && flag2)
			{
				mdb = targetDatabaseId;
				serverSite = adobjectId2;
				return RequestFlags.Pull;
			}
			if (num > num2)
			{
				mdb = sourceDatabaseId;
				serverSite = adobjectId;
				return RequestFlags.Push;
			}
			mdb = targetDatabaseId;
			serverSite = adobjectId2;
			return RequestFlags.Pull;
		}

		protected virtual ADObjectId AutoSelectRequestQueueForPFRequest(OrganizationId orgId)
		{
			Guid guid = Guid.Empty;
			TenantPublicFolderConfigurationCache.Instance.RemoveValue(orgId);
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(orgId);
			PublicFolderInformation hierarchyMailboxInformation = value.GetHierarchyMailboxInformation();
			guid = hierarchyMailboxInformation.HierarchyMailboxGuid;
			if (guid == Guid.Empty)
			{
				base.WriteError(new RecipientTaskException(MrsStrings.PublicFolderMailboxesNotProvisionedForMigration), ExchangeErrorCategory.ServerOperation, null);
			}
			PublicFolderRecipient localMailboxRecipient = value.GetLocalMailboxRecipient(guid);
			return localMailboxRecipient.Database;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.RJProvider != null)
				{
					this.RJProvider.Dispose();
					this.RJProvider = null;
				}
				if (this.MRSClient != null)
				{
					this.MRSClient.Dispose();
					this.MRSClient = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			try
			{
				ReportEntry reportEntry = new ReportEntry(MrsStrings.ReportRequestCreated(this.ExecutingUserIdentity));
				reportEntry.Connectivity = new ConnectivityRec(ServerKind.Cmdlet, VersionInformation.MRS);
				this.GeneralReportEntries.Insert(0, reportEntry);
				RequestTaskHelper.ValidateItemLimits(this.BadItemLimit, this.LargeItemLimit, this.AcceptLargeDataLoss, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), this.ExecutingUserIdentity);
				if (this.SuspendComment != null && !this.Suspend)
				{
					base.WriteError(new SuspendCommentWithoutSuspendPermanentException(), ErrorCategory.InvalidArgument, this.SuspendComment);
				}
				if (!string.IsNullOrEmpty(this.SuspendComment) && this.SuspendComment.Length > 4096)
				{
					base.WriteError(new ParameterLengthExceededPermanentException("SuspendComment", 4096), ErrorCategory.InvalidArgument, this.SuspendComment);
				}
				if (!string.IsNullOrEmpty(this.Name) && this.Name.Length > 255)
				{
					base.WriteError(new ParameterLengthExceededPermanentException("Name", 255), ErrorCategory.InvalidArgument, this.Name);
				}
				if (!string.IsNullOrEmpty(this.BatchName) && this.BatchName.Length > 255)
				{
					base.WriteError(new ParameterLengthExceededPermanentException("BatchName", 255), ErrorCategory.InvalidArgument, this.BatchName);
				}
				if (!string.IsNullOrEmpty(this.ContentFilter))
				{
					this.NormalizedContentFilter = this.NormalizeContentFilter();
				}
				this.ValidateFolderFilter();
				base.InternalBeginProcessing();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null);
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			this.CurrentOrgConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 772, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\NewRequest.cs");
			sessionSettings = ADSessionSettings.RescopeToSubtree(sessionSettings);
			this.GCSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 783, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\NewRequest.cs");
			this.RecipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 791, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\NewRequest.cs");
			if (this.RJProvider != null)
			{
				this.RJProvider.Dispose();
				this.RJProvider = null;
			}
			this.RJProvider = new RequestJobProvider(this.RecipSession, this.CurrentOrgConfigSession);
			return this.RJProvider;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				this.RJProvider.AttachToMDB(this.MdbId.ObjectGuid);
				this.InitializeMRSClient();
				TransactionalRequestJob dataObject = this.DataObject;
				dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.Creation, new DateTime?(DateTime.UtcNow));
				dataObject.TimeTracker.CurrentState = RequestState.Queued;
				dataObject.JobType = MRSJobType.RequestJobE15_TenantHint;
				dataObject.RequestGuid = Guid.NewGuid();
				dataObject.AllowedToFinishMove = true;
				dataObject.BatchName = this.BatchName;
				dataObject.BadItemLimit = this.BadItemLimit;
				dataObject.LargeItemLimit = this.LargeItemLimit;
				dataObject.Status = RequestStatus.Queued;
				dataObject.RequestJobState = JobProcessingState.Ready;
				dataObject.Identity = new RequestJobObjectId(dataObject.RequestGuid, this.MdbId.ObjectGuid, null);
				dataObject.RequestQueue = ADObjectIdResolutionHelper.ResolveDN(this.MdbId);
				dataObject.RequestCreator = this.ExecutingUserIdentity;
				this.SetRequestProperties(dataObject);
				this.CreateIndexEntries(dataObject);
				dataObject.Suspend = this.Suspend;
				if (!string.IsNullOrEmpty(this.SuspendComment))
				{
					dataObject.Message = MrsStrings.MoveRequestMessageInformational(new LocalizedString(this.SuspendComment));
				}
				base.InternalValidate();
				if (this.MRSClient != null)
				{
					List<ReportEntry> entries = null;
					using (TransactionalRequestJob transactionalRequestJob = this.MRSClient.ValidateAndPopulateRequestJob(this.DataObject, out entries))
					{
						this.CopyDataToDataObject(transactionalRequestJob);
						RequestTaskHelper.WriteReportEntries(dataObject.Name, entries, dataObject.Identity, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				TransactionalRequestJob dataObject = this.DataObject;
				if (!base.Stopping)
				{
					ReportData reportData = new ReportData(dataObject.RequestGuid, dataObject.ReportVersion);
					reportData.Delete(this.RJProvider.SystemMailbox);
					reportData.Append(this.GeneralReportEntries);
					reportData.Append(this.PerRecordReportEntries);
					reportData.Flush(this.RJProvider.SystemMailbox);
					base.InternalProcessRecord();
					RequestJobLog.Write(dataObject);
					if (this.MRSClient != null)
					{
						if (this.MRSClient.ServerVersion[3])
						{
							this.MRSClient.RefreshMoveRequest2(dataObject.RequestGuid, this.MdbId.ObjectGuid, (int)dataObject.Flags, MoveRequestNotification.Created);
						}
						else
						{
							this.MRSClient.RefreshMoveRequest(dataObject.RequestGuid, this.MdbId.ObjectGuid, MoveRequestNotification.Created);
						}
					}
					dataObject.CreateAsyncNotification((base.ExchangeRunspaceConfig != null) ? base.ExchangeRunspaceConfig.ExecutingUserAsRecipient : null, this.ExtendedAttributes);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalStateReset()
		{
			if (this.DataObject != null)
			{
				this.DataObject.Dispose();
				this.DataObject = null;
			}
			if (this.MRSClient != null)
			{
				this.MRSClient.Dispose();
				this.MRSClient = null;
			}
			this.PerRecordReportEntries.Clear();
			base.InternalStateReset();
			this.MdbId = null;
			this.OrganizationId = null;
			this.Flags = RequestFlags.None;
			this.RequestName = null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return RequestTaskHelper.IsKnownExceptionHandler(exception, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)) || base.IsKnownException(exception);
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			LocalizedException ex = RequestTaskHelper.TranslateExceptionHandler(e);
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

		protected override void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			base.WriteVerbose(TaskVerboseStringHelper.GetReadObjectVerboseString(this.DataObject.Identity, base.DataSession, typeof(TransactionalRequestJob)));
			IConfigurable configurable = null;
			try
			{
				try
				{
					configurable = base.DataSession.Read<TransactionalRequestJob>(this.DataObject.Identity);
				}
				finally
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
				if (configurable == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.DataObject.Identity.ToString(), typeof(TRequest).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, this.DataObject.Identity);
				}
				this.WriteResult(configurable);
			}
			finally
			{
				IDisposable disposable = configurable as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
					configurable = null;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TRequest trequest = this.ConvertToPresentationObject(dataObject as TransactionalRequestJob);
			base.WriteResult(trequest);
		}

		protected virtual void SetRequestProperties(TransactionalRequestJob dataObject)
		{
			dataObject.WorkloadType = this.WorkloadType;
			dataObject.IncludeFolders = this.IncludeFolders;
			dataObject.ExcludeFolders = this.ExcludeFolders;
			dataObject.ExcludeDumpster = this.ExcludeDumpster;
			if (!string.IsNullOrEmpty(this.NormalizedContentFilter))
			{
				dataObject.ContentFilter = this.NormalizedContentFilter;
				dataObject.ContentFilterLCID = this.ContentFilterLanguage.LCID;
			}
			dataObject.ConflictResolutionOption = new ConflictResolutionOption?(this.ConflictResolutionOption);
			dataObject.AssociatedMessagesCopyOption = new FAICopyOption?(this.AssociatedMessagesCopyOption);
			dataObject.Priority = this.Priority;
			dataObject.CompletedRequestAgeLimit = this.CompletedRequestAgeLimit;
			RequestTaskHelper.ApplyOrganization(dataObject, this.OrganizationId ?? OrganizationId.ForestWideOrgId);
			dataObject.Flags = this.Flags;
			dataObject.Name = this.RequestName;
			this.SetSkipMergingAndInternalFlags(dataObject);
		}

		protected virtual void CreateIndexEntries(TransactionalRequestJob dataObject)
		{
			RequestIndexEntryProvider.CreateAndPopulateRequestIndexEntries(dataObject, this.CurrentOrgConfigSession);
		}

		protected virtual void CopyDataToDataObject(TransactionalRequestJob requestJob)
		{
			if (this.DataObject != null)
			{
				this.DataObject.SourceVersion = requestJob.SourceVersion;
				this.DataObject.SourceServer = requestJob.SourceServer;
				this.DataObject.SourceArchiveVersion = requestJob.SourceArchiveVersion;
				this.DataObject.SourceArchiveServer = requestJob.SourceArchiveServer;
				this.DataObject.TargetVersion = requestJob.TargetVersion;
				this.DataObject.TargetServer = requestJob.TargetServer;
				this.DataObject.TargetArchiveVersion = requestJob.TargetArchiveVersion;
				this.DataObject.TargetArchiveServer = requestJob.TargetArchiveServer;
			}
		}

		protected void ValidateLegacyDNMatch(string sourceDN, ADUser targetUser, object indicate)
		{
			if (!StringComparer.OrdinalIgnoreCase.Equals(sourceDN, targetUser.LegacyExchangeDN) && !targetUser.EmailAddresses.Contains(new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.X500, sourceDN, true)))
			{
				if (this.AllowLegacyDNMismatch)
				{
					this.PerRecordReportEntries.Add(new ReportEntry(MrsStrings.ReportRequestAllowedMismatch(this.ExecutingUserIdentity)));
					return;
				}
				base.WriteError(new NonMatchingLegacyDNPermanentException(sourceDN, targetUser.ToString(), "AllowLegacyDNMismatch"), ErrorCategory.InvalidArgument, indicate);
			}
		}

		protected bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		protected abstract TRequest ConvertToPresentationObject(TransactionalRequestJob dataObject);

		protected virtual RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(RequestIndexLocation.AD);
			}
		}

		protected bool CheckRequestOfTypeExists(MRSRequestType requestType)
		{
			RequestIndexEntryQueryFilter filter = new RequestIndexEntryQueryFilter(null, null, requestType, this.DefaultRequestIndexId, false);
			ObjectId rootId = ADHandler.GetRootId(this.CurrentOrgConfigSession, requestType);
			IEnumerable<TRequest> enumerable = ((RequestJobProvider)base.DataSession).IndexProvider.FindPaged<TRequest>(filter, rootId, rootId == null, null, 10);
			foreach (TRequest trequest in enumerable)
			{
				if (trequest.Type == requestType)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual string CheckRequestNameAvailability(string name, ADObjectId identifyingObjectId, bool objectIsMailbox, MRSRequestType requestType, object errorObject, bool wildcardedSearch)
		{
			string text = string.Format("{0}*", name);
			RequestIndexEntryQueryFilter requestIndexEntryQueryFilter = new RequestIndexEntryQueryFilter(wildcardedSearch ? text : name, identifyingObjectId, requestType, this.DefaultRequestIndexId, objectIsMailbox);
			requestIndexEntryQueryFilter.WildcardedNameSearch = wildcardedSearch;
			ObjectId rootId = ADHandler.GetRootId(this.CurrentOrgConfigSession, requestType);
			IEnumerable<TRequest> enumerable = ((RequestJobProvider)base.DataSession).IndexProvider.FindPaged<TRequest>(requestIndexEntryQueryFilter, rootId, rootId == null, null, 10);
			string result;
			using (IEnumerator<TRequest> enumerator = enumerable.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					result = name;
				}
				else if (!wildcardedSearch)
				{
					if (requestType.Equals(MRSRequestType.PublicFolderMigration) || requestType.Equals(MRSRequestType.PublicFolderMove))
					{
						base.WriteError(new MultipleSamePublicFolderMRSJobInstancesNotAllowedException(requestType.ToString()), ErrorCategory.InvalidArgument, errorObject);
					}
					else
					{
						base.WriteError(new NameMustBeUniquePermanentException(name, (identifyingObjectId == null) ? string.Empty : identifyingObjectId.ToString()), ErrorCategory.InvalidArgument, errorObject);
					}
					result = null;
				}
				else
				{
					Regex regex = new Regex(string.Format("^{0}(\\d)?$", name));
					List<uint> list = new List<uint>(10);
					for (uint num = 0U; num < 10U; num += 1U)
					{
						list.Add(num);
					}
					do
					{
						TRequest trequest = enumerator.Current;
						Match match = regex.Match(trequest.Name);
						if (match.Success)
						{
							string value = match.Groups[1].Value;
							uint num2;
							if (string.IsNullOrEmpty(value))
							{
								list.Remove(0U);
							}
							else if (uint.TryParse(value, out num2) && list.Contains(num2) && num2 != 0U)
							{
								list.Remove(num2);
							}
						}
					}
					while (enumerator.MoveNext() && list.Count > 0);
					if (list.Count == 0)
					{
						base.WriteError(new NoAvailableDefaultNamePermanentException(identifyingObjectId.ToString()), ErrorCategory.InvalidArgument, errorObject);
						result = null;
					}
					else if (list.Contains(0U))
					{
						result = name;
					}
					else
					{
						result = string.Format("{0}{1}", name, list[0]);
					}
				}
			}
			return result;
		}

		protected virtual bool IsSupportedDatabaseVersion(int serverVersion, NewRequest<TRequest>.DatabaseSide databaseSide)
		{
			bool flag = serverVersion >= Server.E15MinVersion;
			bool flag2 = serverVersion >= Server.E14MinVersion && serverVersion < Server.E15MinVersion;
			bool flag3 = serverVersion >= Server.E2007SP2MinVersion && serverVersion < Server.E14MinVersion;
			bool flag4 = serverVersion >= Server.E2k3SP2MinVersion && serverVersion < Server.E2007MinVersion;
			switch (databaseSide)
			{
			case NewRequest<TRequest>.DatabaseSide.Source:
				return flag || flag2 || flag3 || flag4;
			case NewRequest<TRequest>.DatabaseSide.Target:
			case NewRequest<TRequest>.DatabaseSide.RequestStorage:
				return flag;
			default:
				return false;
			}
		}

		protected virtual void InitializeMRSClient()
		{
			this.MRSClient = MailboxReplicationServiceClient.Create(this.ConfigSession, MRSJobType.RequestJobE15_TenantHint, this.MdbId.ObjectGuid, this.UnreachableMrsServers);
		}

		protected void ValidateName()
		{
			if (this.Name.Contains("\\"))
			{
				base.WriteError(new InvalidNameCharacterPermanentException(this.Name, "\\"), ErrorCategory.InvalidArgument, this.Name);
			}
		}

		protected void ValidateRootFolders(string sourceRootFolderPath, string targetRootFolderPath)
		{
			if (sourceRootFolderPath != null)
			{
				try
				{
					WellKnownFolderType wellKnownFolderType;
					List<string> list;
					FolderMappingFlags folderMappingFlags;
					FolderFilterParser.Parse(sourceRootFolderPath, out wellKnownFolderType, out list, out folderMappingFlags);
				}
				catch (FolderFilterPermanentException ex)
				{
					base.WriteError(new RootFolderInvalidPermanentException(CommonUtils.FullExceptionMessage(ex), ex), ErrorCategory.InvalidArgument, sourceRootFolderPath);
				}
			}
			if (targetRootFolderPath != null)
			{
				try
				{
					WellKnownFolderType wellKnownFolderType;
					List<string> list;
					FolderMappingFlags folderMappingFlags;
					FolderFilterParser.Parse(targetRootFolderPath, out wellKnownFolderType, out list, out folderMappingFlags);
				}
				catch (FolderFilterPermanentException ex2)
				{
					base.WriteError(new RootFolderInvalidPermanentException(CommonUtils.FullExceptionMessage(ex2), ex2), ErrorCategory.InvalidArgument, targetRootFolderPath);
				}
			}
		}

		protected T CheckDatabase<T>(DatabaseIdParameter databaseIdParameter, NewRequest<TRequest>.DatabaseSide databaseSide, object errorObject, out string serverName, out string serverDN, out ADObjectId serverSite, out int serverVersion) where T : Database, new()
		{
			T result = (T)((object)base.GetDataObject<T>(databaseIdParameter, this.ConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseIdParameter.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseIdParameter.ToString()))));
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(result.Id.ObjectGuid, null, null, FindServerFlags.None);
			serverName = databaseInformation.ServerFqdn;
			serverDN = databaseInformation.ServerDN;
			serverSite = databaseInformation.ServerSite;
			serverVersion = databaseInformation.ServerVersion;
			if (!this.IsSupportedDatabaseVersion(serverVersion, databaseSide))
			{
				base.WriteError(new DatabaseVersionUnsupportedPermanentException(result.Identity.ToString(), serverName, new ServerVersion(serverVersion).ToString()), ErrorCategory.InvalidArgument, errorObject);
			}
			return result;
		}

		protected void RescopeToOrgId(OrganizationId orgId)
		{
			if (orgId != null)
			{
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.RecipSession, orgId))
				{
					this.RecipSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.RecipSession, orgId, true);
				}
				if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.CurrentOrgConfigSession, orgId))
				{
					this.CurrentOrgConfigSession = (ITenantConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.CurrentOrgConfigSession, orgId, true);
					((RequestJobProvider)base.DataSession).IndexProvider.ConfigSession = this.CurrentOrgConfigSession;
				}
				this.OrganizationId = orgId;
			}
		}

		private void SetSkipMergingAndInternalFlags(TransactionalRequestJob dataObject)
		{
			RequestTaskHelper.SetSkipMerging(this.SkipMerging, dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			RequestTaskHelper.SetInternalFlags(this.InternalFlags, dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.SkipMerging == null)
			{
				using (SettingsContextBase.ActivateContext(dataObject))
				{
					dataObject.SkipKnownCorruptions = ConfigBase<MRSConfigSchema>.GetConfig<bool>("SkipKnownCorruptionsDefault");
				}
			}
		}

		private string NormalizeContentFilter()
		{
			RestrictionData restrictionData = null;
			string result = null;
			try
			{
				ContentFilterBuilder.ProcessContentFilter(this.ContentFilter, this.ContentFilterLanguage.LCID, this, new NewRequest<TRequest>.FakeFilterMapper(), out restrictionData, out result);
			}
			catch (ContentFilterPermanentException ex)
			{
				base.WriteError(new ContentFilterInvalidPermanentException(CommonUtils.FullExceptionMessage(ex), ex), ErrorCategory.InvalidArgument, this.ContentFilter);
			}
			base.WriteVerbose(Strings.ContentFilterUsedVerbose(restrictionData.ToString()));
			return result;
		}

		private void ValidateFolderFilter()
		{
			if (this.IncludeFolders != null)
			{
				try
				{
					foreach (string folderPath in this.IncludeFolders)
					{
						WellKnownFolderType wellKnownFolderType;
						List<string> list;
						FolderMappingFlags folderMappingFlags;
						FolderFilterParser.Parse(folderPath, out wellKnownFolderType, out list, out folderMappingFlags);
					}
				}
				catch (FolderFilterPermanentException ex)
				{
					base.WriteError(new FolderFilterInvalidPermanentException(CommonUtils.FullExceptionMessage(ex), ex), ErrorCategory.InvalidArgument, this.IncludeFolders);
				}
			}
			if (this.ExcludeFolders != null)
			{
				try
				{
					foreach (string folderPath2 in this.ExcludeFolders)
					{
						WellKnownFolderType wellKnownFolderType;
						List<string> list;
						FolderMappingFlags folderMappingFlags;
						FolderFilterParser.Parse(folderPath2, out wellKnownFolderType, out list, out folderMappingFlags);
					}
				}
				catch (FolderFilterPermanentException ex2)
				{
					base.WriteError(new FolderFilterInvalidPermanentException(CommonUtils.FullExceptionMessage(ex2), ex2), ErrorCategory.InvalidArgument, this.ExcludeFolders);
				}
			}
		}

		protected enum DatabaseSide
		{
			Source = 1,
			Target,
			RequestStorage
		}

		private class FakeFilterMapper : IFilterBuilderHelper
		{
			PropTag IFilterBuilderHelper.MapNamedProperty(NamedPropData npd, PropType propType)
			{
				return PropTag.Body;
			}

			Guid[] IFilterBuilderHelper.MapPolicyTag(string policyTagStr)
			{
				return new Guid[]
				{
					Guid.NewGuid()
				};
			}

			string[] IFilterBuilderHelper.MapRecipient(string recipientId)
			{
				return new string[]
				{
					"smtp:" + recipientId,
					"legDN:" + recipientId,
					"alias:" + recipientId
				};
			}
		}
	}
}
