using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiMessage : IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		int GetAttachmentTable(int ulFlags, out IExMapiTable iMAPITable);

		int OpenAttach(int attachmentNumber, Guid lpInterface, int ulFlags, out IExMapiAttach iAttach);

		int CreateAttach(Guid lpInterface, int ulFlags, out int attachmentNumber, out IExMapiAttach iAttach);

		int DeleteAttach(int attachmentNumber, IntPtr ulUiParam, IntPtr lpProgress, int ulFlags);

		int GetRecipientTable(int ulFlags, out IExMapiTable iMAPITable);

		int ModifyRecipients(int ulFlags, AdrEntry[] padrList);

		int SubmitMessage(int ulFlags);

		int SetReadFlag(int ulFlags);

		int Deliver(int ulFlags);

		int DoneWithMessage();

		int DuplicateDeliveryCheck(string internetMessageId, long submitTimeAsLong);

		int TransportSendMessage(out PropValue[] lppPropArray);

		int SubmitMessageEx(int ulFlags);
	}
}
