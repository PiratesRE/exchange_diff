using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExMapiMessageHandle : SafeExMapiPropHandle, IExMapiMessage, IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiMessageHandle()
		{
		}

		internal SafeExMapiMessageHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiMessageHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiMessageHandle>(this);
		}

		public int GetAttachmentTable(int ulFlags, out IExMapiTable iMAPITable)
		{
			SafeExMapiTableHandle safeExMapiTableHandle = null;
			int result = SafeExMapiMessageHandle.IMessage_GetAttachmentTable(this.handle, ulFlags, out safeExMapiTableHandle);
			iMAPITable = safeExMapiTableHandle;
			return result;
		}

		public int OpenAttach(int attachmentNumber, Guid lpInterface, int ulFlags, out IExMapiAttach iAttach)
		{
			SafeExMapiAttachHandle safeExMapiAttachHandle = null;
			int result = SafeExMapiMessageHandle.IMessage_OpenAttach(this.handle, attachmentNumber, lpInterface, ulFlags, out safeExMapiAttachHandle);
			iAttach = safeExMapiAttachHandle;
			return result;
		}

		public int CreateAttach(Guid lpInterface, int ulFlags, out int attachmentNumber, out IExMapiAttach iAttach)
		{
			SafeExMapiAttachHandle safeExMapiAttachHandle = null;
			int result = SafeExMapiMessageHandle.IMessage_CreateAttach(this.handle, lpInterface, ulFlags, out attachmentNumber, out safeExMapiAttachHandle);
			iAttach = safeExMapiAttachHandle;
			return result;
		}

		public int DeleteAttach(int attachmentNumber, IntPtr ulUiParam, IntPtr lpProgress, int ulFlags)
		{
			return SafeExMapiMessageHandle.IMessage_DeleteAttach(this.handle, attachmentNumber, ulUiParam, lpProgress, ulFlags);
		}

		public int GetRecipientTable(int ulFlags, out IExMapiTable iMAPITable)
		{
			SafeExMapiTableHandle safeExMapiTableHandle = null;
			int result = SafeExMapiMessageHandle.IMessage_GetRecipientTable(this.handle, ulFlags, out safeExMapiTableHandle);
			iMAPITable = safeExMapiTableHandle;
			return result;
		}

		public int ModifyRecipients(int ulFlags, AdrEntry[] padrList)
		{
			return this.InternalModifyRecipients(ulFlags, padrList);
		}

		private unsafe int InternalModifyRecipients(int ulFlags, AdrEntry[] padrList)
		{
			int bytesToMarshal = AdrList.GetBytesToMarshal(padrList);
			fixed (byte* ptr = new byte[bytesToMarshal])
			{
				AdrList.MarshalToNative(ptr, padrList);
				return SafeExMapiMessageHandle.IMessage_ModifyRecipients(this.handle, ulFlags, (_AdrList*)ptr);
			}
		}

		public int SubmitMessage(int ulFlags)
		{
			return SafeExMapiMessageHandle.IMessage_SubmitMessage(this.handle, ulFlags);
		}

		public int SetReadFlag(int ulFlags)
		{
			return SafeExMapiMessageHandle.IMessage_SetReadFlag(this.handle, ulFlags);
		}

		public int Deliver(int ulFlags)
		{
			return SafeExMapiMessageHandle.IExRpcMessage_Deliver(this.handle, ulFlags);
		}

		public int DoneWithMessage()
		{
			return SafeExMapiMessageHandle.IExRpcMessage_DoneWithMessage(this.handle);
		}

		public int DuplicateDeliveryCheck(string internetMessageId, long submitTimeAsLong)
		{
			return SafeExMapiMessageHandle.IExRpcMessage_DuplicateDeliveryCheck(this.handle, internetMessageId, submitTimeAsLong);
		}

		public int TransportSendMessage(out PropValue[] lppPropArray)
		{
			return this.InternalTransportSendMessage(out lppPropArray);
		}

		private unsafe int InternalTransportSendMessage(out PropValue[] lppPropArray)
		{
			lppPropArray = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = 0;
				int num2 = SafeExMapiMessageHandle.IExRpcMessage_TransportSendMessage(this.handle, out num, out safeExLinkedMemoryHandle);
				if (num2 == 0)
				{
					PropValue[] array = new PropValue[num];
					SPropValue* ptr = (SPropValue*)safeExLinkedMemoryHandle.DangerousGetHandle().ToPointer();
					for (int i = 0; i < num; i++)
					{
						array[i] = new PropValue(ptr + i);
					}
					lppPropArray = array;
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

		public int SubmitMessageEx(int ulFlags)
		{
			return SafeExMapiMessageHandle.IExRpcMessage_SubmitMessageEx(this.handle, ulFlags);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMessage_GetAttachmentTable(IntPtr iMessage, int ulFlags, out SafeExMapiTableHandle iMAPITable);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMessage_OpenAttach(IntPtr iMessage, int attachmentNumber, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int ulFlags, out SafeExMapiAttachHandle iAttach);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMessage_CreateAttach(IntPtr iMessage, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int ulFlags, out int attachmentNumber, out SafeExMapiAttachHandle iAttach);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMessage_DeleteAttach(IntPtr iMessage, int attachmentNumber, IntPtr ulUiParam, IntPtr lpProgress, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMessage_GetRecipientTable(IntPtr iMessage, int ulFlags, out SafeExMapiTableHandle iMAPITable);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMessage_ModifyRecipients(IntPtr iMessage, int ulFlags, _AdrList* padrList);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMessage_SubmitMessage(IntPtr iMessage, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMessage_SetReadFlag(IntPtr iMessage, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMessage_Deliver(IntPtr iMessage, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMessage_DoneWithMessage(IntPtr iMessage);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMessage_DuplicateDeliveryCheck(IntPtr iMessage, [MarshalAs(UnmanagedType.LPStr)] string lpszInetMessageId, long submitTimeAsLong);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMessage_TransportSendMessage(IntPtr iMessage, out int lpcValues, out SafeExLinkedMemoryHandle lppPropArray);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcMessage_SubmitMessageEx(IntPtr iMessage, int ulFlags);
	}
}
