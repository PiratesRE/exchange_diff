using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class DelegateUtilities
	{
		internal static DelegateUserResponseMessageType[] BuildDelegateResponseFromResults(ServiceResult<DelegateUserType>[] results)
		{
			List<DelegateUserResponseMessageType> list = new List<DelegateUserResponseMessageType>();
			ServiceResult<DelegateUserType>.ProcessServiceResults(results, null);
			foreach (ServiceResult<DelegateUserType> serviceResult in results)
			{
				DelegateUserResponseMessageType item = new DelegateUserResponseMessageType(serviceResult.Code, serviceResult.Error, serviceResult.Value);
				list.Add(item);
			}
			return list.ToArray();
		}

		internal static DeliverMeetingRequestsType ConvertToDeliverMeetingRequestType(DelegateRuleType delegateRuleType)
		{
			switch (delegateRuleType)
			{
			case DelegateRuleType.ForwardAndSetAsInformationalUpdate:
				return DeliverMeetingRequestsType.DelegatesAndSendInformationToMe;
			case DelegateRuleType.Forward:
				return DeliverMeetingRequestsType.DelegatesAndMe;
			case DelegateRuleType.ForwardAndDelete:
				return DeliverMeetingRequestsType.DelegatesOnly;
			case DelegateRuleType.NoForward:
				if (ExchangeVersion.Current.Supports(ExchangeVersionType.Exchange2010_SP1))
				{
					return DeliverMeetingRequestsType.NoForward;
				}
				return DeliverMeetingRequestsType.None;
			default:
				ExTraceGlobals.GetDelegateCallTracer.TraceDebug<DelegateRuleType>(0L, "[DelegateUtilities::ConvertToDeliverMeetingRequestType] Unknown DelegateRuleType encountered: {0}", delegateRuleType);
				return DeliverMeetingRequestsType.None;
			}
		}

		internal static DelegateUser GetDelegateUser(UserId user, DelegateUserCollection delegateUsers, ADRecipientSessionContext adRecipientSessionContext)
		{
			ExchangePrincipal exchangePrincipal = DelegateUtilities.GetExchangePrincipal(user, adRecipientSessionContext);
			foreach (DelegateUser delegateUser in delegateUsers)
			{
				if (delegateUser.Delegate != null && exchangePrincipal.LegacyDn.Equals(delegateUser.Delegate.LegacyDn, StringComparison.OrdinalIgnoreCase))
				{
					return delegateUser;
				}
			}
			throw new DelegateExceptionNotDelegate();
		}

		internal static DelegateUser GetDelegateUser(ADRecipient adRecipient, DelegateUserCollection delegateUsers)
		{
			if (adRecipient != null)
			{
				SmtpAddress primarySmtpAddress = adRecipient.PrimarySmtpAddress;
				string strB = adRecipient.PrimarySmtpAddress.ToString();
				foreach (DelegateUser delegateUser in delegateUsers)
				{
					if (delegateUser.PrimarySmtpAddress != null)
					{
						if (string.Compare(delegateUser.PrimarySmtpAddress, strB, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return delegateUser;
						}
						foreach (ProxyAddress proxyAddress in adRecipient.EmailAddresses)
						{
							if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp && string.Compare(delegateUser.PrimarySmtpAddress, proxyAddress.AddressString, StringComparison.OrdinalIgnoreCase) == 0)
							{
								return delegateUser;
							}
						}
					}
				}
			}
			return null;
		}

		private static UserId BuildUserIdFromExchangePrincipal(IExchangePrincipal delegatePrincipal)
		{
			UserId userId = new UserId();
			userId.DisplayName = delegatePrincipal.MailboxInfo.DisplayName;
			userId.PrimarySmtpAddress = delegatePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			if (delegatePrincipal.Sid != null)
			{
				userId.Sid = delegatePrincipal.Sid.ToString();
			}
			return userId;
		}

		internal static DelegateRuleType ConvertToDeliverRuleType(DeliverMeetingRequestsType deliverMeetingRequest)
		{
			switch (deliverMeetingRequest)
			{
			case DeliverMeetingRequestsType.DelegatesOnly:
				return DelegateRuleType.ForwardAndDelete;
			case DeliverMeetingRequestsType.DelegatesAndMe:
				return DelegateRuleType.Forward;
			case DeliverMeetingRequestsType.DelegatesAndSendInformationToMe:
				return DelegateRuleType.ForwardAndSetAsInformationalUpdate;
			case DeliverMeetingRequestsType.NoForward:
				return DelegateRuleType.NoForward;
			default:
				return DelegateRuleType.Forward;
			}
		}

		internal static DeliverMeetingRequestsType ConvertToMeetingRequestsType(DelegateRuleType delegateRuleType)
		{
			switch (delegateRuleType)
			{
			case DelegateRuleType.ForwardAndSetAsInformationalUpdate:
				return DeliverMeetingRequestsType.DelegatesAndSendInformationToMe;
			case DelegateRuleType.Forward:
				return DeliverMeetingRequestsType.DelegatesAndMe;
			case DelegateRuleType.ForwardAndDelete:
				return DeliverMeetingRequestsType.DelegatesOnly;
			case DelegateRuleType.NoForward:
				return DeliverMeetingRequestsType.NoForward;
			default:
				return DeliverMeetingRequestsType.DelegatesAndMe;
			}
		}

		internal static ExchangePrincipal GetExchangePrincipal(UserId userId, ADRecipientSessionContext adRecipientSessionContext)
		{
			if (userId.DistinguishedUser != DistinguishedUserType.None)
			{
				throw new DistinguishedUserNotSupportedException();
			}
			ExchangePrincipal exchangePrincipal = null;
			if (!string.IsNullOrEmpty(userId.PrimarySmtpAddress))
			{
				try
				{
					exchangePrincipal = ExchangePrincipalCache.GetFromCache(userId.PrimarySmtpAddress, adRecipientSessionContext);
					goto IL_7E;
				}
				catch (NonExistentMailboxException innerException)
				{
					throw new DelegateExceptionInvalidDelegateUser(CoreResources.IDs.ErrorDelegateNoUser, innerException);
				}
			}
			if (string.IsNullOrEmpty(userId.Sid))
			{
				throw new MissingUserIdInformationException();
			}
			SecurityIdentifier sid = null;
			try
			{
				sid = new SecurityIdentifier(userId.Sid);
			}
			catch (ArgumentException innerException2)
			{
				throw new InvalidUserSidException(innerException2);
			}
			if (!ExchangePrincipalCache.TryGetFromCache(sid, CallContext.Current.ADRecipientSessionContext, out exchangePrincipal))
			{
				throw new InvalidUserSidException();
			}
			IL_7E:
			if (exchangePrincipal != null)
			{
				DelegateUtilities.ValidateUserId(userId, exchangePrincipal);
				return exchangePrincipal;
			}
			string message = string.Format("Cannot find userPrincipal for {0} in ExchangePrincipalCache!", userId.PrimarySmtpAddress);
			ExTraceGlobals.GetDelegateCallTracer.TraceError(0L, message);
			throw new InvalidDelegateUserIdException(message);
		}

		private static void ValidateUserId(UserId userId, ExchangePrincipal userPrincipal)
		{
			UserId userId2 = DelegateUtilities.BuildUserIdFromExchangePrincipal(userPrincipal);
			if ((!string.IsNullOrEmpty(userId.Sid) && !userId.Sid.Equals(userId2.Sid, StringComparison.OrdinalIgnoreCase)) || (!string.IsNullOrEmpty(userId.PrimarySmtpAddress) && !userId.PrimarySmtpAddress.Equals(userId2.PrimarySmtpAddress, StringComparison.OrdinalIgnoreCase)) || (!string.IsNullOrEmpty(userId.DisplayName) && !userId.DisplayName.Equals(userId2.DisplayName, StringComparison.OrdinalIgnoreCase)))
			{
				string text = string.Format("UserId.SID = {0}, userPrincipal.SID = {1} : UserId.PrimarySmtpAddress = {2}, userPrincipal.PrimarySmtpAddress = {3} : UserId.DisplayName = {4}, userPrincipal.DisplayName = {5}", new object[]
				{
					userId.Sid,
					userId2.Sid,
					userId.PrimarySmtpAddress,
					userId2.PrimarySmtpAddress,
					userId.DisplayName,
					userId2.DisplayName
				});
				ExTraceGlobals.GetDelegateCallTracer.TraceError(0L, "UserId property values SID, PrimarySmtpAddress and DisplayName should match with the corresponding userPrincipal's property values if they are not null! " + text);
				throw new InvalidDelegateUserIdException(text);
			}
		}

		internal static DelegateUserType BuildDelegateUserResponse(DelegateUser delegateUser, bool includePermissions)
		{
			DelegateUserType delegateUserType = new DelegateUserType();
			delegateUserType.UserId = DelegateUtilities.BuildUserIdFromExchangePrincipal(delegateUser.Delegate);
			if (includePermissions && delegateUser.FolderPermissions != null)
			{
				delegateUserType.DelegatePermissions = DelegateUtilities.ConvertXsoFolderPermissionsToDelegatePermissions(delegateUser);
			}
			delegateUserType.ViewPrivateItems = new bool?(delegateUser.CanViewPrivateItems);
			delegateUserType.ReceiveCopiesOfMeetingMessages = new bool?(delegateUser.ReceivesMeetingMessageCopies);
			return delegateUserType;
		}

		private static DelegatePermissionsType ConvertXsoFolderPermissionsToDelegatePermissions(DelegateUser delegateUser)
		{
			IDictionary<DefaultFolderType, PermissionLevel> folderPermissions = delegateUser.FolderPermissions;
			DelegatePermissionsType delegatePermissionsType = new DelegatePermissionsType();
			foreach (DefaultFolderType defaultFolderType in folderPermissions.Keys)
			{
				PermissionLevel permissionLevel = folderPermissions[defaultFolderType];
				DelegateFolderPermissionLevelType delegateFolderPermissionLevelType = DelegateUtilities.ConvertToDelegateFolderPermissionType(permissionLevel);
				switch (defaultFolderType)
				{
				case DefaultFolderType.Calendar:
					delegatePermissionsType.CalendarFolderPermissionLevel = delegateFolderPermissionLevelType;
					continue;
				case DefaultFolderType.Contacts:
					delegatePermissionsType.ContactsFolderPermissionLevel = delegateFolderPermissionLevelType;
					continue;
				case DefaultFolderType.Inbox:
					delegatePermissionsType.InboxFolderPermissionLevel = delegateFolderPermissionLevelType;
					continue;
				case DefaultFolderType.Journal:
					delegatePermissionsType.JournalFolderPermissionLevel = delegateFolderPermissionLevelType;
					continue;
				case DefaultFolderType.Notes:
					delegatePermissionsType.NotesFolderPermissionLevel = delegateFolderPermissionLevelType;
					continue;
				case DefaultFolderType.Tasks:
					delegatePermissionsType.TasksFolderPermissionLevel = delegateFolderPermissionLevelType;
					continue;
				}
				ExTraceGlobals.GetDelegateCallTracer.TraceDebug<string>(0L, "Invalid DefaultFolderType : {0}", defaultFolderType.ToString());
			}
			return delegatePermissionsType;
		}

		internal static PermissionLevel ConvertToXsoPermissionLevel(DelegateFolderPermissionLevelType delegateFolderPermissionLevelType)
		{
			switch (delegateFolderPermissionLevelType)
			{
			case DelegateFolderPermissionLevelType.None:
				return PermissionLevel.None;
			case DelegateFolderPermissionLevelType.Editor:
				return PermissionLevel.Editor;
			case DelegateFolderPermissionLevelType.Reviewer:
				return PermissionLevel.Reviewer;
			case DelegateFolderPermissionLevelType.Author:
				return PermissionLevel.Author;
			default:
				throw new InvalidDelegatePermissionException();
			}
		}

		private static DelegateFolderPermissionLevelType ConvertToDelegateFolderPermissionType(PermissionLevel permissionLevel)
		{
			if (permissionLevel != PermissionLevel.None)
			{
				switch (permissionLevel)
				{
				case PermissionLevel.Editor:
					return DelegateFolderPermissionLevelType.Editor;
				case PermissionLevel.Author:
					return DelegateFolderPermissionLevelType.Author;
				case PermissionLevel.Reviewer:
					return DelegateFolderPermissionLevelType.Reviewer;
				case PermissionLevel.Custom:
					return DelegateFolderPermissionLevelType.Custom;
				}
				ExTraceGlobals.GetDelegateCallTracer.TraceDebug<string>(0L, "Invalid permission level: {0}", permissionLevel.ToString());
				return DelegateFolderPermissionLevelType.Custom;
			}
			return DelegateFolderPermissionLevelType.None;
		}

		internal static void UpdateXsoDelegateUser(DelegateUser xsoDelegateUser, DelegateUserType delegateUserType)
		{
			if (delegateUserType.DelegatePermissions != null)
			{
				DelegateUtilities.UpdateFolderPermissions(xsoDelegateUser.FolderPermissions, delegateUserType.DelegatePermissions);
			}
			if (delegateUserType.ReceiveCopiesOfMeetingMessages != null)
			{
				xsoDelegateUser.ReceivesMeetingMessageCopies = delegateUserType.ReceiveCopiesOfMeetingMessages.Value;
			}
			if (delegateUserType.ViewPrivateItems != null)
			{
				xsoDelegateUser.CanViewPrivateItems = delegateUserType.ViewPrivateItems.Value;
			}
			xsoDelegateUser.Validate();
		}

		private static void UpdateFolderPermissions(IDictionary<DefaultFolderType, PermissionLevel> folderPermissions, DelegatePermissionsType delegatePermissions)
		{
			if (delegatePermissions.InboxFolderPermissionLevel != DelegateFolderPermissionLevelType.Default)
			{
				folderPermissions[DefaultFolderType.Inbox] = DelegateUtilities.ConvertToXsoPermissionLevel(delegatePermissions.InboxFolderPermissionLevel);
			}
			if (delegatePermissions.JournalFolderPermissionLevel != DelegateFolderPermissionLevelType.Default)
			{
				folderPermissions[DefaultFolderType.Journal] = DelegateUtilities.ConvertToXsoPermissionLevel(delegatePermissions.JournalFolderPermissionLevel);
			}
			if (delegatePermissions.NotesFolderPermissionLevel != DelegateFolderPermissionLevelType.Default)
			{
				folderPermissions[DefaultFolderType.Notes] = DelegateUtilities.ConvertToXsoPermissionLevel(delegatePermissions.NotesFolderPermissionLevel);
			}
			if (delegatePermissions.TasksFolderPermissionLevel != DelegateFolderPermissionLevelType.Default)
			{
				folderPermissions[DefaultFolderType.Tasks] = DelegateUtilities.ConvertToXsoPermissionLevel(delegatePermissions.TasksFolderPermissionLevel);
			}
			if (delegatePermissions.CalendarFolderPermissionLevel != DelegateFolderPermissionLevelType.Default)
			{
				folderPermissions[DefaultFolderType.Calendar] = DelegateUtilities.ConvertToXsoPermissionLevel(delegatePermissions.CalendarFolderPermissionLevel);
			}
			if (delegatePermissions.ContactsFolderPermissionLevel != DelegateFolderPermissionLevelType.Default)
			{
				folderPermissions[DefaultFolderType.Contacts] = DelegateUtilities.ConvertToXsoPermissionLevel(delegatePermissions.ContactsFolderPermissionLevel);
			}
		}

		internal static string BuildErrorStringFromProblems(ICollection<KeyValuePair<DelegateSaveState, Exception>> problems)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<DelegateSaveState, Exception> keyValuePair in problems)
			{
				stringBuilder.AppendFormat("Save State {0} : Reason {1} \n", keyValuePair.Key.ToString(), keyValuePair.Value.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
