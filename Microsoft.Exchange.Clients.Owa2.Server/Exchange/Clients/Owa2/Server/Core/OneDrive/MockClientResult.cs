using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockClientResult<T> : IClientResult<T>
	{
		public T Value { get; set; }
	}
}
