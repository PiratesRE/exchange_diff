using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core.Directory
{
	public sealed class DirectoryAssistance
	{
		public static int MaxAddressBooks
		{
			get
			{
				return 2500;
			}
		}

		public static string ToHtmlString(ADObjectId id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			return Convert.ToBase64String(id.ObjectGuid.ToByteArray());
		}

		public static ADObjectId ParseADObjectId(string htmlString)
		{
			ADObjectId result;
			try
			{
				Guid guid = new Guid(Convert.FromBase64String(htmlString));
				result = new ADObjectId(guid);
			}
			catch (FormatException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
			return result;
		}

		public static AddressBookBase FindAddressBook(string base64Guid, UserContext userContext)
		{
			if (base64Guid == null)
			{
				throw new ArgumentNullException("base64Guid");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Guid guid;
			try
			{
				guid = new Guid(Convert.FromBase64String(base64Guid));
			}
			catch (FormatException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
			return DirectoryAssistance.FindAddressBook(guid, userContext);
		}

		public static AddressBookBase FindAddressBook(Guid guid, UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.GlobalAddressListInfo.Id.ObjectGuid.Equals(guid))
			{
				return userContext.GlobalAddressListInfo.ToAddressBookBase();
			}
			if (userContext.AllRoomsAddressBookInfo != null && userContext.AllRoomsAddressBookInfo.Id.ObjectGuid.Equals(guid))
			{
				return userContext.AllRoomsAddressBookInfo.ToAddressBookBase();
			}
			if (userContext.LastUsedAddressBookInfo != null && userContext.LastUsedAddressBookInfo.Id.ObjectGuid.Equals(guid))
			{
				return userContext.LastUsedAddressBookInfo.ToAddressBookBase();
			}
			if (userContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.QueryBaseDNSubTree)
			{
				throw new OwaInvalidRequestException("We cannot have sub address lists for QueryBaseDN container node.");
			}
			IConfigurationSession configurationSession = Utilities.CreateADSystemConfigurationSession(true, ConsistencyMode.IgnoreInvalid, userContext);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid);
			SortBy sortBy = new SortBy(ADObjectSchema.Name, SortOrder.Descending);
			AddressBookBase[] array = configurationSession.Find<AddressBookBase>(userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, QueryScope.SubTree, filter, sortBy, 1);
			if (array.Length == 0 || array[0] == null)
			{
				return null;
			}
			userContext.LastUsedAddressBookInfo = new AddressListInfo(array[0]);
			return array[0];
		}

		internal static AddressListInfo GetAllRoomsAddressBookInfo(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN != null)
			{
				return AddressListInfo.CreateEmpty(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			}
			AddressBookBase allRoomsAddressList = userContext.AllRoomsAddressList;
			if (allRoomsAddressList == null)
			{
				return AddressListInfo.CreateEmpty(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			}
			return new AddressListInfo(allRoomsAddressList);
		}

		public static AddressBookBase[] GetAllAddressBooks(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.DefaultGlobalAddressList)
			{
				IEnumerable<AddressBookBase> allAddressLists = userContext.AllAddressLists;
				List<AddressBookBase> list = new List<AddressBookBase>();
				foreach (AddressBookBase item in allAddressLists)
				{
					list.Add(item);
					if (list.Count >= DirectoryAssistance.MaxAddressBooks)
					{
						break;
					}
				}
				list.Sort(new DirectoryAssistance.DisplayNameComparer());
				return list.ToArray();
			}
			if (userContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.QueryBaseDNAddressList)
			{
				return new AddressBookBase[0];
			}
			return new AddressBookBase[0];
		}

		internal static GlobalAddressListInfo GetGlobalAddressListInfo(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			ADObjectId queryBaseDN = userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN;
			GlobalAddressListInfo globalAddressListInfo;
			if (queryBaseDN == null)
			{
				if (userContext.GlobalAddressList != null)
				{
					globalAddressListInfo = new GlobalAddressListInfo(userContext.GlobalAddressList, GlobalAddressListInfo.GalOrigin.DefaultGlobalAddressList);
				}
				else
				{
					globalAddressListInfo = new GlobalAddressListInfo(new AddressBookBase(), GlobalAddressListInfo.GalOrigin.EmptyGlobalAddressList);
					globalAddressListInfo.DisplayName = LocalizedStrings.GetNonEncoded(1164140307);
					globalAddressListInfo.Id = null;
				}
			}
			else
			{
				IConfigurationSession configurationSession = Utilities.CreateADSystemConfigurationSession(true, ConsistencyMode.IgnoreInvalid, userContext);
				AddressBookBase addressBookBase = configurationSession.Read<AddressBookBase>(new ADObjectId(null, queryBaseDN.ObjectGuid));
				if (addressBookBase != null)
				{
					globalAddressListInfo = new GlobalAddressListInfo(addressBookBase, GlobalAddressListInfo.GalOrigin.QueryBaseDNAddressList);
				}
				else
				{
					globalAddressListInfo = new GlobalAddressListInfo(new AddressBookBase(), GlobalAddressListInfo.GalOrigin.QueryBaseDNSubTree);
					globalAddressListInfo.DisplayName = LocalizedStrings.GetNonEncoded(1164140307);
					globalAddressListInfo.Id = queryBaseDN;
				}
			}
			return globalAddressListInfo;
		}

		public static bool IsADRecipientRoom(ADRecipient adRecipient)
		{
			if (adRecipient == null)
			{
				throw new ArgumentException("adRecipient");
			}
			return DirectoryAssistance.IsADRecipientRoom(adRecipient.RecipientDisplayType);
		}

		public static bool IsADRecipientRoom(RecipientDisplayType? recipientDisplayType)
		{
			return recipientDisplayType == RecipientDisplayType.ConferenceRoomMailbox || recipientDisplayType == RecipientDisplayType.SyncedConferenceRoomMailbox;
		}

		public static bool IsADRecipientDL(RecipientDisplayType? displayType)
		{
			return displayType != null && (displayType == RecipientDisplayType.DistributionGroup || displayType == RecipientDisplayType.DynamicDistributionGroup || displayType == RecipientDisplayType.SecurityDistributionGroup || displayType == RecipientDisplayType.PrivateDistributionList || displayType == RecipientDisplayType.SyncedDynamicDistributionGroup);
		}

		public static string GetFirstResource(MultiValuedProperty<string> resourceMetaData)
		{
			if (resourceMetaData == null)
			{
				throw new ArgumentNullException("resourceMetaData");
			}
			foreach (string text in resourceMetaData)
			{
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return string.Empty;
		}

		public static bool IsRoomsAddressListAvailable(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return userContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.DefaultGlobalAddressList;
		}

		public static bool IsVirtualAddressList(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return userContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.QueryBaseDNSubTree;
		}

		public static bool IsEmptyAddressList(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return userContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.EmptyGlobalAddressList;
		}

		private class DisplayNameComparer : IComparer<AddressBookBase>
		{
			public int Compare(AddressBookBase list1, AddressBookBase list2)
			{
				if (list1 == null)
				{
					throw new ArgumentNullException("list1");
				}
				if (list2 == null)
				{
					throw new ArgumentNullException("list2");
				}
				return string.Compare(list1.DisplayName, list2.DisplayName, true, Thread.CurrentThread.CurrentUICulture);
			}
		}
	}
}
