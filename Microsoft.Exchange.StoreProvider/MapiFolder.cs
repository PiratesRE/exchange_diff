using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiFolder : MapiContainer
	{
		internal MapiFolder(IExMapiFolder iMAPIFolder, IMAPIFolder externalIMAPIFolder, MapiStore mapiStore) : base(iMAPIFolder, externalIMAPIFolder, mapiStore, MapiFolder.IMAPIFolderGuids)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(8274, 15, (long)this.GetHashCode(), "MapiFolder.MapiFolder: this={0}", TraceUtils.MakeHash(this));
			}
			this.iMAPIFolder = iMAPIFolder;
			this.externalIMAPIFolder = externalIMAPIFolder;
		}

		protected override void MapiInternalDispose()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(12370, 15, (long)this.GetHashCode(), "MapiFolder.InternalDispose: this={0}", TraceUtils.MakeHash(this));
			}
			this.iMAPIFolder = null;
			this.externalIMAPIFolder = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiFolder>(this);
		}

		public MapiFolder CreateFolder(string folderName, string folderComment, bool openIfExists)
		{
			return this.CreateFolder(folderName, folderComment, openIfExists, false);
		}

		public MapiFolder CreateSearchFolder(string folderName, string folderComment, bool openIfExists)
		{
			return this.CreateFolder(folderName, folderComment, openIfExists, true, null);
		}

		public MapiFolder CreateFolder(string folderName, string folderComment, bool openIfExists, bool createSearchFolder)
		{
			return this.CreateFolder(folderName, folderComment, openIfExists, createSearchFolder, null);
		}

		public MapiFolder CreateFolder(string folderName, string folderComment, bool openIfExists, bool createSearchFolder, byte[] folderId)
		{
			return this.CreateFolder(folderName, folderComment, openIfExists, createSearchFolder, false, folderId);
		}

		public MapiFolder CreateFolder(string folderName, string folderComment, bool openIfExists, bool createSearchFolder, bool instantSearch, byte[] folderId)
		{
			return this.CreateFolder(folderName, folderComment, openIfExists, createSearchFolder, instantSearch, false, folderId);
		}

		public MapiFolder CreateFolder(string folderName, string folderComment, bool openIfExists, bool createSearchFolder, bool instantSearch, bool optimizedConversationSearch, byte[] folderId)
		{
			return this.CreateFolder(folderName, folderComment, openIfExists, createSearchFolder, instantSearch, optimizedConversationSearch, false, folderId);
		}

		public MapiFolder CreateFolder(string folderName, string folderComment, bool openIfExists, bool createSearchFolder, bool instantSearch, bool optimizedConversationSearch, bool createPublicFolderDumpster, byte[] folderId)
		{
			return this.InternalCreateFolder(folderName, folderComment, openIfExists, createSearchFolder, instantSearch, optimizedConversationSearch, createPublicFolderDumpster, folderId, false);
		}

		public MapiFolder CreateSecureFolder(string folderName, string folderComment, bool openIfExists)
		{
			return this.InternalCreateFolder(folderName, folderComment, openIfExists, false, false, false, false, null, true);
		}

		public MapiFolder CreateSecureFolder(string folderName, string folderComment, bool openIfExists, byte[] folderId)
		{
			return this.InternalCreateFolder(folderName, folderComment, openIfExists, false, false, false, false, folderId, true);
		}

		private MapiFolder InternalCreateFolder(string folderName, string folderComment, bool openIfExists, bool createSearchFolder, bool instantSearch, bool optimizedConversationSearch, bool createPublicFolderDumpster, byte[] folderId, bool createInternalAccess)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (folderName == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("folderName");
			}
			base.LockStore();
			MapiFolder result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace(10322, 15, (long)this.GetHashCode(), "MapiFolder.CreateFolder params: folderName={0}, folderComment={1}, openIfExists={2}, createSearchFolder={3}, folderId={4}", new object[]
					{
						TraceUtils.MakeString(folderName),
						TraceUtils.MakeString(folderComment),
						openIfExists.ToString(),
						createSearchFolder.ToString(),
						TraceUtils.DumpEntryId(folderId)
					});
				}
				MapiFolder mapiFolder = null;
				CreateFolderFlags createFolderFlags = CreateFolderFlags.None;
				FolderType ulFolderType = FolderType.Generic;
				IExMapiFolder iExInterface = null;
				if (openIfExists)
				{
					createFolderFlags |= CreateFolderFlags.OpenIfExists;
				}
				createFolderFlags |= CreateFolderFlags.UnicodeStrings;
				if (createSearchFolder)
				{
					if (instantSearch)
					{
						createFolderFlags |= CreateFolderFlags.InstantSearch;
						if (optimizedConversationSearch)
						{
							createFolderFlags |= CreateFolderFlags.OptimizedConversationSearch;
						}
					}
					ulFolderType = FolderType.Search;
				}
				if (createPublicFolderDumpster)
				{
					createFolderFlags |= CreateFolderFlags.CreatePublicFolderDumpster;
				}
				if (createInternalAccess)
				{
					createFolderFlags |= CreateFolderFlags.InternalAccess;
				}
				try
				{
					int num;
					if (folderId == null)
					{
						num = this.iMAPIFolder.CreateFolder((int)ulFolderType, folderName, folderComment, (int)createFolderFlags, out iExInterface);
					}
					else
					{
						num = this.iMAPIFolder.CreateFolderEx((int)ulFolderType, folderName, folderComment, folderId, (int)createFolderFlags, out iExInterface);
					}
					if (num != 0)
					{
						base.ThrowIfError("Unable to create folder.", num);
					}
					mapiFolder = new MapiFolder(iExInterface, null, base.MapiStore);
					iExInterface = null;
				}
				finally
				{
					iExInterface.DisposeIfValid();
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14418, 15, (long)this.GetHashCode(), "MapiFolder.CreateFolder results: mapiFolder={0}", TraceUtils.MakeHash(mapiFolder));
				}
				result = mapiFolder;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiMessage CreateMessageForDelivery(CreateMessageFlags flags, string internetMessageId, DateTime? clientSubmitTime)
		{
			base.LockStore();
			bool flag = false;
			MapiMessage mapiMessage = null;
			MapiMessage result;
			try
			{
				try
				{
					base.CheckDisposed();
					MapiMessage mapiMessage2 = null;
					try
					{
						mapiMessage2 = this.CreateMessage(flags);
						if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
						{
							ComponentTrace<MapiNetTags>.Trace<string>(62657, 15, (long)this.GetHashCode(), "MapiFolder.CreateMessageForDelivery results: message={0}", TraceUtils.MakeHash(mapiMessage2));
						}
						if (!string.IsNullOrEmpty(internetMessageId))
						{
							mapiMessage2.DuplicateDeliveryCheck(internetMessageId, clientSubmitTime);
						}
						mapiMessage = mapiMessage2;
						mapiMessage2 = null;
						result = mapiMessage;
					}
					finally
					{
						if (mapiMessage2 != null)
						{
							mapiMessage2.Dispose();
						}
					}
				}
				finally
				{
					base.UnlockStore();
					flag = true;
				}
			}
			finally
			{
				if (!flag && mapiMessage != null)
				{
					mapiMessage.Dispose();
					mapiMessage = null;
				}
			}
			return result;
		}

		public MapiMessage CreateMessage(CreateMessageFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			bool flag = false;
			MapiMessage mapiMessage = null;
			MapiMessage result;
			try
			{
				try
				{
					IExMapiMessage exMapiMessage = null;
					try
					{
						int num = this.iMAPIFolder.CreateMessage((int)flags, out exMapiMessage);
						if (num != 0)
						{
							base.ThrowIfError("Unable to create message.", num);
						}
						mapiMessage = new MapiMessage(exMapiMessage, null, base.MapiStore);
						exMapiMessage = null;
					}
					finally
					{
						exMapiMessage.DisposeIfValid();
					}
					result = mapiMessage;
				}
				finally
				{
					base.UnlockStore();
					flag = true;
				}
			}
			finally
			{
				if (!flag && mapiMessage != null)
				{
					mapiMessage.Dispose();
					mapiMessage = null;
				}
			}
			return result;
		}

		public MapiMessage CreateMessage()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiMessage result;
			try
			{
				MapiMessage mapiMessage = this.CreateMessage(CreateMessageFlags.None);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(9298, 15, (long)this.GetHashCode(), "MapiFolder.CreateMessage results: message={0}", TraceUtils.MakeHash(mapiMessage));
				}
				result = mapiMessage;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiMessage CreateAssociatedMessage()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiMessage result;
			try
			{
				MapiMessage mapiMessage = this.CreateMessage(CreateMessageFlags.Associated);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13394, 15, (long)this.GetHashCode(), "MapiFolder.CreateAssociatedMessage results: message={0}", TraceUtils.MakeHash(mapiMessage));
				}
				result = mapiMessage;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiMessage CreateMessageFromFile(string filePath)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiMessage result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11346, 15, (long)this.GetHashCode(), "MapiFolder.CreateMessageFromFile params: filePath={0}", TraceUtils.MakeString(filePath));
				}
				MapiMessage mapiMessage = null;
				IExRpcManageInterface exRpcManageInterface = null;
				IExMapiMessage exMapiMessage = null;
				IStorage storage = null;
				try
				{
					int num = this.iMAPIFolder.CreateMessage(0, out exMapiMessage);
					if (num != 0)
					{
						base.ThrowIfError("Unable to create message.", num);
					}
					num = NativeMethods.StgOpenStorage(filePath, IntPtr.Zero, NativeMethods.StorageMode.ShareDenyWrite, IntPtr.Zero, 0, out storage);
					if (num != 0)
					{
						throw MapiExceptionHelper.FileOpenFailureException("Couldn't open file: " + filePath, num);
					}
					exRpcManageInterface = ExRpcModule.GetExRpcManage();
					num = exRpcManageInterface.FromIStg(storage, ((SafeExMapiMessageHandle)exMapiMessage).DangerousGetHandle());
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to import message from file.", num, exRpcManageInterface, null);
					}
					mapiMessage = new MapiMessage(exMapiMessage, null, base.MapiStore);
					exMapiMessage = null;
				}
				finally
				{
					MapiUnk.ReleaseObject(storage);
					storage = null;
					exMapiMessage.DisposeIfValid();
					exRpcManageInterface.DisposeIfValid();
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15442, 15, (long)this.GetHashCode(), "MapiFolder.CreateMessageFromFile results: mapiMessage={0}", TraceUtils.MakeHash(mapiMessage));
				}
				result = mapiMessage;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiMessage CreateMessageFromStream(Stream ioStream)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (ioStream == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("ioStream");
			}
			base.LockStore();
			MapiMessage result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(9138, 15, (long)this.GetHashCode(), "MapiFolder.CreateMessageFromStream params: ioStream={0}", TraceUtils.MakeHash(ioStream));
				}
				MapiMessage mapiMessage = null;
				IExRpcManageInterface exRpcManageInterface = null;
				IExMapiMessage exMapiMessage = null;
				try
				{
					int num = this.iMAPIFolder.CreateMessage(0, out exMapiMessage);
					if (num != 0)
					{
						base.ThrowIfError("Unable to create message.", num);
					}
					exRpcManageInterface = ExRpcModule.GetExRpcManage();
					num = exRpcManageInterface.FromIStream(new MapiIStream(ioStream), ((SafeExMapiMessageHandle)exMapiMessage).DangerousGetHandle());
					if (num != 0)
					{
						MapiExceptionHelper.ThrowIfError("Unable to import message from file.", num, exRpcManageInterface, null);
					}
					mapiMessage = new MapiMessage(exMapiMessage, null, base.MapiStore);
					exMapiMessage = null;
				}
				finally
				{
					exMapiMessage.DisposeIfValid();
					exRpcManageInterface.DisposeIfValid();
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13234, 15, (long)this.GetHashCode(), "MapiFolder.CreateMessageFromStream results: mapiMessage={0}", TraceUtils.MakeHash(mapiMessage));
				}
				result = mapiMessage;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void DeleteMessages(DeleteMessagesFlags flags, params byte[][] entryIds)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryIds == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryIds");
			}
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(11186, 15, (long)this.GetHashCode(), "MapiFolder.DeleteMessages params: flags={0}, entryIds={1}", flags.ToString(), TraceUtils.DumpArray(entryIds));
				}
				SBinary[] array = new SBinary[entryIds.GetLength(0)];
				for (int i = 0; i < entryIds.GetLength(0); i++)
				{
					array[i] = new SBinary(entryIds[i]);
				}
				int hr = this.iMAPIFolder.DeleteMessages(array, (int)flags);
				base.ThrowIfErrorOrWarning("Unable to delete message(s).", hr);
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void CopyMessages(CopyMessagesFlags flags, MapiFolder destFolder, params byte[][] entryIds)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryIds == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryIds");
			}
			if (destFolder == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("destFolder");
			}
			base.LockStore();
			try
			{
				destFolder.CheckDisposed();
				destFolder.LockStore();
				try
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string, string, string>(15282, 15, (long)this.GetHashCode(), "MapiFolder.CopyMessages params: flags={0}, destFolder={1}, entryIds={2}", flags.ToString(), TraceUtils.MakeHash(destFolder), TraceUtils.DumpArray(entryIds));
					}
					SBinary[] array = new SBinary[entryIds.GetLength(0)];
					for (int i = 0; i < entryIds.GetLength(0); i++)
					{
						array[i] = new SBinary(entryIds[i]);
					}
					int hr;
					if (destFolder.IsExternal)
					{
						hr = this.iMAPIFolder.CopyMessages_External(array, destFolder.ExternalIMAPIFolder, (int)flags);
					}
					else
					{
						hr = this.iMAPIFolder.CopyMessages(array, destFolder.IMAPIFolder, (int)flags);
					}
					base.ThrowIfErrorOrWarning("Unable to copy message(s).", hr);
				}
				finally
				{
					destFolder.UnlockStore();
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void CopyMessages(CopyMessagesFlags flags, MapiFolder destFolder, PropValue[] pva, params byte[][] entryIds)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryIds == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryIds");
			}
			if (destFolder == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("destFolder");
			}
			if (pva == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("pva");
			}
			if (pva.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("pva", "pva must contain at least 1 element");
			}
			base.LockStore();
			try
			{
				destFolder.CheckDisposed();
				destFolder.LockStore();
				try
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string, string, string>(10162, 15, (long)this.GetHashCode(), "MapiFolder.CopyMessages params: flags={0}, destFolder={1}, entryIds={2}", flags.ToString(), TraceUtils.MakeHash(destFolder), TraceUtils.DumpArray(entryIds));
					}
					SBinary[] array = new SBinary[entryIds.GetLength(0)];
					for (int i = 0; i < entryIds.GetLength(0); i++)
					{
						array[i] = new SBinary(entryIds[i]);
					}
					int hr;
					if (destFolder.IsExternal)
					{
						hr = this.iMAPIFolder.CopyMessagesEx_External(array, destFolder.ExternalIMAPIFolder, (int)flags, pva);
					}
					else
					{
						hr = this.iMAPIFolder.CopyMessagesEx(array, destFolder.IMAPIFolder, (int)flags, pva);
					}
					base.ThrowIfErrorOrWarning("Unable to copy message(s).", hr);
				}
				finally
				{
					destFolder.UnlockStore();
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void CopyMessages(CopyMessagesFlags flags, MapiFolder destFolder, PropValue[] pva, byte[][] entryIds, out byte[][] newEntryIds, out byte[][] newChangeNumbers)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			newEntryIds = null;
			newChangeNumbers = null;
			if (entryIds == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryIds");
			}
			if (destFolder == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("destFolder");
			}
			base.LockStore();
			try
			{
				destFolder.CheckDisposed();
				destFolder.LockStore();
				try
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string, string, string>(49509, 15, (long)this.GetHashCode(), "MapiFolder.CopyMessages params: flags={0}, destFolder={1}, entryIds={2}", flags.ToString(), TraceUtils.MakeHash(destFolder), TraceUtils.DumpArray(entryIds));
					}
					SBinary[] array = new SBinary[entryIds.GetLength(0)];
					for (int i = 0; i < entryIds.GetLength(0); i++)
					{
						array[i] = new SBinary(entryIds[i]);
					}
					int hr;
					if (destFolder.IsExternal)
					{
						hr = this.iMAPIFolder.CopyMessagesEID_External(array, destFolder.ExternalIMAPIFolder, (int)flags, pva, out newEntryIds, out newChangeNumbers);
					}
					else
					{
						hr = this.iMAPIFolder.CopyMessagesEID(array, destFolder.IMAPIFolder, (int)flags, pva, out newEntryIds, out newChangeNumbers);
					}
					base.ThrowIfErrorOrWarning("Unable to copy message(s).", hr);
				}
				finally
				{
					destFolder.UnlockStore();
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void CopyFolder(CopyFolderFlags flags, MapiFolder destFolder, byte[] entryId, string newFolderName)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (destFolder == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("destFolder");
			}
			base.LockStore();
			try
			{
				destFolder.CheckDisposed();
				destFolder.LockStore();
				try
				{
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string, string, string>(14258, 15, (long)this.GetHashCode(), "MapiFolder.CopyFolder params: flags={0}, destFolder={1}, entryId={2}", flags.ToString(), TraceUtils.MakeHash(destFolder), TraceUtils.DumpEntryId(entryId));
					}
					flags |= CopyFolderFlags.UnicodeStrings;
					int hr;
					if (destFolder.IsExternal)
					{
						hr = this.iMAPIFolder.CopyFolder_External(entryId.Length, entryId, destFolder.ExternalIMAPIFolder, newFolderName, (int)flags);
					}
					else
					{
						hr = this.iMAPIFolder.CopyFolder(entryId.Length, entryId, destFolder.IMAPIFolder, newFolderName, (int)flags);
					}
					base.ThrowIfErrorOrWarning("Unable to copy folder.", hr);
				}
				finally
				{
					destFolder.UnlockStore();
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void DeleteFolder(byte[] entryId)
		{
			this.DeleteFolder(entryId, DeleteFolderFlags.DeleteMessages | DeleteFolderFlags.DelSubFolders);
		}

		public void DeleteFolder(byte[] entryId, DeleteFolderFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(12210, 15, (long)this.GetHashCode(), "MapiFolder.DeleteFolder params: entryId={0}, flags={1}", TraceUtils.DumpEntryId(entryId), flags.ToString());
				}
				int hr = this.iMAPIFolder.DeleteFolder(entryId, (int)flags);
				base.ThrowIfErrorOrWarning("Unable to delete folder.", hr);
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public MessageStatus GetMessageStatus(byte[] entryId)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryId == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryId");
			}
			base.LockStore();
			MessageStatus result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(16306, 15, (long)this.GetHashCode(), "MapiFolder.GetMessageStatus params: entryId={0}", TraceUtils.DumpEntryId(entryId));
				}
				MessageStatus messageStatus2;
				int messageStatus = this.iMAPIFolder.GetMessageStatus(entryId, 0, out messageStatus2);
				if (messageStatus != 0)
				{
					base.ThrowIfError("Unable to get message status.", messageStatus);
				}
				result = messageStatus2;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void SetMessageStatus(byte[] entryId, MessageStatus messageStatusSet, MessageStatus messageStatusClear)
		{
			MessageStatus messageStatus;
			this.SetMessageStatus(entryId, messageStatusSet, messageStatusClear, out messageStatus);
		}

		public void SetMessageStatus(byte[] entryId, MessageStatus messageStatusSet, MessageStatus messageStatusClear, out MessageStatus oldStatus)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string, MessageStatus, MessageStatus>(8626, 15, (long)this.GetHashCode(), "MapiFolder.SetMessageStatus params: entryId={0} set={1} clear={2}", TraceUtils.DumpEntryId(entryId), messageStatusSet, messageStatusClear);
				}
				MessageStatus ulNewStatusMask = messageStatusSet | messageStatusClear;
				int num = this.iMAPIFolder.SetMessageStatus(entryId, messageStatusSet, ulNewStatusMask, out oldStatus);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set message status.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void EmptyFolder(EmptyFolderFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(12722, 15, (long)this.GetHashCode(), "MapiFolder.EmptyFolder params: flags={0}", flags.ToString());
				}
				int hr = this.iMAPIFolder.EmptyFolder((int)flags);
				base.ThrowIfErrorOrWarning("Unable to empty folder.", hr);
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void SetReadFlags(SetReadFlags flags, params byte[][] entryIds)
		{
			this.SetReadFlags(flags, entryIds, false);
		}

		public void SetReadFlags(SetReadFlags flags, byte[][] entryIds, bool throwIfWarning)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (entryIds == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("entryIds");
			}
			if (entryIds.GetLength(0) < 1)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("entryIds", "entryIds must contain at least 1 value");
			}
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(10674, 15, (long)this.GetHashCode(), "MapiFolder.SetReadFlags params: flags={0}, entryIds={1}", flags.ToString(), TraceUtils.DumpArray(entryIds));
				}
				this.SetReadFlagsEx(flags, entryIds, throwIfWarning);
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void SetReadFlagsOnAllMessages(SetReadFlags flags)
		{
			this.SetReadFlagsOnAllMessages(flags, false);
		}

		public void SetReadFlagsOnAllMessages(SetReadFlags flags, bool throwIfWarning)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14770, 15, (long)this.GetHashCode(), "MapiFolder.SetReadFlagsOnAllMessages params: flags={0}", flags.ToString());
				}
				this.SetReadFlagsEx(flags, null, throwIfWarning);
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public MapiFolder OpenSubFolderByName(string displayName)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (displayName == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("displayName");
			}
			if (displayName == "")
			{
				throw MapiExceptionHelper.ArgumentException("displayName", "param should be not empty");
			}
			base.LockStore();
			MapiFolder result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(9650, 15, (long)this.GetHashCode(), "MapiFolder.OpenSubFolderByName params: displayName={0}", TraceUtils.MakeString(displayName));
				}
				using (MapiTable hierarchyTable = base.GetHierarchyTable())
				{
					byte[] array = (byte[])hierarchyTable.QueryOneValue(PropTag.EntryId, Restriction.Content(PropTag.DisplayName, displayName, ContentFlags.IgnoreCase));
					MapiFolder mapiFolder = null;
					if (array != null)
					{
						IExInterface iExInterface = null;
						try
						{
							int num;
							iExInterface = base.InternalOpenEntry(array, OpenEntryFlags.BestAccess | OpenEntryFlags.DeferredErrors, out num);
							if (num != 3)
							{
								MapiExceptionHelper.ArgumentException("objType", "Opening folder didn't return folder type");
							}
							mapiFolder = new MapiFolder(iExInterface.ToInterface<IExMapiFolder>(), null, base.MapiStore);
							iExInterface = null;
						}
						finally
						{
							iExInterface.DisposeIfValid();
						}
					}
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(13746, 15, (long)this.GetHashCode(), "MapiFolder.OpenSubFolderByName results: folder={0}", TraceUtils.MakeHash(mapiFolder));
					}
					result = mapiFolder;
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public RulesMapiModifyTable GetRulesModifyTable()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			return new RulesMapiModifyTable(this);
		}

		internal RulesMapiTable GetRulesTable()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			RulesMapiTable result;
			try
			{
				RulesMapiTable rulesMapiTable = null;
				IExMapiTable exMapiTable = null;
				try
				{
					int contentsTable = this.iMAPIContainer.GetContentsTable(104, out exMapiTable);
					if (contentsTable != 0)
					{
						base.ThrowIfError("Unable to get contents table.", contentsTable);
					}
					rulesMapiTable = new RulesMapiTable(exMapiTable, this, base.MapiStore);
					exMapiTable = null;
				}
				finally
				{
					exMapiTable.DisposeIfValid();
				}
				result = rulesMapiTable;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public Rule[] GetRulesForTransport(out RulesRetrievalInfo retrievalInfo, params PropTag[] extraProps)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			retrievalInfo = RulesRetrievalInfo.None;
			base.LockStore();
			Rule[] result2;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace(55415, 15, (long)this.GetHashCode(), "MapiFolder.GetRulesForTransport");
				}
				bool flag = extraProps.Length == 0 && base.MapiStore.IsVersionGreaterOrEqualThan(MapiFolder.TransportRulesSnapshotSupportVersion) && base.MapiStore.ClassicRulesInterfaceAvailable;
				Guid empty = Guid.Empty;
				Guid empty2 = Guid.Empty;
				if (flag)
				{
					byte[] array;
					this.GetRulesCachedProperties(out array, out empty);
					byte[] bytes = base.GetProp(PropTag.MappingSignature).GetBytes();
					empty2 = new Guid(bytes);
					if (array != null)
					{
						try
						{
							Guid a;
							Guid a2;
							Rule[] result = this.DeserializeRules(array, out a, out a2);
							if (a2 == empty && a == empty2)
							{
								retrievalInfo = RulesRetrievalInfo.CacheHit;
								return result;
							}
						}
						catch (EndOfStreamException)
						{
							ComponentTrace<MapiNetTags>.Trace(36983, 15, (long)this.GetHashCode(), "DeserializeRules failed with EndOfStreamException");
							retrievalInfo = RulesRetrievalInfo.CacheCorruption;
						}
						catch (MapiPermanentException ex)
						{
							ComponentTrace<MapiNetTags>.Trace(53367, 15, (long)this.GetHashCode(), "DeserializeRules failed with: " + ex.DiagCtx);
							retrievalInfo = RulesRetrievalInfo.CacheCorruption;
						}
						catch (MapiRetryableException ex2)
						{
							ComponentTrace<MapiNetTags>.Trace(41079, 15, (long)this.GetHashCode(), "DeserializeRules failed with: " + ex2.DiagCtx);
							retrievalInfo = RulesRetrievalInfo.CacheCorruption;
						}
					}
				}
				Rule[] rules = this.GetRules(new PropTag[0]);
				if (retrievalInfo == RulesRetrievalInfo.None)
				{
					retrievalInfo = (flag ? RulesRetrievalInfo.CacheMiss : RulesRetrievalInfo.CacheNotSupport);
				}
				if (flag)
				{
					this.SetRulesCachedProperties(rules, empty2, empty);
				}
				result2 = rules;
			}
			finally
			{
				base.UnlockStore();
			}
			return result2;
		}

		public Rule[] GetRules(params PropTag[] extraProps)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			Rule[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace(11698, 15, (long)this.GetHashCode(), "MapiFolder.GetRules");
				}
				List<Rule> list = new List<Rule>();
				if (base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					using (MapiModifyTable mapiModifyTable = (MapiModifyTable)base.OpenProperty(PropTagHelper.PropTagFromIdAndType(16353, PropType.Object), InterfaceIds.IExchangeModifyTable, 0, OpenPropertyFlags.DeferredErrors))
					{
						using (MapiTable table = mapiModifyTable.GetTable(GetTableFlags.DeferredErrors))
						{
							table.SetColumns(Rule.GetUnmarshalColumns());
							table.SeekRow(BookMark.Beginning, 0);
							for (;;)
							{
								PropValue[][] array = table.QueryRows(1000);
								if (array.Length == 0)
								{
									break;
								}
								foreach (PropValue[] cols in array)
								{
									Rule item = new Rule(cols, null, this, true);
									list.Add(item);
								}
							}
						}
					}
				}
				using (MapiTable contentsTable = base.GetContentsTable(ContentsTableFlags.NoNotifications | ContentsTableFlags.Associated | ContentsTableFlags.DeferredErrors))
				{
					contentsTable.Restrict(base.MapiStore.ClassicRulesInterfaceAvailable ? Rule.ExtendedRule : Rule.NewRuleMessages);
					ICollection<PropTag> collection = Rule.GetUnmarshalExColumns();
					if (extraProps != null)
					{
						List<PropTag> list2 = new List<PropTag>(collection);
						list2.AddRange(extraProps);
						collection = list2;
					}
					contentsTable.SetColumns(collection);
					contentsTable.SeekRow(BookMark.Beginning, 0);
					for (;;)
					{
						PropValue[][] array3 = contentsTable.QueryRows(1000);
						if (array3.Length == 0)
						{
							break;
						}
						foreach (PropValue[] cols2 in array3)
						{
							try
							{
								Rule item2 = new Rule(cols2, extraProps, this, false);
								list.Add(item2);
							}
							catch (MapiExceptionNotFound)
							{
							}
						}
					}
				}
				Rule[] array5 = list.ToArray();
				Array.Sort<Rule>(array5);
				result = array5;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void SetRules(Rule[] rules)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (rules == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("rules");
			}
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15794, 15, (long)this.GetHashCode(), "MapiFolder.SetRules params: {0}", TraceUtils.DumpArray(rules));
				}
				using (MapiTable contentsTable = base.GetContentsTable(ContentsTableFlags.NoNotifications | ContentsTableFlags.Associated | ContentsTableFlags.DeferredErrors))
				{
					contentsTable.Restrict(Rule.AllRuleMessages);
					contentsTable.SetColumns(MapiFolder.EntryIdColumns);
					for (;;)
					{
						contentsTable.SeekRow(BookMark.Beginning, 0);
						PropValue[][] array = contentsTable.QueryRows(1000);
						if (array.Length == 0)
						{
							break;
						}
						byte[][] array2 = new byte[array.Length][];
						int num = 0;
						foreach (PropValue[] array4 in array)
						{
							array2[num] = array4[0].GetBytes();
							num++;
						}
						this.DeleteMessages(DeleteMessagesFlags.None, array2);
					}
				}
				this.AddRules(rules);
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void AddRules(params Rule[] rules)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (rules == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("rules");
			}
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8882, 15, (long)this.GetHashCode(), "MapiFolder.AddRules params: {0}", TraceUtils.DumpArray(rules));
				}
				int num = 0;
				if (base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					using (MapiModifyTable mapiModifyTable = (MapiModifyTable)base.OpenProperty(PropTagHelper.PropTagFromIdAndType(16353, PropType.Object), InterfaceIds.IExchangeModifyTable, 0, OpenPropertyFlags.DeferredErrors))
					{
						List<RowEntry> list = new List<RowEntry>();
						foreach (Rule rule in rules)
						{
							if (!rule.IsExtended)
							{
								list.Add(RowEntry.Add(rule.ToProperties(false, true)));
								if (list.Count > 1000)
								{
									mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
									list = new List<RowEntry>();
								}
							}
						}
						if (list.Count > 0)
						{
							mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
						}
						goto IL_F4;
					}
				}
				num = this.UpdateTotalRulesSize();
				IL_F4:
				foreach (Rule rule2 in rules)
				{
					if (!base.MapiStore.ClassicRulesInterfaceAvailable || rule2.IsExtended)
					{
						this.WriteExRule(false, rule2);
					}
				}
				if (!base.MapiStore.IsMoveDestination && !base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					int num2 = this.UpdateTotalRulesSize();
					if (num2 > num && num2 > base.MapiStore.GetProp(PropTag.RulesSize).GetInt())
					{
						MapiExceptionHelper.ThrowIfError("Rules collectively occupy too much space", -2147024882);
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void DeleteRules(params Rule[] rules)
		{
			base.CheckDisposed();
			if (rules == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("rules");
			}
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(12978, 15, (long)this.GetHashCode(), "MapiFolder.DeleteRules params: {0}", TraceUtils.DumpArray(rules));
				}
				if (base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					using (MapiModifyTable mapiModifyTable = (MapiModifyTable)base.OpenProperty(PropTagHelper.PropTagFromIdAndType(16353, PropType.Object), InterfaceIds.IExchangeModifyTable, 0, OpenPropertyFlags.DeferredErrors))
					{
						List<RowEntry> list = new List<RowEntry>();
						foreach (Rule rule in rules)
						{
							if (rule.IDx == null)
							{
								list.Add(RowEntry.Remove(new PropValue[]
								{
									new PropValue(Rule.PR_RULE_ID, rule.ID)
								}));
								if (list.Count > 1000)
								{
									mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
									list = new List<RowEntry>();
								}
							}
						}
						if (list.Count > 0)
						{
							mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
						}
					}
				}
				List<byte[]> list2 = new List<byte[]>();
				foreach (Rule rule2 in rules)
				{
					if (rule2.IDx != null)
					{
						list2.Add(rule2.IDx);
						if (list2.Count > 1000)
						{
							this.DeleteMessages(DeleteMessagesFlags.None, list2.ToArray());
							list2 = new List<byte[]>();
						}
					}
				}
				if (list2.Count > 0)
				{
					this.DeleteMessages(DeleteMessagesFlags.None, list2.ToArray());
				}
				if (!base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					this.UpdateTotalRulesSize();
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void ModifyRules(params Rule[] rules)
		{
			base.CheckDisposed();
			if (rules == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("rules");
			}
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(10930, 15, (long)this.GetHashCode(), "MapiFolder.ModifyRules params: {0}", TraceUtils.DumpArray(rules));
				}
				int num = 0;
				if (base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					using (MapiModifyTable mapiModifyTable = (MapiModifyTable)base.OpenProperty(PropTagHelper.PropTagFromIdAndType(16353, PropType.Object), InterfaceIds.IExchangeModifyTable, 0, OpenPropertyFlags.DeferredErrors))
					{
						List<RowEntry> list = new List<RowEntry>();
						foreach (Rule rule in rules)
						{
							if (rule.IDx == null)
							{
								list.Add(RowEntry.Modify(rule.ToProperties(true, true)));
								if (list.Count > 1000)
								{
									mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
									list = new List<RowEntry>();
								}
							}
						}
						if (list.Count > 0)
						{
							mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
						}
						goto IL_EE;
					}
				}
				num = this.UpdateTotalRulesSize();
				IL_EE:
				foreach (Rule rule2 in rules)
				{
					if (rule2.IDx != null)
					{
						this.WriteExRule(true, rule2);
					}
				}
				if (!base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					int num2 = this.UpdateTotalRulesSize();
					if (num2 > num && num2 > base.MapiStore.GetProp(PropTag.RulesSize).GetInt())
					{
						MapiExceptionHelper.ThrowIfError("Rules collectively occupy too much space", -2147024882);
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public void SaveRuleBatch(IEnumerable<Rule> ruleBatch)
		{
			base.CheckDisposed();
			if (ruleBatch == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("rules");
			}
			Rule[] array = ruleBatch.ToArray<Rule>();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(65295, 15, (long)this.GetHashCode(), "MapiFolder.SaveRuleBatch params: {0}", TraceUtils.DumpArray(array));
				}
				int num = 0;
				bool flag = false;
				bool flag2 = false;
				if (base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					using (MapiModifyTable mapiModifyTable = (MapiModifyTable)base.OpenProperty(PropTagHelper.PropTagFromIdAndType(16353, PropType.Object), InterfaceIds.IExchangeModifyTable, 0, OpenPropertyFlags.DeferredErrors))
					{
						List<RowEntry> list = new List<RowEntry>();
						foreach (Rule rule3 in array)
						{
							RowEntry item = RowEntry.Empty();
							switch (rule3.Operation)
							{
							case RuleOperation.Create:
								flag = true;
								if (!rule3.IsExtended)
								{
									item = RowEntry.Add(rule3.ToProperties(false, true));
								}
								break;
							case RuleOperation.Update:
								flag2 = true;
								if (rule3.IDx == null)
								{
									item = RowEntry.Modify(rule3.ToProperties(true, true));
								}
								break;
							case RuleOperation.Delete:
								if (rule3.IDx == null)
								{
									item = RowEntry.Remove(new PropValue[]
									{
										new PropValue(Rule.PR_RULE_ID, rule3.ID)
									});
								}
								break;
							}
							if (!item.IsEmpty)
							{
								list.Add(item);
								if (list.Count > 1000)
								{
									mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
									list = new List<RowEntry>();
								}
							}
						}
						if (list.Count > 0)
						{
							mapiModifyTable.ModifyTable(ModifyTableFlags.None, list);
						}
						goto IL_1CB;
					}
				}
				if (!flag && !flag2)
				{
					if (!array.Any((Rule rule) => RuleOperation.Create == rule.Operation || RuleOperation.Update == rule.Operation))
					{
						goto IL_1CB;
					}
				}
				num = this.UpdateTotalRulesSize();
				IL_1CB:
				List<byte[]> list2 = new List<byte[]>();
				foreach (Rule rule2 in array)
				{
					switch (rule2.Operation)
					{
					case RuleOperation.Create:
						flag = true;
						if (!base.MapiStore.ClassicRulesInterfaceAvailable || rule2.IsExtended)
						{
							this.WriteExRule(false, rule2);
						}
						break;
					case RuleOperation.Update:
						flag2 = true;
						if (rule2.IDx != null)
						{
							this.WriteExRule(true, rule2);
						}
						break;
					case RuleOperation.Delete:
						if (rule2.IDx != null)
						{
							list2.Add(rule2.IDx);
							if (1000 < list2.Count)
							{
								this.DeleteMessages(DeleteMessagesFlags.None, list2.ToArray());
								list2 = new List<byte[]>();
							}
						}
						break;
					}
				}
				if (0 < list2.Count)
				{
					this.DeleteMessages(DeleteMessagesFlags.None, list2.ToArray());
				}
				if (!base.MapiStore.ClassicRulesInterfaceAvailable)
				{
					int num2 = this.UpdateTotalRulesSize();
					if ((flag || flag2) && num2 > num && num2 > base.MapiStore.GetProp(PropTag.RulesSize).GetInt())
					{
						MapiExceptionHelper.ThrowIfError("Rules collectively occupy too much space", -2147024882);
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public Restriction DeserializeRestriction(byte[] blob)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (blob == null || blob.Length == 0)
			{
				return null;
			}
			base.LockStore();
			Restriction result;
			try
			{
				Restriction restriction;
				int num = this.iMAPIFolder.HrDeserializeSRestrictionEx(blob, out restriction);
				if (num != 0)
				{
					base.ThrowIfError("Unable to deserialize restriction.", num);
				}
				result = restriction;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public RuleAction[] DeserializeActions(byte[] blob)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (blob == null || blob.Length == 0)
			{
				return Array<RuleAction>.Empty;
			}
			base.LockStore();
			RuleAction[] result;
			try
			{
				RuleAction[] array;
				int num = this.iMAPIFolder.HrDeserializeActionsEx(blob, out array);
				if (num != 0)
				{
					base.ThrowIfError("Unable to deserialize extended rule actions.", num);
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiSynchronizer CreateContentsSynchronizer()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiSynchronizer result;
			try
			{
				MapiSynchronizer mapiSynchronizer = (MapiSynchronizer)base.OpenProperty(PropTag.ContentsSynchronizer, InterfaceIds.IExchangeExportChanges, 0, OpenPropertyFlags.None);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(15026, 15, (long)this.GetHashCode(), "MapiFolder.CreateContentsSynchronizer results: synchronizer={0}", TraceUtils.MakeHash(mapiSynchronizer));
				}
				result = mapiSynchronizer;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiSynchronizer CreateHierarchySynchronizer()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiSynchronizer result;
			try
			{
				MapiSynchronizer mapiSynchronizer = (MapiSynchronizer)base.OpenProperty(PropTag.HierarchySynchronizer, InterfaceIds.IExchangeExportChanges, 0, OpenPropertyFlags.None);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(9906, 15, (long)this.GetHashCode(), "MapiFolder.CreateHierarchySynchronizer results: synchronizer={0}", TraceUtils.MakeHash(mapiSynchronizer));
				}
				result = mapiSynchronizer;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiCollector CreateContentsCollector()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiCollector result;
			try
			{
				MapiCollector mapiCollector = (MapiCollector)base.OpenProperty(PropTag.Collector, InterfaceIds.IExchangeImportContentsChanges, 0, OpenPropertyFlags.None);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(14002, 15, (long)this.GetHashCode(), "MapiFolder.CreateContentsCollector results: collector={0}", TraceUtils.MakeHash(mapiCollector));
				}
				result = mapiCollector;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiCollector CreateContentsCollectorPartial()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiCollector result;
			try
			{
				MapiCollector mapiCollector = (MapiCollector)base.OpenProperty(PropTag.Collector, InterfaceIds.IExchangeImportContentsChanges3, 0, OpenPropertyFlags.None);
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11954, 15, (long)this.GetHashCode(), "MapiFolder.CreateContentsCollectorPartial results: collector={0}", TraceUtils.MakeHash(mapiCollector));
				}
				result = mapiCollector;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiCollectorEx CreateContentsCollectorEx(byte[] stateIdsetGiven, byte[] stateCnsetSeen, byte[] stateCnsetSeenFAI, byte[] stateCnsetRead, CollectorConfigFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			if (stateCnsetSeenFAI == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeenFAI");
			}
			if (stateCnsetRead == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetRead");
			}
			base.LockStore();
			MapiCollectorEx result;
			try
			{
				IExInterface exInterface = null;
				MapiCollectorEx mapiCollectorEx = null;
				bool flag = false;
				try
				{
					exInterface = base.InternalOpenProperty(PropTag.Collector, InterfaceIds.IExchangeImportContentsChanges4, 0, OpenPropertyFlags.None);
					mapiCollectorEx = new MapiCollectorEx(new SafeExImportContentsChanges4Handle((SafeExInterfaceHandle)exInterface), base.MapiStore, stateIdsetGiven, stateCnsetSeen, stateCnsetSeenFAI, stateCnsetRead, flags);
					exInterface = null;
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(54241, 15, (long)this.GetHashCode(), "MapiFolder.CreateContentsCollectorEx results: collector={0}", TraceUtils.MakeHash(mapiCollectorEx));
					}
					flag = true;
					result = mapiCollectorEx;
				}
				finally
				{
					if (!flag)
					{
						exInterface.DisposeIfValid();
						if (mapiCollectorEx != null)
						{
							mapiCollectorEx.Dispose();
						}
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiHierarchyCollector CreateHierarchyCollector()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiHierarchyCollector result;
			try
			{
				IExInterface iExInterface = null;
				MapiHierarchyCollector mapiHierarchyCollector = null;
				bool flag = false;
				try
				{
					iExInterface = base.InternalOpenProperty(PropTag.Collector, InterfaceIds.IExchangeImportHierarchyChanges, 0, OpenPropertyFlags.None);
					mapiHierarchyCollector = new MapiHierarchyCollector(iExInterface.ToInterface<IExImportHierarchyChanges>(), base.MapiStore);
					iExInterface = null;
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(16050, 15, (long)this.GetHashCode(), "MapiFolder.CreateHierarchyCollector results: collector={0}", TraceUtils.MakeHash(mapiHierarchyCollector));
					}
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						iExInterface.DisposeIfValid();
						if (mapiHierarchyCollector != null)
						{
							mapiHierarchyCollector.Dispose();
						}
					}
				}
				result = mapiHierarchyCollector;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiHierarchyCollectorEx CreateHierarchyCollectorEx(byte[] stateIdsetGiven, byte[] stateCnsetSeen, CollectorConfigFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			base.LockStore();
			MapiHierarchyCollectorEx result;
			try
			{
				IExInterface exInterface = null;
				MapiHierarchyCollectorEx mapiHierarchyCollectorEx = null;
				bool flag = false;
				try
				{
					exInterface = base.InternalOpenProperty(PropTag.Collector, InterfaceIds.IExchangeImportHierarchyChanges2, 0, OpenPropertyFlags.None);
					mapiHierarchyCollectorEx = new MapiHierarchyCollectorEx(new SafeExImportHierarchyChanges2Handle((SafeExInterfaceHandle)exInterface), base.MapiStore, stateIdsetGiven, stateCnsetSeen, flags);
					exInterface = null;
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(54301, 15, (long)this.GetHashCode(), "MapiFolder.CreateHierarchyCollectorEx results: collector={0}", TraceUtils.MakeHash(mapiHierarchyCollectorEx));
					}
					flag = true;
					result = mapiHierarchyCollectorEx;
				}
				finally
				{
					if (!flag)
					{
						exInterface.DisposeIfValid();
						if (mapiHierarchyCollectorEx != null)
						{
							mapiHierarchyCollectorEx.Dispose();
						}
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiManifest CreateExportManifest()
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiManifest result;
			try
			{
				IExInterface iExInterface = null;
				MapiManifest mapiManifest;
				try
				{
					iExInterface = base.InternalOpenProperty(PropTag.ContentsSynchronizer, InterfaceIds.IExchangeExportManifest, 0, OpenPropertyFlags.None);
					mapiManifest = new MapiManifest(iExInterface.ToInterface<IExExportManifest>(), base.MapiStore);
					iExInterface = null;
				}
				finally
				{
					iExInterface.DisposeIfValid();
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(8370, 15, (long)this.GetHashCode(), "MapiFolder.CreateExportManifest results: manifest={0}", TraceUtils.MakeHash(mapiManifest));
				}
				result = mapiManifest;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiManifestEx CreateExportManifestEx(SyncConfigFlags flags, Restriction restriction, byte[] stateIdsetGiven, byte[] stateCnsetSeen, byte[] stateCnsetSeenFAI, byte[] stateCnsetRead, IMapiManifestExCallback iMapiManifestCallback, ICollection<PropTag> tags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			if (stateCnsetSeenFAI == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeenFAI");
			}
			if (stateCnsetRead == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetRead");
			}
			base.LockStore();
			MapiManifestEx result;
			try
			{
				IExInterface exInterface = null;
				MapiManifestEx mapiManifestEx = null;
				bool flag = false;
				try
				{
					exInterface = base.InternalOpenProperty(PropTag.ContentsSynchronizer, InterfaceIds.IExchangeExportManifestEx, 0, OpenPropertyFlags.None);
					mapiManifestEx = new MapiManifestEx(new SafeExExportManifestExHandle((SafeExInterfaceHandle)exInterface), base.MapiStore, flags, restriction, stateIdsetGiven, stateCnsetSeen, stateCnsetSeenFAI, stateCnsetRead, iMapiManifestCallback, tags);
					exInterface = null;
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(44765, 15, (long)this.GetHashCode(), "MapiFolder.CreateExportManifestEx results: manifest={0}", TraceUtils.MakeHash(mapiManifestEx));
					}
					flag = true;
					result = mapiManifestEx;
				}
				finally
				{
					if (!flag)
					{
						exInterface.DisposeIfValid();
						if (mapiManifestEx != null)
						{
							mapiManifestEx.Dispose();
						}
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiHierarchyManifestEx CreateExportHierarchyManifestEx(SyncConfigFlags flags, byte[] stateIdsetGiven, byte[] stateCnsetSeen, IMapiHierarchyManifestCallback iMapiManifestCallback, ICollection<PropTag> tagsInclude, ICollection<PropTag> tagsExclude)
		{
			return this.CreateExportHierarchyManifestEx(flags, null, stateIdsetGiven, stateCnsetSeen, iMapiManifestCallback, tagsInclude, tagsExclude);
		}

		public MapiHierarchyManifestEx CreateExportHierarchyManifestEx(SyncConfigFlags flags, Restriction restriction, byte[] stateIdsetGiven, byte[] stateCnsetSeen, IMapiHierarchyManifestCallback iMapiManifestCallback, ICollection<PropTag> tagsInclude, ICollection<PropTag> tagsExclude)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			base.LockStore();
			MapiHierarchyManifestEx result;
			try
			{
				IExInterface exInterface = null;
				MapiHierarchyManifestEx mapiHierarchyManifestEx = null;
				bool flag = false;
				try
				{
					exInterface = base.InternalOpenProperty(PropTag.HierarchySynchronizer, InterfaceIds.IExchangeExportHierManifestEx, 0, OpenPropertyFlags.None);
					mapiHierarchyManifestEx = new MapiHierarchyManifestEx(new SafeExExportHierManifestExHandle((SafeExInterfaceHandle)exInterface), base.MapiStore, flags, restriction, stateIdsetGiven, stateCnsetSeen, iMapiManifestCallback, tagsInclude, tagsExclude);
					exInterface = null;
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(44769, 15, (long)this.GetHashCode(), "MapiFolder.CreateExportHierManifestEx results: manifest={0}", TraceUtils.MakeHash(mapiHierarchyManifestEx));
					}
					flag = true;
					result = mapiHierarchyManifestEx;
				}
				finally
				{
					if (!flag)
					{
						exInterface.DisposeIfValid();
						if (mapiHierarchyManifestEx != null)
						{
							mapiHierarchyManifestEx.Dispose();
						}
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiSynchronizerEx CreateSynchronizerEx(byte[] stateIdsetGiven, byte[] stateCnsetSeen, byte[] stateCnsetSeenFAI, byte[] stateCnsetRead, SyncConfigFlags flags, Restriction restriction, ICollection<PropTag> tagsInclude, ICollection<PropTag> tagsExclude, int fastTransferBlockSize)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			if (stateCnsetSeenFAI == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeenFAI");
			}
			if (stateCnsetRead == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetRead");
			}
			base.LockStore();
			MapiSynchronizerEx result;
			try
			{
				IExInterface exInterface = null;
				SafeExExportContentsChangesExHandle safeExExportContentsChangesExHandle = null;
				MapiSynchronizerEx mapiSynchronizerEx = null;
				bool flag = false;
				try
				{
					exInterface = base.InternalOpenProperty(PropTag.ContentsSynchronizer, InterfaceIds.IExchangeExportContentsChangesEx, 0, OpenPropertyFlags.None);
					safeExExportContentsChangesExHandle = new SafeExExportContentsChangesExHandle((SafeExInterfaceHandle)exInterface);
					exInterface = null;
					mapiSynchronizerEx = new MapiSynchronizerEx(safeExExportContentsChangesExHandle, base.MapiStore, stateIdsetGiven, stateCnsetSeen, stateCnsetSeenFAI, stateCnsetRead, flags, restriction, tagsInclude, tagsExclude, fastTransferBlockSize);
					safeExExportContentsChangesExHandle = null;
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(42355, 15, (long)this.GetHashCode(), "MapiFolder.CreateSynchronizerEx results: synchronizer={0}", TraceUtils.MakeHash(mapiSynchronizerEx));
					}
					flag = true;
					result = mapiSynchronizerEx;
				}
				finally
				{
					if (!flag)
					{
						exInterface.DisposeIfValid();
						safeExExportContentsChangesExHandle.DisposeIfValid();
						if (mapiSynchronizerEx != null)
						{
							mapiSynchronizerEx.Dispose();
						}
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiHierarchySynchronizerEx CreateHierarchySynchronizerEx(byte[] stateIdsetGiven, byte[] stateCnsetSeen, SyncConfigFlags flags, Restriction restriction, ICollection<PropTag> tagsInclude, ICollection<PropTag> tagsExclude, int fastTransferBlockSize)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			base.LockStore();
			MapiHierarchySynchronizerEx result;
			try
			{
				IExInterface exInterface = null;
				SafeExExportHierarchyChangesExHandle safeExExportHierarchyChangesExHandle = null;
				MapiHierarchySynchronizerEx mapiHierarchySynchronizerEx = null;
				bool flag = false;
				try
				{
					exInterface = base.InternalOpenProperty(PropTag.HierarchySynchronizer, InterfaceIds.IExchangeExportHierarchyChangesEx, 0, OpenPropertyFlags.None);
					safeExExportHierarchyChangesExHandle = new SafeExExportHierarchyChangesExHandle((SafeExInterfaceHandle)exInterface);
					exInterface = null;
					mapiHierarchySynchronizerEx = new MapiHierarchySynchronizerEx(safeExExportHierarchyChangesExHandle, base.MapiStore, stateIdsetGiven, stateCnsetSeen, flags, restriction, tagsInclude, tagsExclude, fastTransferBlockSize);
					safeExExportHierarchyChangesExHandle = null;
					if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
					{
						ComponentTrace<MapiNetTags>.Trace<string>(42643, 15, (long)this.GetHashCode(), "MapiFolder.CreateHierarchySynchronizerEx results: synchronizer={0}", TraceUtils.MakeHash(mapiHierarchySynchronizerEx));
					}
					flag = true;
					result = mapiHierarchySynchronizerEx;
				}
				finally
				{
					if (!flag)
					{
						exInterface.DisposeIfValid();
						safeExExportHierarchyChangesExHandle.DisposeIfValid();
						if (mapiHierarchySynchronizerEx != null)
						{
							mapiHierarchySynchronizerEx.Dispose();
						}
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public bool IsContentAvailable
		{
			get
			{
				base.CheckDisposed();
				base.BlockExternalObjectCheck();
				base.LockStore();
				bool result;
				try
				{
					bool flag;
					int num = this.iMAPIFolder.IsContentAvailable(out flag);
					if (num != 0)
					{
						base.ThrowIfError("Unable to check if the folder has content.", num);
					}
					result = flag;
				}
				finally
				{
					base.UnlockStore();
				}
				return result;
			}
		}

		public string[] GetReplicaServers()
		{
			ushort num;
			return this.GetReplicaServers(out num);
		}

		public string[] GetReplicaServers(out ushort cheapServerCount)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			string[] result;
			try
			{
				string[] array;
				uint num;
				int replicaServers = this.iMAPIFolder.GetReplicaServers(out array, out num);
				if (replicaServers != 0)
				{
					base.ThrowIfError("Unable to get replica list for the folder.", replicaServers);
				}
				cheapServerCount = (ushort)num;
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void SetMessageFlags(byte[] entryId, MessageFlags messageFlagsSet, MessageFlags messageFlagsClear)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string, MessageFlags, MessageFlags>(10418, 15, (long)this.GetHashCode(), "MapiFolder.SetMessageFlags params: entryId={0} set={1} clear={2}", TraceUtils.DumpEntryId(entryId), messageFlagsSet, messageFlagsClear);
				}
				MessageFlags ulMask = messageFlagsSet | messageFlagsClear;
				int num = this.iMAPIFolder.SetMessageFlags(entryId.Length, entryId, (uint)messageFlagsSet, (uint)ulMask);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set message flags.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		protected override int SetPropsCall(ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems, bool trackChanges)
		{
			return this.iMAPIFolder.SetPropsEx(trackChanges, lpPropArray, out lppProblems);
		}

		public PropProblem[] SetPropsConditional(Restriction condition, params PropValue[] pva)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (pva == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("pva");
			}
			if (pva.Length <= 0)
			{
				throw MapiExceptionHelper.ArgumentOutOfRangeException("pva", "pva must contain at least 1 element");
			}
			base.LockStore();
			PropProblem[] result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string, string>(65381, 15, (long)this.GetHashCode(), "MapiFolder.SetPropsConditional params: condition={0}, pva={1}", TraceUtils.MakeHash(condition), TraceUtils.DumpPropValsArray(pva));
				}
				PropProblem[] array = null;
				int num = this.iMAPIFolder.SetPropsConditional(condition, pva, out array);
				if (num != 0)
				{
					base.ThrowIfError("Unable to set properties on folder conditionally.", num);
				}
				if (array != null && ComponentTrace<MapiNetTags>.CheckEnabled(15))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(48997, 15, (long)this.GetHashCode(), "MapiProp.SetPropsConditional problems: ppa={0}", TraceUtils.DumpPropProblemsArray(array));
				}
				result = array;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		protected override int DeletePropsCall(ICollection<PropTag> tags, out PropProblem[] problemProps, bool trackChanges)
		{
			return this.iMAPIFolder.DeletePropsEx(trackChanges, tags, out problemProps);
		}

		public void ExportMessages(IMapiFxProxy fxProxy, CopyMessagesFlags flags, params byte[][] entryIds)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			if (fxProxy == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("fxProxy");
			}
			if (ComponentTrace<MapiNetTags>.CheckEnabled(15))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(54589, 15, (long)this.GetHashCode(), "MapiFolder.ExportMessages params: fxProxy={0}, flags={1}, entryIds={2}", TraceUtils.MakeHash(fxProxy), flags.ToString(), TraceUtils.DumpArray(entryIds));
			}
			FxProxyWrapper fxProxyWrapper = null;
			using (MapiFolder mapiFolder = (MapiFolder)FxProxyWrapper.CreateFxProxyWrapper(fxProxy, base.MapiStore, out fxProxyWrapper))
			{
				try
				{
					this.CopyMessages(flags, mapiFolder, entryIds);
				}
				catch (MapiPermanentException)
				{
					if (fxProxyWrapper.LastException != null)
					{
						MapiExceptionHelper.ThrowImportFailureException("Data import failed", fxProxyWrapper.LastException);
					}
					throw;
				}
				catch (MapiRetryableException)
				{
					if (fxProxyWrapper.LastException != null)
					{
						MapiExceptionHelper.ThrowImportFailureException("Data import failed", fxProxyWrapper.LastException);
					}
					throw;
				}
			}
		}

		internal byte[] SerializeRestriction(Restriction restriction)
		{
			if (restriction == null)
			{
				return null;
			}
			byte[] result;
			int num = this.iMAPIFolder.HrSerializeSRestrictionEx(restriction, out result);
			if (num != 0)
			{
				base.ThrowIfError("Unable to serialize restriction.", num);
			}
			return result;
		}

		public byte[] SerializeActions(RuleAction[] actions)
		{
			byte[] result;
			int num = this.iMAPIFolder.HrSerializeActionsEx(actions, out result);
			if (num != 0)
			{
				base.ThrowIfError("Unable to serialize extended rule actions.", num);
			}
			return result;
		}

		protected IExMapiFolder IMAPIFolder
		{
			get
			{
				return this.iMAPIFolder;
			}
		}

		protected IMAPIFolder ExternalIMAPIFolder
		{
			get
			{
				return this.externalIMAPIFolder;
			}
		}

		private void SetReadFlagsEx(SetReadFlags flags, byte[][] entryIds, bool throwIfWarning)
		{
			int num;
			if (entryIds != null && entryIds.GetLength(0) > 0)
			{
				SBinary[] array = new SBinary[entryIds.GetLength(0)];
				for (int i = 0; i < entryIds.GetLength(0); i++)
				{
					array[i] = new SBinary(entryIds[i]);
				}
				num = this.iMAPIFolder.SetReadFlags(array, (int)flags);
			}
			else
			{
				num = this.iMAPIFolder.SetReadFlags(null, (int)flags);
			}
			if (num != 0)
			{
				if (throwIfWarning)
				{
					base.ThrowIfErrorOrWarning("Unable to set read flag.", num);
					return;
				}
				base.ThrowIfError("Unable to set read flag.", num);
			}
		}

		private int UpdateTotalRulesSize()
		{
			int num = 0;
			bool flag = false;
			using (MapiTable contentsTable = base.GetContentsTable(ContentsTableFlags.NoNotifications | ContentsTableFlags.Associated | ContentsTableFlags.DeferredErrors))
			{
				contentsTable.Restrict(Rule.MiddleTierRule);
				contentsTable.SetColumns(Rule.UnmarshalExColumns);
				contentsTable.SeekRow(BookMark.Beginning, 0);
				for (;;)
				{
					PropValue[][] array = contentsTable.QueryRows(1000);
					if (array.Length == 0)
					{
						break;
					}
					flag = true;
					foreach (PropValue[] array3 in array)
					{
						if ((array3[6].GetInt() & 1) != 0)
						{
							byte[] bytes = array3[9].GetBytes();
							if (bytes != null)
							{
								num += bytes.Length;
							}
							bytes = array3[8].GetBytes();
							if (bytes != null)
							{
								num += bytes.Length;
							}
							bytes = array3[5].GetBytes();
							if (bytes != null)
							{
								num += bytes.Length;
							}
							string @string = array3[4].GetString();
							if (@string != null)
							{
								num += @string.Length;
							}
							@string = array3[3].GetString();
							if (@string != null)
							{
								num += @string.Length;
							}
							num += 24;
						}
					}
				}
			}
			base.SetProps(new PropValue[]
			{
				new PropValue(PropTag.RulesSize, num),
				new PropValue(PropTag.HasRules, flag)
			});
			return num;
		}

		private void WriteExRule(bool modifyExisting, Rule rule)
		{
			MapiMessage mapiMessage = null;
			try
			{
				if (modifyExisting)
				{
					mapiMessage = (MapiMessage)base.OpenEntry(rule.IDx, OpenEntryFlags.Modify);
				}
				else
				{
					mapiMessage = this.CreateMessage(CreateMessageFlags.Associated);
				}
				mapiMessage.DeleteProps(Rule.RuleMsgPreDeleteProps);
				ICollection<PropValue> props = rule.ToProperties(modifyExisting, false);
				mapiMessage.SetProps(props);
				if (rule.Condition != null)
				{
					byte[] array = this.SerializeRestriction(rule.Condition);
					using (MapiStream mapiStream = mapiMessage.OpenStream(Rule.PR_EX_RULE_CONDITION, OpenPropertyFlags.Create | OpenPropertyFlags.DeferredErrors))
					{
						mapiStream.Write(array, 0, array.Length);
					}
				}
				byte[] array2 = this.SerializeActions(rule.Actions);
				using (MapiStream mapiStream2 = mapiMessage.OpenStream(Rule.PR_EX_RULE_ACTIONS, OpenPropertyFlags.Create | OpenPropertyFlags.DeferredErrors))
				{
					mapiStream2.Write(array2, 0, array2.Length);
				}
				mapiMessage.SaveChanges(SaveChangesFlags.SkipQuotaCheck);
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
		}

		private byte[] SetRulesCachedProperties(Rule[] rules, Guid currentMappingSignatureGuid, Guid currentSnapshotId)
		{
			byte[] array = this.SerializeRules(rules, currentMappingSignatureGuid, currentSnapshotId);
			using (MapiStream mapiStream = base.OpenStream(PropTag.TransportRulesSnapshot, OpenPropertyFlags.Create | OpenPropertyFlags.DeferredErrors))
			{
				bool flag = false;
				try
				{
					mapiStream.LockRegion(0L, 2147483647L, 1);
					flag = true;
					mapiStream.Write(array, 0, array.Length);
				}
				catch (MapiExceptionLockViolation)
				{
					ComponentTrace<MapiNetTags>.Trace(36343, 15, (long)this.GetHashCode(), "Failed to lock the stream");
				}
				finally
				{
					if (flag)
					{
						mapiStream.UnlockRegion(0L, 2147483647L, 1);
					}
				}
			}
			return array;
		}

		private void GetRulesCachedProperties(out byte[] snapshotBlob, out Guid snapshotId)
		{
			snapshotBlob = null;
			snapshotId = Guid.Empty;
			PropValue[] props = base.GetProps(MapiFolder.TransportRulesSnapshotProperties);
			if (props[0].IsError() && props[0].GetErrorValue() != -2147221233)
			{
				return;
			}
			if (!props[0].IsError())
			{
				snapshotId = props[0].GetGuid();
			}
			if (!props[1].IsError())
			{
				snapshotBlob = props[1].GetBytes();
				return;
			}
			if (props[1].GetErrorValue() != -2147024882)
			{
				return;
			}
			BufferPool bufferPool;
			byte[] buffer = BufferPools.GetBuffer(262144, out bufferPool);
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (MapiStream mapiStream = base.OpenStream(PropTag.TransportRulesSnapshot, OpenPropertyFlags.DeferredErrors))
					{
						bool flag = false;
						try
						{
							mapiStream.LockRegion(0L, 2147483647L, 0);
							flag = true;
							int num;
							do
							{
								num = mapiStream.Read(buffer, 0, buffer.Length);
								if (num > 0)
								{
									memoryStream.Write(buffer, 0, num);
								}
							}
							while (num == buffer.Length);
						}
						catch (MapiExceptionLockViolation)
						{
							ComponentTrace<MapiNetTags>.Trace(52727, 15, (long)this.GetHashCode(), "Failed to lock the stream");
							return;
						}
						finally
						{
							if (flag)
							{
								mapiStream.UnlockRegion(0L, 2147483647L, 0);
							}
						}
					}
					snapshotBlob = memoryStream.ToArray();
				}
			}
			finally
			{
				if (bufferPool != null && buffer != null)
				{
					bufferPool.Release(buffer);
				}
			}
		}

		private byte[] SerializeRules(Rule[] rules, Guid mappingSignatureGuid, Guid rulesSnapshotId)
		{
			byte[] result;
			using (BinarySerializer binarySerializer = new BinarySerializer())
			{
				binarySerializer.Write(16121164246661726210UL);
				binarySerializer.Write(mappingSignatureGuid);
				binarySerializer.Write(rulesSnapshotId);
				byte[] array = (byte[])base.GetProp(PropTag.StoreEntryid).Value;
				binarySerializer.Write(rules.Length);
				foreach (Rule rule in rules)
				{
					rule.SerializeRule(binarySerializer, this);
				}
				result = binarySerializer.ToArray();
			}
			return result;
		}

		private Rule[] DeserializeRules(byte[] rulesBlob, out Guid mappingSignatureGuid, out Guid rulesSnapshotId)
		{
			List<Rule> list = new List<Rule>();
			using (BinaryDeserializer binaryDeserializer = new BinaryDeserializer(rulesBlob))
			{
				ulong num = binaryDeserializer.ReadUInt64();
				if (num != 16121164246661726210UL)
				{
					string message = string.Format("Transport rules blob version mismatch. Expected: {0}. Actual: {0}", 16121164246661726210UL, num);
					ComponentTrace<MapiNetTags>.Trace(56823, 15, (long)this.GetHashCode(), message);
					throw MapiExceptionHelper.DataIntegrityException(message);
				}
				mappingSignatureGuid = binaryDeserializer.ReadGuid();
				rulesSnapshotId = binaryDeserializer.ReadGuid();
				int capacity = binaryDeserializer.ReadInt();
				list.Capacity = capacity;
				while (capacity-- != 0)
				{
					Rule item = Rule.DeserializeRule(binaryDeserializer, this);
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		private const ulong TransportRulesSnapshotVersion = 16121164246661726210UL;

		private const int TransportRulesSnapshotReadBufferSize = 262144;

		private IExMapiFolder iMAPIFolder;

		private IMAPIFolder externalIMAPIFolder;

		private static readonly Guid[] IMAPIFolderGuids = new Guid[]
		{
			InterfaceIds.IMAPIFolderGuid
		};

		private static readonly PropTag[] EntryIdColumns = new PropTag[]
		{
			PropTag.EntryId
		};

		private static readonly PropTag[] TransportRulesSnapshotProperties = new PropTag[]
		{
			PropTag.TransportRulesSnapshotId,
			PropTag.TransportRulesSnapshot
		};

		private static readonly int[] TransportRulesSnapshotSupportVersion = new int[]
		{
			14,
			1,
			149,
			0
		};
	}
}
