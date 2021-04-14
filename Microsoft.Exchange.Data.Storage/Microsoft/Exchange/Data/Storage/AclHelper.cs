using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AclHelper
	{
		public static bool TryGetUserFromEntryId(byte[] memberEntryId, StoreSession session, IRecipientSession recipientSession, ExternalUserCollection externalUsers, out string legacyDN, out SecurityIdentifier securityIdentifier, out List<SecurityIdentifier> sidHistory, out bool isGroup, out string displayName)
		{
			return AclHelper.TryGetUserFromEntryId(memberEntryId, session, recipientSession, new LazilyInitialized<ExternalUserCollection>(() => externalUsers), out legacyDN, out securityIdentifier, out sidHistory, out isGroup, out displayName);
		}

		public static bool TryGetUserFromEntryId(byte[] memberEntryId, StoreSession session, IRecipientSession recipientSession, LazilyInitialized<ExternalUserCollection> externalUsers, out string legacyDN, out SecurityIdentifier securityIdentifier, out List<SecurityIdentifier> sidHistory, out bool isGroup, out string displayName)
		{
			legacyDN = AclHelper.LegacyDnFromEntryId(memberEntryId);
			if (AddressBookEntryId.IsLocalDirctoryAddressBookEntryId(memberEntryId))
			{
				sidHistory = null;
				return AclHelper.ResolveLocalDirectoryUserFromAddressBookEntryId(memberEntryId, externalUsers, out securityIdentifier, out isGroup, out displayName);
			}
			return AclHelper.ResolveRecipientParametersFromLegacyDN(legacyDN, session, recipientSession, out securityIdentifier, out sidHistory, out isGroup, out displayName);
		}

		internal static string LegacyDnFromEntryId(byte[] entryId)
		{
			string result;
			Eidt eidt;
			if (entryId.Length == 0)
			{
				result = string.Empty;
			}
			else if (AddressBookEntryId.IsLocalDirctoryAddressBookEntryId(entryId))
			{
				result = AddressBookEntryId.MakeLegacyDnFromLocalDirctoryAddressBookEntryId(entryId);
				eidt = Eidt.User;
			}
			else if (!AddressBookEntryId.IsAddressBookEntryId(entryId, out eidt, out result))
			{
				throw new InvalidParamException(new LocalizedString("Invalid ABEID"));
			}
			return result;
		}

		internal static ExternalUser TryGetExternalUser(SecurityIdentifier sid, ExternalUserCollection externalUsers)
		{
			ExternalUser result = null;
			if (ExternalUser.IsExternalUserSid(sid) && externalUsers != null)
			{
				result = externalUsers.FindExternalUser(sid);
			}
			return result;
		}

		internal static bool IsNTUserLegacyDN(string legacyDN, CultureInfo cultureInfo, SecurityIdentifier securityIdentifier, out string displayName)
		{
			if (legacyDN != null && legacyDN.StartsWith("NT:", StringComparison.OrdinalIgnoreCase))
			{
				displayName = ClientStrings.NTUser.ToString(cultureInfo) + securityIdentifier.Value;
				return true;
			}
			displayName = string.Empty;
			return false;
		}

		internal static string CreateNTUserStrignRepresentation(SecurityIdentifier securityIdentifier)
		{
			return "NT:" + securityIdentifier.Value;
		}

		internal static string CreateLocalUserStrignRepresentation(SecurityIdentifier securityIdentifier)
		{
			return "Local:" + securityIdentifier.Value;
		}

		internal static bool IsGroupRecipientType(RecipientType recipientType)
		{
			return recipientType == RecipientType.Group || recipientType == RecipientType.MailUniversalDistributionGroup || recipientType == RecipientType.MailUniversalSecurityGroup || recipientType == RecipientType.MailNonUniversalGroup || recipientType == RecipientType.DynamicDistributionGroup;
		}

		private static bool ResolveRecipientParametersFromLegacyDN(string legacyDN, StoreSession session, IRecipientSession recipientSession, out SecurityIdentifier securityIdentifier, out List<SecurityIdentifier> sidHistory, out bool isGroup, out string displayName)
		{
			securityIdentifier = null;
			sidHistory = null;
			isGroup = false;
			displayName = string.Empty;
			if (legacyDN == string.Empty)
			{
				securityIdentifier = AclHelper.everyoneSecurityIdentifier;
				return true;
			}
			if (string.Compare(legacyDN, "Anonymous", StringComparison.OrdinalIgnoreCase) == 0)
			{
				securityIdentifier = AclHelper.anonymousSecurityIdentifier;
				displayName = "Anonymous";
				return true;
			}
			MiniRecipient[] array = recipientSession.FindMiniRecipient(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, MiniRecipientSchema.LegacyExchangeDN, legacyDN), null, 2, Array<PropertyDefinition>.Empty);
			if (array == null || array.Length == 0)
			{
				return false;
			}
			if (array.Length > 1)
			{
				throw new NonUniqueLegacyExchangeDNException(ServerStrings.ErrorNonUniqueLegacyDN(legacyDN));
			}
			SecurityIdentifier masterAccountSid = array[0].MasterAccountSid;
			if (masterAccountSid != null && !masterAccountSid.IsWellKnown(WellKnownSidType.SelfSid))
			{
				securityIdentifier = masterAccountSid;
			}
			else
			{
				securityIdentifier = array[0].Sid;
				MultiValuedProperty<SecurityIdentifier> sidHistory2 = array[0].SidHistory;
				if (sidHistory2 != null && sidHistory2.Count != 0)
				{
					sidHistory = new List<SecurityIdentifier>(sidHistory2);
				}
			}
			if (securityIdentifier == null)
			{
				throw new CorruptDataException(ServerStrings.UserSidNotFound(legacyDN));
			}
			isGroup = AclHelper.IsGroupRecipientType(array[0].RecipientType);
			if (!AclHelper.IsNTUserLegacyDN(legacyDN, session.InternalPreferedCulture, securityIdentifier, out displayName))
			{
				displayName = array[0].DisplayName;
			}
			return true;
		}

		private static bool ResolveLocalDirectoryUserFromAddressBookEntryId(byte[] entryId, LazilyInitialized<ExternalUserCollection> externalUsers, out SecurityIdentifier securityIdentifier, out bool isGroup, out string displayName)
		{
			securityIdentifier = null;
			isGroup = false;
			displayName = string.Empty;
			securityIdentifier = AddressBookEntryId.MakeSidFromLocalDirctoryAddressBookEntryId(entryId);
			ExternalUser externalUser = AclHelper.TryGetExternalUser(securityIdentifier, externalUsers);
			if (externalUser == null)
			{
				throw new ExternalUserNotFoundException(securityIdentifier);
			}
			displayName = externalUser.Name;
			return true;
		}

		internal const long EveryoneRowId = 0L;

		internal const long AnonymousRowId = -1L;

		internal const string AnonymousString = "Anonymous";

		private const string NTUserLegacyDNPrefix = "NT:";

		private const string LocalUserLegacyDNPrefix = "Local:";

		private static readonly SecurityIdentifier everyoneSecurityIdentifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

		private static readonly SecurityIdentifier anonymousSecurityIdentifier = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);
	}
}
