using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class RetentionUtils
	{
		public static bool UserHasArchive(ReducedRecipient user)
		{
			bool result = false;
			if (user != null)
			{
				result = (user.ArchiveState == ArchiveState.Local || user.ArchiveState == ArchiveState.HostedProvisioned);
			}
			return result;
		}

		public static void PopulateRetentionTypes(DropDownList ddlRetentionType)
		{
			Type typeFromHandle = typeof(ElcFolderType);
			foreach (object obj in Enum.GetValues(typeFromHandle))
			{
				ElcFolderType elcFolderType = (ElcFolderType)obj;
				if (elcFolderType != ElcFolderType.ManagedCustomFolder && elcFolderType != ElcFolderType.NonIpmRoot && elcFolderType != ElcFolderType.LegacyArchiveJournals && elcFolderType != ElcFolderType.All && elcFolderType != ElcFolderType.Personal && elcFolderType != ElcFolderType.RecoverableItems && elcFolderType != ElcFolderType.Contacts && elcFolderType != ElcFolderType.Tasks)
				{
					string value = LocalizedDescriptionAttribute.FromEnum(typeFromHandle, elcFolderType);
					string text = elcFolderType.ToString();
					ListItem item = new ListItem(RtlUtil.ConvertToDecodedBidiString(value, RtlUtil.IsRtl), text);
					ddlRetentionType.Items.Add(item);
					if (elcFolderType == ElcFolderType.Inbox)
					{
						ddlRetentionType.SelectedValue = text;
					}
				}
			}
		}

		public static void PopulateRetentionActions(RadioButtonList rblRetentionAction, bool includeArchive)
		{
			RetentionActionType[] array = new RetentionActionType[]
			{
				RetentionActionType.DeleteAndAllowRecovery,
				RetentionActionType.PermanentlyDelete,
				RetentionActionType.MoveToArchive
			};
			foreach (RetentionActionType retentionActionType in array)
			{
				if (RetentionActionType.MoveToArchive != retentionActionType || includeArchive)
				{
					string value = string.Empty;
					switch (retentionActionType)
					{
					case RetentionActionType.DeleteAndAllowRecovery:
						value = Strings.RetentionActionEnumDeleteAndAllowRecovery;
						break;
					case RetentionActionType.PermanentlyDelete:
						value = Strings.RetentionActionEnumPermanentlyDelete;
						break;
					case RetentionActionType.MoveToArchive:
						value = Strings.RetentionActionEnumMoveToArchive;
						break;
					}
					string value2 = retentionActionType.ToString();
					ListItem item = new ListItem(RtlUtil.ConvertToDecodedBidiString(value, RtlUtil.IsRtl), value2);
					rblRetentionAction.Items.Add(item);
				}
			}
			if (rblRetentionAction.Items.Count > 0)
			{
				rblRetentionAction.Items[0].Selected = true;
			}
		}

		public static string GetLocalizedType(ElcFolderType retentionTagType)
		{
			string result = LocalizedDescriptionAttribute.FromEnum(typeof(ElcFolderType), retentionTagType);
			if (retentionTagType != ElcFolderType.All)
			{
				if (retentionTagType == ElcFolderType.Personal)
				{
					result = Strings.RetentionTagTypePersonal;
				}
			}
			else
			{
				result = Strings.RetentionTagTypeAll;
			}
			return result;
		}

		public static string GetLocalizedRetentionActionType(RetentionActionType retentionActionType)
		{
			return (retentionActionType == RetentionActionType.MoveToArchive) ? Strings.RetentionActionTypeArchive : Strings.RetentionActionTypeDelete;
		}
	}
}
