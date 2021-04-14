using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public abstract class ClientViewState
	{
		public abstract PreFormActionResponse ToPreFormActionResponse();

		public string ToQueryString()
		{
			return "?" + this.ToPreFormActionResponse().GetUrl();
		}
	}
}
