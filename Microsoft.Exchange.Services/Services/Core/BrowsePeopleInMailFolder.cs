using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class BrowsePeopleInMailFolder : FindPeopleImplementation
	{
		public BrowsePeopleInMailFolder(FindPeopleParameters parameters, MailboxSession mailboxSession, StoreId folderId, ITracer tracer) : base(parameters, null, false)
		{
			ServiceCommandBase.ThrowIfNull(parameters, "parameters", "BrowsePeopleInMailFolder::BrowsePeopleInMailFolder");
			ServiceCommandBase.ThrowIfNull(mailboxSession, "mailboxSession", "BrowsePeopleInMailFolder::BrowsePeopleInMailFolder");
			ServiceCommandBase.ThrowIfNull(folderId, "folderId", "BrowsePeopleInMailFolder::BrowsePeopleInMailFolder");
			ServiceCommandBase.ThrowIfNull(tracer, "tracer", "BrowsePeopleInMailFolder::BrowsePeopleInMailFolder");
			this.mailboxSession = mailboxSession;
			this.folderId = folderId;
			this.tracer = tracer;
		}

		public override void Validate()
		{
			base.Validate();
			if (base.AggregationRestriction != null || base.Restriction != null)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			if (base.SortResults != null)
			{
				throw new ServiceArgumentException((CoreResources.IDs)2841035169U);
			}
		}

		public override FindPeopleResult Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			FindPeopleResult findPeopleResult = this.ExecuteInternal();
			stopwatch.Stop();
			base.Log(FindPeopleMetadata.BrowseSendersTime, stopwatch.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.PersonalCount, findPeopleResult.PersonaList.Length);
			return findPeopleResult;
		}

		private FindPeopleResult ExecuteInternal()
		{
			int num = base.MaxRows;
			if (num > 20)
			{
				this.tracer.TraceDebug<int, int>((long)this.GetHashCode(), "Count requested is {0}. This exceeds the MaxCount specified {1}", num, 20);
				num = 20;
			}
			Dictionary<string, int> senderDataFromMailFolder = this.GetSenderDataFromMailFolder();
			IEnumerable<Persona> source;
			if (senderDataFromMailFolder.Count > 0)
			{
				source = BrowsePeopleInMailFolder.GetPersonasFromRecipientCache(this.mailboxSession, senderDataFromMailFolder, num, this.tracer, (long)this.GetHashCode());
			}
			else
			{
				source = Enumerable.Empty<Persona>();
			}
			return FindPeopleResult.CreateMailFolderBrowseResult(source.ToArray<Persona>());
		}

		internal static List<Persona> GetPersonasFromRecipientCache(MailboxSession mailboxSession, Dictionary<string, int> senderSmtpToUnreadCountMapping, int requestedCount, ITracer tracer, long hashCode)
		{
			ContactsEnumerator<IStorePropertyBag> contactsEnumerator = ContactsEnumerator<IStorePropertyBag>.CreateContactsOnlyEnumerator(mailboxSession, DefaultFolderType.RecipientCache, BrowsePeopleInMailFolder.RecipientCacheSortColumns, BrowsePeopleInMailFolder.RecipientCacheContactProperties, (IStorePropertyBag propertyBag) => propertyBag, new XSOFactory());
			List<IStorePropertyBag> list = new List<IStorePropertyBag>(requestedCount);
			List<Persona> list2 = new List<Persona>(requestedCount);
			foreach (IStorePropertyBag storePropertyBag in contactsEnumerator)
			{
				PersonType valueOrDefault = storePropertyBag.GetValueOrDefault<PersonType>(ContactSchema.PersonType, PersonType.Unknown);
				if (valueOrDefault == PersonType.Person)
				{
					string valueOrDefault2 = storePropertyBag.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, null);
					if (!string.IsNullOrEmpty(valueOrDefault2))
					{
						int num;
						if (!senderSmtpToUnreadCountMapping.TryGetValue(valueOrDefault2, out num))
						{
							tracer.TraceDebug<string>(hashCode, "EmailAddress {0} does not belong to a favorite sender. So ignoring this entry", valueOrDefault2);
						}
						else
						{
							if (num > 0)
							{
								list2.Add(FindPeopleImplementation.GetPersona(mailboxSession, storePropertyBag, num, PersonType.Person));
								tracer.TraceDebug<string, int>(hashCode, "GetPersonasFromRecipientCache: Added to persona list. EmailAddress: {0} and unreadCount: {1}", valueOrDefault2, num);
							}
							else
							{
								list.Add(storePropertyBag);
								tracer.TraceDebug<string, int>(hashCode, "GetPersonasFromRecipientCache: Added to personas with zero unread count. EmailAddress: {0} and unreadCount: {1}", valueOrDefault2, num);
							}
							senderSmtpToUnreadCountMapping.Remove(valueOrDefault2);
							if (list2.Count == requestedCount)
							{
								break;
							}
						}
					}
				}
			}
			int num2 = 0;
			while (num2 < list.Count && list2.Count < requestedCount)
			{
				IStorePropertyBag propertyBag2 = list[num2];
				list2.Add(FindPeopleImplementation.GetPersona(mailboxSession, propertyBag2, 0, PersonType.Person));
				num2++;
			}
			return list2;
		}

		private Dictionary<string, int> GetSenderDataFromMailFolder()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			using (Folder folder = Folder.Bind(this.mailboxSession, this.folderId))
			{
				SendersInMailFolder sendersInMailFolder = new SendersInMailFolder(folder);
				IEnumerable<IStorePropertyBag> sendersFromMailFolder = sendersInMailFolder.GetSendersFromMailFolder(BrowsePeopleInMailFolder.SenderAndUnreadProperties);
				foreach (IStorePropertyBag storePropertyBag in sendersFromMailFolder)
				{
					string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(MessageItemSchema.SenderSmtpAddress, null);
					if (!string.IsNullOrEmpty(valueOrDefault))
					{
						int valueOrDefault2 = storePropertyBag.GetValueOrDefault<int>(FolderSchema.UnreadCount, 0);
						dictionary.Add(valueOrDefault, valueOrDefault2);
						this.tracer.TraceDebug<string, int>((long)this.GetHashCode(), "GetSenderDataFromMailFolder: Added sender with smtpAddress: {0} and unreadCount: {1}", valueOrDefault, valueOrDefault2);
					}
				}
			}
			return dictionary;
		}

		public const int MaxCount = 20;

		public static readonly PropertyDefinition[] SenderAndUnreadProperties = new PropertyDefinition[]
		{
			MessageItemSchema.SenderSmtpAddress,
			FolderSchema.UnreadCount
		};

		private readonly MailboxSession mailboxSession;

		private readonly StoreId folderId;

		private readonly ITracer tracer;

		private static readonly PropertyDefinition[] RecipientCacheContactProperties = new PropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email1AddrType,
			StoreObjectSchema.DisplayName,
			ContactSchema.IMAddress,
			ContactSchema.PersonId,
			ContactSchema.PersonType,
			ContactSchema.RelevanceScore
		};

		private static readonly SortBy[] RecipientCacheSortColumns = new SortBy[]
		{
			new SortBy(ContactSchema.RelevanceScore, SortOrder.Ascending)
		};
	}
}
