﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "HostedContentFilterPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveHostedContentFilterPolicy : RemoveSystemConfigurationObjectTask<HostedContentFilterPolicyIdParameter, HostedContentFilterPolicy>
	{
		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return base.InternalForce;
			}
			set
			{
				base.InternalForce = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveHostedContentFilterPolicy(this.Identity.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			if (base.DataObject.IsDefault && !this.Force)
			{
				base.WriteError(new OperationNotAllowedException(Strings.ErrorDefaultHostedContentFilterPolicyCannotBeDeleted), ErrorCategory.InvalidOperation, base.DataObject);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(base.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(base.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			FfoDualWriter.DeleteFromFfo<HostedContentFilterPolicy>(this, base.DataObject);
			TaskLogger.LogExit();
		}
	}
}
