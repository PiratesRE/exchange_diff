using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Disable", "MailUser", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableMailUser : DisableMailUserBase<MailUserIdParameter>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["SoftDeletedMailUser"] ?? false);
			}
			set
			{
				base.Fields["SoftDeletedMailUser"] = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			if (this.IncludeSoftDeletedObjects)
			{
				base.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			TaskLogger.LogExit();
		}
	}
}
