using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IMailboxLockName : ILockName, IEquatable<ILockName>, IComparable<ILockName>
	{
		Guid DatabaseGuid { get; }

		int MailboxPartitionNumber { get; }
	}
}
