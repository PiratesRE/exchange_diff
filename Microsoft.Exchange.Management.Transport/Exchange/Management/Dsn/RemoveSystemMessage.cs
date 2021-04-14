using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Dsn
{
	[Cmdlet("Remove", "SystemMessage", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSystemMessage : RemoveSystemConfigurationObjectTask<SystemMessageIdParameter, SystemMessage>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveSystemMessage(this.Identity.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				ADObjectId orgContainerId = (base.DataSession as IConfigurationSession).GetOrgContainerId();
				return SystemMessage.GetDsnCustomizationContainer(orgContainerId);
			}
		}
	}
}
