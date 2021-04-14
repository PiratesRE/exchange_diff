using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ThrottlingPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetThrottlingPolicy : GetMultitenancySystemConfigurationObjectTask<ThrottlingPolicyIdParameter, ThrottlingPolicy>
	{
		[Parameter(Mandatory = false)]
		public ThrottlingPolicyScopeType ThrottlingPolicyScope
		{
			get
			{
				return (ThrottlingPolicyScopeType)base.Fields[ThrottlingPolicySchema.ThrottlingPolicyScope];
			}
			set
			{
				base.VerifyValues<ThrottlingPolicyScopeType>(GetThrottlingPolicy.AllowedThrottlingPolicyScopeTypes, value);
				base.Fields[ThrottlingPolicySchema.ThrottlingPolicyScope] = value;
				if (value == ThrottlingPolicyScopeType.Global)
				{
					this.getGlobalPolicy = true;
				}
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Explicit { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Diagnostics { get; set; }

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity == null)
				{
					ADObjectId adobjectId;
					if (this.getGlobalPolicy)
					{
						adobjectId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
					}
					else
					{
						adobjectId = base.CurrentOrgContainerId;
						if (base.SharedConfiguration != null)
						{
							adobjectId = base.SharedConfiguration.SharedConfigurationCU.Id;
						}
					}
					return adobjectId.GetChildId("Global Settings");
				}
				return null;
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Static;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return base.OptionalIdentityData.AdditionalFilter;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.Fields.IsModified(ThrottlingPolicySchema.ThrottlingPolicyScope))
			{
				base.OptionalIdentityData.AdditionalFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					base.OptionalIdentityData.AdditionalFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ThrottlingPolicySchema.ThrottlingPolicyScope, this.ThrottlingPolicyScope)
				});
			}
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.getGlobalPolicy)
			{
				return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 187, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\throttling\\GetThrottlingPolicy.cs");
			}
			return base.CreateSession();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (base.WriteObjectCount == 0U && base.Fields.IsModified(ThrottlingPolicySchema.ThrottlingPolicyScope) && this.ThrottlingPolicyScope == ThrottlingPolicyScopeType.Organization && !this.Explicit)
			{
				DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 215, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\throttling\\GetThrottlingPolicy.cs");
				try
				{
					ThrottlingPolicy defaultOrganizationEffectiveThrottlingPolicy = ThrottlingPolicy.GetDefaultOrganizationEffectiveThrottlingPolicy();
					this.WriteWarning(Strings.WarningReturnDefaultOrganizationThrottlingPolicy);
					base.WriteResult(defaultOrganizationEffectiveThrottlingPolicy);
				}
				catch (GlobalThrottlingPolicyNotFoundException)
				{
					base.WriteError(new ManagementObjectNotFoundException(DirectoryStrings.GlobalThrottlingPolicyNotFoundException), ErrorCategory.ObjectNotFound, null);
				}
				catch (GlobalThrottlingPolicyAmbiguousException)
				{
					base.WriteError(new ManagementObjectAmbiguousException(DirectoryStrings.GlobalThrottlingPolicyAmbiguousException), ErrorCategory.InvalidResult, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ThrottlingPolicy throttlingPolicy = (ThrottlingPolicy)dataObject;
			if (!this.Explicit)
			{
				try
				{
					throttlingPolicy.ConvertToEffectiveThrottlingPolicy(false);
				}
				catch (GlobalThrottlingPolicyNotFoundException)
				{
					base.WriteError(new ManagementObjectNotFoundException(DirectoryStrings.GlobalThrottlingPolicyNotFoundException), ErrorCategory.ObjectNotFound, null);
				}
				catch (GlobalThrottlingPolicyAmbiguousException)
				{
					base.WriteError(new ManagementObjectAmbiguousException(DirectoryStrings.GlobalThrottlingPolicyAmbiguousException), ErrorCategory.InvalidResult, null);
				}
			}
			if (!this.Diagnostics)
			{
				throttlingPolicy.Diagnostics = null;
			}
			base.WriteResult(throttlingPolicy);
		}

		private static readonly ThrottlingPolicyScopeType[] AllowedThrottlingPolicyScopeTypes = (ThrottlingPolicyScopeType[])Enum.GetValues(typeof(ThrottlingPolicyScopeType));

		private bool getGlobalPolicy;
	}
}
