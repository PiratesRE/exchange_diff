using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "HostedConnectionFilterPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetHostedConnectionFilterPolicy : SetSystemConfigurationObjectTask<HostedConnectionFilterPolicyIdParameter, HostedConnectionFilterPolicy>
	{
		[Parameter]
		public SwitchParameter MakeDefault { get; set; }

		[Parameter]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveHostedConnectionFilterPolicy(this.Identity.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			if (!this.IgnoreDehydratedFlag)
			{
				SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogEnter();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			ADObject adobject = dataObject as ADObject;
			if (adobject != null)
			{
				this.dualWriter = new FfoDualWriter(adobject.Name);
			}
			base.StampChangesOn(dataObject);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			HostedConnectionFilterPolicy hostedConnectionFilterPolicy = null;
			if (this.MakeDefault && !this.DataObject.IsDefault)
			{
				this.DataObject.IsDefault = true;
				hostedConnectionFilterPolicy = ((ITenantConfigurationSession)base.DataSession).GetDefaultFilteringConfiguration<HostedConnectionFilterPolicy>();
				if (hostedConnectionFilterPolicy != null && hostedConnectionFilterPolicy.IsDefault)
				{
					hostedConnectionFilterPolicy.IsDefault = false;
					base.DataSession.Save(hostedConnectionFilterPolicy);
				}
			}
			else if (base.Fields.Contains("MakeDefault") && !this.MakeDefault && this.DataObject.IsDefault)
			{
				base.WriteError(new OperationNotAllowedException(Strings.OperationNotAllowed), ErrorCategory.InvalidOperation, this.MakeDefault);
			}
			try
			{
				base.InternalProcessRecord();
				hostedConnectionFilterPolicy = null;
				this.dualWriter.Save<HostedConnectionFilterPolicy>(this, this.DataObject);
			}
			finally
			{
				if (hostedConnectionFilterPolicy != null)
				{
					hostedConnectionFilterPolicy.IsDefault = true;
					base.DataSession.Save(hostedConnectionFilterPolicy);
				}
			}
			TaskLogger.LogExit();
		}

		private FfoDualWriter dualWriter;
	}
}
