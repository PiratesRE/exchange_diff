using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class CommandSprite : BaseSprite
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.CssClass = this.CssClass + " " + this.SpriteCssClass;
			this.ImageUrl = BaseSprite.GetSpriteImageSrc(this);
			string value = (this.AlternateText == null) ? "" : this.AlternateText;
			base.Attributes.Add("title", value);
			this.GenerateEmptyAlternateText = true;
		}

		public CommandSprite.SpriteId ImageId { get; set; }

		public string SpriteCssClass
		{
			get
			{
				return CommandSprite.GetCssClass(this.ImageId);
			}
		}

		public bool IsRenderable
		{
			get
			{
				return this.ImageId != CommandSprite.SpriteId.NONE;
			}
		}

		public static string GetCssClass(CommandSprite.SpriteId spriteid)
		{
			if (spriteid == CommandSprite.SpriteId.NONE)
			{
				return string.Empty;
			}
			return CommandSprite.GetBaseCssClass() + " " + CommandSprite.ImageStyles[(int)spriteid];
		}

		private static string GetBaseCssClass()
		{
			string text = CommandSprite.BaseCssClass;
			if (CommandSprite.HasDCImage && BaseSprite.IsDataCenter)
			{
				text = "DC" + text;
			}
			return text;
		}

		public static readonly string BaseCssClass = "CommandSprite";

		public static readonly bool HasDCImage = false;

		private static readonly string[] ImageStyles = new string[]
		{
			string.Empty,
			"CV-CS",
			"AX-CS",
			"AM-CS",
			"AN-CS",
			"DisabledButtonPart .B-CS",
			"B-CS",
			"EnabledButtonPart:hover .B-CS",
			"I-CS",
			"J-CS",
			"K-CS",
			"H-CS",
			"F-CS",
			"G-CS",
			"EnabledToolBarItem:hover .CB-CS",
			"CB-CS",
			"BZ-CS",
			"BX-CS",
			"BY-CS",
			"CA-CS",
			"DisabledToolBarItem .BZ-CS",
			"EnabledToolBarItem:hover .CA-CS",
			"DisabledToolBarItem .CA-CS",
			"EnabledToolBarItem:hover .BZ-CS",
			"DisabledToolBarItem .CB-CS",
			"search-publicfoldersearch",
			"CF-CS",
			"CG-CS",
			"CI-CS",
			"CH-CS",
			"DisabledToolBarItem .CE-CS",
			"CD-CS",
			"CC-CS",
			"EnabledToolBarItem:hover .CD-CS",
			"EnabledToolBarItem:hover .CE-CS",
			"CE-CS",
			"BW-CS",
			"MoveDown",
			"DisabledToolBarItem .BP-CS",
			"EnabledToolBarItem:hover .BP-CS",
			"MoveUp",
			"DisabledToolBarItem .MoveDown",
			"EnabledToolBarItem:hover .MoveDown",
			"BN-CS",
			"BM-CS",
			"BL-CS",
			"BP-CS",
			"EnabledToolBarItem:hover .BO-CS",
			"BO-CS",
			"BU-CS",
			"BT-CS",
			"DisabledToolBarItem .BS-CS",
			"DisabledToolBarItem .BV-CS",
			"EnabledToolBarItem:hover .BV-CS",
			"BV-CS",
			"BQ-CS",
			"DisabledToolBarItem .MoveUp",
			"EnabledToolBarItem:hover .MoveUp",
			"EnabledToolBarItem:hover .BS-CS",
			"BS-CS",
			"BR-CS",
			"CJ-CS",
			"CS-CS",
			"DisabledToolBarItem .ToolBarRemoveAll",
			"EnabledToolBarItem:hover .ToolBarRemoveAll",
			"DisabledToolBarItem .CT-CS",
			"EnabledToolBarItem:hover .CT-CS",
			"CT-CS",
			"CR-CS",
			"WhiteEditButton",
			"DisabledToolBarItem .EditButton",
			"ToolBarRemoveAll",
			"DisabledToolBarItem .CR-CS",
			"EnabledToolBarItem:hover .CR-CS",
			"CU-CS",
			"DC-CS",
			"DisabledToolBarItem .DB-CS",
			"EnabledToolBarItem:hover .DB-CS",
			"DisabledToolBarItem .DD-CS",
			"EnabledToolBarItem:hover .DD-CS",
			"DD-CS",
			"CY-CS",
			"CX-CS",
			"CW-CS",
			"DB-CS",
			"DA-CS",
			"CZ-CS",
			"CM-CS",
			"DisabledToolBarItem .CL-CS",
			"EnabledToolBarItem:hover .CL-CS",
			"CN-CS",
			"DisabledToolBarItem .CM-CS",
			"EnabledToolBarItem:hover .CM-CS",
			"CK-CS",
			"DisabledToolBarItem .CJ-CS",
			"EnabledToolBarItem:hover .CJ-CS",
			"CL-CS",
			"DisabledToolBarItem .CK-CS",
			"EnabledToolBarItem:hover .CK-CS",
			"EnabledToolBarItem:hover .CN-CS",
			"EnabledToolBarItem:hover .CQ-CS",
			"CQ-CS",
			"WhiteCloseButton",
			"EnabledToolBarItem:hover .EditButton",
			"EditButton",
			"DisabledToolBarItem .CQ-CS",
			"EnabledToolBarItem:hover .CO-CS",
			"CO-CS",
			"DisabledToolBarItem .CN-CS",
			"CloseButton",
			"CP-CS",
			"DisabledToolBarItem .CO-CS",
			"BK-CS",
			"DisabledArrow",
			"DisabledToolBarItem .X-CS",
			"EnabledToolBarItem:hover .X-CS",
			"AA-CS",
			"Z-CS",
			"Y-CS",
			"X-CS",
			"DisabledToolBarItem .V-CS",
			"EnabledToolBarItem:hover .V-CS",
			"V-CS",
			"DisabledButtonPart .W-CS",
			"EnabledButtonPart:hover .W-CS",
			"W-CS",
			"DisabledToolBarItem .AE-CS",
			"EnabledToolBarItem:hover .AE-CS",
			"AE-CS",
			"EnabledToolBarItem:hover .AF-CS",
			"A-CS",
			"AF-CS",
			"AD-CS",
			"DisabledToolBarItem .AB-CS",
			"EnabledToolBarItem:hover .AB-CS",
			"AB-CS",
			"DisabledToolBarItem .AC-CS",
			"EnabledToolBarItem:hover .AC-CS",
			"AC-CS",
			"DisabledToolBarItem .M-CS",
			"EnabledToolBarItem:hover .M-CS",
			"M-CS",
			"Q-CS",
			"P-CS",
			"O-CS",
			"DisabledToolBarItem .L-CS",
			"D-CS",
			"C-CS",
			"EnabledToolBarItem:hover .A-CS",
			"EnabledToolBarItem:hover .L-CS",
			"L-CS",
			"E-CS",
			"Copy",
			"T-CS",
			"S-CS",
			"DisabledButtonPart .U-CS",
			"EnabledButtonPart:hover .U-CS",
			"U-CS",
			"DisabledToolBarItem .R-CS",
			"ChoiceButton",
			"DisabledToolBarItem .Q-CS",
			"EnabledToolBarItem:hover .Q-CS",
			"EnabledToolBarItem:hover .R-CS",
			"R-CS",
			"clear-default",
			"DisabledToolBarItem .AF-CS",
			"AU-CS",
			"DisabledToolBarItem .AT-CS",
			"AW-CS",
			"AZ-CS",
			"AY-CS",
			"EnabledToolBarItem:hover .AT-CS",
			"BG-CS",
			"FacebookLogo",
			"BI-CS",
			"AT-CS",
			"AS-CS",
			"BA-CS",
			"BE-CS",
			"BD-CS",
			"BF-CS",
			"DisabledToolBarItem .BF-CS",
			"EnabledToolBarItem:hover .BF-CS",
			"DisabledToolBarItem .BC-CS",
			"LinkedInLogo",
			"BB-CS",
			"BC-CS",
			"BH-CS",
			"EnabledToolBarItem:hover .BC-CS",
			"DisabledToolBarItem .AR-CS",
			"AK-CS",
			"DisabledButtonPart .Ellipsis",
			"EnabledButtonPart:hover .Ellipsis",
			"EnabledToolBarItem:hover .AO-CS",
			"AO-CS",
			"AL-CS",
			"BJ-CS",
			"AH-CS",
			"AG-CS",
			"Ellipsis",
			"AJ-CS",
			"AI-CS",
			"EnabledToolBarItem:hover .AQ-CS",
			"AQ-CS",
			"DisabledToolBarItem .AQ-CS",
			"EnabledToolBarItem:hover .AR-CS",
			"DisabledToolBarItem .AO-CS",
			"AR-CS",
			"EnabledArrow",
			"AP-CS",
			"EnabledToolBarItem:hover .search-default",
			"search-default",
			"DisabledToolBarItem .search-default",
			"AV-CS",
			"PagePre",
			"PageFirst",
			"PageLast",
			"PageNext",
			"SrtDsc",
			"SrtAsc",
			"DeleteButtonSmall",
			"N-CS"
		};

		public enum SpriteId
		{
			NONE,
			UMAutoAttendant32,
			HotmailSubscription28,
			EmailSubscription28,
			Eml,
			AddDropdown_disable,
			AddDropdown,
			AddDropdown_hover,
			AudioQualityOneBar,
			AudioQualityThreeBars,
			AudioQualityTwoBars,
			AudioQualityNotAvailable,
			AudioQualityFiveBars,
			AudioQualityFourBars,
			ReleaseMessage_hover,
			ReleaseMessage,
			Print,
			PerfConsoleCommand,
			PreviewMailBoxSearch,
			RecoverMailbox16,
			Print_disable,
			RecoverMailbox16_hover,
			RecoverMailbox16_disable,
			Print_hover,
			ReleaseMessage_disable,
			SearchPublicFolder,
			Room16,
			Shared16,
			SmileFaceGray16,
			SingUpAddress,
			RetrieveLog16_disable,
			ReSetVDir16,
			RemoteMailbox16,
			ReSetVDir16_hover,
			RetrieveLog16_hover,
			RetrieveLog16,
			Password16,
			MoveDownCommand,
			MetroAdd_disable,
			MetroAdd_hover,
			MoveUpCommand,
			MoveDownCommand_disable,
			MoveDownCommand_hover,
			MailUser,
			MailEnabledPublicFolder,
			MailboxSearchSucceeded,
			MetroAdd,
			ManageDAGMembership16_hover,
			ManageDAGMembership16,
			O365Reports,
			NewRoleAssignmentPolicy16,
			NewDeviceAccessRule16_disable,
			OutlookSettings_disable,
			OutlookSettings_hover,
			OutlookSettings,
			NewActiveSyncPolicy16,
			MoveUpCommand_disable,
			MoveUpCommand_hover,
			NewDeviceAccessRule16_hover,
			NewDeviceAccessRule16,
			NewAdminRoleGroup16,
			Start,
			TransportRule16,
			ToolBarRemoveAll_disable,
			ToolBarRemoveAll_hover,
			UMAudioQualityDetails_disable,
			UMAudioQualityDetails_hover,
			UMAudioQualityDetails,
			ToolBarRemove,
			ToolBarPropertiesWhite,
			ToolBarProperties_disable,
			ToolBarRemoveAll,
			ToolBarRemove_disable,
			ToolBarRemove_hover,
			UMAutoAttendant16,
			Users16,
			UnblockDevice16_disable,
			UnblockDevice16_hover,
			WipeMobileDevice16_disable,
			WipeMobileDevice16_hover,
			WipeMobileDevice16,
			UMIPGateways16,
			UMHuntGroups16,
			UMDialPlans16,
			UnblockDevice16,
			UMMailboxPolicy16,
			UMMailboxFeature,
			Stop,
			StartMailBoxSearch_disable,
			StartMailBoxSearch_hover,
			StopAndRetrieveLog16,
			Stop_disable,
			Stop_hover,
			StartLogging16,
			Start_disable,
			Start_hover,
			StartMailBoxSearch,
			StartLogging16_disable,
			StartLogging16_hover,
			StopAndRetrieveLog16_hover,
			ToolBarDelete_hover,
			ToolBarDelete,
			ToolBarCloseWhite,
			ToolBarProperties_hover,
			ToolBarProperties,
			ToolBarDelete_disable,
			StopMailboxSearch_hover,
			StopMailboxSearch,
			StopAndRetrieveLog16_disable,
			ToolBarClose,
			StopRequest,
			StopMailboxSearch_disable,
			MailboxSearchStopping,
			DisabledArrow,
			Disable_disable,
			Disable_hover,
			DistributionGroup16,
			DistributionGroup,
			DisplayDiscoveryPassword16,
			Disable,
			CopyAdminRoleGroup16_disable,
			CopyAdminRoleGroup16_hover,
			CopyAdminRoleGroup16,
			CopyAdminRoleGroup16Dropdown_disable,
			CopyAdminRoleGroup16Dropdown_hover,
			CopyAdminRoleGroup16Dropdown,
			EASAllowUser16_disable,
			EASAllowUser16_hover,
			EASAllowUser16,
			EASBlockUser16_hover,
			AddDAGNetwork16,
			EASBlockUser16,
			DynamicDistributionGroup,
			DistributionGroupAdd_disable,
			DistributionGroupAdd_hover,
			DistributionGroupAdd,
			DistributionGroupDepart_disable,
			DistributionGroupDepart_hover,
			DistributionGroupDepart,
			BlockDevice16_disable,
			BlockDevice16_hover,
			BlockDevice16,
			CancelDeviceWipe16,
			ButtonsPanelNext16,
			ButtonsPanelBack16,
			AutodiscoverDomain_disable,
			AdvancedSearchDefault,
			AddressBook16,
			AddDAGNetwork16_hover,
			AutodiscoverDomain_hover,
			AutodiscoverDomain,
			Archive16,
			Copy,
			Contacts16,
			Contact,
			Copy16_disable,
			Copy16_hover,
			Copy16,
			ConfigCASDomain_disable,
			ChoiceButton16,
			CancelDeviceWipe16_disable,
			CancelDeviceWipe16_hover,
			ConfigCASDomain_hover,
			ConfigCASDomain,
			ClearDefault,
			EASBlockUser16_disable,
			FVAEnabled,
			ForwardEmail_disable,
			HotmailSubscription16,
			JournalRule16,
			InboxRule16,
			ForwardEmail_hover,
			MailboxSearchFailed,
			FacebookLogo,
			MailboxSearchPartiallySucceeded,
			ForwardEmail,
			Federated16,
			Legacy16,
			Mailbox16,
			Mailbox,
			MailboxSearch,
			MailboxSearch_disable,
			MailboxSearch_hover,
			ListViewRefresh_disable,
			LinkedInLogo,
			Linked16,
			ListViewRefresh,
			MailboxSearchInProgress,
			ListViewRefresh_hover,
			ExportPST_disable,
			EmailMigration16,
			Ellipsis_disable,
			Ellipsis_hover,
			Enable_hover,
			Enable,
			EmailSubscription16,
			MailboxSearchStopped,
			EditMailboxSearchPage,
			EditMailBoxSearch16,
			Ellipsis,
			ElevationReject,
			ElevationAccept,
			ExportDay16_hover,
			ExportDay16,
			ExportDay16_disable,
			ExportPST_hover,
			Enable_disable,
			ExportPST,
			EnabledArrow,
			Equipment16,
			SearchDefault_hover,
			SearchDefault,
			SearchDefault_disable,
			HelpCommand,
			PagePre,
			PageFirst,
			PageLast,
			PageNext,
			SortOrderDesc,
			SortOrderAsc,
			ToolBarDeleteSmall,
			Bullet
		}
	}
}
