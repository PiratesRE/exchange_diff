using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SegmentOperationResult
	{
		internal static readonly OperationResult NeutralOperationResult;

		internal OperationResult OperationResult;

		internal LocalizedException Exception;

		internal int CompletedWork;

		internal bool IsCompleted;
	}
}
