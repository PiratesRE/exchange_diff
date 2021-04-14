using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetObjectTaskBase<TPublicObject, TDataObject> : SetTaskBase<TDataObject>, IDynamicParameters where TPublicObject : IConfigurable, new() where TDataObject : IConfigurable, new()
	{
		protected SetObjectTaskBase()
		{
			this.dynamicParametersInstance = this.CreateBlankPublicObjectInstance();
		}

		protected TPublicObject DynamicParametersInstance
		{
			get
			{
				return this.dynamicParametersInstance;
			}
		}

		protected virtual TPublicObject Instance
		{
			get
			{
				return (TPublicObject)((object)base.Fields["Instance"]);
			}
			set
			{
				base.Fields["Instance"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.IsObjectStateChanged())
			{
				bool flag = false;
				if (!base.TryGetVariableValue<bool>("ExchangeDisableNotChangedWarning", out flag) || !flag)
				{
					TDataObject dataObject = this.DataObject;
					if (dataObject.Identity != null)
					{
						TDataObject dataObject2 = this.DataObject;
						this.WriteWarning(Strings.WarningForceMessageWithId(dataObject2.Identity.ToString()));
					}
					else
					{
						this.WriteWarning(Strings.WarningForceMessage);
					}
				}
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			if (base.ParameterSetName != "Instance")
			{
				this.Instance = this.CreateBlankPublicObjectInstance();
			}
			TaskLogger.LogExit();
		}

		public virtual object GetDynamicParameters()
		{
			return this.dynamicParametersInstance;
		}

		protected virtual TPublicObject CreateBlankPublicObjectInstance()
		{
			TPublicObject tpublicObject = (default(TPublicObject) == null) ? Activator.CreateInstance<TPublicObject>() : default(TPublicObject);
			ConfigurableObject configurableObject = tpublicObject as ConfigurableObject;
			if (configurableObject != null)
			{
				configurableObject.EnableSaveCalculatedValues();
				configurableObject.InitializeSchema();
			}
			return tpublicObject;
		}

		protected virtual bool IsObjectStateChanged()
		{
			TDataObject dataObject = this.DataObject;
			return dataObject.ObjectState != ObjectState.Unchanged;
		}

		protected virtual void StampChangesOn(IConfigurable dataObject)
		{
			dataObject.CopyChangesFrom(this.Instance);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable configurable = this.ResolveDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			ADObject adobject = configurable as ADObject;
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
			TPublicObject instance = this.Instance;
			instance.CopyChangesFrom(this.dynamicParametersInstance);
			this.StampChangesOn(configurable);
			if (base.HasErrors)
			{
				return null;
			}
			TaskLogger.LogExit();
			return configurable;
		}

		protected abstract IConfigurable ResolveDataObject();

		private readonly TPublicObject dynamicParametersInstance;
	}
}
