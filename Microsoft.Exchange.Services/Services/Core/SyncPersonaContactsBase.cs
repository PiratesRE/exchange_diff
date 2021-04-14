using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class SyncPersonaContactsBase<RequestType, SingleItemType> : SingleStepServiceCommand<RequestType, SingleItemType> where RequestType : SyncPersonaContactsRequestBase, new() where SingleItemType : SyncPersonaContactsResponseBase, new()
	{
		public SyncPersonaContactsBase(CallContext callContext, RequestType request, SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStateInfo syncStateInfo) : base(callContext, request)
		{
			OwsLogRegistry.Register(base.GetType().Name, typeof(SyncPersonaContactsMetadata), new Type[0]);
			this.syncStateInfo = syncStateInfo;
		}

		internal override ServiceResult<SingleItemType> Execute()
		{
			SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug<string>((long)this.GetHashCode(), "SyncPersonaContactsBase.Execute: User '{0}'", base.CallContext.EffectiveCaller.PrimarySmtpAddress);
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.mailboxSession = base.GetMailboxIdentityMailboxSession();
			this.determiner = PropertyListForViewRowDeterminer.BuildForPersonObjects(SyncPersonaContactsBase<RequestType, SingleItemType>.FullPersonaShape);
			this.contactPropertyList = PropertyDefinitionCollection.Merge<PropertyDefinition>(new IEnumerable<PropertyDefinition>[]
			{
				this.determiner.GetPropertiesToFetch(),
				Person.RequiredProperties,
				SyncPersonaContactsBase<RequestType, SingleItemType>.ConversationIdProperties
			});
			this.personaPropertyList = Persona.GetPropertyListForPersonaResponseShape(SyncPersonaContactsBase<RequestType, SingleItemType>.FullPersonaShape);
			this.personaPropertyDefinitions = this.personaPropertyList.GetPropertyDefinitions();
			SortBy[] sortByProperties = new SortBy[]
			{
				new SortBy(ItemSchema.ConversationId, SortOrder.Ascending)
			};
			using (AllContactsCursor allContactsCursor = new AllContactsCursor(this.mailboxSession, this.contactPropertyList, sortByProperties))
			{
				RequestType request = base.Request;
				if (string.IsNullOrEmpty(request.SyncState))
				{
					RequestType request2 = base.Request;
					if (request2.MaxPersonas > 0)
					{
						int estimatedRowCount = allContactsCursor.EstimatedRowCount;
						RequestType request3 = base.Request;
						if (estimatedRowCount > request3.MaxPersonas)
						{
							ExTraceGlobals.SyncPeopleCallTracer.TraceWarning<int>((long)this.GetHashCode(), "Too many contacts ({0}), failing SyncPersonaContacts request", allContactsCursor.EstimatedRowCount);
							throw new TooManyContactsException();
						}
					}
				}
				this.responseMessage = this.ExecuteAndGetResult(allContactsCursor);
			}
			stopwatch.Stop();
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.TotalTime, stopwatch.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.PeopleCount, this.processedPersons.Count);
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.DeletedPeopleCount, this.responseMessage.DeletedPeople.Length);
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.IncludesLastItemInRange, this.responseMessage.IncludesLastItemInRange);
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.ContactsEnumerated, this.contactsEnumerated);
			if (this.responseMessage.SyncState != null)
			{
				base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.SyncStateSize, this.responseMessage.SyncState.Length);
				base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.SyncStateHash, this.responseMessage.SyncState.GetHashCode());
			}
			if (this.invalidContacts > 0)
			{
				base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.InvalidContacts, this.invalidContacts);
			}
			SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug<int, int, bool>((long)this.GetHashCode(), "SyncPersonaContactsBase:.Execute: End, {0} personas, {1} deleted, last = {2}", this.processedPersons.Count, this.deletedPeople.Count, this.responseMessage.IncludesLastItemInRange);
			return new ServiceResult<SingleItemType>(this.responseMessage);
		}

		protected abstract SingleItemType ExecuteAndGetResult(AllContactsCursor cursor);

		protected abstract void AddContacts(PersonId personId, List<IStorePropertyBag> contacts);

		protected bool PopulateFolderListAndCheckIfChanged()
		{
			Dictionary<StoreObjectId, SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder> dictionary = this.syncStates;
			this.syncStates = new Dictionary<StoreObjectId, SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder>();
			int num = 0;
			ContactFoldersEnumerator contactFoldersEnumerator = new ContactFoldersEnumerator(this.mailboxSession, ContactFoldersEnumeratorOptions.SkipDeletedFolders);
			foreach (IStorePropertyBag storePropertyBag in contactFoldersEnumerator)
			{
				StoreObjectId objectId = ((VersionedId)storePropertyBag.TryGetProperty(FolderSchema.Id)).ObjectId;
				num ^= objectId.ToBase64String().GetHashCode();
				SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder syncStatePerFolder = null;
				if (dictionary.TryGetValue(objectId, out syncStatePerFolder))
				{
					dictionary.Remove(objectId);
					this.syncStates[objectId] = syncStatePerFolder;
				}
				else
				{
					syncStatePerFolder = new SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder();
					syncStatePerFolder.FolderId = objectId;
					this.syncStates[objectId] = syncStatePerFolder;
				}
			}
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.FolderCount, this.syncStates.Count);
			if (dictionary.Count > 0)
			{
				SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.PopulateFolderListAndCheckIfChanged: A previously synced folder was deleted. Throwing FullSyncRequiredException.");
				throw new FullSyncRequiredException();
			}
			if (num != this.myContactsHash)
			{
				this.lastPersonId = null;
				this.querySyncInProgress = true;
				this.myContactsHash = num;
				return true;
			}
			return false;
		}

		protected void DoQuerySync(AllContactsCursor cursor)
		{
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.QuerySync, true);
			SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.DoQuerySync: Start");
			this.includesLastItemInRange = false;
			if (this.lastPersonId != null)
			{
				SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.DoQuerySync: Continuing initial sync. Seeking to lastPersonId.");
				ConversationId propertyValue = ConversationId.Create(this.lastPersonId.GetBytes());
				QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.ConversationId, propertyValue);
				if (!cursor.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
				{
					SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.DoQuerySync: Failed to sync to lastPersonId, resyncing all contacts");
					cursor.SeekToOffset(0);
				}
			}
			while (cursor.Current != null)
			{
				int num = this.currentReturnSize;
				RequestType request = base.Request;
				if (num >= request.MaxChangesReturned)
				{
					break;
				}
				PersonId personId = (PersonId)cursor.Current.TryGetProperty(ContactSchema.PersonId);
				this.AddPerson(personId, cursor);
			}
			if (cursor.Current == null)
			{
				this.lastPersonId = null;
				this.querySyncInProgress = false;
				this.includesLastItemInRange = true;
			}
			else
			{
				this.lastPersonId = (PersonId)cursor.Current.TryGetProperty(ContactSchema.PersonId);
			}
			SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.DoQuerySync: End");
		}

		private void AddPerson(PersonId personId, AllContactsCursor cursor)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					List<IStorePropertyBag> list = new List<IStorePropertyBag>();
					while (cursor.Current != null)
					{
						PersonId personId2 = (PersonId)cursor.Current.TryGetProperty(ContactSchema.PersonId);
						if (!personId2.Equals(personId))
						{
							break;
						}
						ConversationId conversationId = cursor.Current.TryGetProperty(ItemSchema.ConversationId) as ConversationId;
						if (conversationId != null && ArrayComparer<byte>.Comparer.Equals(conversationId.GetBytes(), personId2.GetBytes()))
						{
							StoreObjectId key = (StoreObjectId)cursor.Current.TryGetProperty(StoreObjectSchema.ParentItemId);
							if (this.syncStates.ContainsKey(key))
							{
								list.Add(cursor.Current);
							}
						}
						else
						{
							SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug<string>((long)this.GetHashCode(), "SyncPersonaContactsBase.AddPerson: Contact's PR_CONVERSATION_ID is invalid. PersonId = {0}", personId.ToBase64String());
							this.invalidContacts++;
						}
						this.contactsEnumerated++;
						cursor.MoveNext();
					}
					ItemId item = IdConverter.PersonaIdFromPersonId(this.mailboxSession.MailboxGuid, personId);
					if (this.processedPersons.Contains(personId))
					{
						SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.AddPerson: This PersonaId is already included in the response");
						return;
					}
					this.processedPersons.Add(personId);
					if (list.Count == 0)
					{
						this.deletedPeople.Add(item);
						this.currentReturnSize++;
						return;
					}
					SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug<string, int>((long)this.GetHashCode(), "SyncPersonaContactsBase.AddPerson: Creating Persona {0} from {1} contacts", personId.ToBase64String(), list.Count);
					this.AddContacts(personId, list);
					this.personaIdsAdded.Add(item);
				}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			}
			catch (LocalizedException ex)
			{
				SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug<string, LocalizedException>((long)this.GetHashCode(), "SyncPersonaContactsBase.AddPerson: Exception thrown while processing person {0}: {1}", personId.ToBase64String(), ex);
				base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.ExceptionPersonId, personId.ToBase64String());
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, ex, "SyncPeople_Exception");
			}
		}

		protected void GetIcsCatchUpStates()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder syncStatePerFolder in this.syncStates.Values)
			{
				MailboxSyncProviderFactory mailboxSyncProviderFactory = new MailboxSyncProviderFactory(this.mailboxSession, syncStatePerFolder.FolderId);
				mailboxSyncProviderFactory.GenerateReadFlagChanges();
				mailboxSyncProviderFactory.GenerateConversationChanges();
				using (MailboxSyncProvider mailboxSyncProvider = (MailboxSyncProvider)mailboxSyncProviderFactory.CreateSyncProvider(null))
				{
					ServicesFolderSyncState servicesFolderSyncState = new ServicesFolderSyncState(syncStatePerFolder.FolderId, mailboxSyncProvider, null);
					SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.GetIcsCatchUpStates: Fetching catch-up sync state");
					servicesFolderSyncState.Watermark = mailboxSyncProvider.GetMaxItemWatermark(servicesFolderSyncState.Watermark);
					syncStatePerFolder.IcsSyncState = servicesFolderSyncState.SerializeAsBase64String();
				}
			}
			stopwatch.Stop();
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.CatchUpTime, stopwatch.ElapsedMilliseconds);
		}

		protected void DoIcsSync(AllContactsCursor cursor)
		{
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.IcsSync, true);
			Stopwatch stopwatch = new Stopwatch();
			SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.DoIcsSync: Start");
			int num = 0;
			this.includesLastItemInRange = true;
			foreach (SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder syncStatePerFolder in this.syncStates.Values)
			{
				int num2 = this.currentReturnSize;
				RequestType request = base.Request;
				if (num2 >= request.MaxChangesReturned)
				{
					this.includesLastItemInRange = false;
					break;
				}
				MailboxSyncProviderFactory mailboxSyncProviderFactory = new MailboxSyncProviderFactory(this.mailboxSession, syncStatePerFolder.FolderId);
				mailboxSyncProviderFactory.GenerateReadFlagChanges();
				mailboxSyncProviderFactory.GenerateConversationChanges();
				using (MailboxSyncProvider mailboxSyncProvider = (MailboxSyncProvider)mailboxSyncProviderFactory.CreateSyncProvider(null))
				{
					ServicesFolderSyncState servicesFolderSyncState = new ServicesFolderSyncState(syncStatePerFolder.FolderId, mailboxSyncProvider, syncStatePerFolder.IcsSyncState);
					bool flag = true;
					while (flag)
					{
						int num3 = this.currentReturnSize;
						RequestType request2 = base.Request;
						if (num3 >= request2.MaxChangesReturned)
						{
							break;
						}
						Microsoft.Exchange.Diagnostics.Trace trace = SyncPersonaContactsBase<RequestType, SingleItemType>.tracer;
						long id = (long)this.GetHashCode();
						string formatString = "SyncPersonaContactsBase.DoIcsSync: Requesting {0} changes from ICS";
						RequestType request3 = base.Request;
						trace.TraceDebug<int>(id, formatString, request3.MaxChangesReturned - this.currentReturnSize);
						Dictionary<ISyncItemId, ServerManifestEntry> dictionary = new Dictionary<ISyncItemId, ServerManifestEntry>();
						stopwatch.Start();
						MailboxSyncProvider mailboxSyncProvider2 = mailboxSyncProvider;
						ISyncWatermark watermark = servicesFolderSyncState.Watermark;
						ISyncWatermark maxSyncWatermark = null;
						bool enumerateDeletes = true;
						RequestType request4 = base.Request;
						flag = mailboxSyncProvider2.GetNewOperations(watermark, maxSyncWatermark, enumerateDeletes, request4.MaxChangesReturned - this.currentReturnSize, null, dictionary);
						stopwatch.Stop();
						SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug<int>((long)this.GetHashCode(), "SyncPersonaContactsBase.DoIcsSync: Received {0} entries from ICS", dictionary.Count);
						foreach (ServerManifestEntry serverManifestEntry in dictionary.Values)
						{
							num++;
							SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug<ChangeType, ConversationId>((long)this.GetHashCode(), "SyncPersonaContactsBase.DoIcsSyncs: {0} for person {1}", serverManifestEntry.ChangeType, serverManifestEntry.ConversationId);
							if (serverManifestEntry.ConversationId == null)
							{
								SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.DoIcsSync: ICS returned a conversation without a ConversationId");
							}
							else
							{
								QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.ConversationId, serverManifestEntry.ConversationId);
								cursor.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
								PersonId personId = PersonId.Create(serverManifestEntry.ConversationId.GetBytes());
								this.AddPerson(personId, cursor);
							}
						}
					}
					syncStatePerFolder.IcsSyncState = servicesFolderSyncState.SerializeAsBase64String();
					if (flag)
					{
						this.includesLastItemInRange = false;
					}
				}
			}
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.IcsTime, stopwatch.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.IcsChangesProcessed, num);
			SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.DoIcsSync: End");
		}

		protected void SplitSyncStates(string syncStateString, Action<string[]> additionalParsing)
		{
			if (string.IsNullOrEmpty(syncStateString))
			{
				return;
			}
			GrayException.MapAndReportGrayExceptions(delegate()
			{
				string[] array = syncStateString.Split(new char[]
				{
					','
				});
				if (array[this.syncStateInfo.VersionIndex] != this.syncStateInfo.VersionPrefix)
				{
					SyncPersonaContactsBase<RequestType, SingleItemType>.tracer.TraceDebug((long)this.GetHashCode(), "SyncPersonaContactsBase.SplitSyncStates: Received invalid sync state format, gonna continue with no sync state (full sync)");
					RequestType request = this.Request;
					request.SyncState = null;
					return;
				}
				if (!bool.TryParse(array[this.syncStateInfo.QuerySyncInProgressIndex], out this.querySyncInProgress))
				{
					throw new InvalidSyncStateDataException();
				}
				if (!string.IsNullOrEmpty(array[this.syncStateInfo.LastPersonIdIndex]))
				{
					this.lastPersonId = PersonId.Create(array[this.syncStateInfo.LastPersonIdIndex]);
				}
				if (!string.IsNullOrEmpty(array[this.syncStateInfo.MyContactsHashIndex]))
				{
					int.TryParse(array[this.syncStateInfo.MyContactsHashIndex], out this.myContactsHash);
				}
				if ((array.Length - this.syncStateInfo.SyncStatesStartIndex) % 2 != 0)
				{
					throw new InvalidSyncStateDataException();
				}
				for (int i = this.syncStateInfo.SyncStatesStartIndex; i < array.Length; i += 2)
				{
					SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder syncStatePerFolder = new SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder();
					syncStatePerFolder.FolderId = StoreObjectId.Deserialize(array[i]);
					syncStatePerFolder.IcsSyncState = array[i + 1];
					this.syncStates[syncStatePerFolder.FolderId] = syncStatePerFolder;
				}
				if (additionalParsing != null)
				{
					additionalParsing(array);
				}
			}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
		}

		protected string JoinSyncStates(params KeyValuePair<int, string>[] additionalValues)
		{
			string[] syncStateArray = new string[this.syncStateInfo.SyncStatesStartIndex + this.syncStates.Count * 2];
			syncStateArray[this.syncStateInfo.VersionIndex] = this.syncStateInfo.VersionPrefix;
			syncStateArray[this.syncStateInfo.QuerySyncInProgressIndex] = this.querySyncInProgress.ToString();
			syncStateArray[this.syncStateInfo.LastPersonIdIndex] = ((this.lastPersonId != null) ? this.lastPersonId.ToBase64String() : string.Empty);
			syncStateArray[this.syncStateInfo.MyContactsHashIndex] = this.myContactsHash.ToString();
			additionalValues.ToList<KeyValuePair<int, string>>().ForEach(delegate(KeyValuePair<int, string> value)
			{
				syncStateArray[value.Key] = value.Value;
			});
			int num = this.syncStateInfo.SyncStatesStartIndex;
			foreach (SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder syncStatePerFolder in this.syncStates.Values)
			{
				syncStateArray[num] = syncStatePerFolder.FolderId.ToBase64String();
				syncStateArray[num + 1] = syncStatePerFolder.IcsSyncState;
				num += 2;
			}
			return string.Join(",", syncStateArray);
		}

		protected string[] GetJumpHeaderSortKeys()
		{
			string[] array = null;
			RequestType request = base.Request;
			if (request.JumpHeaderValues != null)
			{
				RequestType request2 = base.Request;
				int num = request2.JumpHeaderValues.Length;
				array = new string[num];
				for (int i = 0; i < num; i++)
				{
					string[] array2 = array;
					int num2 = i;
					CultureInfo preferedCulture = this.mailboxSession.PreferedCulture;
					RequestType request3 = base.Request;
					array2[num2] = PeopleStringUtils.ComputeSortKey(preferedCulture, request3.JumpHeaderValues[i]);
				}
			}
			return array;
		}

		private const string ClassName = "SyncPersonaContactsBase";

		protected MailboxSession mailboxSession;

		private SingleItemType responseMessage = Activator.CreateInstance<SingleItemType>();

		protected PropertyDefinition[] contactPropertyList;

		protected ToServiceObjectForPropertyBagPropertyList personaPropertyList;

		protected PropertyDefinition[] personaPropertyDefinitions;

		protected readonly List<ItemId> deletedPeople = new List<ItemId>();

		protected bool querySyncInProgress = true;

		private PersonId lastPersonId;

		private HashSet<PersonId> processedPersons = new HashSet<PersonId>();

		protected int currentReturnSize;

		protected int contactsEnumerated;

		protected List<ItemId> personaIdsAdded = new List<ItemId>();

		protected bool includesLastItemInRange;

		private static readonly PersonaResponseShape FullPersonaShape = new PersonaResponseShape(ShapeEnum.AllProperties, new PropertyPath[]
		{
			PersonaSchema.FolderIds.PropertyPath,
			PersonaSchema.IsFavorite.PropertyPath,
			PersonaSchema.ThirdPartyPhotoUrls.PropertyPath,
			new PropertyUri(PropertyUriEnum.PersonaDisplayNameFirstLastSortKey),
			new PropertyUri(PropertyUriEnum.PersonaDisplayNameLastFirstSortKey),
			new PropertyUri(PropertyUriEnum.PersonaCompanyNameSortKey),
			new PropertyUri(PropertyUriEnum.PersonaHomeCitySortKey),
			new PropertyUri(PropertyUriEnum.PersonaWorkCitySortKey),
			new PropertyUri(PropertyUriEnum.PersonaDisplayNameFirstLastHeader),
			new PropertyUri(PropertyUriEnum.PersonaDisplayNameLastFirstHeader),
			PersonaSchema.Alias.PropertyPath
		});

		private static readonly PropertyDefinition[] ConversationIdProperties = new PropertyDefinition[]
		{
			ItemSchema.ConversationId,
			ContactSchema.PersonId
		};

		private static Microsoft.Exchange.Diagnostics.Trace tracer = ExTraceGlobals.SyncPersonaContactsBaseTracer;

		private PropertyListForViewRowDeterminer determiner;

		private int myContactsHash;

		private Dictionary<StoreObjectId, SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder> syncStates = new Dictionary<StoreObjectId, SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStatePerFolder>();

		private int invalidContacts;

		private SyncPersonaContactsBase<RequestType, SingleItemType>.SyncStateInfo syncStateInfo;

		private class SyncStatePerFolder
		{
			public StoreObjectId FolderId;

			public string IcsSyncState;
		}

		internal class SyncStateInfo
		{
			public string VersionPrefix { get; set; }

			public int VersionIndex { get; set; }

			public int QuerySyncInProgressIndex { get; set; }

			public int LastPersonIdIndex { get; set; }

			public int MyContactsHashIndex { get; set; }

			public int SyncStatesStartIndex { get; set; }
		}
	}
}
