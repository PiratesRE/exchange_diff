using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	internal static class CannedAddressListsFilterHelper
	{
		static CannedAddressListsFilterHelper()
		{
			CannedAddressListsFilterHelper.DefaultAllModernGroupsFilter = CannedAddressListsFilterHelper.CompleteRecipientFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.GroupMailbox));
			CannedAddressListsFilterHelper.InitializeOlderLdapFilter();
			CannedAddressListsFilterHelper.InitializeRecipientFilter();
		}

		internal static Dictionary<string, QueryFilter> RecipientFiltersOfAddressList
		{
			get
			{
				return CannedAddressListsFilterHelper.recipientFiltersOfAddressList;
			}
		}

		private static Dictionary<string, string[]> LdapFiltersOfOlderAddressList
		{
			get
			{
				return CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList;
			}
		}

		private static QueryFilter CompleteRecipientFilter(QueryFilter restrictions)
		{
			return new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(ADRecipientSchema.Alias),
				restrictions
			});
		}

		private static void InitializeRecipientFilter()
		{
			CannedAddressListsFilterHelper.recipientFiltersOfAddressList = new Dictionary<string, QueryFilter>();
			CannedAddressListsFilterHelper.recipientFiltersOfAddressList.Add(CannedAddressListsFilterHelper.DefaultAllContacts, CannedAddressListsFilterHelper.DefaultAllContactsFilter);
			CannedAddressListsFilterHelper.recipientFiltersOfAddressList.Add(CannedAddressListsFilterHelper.DefaultAllDistributionLists, CannedAddressListsFilterHelper.DefaultAllDistributionListsFilter);
			CannedAddressListsFilterHelper.recipientFiltersOfAddressList.Add(CannedAddressListsFilterHelper.DefaultAllRooms, CannedAddressListsFilterHelper.DefaultAllRoomsFilter);
			CannedAddressListsFilterHelper.recipientFiltersOfAddressList.Add(CannedAddressListsFilterHelper.DefaultAllUsers, CannedAddressListsFilterHelper.DefaultAllUsersFilter);
			CannedAddressListsFilterHelper.recipientFiltersOfAddressList.Add(CannedAddressListsFilterHelper.DefaultAllModernGroups, CannedAddressListsFilterHelper.DefaultAllModernGroupsFilter);
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				CannedAddressListsFilterHelper.recipientFiltersOfAddressList.Add(CannedAddressListsFilterHelper.DefaultPublicFolders, CannedAddressListsFilterHelper.DefaultPublicFoldersFilter);
			}
		}

		private static void InitializeOlderLdapFilter()
		{
			CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList = new Dictionary<string, string[]>();
			CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList.Add(CannedAddressListsFilterHelper.DefaultAllContacts, new string[]
			{
				"(& (mailnickname=*) (| (&(objectCategory=person)(objectClass=contact)) ))"
			});
			CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList.Add(CannedAddressListsFilterHelper.DefaultAllDistributionLists, new string[]
			{
				"(& (mailnickname=*) (| (objectCategory=group) ))"
			});
			CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList.Add(CannedAddressListsFilterHelper.DefaultAllRooms, new string[]
			{
				"(& (mailnickname=*) (| (msExchResourceMetaData=ResourceType:Room) ))",
				"(&(mailnickname=*)(msExchResourceMetaData=ResourceType:Room))"
			});
			CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList.Add(CannedAddressListsFilterHelper.DefaultAllUsers, new string[]
			{
				"(&(mailnickname=*)(|(&(objectCategory=person)(objectClass=user)(!(homeMDB=*))(!(msExchHomeServerName=*)))(&(objectCategory=person)(objectClass=user)(|(homeMDB=*)(msExchHomeServerName=*)))))"
			});
			CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList.Add(CannedAddressListsFilterHelper.DefaultAllModernGroups, new string[]
			{
				"(&(!(!objectClass=user)))(objectCategory=person)(mailNickname=*)(msExchHomeServerName=*)(msExchRecipientTypeDetails=1099511627776))"
			});
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				CannedAddressListsFilterHelper.ldapFiltersOfOlderAddressList.Add(CannedAddressListsFilterHelper.DefaultPublicFolders, new string[]
				{
					"(& (mailnickname=*) (| (objectCategory=publicFolder) ))"
				});
			}
		}

		internal static Dictionary<QueryFilter, QueryFilter> GetFindFiltersForCannedAddressLists()
		{
			Dictionary<QueryFilter, QueryFilter> dictionary = new Dictionary<QueryFilter, QueryFilter>();
			foreach (KeyValuePair<string, QueryFilter> keyValuePair in CannedAddressListsFilterHelper.RecipientFiltersOfAddressList)
			{
				string key = keyValuePair.Key;
				QueryFilter value = keyValuePair.Value;
				dictionary.Add(CannedAddressListsFilterHelper.GetFindFilterForCannedAddressLists(key, value), value);
			}
			return dictionary;
		}

		internal static QueryFilter GetFindFilterForCannedAddressLists(string name, QueryFilter recipientFilter)
		{
			string propertyValue = LdapFilterBuilder.LdapFilterFromQueryFilter(recipientFilter);
			return new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AddressBookBaseSchema.LdapRecipientFilter, propertyValue),
				CannedAddressListsFilterHelper.GetFindFilterForOlderAddressList(name)
			});
		}

		private static QueryFilter GetFindFilterForOlderAddressList(string name)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			foreach (string propertyValue in CannedAddressListsFilterHelper.LdapFiltersOfOlderAddressList[name])
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, AddressBookBaseSchema.LdapRecipientFilter, propertyValue));
			}
			return new OrFilter(list.ToArray());
		}

		internal static bool IsKnownException(Exception exception)
		{
			return exception is CannotDetermineExchangeModeException;
		}

		internal static string DefaultAllContacts = Strings.DefaultAllContacts;

		internal static string DefaultAllGroups = Strings.DefaultAllGroups;

		internal static string DefaultAllRooms = Strings.DefaultAllRooms;

		internal static string DefaultAllUsers = Strings.DefaultAllUsers;

		internal static string DefaultAllModernGroups = Strings.DefaultAllModernGroups;

		internal static string DefaultPublicFolders = Strings.DefaultPublicFolders;

		internal static string DefaultAllDistributionLists = Strings.DefaultAllDistributionLists;

		internal static QueryFilter DefaultAllContactsFilter = CannedAddressListsFilterHelper.CompleteRecipientFilter(new AndFilter(new QueryFilter[]
		{
			new TextFilter(ADObjectSchema.ObjectCategory, "person", MatchOptions.FullString, MatchFlags.Default),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "contact")
		}));

		internal static QueryFilter DefaultAllDistributionListsFilter = CannedAddressListsFilterHelper.CompleteRecipientFilter(new TextFilter(ADObjectSchema.ObjectCategory, "group", MatchOptions.FullString, MatchFlags.Default));

		internal static QueryFilter DefaultAllRoomsFilter = CannedAddressListsFilterHelper.CompleteRecipientFilter(new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientDisplayType, RecipientDisplayType.ConferenceRoomMailbox),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientDisplayType, RecipientDisplayType.SyncedConferenceRoomMailbox)
		}));

		internal static QueryFilter DefaultAllUsersFilter = CannedAddressListsFilterHelper.CompleteRecipientFilter(new AndFilter(new QueryFilter[]
		{
			new OrFilter(new QueryFilter[]
			{
				new AndFilter(new QueryFilter[]
				{
					new TextFilter(ADObjectSchema.ObjectCategory, "person", MatchOptions.FullString, MatchFlags.Default),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "user"),
					new NotFilter(new ExistsFilter(IADMailStorageSchema.Database)),
					new NotFilter(new ExistsFilter(IADMailStorageSchema.ServerLegacyDN))
				}),
				new AndFilter(new QueryFilter[]
				{
					new TextFilter(ADObjectSchema.ObjectCategory, "person", MatchOptions.FullString, MatchFlags.Default),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "user"),
					new OrFilter(new QueryFilter[]
					{
						new ExistsFilter(IADMailStorageSchema.Database),
						new ExistsFilter(IADMailStorageSchema.ServerLegacyDN)
					})
				})
			}),
			new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.GroupMailbox))
		}));

		internal static QueryFilter DefaultAllModernGroupsFilter;

		internal static QueryFilter DefaultPublicFoldersFilter = CannedAddressListsFilterHelper.CompleteRecipientFilter(new TextFilter(ADObjectSchema.ObjectCategory, "publicFolder", MatchOptions.FullString, MatchFlags.Default));

		private static Dictionary<string, QueryFilter> recipientFiltersOfAddressList;

		private static Dictionary<string, string[]> ldapFiltersOfOlderAddressList;
	}
}
