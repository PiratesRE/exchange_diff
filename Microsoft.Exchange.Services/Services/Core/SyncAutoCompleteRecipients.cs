using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Serialization;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SyncAutoCompleteRecipients : SyncPersonaContactsBase<SyncAutoCompleteRecipientsRequest, SyncAutoCompleteRecipientsResponseMessage>
	{
		public SyncAutoCompleteRecipients(CallContext callContext, SyncAutoCompleteRecipientsRequest request) : base(callContext, request, SyncAutoCompleteRecipients.syncStateInfo)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			if (base.Result.Value == null)
			{
				return new SyncAutoCompleteRecipientsResponseMessage(base.Result.Code, base.Result.Error);
			}
			return base.Result.Value;
		}

		protected override SyncAutoCompleteRecipientsResponseMessage ExecuteAndGetResult(AllContactsCursor cursor)
		{
			SyncAutoCompleteRecipients.tracer.TraceDebug((long)this.GetHashCode(), "SyncAutoCompleteRecipients: Being executing");
			base.SplitSyncStates(base.Request.SyncState, delegate(string[] parts)
			{
				this.recipientCacheOffset = (string.IsNullOrEmpty(parts[SyncAutoCompleteRecipients.syncStateInfo.RecipientCacheOffsetIndex]) ? null : new int?(int.Parse(parts[SyncAutoCompleteRecipients.syncStateInfo.RecipientCacheOffsetIndex])));
				this.recipentCacheLastModifedTime = (string.IsNullOrEmpty(parts[SyncAutoCompleteRecipients.syncStateInfo.RecipientCacheLmtIndex]) ? null : new int?(int.Parse(parts[SyncAutoCompleteRecipients.syncStateInfo.RecipientCacheLmtIndex])));
			});
			if (base.PopulateFolderListAndCheckIfChanged())
			{
				this.recipientCacheOffset = new int?(0);
			}
			if (string.IsNullOrEmpty(base.Request.SyncState))
			{
				this.querySyncInProgress = true;
				if (!base.Request.FullSyncOnly)
				{
					base.GetIcsCatchUpStates();
				}
			}
			if (this.querySyncInProgress)
			{
				if (this.recipientCacheOffset != null)
				{
					this.AddRecipientCacheFirst();
				}
				if (this.recipientCacheOffset == null)
				{
					base.DoQuerySync(cursor);
				}
			}
			if (!this.querySyncInProgress && !base.Request.FullSyncOnly)
			{
				base.DoIcsSync(cursor);
			}
			SyncAutoCompleteRecipientsResponseMessage syncAutoCompleteRecipientsResponseMessage = new SyncAutoCompleteRecipientsResponseMessage();
			syncAutoCompleteRecipientsResponseMessage.SyncState = base.JoinSyncStates(new KeyValuePair<int, string>[]
			{
				new KeyValuePair<int, string>(SyncAutoCompleteRecipients.syncStateInfo.RecipientCacheLmtIndex, this.recipentCacheLastModifedTime.ToString()),
				new KeyValuePair<int, string>(SyncAutoCompleteRecipients.syncStateInfo.RecipientCacheOffsetIndex, this.recipientCacheOffset.ToString())
			});
			syncAutoCompleteRecipientsResponseMessage.Recipients = this.recipients.ToArray();
			syncAutoCompleteRecipientsResponseMessage.DeletedPeople = this.deletedPeople.Union(from p in (from p in this.recipients
			select p.PersonaId).Distinct<string>()
			select new ItemId(p, null)).ToArray<ItemId>();
			syncAutoCompleteRecipientsResponseMessage.JumpHeaderSortKeys = base.GetJumpHeaderSortKeys();
			syncAutoCompleteRecipientsResponseMessage.SortKeyVersion = PeopleStringUtils.ComputeSortVersion(this.mailboxSession.PreferedCulture);
			syncAutoCompleteRecipientsResponseMessage.IncludesLastItemInRange = this.includesLastItemInRange;
			if (this.includesLastItemInRange && base.Request.FullSyncOnly)
			{
				syncAutoCompleteRecipientsResponseMessage.SyncState = null;
			}
			SyncAutoCompleteRecipients.tracer.TraceDebug((long)this.GetHashCode(), "SyncAutoCompleteRecipients: Done executing");
			return syncAutoCompleteRecipientsResponseMessage;
		}

		protected override void AddContacts(PersonId personId, List<IStorePropertyBag> contacts)
		{
			SyncAutoCompleteRecipients.tracer.TraceDebug<int, PersonId>((long)this.GetHashCode(), "SyncAutoCompleteRecipients: Adding {0} contacts for {1}", contacts.Count, personId);
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.SyncPersonaContactsType, "SyncAutoCompleteRecipients");
			IList<AutoCompleteRecipient> list = (from recipient in contacts.SelectMany(new Func<IStorePropertyBag, IEnumerable<AutoCompleteRecipient>>(this.GetRecipientsForContact))
			group recipient by recipient.EmailAddress.ToLower() into @group
			select (from recipient in @group
			orderby recipient.RelevanceScore
			select recipient).First<AutoCompleteRecipient>()).ToList<AutoCompleteRecipient>();
			if (this.querySyncInProgress)
			{
				list = (from p in list
				where p.RelevanceScore == int.MaxValue
				select p).ToList<AutoCompleteRecipient>();
			}
			if (list.Count > 0)
			{
				this.currentReturnSize++;
			}
			this.recipients.AddRange(list);
		}

		private void AddRecipientCacheFirst()
		{
			SyncAutoCompleteRecipients.tracer.TraceDebug<int?>((long)this.GetHashCode(), "SyncAutoCompleteRecipients: Begin returning recipient cache. Page pos: {0}", this.recipientCacheOffset);
			SortBy[] sortColumns = new SortBy[]
			{
				new SortBy(ContactSchema.RelevanceScore, SortOrder.Ascending),
				new SortBy(ItemSchema.ConversationId, SortOrder.Ascending)
			};
			Folder folder = Folder.Bind(this.mailboxSession, DefaultFolderType.RecipientCache);
			if (this.recipentCacheLastModifedTime == null)
			{
				this.recipentCacheLastModifedTime = new int?(SyncAutoCompleteRecipients.GetLastModifiedSequenceNumber(folder));
			}
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortColumns, this.contactPropertyList))
			{
				queryResult.SeekToOffset(SeekReference.OriginBeginning, this.recipientCacheOffset.Value);
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(base.Request.MaxChangesReturned);
				this.recipients.AddRange(propertyBags.SelectMany(new Func<IStorePropertyBag, IEnumerable<AutoCompleteRecipient>>(this.GetRecipientsForContact)));
				this.currentReturnSize += propertyBags.Length;
				SyncAutoCompleteRecipients.tracer.TraceDebug<int>((long)this.GetHashCode(), "SyncAutoCompleteRecipients: Added {0} recipients from cache", propertyBags.Length);
				if (propertyBags.Length == base.Request.MaxChangesReturned)
				{
					this.recipientCacheOffset = new int?(this.recipientCacheOffset.Value + base.Request.MaxChangesReturned);
					SyncAutoCompleteRecipients.tracer.TraceDebug((long)this.GetHashCode(), "SyncAutoCompleteRecipients: Done with current batch, more results in next call");
				}
				else
				{
					SyncAutoCompleteRecipients.tracer.TraceDebug((long)this.GetHashCode(), "SyncAutoCompleteRecipients: Done with paging cache");
					int lastModifiedSequenceNumber = SyncAutoCompleteRecipients.GetLastModifiedSequenceNumber(folder);
					if (lastModifiedSequenceNumber != this.recipentCacheLastModifedTime)
					{
						SyncAutoCompleteRecipients.tracer.TraceDebug<int?, int>((long)this.GetHashCode(), "SyncAutoCompleteRecipients: CONFLICT: it appears the cache has changed between pages: init LMT:{0} current:{1}", this.recipentCacheLastModifedTime, lastModifiedSequenceNumber);
						throw new FullSyncRequiredException();
					}
					this.recipentCacheLastModifedTime = null;
					this.recipientCacheOffset = null;
				}
			}
		}

		private static int GetLastModifiedSequenceNumber(Folder folder)
		{
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, new SortBy[]
			{
				new SortBy(InternalSchema.ArticleId, SortOrder.Descending)
			}, new PropertyDefinition[]
			{
				InternalSchema.ArticleId
			}))
			{
				object[][] rows = queryResult.GetRows(1);
				if (rows.Length != 0)
				{
					return (int)rows[0][0];
				}
			}
			return -1;
		}

		private IEnumerable<AutoCompleteRecipient> GetRecipientsForContact(IStorePropertyBag contact)
		{
			HashSet<string> stringSet = SyncAutoCompleteRecipients.GetStringSet(contact, new PropertyDefinition[]
			{
				ContactSchema.Email1EmailAddress,
				ContactSchema.Email2EmailAddress,
				ContactSchema.Email3EmailAddress,
				ContactSchema.PrimarySmtpAddress
			});
			return from p in stringSet
			select new AutoCompleteRecipient
			{
				DisplayName = contact.TryGetProperty(StoreObjectSchema.DisplayName).ToString(),
				EmailAddress = p,
				FolderName = SyncAutoCompleteRecipients.GetString(contact, ItemSchema.ParentDisplayName),
				PersonaId = IdConverter.PersonaIdFromPersonId(this.mailboxSession.MailboxGuid, contact.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null)).Id,
				RecipientId = contact.GetValueOrDefault<StoreId>(ItemSchema.Id, null).ToBase64String(),
				RelevanceScore = contact.GetValueOrDefault<int>(ContactSchema.RelevanceScore, int.MaxValue),
				Surname = SyncAutoCompleteRecipients.GetString(contact, ContactSchema.Surname),
				GivenName = SyncAutoCompleteRecipients.GetString(contact, ContactSchema.GivenName)
			};
		}

		private static HashSet<string> GetStringSet(IStorePropertyBag propertyBag, params PropertyDefinition[] properties)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (PropertyDefinition property in properties)
			{
				string @string = SyncAutoCompleteRecipients.GetString(propertyBag, property);
				if (@string != null)
				{
					hashSet.Add(@string);
				}
			}
			return hashSet;
		}

		private static string GetString(IStorePropertyBag propertyBag, PropertyDefinition property)
		{
			string text = propertyBag.GetValueOrDefault<string>(property, string.Empty).Trim();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		private const string ClassName = "SyncAutoCompleteRecipients";

		private readonly List<AutoCompleteRecipient> recipients = new List<AutoCompleteRecipient>();

		private static Trace tracer = ExTraceGlobals.SyncAutoCompleteRecipientsTracer;

		private int? recipientCacheOffset = new int?(0);

		private int? recipentCacheLastModifedTime = null;

		private static readonly SyncAutoCompleteRecipients.SyncStateInfoForRecipients syncStateInfo = new SyncAutoCompleteRecipients.SyncStateInfoForRecipients
		{
			VersionPrefix = "SS3",
			VersionIndex = 0,
			QuerySyncInProgressIndex = 1,
			LastPersonIdIndex = 2,
			MyContactsHashIndex = 3,
			RecipientCacheOffsetIndex = 4,
			RecipientCacheLmtIndex = 5,
			SyncStatesStartIndex = 6
		};

		private class SyncStateInfoForRecipients : SyncPersonaContactsBase<SyncAutoCompleteRecipientsRequest, SyncAutoCompleteRecipientsResponseMessage>.SyncStateInfo
		{
			public int RecipientCacheOffsetIndex { get; set; }

			public int RecipientCacheLmtIndex { get; set; }
		}
	}
}
