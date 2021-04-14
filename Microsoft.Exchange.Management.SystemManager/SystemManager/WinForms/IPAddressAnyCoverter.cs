using System;
using System.Net;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class IPAddressAnyCoverter : TextConverter
	{
		protected override object ParseObject(string s, IFormatProvider provider)
		{
			if (IPAddress.Any.ToString().Equals(s))
			{
				return IPAddress.Any;
			}
			return base.ParseObject(s, provider);
		}
	}
}
