using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class StorageFxProxyPool : FxProxyPool<StorageFxProxyPool.FolderEntry, StorageFxProxyPool.MessageEntry>
	{
		public StorageFxProxyPool(StorageDestinationMailbox destMailbox, ICollection<byte[]> folderIds) : base(folderIds)
		{
			this.destMailbox = destMailbox;
		}

		protected override StorageFxProxyPool.FolderEntry CreateFolder(FolderRec folderRec)
		{
			IDestinationMailbox destinationMailbox = this.destMailbox;
			byte[] folderID;
			destinationMailbox.CreateFolder(folderRec, CreateFolderFlags.None, out folderID);
			return this.OpenFolder(folderID);
		}

		protected override StorageFxProxyPool.FolderEntry OpenFolder(byte[] folderId)
		{
			StorageDestinationFolder folder = this.destMailbox.GetFolder<StorageDestinationFolder>(folderId);
			if (folder == null)
			{
				return null;
			}
			return StorageFxProxyPool.FolderEntry.Wrap(folder, this.destMailbox.Flags.HasFlag(LocalMailboxFlags.Move));
		}

		protected override void MailboxSetItemProperties(ItemPropertiesBase props)
		{
			if (props != null)
			{
				props.Apply(this.destMailbox.PSHandler, (MailboxSession)this.destMailbox.StoreSession);
			}
		}

		protected override byte[] FolderGetObjectData(StorageFxProxyPool.FolderEntry folder)
		{
			return MapiUtils.MapiFolderObjectData;
		}

		protected override void FolderProcessRequest(StorageFxProxyPool.FolderEntry entry, FxOpcodes opcode, byte[] request)
		{
			entry.Proxy.ProcessRequest(opcode, request);
		}

		protected override void FolderSetProps(StorageFxProxyPool.FolderEntry folder, PropValueData[] pvda)
		{
			this.SetProps(folder.WrappedObject.FxFolder.PropertyBag, pvda);
		}

		protected override void FolderSetItemProperties(StorageFxProxyPool.FolderEntry folder, ItemPropertiesBase props)
		{
			if (props != null)
			{
				props.Apply(folder.WrappedObject.CoreFolder);
			}
		}

		protected override StorageFxProxyPool.MessageEntry FolderOpenMessage(StorageFxProxyPool.FolderEntry folder, byte[] entryID)
		{
			throw new NotImplementedException("FolderOpenMessage");
		}

		protected override StorageFxProxyPool.MessageEntry FolderCreateMessage(StorageFxProxyPool.FolderEntry folder, bool isAssociated)
		{
			IMessage message = folder.WrappedObject.FxFolder.CreateMessage(isAssociated);
			return StorageFxProxyPool.MessageEntry.Wrap(message as MessageAdaptor, this.destMailbox.Flags.HasFlag(LocalMailboxFlags.Move));
		}

		protected override void FolderDeleteMessage(StorageFxProxyPool.FolderEntry folder, byte[] entryID)
		{
			folder.WrappedObject.CoreFolder.DeleteItems(DeleteItemFlags.HardDelete, new StoreObjectId[]
			{
				StoreObjectId.FromProviderSpecificId(entryID)
			});
		}

		protected override byte[] MessageGetObjectData(StorageFxProxyPool.MessageEntry message)
		{
			return StorageMessageProxy.ObjectData;
		}

		protected override void MessageProcessRequest(StorageFxProxyPool.MessageEntry message, FxOpcodes opcode, byte[] request)
		{
			message.Proxy.ProcessRequest(opcode, request);
		}

		protected override void MessageSetProps(StorageFxProxyPool.MessageEntry entry, PropValueData[] pvda)
		{
			if (entry.MimeStream != null || entry.CachedItemProperties.Count > 0)
			{
				entry.CachedPropValues.AddRange(pvda);
				return;
			}
			this.SetProps(entry.WrappedObject.PropertyBag, pvda);
		}

		protected override void MessageSetItemProperties(StorageFxProxyPool.MessageEntry message, ItemPropertiesBase props)
		{
			if (props != null)
			{
				message.CachedItemProperties.Add(props);
			}
		}

		protected override byte[] MessageSaveChanges(StorageFxProxyPool.MessageEntry entry)
		{
			CoreItem referencedObject = entry.WrappedObject.ReferenceCoreItem.ReferencedObject;
			if (entry.MimeStream != null || entry.CachedItemProperties.Count > 0)
			{
				using (Item item = new Item(referencedObject, true))
				{
					if (entry.MimeStream != null)
					{
						InboundConversionOptions scopedInboundConversionOptions = MapiUtils.GetScopedInboundConversionOptions(this.destMailbox.StoreSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
						using (entry.MimeStream)
						{
							ItemConversion.ConvertAnyMimeToItem(item, entry.MimeStream, scopedInboundConversionOptions);
						}
					}
					foreach (ItemPropertiesBase itemPropertiesBase in entry.CachedItemProperties)
					{
						itemPropertiesBase.Apply((MailboxSession)this.destMailbox.StoreSession, item);
					}
				}
			}
			if (entry.CachedPropValues.Count > 0)
			{
				this.SetProps(entry.WrappedObject.PropertyBag, entry.CachedPropValues.ToArray());
			}
			entry.WrappedObject.Save();
			referencedObject.PropertyBag.Load(StorageFxProxyPool.EntryIdPropDef);
			return referencedObject.PropertyBag[StorageFxProxyPool.EntryIdPropDef[0]] as byte[];
		}

		protected override void MessageWriteToMime(StorageFxProxyPool.MessageEntry entry, byte[] buffer)
		{
			if (entry.MimeStream == null)
			{
				entry.MimeStream = TemporaryStorage.Create();
			}
			entry.MimeStream.Write(buffer, 0, buffer.Length);
		}

		private static MessageFlags UpdateMessageFlags(IPropertyBag propertyBag, MessageFlags flagsFromSource)
		{
			MessageFlags messageFlags = MessageFlags.None;
			AnnotatedPropertyValue annotatedProperty = propertyBag.GetAnnotatedProperty(PropertyTag.MessageFlags);
			if (!annotatedProperty.PropertyValue.IsError)
			{
				messageFlags = (MessageFlags)((int)annotatedProperty.PropertyValue.Value);
			}
			if (flagsFromSource.HasFlag(MessageFlags.Read))
			{
				messageFlags |= MessageFlags.Read;
			}
			else
			{
				messageFlags &= ~MessageFlags.Read;
			}
			if (flagsFromSource.HasFlag(MessageFlags.Unsent))
			{
				messageFlags |= MessageFlags.Unsent;
			}
			else
			{
				messageFlags &= ~MessageFlags.Unsent;
			}
			return messageFlags;
		}

		private static bool ShouldUpdateIconIndex(IPropertyBag propertyBag, IconIndex iconIndexFromSource)
		{
			if (iconIndexFromSource.Equals(IconIndex.BaseMail))
			{
				AnnotatedPropertyValue annotatedProperty = propertyBag.GetAnnotatedProperty(new PropertyTag(276824067U));
				if (annotatedProperty.PropertyValue.IsError)
				{
					return false;
				}
				IconIndex iconIndex = (IconIndex)annotatedProperty.PropertyValue.Value;
				if (iconIndex != IconIndex.MailReplied && iconIndex != IconIndex.MailForwarded)
				{
					return false;
				}
			}
			return true;
		}

		[Conditional("Debug")]
		private void ValidateMessageEntry(StorageFxProxyPool.MessageEntry entry)
		{
			bool isOlcSync = this.destMailbox.IsOlcSync;
		}

		private void SetProps(IPropertyBag propertyBag, PropValueData[] pvda)
		{
			if (pvda == null)
			{
				return;
			}
			int i = 0;
			while (i < pvda.Length)
			{
				PropValueData propValueData = pvda[i];
				object obj = propValueData.Value;
				if (obj is DateTime)
				{
					obj = new ExDateTime(ExTimeZone.TimeZoneFromKind(((DateTime)obj).Kind), (DateTime)obj);
				}
				PropTag propTag = (PropTag)propValueData.PropTag;
				PropTag propTag2 = propTag;
				if (propTag2 == PropTag.MessageFlags)
				{
					MessageFlags messageFlags = StorageFxProxyPool.UpdateMessageFlags(propertyBag, (MessageFlags)((int)obj));
					obj = (int)messageFlags;
					goto IL_8B;
				}
				if (propTag2 != (PropTag)276824067U)
				{
					goto IL_8B;
				}
				if (StorageFxProxyPool.ShouldUpdateIconIndex(propertyBag, (IconIndex)obj))
				{
					goto Block_5;
				}
				IL_B5:
				i++;
				continue;
				Block_5:
				try
				{
					IL_8B:
					propertyBag.SetProperty(new PropertyValue(new PropertyTag((uint)propValueData.PropTag), obj));
				}
				catch (ArgumentException ex)
				{
					throw new ExArgumentException(ex.Message, ex);
				}
				goto IL_B5;
			}
		}

		private static readonly PropertyTagPropertyDefinition[] EntryIdPropDef = new PropertyTagPropertyDefinition[]
		{
			PropertyTagPropertyDefinition.CreateCustom("EntryId", 268370178U)
		};

		private StorageDestinationMailbox destMailbox;

		internal abstract class StorageEntry<T> : DisposableWrapper<T> where T : class, IDisposable
		{
			protected StorageEntry(T entry) : base(entry, true)
			{
			}

			public IMapiFxProxy Proxy { get; protected set; }

			protected override void InternalDispose(bool disposing)
			{
				if (disposing && this.Proxy != null)
				{
					this.Proxy.Dispose();
					this.Proxy = null;
				}
				base.InternalDispose(disposing);
			}
		}

		internal class FolderEntry : StorageFxProxyPool.StorageEntry<StorageDestinationFolder>
		{
			private FolderEntry(StorageDestinationFolder folder, bool isMoveUser) : base(folder)
			{
				base.Proxy = new StorageFolderProxy(folder, isMoveUser);
			}

			public static StorageFxProxyPool.FolderEntry Wrap(StorageDestinationFolder folder, bool isMoveUser)
			{
				if (folder != null)
				{
					return new StorageFxProxyPool.FolderEntry(folder, isMoveUser);
				}
				return null;
			}
		}

		internal class MessageEntry : StorageFxProxyPool.StorageEntry<MessageAdaptor>
		{
			private MessageEntry(MessageAdaptor message, bool isMoveUser) : base(message)
			{
				base.Proxy = new StorageMessageProxy(message, isMoveUser);
				this.MimeStream = null;
				this.CachedPropValues = new List<PropValueData>(10);
				this.CachedItemProperties = new List<ItemPropertiesBase>(1);
			}

			internal Stream MimeStream { get; set; }

			internal List<PropValueData> CachedPropValues { get; private set; }

			internal List<ItemPropertiesBase> CachedItemProperties { get; private set; }

			public static StorageFxProxyPool.MessageEntry Wrap(MessageAdaptor message, bool isMoveUser)
			{
				if (message != null)
				{
					return new StorageFxProxyPool.MessageEntry(message, isMoveUser);
				}
				return null;
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					if (this.MimeStream != null)
					{
						this.MimeStream.Dispose();
						this.MimeStream = null;
					}
					base.InternalDispose(disposing);
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<StorageFxProxyPool.MessageEntry>(this);
			}
		}
	}
}
