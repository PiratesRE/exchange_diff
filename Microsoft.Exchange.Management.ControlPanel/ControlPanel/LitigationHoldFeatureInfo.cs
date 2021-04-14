using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class LitigationHoldFeatureInfo : MailboxFeatureInfo
	{
		public LitigationHoldFeatureInfo(Mailbox mailbox) : base(mailbox)
		{
			base.UseModalDialogForEdit = true;
			base.UseModalDialogForEnable = true;
			base.Name = Strings.LitigationHoldFeatureName;
			base.EnableWizardDialogHeight = new int?(450);
			base.EnableWizardDialogWidth = new int?(510);
			base.PropertiesDialogHeight = new int?(450);
			base.PropertiesDialogWidth = new int?(510);
			this.IconAltText = Strings.MailboxAltText;
			this.SpriteId = CommandSprite.GetCssClass(CommandSprite.SpriteId.Mailbox16);
			if (mailbox != null)
			{
				this.Caption = mailbox.DisplayName;
				bool litigationHoldEnabled = mailbox.LitigationHoldEnabled;
				this.Status = (litigationHoldEnabled ? ClientStrings.EnabledDisplayText : ClientStrings.DisabledDisplayText);
				if (base.IsInRole(LitigationHoldFeatureInfo.enableRoles) && !litigationHoldEnabled)
				{
					this.CanChangeStatus = true;
					base.EnableCommandUrl = "EditLitigationHold.aspx";
				}
				if (base.IsInRole(LitigationHoldFeatureInfo.editRoles) && litigationHoldEnabled)
				{
					base.EditCommandUrl = "EditLitigationHold.aspx";
				}
				if (base.IsInRole(LitigationHoldFeatureInfo.disableRoles) && litigationHoldEnabled)
				{
					this.CanChangeStatus = true;
				}
				this.RetentionComment = mailbox.RetentionComment;
				this.RetentionUrl = mailbox.RetentionUrl;
				if (mailbox.LitigationHoldDate != null)
				{
					this.LitigationHoldDate = mailbox.LitigationHoldDate.Value.ToUniversalTime().UtcToUserDateTimeString();
				}
				else
				{
					this.LitigationHoldDate = Strings.LitigationHoldDateNotSet;
				}
				if (!string.IsNullOrEmpty(mailbox.LitigationHoldOwner))
				{
					this.LitigationHoldOwner = mailbox.LitigationHoldOwner;
				}
				else
				{
					this.LitigationHoldOwner = Strings.LitigationHoldOwnerNotSet;
				}
			}
			if (!base.IsReadOnly && mailbox.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				base.ShowReadOnly = true;
				this.CanChangeStatus = false;
				base.EditCommandUrl = null;
			}
		}

		[DataMember]
		public string Caption { get; private set; }

		[DataMember]
		public string RetentionComment { get; private set; }

		[DataMember]
		public string RetentionUrl { get; private set; }

		[DataMember]
		public string LitigationHoldDate { get; private set; }

		[DataMember]
		public string LitigationHoldOwner { get; private set; }

		private const string LitigationHoldUrl = "EditLitigationHold.aspx";

		private static readonly string[] enableRoles = new string[]
		{
			"Get-Mailbox?Identity@R:Organization+Set-Mailbox?Identity&LitigationHoldEnabled&RetentionComment&RetentionUrl@W:Organization"
		};

		private static readonly string[] editRoles = new string[]
		{
			"Get-Mailbox?Identity@R:Organization",
			"Get-Mailbox?Identity@R:Organization+Set-Mailbox?Identity&RetentionComment&RetentionUrl@W:Organization"
		};

		private static readonly string[] disableRoles = new string[]
		{
			"Set-Mailbox?Identity&LitigationHoldEnabled@W:Organization"
		};
	}
}
