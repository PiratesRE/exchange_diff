using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Remove", "UMIPGateway", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveUMIPGateway : RemoveSystemConfigurationObjectTask<UMIPGatewayIdParameter, UMIPGateway>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveUMIPGateway(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!base.HasErrors)
			{
				foreach (UMHuntGroup instance in base.DataObject.HuntGroups)
				{
					base.DataSession.Delete(instance);
				}
			}
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_IPGatewayRemoved, null, new object[]
				{
					base.DataObject.Name,
					base.DataObject.Address.ToString()
				});
			}
			TaskLogger.LogExit();
		}
	}
}
