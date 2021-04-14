using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas.Commands;
using Microsoft.Exchange.Connections.Eas.Commands.ItemOperations;
using Microsoft.Exchange.Connections.Eas.Commands.Sync;
using Microsoft.Exchange.Connections.Eas.Model.Extensions;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSync;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EasFolder : EasFolderBase
	{
		protected EasFolder()
		{
		}

		protected EasFolder(Add add, UserSmtpAddress userSmtpAddress) : base(add.ServerId, add.ParentId, add.DisplayName, add.GetEasFolderType())
		{
			this.UserSmtpAddressString = (string)userSmtpAddress;
		}

		internal string UserSmtpAddressString { get; private set; }

		internal static bool IsCalendarFolder(EasFolderType easFolderType)
		{
			return easFolderType == EasFolderType.Calendar || easFolderType == EasFolderType.UserCalendar;
		}

		internal static bool IsContactFolder(EasFolderType easFolderType)
		{
			return easFolderType == EasFolderType.Contacts || easFolderType == EasFolderType.UserContacts;
		}

		protected override List<MessageRec> InternalLookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			ArgumentValidator.ThrowIfInvalidValue<PropTag>("ptagToLookup", ptagToLookup, (PropTag ptag) => ptag == PropTag.EntryId);
			List<string> list = new List<string>(keysToLookup.Count);
			foreach (byte[] entryId in keysToLookup)
			{
				list.Add(EasMailbox.GetStringId(entryId));
			}
			List<Fetch> fetchList = new List<Fetch>(keysToLookup.Count);
			CommonUtils.ProcessInBatches<string>(list.ToArray(), 10, delegate(string[] messageBatch)
			{
				ItemOperationsResponse itemOperationsResponse = this.Mailbox.EasConnectionWrapper.LookupItems(messageBatch, this.ServerId);
				fetchList.AddRange(itemOperationsResponse.Response.Fetches);
			});
			return this.CreateMessageRecsForFetches(fetchList);
		}

		protected int GetItemEstimate(EasConnectionWrapper easConnectionWrapper, EasSyncOptions options)
		{
			MrsTracer.Provider.Function("EasFolder.GetItemEstimate: SyncKey={0}", new object[]
			{
				options.SyncKey
			});
			return easConnectionWrapper.GetCountOfItemsToSync(base.ServerId, options);
		}

		protected EasSyncResult SyncMessages(EasConnectionWrapper easConnectionWrapper, EasSyncOptions options)
		{
			MrsTracer.Provider.Function("EasFolder.SyncMessages: SyncKey={0}", new object[]
			{
				options.SyncKey
			});
			bool recentOnly = !EasFolder.IsCalendarFolder(base.EasFolderType) && options.RecentOnly && !EasFolder.IsContactFolder(base.EasFolderType);
			SyncResponse syncResponse;
			try
			{
				syncResponse = easConnectionWrapper.Sync(base.ServerId, options, recentOnly);
			}
			catch (EasRequiresSyncKeyResetException ex)
			{
				MrsTracer.Provider.Error("Encountered RequiresSyncKeyReset error: {0}", new object[]
				{
					ex
				});
				options.SyncKey = "0";
				syncResponse = easConnectionWrapper.Sync(base.ServerId, options, recentOnly);
			}
			if (!(options.SyncKey == "0"))
			{
				return this.GetMessageRecsAndNewSyncKey(syncResponse, options);
			}
			return this.ProcessPrimingSync(easConnectionWrapper, options, syncResponse);
		}

		protected IEnumerable<MessageRec> CreateMessageRecsForDeletions(List<DeleteCommand> deletions)
		{
			foreach (DeleteCommand delete in deletions)
			{
				yield return new MessageRec(EasMailbox.GetEntryId(delete.ServerId), base.EntryId, DateTime.MinValue, 0, MsgRecFlags.Deleted, this.GetAdditionalProps(delete));
			}
			yield break;
		}

		private static DateTime GetCreationTimestamp(string dateReceived)
		{
			DateTime minValue;
			if (!DateTime.TryParse(dateReceived, out minValue))
			{
				minValue = DateTime.MinValue;
			}
			return minValue;
		}

		private List<MessageRec> CreateMessageRecsForFetches(IReadOnlyCollection<Fetch> fetches)
		{
			List<MessageRec> list = new List<MessageRec>(fetches.Count);
			foreach (Fetch fetch in fetches)
			{
				if (fetch.Status == 1)
				{
					list.Add(new MessageRec(EasMailbox.GetEntryId(fetch.ServerId), base.EntryId, EasFolder.GetCreationTimestamp(fetch.Properties.DateReceived), (int)fetch.Properties.Body.EstimatedDataSize, MsgRecFlags.None, this.GetAdditionalProps(fetch)));
				}
			}
			return list;
		}

		private IEnumerable<MessageRec> CreateMessageRecsForAdditions(List<AddCommand> additions)
		{
			foreach (AddCommand addition in additions)
			{
				yield return new MessageRec(EasMailbox.GetEntryId(addition.ServerId), base.EntryId, EasFolder.GetCreationTimestamp(addition.ApplicationData.DateReceived), (int)((addition.ApplicationData.Body == null) ? 0U : addition.ApplicationData.Body.EstimatedDataSize), MsgRecFlags.None, this.GetAdditionalProps(addition));
			}
			yield break;
		}

		private IEnumerable<MessageRec> CreateMessageRecsForChanges(List<ChangeCommand> changes)
		{
			foreach (ChangeCommand change in changes)
			{
				int estimatedDataSize = (int)((change.ApplicationData.Body == null) ? 0U : change.ApplicationData.Body.EstimatedDataSize);
				yield return new MessageRec(EasMailbox.GetEntryId(change.ServerId), base.EntryId, DateTime.MinValue, estimatedDataSize, MsgRecFlags.None, this.GetAdditionalProps(change));
			}
			yield break;
		}

		private string GetSyncKeyForFolder(SyncResponse syncResponse, out bool hasMoreAvailable)
		{
			Collection collection2 = syncResponse.Collections.Find((Collection collection) => collection.CollectionId == base.ServerId);
			hasMoreAvailable = (collection2 != null && collection2.HasMoreAvailable());
			if (collection2 == null)
			{
				return null;
			}
			return collection2.SyncKey;
		}

		private EasSyncResult ProcessPrimingSync(EasConnectionWrapper easConnection, EasSyncOptions options, SyncResponse syncResponse)
		{
			bool flag;
			string syncKeyForFolder = this.GetSyncKeyForFolder(syncResponse, out flag);
			if (string.IsNullOrEmpty(syncKeyForFolder))
			{
				throw new EasSyncCouldNotFindFolderException(base.ServerId);
			}
			options.SyncKey = syncKeyForFolder;
			return this.SyncMessages(easConnection, options);
		}

		private EasSyncResult GetMessageRecsAndNewSyncKey(SyncResponse syncResponse, EasSyncOptions options)
		{
			List<MessageRec> list = new List<MessageRec>(options.MaxNumberOfMessage);
			bool hasMoreAvailable;
			string text = this.GetSyncKeyForFolder(syncResponse, out hasMoreAvailable);
			if (string.IsNullOrEmpty(text))
			{
				text = options.SyncKey;
			}
			else
			{
				list.AddRange(this.CreateMessageRecsForAdditions(syncResponse.Additions));
				list.AddRange(this.CreateMessageRecsForDeletions(syncResponse.Deletions));
				list.AddRange(this.CreateMessageRecsForChanges(syncResponse.Changes));
			}
			return new EasSyncResult
			{
				MessageRecs = list,
				SyncKeyRequested = options.SyncKey,
				NewSyncKey = text,
				HasMoreAvailable = hasMoreAvailable
			};
		}

		private PropValueData[] GetAdditionalProps(Fetch fetch)
		{
			return SyncEmailUtils.GetMessageProps(new SyncEmailContext
			{
				IsRead = new bool?(fetch.IsRead()),
				IsDraft = new bool?(base.EasFolderType == EasFolderType.Drafts),
				SyncMessageId = fetch.ServerId
			}, this.UserSmtpAddressString, base.EntryId, new PropValueData[]
			{
				new PropValueData(PropTag.LastModificationTime, DateTime.UtcNow)
			});
		}

		private PropValueData[] GetAdditionalProps(AddCommand add)
		{
			List<PropValueData> list = new List<PropValueData>();
			list.Add(new PropValueData(PropTag.LastModificationTime, CommonUtils.DefaultLastModificationTime));
			list.Add(new PropValueData(PropTag.ObjectType, 0));
			if (EasFolder.IsContactFolder(base.EasFolderType))
			{
				List<PropValueData> contactProperties = EasFxContactMessage.GetContactProperties(add.ApplicationData);
				if (contactProperties.Count > 0)
				{
					list.AddRange(contactProperties);
				}
			}
			return SyncEmailUtils.GetMessageProps(new SyncEmailContext
			{
				IsRead = new bool?(add.IsRead()),
				IsDraft = new bool?(base.EasFolderType == EasFolderType.Drafts),
				SyncMessageId = add.ServerId
			}, this.UserSmtpAddressString, base.EntryId, list.ToArray());
		}

		private PropValueData[] GetAdditionalProps(ChangeCommand change)
		{
			bool? isRead = change.IsRead();
			EasMessageCategory easMessageCategory = (isRead == null) ? EasMessageCategory.AddOrUpdate : (isRead.Value ? EasMessageCategory.ChangeToRead : EasMessageCategory.ChangeToUnread);
			List<PropValueData> list = new List<PropValueData>();
			list.Add(new PropValueData(PropTag.LastModificationTime, DateTime.UtcNow));
			list.Add(new PropValueData(PropTag.ObjectType, (int)easMessageCategory));
			if (EasFolder.IsContactFolder(base.EasFolderType))
			{
				List<PropValueData> contactProperties = EasFxContactMessage.GetContactProperties(change.ApplicationData);
				if (contactProperties.Count > 0)
				{
					list.AddRange(contactProperties);
				}
			}
			return SyncEmailUtils.GetMessageProps(new SyncEmailContext
			{
				IsRead = isRead,
				IsDraft = new bool?(base.EasFolderType == EasFolderType.Drafts),
				SyncMessageId = change.ServerId
			}, this.UserSmtpAddressString, base.EntryId, list.ToArray());
		}

		private PropValueData[] GetAdditionalProps(DeleteCommand delete)
		{
			return SyncEmailUtils.GetMessageProps(new SyncEmailContext
			{
				SyncMessageId = delete.ServerId
			}, this.UserSmtpAddressString, base.EntryId, new PropValueData[]
			{
				new PropValueData(PropTag.ObjectType, 1)
			});
		}
	}
}
