using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class HierarchySyncMetadataItem : Item, IHierarchySyncMetadataItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal HierarchySyncMetadataItem(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return HierarchySyncMetadataItemSchema.Instance;
			}
		}

		public ExDateTime LastAttemptedSyncTime
		{
			get
			{
				this.CheckDisposed("LastAttemptedSyncTime::get");
				return base.GetValueOrDefault<ExDateTime>(HierarchySyncMetadataItemSchema.LastAttemptedSyncTime);
			}
			set
			{
				this.CheckDisposed("LastAttemptedSyncTime::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.LastAttemptedSyncTime, value);
			}
		}

		public ExDateTime LastFailedSyncTime
		{
			get
			{
				this.CheckDisposed("LastFailedSyncTime::get");
				return base.GetValueOrDefault<ExDateTime>(HierarchySyncMetadataItemSchema.LastFailedSyncTime);
			}
			set
			{
				this.CheckDisposed("LastFailedSyncTime::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.LastFailedSyncTime, value);
			}
		}

		public ExDateTime LastSuccessfulSyncTime
		{
			get
			{
				this.CheckDisposed("LastSuccessfulSyncTime::get");
				return base.GetValueOrDefault<ExDateTime>(HierarchySyncMetadataItemSchema.LastSuccessfulSyncTime);
			}
			set
			{
				this.CheckDisposed("LastSuccessfulSyncTime::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.LastSuccessfulSyncTime, value);
			}
		}

		public ExDateTime FirstFailedSyncTimeAfterLastSuccess
		{
			get
			{
				this.CheckDisposed("FirstFailedSyncTimeAfterLastSuccess::get");
				return base.GetValueOrDefault<ExDateTime>(HierarchySyncMetadataItemSchema.FirstFailedSyncTimeAfterLastSuccess);
			}
			set
			{
				this.CheckDisposed("FirstFailedSyncTimeAfterLastSuccess::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.FirstFailedSyncTimeAfterLastSuccess, value);
			}
		}

		public string LastSyncFailure
		{
			get
			{
				this.CheckDisposed("LastSyncFailure::get");
				return base.GetValueOrDefault<string>(HierarchySyncMetadataItemSchema.LastSyncFailure);
			}
			set
			{
				this.CheckDisposed("LastSyncFailure::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.LastSyncFailure, value);
			}
		}

		public int NumberOfAttemptsAfterLastSuccess
		{
			get
			{
				this.CheckDisposed("NumberOfAttemptsAfterLastSuccess::get");
				return base.GetValueOrDefault<int>(HierarchySyncMetadataItemSchema.NumberOfAttemptsAfterLastSuccess);
			}
			set
			{
				this.CheckDisposed("NumberOfAttemptsAfterLastSuccess::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.NumberOfAttemptsAfterLastSuccess, value);
			}
		}

		public int NumberOfBatchesExecuted
		{
			get
			{
				this.CheckDisposed("NumberOfBatchesExecuted::get");
				return base.GetValueOrDefault<int>(HierarchySyncMetadataItemSchema.NumberOfBatchesExecuted);
			}
			set
			{
				this.CheckDisposed("NumberOfBatchesExecuted::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.NumberOfBatchesExecuted, value);
			}
		}

		public int NumberOfFoldersSynced
		{
			get
			{
				this.CheckDisposed("NumberOfFoldersSynced::get");
				return base.GetValueOrDefault<int>(HierarchySyncMetadataItemSchema.NumberOfFoldersSynced);
			}
			set
			{
				this.CheckDisposed("NumberOfFoldersSynced::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.NumberOfFoldersSynced, value);
			}
		}

		public int NumberOfFoldersToBeSynced
		{
			get
			{
				this.CheckDisposed("NumberOfFoldersToBeSynced::get");
				return base.GetValueOrDefault<int>(HierarchySyncMetadataItemSchema.NumberOfFoldersToBeSynced);
			}
			set
			{
				this.CheckDisposed("NumberOfFoldersToBeSynced::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.NumberOfFoldersToBeSynced, value);
			}
		}

		public int BatchSize
		{
			get
			{
				this.CheckDisposed("BatchSize::get");
				return base.GetValueOrDefault<int>(HierarchySyncMetadataItemSchema.BatchSize);
			}
			set
			{
				this.CheckDisposed("BatchSize::set");
				base.SetOrDeleteProperty(HierarchySyncMetadataItemSchema.BatchSize, value);
			}
		}

		public void SetPartiallyCommittedFolderIds(IdSet value)
		{
			this.CheckDisposed("SetPartiallyCommittedFolderIds");
			using (Stream stream = HierarchySyncMetadataItem.SyncMetadataAttachmentStream.CreateAttachment(this, "PartiallyCommittedFolderIds", true))
			{
				if (value != null)
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
					{
						value.SerializeWithReplGuids(streamWriter);
					}
				}
			}
		}

		public IdSet GetPartiallyCommittedFolderIds()
		{
			this.CheckDisposed("GetPartiallyCommittedFolderIds::get");
			IdSet result = null;
			using (Stream existingAttachment = HierarchySyncMetadataItem.SyncMetadataAttachmentStream.GetExistingAttachment(this, "PartiallyCommittedFolderIds", StreamBase.Capabilities.Readable))
			{
				if (existingAttachment != null)
				{
					using (StreamReader streamReader = new StreamReader(existingAttachment))
					{
						result = IdSet.ParseWithReplGuids(streamReader);
					}
				}
			}
			return result;
		}

		private new static HierarchySyncMetadataItem Bind(StoreSession session, StoreId itemId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<HierarchySyncMetadataItem>(session, itemId, HierarchySyncMetadataItemSchema.Instance, propsToReturn);
		}

		private static HierarchySyncMetadataItem Create(StoreSession session, StoreId folderId)
		{
			HierarchySyncMetadataItem hierarchySyncMetadataItem = ItemBuilder.CreateNewItem<HierarchySyncMetadataItem>(session, folderId, ItemCreateInfo.HierarchySyncMetadataInfo, CreateMessageType.Associated);
			hierarchySyncMetadataItem[StoreObjectSchema.ItemClass] = "IPM.HierarchySync.Metadata";
			return hierarchySyncMetadataItem;
		}

		public Stream GetSyncStateReadStream()
		{
			this.CheckDisposed("GetSyncStateReadStream");
			return HierarchySyncMetadataItem.SyncMetadataAttachmentStream.GetExistingAttachment(this, "SyncState", StreamBase.Capabilities.Readable);
		}

		public Stream GetSyncStateOverrideStream()
		{
			this.CheckDisposed("GetSyncStateOverrideStream");
			return HierarchySyncMetadataItem.SyncMetadataAttachmentStream.CreateAttachment(this, "SyncState", true);
		}

		public Stream GetFinalJobSyncStateReadStream()
		{
			this.CheckDisposed("GetFinalJobSyncStateReadStream");
			return HierarchySyncMetadataItem.SyncMetadataAttachmentStream.GetExistingAttachment(this, "FinalJobSyncState", StreamBase.Capabilities.Readable);
		}

		public Stream GetFinalJobSyncStateWriteStream(bool overrideIfExisting)
		{
			this.CheckDisposed("GetFinalJobSyncStateWriteStream");
			return HierarchySyncMetadataItem.SyncMetadataAttachmentStream.CreateAttachment(this, "FinalJobSyncState", overrideIfExisting);
		}

		public void CommitSyncStateForCompletedBatch()
		{
			this.CheckDisposed("CommitSyncStateForCompletedBatch");
			this.DeleteAttachment("SyncState");
			this.RenameAttachment("FinalJobSyncState", "SyncState");
		}

		public void ClearSyncState()
		{
			this.CheckDisposed("DeleteSyncState");
			this.DeleteAttachment("SyncState");
			this.DeleteAttachment("FinalJobSyncState");
			this.DeleteAttachment("PartiallyCommittedFolderIds");
		}

		private StreamAttachment GetStreamAttachment(string name)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				foreach (AttachmentHandle handle in base.AttachmentCollection)
				{
					Attachment attachment = base.AttachmentCollection.Open(handle, null);
					disposeGuard.Add<Attachment>(attachment);
					if (attachment.AttachmentType == AttachmentType.Stream && !string.IsNullOrWhiteSpace(attachment.FileName) && attachment.FileName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						StreamAttachment streamAttachment = attachment as StreamAttachment;
						if (streamAttachment != null)
						{
							disposeGuard.Success();
							return streamAttachment;
						}
					}
					attachment.Dispose();
				}
			}
			return null;
		}

		private StreamAttachment CreateStreamAttachment(string name)
		{
			base.OpenAsReadWrite();
			StreamAttachment result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				StreamAttachment streamAttachment = base.AttachmentCollection.Create(AttachmentType.Stream) as StreamAttachment;
				disposeGuard.Add<StreamAttachment>(streamAttachment);
				streamAttachment.FileName = name;
				disposeGuard.Success();
				result = streamAttachment;
			}
			return result;
		}

		private void DeleteAttachment(string attachmentName)
		{
			base.OpenAsReadWrite();
			using (StreamAttachment streamAttachment = this.GetStreamAttachment(attachmentName))
			{
				if (streamAttachment != null)
				{
					HierarchySyncMetadataItem.Tracer.TraceDebug<string, AttachmentId>((long)this.GetHashCode(), "HierarchySyncMetadataItem:DeleteAttachment - Removing attachment with name='{0}' and Id='{1}'", attachmentName, streamAttachment.Id);
					base.AttachmentCollection.Remove(streamAttachment.Id);
				}
				else
				{
					HierarchySyncMetadataItem.Tracer.TraceDebug<string>((long)this.GetHashCode(), "HierarchySyncMetadataItem:DeleteAttachment - Didn't find an stream attachment with name='{0}'", attachmentName);
				}
			}
		}

		private void RenameAttachment(string originalAttachmentName, string newAttachmentName)
		{
			base.OpenAsReadWrite();
			using (StreamAttachment streamAttachment = this.GetStreamAttachment(originalAttachmentName))
			{
				if (streamAttachment != null)
				{
					HierarchySyncMetadataItem.Tracer.TraceDebug<AttachmentId, string, string>((long)this.GetHashCode(), "HierarchySyncMetadataItem:RenameAttachment - Renaming attachment with Id='{0}'. Old name='{1}', new name='{2}'.", streamAttachment.Id, originalAttachmentName, newAttachmentName);
					streamAttachment.FileName = newAttachmentName;
					streamAttachment.Save();
				}
				else
				{
					HierarchySyncMetadataItem.Tracer.TraceDebug<string>((long)this.GetHashCode(), "HierarchySyncMetadataItem:RenameAttachment - Didn't find an stream attachment with name='{0}'", originalAttachmentName);
				}
			}
		}

		internal const string PartiallyCommittedFolderIdsAttachmentName = "PartiallyCommittedFolderIds";

		internal const string SyncStateAttachmentName = "SyncState";

		internal const string FinalJobSyncStateAttachmentName = "FinalJobSyncState";

		private const int ItemQueryBatchSize = 100;

		private static readonly Trace Tracer = ExTraceGlobals.PublicFoldersTracer;

		private static readonly PropertyDefinition[] ItemQueryColumns = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.LastModifiedTime,
			StoreObjectSchema.ItemClass
		};

		private static readonly SortBy[] ItemQuerySortOrder = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private class SyncMetadataAttachmentStream : StreamWrapper
		{
			private SyncMetadataAttachmentStream(StreamAttachment streamAttachment, Stream contentStream, StreamBase.Capabilities streamCapabilities) : base(contentStream, true, streamCapabilities | StreamBase.Capabilities.Seekable)
			{
				ArgumentValidator.ThrowIfNull("streamAttachment", streamAttachment);
				ArgumentValidator.ThrowIfNull("contentStream", contentStream);
				this.streamAttachment = streamAttachment;
				this.shouldSaveOnDispose = ((streamCapabilities & StreamBase.Capabilities.Writable) == StreamBase.Capabilities.Writable);
			}

			private static HierarchySyncMetadataItem.SyncMetadataAttachmentStream Instantate(StreamAttachment attachment, StreamBase.Capabilities streamCapabilities)
			{
				if (attachment == null)
				{
					HierarchySyncMetadataItem.Tracer.TraceDebug(0L, "SyncMetadataAttachmentStream:Instantate - Returning null stream as attachment was also null");
					return null;
				}
				HierarchySyncMetadataItem.SyncMetadataAttachmentStream result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					disposeGuard.Add<StreamAttachment>(attachment);
					PropertyOpenMode propertyOpenMode = ((streamCapabilities & StreamBase.Capabilities.Writable) == StreamBase.Capabilities.Writable) ? PropertyOpenMode.Modify : PropertyOpenMode.ReadOnly;
					HierarchySyncMetadataItem.Tracer.TraceDebug<PropertyOpenMode, StreamBase.Capabilities>(0L, "SyncMetadataAttachmentStream:Instantate - Getting content stream. Open Mode={0}, Stream Capabilities={1}", propertyOpenMode, streamCapabilities);
					Stream contentStream = attachment.GetContentStream(propertyOpenMode);
					disposeGuard.Add<Stream>(contentStream);
					HierarchySyncMetadataItem.SyncMetadataAttachmentStream syncMetadataAttachmentStream = new HierarchySyncMetadataItem.SyncMetadataAttachmentStream(attachment, contentStream, streamCapabilities);
					disposeGuard.Success();
					result = syncMetadataAttachmentStream;
				}
				return result;
			}

			public static HierarchySyncMetadataItem.SyncMetadataAttachmentStream GetExistingAttachment(HierarchySyncMetadataItem item, string attachmentName, StreamBase.Capabilities streamCapabilities)
			{
				ArgumentValidator.ThrowIfNull("item", item);
				ArgumentValidator.ThrowIfNullOrWhiteSpace("attachmentName", attachmentName);
				return HierarchySyncMetadataItem.SyncMetadataAttachmentStream.Instantate(item.GetStreamAttachment(attachmentName), streamCapabilities);
			}

			public static HierarchySyncMetadataItem.SyncMetadataAttachmentStream CreateAttachment(HierarchySyncMetadataItem item, string attachmentName, bool overrideIfExisting)
			{
				ArgumentValidator.ThrowIfNull("item", item);
				ArgumentValidator.ThrowIfNullOrWhiteSpace("attachmentName", attachmentName);
				using (StreamAttachment streamAttachment = item.GetStreamAttachment(attachmentName))
				{
					if (streamAttachment != null)
					{
						if (!overrideIfExisting)
						{
							HierarchySyncMetadataItem.Tracer.TraceDebug(0L, "SyncMetadataAttachmentStream:CreateAttachment - Skiping creation of sync state attachment and one already exist and override was not selected");
							return null;
						}
						item.DeleteAttachment(attachmentName);
					}
				}
				return HierarchySyncMetadataItem.SyncMetadataAttachmentStream.Instantate(item.CreateStreamAttachment(attachmentName), StreamBase.Capabilities.Writable);
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (this.streamAttachment != null)
				{
					if (this.shouldSaveOnDispose)
					{
						this.streamAttachment.Save();
					}
					this.streamAttachment.Dispose();
					this.streamAttachment = null;
				}
			}

			private readonly bool shouldSaveOnDispose;

			private StreamAttachment streamAttachment;
		}
	}
}
