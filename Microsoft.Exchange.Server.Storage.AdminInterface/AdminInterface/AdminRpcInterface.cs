using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public static class AdminRpcInterface
	{
		public static void StartAcceptingCalls()
		{
			using (LockManager.Lock(AdminRpcInterface.lockName, LockManager.LockType.AdminRpcInterfaceExclusive))
			{
				AdminRpcInterface.state = AdminRpcInterface.InterfaceState.Enabled;
			}
		}

		public static void StopAcceptingCalls()
		{
			using (LockManager.Lock(AdminRpcInterface.lockName, LockManager.LockType.AdminRpcInterfaceExclusive))
			{
				AdminRpcInterface.state = AdminRpcInterface.InterfaceState.Stopped;
			}
		}

		public static ErrorCode EcEnterRpcCall()
		{
			LockManager.GetLock(AdminRpcInterface.lockName, LockManager.LockType.AdminRpcInterfaceShared);
			ErrorCode errorCode;
			switch (AdminRpcInterface.state)
			{
			case AdminRpcInterface.InterfaceState.NotYetEnabled:
				errorCode = ErrorCode.CreateNotInitialized((LID)56984U);
				goto IL_50;
			case AdminRpcInterface.InterfaceState.Stopped:
				errorCode = ErrorCode.CreateExiting((LID)40600U);
				goto IL_50;
			}
			errorCode = ErrorCode.NoError;
			IL_50:
			if (errorCode != ErrorCode.NoError)
			{
				LockManager.ReleaseLock(AdminRpcInterface.lockName, LockManager.LockType.AdminRpcInterfaceShared);
			}
			return errorCode;
		}

		public static void ExitRpcCall()
		{
			LockManager.ReleaseLock(AdminRpcInterface.lockName, LockManager.LockType.AdminRpcInterfaceShared);
		}

		private static readonly LockName<Guid> lockName = new LockName<Guid>(Guid.NewGuid(), LockManager.LockLevel.AdminRpcInterface);

		private static AdminRpcInterface.InterfaceState state = AdminRpcInterface.InterfaceState.NotYetEnabled;

		private enum InterfaceState
		{
			NotYetEnabled,
			Enabled,
			Stopped
		}
	}
}
