using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	internal abstract class MailboxSearchRpcServer : RpcServerBase
	{
		public abstract SearchErrorInfo Start(SearchId searchId, Guid ownerGuid);

		public abstract SearchErrorInfo GetStatus(SearchId searchId, out SearchStatus searchStatus);

		public abstract SearchErrorInfo Abort(SearchId searchId, Guid userGuid);

		public abstract SearchErrorInfo Remove(SearchId searchId, [MarshalAs(UnmanagedType.U1)] bool removeLogs);

		public abstract SearchErrorInfo StartEx(SearchId searchId, string ownerId);

		public abstract SearchErrorInfo AbortEx(SearchId searchId, string userId);

		public abstract SearchErrorInfo UpdateStatus(SearchId searchId);

		public MailboxSearchRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IMailboxSearch_v1_0_s_ifspec;
	}
}
