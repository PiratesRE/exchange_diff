using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "TenantRelocationRequest", DefaultParameterSetName = "PartitionWide")]
	public sealed class GetTenantRelocationRequest : GetSystemConfigurationObjectTask<TenantRelocationRequestIdParameter, TenantRelocationRequest>
	{
		[Parameter(Mandatory = true, ParameterSetName = "PartitionWide")]
		public new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return (AccountPartitionIdParameter)base.Fields["AccountPartitionParam"];
			}
			set
			{
				base.Fields["AccountPartitionParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public SwitchParameter SourceStateOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["SourceStateOnlyParam"] ?? false);
			}
			set
			{
				base.Fields["SourceStateOnlyParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public RelocationStateRequested RelocationStateRequested
		{
			get
			{
				return (RelocationStateRequested)base.Fields["RelocationStateRequestedParam"];
			}
			set
			{
				base.Fields["RelocationStateRequestedParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public RelocationStatusDetailsSource RelocationStatusDetailsSource
		{
			get
			{
				return (RelocationStatusDetailsSource)base.Fields["RelocationStatusDetailsSourceParam"];
			}
			set
			{
				base.Fields["RelocationStatusDetailsSourceParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public RelocationError RelocationLastError
		{
			get
			{
				return (RelocationError)base.Fields["RelocationLastErrorParam"];
			}
			set
			{
				base.Fields["RelocationLastErrorParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public SwitchParameter Suspended
		{
			get
			{
				return (SwitchParameter)(base.Fields["SuspendedParam"] ?? false);
			}
			set
			{
				base.Fields["SuspendedParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public SwitchParameter Lockdown
		{
			get
			{
				return (SwitchParameter)(base.Fields["LockdownParam"] ?? false);
			}
			set
			{
				base.Fields["LockdownParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public SwitchParameter StaleLockdown
		{
			get
			{
				return (SwitchParameter)(base.Fields["StaleLockdownParam"] ?? false);
			}
			set
			{
				base.Fields["StaleLockdownParam"] = value;
			}
		}

		[Parameter(ParameterSetName = "PartitionWide")]
		public SwitchParameter HasPermanentError
		{
			get
			{
				return (SwitchParameter)(base.Fields["HasPermanentErrorParam"] ?? false);
			}
			set
			{
				base.Fields["HasPermanentErrorParam"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = base.Fields.IsModified("RelocationStateRequestedParam") ? new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationStateRequested, (int)this.RelocationStateRequested) : null;
				QueryFilter queryFilter2 = base.Fields.IsModified("RelocationStatusDetailsSourceParam") ? new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationStatusDetailsRaw, (byte)this.RelocationStatusDetailsSource) : null;
				QueryFilter queryFilter3 = base.Fields.IsModified("RelocationLastErrorParam") ? new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.RelocationLastError, (int)this.RelocationLastError) : null;
				QueryFilter queryFilter4 = base.Fields.IsModified("SuspendedParam") ? new ComparisonFilter(ComparisonOperator.Equal, TenantRelocationRequestSchema.Suspended, this.Suspended) : null;
				QueryFilter queryFilter5 = base.Fields.IsModified("LockdownParam") ? (this.Lockdown ? TenantRelocationRequest.LockedRelocationRequestsFilter : new NotFilter(TenantRelocationRequest.LockedRelocationRequestsFilter)) : null;
				QueryFilter queryFilter6 = null;
				if (base.Fields.IsModified("StaleLockdownParam"))
				{
					int config = TenantRelocationConfigImpl.GetConfig<int>("MaxTenantLockDownTimeInMinutes");
					ExDateTime olderThan = ExDateTime.Now.AddMinutes((double)(-(double)config));
					queryFilter6 = TenantRelocationRequest.GetStaleLockedRelocationRequestsFilter(olderThan, false);
					if (!this.StaleLockdown)
					{
						queryFilter6 = new NotFilter(queryFilter6);
					}
				}
				QueryFilter queryFilter7 = base.Fields.IsModified("HasPermanentErrorParam") ? new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.HasPermanentError, this.HasPermanentError) : null;
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					TenantRelocationRequest.TenantRelocationRequestFilter,
					queryFilter,
					queryFilter2,
					queryFilter3,
					queryFilter4,
					queryFilter5,
					queryFilter6,
					queryFilter7
				});
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.AccountPartition == null && this.Identity == null)
			{
				base.WriteError(new NotSupportedException(Strings.ErrorUnknownPartition), ErrorCategory.InvalidData, null);
			}
			PartitionId partitionId;
			if (this.Identity != null)
			{
				if (this.Identity.RawIdentity.Contains("*"))
				{
					base.WriteError(new ArgumentException(Strings.ErrorWildcardNotSupportedInRelocationIdentity(this.Identity.RawIdentity)), ErrorCategory.InvalidOperation, this.Identity);
				}
				OrganizationId organizationId = this.Identity.ResolveOrganizationId();
				partitionId = organizationId.PartitionId;
			}
			else
			{
				partitionId = RecipientTaskHelper.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			this.sourceForestRIDMaster = ForestTenantRelocationsCache.GetRidMasterName(partitionId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 223, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Relocation\\GetTenantRelocationRequest.cs");
			if (base.DomainController != null && !this.sourceForestRIDMaster.StartsWith(base.DomainController, StringComparison.OrdinalIgnoreCase))
			{
				ForestTenantRelocationsCache.Reset();
			}
			tenantConfigurationSession.SessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			return tenantConfigurationSession;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity
			});
			TenantRelocationRequest tenantRelocationRequest = (TenantRelocationRequest)dataObject;
			if (!this.SourceStateOnly)
			{
				if (tenantRelocationRequest.TargetForest != null)
				{
					this.targetForestRIDMaster = ForestTenantRelocationsCache.GetRidMasterName(new PartitionId(tenantRelocationRequest.TargetForest));
				}
				Exception ex;
				TenantRelocationRequest.PopulatePresentationObject(tenantRelocationRequest, this.targetForestRIDMaster, out ex);
				if (ex != null)
				{
					if (ex is CannotFindTargetTenantException)
					{
						base.WriteWarning(ex.Message);
					}
					else
					{
						base.WriteError(ex, ErrorCategory.InvalidOperation, tenantRelocationRequest.Identity);
					}
				}
				GetTenantRelocationRequest.PopulateGlsProperty(tenantRelocationRequest, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				GetTenantRelocationRequest.PopulateRidMasterProperties(tenantRelocationRequest, this.sourceForestRIDMaster, this.targetForestRIDMaster, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				if (tenantRelocationRequest.OriginatingServer != this.sourceForestRIDMaster)
				{
					this.warning = Strings.WarningShouldReadFromRidMaster(tenantRelocationRequest.OriginatingServer, this.sourceForestRIDMaster);
				}
			}
			base.WriteResult(tenantRelocationRequest);
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if (!string.IsNullOrEmpty(this.warning.ToString()))
			{
				this.WriteWarning(this.warning);
			}
			base.InternalEndProcessing();
			TaskLogger.LogExit();
		}

		internal static void PopulateRidMasterProperties(TenantRelocationRequest presentationObject, string sourceForestRIDMaster, string targetForestRIDMaster, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			presentationObject.SourceForestRIDMaster = sourceForestRIDMaster;
			presentationObject.TargetForestRIDMaster = targetForestRIDMaster;
		}

		internal static bool TryGlsLookupByExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId, out string resourceForestFqdn, out string accountForestFqdn, out string tenantContainerCN, out Exception exception)
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			resourceForestFqdn = null;
			accountForestFqdn = null;
			tenantContainerCN = null;
			bool result = false;
			try
			{
				result = glsDirectorySession.TryGetTenantForestsByOrgGuid(externalDirectoryOrganizationId, out resourceForestFqdn, out accountForestFqdn, out tenantContainerCN);
				exception = null;
			}
			catch (GlsTransientException ex)
			{
				exception = ex;
			}
			catch (GlsTenantNotFoundException ex2)
			{
				exception = ex2;
			}
			catch (GlsPermanentException ex3)
			{
				exception = ex3;
			}
			return result;
		}

		internal static void PopulateGlsProperty(TenantRelocationRequest presentationObject, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			if (ADSessionSettings.IsGlsDisabled)
			{
				presentationObject.GLSResolvedForest = GetTenantRelocationRequest.GlsDisabled;
				return;
			}
			Guid externalDirectoryOrganizationId = new Guid(presentationObject.ExternalDirectoryOrganizationId);
			string text;
			string glsresolvedForest;
			string text2;
			Exception ex;
			if (GetTenantRelocationRequest.TryGlsLookupByExternalDirectoryOrganizationId(externalDirectoryOrganizationId, out text, out glsresolvedForest, out text2, out ex))
			{
				presentationObject.GLSResolvedForest = glsresolvedForest;
			}
			else
			{
				presentationObject.GLSResolvedForest = GetTenantRelocationRequest.GlsLookupFailed;
			}
			if (ex != null)
			{
				presentationObject.GLSResolvedForest = "<" + ex.GetType().Name + ">";
				if (writeVerbose != null)
				{
					writeVerbose(Strings.ErrorInGlsLookup(ex.ToString()));
				}
			}
		}

		internal const string PartitionWide = "PartitionWide";

		internal const string AccountPartitionParam = "AccountPartitionParam";

		internal const string SourceStateOnlyParam = "SourceStateOnlyParam";

		internal const string RelocationStateRequestedParam = "RelocationStateRequestedParam";

		internal const string RelocationStatusDetailsSourceParam = "RelocationStatusDetailsSourceParam";

		internal const string RelocationLastErrorParam = "RelocationLastErrorParam";

		internal const string SuspendedParam = "SuspendedParam";

		internal const string LockdownParam = "LockdownParam";

		internal const string StaleLockdownParam = "StaleLockdownParam";

		internal const string HasPermanentErrorParam = "HasPermanentErrorParam";

		private string sourceForestRIDMaster;

		private string targetForestRIDMaster;

		private LocalizedString warning;

		internal static readonly string GlsDisabled = "<GLS disabled>";

		internal static readonly string GlsLookupFailed = "<GLS lookup failed - tenant not found>";
	}
}
