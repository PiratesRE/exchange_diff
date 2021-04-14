using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IClientResult<T>
	{
		T Value { get; }
	}
}
