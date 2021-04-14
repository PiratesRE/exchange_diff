using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ExchangePictureBox : PictureBox
	{
		public ExchangePictureBox()
		{
			base.Name = "ExchangePictureBox";
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			if (specified == BoundsSpecified.All)
			{
				factor.Height = Math.Min(factor.Height, factor.Width);
				factor.Width = factor.Height;
			}
			base.ScaleControl(factor, specified);
		}
	}
}
