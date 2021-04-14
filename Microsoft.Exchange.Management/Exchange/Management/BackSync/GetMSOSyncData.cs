using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;
using Microsoft.Exchange.Management.BackSync.Configuration;
using Microsoft.Exchange.Management.BackSync.Processors;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.BackSync
{
	[Cmdlet("Get", "MSOSyncData", DefaultParameterSetName = "IncrementalSyncParameterSet")]
	public sealed class GetMSOSyncData : GetTaskBase<ADRawEntry>
	{
		private IDataProcessor DataProcessor
		{
			get
			{
				if (this.dataProcessor == null)
				{
					this.dataProcessor = this.CreateDataProcessor();
				}
				return this.dataProcessor;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ObjectFullSyncInitialCallParameterSet")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "IncrementalSyncParameterSet")]
		[Parameter(Mandatory = true, ParameterSetName = "TenantFullSyncInitialCallParameterSet")]
		public byte[] Cookie
		{
			get
			{
				return (byte[])base.Fields["Cookie"];
			}
			set
			{
				base.Fields["Cookie"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ObjectFullSyncInitialCallParameterSet")]
		[Parameter(Mandatory = true, ParameterSetName = "ObjectFullSyncInitialCallFromMergeSyncParameterSet")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "ObjectFullSyncInitialCallFromTenantFullSyncParameterSet")]
		public SyncObjectId[] ObjectIds
		{
			get
			{
				return (SyncObjectId[])base.Fields["ObjectIds"];
			}
			set
			{
				base.Fields["ObjectIds"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TenantFullSyncInitialCallParameterSet")]
		[ValidateNotNullOrEmpty]
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

		[Parameter(Mandatory = false, ParameterSetName = "ObjectFullSyncInitialCallFromMergeSyncParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ObjectFullSyncInitialCallParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ObjectFullSyncInitialCallFromTenantFullSyncParameterSet")]
		public BackSyncOptions SyncOptions
		{
			get
			{
				return (BackSyncOptions)(base.Fields["SyncOptions"] ?? BackSyncOptions.None);
			}
			set
			{
				base.Fields["SyncOptions"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ObjectFullSyncSubsequentCallParameterSet")]
		[ValidateNotNullOrEmpty]
		public byte[] ObjectFullSyncPageToken
		{
			get
			{
				return (byte[])base.Fields["ObjectFullSyncPageToken"];
			}
			set
			{
				base.Fields["ObjectFullSyncPageToken"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "TenantFullSyncSubsequentCallParameterSet")]
		public byte[] TenantFullSyncPageToken
		{
			get
			{
				return (byte[])base.Fields["TenantFullSyncPageToken"];
			}
			set
			{
				base.Fields["TenantFullSyncPageToken"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ObjectFullSyncInitialCallFromTenantFullSyncParameterSet")]
		[ValidateNotNullOrEmpty]
		public byte[] TenantFullSyncPageTokenContext
		{
			get
			{
				return (byte[])base.Fields["TenantFullSyncPageTokenContext"];
			}
			set
			{
				base.Fields["TenantFullSyncPageTokenContext"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "MergeInitialCallParameterSet")]
		public byte[] MergeTenantFullSyncPageToken
		{
			get
			{
				return (byte[])base.Fields["MergeTenantFullSyncPageToken"];
			}
			set
			{
				base.Fields["MergeTenantFullSyncPageToken"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MergeInitialCallParameterSet")]
		[ValidateNotNullOrEmpty]
		public byte[] MergeIncrementalSyncCookie
		{
			get
			{
				return (byte[])base.Fields["MergeIncrementalSyncCookie"];
			}
			set
			{
				base.Fields["MergeIncrementalSyncCookie"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MergeSubsequentCallParameterSet")]
		[ValidateNotNullOrEmpty]
		public byte[] MergePageToken
		{
			get
			{
				return (byte[])base.Fields["MergePageToken"];
			}
			set
			{
				base.Fields["MergePageToken"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "ObjectFullSyncInitialCallFromMergeSyncParameterSet")]
		public byte[] MergePageTokenContext
		{
			get
			{
				return (byte[])base.Fields["MergePageTokenContext"];
			}
			set
			{
				base.Fields["MergePageTokenContext"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public ServiceInstanceId ServiceInstance
		{
			get
			{
				return (ServiceInstanceId)base.Fields["ServiceInstance"];
			}
			set
			{
				base.Fields["ServiceInstance"] = value;
			}
		}

		static GetMSOSyncData()
		{
			if (SyncConfiguration.EnableCloudPublicDelegatesRecipientFiltering())
			{
				GetMSOSyncData.PropertyFilterMap.Add(SyncUserSchema.CloudPublicDelegates, RecipientTypeDetails.UserMailbox);
			}
		}

		private ADObjectId GetRootOrgId()
		{
			return ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
		}

		private void ResolveOrganization(bool resolveRetiredTenant = false)
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(this.currentPartitionId), 338, "ResolveOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
			tenantConfigurationSession.SessionSettings.TenantConsistencyMode = (resolveRetiredTenant ? TenantConsistencyMode.IncludeRetiredTenants : TenantConsistencyMode.IgnoreRetiredTenants);
			string text = null;
			if (this.Organization != null)
			{
				OrganizationIdParameter organization = this.Organization;
				text = organization.ToString();
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Get organization from cmdlet Organization parameter. orgId {0}.", organization.RawIdentity);
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "orgIdString {0}", text);
				this.tenantFullSyncOrganizationCU = (ExchangeConfigurationUnit)base.GetDataObject<ExchangeConfigurationUnit>(organization, tenantConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(text)), new LocalizedString?(Strings.ErrorOrganizationNotUnique(text)));
			}
			else if (this.tenantFullSyncPageToken != null)
			{
				text = this.tenantFullSyncPageToken.TenantExternalDirectoryId.ToString();
				this.tenantFullSyncOrganizationCU = this.GetCUForExternalDirectoryOrganizationId(tenantConfigurationSession, text);
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Get organization from tenant token. orgId {0}.", text);
			}
			else if (this.mergePageToken != null)
			{
				text = this.mergePageToken.TenantExternalDirectoryId.ToString();
				this.tenantFullSyncOrganizationCU = this.GetCUForExternalDirectoryOrganizationId(tenantConfigurationSession, text);
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Get organization from merge page token. orgId {0}.", text);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "tenantFullSyncOrganizationCU {0}", this.tenantFullSyncOrganizationCU.DistinguishedName);
			if (GetMSOSyncData.msOnlineScope == null)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Find MSO scope {0} ...", "MSOnlinePartnerScope");
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 382, "ResolveOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
				ManagementScope managementScope = topologyConfigurationSession.ReadRootOrgManagementScopeByName("MSOnlinePartnerScope");
				if (managementScope == null)
				{
					ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Unable to find MSO scope {0}", "MSOnlinePartnerScope");
					base.WriteError(new ADExternalException(Strings.ErrorScopeNotFound("MSOnlinePartnerScope")), ErrorCategory.ObjectNotFound, "MSOnlinePartnerScope");
				}
				if (!ExchangeRunspaceConfiguration.TryStampQueryFilterOnManagementScope(managementScope))
				{
					ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "TryStampQueryFilterOnManagementScope failed");
					base.WriteError(new DataValidationException(new PropertyValidationError(Strings.ErrorAcceptedDomainExists(managementScope.Filter), ManagementScopeSchema.Filter, managementScope.Filter)), ErrorCategory.InvalidData, managementScope.Filter);
				}
				GetMSOSyncData.msOnlineScope = managementScope;
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "ResolveOrganization msOnlineScope {0}", GetMSOSyncData.msOnlineScope.DistinguishedName);
			}
			TenantOrganizationPresentationObject obj = new TenantOrganizationPresentationObject(this.tenantFullSyncOrganizationCU);
			if (!OpathFilterEvaluator.FilterMatches(GetMSOSyncData.msOnlineScope.QueryFilter, obj))
			{
				ADObjectId adobjectId;
				string text2 = base.TryGetExecutingUserId(out adobjectId) ? adobjectId.ToCanonicalName() : base.ExecutingUserIdentityName;
				ExTraceGlobals.BackSyncTracer.TraceError<string, string>((long)SyncConfiguration.TraceId, "User {0} has no access to orgId {1}", text2, text);
				base.WriteError(new ADScopeException(Strings.ErrorOrgOutOfPartnerScope(text2, text)), (ErrorCategory)1004, this.tenantFullSyncOrganizationCU);
			}
		}

		protected override void InternalBeginProcessing()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "GetMSOSyncData.InternalBeginProcessing entering");
			this.dirSyncBasedTenantFullSyncThreshold = SyncConfiguration.DirSyncBasedTenantFullSyncThreshold();
			base.InternalBeginProcessing();
			this.performanceCounterSession = this.CreatePerformanceCounterSession();
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Created performance counter session {0}", this.performanceCounterSession.GetType().Name);
			this.performanceCounterSession.Initialize();
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Initialized performance counter session");
			try
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Process input cookie based on parameter set {0}", base.ParameterSetName.ToString());
				this.currentPartitionId = GetMSOSyncData.GetPartitionIdFromServiceInstance(this.ServiceInstance);
				if (null == this.currentPartitionId)
				{
					base.WriteError(new CannotResolvePartitionFromInstanceIdException(this.ServiceInstance.ToString()), ErrorCategory.InvalidArgument, null);
				}
				string parameterSetName;
				if ((parameterSetName = base.ParameterSetName) != null)
				{
					if (<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6000546-1 == null)
					{
						<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6000546-1 = new Dictionary<string, int>(9)
						{
							{
								"IncrementalSyncParameterSet",
								0
							},
							{
								"ObjectFullSyncInitialCallParameterSet",
								1
							},
							{
								"ObjectFullSyncInitialCallFromTenantFullSyncParameterSet",
								2
							},
							{
								"ObjectFullSyncInitialCallFromMergeSyncParameterSet",
								3
							},
							{
								"ObjectFullSyncSubsequentCallParameterSet",
								4
							},
							{
								"TenantFullSyncInitialCallParameterSet",
								5
							},
							{
								"TenantFullSyncSubsequentCallParameterSet",
								6
							},
							{
								"MergeInitialCallParameterSet",
								7
							},
							{
								"MergeSubsequentCallParameterSet",
								8
							}
						};
					}
					int num;
					if (<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6000546-1.TryGetValue(parameterSetName, out num))
					{
						switch (num)
						{
						case 0:
							this.syncCookie = ((this.Cookie == null) ? new BackSyncCookie(this.ServiceInstance) : BackSyncCookie.Parse(this.Cookie));
							this.ValidateServiceInstance(this.syncCookie.ServiceInstanceId);
							this.invocationId = this.syncCookie.InvocationId;
							break;
						case 1:
						{
							BackSyncCookie backSyncCookie = BackSyncCookie.Parse(this.Cookie);
							this.ValidateServiceInstance(backSyncCookie.ServiceInstanceId);
							this.objectFullSyncPageToken = new ObjectFullSyncPageToken(backSyncCookie.InvocationId, this.ObjectIds, this.SyncOptions, backSyncCookie.ServiceInstanceId);
							this.invocationId = this.objectFullSyncPageToken.InvocationId;
							break;
						}
						case 2:
						{
							TenantFullSyncPageToken tenantFullSyncPageToken = Microsoft.Exchange.Data.Directory.Sync.TenantFullSyncPageToken.Parse(this.TenantFullSyncPageTokenContext);
							this.ValidateServiceInstance(tenantFullSyncPageToken.ServiceInstanceId);
							this.objectFullSyncPageToken = new ObjectFullSyncPageToken(tenantFullSyncPageToken.InvocationId, this.ObjectIds, this.SyncOptions, tenantFullSyncPageToken.ServiceInstanceId);
							this.invocationId = this.objectFullSyncPageToken.InvocationId;
							break;
						}
						case 3:
						{
							MergePageToken mergePageToken = Microsoft.Exchange.Data.Directory.Sync.MergePageToken.Parse(this.MergePageTokenContext);
							this.ValidateServiceInstance(mergePageToken.ServiceInstanceId);
							this.objectFullSyncPageToken = new ObjectFullSyncPageToken(mergePageToken.InvocationId, this.ObjectIds, this.SyncOptions, mergePageToken.ServiceInstanceId);
							this.invocationId = this.objectFullSyncPageToken.InvocationId;
							break;
						}
						case 4:
							this.objectFullSyncPageToken = Microsoft.Exchange.Data.Directory.Sync.ObjectFullSyncPageToken.Parse(this.ObjectFullSyncPageToken);
							this.ValidateServiceInstance(this.objectFullSyncPageToken.ServiceInstanceId);
							this.invocationId = this.objectFullSyncPageToken.InvocationId;
							break;
						case 5:
						{
							this.ResolveOrganization(false);
							BackSyncCookie backSyncCookie = BackSyncCookie.Parse(this.Cookie);
							this.ValidateServiceInstance(backSyncCookie.ServiceInstanceId);
							this.tenantFullSyncPageToken = new TenantFullSyncPageToken(SyncConfiguration.EnableIgnoreCookieDCDuringTenantFaultin() ? Guid.Empty : backSyncCookie.InvocationId, new Guid(this.tenantFullSyncOrganizationCU.ExternalDirectoryOrganizationId), this.tenantFullSyncOrganizationCU.OrganizationalUnitLink, backSyncCookie.ServiceInstanceId, this.ShouldUseDirSyncBasedTenantFullSync());
							this.invocationId = this.tenantFullSyncPageToken.InvocationId;
							if (!this.ShouldUseDirSyncBasedTenantFullSync() && this.invocationId == Guid.Empty)
							{
								this.invocationId = this.tenantFullSyncPageToken.SelectDomainController(this.currentPartitionId);
							}
							break;
						}
						case 6:
							this.tenantFullSyncPageToken = Microsoft.Exchange.Data.Directory.Sync.TenantFullSyncPageToken.Parse(this.TenantFullSyncPageToken);
							this.ValidateServiceInstance(this.tenantFullSyncPageToken.ServiceInstanceId);
							this.ResolveOrganization(false);
							this.invocationId = this.tenantFullSyncPageToken.InvocationId;
							if (this.invocationId == Guid.Empty)
							{
								this.invocationId = this.tenantFullSyncPageToken.SelectDomainController(this.currentPartitionId);
							}
							break;
						case 7:
							this.mergePageToken = new MergePageToken(this.MergeTenantFullSyncPageToken, this.MergeIncrementalSyncCookie);
							this.ValidateServiceInstance(this.mergePageToken.ServiceInstanceId);
							this.ResolveOrganization(true);
							this.invocationId = this.mergePageToken.InvocationId;
							break;
						case 8:
							this.mergePageToken = Microsoft.Exchange.Data.Directory.Sync.MergePageToken.Parse(this.MergePageToken);
							this.ValidateServiceInstance(this.mergePageToken.ServiceInstanceId);
							this.ResolveOrganization(true);
							this.invocationId = this.mergePageToken.InvocationId;
							if (this.invocationId == Guid.Empty)
							{
								this.invocationId = this.mergePageToken.SelectDomainController(this.currentPartitionId);
							}
							break;
						default:
							goto IL_4EB;
						}
						ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "this.invocationId = {0}", this.invocationId);
						return;
					}
				}
				IL_4EB:
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Not supported parameter set {0}", base.ParameterSetName);
				throw new NotSupportedException("not supported parameter set " + base.ParameterSetName);
			}
			catch
			{
				ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "GetMSOSyncData.InternalBeginProcessing encountered exception");
				this.performanceCounterSession.IncrementUserError();
				throw;
			}
		}

		protected override void InternalStateReset()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "GetMSOSyncData.InternalStateReset entering");
			this.syncConfiguration = this.CreateSyncConfiguration();
			try
			{
				base.InternalStateReset();
			}
			catch (Exception ex)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "GetMSOSyncData.InternalStateReset exception {0}", ex.ToString());
				this.performanceCounterSession.IncrementSystemError();
				Exception ex2 = this.HandleException(ex);
				if (ex2 == ex)
				{
					throw;
				}
				throw ex2;
			}
			finally
			{
				this.ProcessPerformanceCounters();
			}
		}

		protected override void InternalProcessRecord()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "GetMSOSyncData.InternalProcessRecord entering");
			try
			{
				base.InternalProcessRecord();
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "GetMSOSyncData.InternalProcessRecord flush ...");
				this.DataProcessor.Flush(new Func<byte[]>(this.syncConfiguration.GetResultCookie), this.syncConfiguration.MoreData);
			}
			catch (Exception ex)
			{
				this.performanceCounterSession.IncrementSystemError();
				Exception ex2 = this.HandleException(ex);
				if (ex2 == ex)
				{
					throw;
				}
				throw ex2;
			}
			finally
			{
				this.ProcessPerformanceCounters();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is InvalidCookieException || exception is InvalidCookieServiceInstanceIdException || exception is BackSyncDataSourceTransientException || exception is BackSyncDataSourceUnavailableException || exception is BackSyncDataSourceReplicationException || (this.syncConfiguration != null && this.syncConfiguration.IsKnownException(exception));
		}

		protected override IConfigDataProvider CreateSession()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Create IConfigurationSession and IRecipientSession ...");
			string text = string.Empty;
			if (this.invocationId != Guid.Empty)
			{
				text = SyncConfiguration.FindDomainControllerByInvocationId(this.invocationId, this.currentPartitionId);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "CreateSession domainController {0}", text);
			ITopologyConfigurationSession rootOrgConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(text, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.currentPartitionId), 653, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
			ADSessionSettings adsessionSettings = ADSessionSettings.SessionSettingsFactory.Default.FromAllTenantsPartitionId(this.currentPartitionId);
			adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			ITenantConfigurationSession tenantSystemConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(text, true, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 663, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
			ADSessionSettings adsessionSettings2 = (this.tenantFullSyncOrganizationCU == null) ? ADSessionSettings.FromAllTenantsPartitionId(this.currentPartitionId) : ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.GetRootOrgId(), this.tenantFullSyncOrganizationCU.OrganizationId, base.ExecutingUserOrganizationId, false, false);
			adsessionSettings2.IncludeSoftDeletedObjects = true;
			adsessionSettings2.ServerSettings.RecipientViewRoot = null;
			adsessionSettings2.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.PartiallyConsistent, adsessionSettings2, 687, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
			tenantRecipientSession.UseGlobalCatalog = false;
			tenantRecipientSession.DomainController = text;
			tenantRecipientSession.LogSizeLimitExceededEvent = false;
			this.syncConfiguration.SetConfiguration(rootOrgConfigurationSession, tenantSystemConfigurationSession, tenantRecipientSession);
			return tenantRecipientSession;
		}

		protected override IEnumerable<ADRawEntry> GetPagedData()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Get {0} data page", this.syncConfiguration.GetType().Name);
			return this.syncConfiguration.GetDataPage();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ADRawEntry adrawEntry = (ADRawEntry)dataObject;
			ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "GetMSOSyncData:: - Start processing object {0}.", adrawEntry.Id);
			PropertyBag propertyBag = adrawEntry.propertyBag;
			ProcessorHelper.TracePropertBag("GetMSOSyncData::WriteResult", propertyBag);
			this.DataProcessor.Process(propertyBag);
		}

		private ExchangeConfigurationUnit GetCUForExternalDirectoryOrganizationId(ITenantConfigurationSession tenantConfigSession, string externalDirectoryOrganizationId)
		{
			ExchangeConfigurationUnit exchangeConfigurationUnitByExternalId = tenantConfigSession.GetExchangeConfigurationUnitByExternalId(externalDirectoryOrganizationId);
			if (exchangeConfigurationUnitByExternalId == null)
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(externalDirectoryOrganizationId));
			}
			return exchangeConfigurationUnitByExternalId;
		}

		private Exception HandleException(Exception exception)
		{
			Exception ex = this.syncConfiguration.HandleException(exception);
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_BackSyncExceptionCaught, new string[]
			{
				this.syncConfigurationMode,
				(ex == null) ? string.Empty : ex.ToString()
			});
			return ex;
		}

		private SyncConfiguration CreateSyncConfiguration()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "GetMSOSyncData.CreateSyncConfiguration entering");
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetMSOSyncData.CreateSyncConfiguration ParameterSetName {0}", base.ParameterSetName);
			string parameterSetName;
			switch (parameterSetName = base.ParameterSetName)
			{
			case "IncrementalSyncParameterSet":
				this.syncConfigurationMode = "IncrementalSync";
				return new IncrementalSyncConfiguration(this.syncCookie, this.invocationId, new OutputResultDelegate(this.WriteCookieAndResponse), new GetMSOSyncData.SyncEventLogger(), new ExcludedObjectReporter());
			case "ObjectFullSyncInitialCallParameterSet":
			case "ObjectFullSyncInitialCallFromTenantFullSyncParameterSet":
			case "ObjectFullSyncInitialCallFromMergeSyncParameterSet":
			case "ObjectFullSyncSubsequentCallParameterSet":
				this.syncConfigurationMode = "ObjectFullSync";
				return new ObjectFullSyncConfiguration(this.objectFullSyncPageToken, this.invocationId, new OutputResultDelegate(this.WriteCookieAndResponse), new GetMSOSyncData.SyncEventLogger(), new FullSyncObjectErrorReporter(this.performanceCounterSession));
			case "TenantFullSyncInitialCallParameterSet":
			case "TenantFullSyncSubsequentCallParameterSet":
				this.syncConfigurationMode = "TenantFullSync";
				if ((base.ParameterSetName == "TenantFullSyncInitialCallParameterSet" && this.ShouldUseDirSyncBasedTenantFullSync()) || (base.ParameterSetName == "TenantFullSyncSubsequentCallParameterSet" && this.tenantFullSyncPageToken.TenantScopedBackSyncCookie != null))
				{
					return new DirSyncBasedTenantFullSyncConfiguration(this.tenantFullSyncPageToken, this.tenantFullSyncOrganizationCU, this.invocationId, new OutputResultDelegate(this.WriteCookieAndResponse), new GetMSOSyncData.SyncEventLogger(), new VerboseObjectErrorReporter(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)));
				}
				return new TenantFullSyncConfiguration(this.tenantFullSyncPageToken, this.invocationId, new OutputResultDelegate(this.WriteCookieAndResponse), new GetMSOSyncData.SyncEventLogger(), new VerboseObjectErrorReporter(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)));
			case "MergeInitialCallParameterSet":
			case "MergeSubsequentCallParameterSet":
				this.syncConfigurationMode = "Merge";
				if (this.mergePageToken.TenantScopedBackSyncCookie != null)
				{
					return new DirSyncBasedMergeConfiguration(this.mergePageToken, this.tenantFullSyncOrganizationCU, this.invocationId, new OutputResultDelegate(this.WriteCookieAndResponse), new GetMSOSyncData.SyncEventLogger(), new VerboseObjectErrorReporter(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)), this.currentPartitionId);
				}
				return new MergeConfiguration(this.mergePageToken, this.invocationId, new OutputResultDelegate(this.WriteCookieAndResponse), new GetMSOSyncData.SyncEventLogger(), new VerboseObjectErrorReporter(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)), this.currentPartitionId);
			}
			throw new NotSupportedException("not supported parameter set " + base.ParameterSetName);
		}

		private IDataProcessor CreateDataProcessor()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "GetMSOSyncData.CreateDataProcessor entering");
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetMSOSyncData.CreateDataProcessor ParameterSetName {0}", base.ParameterSetName);
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6000550-1 == null)
				{
					<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6000550-1 = new Dictionary<string, int>(9)
					{
						{
							"IncrementalSyncParameterSet",
							0
						},
						{
							"ObjectFullSyncInitialCallParameterSet",
							1
						},
						{
							"ObjectFullSyncInitialCallFromTenantFullSyncParameterSet",
							2
						},
						{
							"ObjectFullSyncInitialCallFromMergeSyncParameterSet",
							3
						},
						{
							"ObjectFullSyncSubsequentCallParameterSet",
							4
						},
						{
							"TenantFullSyncInitialCallParameterSet",
							5
						},
						{
							"TenantFullSyncSubsequentCallParameterSet",
							6
						},
						{
							"MergeInitialCallParameterSet",
							7
						},
						{
							"MergeSubsequentCallParameterSet",
							8
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6000550-1.TryGetValue(parameterSetName, out num))
				{
					IDataProcessor dataProcessor;
					switch (num)
					{
					case 0:
						dataProcessor = this.CreateIncrementalSyncDataProcessor(false);
						break;
					case 1:
					case 2:
					case 3:
					case 4:
						dataProcessor = this.CreateObjectFullSyncDataProcessor();
						break;
					case 5:
						dataProcessor = (this.ShouldUseDirSyncBasedTenantFullSync() ? this.CreateIncrementalSyncDataProcessor(true) : this.CreateFullSyncDataProcessor(null));
						break;
					case 6:
						dataProcessor = ((this.tenantFullSyncPageToken.TenantScopedBackSyncCookie != null) ? this.CreateIncrementalSyncDataProcessor(true) : this.CreateFullSyncDataProcessor(null));
						break;
					case 7:
					case 8:
						dataProcessor = ((this.mergePageToken.TenantScopedBackSyncCookie != null) ? this.CreateIncrementalSyncDataProcessor(true) : this.CreateFullSyncDataProcessor(null));
						break;
					default:
						goto IL_170;
					}
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetMSOSyncData: Data processor has been created {0}", dataProcessor.GetType().Name);
					return dataProcessor;
				}
			}
			IL_170:
			throw new NotSupportedException("not supported parameter set " + base.ParameterSetName);
		}

		private PerformanceCounterSession CreatePerformanceCounterSession()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Create performance counter session for parameter set {0}", base.ParameterSetName);
			string parameterSetName;
			switch (parameterSetName = base.ParameterSetName)
			{
			case "IncrementalSyncParameterSet":
				return new IncrementalSyncPerformanceCounterSession(GetMSOSyncData.enablePerformanceCounters);
			case "ObjectFullSyncInitialCallParameterSet":
			case "ObjectFullSyncSubsequentCallParameterSet":
			case "ObjectFullSyncInitialCallFromTenantFullSyncParameterSet":
			case "ObjectFullSyncInitialCallFromMergeSyncParameterSet":
				return new ObjectFullSyncPerformanceCounterSession(GetMSOSyncData.enablePerformanceCounters);
			case "TenantFullSyncInitialCallParameterSet":
			case "TenantFullSyncSubsequentCallParameterSet":
			case "MergeInitialCallParameterSet":
			case "MergeSubsequentCallParameterSet":
				return new TenantFullSyncPerformanceCounterSession(GetMSOSyncData.enablePerformanceCounters);
			}
			throw new NotSupportedException("not supported parameter set " + base.ParameterSetName);
		}

		private void ProcessPerformanceCounters()
		{
			if (base.ParameterSetName == "IncrementalSyncParameterSet")
			{
				bool sameCookie = false;
				object obj;
				if (base.SessionState.Variables.TryGetValue("BackSyncLastCookie", out obj))
				{
					sameCookie = this.IsCookieParameterSameAs(obj as byte[]);
				}
				base.SessionState.Variables["BackSyncLastCookie"] = this.Cookie;
				this.performanceCounterSession.ReportSameCookie(sameCookie);
			}
			this.performanceCounterSession.Finish();
		}

		private bool IsCookieParameterSameAs(byte[] cookie)
		{
			if (this.Cookie == null || this.Cookie.Length == 0 || cookie == null)
			{
				return false;
			}
			if (this.Cookie.Length != cookie.Length)
			{
				return false;
			}
			for (int i = 0; i < this.Cookie.Length; i++)
			{
				if (this.Cookie[i] != cookie[i])
				{
					return false;
				}
			}
			return true;
		}

		private void WriteCookieAndResponse(byte[] serializedCookie, object response)
		{
			this.WriteResultWithAuditLog(new SyncData(serializedCookie, response));
		}

		private void WriteResultWithAuditLog(IConfigurable dataObject)
		{
			base.WriteResult(dataObject);
			if (BackSyncAuditLog.IsEnabled)
			{
				BackSyncAuditLog.Instance.Append(base.ExecutingUserIdentityName, this.Cookie, this.GetParametersExceptCookie(), ((SyncData)dataObject).Response, this.syncConfiguration.ErrorSyncObjects);
			}
		}

		private NameValueCollection GetParametersExceptCookie()
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			foreach (object obj in base.Fields.Keys)
			{
				string text = (string)obj;
				if (text != "Cookie")
				{
					object obj2 = base.Fields[text];
					string a;
					if ((a = text) != null)
					{
						if (!(a == "SyncOptions"))
						{
							if (a == "ObjectIds")
							{
								SyncObjectId[] array = obj2 as SyncObjectId[];
								if (array != null)
								{
									foreach (SyncObjectId syncObjectId in array)
									{
										nameValueCollection.Add("ObjectIds", syncObjectId.ToString());
									}
									continue;
								}
								continue;
							}
						}
						else
						{
							BackSyncOptions backSyncOptions = (BackSyncOptions)obj2;
							if (backSyncOptions != BackSyncOptions.None)
							{
								nameValueCollection.Add("SyncOptions", backSyncOptions.ToString());
								continue;
							}
							continue;
						}
					}
					if (obj2 is byte[])
					{
						nameValueCollection.Add(text, Convert.ToBase64String((byte[])obj2));
					}
					else
					{
						nameValueCollection.Add(text, obj2.ToString());
					}
				}
			}
			return nameValueCollection;
		}

		private IDataProcessor CreateFullSyncDataProcessor(OrganizationLookup organizationLookup = null)
		{
			FullSyncConfiguration config = (FullSyncConfiguration)this.syncConfiguration;
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "CreateFullSyncDataProcessor");
			ExcludedObjectReporter reporter = (ExcludedObjectReporter)config.ExcludedObjectReporter;
			LinkTargetPropertyLookup linkTargetPropertyLookup = new LinkTargetPropertyLookup(new Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]>(config.GetProperties));
			PagedOutputResultWriter pagedOutputResultWriter = new PagedOutputResultWriter(new WriteResultDelegate(this.WriteResultWithAuditLog), (IEnumerable<SyncObject> objects, bool moreData, byte[] cookie, ServiceInstanceId serviceInstance) => SyncObject.CreateGetDirectoryObjectsResponse(objects, moreData, cookie, config.GetReportedErrors(), serviceInstance), null, new AddErrorSyncObjectDelegate(config.AddErrorSyncObject), this.ServiceInstance);
			IDataProcessor next = pagedOutputResultWriter;
			if (GetMSOSyncData.IsTenantRelocationSupportEnabled())
			{
				if (organizationLookup == null)
				{
					QueryFilter managementScopeFilter = GetMSOSyncData.GetManagementScopeFilter();
					organizationLookup = new OrganizationLookup(new Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]>(config.GetOrganizationProperties), managementScopeFilter);
				}
				next = new TenantRelocationProcessor(next, organizationLookup, reporter, new GetTenantRelocationStateDelegate(GetMSOSyncData.GetTenantRelocationState), config.InvocationId, false);
			}
			NeverSetAttributesFilter next2 = new NeverSetAttributesFilter(next);
			PropertyReferenceTargetMissingPropertyResolver next3 = new PropertyReferenceTargetMissingPropertyResolver(next2, linkTargetPropertyLookup);
			LinkTargetMissingPropertyResolver next4 = new LinkTargetMissingPropertyResolver(next3, linkTargetPropertyLookup);
			BatchLookup next5 = new BatchLookup(next4, linkTargetPropertyLookup);
			RecipientTypeSpecificPropertyFilter recipientTypeSpecificPropertyFilter = new RecipientTypeSpecificPropertyFilter(next5, GetMSOSyncData.PropertyFilterMap);
			next = recipientTypeSpecificPropertyFilter;
			if (SyncConfiguration.InlcudeLinks(config.FullSyncPageToken.SyncOptions))
			{
				next = new Metadata2LinkTranslator(recipientTypeSpecificPropertyFilter);
			}
			RecipientTypeDetails acceptedRecipientTypes = RecipientTaskHelper.GetAcceptedRecipientTypes();
			return new RecipientTypeFilter(next, acceptedRecipientTypes, reporter);
		}

		private static TenantRelocationState GetTenantRelocationState(ADObjectId tenantOUId, out bool isSourceTenant, bool readThrough)
		{
			TenantRelocationState tenantRelocationState = TenantRelocationStateCache.GetTenantRelocationState(tenantOUId.Name, tenantOUId.GetPartitionId(), out isSourceTenant, readThrough);
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "GetTenantRelocationState:: - {0} relocation state for tenant {1} is: isSource={2}, sourceForest={3}, sourceState={4}, targetForest={5}, targetState={6}.", new object[]
			{
				readThrough ? "Latest" : "Cached",
				tenantOUId,
				isSourceTenant,
				tenantRelocationState.SourceForestFQDN,
				tenantRelocationState.SourceForestState,
				tenantRelocationState.TargetForestFQDN,
				tenantRelocationState.TargetForestState
			});
			return tenantRelocationState;
		}

		private IDataProcessor CreateObjectFullSyncDataProcessor()
		{
			ObjectFullSyncConfiguration objectFullSyncConfiguration = (ObjectFullSyncConfiguration)this.syncConfiguration;
			QueryFilter managementScopeFilter = GetMSOSyncData.GetManagementScopeFilter();
			OrganizationLookup organizationLookup = new OrganizationLookup(new Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]>(objectFullSyncConfiguration.GetOrganizationProperties), managementScopeFilter);
			RecipientTypeDetails acceptedRecipientTypes = RecipientTaskHelper.GetAcceptedRecipientTypes();
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "CreateObjectFullSyncDataProcessor acceptedRecipientTypes = {0}", acceptedRecipientTypes.ToString());
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "CreateObjectFullSyncDataProcessor orgQueryFilter = {0}", (managementScopeFilter != null) ? managementScopeFilter.ToString() : "NULL");
			IDataProcessor dataProcessor = this.CreateFullSyncDataProcessor(organizationLookup);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.CreateDataProcessor baseProcessor {0}", dataProcessor.GetType().Name);
			ExcludedObjectReporter reporter = (ExcludedObjectReporter)objectFullSyncConfiguration.ExcludedObjectReporter;
			RecipientDeletedDuringOrganizationDeletionFilter next = new RecipientDeletedDuringOrganizationDeletionFilter(dataProcessor, organizationLookup, reporter);
			OrganizationFilter next2 = new OrganizationFilter(next, organizationLookup, reporter, false);
			return new BatchLookup(next2, organizationLookup);
		}

		private IDataProcessor CreateIncrementalSyncDataProcessor(bool isDirSyncBasedTenantFullSync = false)
		{
			IncrementalSyncConfiguration incrementalSyncConfiguration = (IncrementalSyncConfiguration)this.syncConfiguration;
			QueryFilter managementScopeFilter = GetMSOSyncData.GetManagementScopeFilter();
			RecipientTypeDetails acceptedRecipientTypes = RecipientTaskHelper.GetAcceptedRecipientTypes();
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "CreateIncrementalSyncDataProcessor acceptedRecipientTypes = {0}", acceptedRecipientTypes.ToString());
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "CreateIncrementalSyncDataProcessor orgQueryFilter = {0}", (managementScopeFilter != null) ? managementScopeFilter.ToString() : "NULL");
			Dictionary<ADObjectId, ADRawEntry> propertyCache = new Dictionary<ADObjectId, ADRawEntry>();
			ObjectPropertyLookup objectPropertyLookup = new ObjectPropertyLookup(new Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]>(incrementalSyncConfiguration.GetProperties), propertyCache);
			LinkTargetPropertyLookup linkTargetPropertyLookup = new LinkTargetPropertyLookup(new Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]>(incrementalSyncConfiguration.GetProperties), propertyCache);
			OrganizationLookup organizationLookup = new OrganizationLookup(new Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]>(incrementalSyncConfiguration.GetOrganizationProperties), managementScopeFilter);
			ExcludedObjectReporter reporter = (ExcludedObjectReporter)incrementalSyncConfiguration.ExcludedObjectReporter;
			PagedOutputResultWriter pagedOutputResultWriter;
			if (isDirSyncBasedTenantFullSync)
			{
				pagedOutputResultWriter = new PagedOutputResultWriter(new WriteResultDelegate(this.WriteResultWithAuditLog), (IEnumerable<SyncObject> objects, bool moreData, byte[] cookie, ServiceInstanceId serviceInstance) => SyncObject.CreateGetDirectoryObjectsResponse(objects, moreData, cookie, new DirectoryObjectError[0], serviceInstance), new Action<int>(this.performanceCounterSession.ReportChangeCount), new AddErrorSyncObjectDelegate(incrementalSyncConfiguration.AddErrorSyncObject), this.ServiceInstance);
			}
			else
			{
				pagedOutputResultWriter = new PagedOutputResultWriter(new WriteResultDelegate(this.WriteResultWithAuditLog), new Func<IEnumerable<SyncObject>, bool, byte[], ServiceInstanceId, object>(SyncObject.CreateGetChangesResponse), new Action<int>(this.performanceCounterSession.ReportChangeCount), new AddErrorSyncObjectDelegate(incrementalSyncConfiguration.AddErrorSyncObject), this.ServiceInstance);
			}
			IDataProcessor next = pagedOutputResultWriter;
			bool flag = GetMSOSyncData.IsTenantRelocationSupportEnabled();
			if (flag)
			{
				next = new TenantRelocationProcessor(next, organizationLookup, reporter, new GetTenantRelocationStateDelegate(GetMSOSyncData.GetTenantRelocationState), incrementalSyncConfiguration.InvocationId, true);
			}
			PropertyReferenceTargetMissingPropertyResolver next2 = new PropertyReferenceTargetMissingPropertyResolver(next, linkTargetPropertyLookup);
			LinkTargetMissingPropertyResolver next3 = new LinkTargetMissingPropertyResolver(next2, linkTargetPropertyLookup);
			BatchLookup next4 = new BatchLookup(next3, linkTargetPropertyLookup);
			RecipientTypeSpecificPropertyFilter next5 = new RecipientTypeSpecificPropertyFilter(next4, GetMSOSyncData.PropertyFilterMap);
			RecipientTypeFilter next6 = new RecipientTypeFilter(next5, acceptedRecipientTypes, reporter);
			RecipientDeletedDuringOrganizationDeletionFilter next7 = new RecipientDeletedDuringOrganizationDeletionFilter(next6, organizationLookup, reporter);
			MissingPropertyResolver missingPropertyResolver = new MissingPropertyResolver(next7, objectPropertyLookup);
			incrementalSyncConfiguration.MissingPropertyResolver = missingPropertyResolver;
			BatchLookup next8 = new BatchLookup(missingPropertyResolver, objectPropertyLookup);
			OrganizationFilter next9 = new OrganizationFilter(next8, organizationLookup, reporter, flag);
			BatchLookup next10 = new BatchLookup(next9, organizationLookup);
			return new IncludedBackIntoBacksyncDetector(next10, this.ServiceInstance);
		}

		private static QueryFilter GetManagementScopeFilter()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1211, "GetManagementScopeFilter", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
			string text = "MSOnlinePartnerScope";
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetManagementScopeFilter partnerScopeName {0}", text);
			QueryFilter queryFilter = null;
			ManagementScope managementScope = topologyConfigurationSession.ReadRootOrgManagementScopeByName(text);
			if (managementScope == null)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Scope not found for name {0}", text);
				throw new ADExternalException(Strings.ErrorScopeNotFound(text));
			}
			ScopeRestrictionType scopeRestrictionType = managementScope.ScopeRestrictionType;
			string arg;
			RBACHelper.TryConvertPowershellFilterIntoQueryFilter(managementScope.Filter, scopeRestrictionType, null, out queryFilter, out arg);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetManagementScopeFilter orgQueryFilter {0}", (queryFilter != null) ? queryFilter.ToString() : "NULL");
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetManagementScopeFilter errorString {0}", arg);
			return queryFilter;
		}

		private void ValidateServiceInstance(ServiceInstanceId cookieServiceInstanceId)
		{
			if (!this.ServiceInstance.Equals(cookieServiceInstanceId))
			{
				ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "Cookie ServiceInstanceId and parameter ServiceInstanceId are different");
				base.WriteError(new CookieAndParameterServiceInstanceIdMismatchException(cookieServiceInstanceId.ToString(), this.ServiceInstance.ToString()), ErrorCategory.InvalidArgument, cookieServiceInstanceId);
			}
		}

		private bool ShouldUseDirSyncBasedTenantFullSync()
		{
			if (base.ParameterSetName != "TenantFullSyncInitialCallParameterSet")
			{
				ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "this.ParameterSetName != TenantFullSyncInitialCallParameterSet");
				throw new InvalidOperationException("ParameterSetName");
			}
			if (this.dirSyncBasedTenantFullSyncThreshold < 0L)
			{
				return false;
			}
			if (this.currentTenantSize < 0L)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, this.tenantFullSyncOrganizationCU.OrganizationId, base.ExecutingUserOrganizationId, false);
				IConfigurationSession configSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 1282, "ShouldUseDirSyncBasedTenantFullSync", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
				this.currentTenantSize = GetMSOSyncData.GetTenantSize(configSession, this.tenantFullSyncOrganizationCU.OrganizationId);
				ExTraceGlobals.BackSyncTracer.TraceDebug<OrganizationId, long>((long)SyncConfiguration.TraceId, "ShouldUseDirSyncBasedTenantFullSync: Organization: {0} Size: {1}.", this.tenantFullSyncOrganizationCU.OrganizationId, this.currentTenantSize);
			}
			return this.currentTenantSize > this.dirSyncBasedTenantFullSyncThreshold;
		}

		private static PartitionId GetPartitionIdFromServiceInstance(ServiceInstanceId serviceInstance)
		{
			PartitionId partitionId = null;
			Exception ex;
			if (PartitionId.TryParse(ServiceInstanceId.GetShortName(serviceInstance.InstanceId), out partitionId, out ex))
			{
				partitionId = (ADAccountPartitionLocator.IsKnownPartition(partitionId) ? partitionId : null);
			}
			if (null == partitionId)
			{
				string serviceInstanceId = ServiceInstanceId.GetServiceInstanceId(TopologyProvider.LocalForestFqdn);
				if (serviceInstanceId.Equals(serviceInstance.InstanceId, StringComparison.InvariantCultureIgnoreCase))
				{
					partitionId = PartitionId.LocalForest;
					partitionId = (ADAccountPartitionLocator.IsKnownPartition(partitionId) ? partitionId : null);
				}
			}
			if (null == partitionId)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1334, "GetPartitionIdFromServiceInstance", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
				topologyConfigurationSession.UseConfigNC = false;
				ITopologyConfigurationSession topologyConfigurationSession2 = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1338, "GetPartitionIdFromServiceInstance", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\BackSync\\GetMSOSyncData.cs");
				ServiceInstanceIdParameter serviceInstanceIdParameter = new ServiceInstanceIdParameter(serviceInstance);
				IEnumerable<SyncServiceInstance> objects = serviceInstanceIdParameter.GetObjects<SyncServiceInstance>(SyncServiceInstance.GetMsoSyncRootContainer(), topologyConfigurationSession);
				using (IEnumerator<SyncServiceInstance> enumerator = objects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						SyncServiceInstance syncServiceInstance = enumerator.Current;
						if (enumerator.MoveNext())
						{
							throw new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(serviceInstanceIdParameter.ToString()));
						}
						if (syncServiceInstance != null && !ADObjectId.IsNullOrEmpty(syncServiceInstance.AccountPartition))
						{
							ADObjectId accountPartition = syncServiceInstance.AccountPartition;
							AccountPartition accountPartition2 = topologyConfigurationSession2.Read<AccountPartition>(accountPartition);
							if (accountPartition2 != null)
							{
								partitionId = accountPartition2.PartitionId;
								partitionId = (ADAccountPartitionLocator.IsKnownPartition(partitionId) ? partitionId : null);
							}
						}
					}
				}
			}
			return partitionId;
		}

		private static long GetTenantSize(IConfigurationSession configSession, OrganizationId organizationId)
		{
			long num = 0L;
			num += (long)SystemAddressListMemberCount.GetCount(configSession, organizationId, "All Mailboxes(VLV)", false);
			num += (long)SystemAddressListMemberCount.GetCount(configSession, organizationId, "All Mail Users(VLV)", false);
			num += (long)SystemAddressListMemberCount.GetCount(configSession, organizationId, "All Contacts(VLV)", false);
			return num + (long)SystemAddressListMemberCount.GetCount(configSession, organizationId, "All Groups(VLV)", false);
		}

		private static bool IsTenantRelocationSupportEnabled()
		{
			return SyncConfiguration.GetConfigurationValue<int>("TenantRelocationEnabled", 1) == 1;
		}

		private const string LastCookie = "BackSyncLastCookie";

		private const string MSOnlinePartnerScope = "MSOnlinePartnerScope";

		private const string IncrementalSyncParameterSet = "IncrementalSyncParameterSet";

		private const string ObjectFullSyncInitialCallParameterSet = "ObjectFullSyncInitialCallParameterSet";

		private const string ObjectFullSyncInitialCallFromTenantFullSyncParameterSet = "ObjectFullSyncInitialCallFromTenantFullSyncParameterSet";

		private const string ObjectFullSyncInitialCallFromMergeSyncParameterSet = "ObjectFullSyncInitialCallFromMergeSyncParameterSet";

		private const string ObjectFullSyncSubsequentCallParameterSet = "ObjectFullSyncSubsequentCallParameterSet";

		private const string TenantFullSyncInitialCallParameterSet = "TenantFullSyncInitialCallParameterSet";

		private const string TenantFullSyncSubsequentCallParameterSet = "TenantFullSyncSubsequentCallParameterSet";

		private const string MergeInitialCallParameterSet = "MergeInitialCallParameterSet";

		private const string MergeSubsequentCallParameterSet = "MergeSubsequentCallParameterSet";

		private const string CookieParamName = "Cookie";

		private const string ObjectIdsName = "ObjectIds";

		private const string OrganizationName = "Organization";

		private const string SyncOptionsName = "SyncOptions";

		private const string ObjectFullSyncPageTokenName = "ObjectFullSyncPageToken";

		private const string TenantFullSyncPageTokenName = "TenantFullSyncPageToken";

		private const string TenantFullSyncPageTokenContextName = "TenantFullSyncPageTokenContext";

		private const string MergePageTokenName = "MergePageToken";

		private const string MergePageTokenContextName = "MergePageTokenContext";

		private const string MergeTenantFullSyncPageTokenName = "MergeTenantFullSyncPageToken";

		private const string MergeIncrementalSyncCookieName = "MergeIncrementalSyncCookie";

		private const string ServiceInstanceParamName = "ServiceInstance";

		private static readonly IDictionary<ADPropertyDefinition, RecipientTypeDetails> PropertyFilterMap = new Dictionary<ADPropertyDefinition, RecipientTypeDetails>
		{
			{
				SyncUserSchema.CloudMsExchBlockedSendersHash,
				RecipientTypeDetails.UserMailbox
			},
			{
				SyncUserSchema.CloudMsExchSafeRecipientsHash,
				RecipientTypeDetails.UserMailbox
			},
			{
				SyncUserSchema.CloudMsExchSafeSendersHash,
				RecipientTypeDetails.UserMailbox
			},
			{
				SyncUserSchema.CloudMsExchUCVoiceMailSettings,
				RecipientTypeDetails.UserMailbox | RecipientTypeDetails.MailUser
			},
			{
				SyncUserSchema.ServiceOriginatedResource,
				RecipientTypeDetails.UserMailbox | RecipientTypeDetails.MailUser
			}
		};

		private static ManagementScope msOnlineScope;

		private static bool enablePerformanceCounters = true;

		private string syncConfigurationMode;

		private SyncConfiguration syncConfiguration;

		private IDataProcessor dataProcessor;

		private BackSyncCookie syncCookie;

		private ObjectFullSyncPageToken objectFullSyncPageToken;

		private TenantFullSyncPageToken tenantFullSyncPageToken;

		private MergePageToken mergePageToken;

		private Guid invocationId;

		private PartitionId currentPartitionId;

		private PerformanceCounterSession performanceCounterSession;

		private ExchangeConfigurationUnit tenantFullSyncOrganizationCU;

		private long dirSyncBasedTenantFullSyncThreshold;

		private long currentTenantSize = -1L;

		internal class SyncEventLogger : ISyncEventLogger
		{
			public void LogSerializationFailedEvent(string objectId, int errorCount)
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_BackSyncExcludeFromBackSync, new string[]
				{
					objectId,
					errorCount.ToString()
				});
			}

			public void LogTooManyObjectReadRestartsEvent(string objectId, int pagedLinkReadRestartsLimit)
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_BackSyncTooManyObjectReadRestarts, new string[]
				{
					objectId,
					pagedLinkReadRestartsLimit.ToString()
				});
			}

			public void LogFullSyncFallbackDetectedEvent(BackSyncCookie previousCookie, BackSyncCookie currentCookie)
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_BackSyncFullSyncFailbackDetected, new string[]
				{
					previousCookie.LastWhenChanged.ToString(),
					currentCookie.LastWhenChanged.ToString(),
					previousCookie.InvocationId.ToString(),
					currentCookie.InvocationId.ToString(),
					Convert.ToBase64String(previousCookie.ToByteArray()),
					Convert.ToBase64String(currentCookie.ToByteArray())
				});
			}
		}
	}
}
