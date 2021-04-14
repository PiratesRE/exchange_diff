using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal class OwaServiceUriComparer : IComparer<OwaService>
	{
		public int Compare(OwaService x, OwaService y)
		{
			int num = Uri.Compare(x.Url, y.Url, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);
			if (num == 0)
			{
				return x.AuthenticationMethod - y.AuthenticationMethod;
			}
			return num;
		}
	}
}
