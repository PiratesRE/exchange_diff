using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "GlobalMonitoringOverride", SupportsShouldProcess = true)]
	public sealed class RemoveGlobalMonitoringOverride : SingletonSystemConfigurationObjectActionTask<MonitoringOverride>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0)]
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

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMonitoringOverride(this.PropertyName, this.helper.MonitoringItemIdentity, this.ItemType.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.helper.ParseAndValidateIdentity(this.Identity, true);
			this.configSession = (ITopologyConfigurationSession)base.CreateSession();
			ADObjectId descendantId = base.RootOrgContainerId.GetDescendantId(MonitoringOverride.RdnContainer);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MonitoringOverride.ContainerName);
			Container[] array = this.configSession.Find<Container>(descendantId, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				base.WriteError(new FailedToRunGlobalMonitoringOverrideException(MonitoringOverride.ContainerName), ErrorCategory.ObjectNotFound, null);
			}
			this.overridesContainer = array[0];
			Container container = this.GetContainer(this.overridesContainer, this.ItemType.ToString());
			this.healthSetContainer = this.GetContainer(container, this.helper.HealthSet);
			this.monitoringItemContainer = this.GetContainer(this.healthSetContainer, this.helper.MonitoringItemName);
			QueryFilter filter2 = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.PropertyName);
			this.monitoringItem = this.GetChildren(this.monitoringItemContainer.Id, filter2)[0];
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IConfigurable[] children = this.GetChildren(this.monitoringItemContainer.Id, null);
			if (children.Length == 1)
			{
				IConfigurable[] children2 = this.GetChildren(this.healthSetContainer.Id, null);
				if (children2.Length == 1)
				{
					this.configSession.DeleteTree(this.healthSetContainer, null);
				}
				else
				{
					this.configSession.DeleteTree(this.monitoringItemContainer, null);
				}
			}
			else
			{
				base.DataSession.Delete(this.monitoringItem);
			}
			this.overridesContainer.EncryptionKey0 = Guid.NewGuid().ToByteArray();
			base.DataSession.Save(this.overridesContainer);
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(Strings.SuccessRemoveGlobalMonitoringOverride(this.helper.MonitoringItemIdentity));
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is InvalidIdentityException || DataAccessHelper.IsDataAccessKnownException(exception) || base.IsKnownException(exception);
		}

		private Container GetContainer(Container parentContainer, string containerName)
		{
			Container childContainer = parentContainer.GetChildContainer(containerName);
			if (childContainer == null)
			{
				base.WriteError(new FailedToRunGlobalMonitoringOverrideException(containerName), ErrorCategory.ObjectNotFound, null);
			}
			return childContainer;
		}

		private IConfigurable[] GetChildren(ADObjectId containerId, QueryFilter filter)
		{
			IConfigurable[] array = this.configSession.Find<MonitoringOverride>(containerId, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				base.WriteError(new OverrideNotFoundException(this.helper.MonitoringItemIdentity, this.ItemType.ToString(), this.PropertyName), ErrorCategory.ObjectNotFound, null);
			}
			return array;
		}

		private ITopologyConfigurationSession configSession;

		private IConfigurable monitoringItem;

		private Container overridesContainer;

		private Container healthSetContainer;

		private Container monitoringItemContainer;

		private MonitoringOverrideHelpers helper = new MonitoringOverrideHelpers();
	}
}
