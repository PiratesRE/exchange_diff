using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CalendarLoggingHelper
	{
		internal static ICollection<PropertyDefinition> RequiredOriginalProperties
		{
			get
			{
				return CalendarLoggingHelper.requiredOriginalProperties;
			}
		}

		internal static bool ShouldLogInitialCheck(StoreObjectId itemId, COWTriggerAction operation)
		{
			Util.ThrowOnNullArgument(itemId, "itemId");
			return CalendarLoggingHelper.ShouldLog(operation) && (itemId.ObjectType == StoreObjectType.Unknown || CalendarLoggingHelper.IsCalendarItem(itemId));
		}

		internal static bool ShouldBeCopiedOnWrite(StoreObjectId id)
		{
			return id != null && id.ObjectType != StoreObjectType.CalendarItemOccurrence;
		}

		public static bool IsCalendarItem(StoreObjectId itemId)
		{
			StoreObjectType objectType = itemId.ObjectType;
			if (objectType <= StoreObjectType.MeetingForwardNotification)
			{
				switch (objectType)
				{
				case StoreObjectType.MeetingMessage:
				case StoreObjectType.MeetingRequest:
				case StoreObjectType.MeetingResponse:
				case StoreObjectType.MeetingCancellation:
				case StoreObjectType.CalendarItem:
				case StoreObjectType.CalendarItemOccurrence:
					break;
				case StoreObjectType.ConflictMessage:
					return false;
				default:
					if (objectType != StoreObjectType.MeetingForwardNotification)
					{
						return false;
					}
					break;
				}
			}
			else if (objectType != StoreObjectType.MeetingInquiryMessage && objectType != StoreObjectType.CalendarItemSeries)
			{
				switch (objectType)
				{
				case StoreObjectType.MeetingRequestSeries:
				case StoreObjectType.MeetingResponseSeries:
				case StoreObjectType.MeetingCancellationSeries:
				case StoreObjectType.MeetingForwardNotificationSeries:
				case StoreObjectType.ParkedMeetingMessage:
					break;
				default:
					return false;
				}
			}
			return true;
		}

		public static string GetCalendarPermissionsLog(MailboxSession mailboxSession, Folder folder)
		{
			if (mailboxSession == null || folder == null)
			{
				return string.Empty;
			}
			SecurityIdentifier arg = IdentityHelper.SidFromLogonIdentity(mailboxSession.Identity);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("Folder Name: {0} Security Descriptor: {1}\n", folder.DisplayName, arg));
			PermissionSet permissionSet = folder.GetPermissionSet();
			if (permissionSet.AnonymousPermission != null)
			{
				PermissionLevel permissionLevel = permissionSet.AnonymousPermission.PermissionLevel;
				MemberRights memberRights = permissionSet.AnonymousPermission.MemberRights;
				stringBuilder.Append(string.Format("Anonymous Permission:{0} Member Rights:{1}", permissionLevel, memberRights));
			}
			if (permissionSet.DefaultPermission != null)
			{
				PermissionLevel permissionLevel2 = permissionSet.DefaultPermission.PermissionLevel;
				MemberRights memberRights2 = permissionSet.DefaultPermission.MemberRights;
				stringBuilder.Append(string.Format("\nDefault Permission:{0} Member Rights:{1}", permissionLevel2, memberRights2));
			}
			foreach (Permission permission in permissionSet)
			{
				MemberRights memberRights3 = permission.MemberRights;
				PermissionLevel permissionLevel3 = permission.PermissionLevel;
				if (permission.Principal != null)
				{
					string text = permission.Principal.ToString();
					string text2 = permission.Principal.Type.ToString();
					if (permission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal)
					{
						if (permission.Principal.ADRecipient != null)
						{
							SmtpAddress primarySmtpAddress = permission.Principal.ADRecipient.PrimarySmtpAddress;
							text = permission.Principal.ADRecipient.PrimarySmtpAddress.ToString();
						}
					}
					else if (permission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.SpecialPrincipal)
					{
						text2 = permission.Principal.SpecialType.ToString();
					}
					stringBuilder.Append(string.Format("\nPermission:{0} UserType:{1} User:{2} Member Rights:{3}", new object[]
					{
						permissionLevel3,
						text2,
						text,
						memberRights3
					}));
				}
				else
				{
					stringBuilder.Append(string.Format("\nPermission:{0} UserType:'Unknown' User:'Unknown' Member Rights:{1}", permissionLevel3, memberRights3));
				}
			}
			return stringBuilder.ToString();
		}

		public static bool ShouldLog(COWTriggerAction operation)
		{
			switch (operation)
			{
			case COWTriggerAction.Create:
			case COWTriggerAction.Update:
				return true;
			case COWTriggerAction.ItemBind:
			case COWTriggerAction.Submit:
			case COWTriggerAction.FolderBind:
				return false;
			case COWTriggerAction.Copy:
				return false;
			case COWTriggerAction.Move:
			case COWTriggerAction.MoveToDeletedItems:
			case COWTriggerAction.SoftDelete:
			case COWTriggerAction.HardDelete:
			case COWTriggerAction.DoneWithMessageDelete:
				return true;
			default:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The specified item change operation ('{0}') is not supported by Calendar Logging.", new object[]
				{
					operation
				}));
			}
		}

		internal static bool ShouldLog(ICoreItem item, COWTriggerAction action)
		{
			return item != null && ((item.StoreObjectId != null || action == COWTriggerAction.Create) && (item.IsDirty || action != COWTriggerAction.Update) && CalendarLoggingHelper.ShouldLog(action)) && CalendarLoggingHelper.IsCalendarItem(item);
		}

		internal static void AddMetadata(ICoreItem item, COWTriggerAction action, FolderChangeOperationFlags? folderChangeOperationFlags)
		{
			Dictionary<PropertyDefinition, object> metadata = CalendarLoggingHelper.GetMetadata(item, action, folderChangeOperationFlags);
			foreach (PropertyDefinition propertyDefinition in metadata.Keys)
			{
				item.PropertyBag[propertyDefinition] = metadata[propertyDefinition];
			}
		}

		private static Dictionary<PropertyDefinition, object> GetMetadata(ICoreItem item, COWTriggerAction action, FolderChangeOperationFlags? folderChangeOperationFlags)
		{
			Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>(6);
			item.PropertyBag.Load(CalendarLoggingHelper.RequiredOriginalProperties);
			switch (action)
			{
			case COWTriggerAction.MoveToDeletedItems:
			case COWTriggerAction.SoftDelete:
			case COWTriggerAction.HardDelete:
				if (folderChangeOperationFlags != null)
				{
					dictionary.Add(InternalSchema.ClientIntent, CalendarLoggingHelper.GetClientIntentFromFolderChangeOperationFlags(folderChangeOperationFlags.Value, item.PropertyBag.GetValueOrDefault<string>(InternalSchema.CalendarLogTriggerAction, string.Empty), item.PropertyBag.GetValueOrDefault<ClientIntentFlags>(InternalSchema.ClientIntent, ClientIntentFlags.None)));
				}
				break;
			}
			dictionary.Add(InternalSchema.OriginalFolderId, item.PropertyBag.GetValueOrDefault<object>(InternalSchema.ParentEntryId, Array<byte>.Empty));
			dictionary.Add(InternalSchema.OriginalCreationTime, item.PropertyBag.GetValueOrDefault<object>(InternalSchema.CreationTime, ExDateTime.MinValue));
			dictionary.Add(InternalSchema.OriginalEntryId, item.PropertyBag.GetValueOrDefault<object>(InternalSchema.EntryId, Array<byte>.Empty));
			dictionary.Add(InternalSchema.ItemVersion, item.PropertyBag.GetValueOrDefault<int>(InternalSchema.ItemVersion, 0) + 1);
			dictionary.Add(InternalSchema.ChangeList, item.LocationIdentifierHelperInstance.ChangeBuffer);
			return dictionary;
		}

		private static ClientIntentFlags GetClientIntentFromFolderChangeOperationFlags(FolderChangeOperationFlags flags, string previousAction, ClientIntentFlags carryOverIntent)
		{
			ClientIntentFlags result;
			if ((flags & FolderChangeOperationFlags.DeclineCalendarItemWithoutResponse) == FolderChangeOperationFlags.DeclineCalendarItemWithoutResponse)
			{
				result = ClientIntentFlags.DeletedWithNoResponse;
			}
			else if ((flags & FolderChangeOperationFlags.DeclineCalendarItemWithResponse) == FolderChangeOperationFlags.DeclineCalendarItemWithResponse)
			{
				result = ClientIntentFlags.RespondedDecline;
			}
			else if ((flags & FolderChangeOperationFlags.CancelCalendarItem) == FolderChangeOperationFlags.CancelCalendarItem)
			{
				result = ClientIntentFlags.MeetingCanceled;
			}
			else
			{
				result = ((CalendarLoggingHelper.updateActionString == previousAction) ? carryOverIntent : ClientIntentFlags.None);
			}
			return result;
		}

		private static bool IsCalendarItem(ICoreItem item)
		{
			string valueOrDefault = item.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			return ObjectClass.IsCalendarItem(valueOrDefault) || ObjectClass.IsMeetingMessage(valueOrDefault) || ObjectClass.IsRecurrenceException(valueOrDefault) || ObjectClass.IsMeetingInquiry(valueOrDefault) || ObjectClass.IsCalendarItemSeries(valueOrDefault) || ObjectClass.IsMeetingMessageSeries(valueOrDefault) || ObjectClass.IsParkedMeetingMessage(valueOrDefault);
		}

		private static readonly ICollection<PropertyDefinition> requiredOriginalProperties = new PropertyDefinition[]
		{
			InternalSchema.ParentEntryId,
			InternalSchema.CreationTime,
			InternalSchema.ItemClass,
			InternalSchema.HasBeenSubmitted,
			InternalSchema.ItemVersion,
			InternalSchema.CalendarLogTriggerAction,
			InternalSchema.ClientIntent
		};

		private static string updateActionString = COWTriggerAction.Update.ToString();
	}
}
