using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class ClientResultWrapper<T> : IClientResult<T>
	{
		public T Value
		{
			get
			{
				return this.backingClientResult.Value;
			}
		}

		public ClientResultWrapper(ClientResult<T> result)
		{
			this.backingClientResult = result;
		}

		private ClientResult<T> backingClientResult;
	}
}
