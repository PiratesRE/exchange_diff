using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IcsCallback : IMapiManifestCallback
	{
		internal IcsCallback(MailboxSyncProvider mailboxSyncProvider, Dictionary<ISyncItemId, ServerManifestEntry> serverManifest, int numOperations, MailboxSyncWatermark syncWatermark)
		{
			this.mailboxSyncProvider = mailboxSyncProvider;
			this.serverManifest = serverManifest;
			this.numOperations = numOperations;
			this.moreAvailable = false;
			this.syncWatermark = syncWatermark;
			this.lastServerManifestEntry = null;
		}

		public static PropTag[] PropTags
		{
			get
			{
				return IcsCallback.propTags;
			}
		}

		public ServerManifestEntry ExtraServerManiferEntry
		{
			get
			{
				if (!this.moreAvailable)
				{
					return null;
				}
				return this.lastServerManifestEntry;
			}
		}

		public bool MoreAvailable
		{
			get
			{
				return this.moreAvailable;
			}
		}

		public void Bind(MailboxSyncWatermark syncWatermark, int numOperations, Dictionary<ISyncItemId, ServerManifestEntry> serverManifest)
		{
			this.syncWatermark = syncWatermark;
			this.numOperations = numOperations;
			this.serverManifest = serverManifest;
			this.moreAvailable = false;
			this.lastServerManifestEntry = null;
		}

		public ManifestCallbackStatus Change(byte[] entryId, byte[] sourceKey, byte[] changeKey, byte[] changeList, DateTime lastModifiedTime, ManifestChangeType changeType, bool associated, PropValue[] properties)
		{
			EnumValidator.ThrowIfInvalid<ManifestChangeType>(changeType, "changeType");
			if (ExTraceGlobals.SyncTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				this.TraceChangeChangeCallbackProps(entryId, sourceKey, changeKey, changeList, lastModifiedTime, changeType, associated, properties);
			}
			int? num = null;
			string text = null;
			bool read = false;
			ConversationId conversationId = null;
			bool firstMessageInConversation = false;
			ExDateTime? filterDate = null;
			foreach (PropValue propValue in properties)
			{
				if (!propValue.IsError())
				{
					PropTag propTag = propValue.PropTag;
					if (propTag <= PropTag.MessageDeliveryTime)
					{
						if (propTag != PropTag.MessageClass)
						{
							ConversationIndex index;
							if (propTag != PropTag.ConversationIndex)
							{
								if (propTag == PropTag.MessageDeliveryTime)
								{
									if (propValue.PropType == PropType.SysTime)
									{
										filterDate = new ExDateTime?((ExDateTime)propValue.GetDateTime());
									}
								}
							}
							else if (propValue.PropType == PropType.Binary && ConversationIndex.TryCreate(propValue.GetBytes(), out index) && index != ConversationIndex.Empty && index.Components != null && index.Components.Count == 1)
							{
								firstMessageInConversation = true;
							}
						}
						else if (propValue.PropType == PropType.String)
						{
							text = propValue.GetString();
						}
					}
					else if (propTag != PropTag.MessageFlags)
					{
						if (propTag != PropTag.InternetArticleNumber)
						{
							if (propTag == PropTag.ConversationId)
							{
								if (propValue.PropType == PropType.Binary)
								{
									conversationId = ConversationId.Create(propValue.GetBytes());
								}
							}
						}
						else
						{
							if (propValue.PropType != PropType.Int)
							{
								return ManifestCallbackStatus.Continue;
							}
							num = new int?(propValue.GetInt());
						}
					}
					else if (propValue.PropType == PropType.Int)
					{
						MessageFlags @int = (MessageFlags)propValue.GetInt();
						read = ((@int & MessageFlags.IsRead) == MessageFlags.IsRead);
					}
				}
			}
			if (changeType == ManifestChangeType.Add || changeType == ManifestChangeType.Change)
			{
				if (num == null)
				{
					return ManifestCallbackStatus.Continue;
				}
				StoreObjectId id = StoreObjectId.FromProviderSpecificId(entryId, (text == null) ? StoreObjectType.Unknown : ObjectClass.GetObjectType(text));
				MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(id);
				MailboxSyncWatermark mailboxSyncWatermark = MailboxSyncWatermark.CreateForSingleItem();
				mailboxSyncWatermark.UpdateWithChangeNumber(num.Value, read);
				ServerManifestEntry serverManifestEntry = this.mailboxSyncProvider.CreateItemChangeManifestEntry(mailboxSyncItemId, mailboxSyncWatermark);
				serverManifestEntry.IsNew = (changeType == ManifestChangeType.Add);
				serverManifestEntry.MessageClass = text;
				serverManifestEntry.ConversationId = conversationId;
				serverManifestEntry.FirstMessageInConversation = firstMessageInConversation;
				serverManifestEntry.FilterDate = filterDate;
				mailboxSyncItemId.ChangeKey = changeKey;
				this.lastServerManifestEntry = serverManifestEntry;
			}
			else
			{
				StoreObjectId id2 = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Unknown);
				MailboxSyncItemId mailboxSyncItemId2 = MailboxSyncItemId.CreateForExistingItem(this.mailboxSyncProvider.FolderSync, id2);
				if (mailboxSyncItemId2 == null)
				{
					return ManifestCallbackStatus.Continue;
				}
				this.lastServerManifestEntry = MailboxSyncProvider.CreateItemDeleteManifestEntry(mailboxSyncItemId2);
				this.lastServerManifestEntry.ConversationId = conversationId;
			}
			return this.CheckYieldOrStop();
		}

		public ManifestCallbackStatus Delete(byte[] entryId, bool softDelete, bool expiry)
		{
			if (ExTraceGlobals.SyncTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				this.TraceDeleteCallbackProps(entryId, softDelete, expiry);
			}
			if (softDelete)
			{
				return ManifestCallbackStatus.Continue;
			}
			StoreObjectId id = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Unknown);
			MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForExistingItem(this.mailboxSyncProvider.FolderSync, id);
			if (mailboxSyncItemId == null)
			{
				return ManifestCallbackStatus.Continue;
			}
			this.lastServerManifestEntry = MailboxSyncProvider.CreateItemDeleteManifestEntry(mailboxSyncItemId);
			return this.CheckYieldOrStop();
		}

		public ManifestCallbackStatus ReadUnread(byte[] entryId, bool read)
		{
			ManifestCallbackStatus result = ManifestCallbackStatus.Continue;
			StoreObjectId id = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Unknown);
			ISyncItemId syncItemId = MailboxSyncItemId.CreateForExistingItem(this.mailboxSyncProvider.FolderSync, id);
			if (syncItemId == null)
			{
				return result;
			}
			ServerManifestEntry serverManifestEntry = this.mailboxSyncProvider.CreateReadFlagChangeManifestEntry(syncItemId, read);
			if (serverManifestEntry != null)
			{
				this.lastServerManifestEntry = serverManifestEntry;
			}
			return this.CheckYieldOrStop();
		}

		public void TraceDeleteCallbackProps(byte[] entryId, bool softDelete, bool expiry)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append("ICS Change Callback: ");
			stringBuilder.Append("entryId=");
			stringBuilder.Append((entryId == null) ? "null" : Convert.ToBase64String(entryId));
			stringBuilder.Append(" softDelete=");
			stringBuilder.Append(softDelete.ToString());
			stringBuilder.Append(" expiry=");
			stringBuilder.Append(expiry.ToString());
			ExTraceGlobals.SyncTracer.Information<StringBuilder>((long)this.GetHashCode(), "{0}", stringBuilder);
		}

		private ManifestCallbackStatus CheckYieldOrStop()
		{
			if (this.numOperations != -1)
			{
				if (this.serverManifest.Count >= this.numOperations)
				{
					this.moreAvailable = true;
					return ManifestCallbackStatus.Stop;
				}
				if (this.serverManifest.Count >= this.numOperations - 1)
				{
					if (this.lastServerManifestEntry != null)
					{
						if (this.lastServerManifestEntry.Watermark != null)
						{
							this.syncWatermark.ChangeNumber = ((MailboxSyncWatermark)this.lastServerManifestEntry.Watermark).ChangeNumber;
						}
						this.serverManifest[this.lastServerManifestEntry.Id] = this.lastServerManifestEntry;
						this.lastServerManifestEntry = null;
					}
					return ManifestCallbackStatus.Yield;
				}
			}
			if (this.lastServerManifestEntry != null)
			{
				if (this.lastServerManifestEntry.Watermark != null)
				{
					this.syncWatermark.ChangeNumber = ((MailboxSyncWatermark)this.lastServerManifestEntry.Watermark).ChangeNumber;
				}
				this.serverManifest[this.lastServerManifestEntry.Id] = this.lastServerManifestEntry;
				this.lastServerManifestEntry = null;
			}
			return ManifestCallbackStatus.Continue;
		}

		private void TraceChangeChangeCallbackProps(byte[] entryId, byte[] sourceKey, byte[] changeKey, byte[] changeList, DateTime lastModifiedTime, ManifestChangeType changeType, bool associated, PropValue[] properties)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append("ICS Change Callback: ");
			stringBuilder.Append("entryId=");
			stringBuilder.Append((entryId == null) ? "null" : Convert.ToBase64String(entryId));
			stringBuilder.Append(" sourceKey=");
			stringBuilder.Append((sourceKey == null) ? "null" : Convert.ToBase64String(sourceKey));
			stringBuilder.Append(" changeKey=");
			stringBuilder.Append((changeKey == null) ? "null" : Convert.ToBase64String(changeKey));
			stringBuilder.Append(" changeList=");
			stringBuilder.Append((changeList == null) ? "null" : Convert.ToBase64String(changeList));
			stringBuilder.Append(" lastModifiedTime=");
			stringBuilder.Append(lastModifiedTime.ToString());
			stringBuilder.Append(" changeType=");
			stringBuilder.Append(changeType.ToString());
			stringBuilder.Append(" associated=");
			stringBuilder.Append(associated.ToString());
			stringBuilder.Append(" properties.Length=");
			stringBuilder.Append((properties == null) ? "null" : properties.Length.ToString());
			stringBuilder.Append("{");
			if (properties != null)
			{
				for (int i = 0; i < properties.Length; i++)
				{
					stringBuilder.Append("{");
					if (properties[i].IsError())
					{
						stringBuilder.Append("Error: " + properties[i].GetErrorValue().ToString());
					}
					else
					{
						stringBuilder.Append(properties[i].PropTag.ToString() + ", ");
						stringBuilder.Append(properties[i].PropType.ToString() + ", ");
						stringBuilder.Append((properties[i].Value == null) ? "null" : properties[i].Value);
					}
					stringBuilder.Append("}");
				}
			}
			stringBuilder.Append("}");
			ExTraceGlobals.SyncTracer.Information<StringBuilder>((long)this.GetHashCode(), "{0}", stringBuilder);
		}

		private static readonly PropTag[] propTags = new PropTag[]
		{
			PropTag.MessageFlags,
			PropTag.InternetArticleNumber,
			PropTag.MessageClass,
			PropTag.ConversationId,
			PropTag.ConversationIndex,
			PropTag.MessageDeliveryTime
		};

		private MailboxSyncProvider mailboxSyncProvider;

		private bool moreAvailable;

		private int numOperations;

		private Dictionary<ISyncItemId, ServerManifestEntry> serverManifest;

		private MailboxSyncWatermark syncWatermark;

		private ServerManifestEntry lastServerManifestEntry;

		private enum PropTagEnum
		{
			MessageFlags,
			ArticleId,
			MessageClass,
			ConversationId,
			ConversationIndex,
			MessageDeliveryTime,
			Total
		}
	}
}
