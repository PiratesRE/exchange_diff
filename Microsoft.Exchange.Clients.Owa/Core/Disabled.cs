using System;
using System.Net;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class Disabled : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			Utilities.EndResponse(this.Context, HttpStatusCode.Forbidden);
		}
	}
}
