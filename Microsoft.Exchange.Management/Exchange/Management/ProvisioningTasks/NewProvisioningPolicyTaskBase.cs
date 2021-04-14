using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	public abstract class NewProvisioningPolicyTaskBase<TDataObject> : NewMultitenancyFixedNameSystemConfigurationObjectTask<TDataObject> where TDataObject : ADProvisioningPolicy, new()
	{
		protected abstract ADObjectId ContainerRdn { get; }

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (TDataObject)((object)base.PrepareDataObject());
			if (!base.HasErrors)
			{
				ADObjectId descendantId = base.CurrentOrgContainerId.GetDescendantId(ADProvisioningPolicy.RdnContainer);
				TDataObject[] array = this.ConfigurationSession.Find<TDataObject>(descendantId, QueryScope.SubTree, null, null, 2);
				if (array != null && array.Length != 0)
				{
					if (1 == array.Length)
					{
						Exception exception = new InvalidOperationException(Strings.ErrorOnlyOneProvisioningPolicyAllowed(array[0].Identity.ToString()));
						ErrorCategory category = ErrorCategory.InvalidOperation;
						TDataObject dataObject = this.DataObject;
						base.WriteError(exception, category, dataObject.Name);
					}
					else
					{
						Exception exception2 = new InvalidOperationException(Strings.ErrorMultipleProvisioningPolicyExist);
						ErrorCategory category2 = ErrorCategory.InvalidOperation;
						TDataObject dataObject2 = this.DataObject;
						base.WriteError(exception2, category2, dataObject2.Name);
					}
				}
				TDataObject dataObject3 = this.DataObject;
				ADObjectId descendantId2 = base.CurrentOrgContainerId.GetDescendantId(this.ContainerRdn);
				TDataObject dataObject4 = this.DataObject;
				dataObject3.SetId(descendantId2.GetChildId(dataObject4.Name));
				TDataObject dataObject5 = this.DataObject;
				if (dataObject5.Scopes.Count != 0)
				{
					TDataObject dataObject6 = this.DataObject;
					dataObject6.Scopes.Clear();
				}
				TDataObject dataObject7 = this.DataObject;
				dataObject7.Scopes.Add(base.CurrentOrgContainerId);
				TDataObject dataObject8 = this.DataObject;
				if (dataObject8.TargetObjects.Count != 0)
				{
					TDataObject dataObject9 = this.DataObject;
					dataObject9.TargetObjects.Clear();
				}
				TDataObject dataObject10 = this.DataObject;
				foreach (Type poType in dataObject10.SupportedPresentationObjectTypes)
				{
					TDataObject dataObject11 = this.DataObject;
					dataObject11.TargetObjects.Add(ProvisioningHelper.GetProvisioningObjectTag(poType));
				}
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}
	}
}
