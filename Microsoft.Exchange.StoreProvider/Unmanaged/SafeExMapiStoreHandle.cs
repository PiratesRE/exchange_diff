using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExMapiStoreHandle : SafeExMapiPropHandle, IExMapiStore, IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiStoreHandle()
		{
		}

		internal SafeExMapiStoreHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiStoreHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiStoreHandle>(this);
		}

		public int AdviseEx(byte[] lpEntryId, AdviseFlags ulEventMask, IntPtr iOnNotifyDelegate, ulong callbackId, out IntPtr piConnection)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_AdviseEx(this.handle, (lpEntryId != null) ? lpEntryId.Length : 0, lpEntryId, ulEventMask, iOnNotifyDelegate, callbackId, out piConnection);
		}

		public int Unadvise(IntPtr iConnection)
		{
			return SafeExMapiStoreHandle.IMsgStore_Unadvise(this.handle, iConnection);
		}

		public int OpenEntry(byte[] lpEntryID, Guid lpInterface, int ulFlags, out int lpulObjType, out IExInterface iObj)
		{
			SafeExInterfaceHandle safeExInterfaceHandle;
			int result = SafeExMapiStoreHandle.IMsgStore_OpenEntry(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, lpInterface, ulFlags, out lpulObjType, out safeExInterfaceHandle);
			iObj = safeExInterfaceHandle;
			return result;
		}

		public int GetPerUser(byte[] ltid, bool fSendOnlyIfChanged, int ibStream, int cbDataLimit, out byte[] data, out bool fLast)
		{
			data = null;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int num = 0;
			int result;
			try
			{
				int num2 = SafeExMapiStoreHandle.IExRpcMsgStore_GetPerUser(this.handle, ltid, fSendOnlyIfChanged, ibStream, cbDataLimit, out safeExMemoryHandle, out num, out fLast);
				if (num2 == 0)
				{
					if (num > 0 && safeExMemoryHandle != null)
					{
						data = new byte[num];
						Marshal.Copy(safeExMemoryHandle.DangerousGetHandle(), data, 0, num);
					}
					else
					{
						data = Array<byte>.Empty;
					}
				}
				result = num2;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int SetPerUser(byte[] ltid, Guid? guidReplica, int lib, byte[] pb, int cb, bool fLast)
		{
			return this.InternalSetPerUser(ltid, guidReplica, lib, pb, cb, fLast);
		}

		private unsafe int InternalSetPerUser(byte[] ltid, Guid? guidReplica, int lib, byte[] pb, int cb, bool fLast)
		{
			Guid valueOrDefault = guidReplica.GetValueOrDefault();
			Guid* guidReplica2 = (guidReplica != null) ? (&valueOrDefault) : null;
			return SafeExMapiStoreHandle.IExRpcMsgStore_SetPerUser(this.handle, ltid, guidReplica2, lib, pb, cb, fLast);
		}

		public int SetReceiveFolder(string lpwszMessageClass, int ulFlags, byte[] lpEntryID)
		{
			return SafeExMapiStoreHandle.IMsgStore_SetReceiveFolder(this.handle, lpwszMessageClass, ulFlags, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID);
		}

		public int GetReceiveFolder(string lpwszMessageClass, int ulFlags, out byte[] lppEntryId, out string lppszExplicitClass)
		{
			lppEntryId = null;
			lppszExplicitClass = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int num = 0;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle2 = null;
			int result;
			try
			{
				int num2 = SafeExMapiStoreHandle.IMsgStore_GetReceiveFolder(this.handle, lpwszMessageClass, ulFlags, out num, out safeExLinkedMemoryHandle, out safeExLinkedMemoryHandle2);
				if (num2 == 0)
				{
					byte[] array = new byte[num];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num);
					lppEntryId = array;
					if (!safeExLinkedMemoryHandle2.IsInvalid)
					{
						lppszExplicitClass = Marshal.PtrToStringUni(safeExLinkedMemoryHandle2.DangerousGetHandle());
					}
				}
				result = num2;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
				if (safeExLinkedMemoryHandle2 != null)
				{
					safeExLinkedMemoryHandle2.Dispose();
				}
			}
			return result;
		}

		public int GetReceiveFolderInfo(out PropValue[][] lpSRowSet)
		{
			lpSRowSet = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_GetReceiveFolderInfo(this.handle, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					lpSRowSet = SRowSet.Unmarshal(safeExLinkedMemoryHandle);
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int StoreLogoff(ref int ulFlags)
		{
			return SafeExMapiStoreHandle.IMsgStore_StoreLogoff(this.handle, ref ulFlags);
		}

		public int AbortSubmit(byte[] lpEntryID, int ulFlags)
		{
			return SafeExMapiStoreHandle.IMsgStore_AbortSubmit(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, ulFlags);
		}

		public int CreateEntryId(long fid, long mid, bool fMessage, bool fLongTerm, out byte[] lppEntryId)
		{
			lppEntryId = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_CreateEntryId(this.handle, fid, mid, fMessage, fLongTerm, out num2, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = new byte[num2];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num2);
					lppEntryId = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetShortTermIdsFromLongTermEntryId(byte[] lpEntryID, out bool pfMessage, out long pFid, out long pMid)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetShortTermIdsFromLongTermEntryId(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, out pfMessage, out pFid, out pMid);
		}

		public int CreateEntryIdFromLegacyDN(string lpszLegacyDN, out byte[] lppEntryId)
		{
			lppEntryId = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_CreateEntryIdFromLegacyDN(this.handle, lpszLegacyDN, out num2, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = new byte[num2];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num2);
					lppEntryId = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetParentEntryId(byte[] lpEntryID, out byte[] lppParentEntryId)
		{
			lppParentEntryId = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_GetParentEntryId(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, out num2, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = new byte[num2];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num2);
					lppParentEntryId = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetAddressTypes(out string[] lppszAddressTypes)
		{
			lppszAddressTypes = null;
			IntPtr zero = IntPtr.Zero;
			int result;
			try
			{
				int num = 0;
				int num2 = SafeExMapiStoreHandle.IExRpcMsgStore_GetAddressTypes(this.handle, out num, out zero);
				if (num2 == 0)
				{
					string[] array = new string[num];
					for (int i = 0; i < num; i++)
					{
						IntPtr ptr = Marshal.ReadIntPtr(zero, i * IntPtr.Size);
						array[i] = Marshal.PtrToStringAnsi(ptr);
					}
					lppszAddressTypes = array;
				}
				result = num2;
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					SafeExMemoryHandle.FreePvFnEx(zero);
				}
			}
			return result;
		}

		public int BackoffNow(out int iNow)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_BackoffNow(this.handle, out iNow);
		}

		public int CompressEntryId(byte[] lpEntryID, out byte[] lppCompressedEntryId)
		{
			lppCompressedEntryId = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_CompressEntryId(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, out num2, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = new byte[num2];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num2);
					lppCompressedEntryId = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int ExpandEntryId(byte[] lpCompressedEntryID, out byte[] lppEntryId)
		{
			lppEntryId = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_ExpandEntryId(this.handle, (lpCompressedEntryID != null) ? lpCompressedEntryID.Length : 0, lpCompressedEntryID, out num2, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = new byte[num2];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num2);
					lppEntryId = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int CreateGlobalIdFromId(long id, out byte[] lppGid)
		{
			lppGid = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_CreateGlobalIdFromId(this.handle, id, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = new byte[22];
					safeExLinkedMemoryHandle.CopyTo(array, 0, array.Length);
					lppGid = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int CreateIdFromGlobalId(byte[] gid, out long id)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_CreateIdFromGlobalId(this.handle, gid, out id);
		}

		public int MapActionsToMDBActions(RuleAction[] actions, out byte[] mdbActions)
		{
			return this.InternalMapActionsToMDBActions(actions, out mdbActions);
		}

		private unsafe int InternalMapActionsToMDBActions(RuleAction[] actions, out byte[] mdbActions)
		{
			mdbActions = null;
			uint bytesToMarshal = (uint)RuleActions.GetBytesToMarshal(actions);
			byte[] array = new byte[bytesToMarshal];
			SafeExMemoryHandle safeExMemoryHandle = null;
			int result;
			try
			{
				int num;
				try
				{
					fixed (byte* ptr = &array[0])
					{
						byte* ptr2 = ptr;
						RuleActions.MarshalToNative(ref ptr2, actions);
						num = SafeExMapiStoreHandle.IExRpcMsgStore_MapActionsToMDBActions(this.handle, (_Actions*)ptr, out bytesToMarshal, out safeExMemoryHandle);
					}
				}
				finally
				{
					byte* ptr = null;
				}
				if (num == 0)
				{
					byte[] array2 = new byte[bytesToMarshal];
					safeExMemoryHandle.CopyTo(array2, 0, (int)bytesToMarshal);
					mdbActions = array2;
				}
				result = num;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetMailboxInstanceGuid(out Guid guidMailboxInstanceGuid)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetMailboxInstanceGuid(this.handle, out guidMailboxInstanceGuid);
		}

		public int GetMdbIdMapping(out ushort replidServer, out Guid guidServer)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetMdbIdMapping(this.handle, out replidServer, out guidServer);
		}

		public int GetSpoolerQueueFid(out long fidSpoolerQ)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetSpoolerQueueFid(this.handle, out fidSpoolerQ);
		}

		public int GetLocalRepIds(uint cid, out MapiLtidNative ltid)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetLocalRepIds(this.handle, cid, out ltid);
		}

		public int CreatePublicEntryId(long fid, long mid, bool fMessage, out byte[] lppEntryId)
		{
			lppEntryId = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiStoreHandle.IExRpcMsgStore_CreatePublicEntryId(this.handle, fid, mid, fMessage, out num2, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = new byte[num2];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num2);
					lppEntryId = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetTransportQueueFolderId(out long fidTransportQueue)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetTransportQueueFolderId(this.handle, out fidTransportQueue);
		}

		public int GetIsRulesInterfaceAvailable(out bool rulesInterfaceAvailable)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetIsRulesInterfaceAvailable(this.handle, out rulesInterfaceAvailable);
		}

		public int SetSpooler()
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_SetSpooler(this.handle);
		}

		public int SpoolerSetMessageLockState(byte[] lpEntryID, int ulLockState)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_SpoolerSetMessageLockState(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, ulLockState);
		}

		public int SpoolerNotifyMessageNewMail(byte[] lpEntryID, string lpszMsgClass, int ulMessageFlags)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_SpoolerNotifyMessageNewMail(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, lpszMsgClass, ulMessageFlags);
		}

		public int GetPerUserGuid(MapiLtidNative ltid, out Guid guid)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetPerUserGuid(this.handle, ltid, out guid);
		}

		public int GetPerUserLtids(Guid guid, out MapiLtidNative[] ltids)
		{
			SafeExMemoryHandle safeExMemoryHandle = null;
			int num = 0;
			ltids = Array<MapiLtidNative>.Empty;
			int result;
			try
			{
				int num2 = SafeExMapiStoreHandle.IExRpcMsgStore_GetPerUserLtids(this.handle, guid, out num, out safeExMemoryHandle);
				if (num != 0)
				{
					ltids = safeExMemoryHandle.ReadMapiLtidNativeArray(num);
				}
				result = num2;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		internal int GetAllPerUserLtids(byte[] ltidStart, out MapiPUDNative[] perUserData, out bool isFinished)
		{
			SafeExMemoryHandle safeExMemoryHandle = null;
			int num = 0;
			perUserData = Array<MapiPUDNative>.Empty;
			int result;
			try
			{
				int num2 = SafeExMapiStoreHandle.IExRpcMsgStore_GetAllPerUserLtids(this.handle, ltidStart, out num, out safeExMemoryHandle, out isFinished);
				if (num != 0)
				{
					perUserData = safeExMemoryHandle.ReadMapiPudNativeArray(num);
				}
				result = num2;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetEffectiveRights(byte[] lpAddressBookEntryId, byte[] lpEntryId, out uint rights)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetEffectiveRights(this.handle, (lpAddressBookEntryId != null) ? lpAddressBookEntryId.Length : 0, lpAddressBookEntryId, (lpEntryId != null) ? lpEntryId.Length : 0, lpEntryId, out rights);
		}

		public int PrereadMessages(byte[][] entryIds)
		{
			return this.InternalPrereadMessages(entryIds);
		}

		private unsafe int InternalPrereadMessages(byte[][] entryIds)
		{
			SBinary[] array = new SBinary[entryIds.GetLength(0)];
			for (int i = 0; i < entryIds.GetLength(0); i++)
			{
				array[i] = new SBinary(entryIds[i]);
			}
			int bytesToMarshal = SBinaryArray.GetBytesToMarshal(array);
			fixed (byte* ptr = new byte[bytesToMarshal])
			{
				SBinaryArray.MarshalToNative(ptr, array);
				return SafeExMapiStoreHandle.IExRpcMsgStore_PrereadMessages(this.handle, (_SBinaryArray*)ptr);
			}
		}

		internal int SetCurrentActivityInfo(Guid activityId, string component, string protocol, string action)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_SetCurrentActivityInfo(this.handle, activityId, component, protocol, action);
		}

		public int GetInTransitStatus(out uint inTransitStatus)
		{
			return SafeExMapiStoreHandle.IExRpcMsgStore_GetInTransitStatus(this.handle, out inTransitStatus);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMsgStore_Advise(IntPtr iMsgStore, int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, AdviseFlags ulEventMask, IMAPIAdviseSink lpAdviseSink, out IntPtr piConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMsgStore_Unadvise(IntPtr iMsgStore, IntPtr iConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMsgStore_OpenEntry(IntPtr iMsgStore, int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int ulFlags, out int lpulObjType, out SafeExInterfaceHandle iObj);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetPerUser(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpLtid, bool fSendOnlyIfChanged, int lib, int cbDataLimit, out SafeExMemoryHandle pb, out int cb, out bool fLast);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcMsgStore_SetPerUser(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpLtid, Guid* guidReplica, int lib, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pb, int cb, bool fLast);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMsgStore_SetReceiveFolder(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszMessageClass, int ulFlags, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMsgStore_GetReceiveFolder(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszMessageClass, int ulFlags, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId, out SafeExLinkedMemoryHandle lppszExplicitClass);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMsgStore_StoreLogoff(IntPtr iMsgStore, ref int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMsgStore_AbortSubmit(IntPtr iMsgStore, int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_CreateEntryId(IntPtr iMsgStore, long fid, long mid, [MarshalAs(UnmanagedType.Bool)] bool fMessage, [MarshalAs(UnmanagedType.Bool)] bool fLongTerm, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetReceiveFolderInfo(IntPtr iMsgStore, out SafeExLinkedMemoryHandle lpSRowSet);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetShortTermIdsFromLongTermEntryId(IntPtr iMsgStore, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.Bool)] out bool pfMessage, out long pFid, out long pMid);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_CreateEntryIdFromLegacyDN(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPStr)] string lpszLegacyDN, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetParentEntryId(IntPtr iMsgStore, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int lpcbParentEntryId, out SafeExLinkedMemoryHandle lppParentEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetAddressTypes(IntPtr iMsgStore, out int cAddressTypes, out IntPtr lppszAddressTypes);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_BackoffNow(IntPtr iMsgStore, out int iNow);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_CompressEntryId(IntPtr iMsgStore, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, out int lpcbCompressedEntryId, out SafeExLinkedMemoryHandle lppCompressedEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_ExpandEntryId(IntPtr iMsgStore, int cbCompressedEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpCompressedEntryID, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_CreateGlobalIdFromId(IntPtr iMsgStore, long id, out SafeExLinkedMemoryHandle lppGid);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_CreateIdFromGlobalId(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] gid, out long id);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcMsgStore_MapActionsToMDBActions(IntPtr iMsgStore, _Actions* pactions, out uint cbBuf, out SafeExMemoryHandle pbBuf);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetMailboxInstanceGuid(IntPtr iMsgStore, out Guid guidMailboxInstanceGuid);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetMdbIdMapping(IntPtr iMsgStore, out ushort replidServer, out Guid guidServer);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetSpoolerQueueFid(IntPtr iMsgStore, out long fidSpoolerQ);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetLocalRepIds(IntPtr iMsgStore, uint cid, out MapiLtidNative ltid);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_CreatePublicEntryId(IntPtr iMsgStore, long fid, long mid, [MarshalAs(UnmanagedType.Bool)] bool fMessage, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetTransportQueueFolderId(IntPtr iMsgStore, out long fidTransportQueue);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetIsRulesInterfaceAvailable(IntPtr iMsgStore, [MarshalAs(UnmanagedType.Bool)] out bool rulesInterfaceAvailable);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_SetSpooler(IntPtr iMsgStore);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_SpoolerSetMessageLockState(IntPtr iMsgStore, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, int ulLockState);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_SpoolerNotifyMessageNewMail(IntPtr iMsgStore, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.LPStr)] string lpszMsgClass, int ulMessageFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetPerUserGuid(IntPtr iMsgStore, MapiLtidNative ltid, out Guid guid);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetPerUserLtids(IntPtr iMsgStore, [In] Guid guid, out int lpcLtids, out SafeExMemoryHandle lppLtids);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetAllPerUserLtids(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] startLtid, out int lpcpud, out SafeExMemoryHandle lppPud, out bool isFinished);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_UpdateDeferredActionMessages(IntPtr iMsgStore, int cbServerEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpServerEntryId, int cbClientEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpClientEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetEffectiveRights(IntPtr iMsgStore, int cbAddressBookEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpAddressBookEntryId, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, out uint rights);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcMsgStore_PrereadMessages(IntPtr iMsgStore, _SBinaryArray* sbaEntryIDs);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_AdviseEx(IntPtr iMsgStore, int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, AdviseFlags ulEventMask, IntPtr iOnNotifyDelegate, ulong callbackId, out IntPtr piConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_SetCurrentActivityInfo(IntPtr iMsgStore, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpGuidActivityId, [MarshalAs(UnmanagedType.LPStr)] [In] string lpszComponent, [MarshalAs(UnmanagedType.LPStr)] [In] string lpszProtocol, [MarshalAs(UnmanagedType.LPStr)] [In] string lpszAction);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMsgStore_GetInTransitStatus(IntPtr iMsgStore, out uint ulInTransitStatus);
	}
}
