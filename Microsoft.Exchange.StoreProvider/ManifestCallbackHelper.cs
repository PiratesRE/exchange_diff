using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ManifestCallbackHelper : ContentsManifestCallbackHelperBase<IMapiManifestCallback>, IExchangeManifestCallback
	{
		public ManifestCallbackHelper(ManifestCheckpoint checkpoint, bool conversations) : base(conversations)
		{
			this.checkpoint = checkpoint;
		}

		public ManifestCallbackQueue<IMapiManifestCallback> SavedReads
		{
			get
			{
				return this.savedReadList;
			}
		}

		internal void SerializeReadCallbacks(Stream stream, MapiStore mapiStore)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			int value = this.SavedReads.Count + base.Reads.Count;
			binaryWriter.Write(value);
			IMapiManifestCallback callback = new ManifestCallbackHelper.CallbackSerializer(binaryWriter, mapiStore);
			this.SavedReads.ExecuteNoDequeue(callback);
			base.Reads.ExecuteNoDequeue(callback);
		}

		internal void DeserializeReadCallbacks(Stream stream, MapiStore mapiStore, bool skipOver)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			ManifestCallbackQueue<IMapiManifestCallback> savedReadsQueue = skipOver ? new ManifestCallbackQueue<IMapiManifestCallback>() : this.SavedReads;
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				ManifestCallbackHelper.CallbackSerializer.DeserializeReadCallback(binaryReader, savedReadsQueue, mapiStore);
			}
		}

		unsafe int IExchangeManifestCallback.Change(ExchangeManifestCallbackChangeFlags flags, int cpvalHeader, SPropValue* ppvalHeader, int cpvalProps, SPropValue* ppvalProps)
		{
			return base.Change(flags, cpvalHeader, ppvalHeader, cpvalProps, ppvalProps);
		}

		unsafe int IExchangeManifestCallback.Delete(ExchangeManifestCallbackDeleteFlags flags, int cElements, _CallbackInfo* lpCallbackInfo)
		{
			int i = 0;
			while (i < cElements)
			{
				byte[] entryId = new byte[lpCallbackInfo->cb];
				Marshal.Copy((IntPtr)((void*)lpCallbackInfo->pb), entryId, 0, lpCallbackInfo->cb);
				long mid = lpCallbackInfo->id;
				ExchangeManifestCallbackDeleteFlags localFlags = flags;
				base.Deletes.Enqueue(delegate(IMapiManifestCallback callback)
				{
					ManifestCallbackStatus result = callback.Delete(entryId, (localFlags & ExchangeManifestCallbackDeleteFlags.SoftDelete) == ExchangeManifestCallbackDeleteFlags.SoftDelete, (localFlags & ExchangeManifestCallbackDeleteFlags.Expiry) == ExchangeManifestCallbackDeleteFlags.Expiry);
					this.checkpoint.IdDeleted(mid);
					return result;
				});
				i++;
				lpCallbackInfo++;
			}
			return 0;
		}

		unsafe int IExchangeManifestCallback.Read(ExchangeManifestCallbackReadFlags flags, int cElements, _CallbackInfo* lpCallbackInfo)
		{
			int i = 0;
			while (i < cElements)
			{
				byte[] entryId = new byte[lpCallbackInfo->cb];
				Marshal.Copy((IntPtr)((void*)lpCallbackInfo->pb), entryId, 0, lpCallbackInfo->cb);
				bool isRead = (flags & ExchangeManifestCallbackReadFlags.Read) == ExchangeManifestCallbackReadFlags.Read;
				base.Reads.Enqueue((IMapiManifestCallback callback) => callback.ReadUnread(entryId, isRead));
				i++;
				lpCallbackInfo++;
			}
			return 0;
		}

		protected override ManifestCallbackStatus DoChangeCallback(IMapiManifestCallback callback, ManifestChangeType changeType, PropValue[] headerProps, PropValue[] messageProps)
		{
			ManifestCallbackStatus result = ManifestCallbackStatus.Continue;
			if (base.Conversations)
			{
				if (changeType != (ManifestChangeType)0)
				{
					result = callback.Change(headerProps[4].GetBytes(), null, null, null, headerProps[2].GetDateTime(), changeType, false, messageProps);
				}
				this.checkpoint.CnSeen(false, headerProps[1].GetLong());
			}
			else if (headerProps.Length == 9)
			{
				result = callback.Change(headerProps[8].GetBytes(), headerProps[0].GetBytes(), headerProps[2].GetBytes(), headerProps[3].GetBytes(), headerProps[1].GetDateTime(), changeType, headerProps[4].GetBoolean(), messageProps);
				this.checkpoint.IdGiven(headerProps[5].GetLong());
				this.checkpoint.CnSeen(headerProps[4].GetBoolean(), headerProps[7].GetLong());
			}
			else
			{
				result = callback.Change(headerProps[9].GetBytes(), headerProps[0].GetBytes(), headerProps[2].GetBytes(), headerProps[3].GetBytes(), headerProps[1].GetDateTime(), changeType, headerProps[4].GetBoolean(), messageProps);
				this.checkpoint.IdGiven(headerProps[5].GetLong());
				this.checkpoint.CnSeen(headerProps[4].GetBoolean(), headerProps[7].GetLong());
				this.checkpoint.CnRead(headerProps[8].GetLong());
			}
			return result;
		}

		private readonly ManifestCheckpoint checkpoint;

		private readonly ManifestCallbackQueue<IMapiManifestCallback> savedReadList = new ManifestCallbackQueue<IMapiManifestCallback>();

		private sealed class CallbackSerializer : IMapiManifestCallback
		{
			public CallbackSerializer(BinaryWriter writer, MapiStore mapiStore)
			{
				this.writer = writer;
				this.mapiStore = mapiStore;
			}

			public static void DeserializeReadCallback(BinaryReader reader, ManifestCallbackQueue<IMapiManifestCallback> savedReadsQueue, MapiStore mapiStore)
			{
				int num = reader.ReadInt32();
				byte[] entryId;
				if (num > 0)
				{
					byte[] entryId2 = reader.ReadBytes(num);
					entryId = mapiStore.ExpandEntryId(entryId2);
				}
				else
				{
					entryId = Array<byte>.Empty;
				}
				bool read = reader.ReadBoolean();
				savedReadsQueue.Enqueue((IMapiManifestCallback callback) => callback.ReadUnread(entryId, read));
			}

			public ManifestCallbackStatus Change(byte[] entryId, byte[] sourceKey, byte[] changeKey, byte[] changeList, DateTime lastModificationTime, ManifestChangeType changeType, bool associated, PropValue[] props)
			{
				throw new NotSupportedException();
			}

			public ManifestCallbackStatus Delete(byte[] entryId, bool softDelete, bool expiry)
			{
				throw new NotSupportedException();
			}

			public ManifestCallbackStatus ReadUnread(byte[] entryId, bool read)
			{
				byte[] array = (entryId != null) ? this.mapiStore.CompressEntryId(entryId) : null;
				int num = (array != null) ? array.Length : 0;
				this.writer.Write(num);
				if (num > 0)
				{
					this.writer.Write(array);
				}
				this.writer.Write(read);
				return ManifestCallbackStatus.Continue;
			}

			private readonly BinaryWriter writer;

			private readonly MapiStore mapiStore;
		}
	}
}
