using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExMapiFolderHandle : SafeExMapiContainerHandle, IExMapiFolder, IExMapiContainer, IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiFolderHandle()
		{
		}

		internal SafeExMapiFolderHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiFolderHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiFolderHandle>(this);
		}

		public int CreateMessage(int ulFlags, out IExMapiMessage iMessage)
		{
			SafeExMapiMessageHandle safeExMapiMessageHandle = null;
			int result = SafeExMapiFolderHandle.IMAPIFolder_CreateMessage(this.handle, IntPtr.Zero, ulFlags, out safeExMapiMessageHandle);
			iMessage = safeExMapiMessageHandle;
			return result;
		}

		public int CopyMessages(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags)
		{
			return this.InternalCopyMessages(sbinArray, IntPtr.Zero, ((SafeExMapiFolderHandle)iMAPIFolderDest).DangerousGetHandle(), IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		private unsafe int InternalCopyMessages(SBinary[] sbinArray, IntPtr lpInterface, IntPtr iMAPIFolderDest, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			int bytesToMarshal = SBinaryArray.GetBytesToMarshal(sbinArray);
			fixed (byte* ptr = new byte[bytesToMarshal])
			{
				SBinaryArray.MarshalToNative(ptr, sbinArray);
				return SafeExMapiFolderHandle.IMAPIFolder_CopyMessages(this.handle, (_SBinaryArray*)ptr, lpInterface, iMAPIFolderDest, ulUIParam, lpProgress, ulFlags);
			}
		}

		public int CopyMessages_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags)
		{
			return this.InternalCopyMessages_External(sbinArray, IntPtr.Zero, iMAPIFolderDest, IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		private unsafe int InternalCopyMessages_External(SBinary[] sbinArray, IntPtr lpInterface, IMAPIFolder iMAPIFolderDest, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			int bytesToMarshal = SBinaryArray.GetBytesToMarshal(sbinArray);
			fixed (byte* ptr = new byte[bytesToMarshal])
			{
				SBinaryArray.MarshalToNative(ptr, sbinArray);
				return SafeExMapiFolderHandle.IMAPIFolder_CopyMessages_External(this.handle, (_SBinaryArray*)ptr, lpInterface, iMAPIFolderDest, ulUIParam, lpProgress, ulFlags);
			}
		}

		public int DeleteMessages(SBinary[] sbinArray, int ulFlags)
		{
			return this.InternalDeleteMessages(sbinArray, IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		private unsafe int InternalDeleteMessages(SBinary[] sbinArray, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags)
		{
			int bytesToMarshal = SBinaryArray.GetBytesToMarshal(sbinArray);
			fixed (byte* ptr = new byte[bytesToMarshal])
			{
				SBinaryArray.MarshalToNative(ptr, sbinArray);
				return SafeExMapiFolderHandle.IMAPIFolder_DeleteMessages(this.handle, (_SBinaryArray*)ptr, ulUIParam, lpProgress, ulFlags);
			}
		}

		public int CreateFolder(int ulFolderType, string lpwszFolderName, string lpwszFolderComment, int ulFlags, out IExMapiFolder iMAPIFolderNew)
		{
			SafeExMapiFolderHandle safeExMapiFolderHandle = null;
			int result = SafeExMapiFolderHandle.IMAPIFolder_CreateFolder(this.handle, ulFolderType, lpwszFolderName, lpwszFolderComment, IntPtr.Zero, ulFlags, out safeExMapiFolderHandle);
			iMAPIFolderNew = safeExMapiFolderHandle;
			return result;
		}

		public int CopyFolder(int cbEntryId, byte[] lpEntryId, IExMapiFolder iMAPIFolderDest, string lpwszNewFolderName, int ulFlags)
		{
			return SafeExMapiFolderHandle.IMAPIFolder_CopyFolder(this.handle, cbEntryId, lpEntryId, IntPtr.Zero, ((SafeExMapiFolderHandle)iMAPIFolderDest).DangerousGetHandle(), lpwszNewFolderName, IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		public int CopyFolder_External(int cbEntryId, byte[] lpEntryId, IMAPIFolder iMAPIFolderDest, string lpwszNewFolderName, int ulFlags)
		{
			return SafeExMapiFolderHandle.IMAPIFolder_CopyFolder_External(this.handle, cbEntryId, lpEntryId, IntPtr.Zero, iMAPIFolderDest, lpwszNewFolderName, IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		public int DeleteFolder(byte[] lpEntryId, int ulFlags)
		{
			return SafeExMapiFolderHandle.IMAPIFolder_DeleteFolder(this.handle, (lpEntryId != null) ? lpEntryId.Length : 0, lpEntryId, IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		public int SetReadFlags(SBinary[] sbinArray, int ulFlags)
		{
			return this.InternalSetReadFlags(sbinArray, ulFlags);
		}

		public unsafe int InternalSetReadFlags(SBinary[] sbinArray, int ulFlags)
		{
			if (sbinArray != null && sbinArray.Length > 0)
			{
				int bytesToMarshal = SBinaryArray.GetBytesToMarshal(sbinArray);
				fixed (byte* ptr = new byte[bytesToMarshal])
				{
					SBinaryArray.MarshalToNative(ptr, sbinArray);
					return SafeExMapiFolderHandle.IMAPIFolder_SetReadFlags(this.handle, (_SBinaryArray*)ptr, IntPtr.Zero, IntPtr.Zero, ulFlags);
				}
			}
			return SafeExMapiFolderHandle.IMAPIFolder_SetReadFlags(this.handle, null, IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		public int GetMessageStatus(byte[] lpEntryId, int ulFlags, out MessageStatus pulMessageStatus)
		{
			return SafeExMapiFolderHandle.IMAPIFolder_GetMessageStatus(this.handle, (lpEntryId != null) ? lpEntryId.Length : 0, lpEntryId, ulFlags, out pulMessageStatus);
		}

		public int SetMessageStatus(byte[] lpEntryId, MessageStatus ulNewStatus, MessageStatus ulNewStatusMask, out MessageStatus pulOldStatus)
		{
			return SafeExMapiFolderHandle.IMAPIFolder_SetMessageStatus(this.handle, (lpEntryId != null) ? lpEntryId.Length : 0, lpEntryId, ulNewStatus, ulNewStatusMask, out pulOldStatus);
		}

		public int EmptyFolder(int ulFlags)
		{
			return SafeExMapiFolderHandle.IMAPIFolder_EmptyFolder(this.handle, IntPtr.Zero, IntPtr.Zero, ulFlags);
		}

		public int IsContentAvailable(out bool isContentAvailable)
		{
			return SafeExMapiFolderHandle.IExRpcFolder_IsContentAvailable(this.handle, out isContentAvailable);
		}

		public int GetReplicaServers(out string[] servers, out uint numberOfCheapServers)
		{
			servers = null;
			numberOfCheapServers = 0U;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				uint num = 0U;
				uint num3;
				int num2 = SafeExMapiFolderHandle.IExRpcFolder_GetReplicaServers(this.handle, out num3, out safeExLinkedMemoryHandle, out num);
				if (num2 == 0)
				{
					numberOfCheapServers = (uint)((ushort)num);
					if (0U < num3 && !safeExLinkedMemoryHandle.IsInvalid)
					{
						string[] array = new string[num3];
						IntPtr intPtr = safeExLinkedMemoryHandle.DangerousGetHandle();
						int num4 = Marshal.SizeOf(typeof(IntPtr));
						int num5 = 0;
						while ((long)num5 < (long)((ulong)num3))
						{
							IntPtr ptr = Marshal.ReadIntPtr(intPtr);
							array[num5] = Marshal.PtrToStringAnsi(ptr);
							intPtr = (IntPtr)((long)intPtr + (long)num4);
							num5++;
						}
						servers = array;
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
			}
			return result;
		}

		public int SetMessageFlags(int cbEntryId, byte[] lpEntryId, uint ulStatus, uint ulMask)
		{
			return SafeExMapiFolderHandle.IExRpcFolder_SetMessageFlags(this.handle, cbEntryId, lpEntryId, ulStatus, ulMask);
		}

		public int CopyMessagesEx(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags, PropValue[] pva)
		{
			return this.InternalCopyMessagesEx(sbinArray, iMAPIFolderDest, ulFlags, pva);
		}

		private unsafe int InternalCopyMessagesEx(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags, PropValue[] pva)
		{
			int num = SBinaryArray.GetBytesToMarshal(sbinArray);
			fixed (byte* ptr = new byte[num])
			{
				SBinaryArray.MarshalToNative(ptr, sbinArray);
				num = 0;
				for (int i = 0; i < pva.Length; i++)
				{
					num += pva[i].GetBytesToMarshal();
				}
				fixed (byte* ptr2 = new byte[num])
				{
					PropValue.MarshalToNative(pva, ptr2);
					return SafeExMapiFolderHandle.IExRpcFolder_CopyMessagesEx(this.handle, (_SBinaryArray*)ptr, ((SafeExMapiFolderHandle)iMAPIFolderDest).DangerousGetHandle(), ulFlags, pva.Length, (SPropValue*)ptr2);
				}
			}
		}

		public int CopyMessagesEx_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, PropValue[] pva)
		{
			return this.InternalCopyMessagesEx_External(sbinArray, iMAPIFolderDest, ulFlags, pva);
		}

		private unsafe int InternalCopyMessagesEx_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, PropValue[] pva)
		{
			int num = SBinaryArray.GetBytesToMarshal(sbinArray);
			fixed (byte* ptr = new byte[num])
			{
				SBinaryArray.MarshalToNative(ptr, sbinArray);
				num = 0;
				for (int i = 0; i < pva.Length; i++)
				{
					num += pva[i].GetBytesToMarshal();
				}
				fixed (byte* ptr2 = new byte[num])
				{
					PropValue.MarshalToNative(pva, ptr2);
					return SafeExMapiFolderHandle.IExRpcFolder_CopyMessagesEx_External(this.handle, (_SBinaryArray*)ptr, iMAPIFolderDest, ulFlags, pva.Length, (SPropValue*)ptr2);
				}
			}
		}

		public int SetPropsConditional(Restriction lpRes, PropValue[] lpPropArray, out PropProblem[] lppProblems)
		{
			return this.InternalSetPropsConditional(lpRes, lpPropArray, out lppProblems);
		}

		private unsafe int InternalSetPropsConditional(Restriction lpRes, PropValue[] lpPropArray, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			int num = lpRes.GetBytesToMarshal();
			byte* ptr = stackalloc byte[(UIntPtr)num];
			SRestriction* ptr2 = (SRestriction*)ptr;
			ptr += (SRestriction.SizeOf + 7 & -8);
			lpRes.MarshalToNative(ptr2, ref ptr);
			for (int i = 0; i < lpPropArray.Length; i++)
			{
				num += lpPropArray[i].GetBytesToMarshal();
			}
			fixed (byte* ptr3 = new byte[num])
			{
				PropValue.MarshalToNative(lpPropArray, ptr3);
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				int result;
				try
				{
					int num2 = SafeExMapiFolderHandle.IExRpcFolder_SetPropsConditional(this.handle, ptr2, lpPropArray.Length, (SPropValue*)ptr3, out safeExLinkedMemoryHandle);
					if (num2 == 0 && !safeExLinkedMemoryHandle.IsInvalid)
					{
						lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
					}
					result = num2;
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
		}

		public int CopyMessagesEID(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags, PropValue[] lpPropArray, out byte[][] lppEntryIds, out byte[][] lppChangeNumbers)
		{
			return this.InternalCopyMessagesEID(sbinArray, iMAPIFolderDest, ulFlags, lpPropArray, out lppEntryIds, out lppChangeNumbers);
		}

		private unsafe int InternalCopyMessagesEID(SBinary[] sbinArray, IExMapiFolder iMAPIFolderDest, int ulFlags, PropValue[] lpPropArray, out byte[][] lppEntryIds, out byte[][] lppChangeNumbers)
		{
			lppEntryIds = null;
			lppChangeNumbers = null;
			int num = SBinaryArray.GetBytesToMarshal(sbinArray);
			fixed (byte* ptr = new byte[num])
			{
				SBinaryArray.MarshalToNative(ptr, sbinArray);
				int num2 = 0;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle2 = null;
				int result;
				try
				{
					if (lpPropArray != null && lpPropArray.Length > 0)
					{
						num = 0;
						for (int i = 0; i < lpPropArray.Length; i++)
						{
							num += lpPropArray[i].GetBytesToMarshal();
						}
						try
						{
							fixed (byte* ptr2 = new byte[num])
							{
								PropValue.MarshalToNative(lpPropArray, ptr2);
								num2 = SafeExMapiFolderHandle.IExRpcFolder_CopyMessagesEID(this.handle, (_SBinaryArray*)ptr, ((SafeExMapiFolderHandle)iMAPIFolderDest).DangerousGetHandle(), ulFlags, lpPropArray.Length, (SPropValue*)ptr2, out safeExLinkedMemoryHandle, out safeExLinkedMemoryHandle2);
								goto IL_E9;
							}
						}
						finally
						{
							byte* ptr2 = null;
						}
					}
					num2 = SafeExMapiFolderHandle.IExRpcFolder_CopyMessagesEID(this.handle, (_SBinaryArray*)ptr, ((SafeExMapiFolderHandle)iMAPIFolderDest).DangerousGetHandle(), ulFlags, 0, null, out safeExLinkedMemoryHandle, out safeExLinkedMemoryHandle2);
					IL_E9:
					if (!safeExLinkedMemoryHandle.IsInvalid)
					{
						lppEntryIds = _SBinaryArray.Unmarshal(safeExLinkedMemoryHandle);
					}
					if (!safeExLinkedMemoryHandle2.IsInvalid)
					{
						lppChangeNumbers = _SBinaryArray.Unmarshal(safeExLinkedMemoryHandle2);
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
		}

		public int CopyMessagesEID_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, PropValue[] lpPropArray, out byte[][] lppEntryIds, out byte[][] lppChangeNumbers)
		{
			return this.InternalCopyMessagesEID_External(sbinArray, iMAPIFolderDest, ulFlags, lpPropArray, out lppEntryIds, out lppChangeNumbers);
		}

		private unsafe int InternalCopyMessagesEID_External(SBinary[] sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, PropValue[] lpPropArray, out byte[][] lppEntryIds, out byte[][] lppChangeNumbers)
		{
			lppEntryIds = null;
			lppChangeNumbers = null;
			int num = SBinaryArray.GetBytesToMarshal(sbinArray);
			fixed (byte* ptr = new byte[num])
			{
				SBinaryArray.MarshalToNative(ptr, sbinArray);
				int num2 = 0;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle2 = null;
				int result;
				try
				{
					if (lpPropArray != null && lpPropArray.Length > 0)
					{
						num = 0;
						for (int i = 0; i < lpPropArray.Length; i++)
						{
							num += lpPropArray[i].GetBytesToMarshal();
						}
						try
						{
							fixed (byte* ptr2 = new byte[num])
							{
								PropValue.MarshalToNative(lpPropArray, ptr2);
								num2 = SafeExMapiFolderHandle.IExRpcFolder_CopyMessagesEID_External(this.handle, (_SBinaryArray*)ptr, iMAPIFolderDest, ulFlags, lpPropArray.Length, (SPropValue*)ptr2, out safeExLinkedMemoryHandle, out safeExLinkedMemoryHandle2);
								goto IL_D2;
							}
						}
						finally
						{
							byte* ptr2 = null;
						}
					}
					num2 = SafeExMapiFolderHandle.IExRpcFolder_CopyMessagesEID_External(this.handle, (_SBinaryArray*)ptr, iMAPIFolderDest, ulFlags, 0, null, out safeExLinkedMemoryHandle, out safeExLinkedMemoryHandle2);
					IL_D2:
					if (!safeExLinkedMemoryHandle.IsInvalid)
					{
						lppEntryIds = _SBinaryArray.Unmarshal(safeExLinkedMemoryHandle);
					}
					if (!safeExLinkedMemoryHandle2.IsInvalid)
					{
						lppChangeNumbers = _SBinaryArray.Unmarshal(safeExLinkedMemoryHandle2);
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
		}

		public int CreateFolderEx(int ulFolderType, string lpwszFolderName, string lpwszFolderComment, byte[] lpEntryId, int ulFlags, out IExMapiFolder iMAPIFolderNew)
		{
			SafeExMapiFolderHandle safeExMapiFolderHandle = null;
			int result = SafeExMapiFolderHandle.IExRpcFolder_CreateFolderEx(this.handle, ulFolderType, lpwszFolderName, lpwszFolderComment, (lpEntryId != null) ? lpEntryId.Length : 0, lpEntryId, IntPtr.Zero, ulFlags, out safeExMapiFolderHandle);
			iMAPIFolderNew = safeExMapiFolderHandle;
			return result;
		}

		public int HrSerializeSRestrictionEx(Restriction prest, out byte[] pbRest)
		{
			return this.InternalHrSerializeSRestrictionEx(prest, out pbRest);
		}

		private unsafe int InternalHrSerializeSRestrictionEx(Restriction prest, out byte[] pbRest)
		{
			pbRest = null;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int result;
			try
			{
				int bytesToMarshal = prest.GetBytesToMarshal();
				byte[] array = new byte[bytesToMarshal];
				try
				{
					fixed (byte* ptr = array)
					{
						SRestriction* ptr2 = (SRestriction*)ptr;
						byte* ptr3 = ptr;
						ptr3 += (SRestriction.SizeOf + 7 & -8);
						prest.MarshalToNative(ptr2, ref ptr3);
						uint num2;
						int num = SafeExMapiFolderHandle.HrSerializeSRestrictionEx(this.handle, ptr2, out safeExMemoryHandle, out num2);
						if (num == 0)
						{
							array = new byte[num2];
							safeExMemoryHandle.CopyTo(array, 0, (int)num2);
							pbRest = array;
						}
						result = num;
					}
				}
				finally
				{
					byte* ptr = null;
				}
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

		public int HrDeserializeSRestrictionEx(byte[] pbRest, out Restriction prest)
		{
			prest = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiFolderHandle.HrDeserializeSRestrictionEx(this.handle, pbRest, (uint)pbRest.Length, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					prest = Restriction.Unmarshal(safeExLinkedMemoryHandle);
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

		public unsafe int HrSerializeActionsEx(RuleAction[] pActions, out byte[] pbActions)
		{
			pbActions = null;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int bytesToMarshal = RuleActions.GetBytesToMarshal(pActions);
			byte[] array = new byte[bytesToMarshal];
			fixed (byte* ptr = &array[0])
			{
				byte* ptr2 = ptr;
				RuleActions.MarshalToNative(ref ptr2, pActions);
			}
			int result;
			try
			{
				try
				{
					fixed (byte* ptr3 = &array[0])
					{
						_Actions* pActions2 = (_Actions*)ptr3;
						uint num2;
						int num = SafeExMapiFolderHandle.HrSerializeActionsEx(this.handle, pActions2, out safeExMemoryHandle, out num2);
						if (num == 0)
						{
							pbActions = new byte[num2];
							safeExMemoryHandle.CopyTo(pbActions, 0, (int)num2);
						}
						result = num;
					}
				}
				finally
				{
					byte* ptr3 = null;
				}
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

		public int HrDeserializeActionsEx(byte[] pbActions, out RuleAction[] pActions)
		{
			pActions = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiFolderHandle.HrDeserializeActionsEx(this.handle, pbActions, (uint)pbActions.Length, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					pActions = RuleActions.Unmarshal(safeExLinkedMemoryHandle.DangerousGetHandle());
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

		public int SetPropsEx(bool trackChanges, ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems)
		{
			return this.InternalSetPropsEx(trackChanges, lpPropArray, out lppProblems);
		}

		private unsafe int InternalSetPropsEx(bool trackChanges, ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			int num = 0;
			foreach (PropValue propValue in lpPropArray)
			{
				num += propValue.GetBytesToMarshal();
			}
			fixed (byte* ptr = new byte[num])
			{
				PropValue.MarshalToNative(lpPropArray, ptr);
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				int result;
				try
				{
					int num2 = SafeExMapiFolderHandle.IExRpcFolder_SetPropsEx(this.handle, trackChanges, lpPropArray.Count, (SPropValue*)ptr, out safeExLinkedMemoryHandle);
					if (!safeExLinkedMemoryHandle.IsInvalid)
					{
						lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
					}
					result = num2;
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
		}

		public int DeletePropsEx(bool trackChanges, ICollection<PropTag> lpPropTags, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			PropTag[] lpPropTagArray = PropTagHelper.SPropTagArray(lpPropTags);
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiFolderHandle.IExRpcFolder_DeletePropsEx(this.handle, trackChanges, lpPropTagArray, out safeExLinkedMemoryHandle);
				if (num == 0 && safeExLinkedMemoryHandle != null)
				{
					lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
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

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_CreateMessage(IntPtr iMAPIFolder, IntPtr lpInterface, int ulFlags, out SafeExMapiMessageHandle iMessage);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIFolder_CopyMessages(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IntPtr lpInterface, IntPtr iMAPIFolderDest, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIFolder_CopyMessages_External(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IntPtr lpInterface, IMAPIFolder iMAPIFolderDest, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIFolder_DeleteMessages(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_CreateFolder(IntPtr iMAPIFolder, int ulFolderType, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderName, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderComment, IntPtr lpInterface, int ulFlags, out SafeExMapiFolderHandle iMAPIFolderNew);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_CopyFolder(IntPtr iMAPIFolder, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, IntPtr lpInterface, IntPtr iMAPIFolderDest, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszNewFolderName, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_CopyFolder_External(IntPtr iMAPIFolder, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, IntPtr lpInterface, IMAPIFolder iMAPIFolderDest, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszNewFolderName, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_DeleteFolder(IntPtr iMAPIFolder, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIFolder_SetReadFlags(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_GetMessageStatus(IntPtr iMAPIFolder, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, int ulFlags, out MessageStatus pulMessageStatus);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_SetMessageStatus(IntPtr iMAPIFolder, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, MessageStatus ulNewStatus, MessageStatus ulNewStatusMask, out MessageStatus pulOldStatus);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIFolder_EmptyFolder(IntPtr iMAPIFolder, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcFolder_IsContentAvailable(IntPtr iMAPIFolder, [MarshalAs(UnmanagedType.Bool)] out bool isContentAvailable);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcFolder_GetReplicaServers(IntPtr iMAPIFolder, out uint numberOfServers, out SafeExLinkedMemoryHandle servers, out uint numberOfCheapServers);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcFolder_SetMessageFlags(IntPtr iMAPIFolder, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, uint ulStatus, uint ulMask);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcFolder_CopyMessagesEx(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IntPtr iMAPIFolderDest, int ulFlags, int cValues, SPropValue* lpPropArray);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcFolder_CopyMessagesEx_External(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, int cValues, SPropValue* lpPropArray);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcFolder_SetPropsConditional(IntPtr iMAPIFolder, [In] SRestriction* lpRes, int cValues, SPropValue* lpPropArray, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcFolder_CopyMessagesEID(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IntPtr iMAPIFolderDest, int ulFlags, int cValues, SPropValue* lpPropArray, out SafeExLinkedMemoryHandle lppEntryIds, out SafeExLinkedMemoryHandle lppChangeNumbers);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcFolder_CopyMessagesEID_External(IntPtr iMAPIFolder, _SBinaryArray* sbinArray, IMAPIFolder iMAPIFolderDest, int ulFlags, int cValues, SPropValue* lpPropArray, out SafeExLinkedMemoryHandle lppEntryIds, out SafeExLinkedMemoryHandle lppChangeNumbers);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcFolder_CreateFolderEx(IntPtr iMAPIFolder, int ulFolderType, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderName, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderComment, int cbEntryId, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] byte[] lpEntryId, IntPtr lpInterface, int ulFlags, out SafeExMapiFolderHandle iMAPIFolderNew);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcFolder_SetPropsEx(IntPtr iMAPIFolder, bool trackChanges, int cValues, SPropValue* lpPropArray, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcFolder_DeletePropsEx(IntPtr iMAPIFolder, bool trackChanges, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int HrSerializeSRestrictionEx(IntPtr iMAPIProp, SRestriction* prest, out SafeExMemoryHandle pbRest, out uint cbRest);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int HrDeserializeSRestrictionEx(IntPtr iMAPIProp, [MarshalAs(UnmanagedType.LPArray)] byte[] pbRest, uint cbRest, out SafeExLinkedMemoryHandle prest);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int HrSerializeActionsEx(IntPtr iMAPIProp, _Actions* pActions, out SafeExMemoryHandle pbActions, out uint cbActions);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int HrDeserializeActionsEx(IntPtr iMAPIProp, [MarshalAs(UnmanagedType.LPArray)] byte[] pbActions, uint cbActions, out SafeExLinkedMemoryHandle pActions);
	}
}
