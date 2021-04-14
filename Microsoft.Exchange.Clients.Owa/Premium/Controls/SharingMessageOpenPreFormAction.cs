using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class SharingMessageOpenPreFormAction : SharingMessagePreFormAction
	{
		protected override string Action
		{
			get
			{
				return "Open";
			}
		}
	}
}
