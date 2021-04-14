using System;

namespace Microsoft.Exchange.Data.ImageAnalysis
{
	public class RegionRect
	{
		public RegionRect()
		{
			this.Left = 0;
			this.Top = 0;
			this.Right = 0;
			this.Bottom = 0;
		}

		public RegionRect(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}

		public int Left { get; set; }

		public int Top { get; set; }

		public int Right { get; set; }

		public int Bottom { get; set; }

		public int Height
		{
			get
			{
				return this.Bottom - this.Top;
			}
		}

		public int Width
		{
			get
			{
				return this.Right - this.Left;
			}
		}
	}
}
