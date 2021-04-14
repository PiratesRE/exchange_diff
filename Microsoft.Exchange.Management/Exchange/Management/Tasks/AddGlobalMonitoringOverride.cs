using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Add", "GlobalMonitoringOverride", SupportsShouldProcess = true, DefaultParameterSetName = "Duration")]
	public sealed class AddGlobalMonitoringOverride : SingletonSystemConfigurationObjectActionTask<MonitoringOverride>
	{
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public string Identity
		{
			get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public MonitoringItemTypeEnum ItemType
		{
			get
			{
				return (MonitoringItemTypeEnum)base.Fields["ItemType"];
			}
			set
			{
				base.Fields["ItemType"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string PropertyName
		{
			get
			{
				return (string)base.Fields["PropertyName"];
			}
			set
			{
				base.Fields["PropertyName"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string PropertyValue
		{
			get
			{
				return (string)base.Fields["PropertyValue"];
			}
			set
			{
				base.Fields["PropertyValue"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Duration")]
		public EnhancedTimeSpan? Duration
		{
			get
			{
				if (!base.Fields.Contains("Duration"))
				{
					return null;
				}
				return (EnhancedTimeSpan?)base.Fields["Duration"];
			}
			set
			{
				base.Fields["Duration"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ApplyVersion")]
		public Version ApplyVersion
		{
			get
			{
				return (Version)base.Fields["ApplyVersion"];
			}
			set
			{
				base.Fields["ApplyVersion"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddMonitoringOverride(this.PropertyName, this.helper.MonitoringItemIdentity, this.ItemType.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.helper.ParseAndValidateIdentity(this.Identity, true);
			if (base.Fields.IsModified("ApplyVersion"))
			{
				MonitoringOverrideHelpers.ValidateApplyVersion(this.ApplyVersion);
			}
			if (base.Fields.IsModified("Duration"))
			{
				MonitoringOverrideHelpers.ValidateOverrideDuration(this.Duration);
			}
			else
			{
				this.Duration = new EnhancedTimeSpan?(EnhancedTimeSpan.FromDays(365.0));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			ADObjectId descendantId = base.RootOrgContainerId.GetDescendantId(MonitoringOverride.RdnContainer);
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.CreateSession();
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MonitoringOverride.ContainerName);
			Container[] array = topologyConfigurationSession.Find<Container>(descendantId, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				base.WriteError(new FailedToRunGlobalMonitoringOverrideException(MonitoringOverride.ContainerName), ErrorCategory.ObjectNotFound, null);
			}
			this.overridesContainer = array[0];
			Container childContainer = this.overridesContainer.GetChildContainer(this.ItemType.ToString());
			if (childContainer == null)
			{
				base.WriteError(new FailedToRunGlobalMonitoringOverrideException(this.ItemType.ToString()), ErrorCategory.ObjectNotFound, null);
			}
			this.typeContainerId = childContainer.Id;
			Container childContainer2 = childContainer.GetChildContainer(this.helper.HealthSet);
			if (childContainer2 != null)
			{
				this.healthSetContainerId = childContainer2.Id;
				Container childContainer3 = childContainer2.GetChildContainer(this.helper.MonitoringItemName);
				if (childContainer3 != null)
				{
					this.monitoringItemContainerId = childContainer3.Id;
					QueryFilter filter2 = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.PropertyName);
					MonitoringOverride[] array2 = topologyConfigurationSession.Find<MonitoringOverride>(this.monitoringItemContainerId, QueryScope.OneLevel, filter2, null, 1);
					if (array2.Length > 0)
					{
						base.WriteError(new PropertyAlreadyHasAnOverrideException(this.PropertyName, this.helper.MonitoringItemIdentity, this.ItemType.ToString()), ErrorCategory.ResourceExists, null);
					}
				}
			}
			TaskLogger.LogExit();
			base.InternalValidate();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			MonitoringOverride monitoringOverride = new MonitoringOverride();
			if (base.HasErrors)
			{
				return null;
			}
			if (this.healthSetContainerId == null)
			{
				this.healthSetContainerId = this.CreateContainer(this.typeContainerId, this.helper.HealthSet);
			}
			if (this.monitoringItemContainerId == null)
			{
				this.monitoringItemContainerId = this.CreateContainer(this.healthSetContainerId, this.helper.MonitoringItemName);
			}
			monitoringOverride.HealthSet = this.helper.HealthSet;
			monitoringOverride.MonitoringItemName = this.helper.MonitoringItemName;
			monitoringOverride.PropertyValue = this.PropertyValue;
			monitoringOverride.CreatedBy = base.ExecutingUserIdentityName;
			if (base.Fields.IsModified("Duration"))
			{
				monitoringOverride.ExpirationTime = new DateTime?(DateTime.UtcNow.AddSeconds(this.Duration.Value.TotalSeconds));
			}
			if (base.Fields.IsModified("ApplyVersion"))
			{
				monitoringOverride.ApplyVersion = this.ApplyVersion;
			}
			monitoringOverride.SetId(this.monitoringItemContainerId.GetChildId(this.PropertyName));
			TaskLogger.LogExit();
			return monitoringOverride;
		}

		protected override void InternalProcessRecord()
		{
			this.overridesContainer.EncryptionKey0 = Guid.NewGuid().ToByteArray();
			base.DataSession.Save(this.overridesContainer);
			base.InternalProcessRecord();
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(Strings.SuccessAddGlobalMonitoringOverride(this.helper.MonitoringItemIdentity));
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is InvalidVersionException || exception is InvalidIdentityException || exception is InvalidDurationException || DataAccessHelper.IsDataAccessKnownException(exception) || base.IsKnownException(exception);
		}

		private ADObjectId CreateContainer(ADObjectId baseContainer, string coantainerName)
		{
			Container container = new Container();
			container.SetId(baseContainer.GetChildId(coantainerName));
			base.DataSession.Save(container);
			return container.Id;
		}

		private ADObjectId typeContainerId;

		private ADObjectId healthSetContainerId;

		private ADObjectId monitoringItemContainerId;

		private Container overridesContainer;

		private MonitoringOverrideHelpers helper = new MonitoringOverrideHelpers();
	}
}
