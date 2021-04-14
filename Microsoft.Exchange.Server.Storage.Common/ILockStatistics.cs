using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface ILockStatistics
	{
		byte GetClientType();

		byte GetOperation();

		void OnAfterLockAcquisition(LockManager.LockType lockType, bool locked, bool contested, ILockStatistics recentOwner, TimeSpan timeSpentWaiting);
	}
}
