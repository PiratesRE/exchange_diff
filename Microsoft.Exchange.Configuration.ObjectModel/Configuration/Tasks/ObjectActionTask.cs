using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class ObjectActionTask<TIdentity, TDataObject> : SetTaskBase<TDataObject> where TIdentity : IIdentityParameter, new() where TDataObject : IConfigurable, new()
	{
		protected virtual bool DelayProvisioning
		{
			get
			{
				return false;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public virtual TIdentity Identity
		{
			get
			{
				return (TIdentity)((object)base.Fields["Identity"]);
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TDataObject tdataObject = (TDataObject)((object)this.ResolveDataObject());
			ADObject adobject = tdataObject as ADObject;
			if (adobject != null)
			{
				base.CurrentOrganizationId = adobject.OrganizationId;
			}
			if (base.CurrentObjectIndex == 0)
			{
				this.ResolveLocalSecondaryIdentities();
				if (base.HasErrors)
				{
					return null;
				}
			}
			if (!this.DelayProvisioning && base.IsProvisioningLayerAvailable)
			{
				this.ProvisionDefaultValues((default(TDataObject) == null) ? Activator.CreateInstance<TDataObject>() : default(TDataObject), tdataObject);
			}
			TaskLogger.LogExit();
			return tdataObject;
		}

		protected virtual void ProvisionDefaultValues(IConfigurable temporaryObject, IConfigurable dataObject)
		{
			ProvisioningLayer.ProvisionDefaultProperties(this, this.ConvertDataObjectToPresentationObject(temporaryObject), this.ConvertDataObjectToPresentationObject(dataObject), false);
			this.ValidateProvisionedProperties(dataObject);
		}

		protected virtual void ValidateProvisionedProperties(IConfigurable dataObject)
		{
		}

		protected virtual IConfigurable ResolveDataObject()
		{
			return base.GetDataObject<TDataObject>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, null, null);
		}

		protected override void InternalValidate()
		{
			if (this.Identity == null && base.ParameterSetName == "Identity")
			{
				base.WriteError(new ArgumentNullException("Identity"), (ErrorCategory)1000, null);
			}
			base.InternalValidate();
		}
	}
}
