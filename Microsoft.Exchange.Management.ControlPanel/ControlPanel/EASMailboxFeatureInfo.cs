using System;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EASMailboxFeatureInfo : MailboxFeatureInfo
	{
		public EASMailboxFeatureInfo(CASMailbox mailbox) : base(mailbox)
		{
			base.UseModalDialogForEdit = false;
			base.Name = Strings.EASMailboxFeatureName;
			base.PropertiesDialogHeight = new int?(460);
			base.PropertiesDialogWidth = new int?(700);
			if (mailbox != null && mailbox.ActiveSyncEnabled)
			{
				this.Status = ClientStrings.EnabledDisplayText;
			}
			else
			{
				this.Status = ClientStrings.DisabledDisplayText;
			}
			base.EnableCommandUrl = null;
			base.EditCommandUrl = "/ecp/UsersGroups/EditMobileMailbox.aspx?id=" + HttpUtility.UrlEncode(base.Identity.RawIdentity) + "&DataTransferMode=Isolation";
			this.CanChangeStatus = RbacPrincipal.Current.IsInRole("Set-CasMailbox?Identity&ActiveSyncEnabled@W:Organization");
		}
	}
}
