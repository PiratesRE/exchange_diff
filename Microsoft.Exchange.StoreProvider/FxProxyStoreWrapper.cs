using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FxProxyStoreWrapper : FxProxyWrapper, IMsgStore, IMAPIProp
	{
		internal FxProxyStoreWrapper(IMapiFxCollector iFxCollector) : base(iFxCollector)
		{
		}

		public int Advise(int cbEntryID, byte[] lpEntryId, AdviseFlags ulEventMask, IMAPIAdviseSink lpAdviseSink, out IntPtr piConnection)
		{
			piConnection = IntPtr.Zero;
			return -2147221246;
		}

		public int Unadvise(IntPtr iConnection)
		{
			return -2147221246;
		}

		public int Slot10()
		{
			return -2147221246;
		}

		public int OpenEntry(int cbEntryID, byte[] lpEntryID, Guid lpInterface, int ulFlags, out int lpulObjType, out object obj)
		{
			lpulObjType = 0;
			obj = null;
			return -2147221246;
		}

		public int SetReceiveFolder(string lpwszMessageClass, int ulFlags, int cbEntryId, byte[] lpEntryID)
		{
			return -2147221246;
		}

		public int GetReceiveFolder(string lpwszMessageClass, int ulFlags, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId, out SafeExLinkedMemoryHandle lppszExplicitClass)
		{
			lpcbEntryId = 0;
			lppEntryId = null;
			lppszExplicitClass = null;
			return -2147221246;
		}

		public int Slot14()
		{
			return -2147221246;
		}

		public int StoreLogoff(ref int ulFlags)
		{
			return -2147221246;
		}

		public int AbortSubmit(int cbEntryID, byte[] lpEntryID, int ulFlags)
		{
			return -2147221246;
		}

		public int Slot17()
		{
			return -2147221246;
		}

		public int Slot18()
		{
			return -2147221246;
		}

		public int Slot19()
		{
			return -2147221246;
		}

		public int Slot1a()
		{
			return -2147221246;
		}
	}
}
