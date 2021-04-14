using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class RemoveTaskBase<TIdentity, TDataObject> : DataAccessTask<TDataObject> where TIdentity : IIdentityParameter where TDataObject : IConfigurable, new()
	{
		protected TDataObject DataObject
		{
			get
			{
				return this.dataObject;
			}
			set
			{
				this.dataObject = value;
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Identity == null)
			{
				base.WriteError(new ArgumentNullException("Identity"), (ErrorCategory)1000, null);
			}
			else
			{
				this.dataObject = (TDataObject)((object)this.ResolveDataObject());
			}
			IVersionable versionable = this.dataObject as IVersionable;
			if (versionable != null && versionable.MaximumSupportedExchangeObjectVersion.IsOlderThan(versionable.ExchangeVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorRemoveNewerObject(this.dataObject.Identity.ToString(), versionable.ExchangeVersion.ExchangeBuild.ToString())), (ErrorCategory)1004, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProvisioningValidation()
		{
			ProvisioningValidationError[] array = ProvisioningLayer.Validate(this, this.ConvertDataObjectToPresentationObject(this.DataObject));
			if (array != null && array.Length > 0)
			{
				foreach (ProvisioningValidationError provisioningValidationError in array)
				{
					ProvisioningValidationException exception = new ProvisioningValidationException(provisioningValidationError.Description, provisioningValidationError.AgentName, provisioningValidationError.Exception);
					this.WriteError(exception, (ErrorCategory)provisioningValidationError.ErrorCategory, null, false);
				}
			}
		}

		protected virtual IConfigurable ResolveDataObject()
		{
			return base.GetDataObject<TDataObject>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, null, null);
		}

		protected virtual bool ShouldSoftDeleteObject()
		{
			return false;
		}

		protected virtual void SaveSoftDeletedObject()
		{
			throw new NotImplementedException("Caller does not implement SaveSoftDeletedObject method.");
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity,
				this.DataObject
			});
			base.WriteVerbose(TaskVerboseStringHelper.GetDeleteObjectVerboseString(this.dataObject.Identity, base.DataSession, typeof(TDataObject)));
			try
			{
				if (this.ShouldSoftDeleteObject())
				{
					this.SaveSoftDeletedObject();
				}
				else
				{
					base.DataSession.Delete(this.dataObject);
				}
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			TaskLogger.LogExit();
		}

		internal sealed override void PreInternalProcessRecord()
		{
			if (base.IsProvisioningLayerAvailable && this.dataObject != null)
			{
				ProvisioningLayer.PreInternalProcessRecord(this, this.ConvertDataObjectToPresentationObject(this.dataObject), false);
			}
		}

		private TDataObject dataObject;
	}
}
