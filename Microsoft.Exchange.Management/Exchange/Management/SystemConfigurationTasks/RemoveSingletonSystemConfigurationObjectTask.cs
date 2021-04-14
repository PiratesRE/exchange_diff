using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class RemoveSingletonSystemConfigurationObjectTask<TDataObject> : DataAccessTask<TDataObject> where TDataObject : IConfigurable, new()
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

		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		internal ADSessionSettings SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, this.SessionSettings, 84, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ExchangeRpcClientAccess\\RemoveSingletonSystemConfigurationObjectTask.cs");
		}

		protected override void InternalStateReset()
		{
			this.sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			base.InternalStateReset();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			IConfigurable[] array = base.DataSession.Find<TDataObject>(null, this.RootId, true, null);
			if (array != null && array.Length == 1)
			{
				this.dataObject = (TDataObject)((object)array[0]);
			}
			IVersionable versionable = this.dataObject as IVersionable;
			if (versionable != null && versionable.MaximumSupportedExchangeObjectVersion.IsOlderThan(versionable.ExchangeVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorRemoveNewerObject(this.dataObject.Identity.ToString(), versionable.ExchangeVersion.ExchangeBuild.ToString())), ErrorCategory.InvalidArgument, null);
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

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.RootId,
				this.DataObject
			});
			try
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetDeleteObjectVerboseString(this.dataObject.Identity, base.DataSession, typeof(TDataObject)));
				base.DataSession.Delete(this.dataObject);
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.WriteError, null);
			}
			catch (NullReferenceException innerException)
			{
				LocalizedException exception2 = new LocalizedException(Strings.ExceptionRemoveNoneExistenceObject, innerException);
				base.WriteError(exception2, ErrorCategory.WriteError, null);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			TaskLogger.LogExit();
		}

		private TDataObject dataObject;

		private ADSessionSettings sessionSettings;
	}
}
