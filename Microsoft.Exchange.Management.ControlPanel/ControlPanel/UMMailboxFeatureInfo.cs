using System;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMMailboxFeatureInfo : MailboxFeatureInfo
	{
		public UMMailboxFeatureInfo(UMMailbox mailbox) : base(mailbox)
		{
			this.Initialize(true, false);
			this.UMMailboxPolicy = mailbox.UMMailboxPolicy.ToString();
			this.WhenChanged = mailbox.WhenChanged.ToString();
		}

		public UMMailboxFeatureInfo(Mailbox mailbox) : base(mailbox)
		{
			this.Initialize(mailbox.UMEnabled, !mailbox.UMEnabled && Utils.UnifiedMessagingAvailable((ADUser)mailbox.DataObject));
		}

		private void Initialize(bool isEnabled, bool canEnable)
		{
			base.UseModalDialogForEdit = false;
			base.Name = Strings.UMMailboxFeatureName;
			this.IconAltText = Strings.UMAltText;
			this.SpriteId = CommandSprite.GetCssClass(CommandSprite.SpriteId.UMMailboxFeature);
			base.EnableWizardDialogHeight = new int?(574);
			base.EnableWizardDialogWidth = new int?(600);
			base.PropertiesDialogHeight = new int?(PopupCommand.DefaultBookmarkedPopupHeight);
			base.PropertiesDialogWidth = new int?(PopupCommand.DefaultBookmarkedPopupWidth);
			if (base.Identity != null)
			{
				this.Status = (isEnabled ? ClientStrings.EnabledDisplayText : ClientStrings.DisabledDisplayText);
				base.EnableCommandUrl = "EnableUMMailbox.aspx";
				if (!isEnabled && canEnable && base.IsInRole(UMMailboxFeatureInfo.enableRoles))
				{
					this.CanChangeStatus = true;
				}
				if (isEnabled)
				{
					if (base.IsInRole(UMMailboxFeatureInfo.disableRoles))
					{
						this.CanChangeStatus = true;
					}
					if (base.IsInRole(UMMailboxFeatureInfo.editRoles))
					{
						base.EditCommandUrl = "EditUMMailbox.aspx?id=" + HttpUtility.UrlEncode(base.Identity.RawIdentity);
					}
				}
			}
		}

		[DataMember]
		public string UMMailboxPolicy { get; protected set; }

		[DataMember]
		public string WhenChanged { get; set; }

		private static readonly string[] enableRoles = new string[]
		{
			"Get-Recipient?Identity@R:Organization",
			"Enable-UMMailbox?Identity@W:Organization",
			"Get-UMMailboxPolicy@R:Organization"
		};

		public static readonly string EnableRolesString = UMMailboxFeatureInfo.enableRoles.StringArrayJoin("+");

		private static readonly string[] disableRoles = new string[]
		{
			"Disable-UMMailbox?Identity@W:Organization"
		};

		public static readonly string DisableRolesString = "Disable-UMMailbox?Identity@W:Organization";

		private static readonly string[] editRoles = new string[]
		{
			"Get-UMMailbox?Identity@R:Organization",
			"Get-UMMailboxPin?Identity@R:Organization"
		};

		public static readonly string EditRolesString = UMMailboxFeatureInfo.editRoles.StringArrayJoin("+");

		public static readonly string CanShowRoles = string.Concat(new string[]
		{
			UMMailboxFeatureInfo.DisableRolesString,
			"+",
			UMMailboxFeatureInfo.EnableRolesString,
			",",
			UMMailboxFeatureInfo.EditRolesString
		});
	}
}
