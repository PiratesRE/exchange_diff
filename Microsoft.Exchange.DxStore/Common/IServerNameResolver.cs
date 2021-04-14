using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public interface IServerNameResolver
	{
		string ResolveName(string shortServerName);
	}
}
