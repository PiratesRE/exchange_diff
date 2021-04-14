using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "RetentionPolicyTag", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRetentionPolicyTag : RemoveSystemConfigurationObjectTask<RetentionPolicyTagIdParameter, RetentionPolicyTag>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRetentionPolicyTag(this.Identity.ToString());
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
			base.InternalValidate();
			if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
			{
				base.WriteError(new ArgumentException(Strings.ErrorWriteOpOnDehydratedTenant), ErrorCategory.InvalidArgument, base.DataObject.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.DataObject != null && SharedConfiguration.IsSharedConfiguration(base.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(base.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.DataObject.GetELCContentSettings().ForEach(delegate(ElcContentSettings x)
			{
				base.DataSession.Delete(x);
			});
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
