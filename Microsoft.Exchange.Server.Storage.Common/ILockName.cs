using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface ILockName : IEquatable<ILockName>, IComparable<ILockName>
	{
		LockManager.LockLevel LockLevel { get; }

		LockManager.NamedLockObject CachedLockObject { get; set; }

		ILockName GetLockNameToCache();
	}
}
