using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MoveObjectInfo<T> : DisposeTrackableBase where T : class
	{
		public MoveObjectInfo(Guid mdbGuid, MapiStore store, byte[] messageId, string folderName, string messageClass, string subject, byte[] searchKey)
		{
			this.store = store;
			this.MessageId = messageId;
			this.FolderId = null;
			this.message = null;
			this.folderName = folderName;
			this.messageClass = messageClass;
			this.searchKey = searchKey;
			this.subject = subject;
		}

		public byte[] MessageId { get; private set; }

		public byte[] FolderId { get; private set; }

		public bool MessageFound
		{
			get
			{
				return this.message != null;
			}
		}

		public DateTime CreationTimestamp
		{
			get
			{
				if (this.message == null)
				{
					return DateTime.MinValue;
				}
				return this.message.GetProp(PropTag.CreationTime).GetDateTime();
			}
		}

		public static List<T> LoadAll(Guid mdbGuid, MapiStore store, string folderName)
		{
			return MoveObjectInfo<T>.LoadAll(null, mdbGuid, store, folderName);
		}

		public static List<T> LoadAll(byte[] searchKey, Guid mdbGuid, MapiStore store, string folderName)
		{
			return MoveObjectInfo<T>.LoadAll(searchKey, mdbGuid, store, folderName, null, null);
		}

		public static List<T> LoadAll(byte[] searchKey, Guid mdbGuid, MapiStore store, string folderName, MoveObjectInfo<T>.IsSupportedObjectTypeDelegate isSupportedObjectType, MoveObjectInfo<T>.EmptyTDelegate emptyT)
		{
			return MoveObjectInfo<T>.LoadAll(searchKey, null, mdbGuid, store, folderName, isSupportedObjectType, emptyT);
		}

		public static List<T> LoadAll(byte[] searchKey, Restriction additionalRestriction, Guid mdbGuid, MapiStore store, string folderName, MoveObjectInfo<T>.IsSupportedObjectTypeDelegate isSupportedObjectType, MoveObjectInfo<T>.EmptyTDelegate emptyT)
		{
			List<T> list = new List<T>();
			using (MapiFolder mapiFolder = MapiUtils.OpenFolderUnderRoot(store, folderName, false))
			{
				if (mapiFolder == null)
				{
					return list;
				}
				using (MapiTable contentsTable = mapiFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
				{
					PropTag propTag = PropTag.ReplyTemplateID;
					contentsTable.SortTable(new SortOrder(propTag, SortFlags.Ascend), SortTableFlags.None);
					List<PropTag> list2 = new List<PropTag>();
					list2.Add(PropTag.EntryId);
					list2.Add(propTag);
					Restriction restriction = null;
					if (searchKey != null)
					{
						restriction = Restriction.EQ(propTag, searchKey);
					}
					if (additionalRestriction != null)
					{
						if (restriction == null)
						{
							restriction = additionalRestriction;
						}
						else
						{
							restriction = Restriction.And(new Restriction[]
							{
								restriction,
								additionalRestriction
							});
						}
					}
					foreach (PropValue[] array2 in MapiUtils.QueryAllRows(contentsTable, restriction, list2))
					{
						byte[] bytes = array2[0].GetBytes();
						byte[] bytes2 = array2[1].GetBytes();
						OpenEntryFlags flags = OpenEntryFlags.Modify | OpenEntryFlags.DontThrowIfEntryIsMissing;
						using (MapiMessage mapiMessage = (MapiMessage)store.OpenEntry(bytes, flags))
						{
							if (mapiMessage != null)
							{
								T t = default(T);
								if (isSupportedObjectType != null)
								{
									if (isSupportedObjectType(mapiMessage, store))
									{
										t = MoveObjectInfo<T>.ReadObjectFromMessage(mapiMessage, false);
									}
									if (t == null && emptyT != null)
									{
										t = emptyT(bytes2);
									}
								}
								else
								{
									t = MoveObjectInfo<T>.ReadObjectFromMessage(mapiMessage, false);
								}
								if (t != null)
								{
									list.Add(t);
								}
								else
								{
									MrsTracer.Common.Error("Unable to deserialize message '{0}'.", new object[]
									{
										bytes
									});
								}
							}
						}
					}
				}
			}
			return list;
		}

		public T ReadObject(ReadObjectFlags flags)
		{
			List<T> list = this.ReadObjectChunks(flags | ReadObjectFlags.LastChunkOnly);
			if (list == null || list.Count <= 0)
			{
				return default(T);
			}
			return list[list.Count - 1];
		}

		public List<T> ReadObjectChunks(ReadObjectFlags flags)
		{
			if ((flags & ReadObjectFlags.Refresh) != ReadObjectFlags.None)
			{
				this.message.Dispose();
				this.message = null;
			}
			if (this.message == null && !this.OpenMessage())
			{
				return null;
			}
			List<T> list = new List<T>();
			T t;
			if ((flags & ReadObjectFlags.LastChunkOnly) == ReadObjectFlags.None)
			{
				using (MapiTable attachmentTable = this.message.GetAttachmentTable())
				{
					if (attachmentTable != null)
					{
						PropValue[][] array = attachmentTable.QueryAllRows(null, MoveObjectInfo<T>.AttachmentTagsToLoad);
						foreach (PropValue[] array3 in array)
						{
							int @int = array3[0].GetInt();
							using (MapiAttach mapiAttach = this.message.OpenAttach(@int))
							{
								using (MapiStream mapiStream = mapiAttach.OpenStream(PropTag.AttachDataBin, OpenPropertyFlags.BestAccess))
								{
									t = MoveObjectInfo<T>.DeserializeFromStream(mapiStream, (flags & ReadObjectFlags.DontThrowOnCorruptData) == ReadObjectFlags.None);
								}
								if (t != null)
								{
									list.Add(t);
								}
							}
						}
					}
				}
			}
			t = MoveObjectInfo<T>.ReadObjectFromMessage(this.message, (flags & ReadObjectFlags.DontThrowOnCorruptData) == ReadObjectFlags.None);
			if (t != null)
			{
				list.Add(t);
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}

		public bool CheckIfUnderlyingMessageHasChanged()
		{
			PropValue prop = this.message.GetProp(PropTag.ChangeKey);
			bool result;
			using (MapiMessage mapiMessage = (MapiMessage)this.store.OpenEntry(this.MessageId))
			{
				PropValue prop2 = mapiMessage.GetProp(PropTag.ChangeKey);
				result = !PropValue.Equals(prop, prop2);
			}
			return result;
		}

		public bool CheckObjectType(MoveObjectInfo<T>.IsSupportedObjectTypeDelegate isSupportedObjectType)
		{
			return isSupportedObjectType(this.message, this.store);
		}

		public bool OpenMessage()
		{
			if (this.MessageId == null)
			{
				this.MessageId = this.FindMessageId();
				if (this.MessageId == null)
				{
					return false;
				}
			}
			OpenEntryFlags flags = OpenEntryFlags.Modify | OpenEntryFlags.DontThrowIfEntryIsMissing;
			this.message = (MapiMessage)this.store.OpenEntry(this.MessageId, flags);
			return this.message != null;
		}

		public void DeleteMessage()
		{
			if (this.MessageId != null)
			{
				using (MapiFolder mapiFolder = MapiUtils.OpenFolderUnderRoot(this.store, this.folderName, false))
				{
					if (mapiFolder != null)
					{
						mapiFolder.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, new byte[][]
						{
							this.MessageId
						});
					}
				}
				if (this.message != null)
				{
					this.message.Dispose();
					this.message = null;
				}
				this.MessageId = null;
				this.FolderId = null;
			}
		}

		public void DeleteOldMessages()
		{
			using (MapiFolder mapiFolder = MapiUtils.OpenFolderUnderRoot(this.store, this.folderName, false))
			{
				if (mapiFolder != null)
				{
					for (int i = 0; i < 100; i++)
					{
						byte[] array = this.FindMessageId();
						if (array == null)
						{
							break;
						}
						mapiFolder.DeleteMessages(DeleteMessagesFlags.None, new byte[][]
						{
							array
						});
					}
				}
			}
		}

		public void SaveObject(T obj, MoveObjectInfo<T>.GetAdditionalProperties getAdditionalPropertiesCallback)
		{
			this.SaveObjectChunks(new List<T>(1)
			{
				obj
			}, 1, getAdditionalPropertiesCallback);
		}

		public void SaveObjectChunks(List<T> chunks, int maxChunks, MoveObjectInfo<T>.GetAdditionalProperties getAdditionalPropertiesCallback)
		{
			if (chunks.Count > maxChunks)
			{
				MrsTracer.Common.Warning("Too many chunks supplied, truncating", new object[0]);
				chunks.RemoveRange(0, chunks.Count - maxChunks);
			}
			bool flag = false;
			if (this.message == null)
			{
				using (MapiFolder mapiFolder = MapiUtils.OpenFolderUnderRoot(this.store, this.folderName, true))
				{
					this.FolderId = mapiFolder.GetProp(PropTag.EntryId).GetBytes();
					this.message = mapiFolder.CreateMessage();
				}
				this.message.SetProps(new PropValue[]
				{
					new PropValue(PropTag.MessageClass, this.messageClass),
					new PropValue(PropTag.Subject, this.subject),
					new PropValue(PropTag.ReplyTemplateID, this.searchKey)
				});
				flag = true;
			}
			if (chunks.Count > 1)
			{
				using (MapiTable attachmentTable = this.message.GetAttachmentTable())
				{
					if (attachmentTable != null)
					{
						int num = attachmentTable.GetRowCount() - (maxChunks - chunks.Count);
						if (num > 0)
						{
							attachmentTable.SetColumns(MoveObjectInfo<T>.AttachmentTagsToLoad);
							PropValue[][] array = attachmentTable.QueryRows(num);
							for (int i = 0; i < num; i++)
							{
								this.message.DeleteAttach(array[i][0].GetInt());
							}
						}
					}
				}
				for (int j = 0; j < chunks.Count - 1; j++)
				{
					int num2;
					using (MapiAttach mapiAttach = this.message.CreateAttach(out num2))
					{
						using (MapiStream mapiStream = mapiAttach.OpenStream(PropTag.AttachDataBin, OpenPropertyFlags.Create))
						{
							MoveObjectInfo<T>.SerializeToStream(chunks[j], mapiStream);
						}
						mapiAttach.SetProps(new PropValue[]
						{
							new PropValue(PropTag.AttachFileName, string.Format("MOI_Chunk_{0:yyyymmdd_HHmmssfff}", DateTime.UtcNow)),
							new PropValue(PropTag.AttachMethod, AttachMethods.ByValue)
						});
						mapiAttach.SaveChanges();
					}
				}
			}
			T obj = chunks[chunks.Count - 1];
			if (getAdditionalPropertiesCallback != null)
			{
				this.message.SetProps(getAdditionalPropertiesCallback(this.store));
			}
			using (MapiStream mapiStream2 = this.message.OpenStream(PropTag.Body, OpenPropertyFlags.Create))
			{
				MoveObjectInfo<T>.SerializeToStream(obj, mapiStream2);
			}
			this.message.SaveChanges();
			if (flag)
			{
				this.MessageId = this.message.GetProp(PropTag.EntryId).GetBytes();
			}
		}

		public void SaveObject(T obj)
		{
			this.SaveObject(obj, null);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.message != null)
				{
					this.message.Dispose();
					this.message = null;
				}
				this.store = null;
				this.FolderId = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MoveObjectInfo<T>>(this);
		}

		private static void SerializeToStream(T obj, Stream stream)
		{
			if (obj == null)
			{
				return;
			}
			if (typeof(T) == typeof(string))
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					streamWriter.Write((string)((object)obj));
					CommonUtils.AppendNewLinesAndFlush(streamWriter);
					return;
				}
			}
			XMLSerializableBase.SerializeToStream(obj, stream, false);
			using (StreamWriter streamWriter2 = new StreamWriter(stream))
			{
				CommonUtils.AppendNewLinesAndFlush(streamWriter2);
			}
		}

		private static T DeserializeFromStream(Stream stream, bool throwOnDeserializationError)
		{
			if (typeof(T) == typeof(string))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					return (T)((object)streamReader.ReadToEnd());
				}
			}
			return XMLSerializableBase.Deserialize<T>(stream, throwOnDeserializationError);
		}

		private static T ReadObjectFromMessage(MapiMessage mapiMessage, bool throwOnDeserializationError)
		{
			T result;
			using (MapiStream mapiStream = mapiMessage.OpenStream(PropTag.Body, OpenPropertyFlags.BestAccess))
			{
				result = MoveObjectInfo<T>.DeserializeFromStream(mapiStream, throwOnDeserializationError);
			}
			return result;
		}

		private byte[] FindMessageId()
		{
			using (MapiFolder mapiFolder = MapiUtils.OpenFolderUnderRoot(this.store, this.folderName, false))
			{
				if (mapiFolder == null)
				{
					return null;
				}
				this.FolderId = mapiFolder.GetProp(PropTag.EntryId).GetBytes();
				using (MapiTable contentsTable = mapiFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
				{
					PropTag propTag = PropTag.ReplyTemplateID;
					contentsTable.SetColumns(new PropTag[]
					{
						PropTag.EntryId
					});
					contentsTable.SortTable(new SortOrder(propTag, SortFlags.Ascend), SortTableFlags.None);
					if (contentsTable.FindRow(Restriction.EQ(propTag, this.searchKey), BookMark.Beginning, FindRowFlag.None))
					{
						PropValue[][] array = contentsTable.QueryRows(1);
						if (array == null || array.Length == 0 || array[0].Length == 0)
						{
							return null;
						}
						return array[0][0].GetBytes();
					}
				}
			}
			return null;
		}

		private static readonly PropTag[] AttachmentTagsToLoad = new PropTag[]
		{
			PropTag.AttachNum
		};

		private MapiMessage message;

		private MapiStore store;

		private string messageClass;

		private string folderName;

		private byte[] searchKey;

		private string subject;

		public delegate bool IsSupportedObjectTypeDelegate(MapiMessage msg, MapiStore store);

		public delegate PropValue[] GetAdditionalProperties(MapiStore store);

		public delegate T EmptyTDelegate(byte[] searchKey);
	}
}
