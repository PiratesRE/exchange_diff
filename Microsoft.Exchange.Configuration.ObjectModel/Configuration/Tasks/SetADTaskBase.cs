using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetADTaskBase<TIdentity, TPublicObject, TDataObject> : SetTenantADTaskBase<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new() where TDataObject : ADObject, new()
	{
		protected bool IsUpgrading
		{
			get
			{
				return this.isUpgrading;
			}
		}

		protected virtual bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return false;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new ADObjectTaskModuleFactory();
		}

		protected virtual bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return false;
		}

		protected override void InternalProcessRecord()
		{
			if (base.IsVerboseOn)
			{
				if (this.IsObjectStateChanged())
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetConfigurableObjectChangedProperties(this.DataObject));
				}
				else
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetConfigurableObjectNonChangedProperties(this.DataObject));
				}
			}
			base.InternalProcessRecord();
		}

		protected virtual void UpgradeExchangeVersion(ADObject adObject)
		{
			adObject.SetExchangeVersion(adObject.MaximumSupportedExchangeObjectVersion);
			ADLegacyVersionableObject adlegacyVersionableObject = adObject as ADLegacyVersionableObject;
			if (adlegacyVersionableObject != null)
			{
				adlegacyVersionableObject.MinAdminVersion = new int?(adObject.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.isUpgrading = false;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			if (this.ExchangeVersionUpgradeSupported)
			{
				ADObject adobject = (ADObject)dataObject;
				if (adobject.ExchangeVersion.IsOlderThan(adobject.MaximumSupportedExchangeObjectVersion) && this.ShouldUpgradeExchangeVersion((ADObject)((object)this.Instance)))
				{
					this.UpgradeExchangeVersion(adobject);
					this.isUpgrading = true;
				}
			}
			base.StampChangesOn(dataObject);
		}

		private bool isUpgrading;
	}
}
