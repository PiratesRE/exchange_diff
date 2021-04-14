using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class SizeExtension
	{
		public static Padding Scale(this Padding padding, SizeF factor)
		{
			return new Padding((int)((float)padding.Left * factor.Width), (int)((float)padding.Top * factor.Height), (int)((float)padding.Right * factor.Width), (int)((float)padding.Bottom * factor.Height));
		}
	}
}
