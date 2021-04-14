using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Inference.GroupingModel;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class GroupRecommendationsHelper
	{
		internal static IReadOnlyList<IRecommendedGroupInfo> GetRecommendedGroupInfos(MailboxSession mailboxSession, Action<string> traceMessageDelegate, Action<Exception> traceExceptionDelegate)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("traceMessageDelegate", traceMessageDelegate);
			ArgumentValidator.ThrowIfNull("traceExceptionDelegate", traceExceptionDelegate);
			IReadOnlyList<IRecommendedGroupInfo> result = null;
			if (GroupRecommendationsHelper.AreRecommendationsAvailable(mailboxSession, traceExceptionDelegate))
			{
				RecommendedGroupsAccessorFactory recommendedGroupsAccessorFactory = new RecommendedGroupsAccessorFactory();
				IRecommendedGroupsGetter readOnlyAccessor = recommendedGroupsAccessorFactory.GetReadOnlyAccessor();
				result = readOnlyAccessor.GetRecommendedGroups(mailboxSession, traceMessageDelegate, traceExceptionDelegate);
			}
			return result;
		}

		internal static IRecommendedGroupInfo GetLatentGroupRecommendation(MailboxSession mailboxSession, SmtpAddress smtpAddress, Action<string> traceMessageDelegate, Action<Exception> traceExceptionDelegate)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("smtpAddress", smtpAddress);
			ArgumentValidator.ThrowIfInvalidValue<SmtpAddress>("smtpAddress", smtpAddress, (SmtpAddress addr) => addr.IsValidAddress);
			ArgumentValidator.ThrowIfNull("traceMessageDelegate", traceMessageDelegate);
			ArgumentValidator.ThrowIfNull("traceExceptionDelegate", traceExceptionDelegate);
			IRecommendedGroupInfo result = null;
			string local = smtpAddress.Local;
			Guid a;
			if (Guid.TryParse(local, out a))
			{
				IReadOnlyList<IRecommendedGroupInfo> recommendedGroupInfos = GroupRecommendationsHelper.GetRecommendedGroupInfos(mailboxSession, traceMessageDelegate, traceExceptionDelegate);
				foreach (IRecommendedGroupInfo recommendedGroupInfo in recommendedGroupInfos)
				{
					if (a == recommendedGroupInfo.ID)
					{
						result = recommendedGroupInfo;
						break;
					}
				}
			}
			return result;
		}

		internal static ModernGroupType ConvertLatentGroupRecommendationToModernGroupType(IRecommendedGroupInfo recommendedGroupInfo, SmtpAddress smtpAddress)
		{
			ArgumentValidator.ThrowIfNull("recommendedGroupInfo", recommendedGroupInfo);
			ArgumentValidator.ThrowIfNull("smtpAddress", smtpAddress);
			ArgumentValidator.ThrowIfInvalidValue<SmtpAddress>("smtpAddress", smtpAddress, (SmtpAddress addr) => addr.IsValidAddress);
			return new ModernGroupType
			{
				SmtpAddress = GroupRecommendationsHelper.CreateOneOffGroupSmtpAddress(recommendedGroupInfo, smtpAddress.Domain),
				DisplayName = (recommendedGroupInfo.Words[0] ?? recommendedGroupInfo.ID.ToString()),
				IsPinned = false
			};
		}

		internal static GetModernGroupResponse ConvertLatentGroupRecommendationToModernGroupResponse(IRecommendedGroupInfo recommendedGroupInfo, SmtpAddress smtpAddress)
		{
			ArgumentValidator.ThrowIfNull("recommendedGroupInfo", recommendedGroupInfo);
			ArgumentValidator.ThrowIfInvalidValue<SmtpAddress>("smtpAddress", smtpAddress, (SmtpAddress addr) => addr.IsValidAddress);
			GetModernGroupResponse getModernGroupResponse = new GetModernGroupResponse();
			getModernGroupResponse.GeneralInfo = new ModernGroupGeneralInfoResponse
			{
				Description = string.Join(", ", recommendedGroupInfo.Words),
				IsMember = false,
				IsOwner = false,
				ModernGroupType = ModernGroupObjectType.None,
				Name = recommendedGroupInfo.Words[0],
				SmtpAddress = GroupRecommendationsHelper.CreateOneOffGroupSmtpAddress(recommendedGroupInfo, smtpAddress.Domain)
			};
			List<ModernGroupMemberType> list = new List<ModernGroupMemberType>(recommendedGroupInfo.Members.Count);
			for (int i = 0; i < recommendedGroupInfo.Members.Count; i++)
			{
				list.Add(new ModernGroupMemberType
				{
					Persona = GroupRecommendationsHelper.ConvertRecommendationMemberToPersona(recommendedGroupInfo.Members[i], null),
					IsOwner = false
				});
			}
			getModernGroupResponse.MembersInfo = new ModernGroupMembersResponse
			{
				Members = list.ToArray(),
				Count = list.Count
			};
			return getModernGroupResponse;
		}

		private static Persona ConvertRecommendationMemberToPersona(string displayName, string smtpAddress)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("displayName", displayName);
			bool flag = string.IsNullOrEmpty(smtpAddress);
			return new Persona
			{
				DisplayName = displayName,
				EmailAddress = new EmailAddressWrapper
				{
					Name = displayName,
					RoutingType = (flag ? null : "SMTP"),
					MailboxType = (flag ? MailboxHelper.MailboxTypeType.OneOff.ToString() : MailboxHelper.MailboxTypeType.Contact.ToString())
				}
			};
		}

		private static string CreateOneOffGroupSmtpAddress(IRecommendedGroupInfo info, string smtpDomain)
		{
			SmtpAddress smtpAddress = new SmtpAddress(info.ID.ToString(), smtpDomain);
			return smtpAddress.ToString();
		}

		private static bool AreRecommendationsAvailable(MailboxSession mailboxSession, Action<Exception> traceExceptionDelegate)
		{
			bool result = false;
			try
			{
				using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox), GroupRecommendationsHelper.InferenceSettingsConfigurationName, UserConfigurationTypes.Dictionary, false, false))
				{
					if (folderConfiguration != null)
					{
						IDictionary dictionary = folderConfiguration.GetDictionary();
						if (dictionary != null && dictionary.Contains(GroupRecommendationsHelper.GroupsRecommendationReadyName))
						{
							return (bool)dictionary[GroupRecommendationsHelper.GroupsRecommendationReadyName];
						}
					}
				}
			}
			catch (Exception obj)
			{
				traceExceptionDelegate(obj);
				result = false;
			}
			return result;
		}

		internal const int RecommendationsToReturn = 3;

		internal static string InferenceSettingsConfigurationName = "Inference.Settings";

		internal static string GroupsRecommendationReadyName = "IsInferenceGroupsRecommendationReady";
	}
}
