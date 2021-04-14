using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FxProxyMessageWrapper : FxProxyWrapper, IMessage, IMAPIProp
	{
		internal FxProxyMessageWrapper(IMapiFxCollector iFxCollector) : base(iFxCollector)
		{
		}

		public int GetAttachmentTable(int ulFlags, out IMAPITable iMAPITable)
		{
			iMAPITable = null;
			return -2147221246;
		}

		public int OpenAttach(int attachmentNumber, Guid lpInterface, int ulFlags, out IAttach iAttach)
		{
			iAttach = null;
			return -2147221246;
		}

		public int CreateAttach(Guid lpInterface, int ulFlags, out int attachmentNumber, out IAttach iAttach)
		{
			attachmentNumber = 0;
			iAttach = null;
			return -2147221246;
		}

		public int DeleteAttach(int attachmentNumber, IntPtr ulUiParam, IntPtr lpProgress, int ulFlags)
		{
			return -2147221246;
		}

		public int GetRecipientTable(int ulFlags, out IMAPITable iMAPITable)
		{
			iMAPITable = null;
			return -2147221246;
		}

		public unsafe int ModifyRecipients(int ulFlags, _AdrList* padrList)
		{
			return -2147221246;
		}

		public int SubmitMessage(int ulFlags)
		{
			return -2147221246;
		}

		public int SetReadFlag(int ulFlags)
		{
			return -2147221246;
		}
	}
}
