using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class DistributionListDataSource : ExchangeListViewDataSource, IListViewDataSource
	{
		internal DistributionListDataSource(UserContext userContext, Hashtable properties, List<Participant> participants, ColumnId sortedColumn, SortOrder order) : base(properties)
		{
			using (List<Participant>.Enumerator enumerator = participants.GetEnumerator())
			{
				AdRecipientBatchQuery adRecipientBatchQuery = new AdRecipientBatchQuery(enumerator, userContext);
				PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
				object[][] array = new object[participants.Count][];
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				for (int i = 0; i < participants.Count; i++)
				{
					array[i] = new object[requestedProperties.Length];
					array[i][base.PropertyIndex(StoreObjectSchema.DisplayName)] = participants[i].DisplayName;
					array[i][base.PropertyIndex(ParticipantSchema.EmailAddress)] = participants[i].EmailAddress;
					array[i][base.PropertyIndex(ParticipantSchema.RoutingType)] = participants[i].RoutingType;
					string text2;
					if (participants[i].Origin is StoreParticipantOrigin)
					{
						string text = ((StoreParticipantOrigin)participants[i].Origin).OriginItemId.ToBase64String();
						if (Utilities.IsMapiPDL(participants[i].RoutingType))
						{
							array[i][base.PropertyIndex(StoreObjectSchema.ItemClass)] = "IPM.DistList";
							array[i][base.PropertyIndex(RecipientSchema.EmailAddrType)] = 0;
							text2 = text;
						}
						else
						{
							int emailAddressIndex = (int)((StoreParticipantOrigin)participants[i].Origin).EmailAddressIndex;
							array[i][base.PropertyIndex(StoreObjectSchema.ItemClass)] = "IPM.Contact";
							array[i][base.PropertyIndex(RecipientSchema.EmailAddrType)] = emailAddressIndex;
							text2 = text + emailAddressIndex;
						}
						array[i][base.PropertyIndex(ItemSchema.RecipientType)] = AddressOrigin.Store;
						array[i][base.PropertyIndex(ContactSchema.Email1)] = participants[i];
						array[i][base.PropertyIndex(ParticipantSchema.OriginItemId)] = text;
					}
					else if (participants[i].Origin is DirectoryParticipantOrigin)
					{
						ADRecipient adRecipient = adRecipientBatchQuery.GetAdRecipient(participants[i].EmailAddress);
						if (adRecipient != null)
						{
							if (Utilities.IsADDistributionList(adRecipient.RecipientType))
							{
								array[i][base.PropertyIndex(StoreObjectSchema.ItemClass)] = "AD.RecipientType.Group";
							}
							else
							{
								array[i][base.PropertyIndex(StoreObjectSchema.ItemClass)] = "AD.RecipientType.User";
							}
							array[i][base.PropertyIndex(ContactSchema.Email1)] = new Participant(participants[i].DisplayName, adRecipient.PrimarySmtpAddress.ToString(), "SMTP");
							text2 = Utilities.GetBase64StringFromADObjectId(adRecipient.Id);
						}
						else
						{
							text2 = "[ADUser]" + participants[i].DisplayName + participants[i].EmailAddress;
							array[i][base.PropertyIndex(StoreObjectSchema.ItemClass)] = "AD.RecipientType.User";
							array[i][base.PropertyIndex(ContactSchema.Email1)] = participants[i];
							string participantProperty = Utilities.GetParticipantProperty<string>(participants[i], ParticipantSchema.SmtpAddress, null);
							if (participantProperty != null)
							{
								array[i][base.PropertyIndex(ParticipantSchema.RoutingType)] = "SMTP";
								array[i][base.PropertyIndex(ParticipantSchema.EmailAddress)] = participantProperty;
							}
						}
						array[i][base.PropertyIndex(ItemSchema.RecipientType)] = AddressOrigin.Directory;
					}
					else
					{
						if (!(participants[i].Origin is OneOffParticipantOrigin))
						{
							throw new ArgumentException("Invalid participant origin type.");
						}
						text2 = participants[i].RoutingType + ":" + participants[i].EmailAddress;
						array[i][base.PropertyIndex(StoreObjectSchema.ItemClass)] = "OneOff";
						array[i][base.PropertyIndex(ItemSchema.RecipientType)] = AddressOrigin.OneOff;
						array[i][base.PropertyIndex(ContactSchema.Email1)] = participants[i];
					}
					if (dictionary.ContainsKey(text2))
					{
						text2 += i;
					}
					dictionary[text2] = 1;
					array[i][base.PropertyIndex(ItemSchema.Id)] = text2;
				}
				Array.Sort<object[]>(array, new DistributionListDataSource.DistributionListMemberComparer(userContext.UserCulture, sortedColumn, order, this));
				base.Items = array;
				base.StartRange = 0;
				base.EndRange = array.Length - 1;
			}
		}

		public string ContainerId
		{
			get
			{
				return string.Empty;
			}
		}

		public string GetItemClass()
		{
			return this.GetItemProperty<string>(StoreObjectSchema.ItemClass);
		}

		public string GetItemId()
		{
			return this.GetItemProperty<string>(ItemSchema.Id);
		}

		public void Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount)
		{
		}

		public void Load(int startRange, int itemCount)
		{
		}

		public void Load(string seekValue, int itemCount)
		{
		}

		public bool LoadAdjacent(ObjectId adjacentObjectId, SeekDirection seekDirection, int itemCount)
		{
			throw new NotImplementedException();
		}

		public override int TotalCount
		{
			get
			{
				return base.RangeCount;
			}
		}

		public int UnreadCount
		{
			get
			{
				return 0;
			}
		}

		public bool UserHasRightToLoad
		{
			get
			{
				return true;
			}
		}

		public const string OneOffContact = "OneOff";

		private class DistributionListMemberComparer : IComparer<object[]>
		{
			public DistributionListMemberComparer(CultureInfo cultureInfo, ColumnId sortedColumn, SortOrder order, DistributionListDataSource dataSource)
			{
				this.cultureInfo = cultureInfo;
				this.sortedColumn = sortedColumn;
				this.order = order;
				this.dataSource = dataSource;
			}

			int IComparer<object[]>.Compare(object[] x, object[] y)
			{
				int num = 0;
				if (this.sortedColumn == ColumnId.MemberIcon)
				{
					num = string.Compare((string)x[this.dataSource.PropertyIndex(StoreObjectSchema.ItemClass)], (string)y[this.dataSource.PropertyIndex(StoreObjectSchema.ItemClass)]);
				}
				else
				{
					string strA;
					string strA2;
					ContactUtilities.GetParticipantEmailAddress((Participant)x[this.dataSource.PropertyIndex(ContactSchema.Email1)], out strA, out strA2);
					string strB;
					string strB2;
					ContactUtilities.GetParticipantEmailAddress((Participant)y[this.dataSource.PropertyIndex(ContactSchema.Email1)], out strB, out strB2);
					switch (this.sortedColumn)
					{
					case ColumnId.MemberDisplayName:
						num = string.Compare(strA2, strB2, false, this.cultureInfo);
						break;
					case ColumnId.MemberEmail:
						num = string.Compare(strA, strB, false, this.cultureInfo);
						break;
					}
				}
				if (this.order == SortOrder.Ascending)
				{
					return num;
				}
				return -num;
			}

			private readonly CultureInfo cultureInfo;

			private readonly ColumnId sortedColumn;

			private readonly SortOrder order;

			private readonly DistributionListDataSource dataSource;
		}
	}
}
