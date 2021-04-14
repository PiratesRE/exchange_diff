using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class DefaultClientViewState : ClientViewState
	{
		public override PreFormActionResponse ToPreFormActionResponse()
		{
			return new PreFormActionResponse
			{
				ApplicationElement = ApplicationElement.StartPage
			};
		}
	}
}
