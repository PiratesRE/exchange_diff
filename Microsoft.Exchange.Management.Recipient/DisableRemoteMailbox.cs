using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Disable", "RemoteMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableRemoteMailbox : DisableMailUserBase<RemoteMailboxIdParameter>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Archive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageDisableArchive(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageDisableRemoteMailbox(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			if ("Archive" == base.ParameterSetName)
			{
				aduser.RemoteRecipientType = ((aduser.RemoteRecipientType &= ~RemoteRecipientType.ProvisionArchive) | RemoteRecipientType.DeprovisionArchive);
			}
			else
			{
				bool flag = (aduser.RemoteRecipientType & RemoteRecipientType.ProvisionArchive) == RemoteRecipientType.ProvisionArchive;
				aduser.RemoteRecipientType = RemoteRecipientType.DeprovisionMailbox;
				if (flag)
				{
					aduser.RemoteRecipientType |= RemoteRecipientType.DeprovisionArchive;
				}
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return RemoteMailbox.FromDataObject((ADUser)dataObject);
		}
	}
}
