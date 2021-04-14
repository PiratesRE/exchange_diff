using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class WebReadyView : OwaPage
	{
		protected override bool HasFrameset
		{
			get
			{
				return true;
			}
		}
	}
}
