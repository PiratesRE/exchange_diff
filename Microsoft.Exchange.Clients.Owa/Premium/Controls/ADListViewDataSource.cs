using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ADListViewDataSource : ExchangeListViewDataSource, IListViewDataSource
	{
		private static Dictionary<RecipientType, string> LoadItemClasses()
		{
			Dictionary<RecipientType, string> dictionary = new Dictionary<RecipientType, string>();
			dictionary[RecipientType.Invalid] = "AD.RecipientType.Invalid";
			dictionary[RecipientType.User] = "AD.RecipientType.User";
			dictionary[RecipientType.UserMailbox] = "AD.RecipientType.MailboxUser";
			dictionary[RecipientType.MailUser] = "AD.RecipientType.MailEnabledUser";
			dictionary[RecipientType.Contact] = "AD.RecipientType.Contact";
			dictionary[RecipientType.MailContact] = "AD.RecipientType.MailEnabledContact";
			dictionary[RecipientType.Group] = "AD.RecipientType.Group";
			dictionary[RecipientType.MailUniversalDistributionGroup] = "AD.RecipientType.MailEnabledUniversalDistributionGroup";
			dictionary[RecipientType.MailUniversalSecurityGroup] = "AD.RecipientType.MailEnabledUniversalSecurityGroup";
			dictionary[RecipientType.MailNonUniversalGroup] = "AD.RecipientType.MailEnabledNonUniversalGroup";
			dictionary[RecipientType.DynamicDistributionGroup] = "AD.RecipientType.DynamicDL";
			dictionary[RecipientType.PublicFolder] = "AD.RecipientType.PublicFolder";
			dictionary[RecipientType.PublicDatabase] = "AD.RecipientType.PublicDatabase";
			dictionary[RecipientType.SystemAttendantMailbox] = "AD.RecipientType.SystemAttendantMailbox";
			return dictionary;
		}

		public static ADListViewDataSource CreateForBrowse(Hashtable properties, AddressBookBase addressBookBase, UserContext userContext)
		{
			return ADListViewDataSource.CreateForBrowse(properties, addressBookBase, null, Culture.GetUserCulture().LCID, null, userContext);
		}

		public static ADListViewDataSource CreateForBrowse(Hashtable properties, AddressBookBase addressBookBase, string cookie, int lcid, string preferredDC, UserContext userContext)
		{
			return new ADListViewDataSource(properties, addressBookBase, cookie, lcid, preferredDC, userContext);
		}

		public static ADListViewDataSource CreateForSearch(Hashtable properties, AddressBookBase addressBookBase, string searchString, UserContext userContext)
		{
			return ADListViewDataSource.CreateForSearch(properties, addressBookBase, searchString, null, 0, Culture.GetUserCulture().LCID, null, userContext);
		}

		public static ADListViewDataSource CreateForSearch(Hashtable properties, AddressBookBase addressBookBase, string searchString, string cookie, int cookieIndex, int lcid, string preferredDC, UserContext userContext)
		{
			return new ADListViewDataSource(properties, addressBookBase, searchString, cookie, cookieIndex, lcid, preferredDC, userContext);
		}

		private ADListViewDataSource(Hashtable properties, AddressBookBase addressBookBase, string cookie, int lcid, string preferredDC, UserContext userContext) : this(properties, addressBookBase, null, cookie, 0, lcid, preferredDC, userContext)
		{
		}

		private ADListViewDataSource(Hashtable properties, AddressBookBase addressBookBase, string searchString, string cookie, int cookieIndex, int lcid, string preferredDC, UserContext userContext) : base(properties)
		{
			if (!properties.ContainsKey(ADObjectSchema.ObjectCategory) || !properties.ContainsKey(ADObjectSchema.Guid) || !properties.ContainsKey(ADRecipientSchema.RecipientType) || !properties.ContainsKey(ADRecipientSchema.RecipientDisplayType))
			{
				throw new ArgumentException("The objectCategory, objectGuid, recipientType attributes need to be included in the'properties' parameter of the ADListViewDataSource constructor");
			}
			this.addressBookBase = addressBookBase;
			this.searchString = searchString;
			this.cookie = cookie;
			this.cookieIndex = cookieIndex;
			this.lcid = lcid;
			this.preferredDC = preferredDC;
			this.userContext = userContext;
			if (!string.IsNullOrEmpty(searchString))
			{
				this.search = true;
			}
			try
			{
				userContext.GetCachedADCount(this.ContainerId, this.search ? searchString : string.Empty);
			}
			catch (Exception)
			{
				this.Load(2147483597, 50, true, false);
				userContext.SetCachedADCount(this.ContainerId, this.search ? searchString : string.Empty, (base.EndRange >= 0) ? (base.EndRange + 1) : 0);
			}
		}

		public string Cookie
		{
			get
			{
				if (this.cookie == null)
				{
					return string.Empty;
				}
				return this.cookie;
			}
		}

		public int Lcid
		{
			get
			{
				if (this.cookie == null)
				{
					return Culture.GetUserCulture().LCID;
				}
				return this.lcid;
			}
		}

		public string PreferredDC
		{
			get
			{
				if (this.preferredDC == null)
				{
					return string.Empty;
				}
				return this.preferredDC;
			}
		}

		public override int TotalCount
		{
			get
			{
				return this.userContext.GetCachedADCount(this.ContainerId, this.search ? this.searchString : string.Empty);
			}
		}

		public int UnreadCount
		{
			get
			{
				return 0;
			}
		}

		public string ContainerId
		{
			get
			{
				return this.addressBookBase.Base64Guid;
			}
		}

		public bool UserHasRightToLoad
		{
			get
			{
				return true;
			}
		}

		public string GetItemId()
		{
			return DirectoryAssistance.ToHtmlString(new ADObjectId(null, this.GetItemProperty<Guid>(ADObjectSchema.Guid, Guid.Empty)));
		}

		public string GetItemClass()
		{
			RecipientDisplayType? itemProperty = this.GetItemProperty<RecipientDisplayType?>(ADRecipientSchema.RecipientDisplayType, null);
			if (itemProperty == RecipientDisplayType.ConferenceRoomMailbox || itemProperty == RecipientDisplayType.SyncedConferenceRoomMailbox)
			{
				return "AD.ResourceType.Room";
			}
			RecipientType itemProperty2 = this.GetItemProperty<RecipientType>(ADRecipientSchema.RecipientType, RecipientType.Invalid);
			string result;
			if (ADListViewDataSource.itemClasses.TryGetValue(itemProperty2, out result))
			{
				return result;
			}
			return ADListViewDataSource.itemClasses[RecipientType.Invalid];
		}

		public void Load(ObjectId seekToItemId, SeekDirection seekDirection, int itemCount)
		{
			throw new NotImplementedException();
		}

		public void Load(string seekValue, int itemCount)
		{
			if (this.search)
			{
				throw new ArgumentException("Can't seek and search at the same time");
			}
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			this.searchString = seekValue;
			this.LoadBrowsePage(0, itemCount, true, requestedProperties);
		}

		public void Load(int startRange, int itemCount)
		{
			this.Load(startRange, itemCount, true, true);
		}

		private void Load(int startRange, int itemCount, bool retry, bool fetchingPhase)
		{
			if (startRange < 0)
			{
				throw new ArgumentException("startRange must be >= 0");
			}
			if (itemCount <= 0)
			{
				throw new ArgumentException("itemCount must be > 0");
			}
			PropertyDefinition[] properties;
			if (fetchingPhase)
			{
				properties = base.GetRequestedProperties();
			}
			else
			{
				properties = new PropertyDefinition[]
				{
					ADObjectSchema.ObjectCategory
				};
			}
			try
			{
				if (this.search)
				{
					if (this.cookieIndex < 0 || this.cookieIndex >= startRange)
					{
						this.cookie = null;
						this.cookieIndex = 0;
					}
					this.LoadPagedSearch(startRange, itemCount, properties, true);
				}
				else
				{
					this.LoadBrowsePage(startRange + 1, itemCount, false, properties);
				}
			}
			catch (ADInvalidHandleCookieException)
			{
				if (retry)
				{
					this.cookie = string.Empty;
					this.Load(startRange, itemCount, false, fetchingPhase);
				}
			}
		}

		private void LoadPagedSearch(int startRange, int itemCount, PropertyDefinition[] properties, bool retry)
		{
			int num = startRange;
			if (!string.IsNullOrEmpty(this.cookie))
			{
				num = startRange - (this.cookieIndex + 1);
			}
			int itemsToSkip = num;
			object[][] array;
			int num2;
			if (DirectoryAssistance.IsEmptyAddressList(this.userContext))
			{
				array = new object[0][];
				num2 = 0;
			}
			else
			{
				array = AddressBookBase.PagedSearch(DirectoryAssistance.IsVirtualAddressList(this.userContext) ? this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN : null, DirectoryAssistance.IsVirtualAddressList(this.userContext) ? null : this.addressBookBase, this.userContext.ExchangePrincipal.MailboxInfo.OrganizationId, AddressBookBase.RecipientCategory.All, this.searchString, itemsToSkip, ref this.cookie, itemCount, out num2, ref this.lcid, ref this.preferredDC, properties);
			}
			if (array.Length > 0)
			{
				base.StartRange = startRange;
				base.EndRange = base.StartRange + (array.Length - 1);
				this.cookieIndex = base.EndRange;
				base.Items = array;
				return;
			}
			if (this.cookie != null && this.cookie.Length != 0 && retry)
			{
				this.cookieIndex = 0;
				this.LoadPagedSearch(0, itemCount, properties, false);
				return;
			}
			if (num2 != 0 && retry)
			{
				if (this.cookieIndex > 0)
				{
					num2 += this.cookieIndex + 1;
				}
				int num3 = num2 - 1;
				startRange = num3 - (itemCount - 1);
				if (startRange < 0)
				{
					startRange = 0;
				}
				this.cookie = null;
				this.cookieIndex = 0;
				this.lcid = Culture.GetUserCulture().LCID;
				this.LoadPagedSearch(startRange, itemCount, properties, false);
				return;
			}
			base.StartRange = int.MinValue;
			base.EndRange = int.MinValue;
			this.cookie = null;
			this.cookieIndex = 0;
			base.Items = array;
		}

		private void LoadBrowsePage(int startRange, int itemCount, bool seekToCondition, PropertyDefinition[] properties)
		{
			int num;
			if (startRange > 1 && startRange < 2147483647)
			{
				num = startRange - 1;
				itemCount++;
			}
			else
			{
				num = startRange;
			}
			object[][] array;
			int num2;
			if (DirectoryAssistance.IsEmptyAddressList(this.userContext))
			{
				array = new object[0][];
				num2 = 0;
			}
			else if (seekToCondition)
			{
				array = this.addressBookBase.BrowseTo(ref this.cookie, this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, ref this.lcid, ref this.preferredDC, this.searchString, itemCount, out num2, DirectoryAssistance.IsVirtualAddressList(this.userContext), properties);
			}
			else
			{
				array = this.addressBookBase.BrowseTo(ref this.cookie, this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, ref this.lcid, ref this.preferredDC, num, itemCount, out num2, DirectoryAssistance.IsVirtualAddressList(this.userContext), properties);
				if (startRange > 1 && startRange < 2147483647)
				{
					itemCount--;
					if (array.Length > 1)
					{
						num2++;
						this.offsetForData = 1;
					}
					else if (array.Length == 1)
					{
						int num3 = num % itemCount;
						if (num3 == 0)
						{
							num = startRange - itemCount;
						}
						else
						{
							num = num - num3 + 1;
						}
						array = this.addressBookBase.BrowseTo(ref this.cookie, this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, ref this.lcid, ref this.preferredDC, num, itemCount, out num2, DirectoryAssistance.IsVirtualAddressList(this.userContext), properties);
					}
				}
			}
			int num4 = (array.Length <= itemCount) ? (array.Length - this.offsetForData) : itemCount;
			if (num4 == 0 && !DirectoryAssistance.IsEmptyAddressList(this.userContext))
			{
				this.offsetForData = 0;
				array = this.addressBookBase.BrowseTo(ref this.cookie, this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, ref this.lcid, ref this.preferredDC, 0, itemCount, out num2, DirectoryAssistance.IsVirtualAddressList(this.userContext), properties);
				startRange = num2 - (itemCount - 1);
				if (startRange < 1)
				{
					startRange = 1;
				}
				array = this.addressBookBase.BrowseTo(ref this.cookie, this.userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN, ref this.lcid, ref this.preferredDC, startRange, itemCount, out num2, DirectoryAssistance.IsVirtualAddressList(this.userContext), properties);
				num4 = ((array.Length < itemCount) ? array.Length : itemCount);
			}
			if (num4 > 0)
			{
				base.StartRange = Math.Max(0, num2 - 1);
				base.EndRange = base.StartRange + num4 - 1;
			}
			else
			{
				base.StartRange = int.MinValue;
				base.EndRange = int.MinValue;
			}
			base.Items = array;
		}

		public override int CurrentItem
		{
			get
			{
				return base.CurrentItem - this.offsetForData;
			}
		}

		public override bool MoveNext()
		{
			base.MoveNext();
			return base.CurrentItem < base.RangeCount + this.offsetForData;
		}

		public override void MoveToItem(int itemIndex)
		{
			if (itemIndex < 0 || base.RangeCount + this.offsetForData <= itemIndex)
			{
				throw new IndexOutOfRangeException("itemIndex=" + itemIndex.ToString() + " must be between 0 and " + (base.RangeCount + this.offsetForData).ToString());
			}
			base.SetIndexer(itemIndex + this.offsetForData);
		}

		public bool LoadAdjacent(ObjectId adjacentObjectId, SeekDirection seekDirection, int itemCount)
		{
			throw new NotImplementedException();
		}

		private const string RoomClass = "AD.ResourceType.Room";

		private static readonly Dictionary<RecipientType, string> itemClasses = ADListViewDataSource.LoadItemClasses();

		private AddressBookBase addressBookBase;

		private string cookie;

		private int cookieIndex;

		private string searchString;

		private bool search;

		private int offsetForData;

		private UserContext userContext;

		private int lcid;

		private string preferredDC;
	}
}
