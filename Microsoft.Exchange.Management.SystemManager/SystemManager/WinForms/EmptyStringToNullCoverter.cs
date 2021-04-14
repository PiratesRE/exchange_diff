using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class EmptyStringToNullCoverter : TextConverter
	{
		protected override object ParseObject(string s, IFormatProvider provider)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			return base.ParseObject(s, provider);
		}
	}
}
