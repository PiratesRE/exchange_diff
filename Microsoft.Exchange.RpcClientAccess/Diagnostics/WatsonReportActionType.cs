using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal enum WatsonReportActionType
	{
		[Obsolete("Invalid. Use any other type.")]
		None,
		Connection,
		IcsDownload,
		MessageAdaptor,
		FolderAdaptor,
		FastTransferState
	}
}
