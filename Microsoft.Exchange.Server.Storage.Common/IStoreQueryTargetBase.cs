using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IStoreQueryTargetBase<T>
	{
		string Name { get; }

		Type[] ParameterTypes { get; }
	}
}
