using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.Sync.CookieManager;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[OutputType(new Type[]
	{
		typeof(DeltaSyncSummary)
	}, ParameterSetName = new string[]
	{
		"GetChanges"
	})]
	[OutputType(new Type[]
	{
		typeof(TenantSyncSummary)
	}, ParameterSetName = new string[]
	{
		"GetContext"
	})]
	[Cmdlet("Get", "MsoDiagnostics", DefaultParameterSetName = "GetChanges")]
	[OutputType(new Type[]
	{
		typeof(BacklogSummary)
	}, ParameterSetName = new string[]
	{
		"EstimateBacklog"
	})]
	public class GetMsoDiagnostics : Task
	{
		[Parameter(ParameterSetName = "GetChanges", ValueFromPipelineByPropertyName = true)]
		[Parameter(ParameterSetName = "GetContext", ValueFromPipelineByPropertyName = true)]
		[Alias(new string[]
		{
			"NextCookie"
		})]
		[AllowNull]
		public byte[] Cookie
		{
			get
			{
				return (byte[])base.Fields[GetMsoDiagnostics.ParameterNames.Cookie];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.Cookie] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "GetChanges")]
		public SwitchParameter DeltaSync
		{
			get
			{
				return (SwitchParameter)base.Fields[GetMsoDiagnostics.ParameterNames.DeltaSync];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.DeltaSync] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EstimateBacklog")]
		public SwitchParameter EstimateBacklog
		{
			get
			{
				return (SwitchParameter)base.Fields[GetMsoDiagnostics.ParameterNames.EstimateBacklog];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.EstimateBacklog] = value;
			}
		}

		[ValidateRange(1, 10)]
		[Parameter(ValueFromPipelineByPropertyName = true)]
		public int MaxNumberOfBatches
		{
			get
			{
				return (int)base.Fields[GetMsoDiagnostics.ParameterNames.MaxNumberOfBatches];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.MaxNumberOfBatches] = value;
			}
		}

		[Parameter(ParameterSetName = "GetContext", Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Alias(new string[]
		{
			"ContextId"
		})]
		public string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)base.Fields[GetMsoDiagnostics.ParameterNames.OrganizationId];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.OrganizationId] = value;
			}
		}

		[Parameter(ParameterSetName = "EstimateBacklog", ValueFromPipelineByPropertyName = true)]
		[Parameter(ParameterSetName = "GetContext", ValueFromPipelineByPropertyName = true)]
		[AllowNull]
		public byte[] PageToken
		{
			get
			{
				return (byte[])base.Fields[GetMsoDiagnostics.ParameterNames.PageToken];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.PageToken] = value;
			}
		}

		[Parameter(ParameterSetName = "GetChanges", ValueFromPipelineByPropertyName = true)]
		[ValidateRange(1, 10)]
		[Parameter(ParameterSetName = "GetContext", ValueFromPipelineByPropertyName = true)]
		public int SampleCountForStatistics
		{
			get
			{
				return (int)base.Fields[GetMsoDiagnostics.ParameterNames.SampleCountForStatistics];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.SampleCountForStatistics] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public string ServiceInstanceId
		{
			get
			{
				return (string)base.Fields[GetMsoDiagnostics.ParameterNames.ServiceInstanceId];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.ServiceInstanceId] = value;
			}
		}

		[Parameter(ParameterSetName = "GetContext", Mandatory = true)]
		public SwitchParameter TenantSync
		{
			get
			{
				return (SwitchParameter)base.Fields[GetMsoDiagnostics.ParameterNames.TenantSync];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.TenantSync] = value;
			}
		}

		[Parameter(ParameterSetName = "GetChanges", ValueFromPipelineByPropertyName = true)]
		[Parameter(ParameterSetName = "GetContext", ValueFromPipelineByPropertyName = true)]
		public SwitchParameter UseLastCommittedCookie
		{
			get
			{
				return (SwitchParameter)base.Fields[GetMsoDiagnostics.ParameterNames.UseLastCommittedCookie];
			}
			set
			{
				base.Fields[GetMsoDiagnostics.ParameterNames.UseLastCommittedCookie] = value;
			}
		}

		public GetMsoDiagnostics()
		{
			this.Cookie = null;
			this.DeltaSync = false;
			this.EstimateBacklog = false;
			this.MaxNumberOfBatches = 3;
			this.ExternalDirectoryOrganizationId = null;
			this.PageToken = null;
			this.SampleCountForStatistics = 3;
			this.ServiceInstanceId = null;
			this.TenantSync = false;
			this.UseLastCommittedCookie = false;
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Cookie != null && this.UseLastCommittedCookie.IsPresent)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.GetMsoDiagnosticsExclusiveParameters(GetMsoDiagnostics.ParameterNames.Cookie.ToString(), GetMsoDiagnostics.ParameterNames.UseLastCommittedCookie.ToString())), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalBeginProcessing()
		{
			this.MsoSyncService = new MsoSyncService();
			if (base.ParameterSetName == "EstimateBacklog")
			{
				this.UseLastCommittedCookie = SwitchParameter.Present;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.MsoSyncService.Dispose();
			}
		}

		protected override void InternalProcessRecord()
		{
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (parameterSetName == "GetChanges")
				{
					this.ProcessGetChanges();
					return;
				}
				if (parameterSetName == "GetContext")
				{
					this.ProcessGetContext();
					return;
				}
				if (!(parameterSetName == "EstimateBacklog"))
				{
					return;
				}
				this.ProcessEstimateBacklog();
			}
		}

		private void FindTheRightCookie()
		{
			if (this.UseLastCommittedCookie.IsPresent)
			{
				CookieManager cookieManager = CookieManagerFactory.Default.GetCookieManager(ForwardSyncCookieType.CompanyIncremental, this.ServiceInstanceId, 1, TimeSpan.FromMinutes(30.0));
				this.Cookie = cookieManager.ReadCookie();
				base.WriteVerbose(Strings.GetMsoDiagnosticsLastCommittedCookieBeingUsed(cookieManager.GetMostRecentCookieTimestamp()));
				return;
			}
			if (this.Cookie == null)
			{
				this.Cookie = this.MsoSyncService.GetNewCookieForAllObjectsTypes(this.ServiceInstanceId);
				this.WriteWarning(Strings.GetMsoDiagnosticsNewCookieIsBeingUsed);
			}
		}

		private void ProcessGetChanges()
		{
			DeltaSyncSummary deltaSyncSummary = new DeltaSyncSummary();
			List<IEnumerable<DeltaSyncBatchResults>> list = deltaSyncSummary.Samples as List<IEnumerable<DeltaSyncBatchResults>>;
			this.FindTheRightCookie();
			ExProgressRecord exProgressRecord = new ExProgressRecord(1, new LocalizedString("Delta Sync"), new LocalizedString("Sync Call"));
			ExProgressRecord exProgressRecord2 = new ExProgressRecord(2, new LocalizedString("Sync Call"), new LocalizedString("GetChanges"));
			exProgressRecord2.ParentActivityId = exProgressRecord.ActivityId;
			for (int i = 0; i < this.SampleCountForStatistics; i++)
			{
				exProgressRecord.CurrentOperation = Strings.GetMsoDiagnosticsProgressIteration(i + 1, this.SampleCountForStatistics);
				exProgressRecord.PercentComplete = i * 100 / this.SampleCountForStatistics;
				exProgressRecord.RecordType = ProgressRecordType.Processing;
				base.WriteProgress(exProgressRecord);
				byte[] lastCookie = this.Cookie;
				List<DeltaSyncBatchResults> list2 = new List<DeltaSyncBatchResults>();
				list.Add(list2);
				for (int j = 0; j < this.MaxNumberOfBatches; j++)
				{
					exProgressRecord2.CurrentOperation = Strings.GetMsoDiagnosticsProgressBatch(j + 1, this.MaxNumberOfBatches);
					exProgressRecord2.PercentComplete = j * 100 / this.MaxNumberOfBatches;
					exProgressRecord2.RecordType = ProgressRecordType.Processing;
					base.WriteProgress(exProgressRecord2);
					ExDateTime now = ExDateTime.Now;
					GetChangesResponse changes = this.MsoSyncService.SyncProxy.GetChanges(new GetChangesRequest(lastCookie));
					TimeSpan responseTime = ExDateTime.Now - now;
					DirectoryChanges getChangesResult = changes.GetChangesResult;
					if (getChangesResult != null)
					{
						DeltaSyncBatchResults deltaSyncBatchResults = new DeltaSyncBatchResults(getChangesResult);
						deltaSyncBatchResults.Stats.ResponseTime = responseTime;
						deltaSyncBatchResults.LastCookie = lastCookie;
						deltaSyncBatchResults.RawResponse = this.MsoSyncService.RawResponse;
						deltaSyncBatchResults.CalculateStats();
						list2.Add(deltaSyncBatchResults);
						lastCookie = getChangesResult.NextCookie;
					}
					if (getChangesResult == null || !getChangesResult.More)
					{
						break;
					}
				}
				if (list2.Last<DeltaSyncBatchResults>().More)
				{
					this.WriteWarning(Strings.GetMsoDiagnosticsMoreDataIsAvailable);
				}
				exProgressRecord2.RecordType = ProgressRecordType.Completed;
				base.WriteProgress(exProgressRecord2);
			}
			exProgressRecord.RecordType = ProgressRecordType.Completed;
			base.WriteProgress(exProgressRecord);
			deltaSyncSummary.CalculateStats();
			base.WriteObject(deltaSyncSummary);
		}

		private void ProcessGetContext()
		{
			TenantSyncSummary tenantSyncSummary = new TenantSyncSummary();
			List<IEnumerable<TenantSyncBatchResults>> list = tenantSyncSummary.Samples as List<IEnumerable<TenantSyncBatchResults>>;
			this.FindTheRightCookie();
			ExProgressRecord exProgressRecord = new ExProgressRecord(1, new LocalizedString("Tenant Sync"), new LocalizedString("Sync Call"));
			ExProgressRecord exProgressRecord2 = new ExProgressRecord(2, new LocalizedString("Sync Call"), new LocalizedString("GetContext"));
			exProgressRecord2.ParentActivityId = exProgressRecord.ActivityId;
			for (int i = 0; i < this.SampleCountForStatistics; i++)
			{
				exProgressRecord.CurrentOperation = Strings.GetMsoDiagnosticsProgressIteration(i + 1, this.SampleCountForStatistics);
				exProgressRecord.PercentComplete = i * 100 / this.SampleCountForStatistics;
				exProgressRecord.RecordType = ProgressRecordType.Processing;
				base.WriteProgress(exProgressRecord);
				byte[] lastCookie = this.Cookie;
				byte[] lastPageToken = this.PageToken;
				string contextId = this.ExternalDirectoryOrganizationId;
				List<TenantSyncBatchResults> list2 = new List<TenantSyncBatchResults>();
				list.Add(list2);
				for (int j = 0; j < this.MaxNumberOfBatches; j++)
				{
					exProgressRecord2.CurrentOperation = Strings.GetMsoDiagnosticsProgressBatch(j + 1, this.MaxNumberOfBatches);
					exProgressRecord2.PercentComplete = j * 100 / this.MaxNumberOfBatches;
					exProgressRecord2.RecordType = ProgressRecordType.Processing;
					base.WriteProgress(exProgressRecord2);
					ExDateTime now = ExDateTime.Now;
					GetContextResponse context = this.MsoSyncService.SyncProxy.GetContext(new GetContextRequest(lastCookie, contextId, lastPageToken));
					TimeSpan responseTime = ExDateTime.Now - now;
					DirectoryObjectsAndLinks getContextResult = context.GetContextResult;
					if (getContextResult != null)
					{
						TenantSyncBatchResults tenantSyncBatchResults = new TenantSyncBatchResults(getContextResult);
						tenantSyncBatchResults.Stats.ResponseTime = responseTime;
						tenantSyncBatchResults.RawResponse = this.MsoSyncService.RawResponse;
						tenantSyncBatchResults.LastPageToken = lastPageToken;
						tenantSyncBatchResults.CalculateStats();
						list2.Add(tenantSyncBatchResults);
						contextId = null;
						lastCookie = null;
						lastPageToken = getContextResult.NextPageToken;
					}
					if (getContextResult == null || !getContextResult.More)
					{
						break;
					}
				}
				if (list2.Last<TenantSyncBatchResults>().More)
				{
					this.WriteWarning(Strings.GetMsoDiagnosticsMoreDataIsAvailable);
				}
				exProgressRecord2.RecordType = ProgressRecordType.Completed;
				base.WriteProgress(exProgressRecord2);
			}
			exProgressRecord.RecordType = ProgressRecordType.Completed;
			base.WriteProgress(exProgressRecord);
			tenantSyncSummary.CalculateStats();
			base.WriteObject(tenantSyncSummary);
		}

		private void ProcessEstimateBacklog()
		{
			BacklogSummary backlogSummary = new BacklogSummary();
			List<BacklogEstimateResults> list = backlogSummary.Batches as List<BacklogEstimateResults>;
			this.FindTheRightCookie();
			ExProgressRecord exProgressRecord = new ExProgressRecord(1, new LocalizedString("Estimate Backlog"), new LocalizedString("Calling EstimateBacklog API"));
			byte[] latestGetChangesCookie = this.Cookie;
			byte[] lastPageToken = this.PageToken;
			for (int i = 0; i < this.MaxNumberOfBatches; i++)
			{
				exProgressRecord.CurrentOperation = Strings.GetMsoDiagnosticsProgressBatch(i + 1, this.MaxNumberOfBatches);
				exProgressRecord.PercentComplete = i * 100 / this.MaxNumberOfBatches;
				exProgressRecord.RecordType = ProgressRecordType.Processing;
				base.WriteProgress(exProgressRecord);
				ExDateTime now = ExDateTime.Now;
				EstimateBacklogResponse estimateBacklogResponse = this.MsoSyncService.SyncProxy.EstimateBacklog(new EstimateBacklogRequest(latestGetChangesCookie, lastPageToken));
				TimeSpan responseTime = ExDateTime.Now - now;
				BacklogEstimateBatch estimateBacklogResult = estimateBacklogResponse.EstimateBacklogResult;
				if (estimateBacklogResult != null)
				{
					BacklogEstimateResults backlogEstimateResults = new BacklogEstimateResults(estimateBacklogResult);
					list.Add(backlogEstimateResults);
					backlogEstimateResults.ResponseTime = responseTime;
					backlogEstimateResults.RawResponse = this.MsoSyncService.RawResponse;
					latestGetChangesCookie = null;
					lastPageToken = estimateBacklogResult.NextPageToken;
				}
				if (estimateBacklogResult == null || estimateBacklogResult.StatusCode != 1)
				{
					break;
				}
			}
			if (list.Last<BacklogEstimateResults>().StatusCode == 1)
			{
				this.WriteWarning(Strings.GetMsoDiagnosticsMoreDataIsAvailable);
			}
			exProgressRecord.RecordType = ProgressRecordType.Completed;
			base.WriteProgress(exProgressRecord);
			backlogSummary.CalculateStats();
			base.WriteObject(backlogSummary);
		}

		private const string GetChangesParameterSet = "GetChanges";

		private const string GetContextParameterSet = "GetContext";

		private const string EstimateBacklogParameterSet = "EstimateBacklog";

		private const int DefaultSampleCountForStatistics = 3;

		private const int MinimumSampleCountForStatistics = 1;

		private const int MaximumSampleCountForStatistics = 10;

		private const int DefaultMaxNumberOfBatches = 3;

		private const int MinimumMaxNumberOfBatches = 1;

		private const int MaximumMaxNumberOfBatches = 10;

		private SyncService MsoSyncService;

		private enum ParameterNames
		{
			Cookie,
			DeltaSync,
			EstimateBacklog,
			MaxNumberOfBatches,
			OrganizationId,
			PageToken,
			SampleCountForStatistics,
			ServiceInstanceId,
			TenantSync,
			UseLastCommittedCookie
		}
	}
}
