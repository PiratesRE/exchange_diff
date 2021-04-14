using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	internal struct StoreTransactionOperation
	{
		[MarshalAs(UnmanagedType.U4)]
		public StoreTransactionOperationType Operation;

		public StoreTransactionData Data;
	}
}
